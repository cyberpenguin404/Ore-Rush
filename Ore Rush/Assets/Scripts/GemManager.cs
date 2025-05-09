using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Gem;

    [SerializeField]
    public List<Vector3> gemPositions = new List<Vector3>();


    private List<Vector2Int> ValidSpawnPositions = new List<Vector2Int>();

    [SerializeField]
    private GridGenerate GridGenerateScript;

    [SerializeField]
    private int StartGemCount;
    [SerializeField]
    private int MaxGemCount;
    [SerializeField]
    private int GemSpawnRate;

    private int _gemCount;
    private double _timer;
    void Start()
    {
        for (int i = 0; i < StartGemCount; i++)
        {
            SpawnGem();
            _gemCount++;
        }
    }
    public void RecalculateSpawnPositions()
    {
        ValidSpawnPositions.Clear();
        for (int x = 0; x < GridGenerateScript.width; x++)
        {
            for (int y = 0; y < GridGenerateScript.height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Vector3 worldPos = GridGenerateScript.GridToWorldPosition(pos);

                if (!GridGenerateScript.wallPositions.Contains(worldPos) && !gemPositions.Contains(worldPos))
                {
                    ValidSpawnPositions.Add(pos);
                }
            }
        }
    }
    private void SpawnGem()
    {
        RecalculateSpawnPositions();
        _gemCount++;

        if (ValidSpawnPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions left to spawn!");
            return;
        }

        Vector2Int gridPosition = ValidSpawnPositions[Random.Range(0, ValidSpawnPositions.Count - 1)];
        Vector3 spawnPosition = GridGenerateScript.GridToWorldPosition(gridPosition);


        gemPositions.Add(spawnPosition);
        ValidSpawnPositions.Remove(gridPosition);
        Instantiate(Gem, spawnPosition, Quaternion.identity);
    }
    public void PickupGem(GameObject gem)
    {
        gemPositions.Remove(gem.transform.position);
        ValidSpawnPositions.Add(GridGenerateScript.WorldToGridPosition(gem.transform.position));
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > GemSpawnRate)
        {
            if (_gemCount < MaxGemCount - 1)
            {
                SpawnGem();
            }
            _timer -= GemSpawnRate;
        }
    }
}
