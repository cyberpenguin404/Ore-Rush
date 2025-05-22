using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerate : MonoBehaviour
{

    private int _width;
    private int _height;

    public GameObject TilePrefab;
    public GameObject WallPrefab;
    public Transform collectionZone; // Assign in Inspector

    [SerializeField]
    private float wallCollapseSpeed;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GemManager gemManager;

    private GameObject[,] gridArray;
    public List<Vector3> wallPositions = new List<Vector3>();
    private List<Vector2Int> specialTilePositions = new List<Vector2Int>();

    public float wallRegenInterval = 60f;

    private Vector3 _spawnPosition = new Vector3(0,1,0);
    public bool _isCollapsingMaze { get; private set; } = false;

    private void Start()
    {
        _width = GameManager.Instance.Width;
        _height = GameManager.Instance.Height;
        //CreateFloor();
        StartCoroutine(RegenerateWallsRoutine());
        AddUnbreakableOuterWalls();
    }
    private void AddUnbreakableOuterWalls()
    {
        for (int x = 0; x < _width; x++)
        {
            Vector3 bottom = GridToWorldPosition(new Vector2Int(x, 0));
            Vector3 top = GridToWorldPosition(new Vector2Int(x, _height - 1));
            Instantiate(WallPrefab, bottom, Quaternion.identity).tag = "Unbreakable";
            Instantiate(WallPrefab, top, Quaternion.identity).tag = "Unbreakable";
        }
        for (int y = 0; y < _height; y++)
        {
            Vector3 left = GridToWorldPosition(new Vector2Int(0, y));
            Vector3 right = GridToWorldPosition(new Vector2Int(_width - 1, y));
            Instantiate(WallPrefab, left, Quaternion.identity).tag = "Unbreakable";
            Instantiate(WallPrefab, right, Quaternion.identity).tag = "Unbreakable";
        }
    }

    void CreateFloor()
    {
        Vector3 start = new Vector3(-(_width - 1) / 2f, 0, -(_height - 1) / 2f);
        gridArray = new GameObject[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);

                // Skip tiles in the central 3x3 zone
                if (IsInCenterSquare(pos, 3))
                    continue;

                Vector3 worldPos = start + new Vector3(x, 0, z);
                GameObject tile = Instantiate(TilePrefab, worldPos, Quaternion.identity, transform);
                gridArray[x, z] = tile;
            }
        }
    }

    IEnumerator RegenerateWallsRoutine()
    {
        while (true)
        {
            ResetPlayerPositions();
            RemoveGems();
            GenerateReachableWallLayout();
            gemManager.SpawnStartingGems();
            yield return new WaitForSeconds(wallRegenInterval);
            StartCoroutine(CollapseMaze());

            while (_isCollapsingMaze)
            {
                yield return null;
            }
        }
    }
    IEnumerator CollapseMaze()
    {
        _isCollapsingMaze = true;
        int maxLayer = Mathf.Max(_width, _height) / 2;
        for (int layer = 0; layer <= maxLayer; layer++)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (IsInCenterSquare(pos, 3)) continue; // Skip the 3x3 center
                    if (!IsInsideGrid(pos)) continue;

                    int distanceFromEdge = Mathf.Min(x, _width - 1 - x, y, _height - 1 - y);
                    if (distanceFromEdge == layer)
                    {
                        Vector3 worldPos = GridToWorldPosition(pos);
                        worldPos.y = 10;

                        if (!wallPositions.Contains(worldPos))
                        {
                            GameManager.Instance.DropWall(worldPos);
                            wallPositions.Add(worldPos);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(wallCollapseSpeed);
        }
        _isCollapsingMaze = false;
    }

    private void RemoveGems()
    {
        foreach (GameObject gem in gemManager.gemObjects.ToList())
        {
            Destroy(gem);
        }
        gemManager.gemObjects.Clear();
    }

    private void ResetPlayerPositions()
    {
        foreach (PlayerHandler player in gameManager.Players)
        {
            player.transform.position = _spawnPosition;
        }
    }

    void GenerateReachableWallLayout()
    {
        // Clear old walls
        foreach (var wall in GameObject.FindGameObjectsWithTag("Wall"))
        {
            Destroy(wall);
        }

        wallPositions.Clear();
        specialTilePositions.Clear();

        // Attempt until a reachable layout is found
        int attempts = 0;
        do
        {
            attempts++;
            wallPositions.Clear();
            specialTilePositions.Clear();

            PlaceRandomWalls();

        } while (!IsCollectionZoneReachable() && attempts < 10);

        // Instantiate walls
        foreach (var pos in wallPositions)
        {;
            GameObject wall = Instantiate(WallPrefab, pos, Quaternion.identity);
            wall.tag = "Wall";
        }
    }

    void PlaceRandomWalls()
    {
        wallPositions.Clear();
        specialTilePositions.Clear();

        // Adjust for maze dimensions
        if (_width % 2 == 0) _width -= 1;
        if (_height % 2 == 0) _height -= 1;

        bool[,] maze = new bool[_width, _height];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(1, 1);

        maze[start.x, start.y] = true;
        stack.Push(start);

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 2),
            new Vector2Int(2, 0),
            new Vector2Int(0, -2),
            new Vector2Int(-2, 0)
        };

        System.Random rng = new System.Random();

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            var shuffled = directions.OrderBy(x => rng.Next()).ToArray();

            foreach (var dir in shuffled)
            {
                Vector2Int next = current + dir;
                if (IsInsideMaze(next, maze) && !maze[next.x, next.y])
                {
                    Vector2Int wall = current + dir / 2;
                    maze[next.x, next.y] = true;
                    maze[wall.x, wall.y] = true;
                    stack.Push(next);
                }
            }
        }

        // Convert empty cells to wall positions (exclude center 5x5)
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (!maze[x, y])
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!IsInCenterSquare(pos, 5) && pos != WorldToGridPosition(collectionZone.position))
                    {
                        wallPositions.Add(GridToWorldPosition(pos));
                        specialTilePositions.Add(pos);
                    }
                }
            }
        }
    }

    bool IsInsideMaze(Vector2Int pos, bool[,] maze)
    {
        return pos.x > 0 && pos.x < _width && pos.y > 0 && pos.y < _height;
    }

    bool IsCollectionZoneReachable()
    {
        Vector2Int start = WorldToGridPosition(collectionZone.position);
        bool[,] visited = new bool[_width, _height];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        int reachable = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            reachable++;

            foreach (Vector2Int dir in new Vector2Int[] {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
            })
            {
                Vector2Int neighbor = current + dir;
                if (IsInsideGrid(neighbor) && !visited[neighbor.x, neighbor.y] && !wallPositions.Contains(GridToWorldPosition(neighbor)))
                {
                    visited[neighbor.x, neighbor.y] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        int walkableTiles = _width * _height - wallPositions.Count;
        return reachable >= walkableTiles;
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x + (_width - 1) / 2f);
        int y = Mathf.RoundToInt(worldPosition.z + (_height - 1) / 2f);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        float x = gridPosition.x - (_width - 1) / 2f;
        float z = gridPosition.y - (_height - 1) / 2f;
        return new Vector3(x, 0, z);
    }

    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height;
    }

    bool IsInCenterSquare(Vector2Int pos, int size)
    {
        int minX = (_width - size) / 2;
        int minY = (_height - size) / 2;
        return pos.x >= minX && pos.x < minX + size && pos.y >= minY && pos.y < minY + size;
    }
}
