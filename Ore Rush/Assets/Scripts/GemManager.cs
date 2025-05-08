using System;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Gem;

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
        }
    }

    private void SpawnGem()
    {
        _gemCount++;
        throw new NotImplementedException();
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
