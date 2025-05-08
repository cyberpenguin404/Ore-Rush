using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class DynamiteScript : MonoBehaviour
{
    public GameObject indicator;         // Reference to the green cube in the scene
    public GameObject pillarPrefab;      // Prefab to instantiate
    public int gridSize = 25;

    private Vector2Int currentGridPos = Vector2Int.zero;
    private bool indicatorActive = false;
    private float moveCooldown = 0.2f;
    private float moveTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        HandleToggle();

        if (indicatorActive)
        {
            HandleMovement();
            HandlePlacement();
        }

        moveTimer += Time.deltaTime;
    }

    void HandleToggle()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            indicatorActive = !indicatorActive;
            indicator.SetActive(indicatorActive);
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal2");
        float vertical = Input.GetAxis("Vertical2");

        Vector2Int direction = Vector2Int.zero;

        if (Mathf.Abs(horizontal) > 0.5f)
            direction.x = (int)Mathf.Sign(horizontal);

        if (Mathf.Abs(vertical) > 0.5f)
            direction.y = (int)Mathf.Sign(vertical);

        if (direction != Vector2Int.zero && moveTimer >= moveCooldown)
        {
            moveTimer = 0f;

            Vector2Int newGridPos = currentGridPos + direction;

            // Clamp to grid size
            newGridPos.x = Mathf.Clamp(newGridPos.x, 0, gridSize - 1);
            newGridPos.y = Mathf.Clamp(newGridPos.y, 0, gridSize - 1);

            currentGridPos = newGridPos;

            indicator.transform.position = new Vector3(currentGridPos.x + 0.5f, 0.5f, currentGridPos.y + 0.5f);
        }
    }

    void HandlePlacement()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            Vector3 spawnPos = new Vector3(currentGridPos.x + 0.5f, 0.5f, currentGridPos.y + 0.5f);
            Instantiate(pillarPrefab, spawnPos, Quaternion.identity);
        }
    }

}
