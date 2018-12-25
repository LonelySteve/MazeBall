using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.MazeCore
{
    /// <summary>
    /// 迷宫生成算法
    /// <para>[抽象类]</para>
    /// </summary>
    public abstract class GenerateMazeAlgoritnm
    {
        public IMazeBuilder Builder
        {
            get => builder;
            set
            {
                builder = value ?? throw new System.ArgumentNullException();
            }
        }
        protected MazeCellsVisitor totalMazeCellsVisitor;
        protected GameObject groundGameObj;
        protected IMazeBuilder builder;
        private MazeArea totalMazeArea;

        public GenerateMazeAlgoritnm(IMazeBuilder builder)
        {
            Builder = builder;
            // 最先建造地面
            groundGameObj = builder.BuildGround();
            // 临时处理地面旋转问题
            // TODO 支持地面预旋转
            if (groundGameObj.transform.localEulerAngles != new Vector3())
            {
                throw new System.ArgumentNullException(nameof(builder));
            }
            totalMazeArea = new MazeArea(new MazeCell[builder.MazeBuildSize.Height, builder.MazeBuildSize.Width]);

            // 遍历实例化迷宫单元
            for (int y = 0; y < Builder.MazeBuildSize.Height; y++)
            {
                for (int x = 0; x < Builder.MazeBuildSize.Width; x++)
                {
                    totalMazeArea.MazeCells[y, x] = new MazeCell(new MazePosition(x, y));
                }
            }

            // 遍历建立每个迷宫单元的墙壁
            for (int y = 0; y < totalMazeArea.MazeRect.Size.Height; y++)
            {
                for (int x = 0; x < totalMazeArea.MazeRect.Size.Width; x++)
                {
                    // 沿 x 轴正向
                    MazeWall wall;
                    var pos = new MazePosition(x, y);

                    // 沿 x 轴负向
                    // 可以认为，除了 x == 0 , 其余的迷宫单元这个方向的墙将会被相对的迷宫单元确定
                    if (x == 0)
                    {
                        (var p3, var s3) = TranslateMazeWallPosition(pos, Direction.NegativeX);
                        wall = new MazeWall(totalMazeArea[y, x], null, p3, s3, builder.BuildWallsQueue);
                        totalMazeArea[y, x][Direction.NegativeX] = wall;
                    }
                    if (x + 1 < totalMazeArea.MazeRect.Size.Width)
                    {
                        (var p1, var s1) = TranslateMazeWallPosition(pos, Direction.PositiveX);
                        wall = new MazeWall(totalMazeArea[y, x], totalMazeArea[y, x + 1], p1, s1, builder.BuildWallsQueue);
                        // 给对面的迷宫单元（如果有）的相反方向设置该墙
                        var x_oppoMazeCell = wall.GetOppositeMazeCell(totalMazeArea[y, x]);
                        x_oppoMazeCell[Direction.NegativeX] = wall;
                    }
                    else
                    {
                        (var p2, var s2) = TranslateMazeWallPosition(pos, Direction.PositiveX);
                        wall = new MazeWall(totalMazeArea[y, x], null, p2, s2, builder.BuildWallsQueue);
                    }
                    totalMazeArea[y, x][Direction.PositiveX] = wall;
                    
                    // 沿 y 轴负向
                    // 可以认为，除了 y == 0 , 其余的迷宫单元这个方向的墙将会被相对的迷宫单元确定
                    if (y == 0)
                    {
                        (var p6, var s6) = TranslateMazeWallPosition(pos, Direction.NegativeY);
                        wall = new MazeWall(totalMazeArea[y, x], null, p6, s6, builder.BuildWallsQueue);
                        totalMazeArea[y, x][Direction.NegativeY] = wall;
                    }
               
                    // 沿 y 轴正向
                    if (y + 1 < totalMazeArea.MazeRect.Size.Height)
                    {
                        (var p4, var s4) = TranslateMazeWallPosition(pos, Direction.PositiveY);
                        wall = new MazeWall(totalMazeArea[y, x], totalMazeArea[y + 1, x], p4, s4, builder.BuildWallsQueue);
                        // 给对面的迷宫单元（如果有）的相反方向设置该墙
                        var y_oppoMazeCell = wall.GetOppositeMazeCell(totalMazeArea[y, x]);
                        y_oppoMazeCell[Direction.NegativeY] = wall;
                    }
                    else
                    {
                        (var p5, var s5) = TranslateMazeWallPosition(pos, Direction.PositiveY);
                        wall = new MazeWall(totalMazeArea[y, x], null, p5, s5, builder.BuildWallsQueue);
                    }
                    totalMazeArea[y, x][Direction.PositiveY] = wall;
                }
            }

            totalMazeCellsVisitor = new MazeCellsVisitor(totalMazeArea);
            // 初始化迷宫
            InitMaze();
        }
        /// <summary>
        /// 生成迷宫
        /// <para>子类应该在重写该方法实现对迷宫模型的生成工作</para>
        /// </summary>
        /// <returns></returns>
        public virtual void GenerateMaze()
        {
            // 初始化迷宫
            InitMaze();
        }
        /// <summary>
        /// 初始化迷宫
        /// <para>子类应该在重写该方法实现对迷宫模型的初始化工作</para>
        /// </summary>
        protected abstract void InitMaze();


        /// <summary>
        /// 将迷宫位置翻译成指定方向迷宫墙的游戏内本地坐标
        /// </summary>
        /// <param name="mazePosition">迷宫位置</param>
        /// <param name="direction">指定方向</param>
        /// <returns></returns>
        protected (Vector3, Vector3) TranslateMazeWallPosition(MazePosition mazePosition, Direction direction)
        {
            var groundPosition = groundGameObj.transform.localPosition;
            var groundScale = groundGameObj.transform.localScale;

            float unit_x = groundScale.x / totalMazeArea.MazeRect.Size.Width;
            float unit_z = groundScale.z / totalMazeArea.MazeRect.Size.Height;

            float half_width = totalMazeArea.MazeRect.Size.Width / 2.0f;
            float half_height = totalMazeArea.MazeRect.Size.Height / 2.0f;

            float left_uppper_local_x = unit_x * (mazePosition.X - half_width);
            float left_uppper_local_z = unit_z * (half_height - mazePosition.Y);

            float center_middle_local_x = left_uppper_local_x + unit_x / 2;
            float center_middle_local_z = left_uppper_local_z - unit_z / 2;

            float right_lower_local_x = left_uppper_local_x + unit_x;
            float right_lower_local_z = left_uppper_local_z - unit_z;

            float local_y = groundScale.y / 2 + Builder.MazeWallHeight / 2;

            switch (direction)
            {
                case Direction.PositiveX:
                    return (new Vector3(right_lower_local_x, local_y, center_middle_local_z), new Vector3(Builder.MazeWallThick, Builder.MazeWallHeight, unit_z));
                case Direction.PositiveY:
                    return (new Vector3(center_middle_local_x, local_y, right_lower_local_z), new Vector3(unit_x, Builder.MazeWallHeight, Builder.MazeWallThick));
                case Direction.NegativeX:
                    return (new Vector3(left_uppper_local_x, local_y, center_middle_local_z), new Vector3(Builder.MazeWallThick, Builder.MazeWallHeight, unit_z));
                case Direction.NegativeY:
                    return (new Vector3(center_middle_local_x, local_y, left_uppper_local_z), new Vector3(unit_x, Builder.MazeWallHeight, Builder.MazeWallThick));
            }
            return (new Vector3(), new Vector3());
        }
    }
}
