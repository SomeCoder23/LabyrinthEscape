using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    #region Singleton
    public static MazeGenerator instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one MazeGenerator!!");
            return;
        }
        instance = this;
    }

    #endregion

    public float scaleFactor = 4;
    //[Tooltip("The value the width and height of the maze will be multiplied by for the next levels")]
    //public int sizeMultiplier = 2;
    [Tooltip("The distance from the player where the random spawning of objects (enemies/pills) should start")]
    public float startSpawnDistance = 3f;
    public GameObject ground, beam;

    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    private MazeCell[,] _mazeGrid;
    SpawnManager spawner;
    Vector3 mazeCenter;

    [HideInInspector]
    public bool generatingMaze = true;
    int width, depth;
    float cellSize;
    float spawnDistance;

    void Start()
    {
        spawner = GetComponent<SpawnManager>();
        StartFirstMaze();
    }

    void StartFirstMaze()
    {
        width = _mazeWidth;
        depth = _mazeDepth;
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
                _mazeGrid[x, z].transform.SetParent(transform);
            }
        }
        cellSize = scaleFactor / 2;
        InitializeMaze();
    }

    void InitializeMaze()
    {
        mazeCenter = new Vector3((width * scaleFactor / 2) - cellSize, 0, (depth * scaleFactor / 2) - cellSize); 
        ground.transform.position = mazeCenter;
        _mazeGrid[width / 2, depth / 2].SpawnCellObject(beam, scaleFactor);
        spawner.CalculateEmptyCells(depth * width);
        ground.transform.localScale = new Vector3(width * scaleFactor, 0.05f, depth * scaleFactor);

        spawnDistance = depth <= 3 ? 0 : startSpawnDistance;
        GenerateMaze(null, _mazeGrid[0, 0]);

        transform.localScale = transform.localScale * scaleFactor;
        beam.gameObject.SetActive(true);
        Invoke("BuildNavMap", 0.05f);
        SetPlayerPosition();
        generatingMaze = false;
    }

    void BuildNavMap()
    {
        NavMeshSurface navMap = GetComponent<NavMeshSurface>();
        if (navMap != null)
        {
            navMap.BuildNavMesh();
        }
    }

    public void SetPlayerPosition()
    {
        //Position player at the start of the maze
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;
        player.position = Vector3.zero;
        controller.enabled = true;
        player.GetComponent<ThirdPersonController>().SetMaxDistance((beam.transform.position - player.position).magnitude);

    }

    public void ResetAndGenerate()
    {
        if (generatingMaze) 
            return;

        generatingMaze = true;
        transform.localScale = Vector3.one;
        spawner.UpdateSpawnCounts();
        int oldW = width;
        int oldD = depth;
        int sizeModifier = spawner.level < 6 ? 0 : spawner.level;
        width = _mazeWidth * spawner.level + sizeModifier;
        depth = _mazeDepth * spawner.level + sizeModifier;
        //Debug.Log("GENERATING NEW LEVEL " + spawner.level + "=> WIDTH = " + width + " // DEPTH = " + depth);

        MazeCell[,] oldCells = _mazeGrid;

        _mazeGrid = new MazeCell[width, depth];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (x < oldW && z < oldD && oldCells[x, z] != null)
                {
                    _mazeGrid[x, z] = oldCells[x, z];
                    _mazeGrid[x, z].Reset();
                }
                else
                {
                    _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
                    _mazeGrid[x, z].transform.SetParent(transform);
                }
            }
        }

        InitializeMaze();
        ScoreManager.instance.StartLevel(spawner.level);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        if (previousCell != null && currentCell.transform.position.magnitude > spawnDistance)
        {
            GameObject spawnObject = previousCell.hasObject ? null : spawner.getRandomObject();
            currentCell.SpawnCellObject(spawnObject, scaleFactor);
        }

        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;
        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < width)
        {
            var cellToRight = _mazeGrid[x + 1, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < depth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    public void ResetMaze()
    {
        foreach (MazeCell cell in _mazeGrid)
            Destroy(cell.gameObject);

        transform.localScale = Vector3.one;
        spawner.ResetValues();
        StartFirstMaze();
    }
}


