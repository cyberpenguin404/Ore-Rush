using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class DynamiteScript : MonoBehaviour
{
    public GameObject indicatorPrefab;  // The green cube
    public GameObject pillarPrefab;     // The 1x1 pillar
    public GameObject player;

    public float moveRepeatDelay = 0.3f;   // Time before repeat starts
    public float moveRepeatRate = 0.1f;    // Time between repeated moves

    public float cooldownDuration = 5f;


    private float moveTimer = 0f;
    private Vector2Int lastInputDir = Vector2Int.zero;

    private float cooldownTimer = 0f;

    private GameObject indicatorInstance;

    private Vector2Int gridPos = Vector2Int.zero; // Start at center (0,0)
    private bool indicatorActive = false;

    private int halfGridSize = 12; // 25x25 centered = -12 to 12

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indicatorInstance = Instantiate(indicatorPrefab, GridToWorldPosition(gridPos), Quaternion.identity);
        indicatorInstance.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleToggle();

        if (indicatorActive)
        {
            HandleMovement();
            
        }
    }


    void HandleToggle()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return; // On cooldown, ignore input
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (!indicatorActive)
            {
                // Snap player position to nearest grid cell
                Vector3 playerPos = player.transform.position;
                gridPos = new Vector2Int(Mathf.RoundToInt(playerPos.x), Mathf.RoundToInt(playerPos.z));

                // Clamp to grid bounds
                gridPos.x = Mathf.Clamp(gridPos.x, -halfGridSize, halfGridSize);
                gridPos.y = Mathf.Clamp(gridPos.y, -halfGridSize, halfGridSize);

                // Update indicator position
                indicatorInstance.transform.position = GridToWorldPosition(gridPos);

                // Enable
                indicatorActive = true;
                indicatorInstance.SetActive(true);
            }
            else
            {
                // Try to place pillar
                Vector3 spawnPos = new Vector3(gridPos.x, 1.25f, gridPos.y);
                bool occupied = false;

                Collider[] colliders = Physics.OverlapBox(spawnPos, Vector3.one * 0.45f);
                foreach (var col in colliders)
                {
                    if (col.CompareTag("Wall"))
                    {
                        occupied = true;
                        break;
                    }
                }

                if (!occupied)
                {
                    // Place pillar
                    Instantiate(pillarPrefab, spawnPos, Quaternion.identity);

                    // Turn indicator OFF and start cooldown
                    indicatorActive = false;
                    indicatorInstance.SetActive(false);
                    cooldownTimer = cooldownDuration;
                }
            }
        }
    }


    void MoveIndicator(Vector2Int direction)
    {
        Vector2Int newPos = gridPos + direction;

        newPos.x = Mathf.Clamp(newPos.x, -halfGridSize, halfGridSize);
        newPos.y = Mathf.Clamp(newPos.y, -halfGridSize, halfGridSize);

        if (newPos != gridPos)
        {
            gridPos = newPos;
            indicatorInstance.transform.position = GridToWorldPosition(gridPos);
        }
    }



    void HandleMovement()
    {
        Vector2Int inputDir = Vector2Int.zero;

        // Priority order: Vertical before Horizontal
        if (Input.GetKey(KeyCode.W)) inputDir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.S)) inputDir = Vector2Int.down;
        else if (Input.GetKey(KeyCode.A)) inputDir = Vector2Int.left;
        else if (Input.GetKey(KeyCode.D)) inputDir = Vector2Int.right;

        if (inputDir != Vector2Int.zero)
        {
            if (inputDir != lastInputDir)
            {
                // New direction → move immediately
                MoveIndicator(inputDir);
                moveTimer = moveRepeatDelay;
            }
            else
            {
                // Holding same key
                moveTimer -= Time.deltaTime;
                if (moveTimer <= 0f)
                {
                    MoveIndicator(inputDir);
                    moveTimer = moveRepeatRate;
                }
            }

            lastInputDir = inputDir;
        }
        else
        {
            // No input
            moveTimer = 0f;
            lastInputDir = Vector2Int.zero;
        }
    }


    Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        // Convert grid pos to world pos (center of cell)
        return new Vector3(gridPosition.x, 0.25f, gridPosition.y);
    }
}
