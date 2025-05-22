using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Gem;

    [SerializeField]
    public List<GameObject> gemObjects = new List<GameObject>();


    private List<Vector2Int> ValidSpawnPositions = new List<Vector2Int>();

    [SerializeField]
    public GridGenerate GridGenerateScript;

    [SerializeField]
    private int StartGemCount;
    [SerializeField]
    private int MaxGemCount;
    [SerializeField]
    private int GemSpawnRate;

    private double _timer;

    public void SpawnStartingGems()
    {
        for (int i = 0; i < StartGemCount; i++)
        {
            SpawnGem();
        }
    }

    public void RecalculateSpawnPositions()
    {
        ValidSpawnPositions.Clear();
        for (int x = 2; x < GameManager.Instance.Width; x++)
        {
            for (int y = 2; y < GameManager.Instance.Height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Vector3 worldPos = GridGenerateScript.GridToWorldPosition(pos);

                if (!GridGenerateScript.wallPositions.Contains(worldPos) && !gemObjects.Exists(obj => obj.transform.position == worldPos))
                {
                    ValidSpawnPositions.Add(pos);
                }
            }
        }
    }
    private void SpawnGem()
    {
        RecalculateSpawnPositions();

        if (ValidSpawnPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions left to spawn!");
            return;
        }

        Vector2Int gridPosition = ValidSpawnPositions[Random.Range(0, ValidSpawnPositions.Count - 1)];
        Vector3 spawnPosition = GridGenerateScript.GridToWorldPosition(gridPosition);


        ValidSpawnPositions.Remove(gridPosition);
        gemObjects.Add(Instantiate(Gem, spawnPosition, Quaternion.identity));
    }
    public void PickupGem(GameObject gem)
    {
        gemObjects.Remove(gem);
        ValidSpawnPositions.Add(GridGenerateScript.WorldToGridPosition(gem.transform.position));
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > GemSpawnRate)
        {
            if (gemObjects.Count < MaxGemCount - 1)
            {
                SpawnGem();
            }
            _timer -= GemSpawnRate;
        }
    }
}
