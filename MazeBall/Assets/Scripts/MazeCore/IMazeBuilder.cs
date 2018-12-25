using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.MazeCore
{
    public interface IMazeBuilder
    {
        /// <summary>
        /// 迷宫建造的尺寸
        /// </summary>
        MazeSize MazeBuildSize { get; }
        /// <summary>
        /// 迷宫开始单元位置
        /// </summary>
        MazePosition MazeStartUnitPosition { get; }
        /// <summary>
        /// 迷宫结束单元位置
        /// </summary>
        MazePosition MazeEndUnitPosition { get; }
        /// <summary>
        /// 迷宫墙的厚度
        /// </summary>
        float MazeWallThick { get; }
        /// <summary>
        /// 迷宫墙的高度
        /// </summary>
        float MazeWallHeight { get; }
        /// <summary>
        /// 建造墙的时间间隔，单位：秒
        /// </summary>
        float BuildInterval { get; }
        /// <summary>
        /// 建立墙的队列
        /// </summary>
        Queue<MazeWall> BuildWallsQueue { get; }
        /// <summary>
        /// 建造地面
        /// </summary>
        GameObject BuildGround();
    }
}
