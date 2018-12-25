using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.MazeCore;
using System.Threading;


public class MazeBuilder : MonoBehaviour, IMazeBuilder
{
    #region MonoBehaviour
    public int mazeBuildWidth = 10;
    public int mazeBuildHeight = 10;
    public int mazeStartUnitPositionX = 0;
    public int mazeStartUnitPositionY = 0;
    public int mazeEndUnitPositionX = 9;
    public int mazeEndUnitPositionY = 9;
    public float mazeWallThick = 0.3f;
    public float mazeWallHeight = 0.5f;
    public float buildInterval = 0.1f;
    public Camera mainCamera;
    private Queue<MazeWall> buildWallsQueue;
    private GameObject prefabGroundObj;
    private GameObject prefabWallObj;
    private GameObject groundObj;

    // Start is called before the first frame update
    void Start()
    {
        prefabGroundObj = Resources.Load("Prefabs/Ground") as GameObject;
        prefabWallObj = Resources.Load("Prefabs/Wall") as GameObject;
        buildWallsQueue = new Queue<MazeWall>();
        var trim = new TrimGenerateMazeAlgoritnm(this);
        trim.GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        if (BuildWallsQueue != null && BuildWallsQueue.Count > 0)
        {
            var wall = BuildWallsQueue.Dequeue();
            if (wall.HasWall)
            {
                if (wall.LocalObject == null)
                {
                    wall.LocalObject = BuildOneWall(wall.LocalPosition, wall.LocalScale);
                }
                else
                {
                    wall.LocalObject.SetActive(true);
                }
            }
            else
            {
                wall.LocalObject?.SetActive(false);
            }

        }
    }
    #endregion
    #region IMazeBuilder
    public MazeSize MazeBuildSize => new MazeSize(mazeBuildWidth, mazeBuildHeight);
    public MazePosition MazeStartUnitPosition => new MazePosition(mazeStartUnitPositionX, mazeStartUnitPositionY);
    public MazePosition MazeEndUnitPosition => new MazePosition(mazeEndUnitPositionX, mazeEndUnitPositionY);
    public float MazeWallThick => mazeWallThick;
    public float MazeWallHeight => mazeWallHeight;
    public float BuildInterval => buildInterval;
    public Queue<MazeWall> BuildWallsQueue => buildWallsQueue;

    public GameObject BuildGround()
    {
        groundObj = Instantiate(prefabGroundObj);
        groundObj.transform.localScale = new Vector3(MazeBuildSize.Width, 1, MazeBuildSize.Height);
        groundObj.transform.parent = transform;
        return groundObj;
    }

    public GameObject BuildOneWall(Vector3 wallPosition, Vector3 wallScale)
    {
        GameObject wall = Instantiate(prefabWallObj);
        wall.transform.position = wallPosition;
        wall.transform.localScale = wallScale;
        wall.transform.parent = groundObj.transform;
        return wall;
    }

    #endregion
}

