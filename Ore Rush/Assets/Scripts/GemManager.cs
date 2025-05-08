using UnityEngine;

public class GemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Gem;

    [SerializeField] private Vector2 gridsize;

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

    private void SpawnGem()
    {
        _gemCount++;
        Instantiate(Gem, new Vector3(Random.Range(-gridsize.x, gridsize.x), 0.5f, Random.Range(-gridsize.y, gridsize.y)), Quaternion.identity);
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
