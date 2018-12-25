using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.MazeCore;
using System.Threading;
using UnityEngine.SceneManagement;


public class MazeBuilder : MonoBehaviour, IMazeBuilder
{
    #region MonoBehaviour
    public int mazeBuildWidth = 25;
    public int mazeBuildHeight = 25;
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
    private GameObject prefabBallObj;
    private GameObject prefabGoal;
    private GameObject groundObj;
    private GameObject playObj;
    private Rigidbody playerRigidbody;


    // Start is called before the first frame update
    void Start()
    {
        prefabGroundObj = Resources.Load("Prefabs/Ground") as GameObject;
        prefabWallObj = Resources.Load("Prefabs/Wall") as GameObject;
        prefabBallObj = Resources.Load("Prefabs/Ball") as GameObject;
        prefabGoal = Resources.Load("Prefabs/Goal") as GameObject;

        buildWallsQueue = new Queue<MazeWall>();

        var trim = new TrimGenerateMazeAlgoritnm(this);

        // 调整摄影机机位
        mainCamera.transform.Rotate(new Vector3(90, 0, 0));
        mainCamera.transform.localPosition = new Vector3(groundObj.transform.localPosition.x, Mathf.Max(mazeBuildHeight, mazeBuildWidth), groundObj.transform.localPosition.z);

        trim.GenerateMaze();

        // 生成球体
        var ball_pos = trim.TranslateMazeCellPosition(trim.TotalMazeCellsVisitor.StartMazeCell.Position);
        // 让球更高一点
        ball_pos += new Vector3(0, 5, 0);
        playObj = BuildBall(ball_pos, new Vector3(0.5f, 0.5f, 0.5f));
        playerRigidbody = playObj.GetComponent<Rigidbody>();
        // 生成目标点
        var goal_pos = trim.TranslateMazeCellPosition(trim.TotalMazeCellsVisitor.EndMazeCell.Position);
        // 让目标点高一点
        BuildGoal(goal_pos, new Vector3(0.5f, 0.5f, 0.5f));
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
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }
    }


    void FixedUpdate()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        playerRigidbody.AddForce(new Vector3(h, 0, v) * 5);
    }

    public GameObject BuildBall(Vector3 wallPosition, Vector3 wallScale)
    {
        GameObject player = Instantiate(prefabBallObj);
        player.tag = player.name = "Player";
        player.transform.position = wallPosition;
        player.transform.localScale = wallScale;
        return player;
    }

    public GameObject BuildGoal(Vector3 goalPosition, Vector3 goalScale)
    {
        GameObject goal = Instantiate(prefabGoal);
        goal.transform.position = goalPosition;
        goal.transform.localScale = goalScale;
        return goal;
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

