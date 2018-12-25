using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.MazeCore
{
    /// <summary>
    /// 迷宫单元访问器
    /// <para>本类提供了对迷宫区域的一系列快捷访问及操作</para>
    /// </summary>
    public class MazeCellsVisitor
    {
        protected MazeArea mazeArea;
        protected MazeCell startMazeCell;
        protected MazeCell endMazeCell;

        public MazeArea MazeArea { get => mazeArea; set => mazeArea = value; }
        public MazeCell StartMazeCell { get => startMazeCell; set => startMazeCell = value; }
        public MazeCell EndMazeCell { get => endMazeCell; set => endMazeCell = value; }


        public MazeCell this[int row, int col]
        {
            get
            {
                var mazeCell = MazeArea.MazeCells[row, col];
                // 设置已访问
                mazeCell.HasVisited = true;
                return mazeCell;
            }
            set
            {
                MazeArea.MazeCells[row, col] = value;
            }
        }

        public MazeCellsVisitor(MazeArea mazeArea)
        {
            this.mazeArea = mazeArea;
            // 使用相对位置设置开始和结束的迷宫单元
            SetStartMazeCellByRelativeLocation();
            SetEndMazeCellByRelativeLocation();
        }


        /// <summary>
        /// 获取相对位置的迷宫单元
        /// <para>[此操作不会改变迷宫单元的访问状态]</para>
        /// </summary>
        /// <param name="location">相对位置</param>
        /// <returns></returns>
        protected MazeCell GetRelativeLocationMazeCell(Location location)
        {
            var pos = mazeArea.MazeRect.Size.GetPositionByRelativeLocation(location);
            return MazeArea.MazeCells[pos.X, pos.Y];
        }

        /// <summary>
        /// 重置所有访问标志
        /// </summary>
        /// <param name="visitFlag">统一重置到的标识，默认为 false</param>
        public void ResetAllVisitedFlags(bool visitFlag = false)
        {
            for (int y = 0; y < MazeArea.MazeRect.Size.Height; y++)
            {
                for (int x = 0; x < MazeArea.MazeRect.Size.Width; x++)
                {
                    MazeArea.MazeCells[y, x].HasVisited = visitFlag;
                }
            }
        }

        /// <summary>
        /// 通过相对位置设置开始迷宫单元
        /// <para>[此操作不会改变迷宫单元的访问状态]</para>
        /// </summary>
        /// <param name="startMazeCellLocation">开始迷宫单元的相对位置</param>
        public void SetStartMazeCellByRelativeLocation(Location startMazeCellLocation = Location.LeftUpper)
        {
            StartMazeCell = GetRelativeLocationMazeCell(startMazeCellLocation);
        }

        /// <summary>
        /// 通过相对位置设置结束迷宫单元
        /// <para>[此操作不会改变迷宫单元的访问状态]</para>
        /// </summary>
        /// <param name="startMazeCellLocation">结束迷宫单元的相对位置</param>
        public void SetEndMazeCellByRelativeLocation(Location endMazeCellLocation = Location.RightLower)
        {
            EndMazeCell = GetRelativeLocationMazeCell(endMazeCellLocation);
        }

        /// <summary>
        /// 打破所有墙
        /// <para>[此操作不会改变迷宫单元的访问状态]</para>
        /// </summary>
        public void BreakAllWall()
        {
            // TODO 更改打破墙的算法，提高效率
            foreach (var cell in mazeArea.MazeCells)
            {
                foreach (var kp in cell.SurroundedMazeWalls)
                {
                    kp.Value.BreakWall();
                }
            }
        }

        /// <summary>
        /// 建立所有墙
        /// <para>[此操作不会改变迷宫单元的访问状态]</para>
        /// </summary>
        public void BuildAllWall()
        {
            // TODO 更改建立墙的算法，提高效率
            foreach (var cell in mazeArea.MazeCells)
            {
                foreach (var kp in cell.SurroundedMazeWalls)
                {
                    kp.Value.BuildWall();
                }
            }
        }

        /// <summary>
        /// 打破迷宫单元一个或多个指定方向的墙
        /// <para>[指定迷宫单元的访问状态将成为 True]</para>
        /// </summary>
        /// <param name="mazePosition">迷宫单元位置</param>
        /// <param name="directions">一个或多个指定方向</param>
        public void BreakOneWall(MazePosition mazePosition, params Direction[] directions)
        {
            foreach (var direction in directions)
            {
                this[mazePosition.X, mazePosition.Y].SurroundedMazeWalls[direction].HasWall = false;
            }
        }

        /// <summary>
        /// 建立迷宫单元一个或多个指定方向的墙
        /// <para>[指定迷宫单元的访问状态将成为 True]</para>
        /// </summary>
        /// <param name="mazePosition">迷宫单元位置</param>
        /// <param name="directions">一个或多个指定方向</param>
        public void BuildOneWall(MazePosition mazePosition, params Direction[] directions)
        {
            foreach (var direction in directions)
            {
                this[mazePosition.X, mazePosition.Y].SurroundedMazeWalls[direction].HasWall = true;
            }
        }
    }
}
