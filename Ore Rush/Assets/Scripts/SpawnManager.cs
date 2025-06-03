using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const int ExcludedZoneAroundCenter = 2;
    [SerializeField]
    private GameObject _gem;
    [SerializeField]
    private GameObject _scaffholding;

    [SerializeField]
    public List<GameObject> GemObjects = new List<GameObject>();

    [SerializeField]
    public List<GameObject> ScaffholdingObjects = new List<GameObject>();


    public List<Vector3> EmptyTiles = new List<Vector3>();
    public List<GameObject> EmptyWalls = new List<GameObject>();

    [SerializeField]
    public GridGenerate GridGenerateScript;

    [SerializeField]
    private int _startScaffholdingCount;
    [SerializeField]
    private int _maxScaffholdingCount;
    [SerializeField]
    private int _scaffholdingSpawnRate;

    [SerializeField]
    private int _startGemCount;
    [SerializeField]
    private int _maxGemCount;
    [SerializeField]
    private int _gemSpawnRate;

    private double _gemSpawnTimer;
    private double _scaffholdingSpawnTimer;

    private int _mapWidth;
    private int _mapHeight;

    private void Start()
    {
    }
    public void InitiateMap()
    {
        CalculateEmptyTilesPositions();
        SpawnStartingGems();
        SpawnStartingScaffholding();
    }
    public void SpawnStartingGems()
    {
        for (int i = 0; i < _startGemCount; i++)
        {
            SpawnGem();
        }
    }
    public void SpawnStartingScaffholding()
    {
        for (int i = 0; i < _startScaffholdingCount; i++)
        {
            SpawnScaffholding();
        }
    }
    public void CalculateEmptyTilesPositions()
    {
        _mapWidth = GameManager.Instance.Width;
        _mapHeight = GameManager.Instance.Height;

        Debug.Log("CalculateEmptyTiles Called");
        Vector2Int center = new Vector2Int(_mapWidth / 2, _mapHeight / 2);

        Debug.Log($"Center is {center}");
        EmptyTiles.Clear();

        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                Vector2Int pos = new(x, y);

                bool isInCenter = Mathf.Abs(pos.x - center.x) <= ExcludedZoneAroundCenter &&
                                  Mathf.Abs(pos.y - center.y) <= ExcludedZoneAroundCenter;

                if (isInCenter)
                {
                    Debug.Log($"tile was in center");
                    continue;
                }

                Vector3 worldPos = GridGenerateScript.GridToWorldPosition(pos);
                bool wallPresent = GridGenerateScript.wallPositions.Contains(worldPos);

                Debug.Log($"Wall present returns {wallPresent}");

                if (!wallPresent)
                {
                    Debug.Log("Added empty tile");
                    EmptyTiles.Add(worldPos);
                }
            }
        }
    }

    private void SpawnScaffholding()
    {

        if (EmptyTiles.Count == 0)
        {
            Debug.LogWarning("No valid positions left to spawn!");
            return;
        }

        Vector3 spawnPosition = EmptyTiles[Random.Range(0, EmptyTiles.Count - 1)];
        GameObject newScaffholding = Instantiate(_scaffholding, spawnPosition, Quaternion.identity);

        EmptyTiles.Remove(spawnPosition);
        ScaffholdingObjects.Add(newScaffholding);
    }
    private void SpawnGem()
    {

        if (EmptyWalls.Count == 0)
        {
            Debug.LogWarning("No valid positions left to spawn!");
            return;
        }

        GameObject spawnPosition = EmptyWalls[Random.Range(0, EmptyWalls.Count - 1)];


        GameObject newGem = Instantiate(_gem, spawnPosition.transform.position, Quaternion.identity);
        EmptyWalls.Remove(spawnPosition);

        GemObjects.Add(newGem);
        spawnPosition.GetComponent<WallScript>().gemInsideMe = newGem; 
        InitializeGemValue(spawnPosition, newGem);
    }

    private static void InitializeGemValue(GameObject spawnPosition, GameObject newGem)
    {
        float distanceFromCollectionZone = Vector3.Distance(spawnPosition.transform.position, GameManager.Instance.GridGenerate.collectionZone.transform.position);
        float maxDistance = Vector3.Distance(GameManager.Instance.GridGenerate.collectionZone.transform.position,
            new Vector3(GameManager.Instance.Width / 2, 0, GameManager.Instance.Height / 2));

        int gemValue = (int)((10 * GameManager.Instance.Stage) + distanceFromCollectionZone / (maxDistance / 10));
        newGem.GetComponent<Gem>().Initialize(gemValue);
    }
    public void PickupGem(GameObject gem)
    {
        GemObjects.Remove(gem);
        EmptyTiles.Add(gem.transform.position);
    }

    void Update()
    {
        if (!GameManager.Instance.MainGameRunning)
            return;
        HandleGemTimer();
        HandleScaffholdingTimer();
    }

    private void HandleGemTimer()
    {
        _gemSpawnTimer += Time.deltaTime;
        if (_gemSpawnTimer > _gemSpawnRate)
        {
            if (GemObjects.Count < _maxGemCount - 1)
            {
                SpawnGem();
            }
            _gemSpawnTimer -= _gemSpawnRate;
        }
    }
    private void HandleScaffholdingTimer()
    {
        _scaffholdingSpawnTimer += Time.deltaTime;
        if (_scaffholdingSpawnTimer > _scaffholdingSpawnRate)
        {
            if (ScaffholdingObjects.Count < _maxScaffholdingCount - 1)
            {
                SpawnScaffholding();
            }
            _scaffholdingSpawnTimer -= _scaffholdingSpawnRate;
        }
    }
}
