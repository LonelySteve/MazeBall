using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.MazeCore
{
    /// <summary>
    /// Trim 迷宫生成算法
    /// </summary>
    public class TrimGenerateMazeAlgoritnm : GenerateMazeAlgoritnm
    {
        /// <summary>
        /// 维护迷宫墙的列表
        /// </summary>
        protected List<MazeWall> mazeWallsList = new List<MazeWall>();
        public TrimGenerateMazeAlgoritnm(IMazeBuilder builder)
            : base(builder)
        {

        }

        public override void GenerateMaze()
        {
            InitMaze();

            while (mazeWallsList.Count > 0)
            {
                var randWall = RandPeekWall();
                if (randWall.OtherMazeCell != null)
                {
                    if ((randWall.MainMazeCell.HasVisited && !randWall.OtherMazeCell.HasVisited) ||
                        (!randWall.MainMazeCell.HasVisited && randWall.OtherMazeCell.HasVisited)
                        )
                    {
                        // 打破墙
                        randWall.BreakWall();
                        randWall.OtherMazeCell.HasVisited = true;
                        // 将对面的那个迷宫单元的墙加入列表
                        mazeWallsList.AddRange(randWall.OtherMazeCell.SurroundedMazeWalls.Select((KeyValuePair<Direction, MazeWall> kp) =>
                        {
                            return kp.Value;
                        }));
                    }
                }
                // 移除墙
                mazeWallsList.Remove(randWall);
            }
        }

        protected override void InitMaze()
        {
            // 封闭所有墙
            TotalMazeCellsVisitor.BuildAllWall();
            // 重置访问标识
            TotalMazeCellsVisitor.ResetAllVisitedFlags();
            //清空列表
            mazeWallsList.Clear();
            // TODO 支持随机选择通路起点
            // 选择起点作为通路起点，把它的相邻的墙加入列表
            mazeWallsList.AddRange(
                TotalMazeCellsVisitor.StartMazeCell.SurroundedMazeWalls.Select(
                    new System.Func<KeyValuePair<Direction, MazeWall>, MazeWall>(
                        (KeyValuePair<Direction, MazeWall> kp) =>
            {
                return kp.Value;
            })));
            // 标记已访问
            TotalMazeCellsVisitor.StartMazeCell.HasVisited = true;
        }

        /// <summary>
        /// 从临时列表中随机挑选一堵墙
        /// </summary>
        /// <returns></returns>
        protected MazeWall RandPeekWall()
        {
            var index = Random.Range(0, mazeWallsList.Count);
            return mazeWallsList[index];
        }
    }
}
