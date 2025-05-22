using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class DynamiteScript : MonoBehaviour
{
    private const float IndicatorVerticalOffset = 2f;
    public GameObject indicatorPrefabPlayer1;
    public GameObject indicatorPrefabPlayer2;

    public GameObject pillarPrefab;     // The 1x1 pillar

    public float moveRepeatDelay = 0.6f;   // Time before repeat starts
    public float moveRepeatRate = 0.1f;    // Time between repeated moves

    public float cooldownDuration = 5f;

    [SerializeField]
    private PlayerHandler PlayerHandlerScript;

    private GameObject _indicatorPrefab;  // The green cube

    private float moveTimer = 0f;
    private Vector2Int lastInputDir = Vector2Int.zero;

    private float cooldownTimer = 0f;

    private GameObject indicatorInstance;

    private Vector2Int gridPos = Vector2Int.zero; // Start at center (0,0)
    private bool indicatorActive = false;

    private int halfGridSize = 12; // 25x25 centered = -12 to 12

    private Vector2 _aimInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _indicatorPrefab = PlayerHandlerScript.PlayerIndex == 1 ? indicatorPrefabPlayer1 : indicatorPrefabPlayer2;
        indicatorInstance = Instantiate(_indicatorPrefab, GridToWorldPosition(gridPos), Quaternion.identity);
        
        indicatorInstance.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleCooldown();

        if (indicatorActive)
        {
            HandleAiming();
        }
    }

    public void ToggleDynamite(InputAction.CallbackContext context)
    {
        if (!context.started || cooldownTimer > 0f)
        {
            return;
        }
            if (!indicatorActive)
            {
                Debug.Log("activated indicator");
                // Snap player position to nearest grid cell
                Vector3 playerPos = transform.position;
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
            Debug.Log("placed pillar");
            bool anyPlaced = false;

            int range = 2; // 5x5 square centered = -2 to +2
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    Vector2Int targetGridPos = new Vector2Int(gridPos.x + dx, gridPos.y + dy);

                    // Clamp to play area
                    targetGridPos.x = Mathf.Clamp(targetGridPos.x, -halfGridSize, halfGridSize);
                    targetGridPos.y = Mathf.Clamp(targetGridPos.y, -halfGridSize, halfGridSize);

                    Vector3 spawnPos = new Vector3(targetGridPos.x, 0, targetGridPos.y);

                    Collider[] colliders = Physics.OverlapBox(spawnPos, Vector3.one * 0.45f);
                    bool occupied = false;
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
                        Instantiate(pillarPrefab, spawnPos, Quaternion.identity);
                        anyPlaced = true;
                    }
                }
            }

            if (anyPlaced)
            {
                indicatorActive = false;
                indicatorInstance.SetActive(false);
                cooldownTimer = cooldownDuration;
            }
        }
    }

    void HandleCooldown()
    {
        if (cooldownTimer > 0f)
        {
            PlayerHandlerScript.DynamiteCooldownText.text = "Dynamite cooldown: " + ((int)cooldownTimer).ToString();
            cooldownTimer -= Time.deltaTime;
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

    public void OnMove(InputAction.CallbackContext context)
    {
        _aimInput = context.ReadValue<Vector2>();
    }

    public void HandleAiming()
    {
        Vector2Int moveDir = Vector2Int.zero;


        // Priority order: Vertical before Horizontal
        if (_aimInput.y > 0.5f)
            moveDir = Vector2Int.up;
        else if (_aimInput.y < -0.5f)
            moveDir = Vector2Int.down;
        else if (_aimInput.x < -0.5f)
            moveDir = Vector2Int.left;
        else if (_aimInput.x > 0.5f)
            moveDir = Vector2Int.right;

        if (moveDir != Vector2Int.zero)
        {
            if (moveDir != lastInputDir)
            {
                // New direction → move immediately
                MoveIndicator(moveDir);
                moveTimer = moveRepeatDelay;
            }
            else
            {
                // Holding same key
                moveTimer -= Time.deltaTime;
                if (moveTimer <= 0f)
                {
                    MoveIndicator(moveDir);
                    moveTimer = moveRepeatRate;
                }
            }

            lastInputDir = moveDir;
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
        return new Vector3(gridPosition.x, IndicatorVerticalOffset, gridPosition.y);
    }
}
