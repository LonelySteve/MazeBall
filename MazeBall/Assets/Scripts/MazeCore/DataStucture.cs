using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MazeCore
{
    #region 枚举类型
    public enum Direction
    {
        /// <summary>
        /// 沿X轴正方向
        /// </summary>
        PositiveX,
        /// <summary>
        /// 沿Y轴正方向
        /// </summary>
        PositiveY,
        /// <summary>
        /// 沿X轴负方向
        /// </summary>
        NegativeX,
        /// <summary>
        /// 沿Y轴正方向
        /// </summary>
        NegativeY
    }

    /// <summary>
    /// 迷宫相对位置表示枚举
    /// </summary>
    public enum Location
    {
        /// <summary>
        /// 左上
        /// </summary>
        LeftUpper,
        /// <summary>
        /// 左中
        /// </summary>
        LeftMiddle,
        /// <summary>
        /// 左下
        /// </summary>
        LeftLower,
        /// <summary>
        /// 中上
        /// </summary>
        CenterUpper,
        /// <summary>
        /// 中中
        /// </summary>
        CenterMiddle,
        /// <summary>
        /// 中下
        /// </summary>
        CenterLower,
        /// <summary>
        /// 右上
        /// </summary>
        RightUpper,
        /// <summary>
        /// 右中
        /// </summary>
        RightMiddle,
        /// <summary>
        /// 右下
        /// </summary>
        RightLower,
    }
    #endregion

    #region 结构体类型
    /// <summary>
    /// 迷宫点
    /// </summary>
    public struct MazePosition
    {
        public int X;
        public int Y;

        public MazePosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// 迷宫尺寸
    /// </summary>
    public struct MazeSize
    {
        public int Width
        {
            set
            {
                if (value < 1)
                {
                    throw new ArgumentNullException();
                }
                width = value;
            }
            get
            {
                return width;
            }
        }
        public int Height
        {
            set
            {
                if (value < 1)
                {
                    throw new ArgumentNullException();
                }
                height = value;
            }
            get
            {
                return height;
            }
        }

        private int width;
        private int height;

        public MazeSize(int width, int height)
        {
            this.width = 0;
            this.height = 0;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 通过相对位置获取相应的迷宫单元绝对位置
        /// </summary>
        /// <param name="location">相对位置</param>
        /// <param name="visitFlag">访问标识，默认为 true</param>
        /// <returns></returns>
        public MazePosition GetPositionByRelativeLocation(Location location)
        {
            MazePosition mazeCellPosition = new MazePosition();
            switch (location)
            {
                case Location.LeftUpper:
                    mazeCellPosition = new MazePosition(0, 0);
                    break;
                case Location.LeftMiddle:
                    mazeCellPosition = new MazePosition(0, height / 2);
                    break;
                case Location.LeftLower:
                    mazeCellPosition = new MazePosition(0, height - 1);
                    break;
                case Location.CenterUpper:
                    mazeCellPosition = new MazePosition(width / 2, 0);
                    break;
                case Location.CenterMiddle:
                    mazeCellPosition = new MazePosition(width / 2, height / 2);
                    break;
                case Location.CenterLower:
                    mazeCellPosition = new MazePosition(width / 2, height - 1);
                    break;
                case Location.RightUpper:
                    mazeCellPosition = new MazePosition(width - 1, 0);
                    break;
                case Location.RightMiddle:
                    mazeCellPosition = new MazePosition(width - 1, height / 2);
                    break;
                case Location.RightLower:
                    mazeCellPosition = new MazePosition(width - 1, height - 1);
                    break;
            }
            return mazeCellPosition;
        }

    }

    /// <summary>
    /// 迷宫的矩形区域
    /// </summary>
    public struct MazeRect
    {
        public MazePosition Position { set; get; }
        public MazeSize Size { set; get; }

        public MazeRect(MazePosition position, MazeSize size)
        {
            Position = position;
            Size = size;
        }
    }

    #endregion

    #region 类
    /// <summary>
    /// 迷宫单元
    /// </summary>
    public class MazeCell
    {

        public MazePosition Position { get; set; }
        /// <summary>
        /// 是否访问了该迷宫单元的标志
        /// </summary>
        public bool HasVisited { get; set; }
        /// <summary>
        /// 周围墙壁的字典
        /// </summary>
        public Dictionary<Direction, MazeWall> SurroundedMazeWalls = new Dictionary<Direction, MazeWall>() {
                { Direction.PositiveX,null},
                { Direction.PositiveY,null},
                { Direction.NegativeX,null},
                { Direction.NegativeY,null}
            };

        /// <summary>
        /// 获取指定方向的迷宫墙
        /// </summary>
        /// <param name="direction">指定方向</param>
        /// <returns></returns>
        public MazeWall this[Direction direction]
        {
            get => SurroundedMazeWalls[direction];
            set
            {
                SurroundedMazeWalls[direction] = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public MazeCell(MazePosition position, bool hasVisited = false)
        {
            Position = position;
            HasVisited = hasVisited;
        }

        /// <summary>
        /// 是否能与指定方向的迷宫单元互通
        /// </summary>
        /// <param name="direction">指定的方向</param>
        /// <returns></returns>
        public bool CanPass(Direction direction)
        {
            return !SurroundedMazeWalls.First((KeyValuePair<Direction, MazeWall> kp) =>
            {
                return kp.Key == direction;
            }).Value.HasWall;
        }
    }

    /// <summary>
    /// 迷宫墙
    /// </summary>
    public class MazeWall
    {
        private bool hasWall;

        /// <summary>
        /// 是否存在墙
        /// </summary>
        public bool HasWall
        {
            get => hasWall;
            set
            {
                if (value)
                {
                    if (!hasWall) // 原来没有墙，需要加上墙
                    {
                        hasWall = value;
                             BuildQueue.Enqueue(this);  
                    }
                }
                else if (hasWall)
                {
                    hasWall = value;
                 
                        BuildQueue.Enqueue(this);
              
                }
                hasWall = value;
            }
        }
        /// <summary>
        /// 必存在的迷宫单元
        /// </summary>
        public MazeCell MainMazeCell { get; set; }
        /// <summary>
        /// 可能不存在的迷宫单元
        /// </summary>
        public MazeCell OtherMazeCell { get; set; }
        /// <summary>
        /// 生成队列
        /// </summary>
        public Queue<MazeWall> BuildQueue { get; private set; }
        /// <summary>
        /// 游戏内本地坐标
        /// </summary>
        public Vector3 LocalPosition
        {
            get;
            protected set;
        }
        /// <summary>
        /// 游戏内本地缩放
        /// </summary>
        public Vector3 LocalScale
        {
            get;
            protected set;
        }
        /// <summary>
        /// 游戏内墙对应的对象
        /// </summary>
        public GameObject LocalObject { get; set; }

        public MazeWall(MazeCell mazeCell, MazeCell otherMazeCell, Vector3 localPosition, Vector3 localScale, Queue<MazeWall> buildQueue, bool hasWall = false)
        {
            MainMazeCell = mazeCell ?? throw new ArgumentNullException(nameof(mazeCell));
            OtherMazeCell = otherMazeCell;
            LocalPosition = localPosition;
            LocalScale = localScale;
            BuildQueue = buildQueue ?? throw new ArgumentNullException(nameof(buildQueue));
            HasWall = hasWall;
        }

        /// <summary>
        /// 获取墙对面的迷宫单元，可能不存在
        /// </summary>
        /// <param name="mazeCell">墙这边的迷宫单元</param>
        /// <returns></returns>
        public MazeCell GetOppositeMazeCell(MazeCell mazeCell)
        {
            if (mazeCell == MainMazeCell)
            {
                return OtherMazeCell;
            }
            else if (mazeCell == OtherMazeCell)
            {
                return MainMazeCell;
            }
            throw new ArgumentException("No matching maze cell found!");
        }

        /// <summary>
        /// 打破墙
        /// </summary>
        public void BreakWall()
        {
            HasWall = false;
        }

        /// <summary>
        /// 建立墙
        /// </summary>
        public void BuildWall()
        {
            HasWall = true;
        }
    }

    /// <summary>
    /// 迷宫区域
    /// </summary>
    public class MazeArea
    {
        public MazeCell[,] MazeCells;
        public MazeRect MazeRect
        {
            get
            {
                return new MazeRect(MazeCells[0, 0].Position, new MazeSize(MazeCells.GetLength(1), MazeCells.GetLength(0)));
            }
        }

        public MazeCell this[int row, int rol]
        {
            get
            {
                return MazeCells[row, rol];
            }
            set
            {
                MazeCells[row, rol] = value;
            }
        }

        public MazeArea(MazeCell[,] mazeCells)
        {
            MazeCells = mazeCells ?? throw new ArgumentNullException(nameof(MazeCells));
        }
    }
    #endregion



}
