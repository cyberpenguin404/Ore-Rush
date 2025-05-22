using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerHandler : MonoBehaviour
{
    public bool spawnWall;

    private State _currentState;

    public string PlayerName;

    public int PlayerIndex;
    public GameObject _carryingObject { get; private set; }

    private float _stunTimer = 0f;
    [SerializeField] private float stunDuration = 2f;

    public int Score;

    private TextMeshProUGUI ScoreText => PlayerIndex == 1 ? GameManager.Instance.ScoreTextPlayer1 : GameManager.Instance.ScoreTextPlayer2;

    private TextMeshProUGUI PickaxeCooldownText => PlayerIndex == 1 ? GameManager.Instance.PickaxeCooldownText1 : GameManager.Instance.PickaxeCooldownText2;
    [HideInInspector]
    public TextMeshProUGUI DynamiteCooldownText => PlayerIndex == 1 ? GameManager.Instance.DynamiteCooldownText1 : GameManager.Instance.DynamiteCooldownText2;
    [SerializeField]
    private float PickaxeCooldown;

    [Header("Materials")]
    [SerializeField]
    private Material player1Material;
    [SerializeField]
    private Material player2Material;

    public float _currentPickaxeCooldown { get; private set; }

    [field: SerializeField] public Vector3 CarryingOffset { get; private set; }
    [field: SerializeField] public float mineRange { get; private set; }
    [field: SerializeField] public float PickaxeStunTime { get; private set; }
    private bool _isStunnedAnimating = false;
    public bool _canTurn = true;

    [Header("Movement Settings")]
    public float PlayerSpeed;
    [SerializeField]
    private CharacterController _charController;

    private Vector2 _moveInput;

    public void OnMovement(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        _currentState = new NoneState(this);

        GameManager.Instance._playerCount++;
        GameManager.Instance.Players.Add(this);
        PlayerIndex = GameManager.Instance._playerCount;
        PlayerName = "Player " + PlayerIndex;

        GetComponent<Renderer>().material = PlayerIndex == 1 ? player1Material : player2Material;
    }
    void Update()
    {
        if (spawnWall)
        {
            GameManager.Instance.DropWall(transform.position + Vector3.up *10);
            spawnWall = false;
        }
        if (_stunTimer > 0)
        {
            _stunTimer -= Time.deltaTime;
            if (!_isStunnedAnimating)
            {
                _isStunnedAnimating = true;
                StartCoroutine(StunAnimation());
            }
            return;
        }

        HandleMovement();
        _currentState.Update();
        HandleCooldowns();
        HandleCarrying();
    }
    private IEnumerator StunAnimation()
    {
        float duration = _stunTimer; // or set a fixed duration like 0.5f
        float elapsed = 0f;
        float shakeAmount = 0.05f;
        float shakeSpeed = 50f;

        Transform model = transform; // or your visual child object
        Vector3 originalPosition = model.localPosition;
        Renderer rend = GetComponentInChildren<Renderer>();
        Color originalColor = rend.material.color;

        rend.material.color = Color.cyan;

        while (_stunTimer > 0)
        {
            float x = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float z = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;
            model.localPosition = originalPosition + new Vector3(x, 0, z);

            yield return null;
        }

        _isStunnedAnimating = false;
        model.localPosition = originalPosition;
        rend.material.color = originalColor;
    }

    private void HandleCarrying()
    {
        if (_carryingObject != null)
        {
            _carryingObject.transform.position = transform.position + CarryingOffset;
        }
    }

    private void HandleCooldowns()
    {
        if (_currentPickaxeCooldown > 0)
        {
            PickaxeCooldownText.text = "Pickaxe cooldown:" + ((int)_currentPickaxeCooldown).ToString();
            GameManager.Instance.PickaxeCooldownSlider1.value = _currentPickaxeCooldown; 
            _currentPickaxeCooldown -= Time.deltaTime;
        }
    }

    public void ChangeState(State state)
    {
        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    public void PickUpGem(GameObject gem)
    {
        if (_carryingObject == null)
        {
            GameManager.Instance.GemManager.PickupGem(gem);
            _carryingObject = gem;
            _carryingObject.GetComponent<Collider>().enabled = false;
        }
    }

    internal void CollectGem()
    {
        if (_carryingObject != null)
        {
            Score++;
            ScoreText.text = "Score: " + Score;
            Destroy(_carryingObject);
        }
    }
    public void StartMine()
    {
        if (_currentState.GetType() == typeof(NoneState))
        {
            ChangeState(new MineState(this));
        }
    }

    internal void Mine(Vector3 direction)
    {

        Debug.Log("Mine called");
        if (_currentPickaxeCooldown > 0)
        {
            //Debug.Log("Cooldown");
            return;
        }


        Ray ray = new Ray(new Vector3(transform.position.x,0,transform.position.z), direction);
        RaycastHit hit;

        int wallLayerMask = 1 << LayerMask.NameToLayer("Pickaxeable");

        if (Physics.Raycast(ray, out hit, mineRange, wallLayerMask))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                _currentPickaxeCooldown = PickaxeCooldown;
                Destroy(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Scaffholding"))
            {
                _currentPickaxeCooldown = PickaxeCooldown;
                hit.collider.GetComponent<ScaffholdingScript>().CollapseScaffholding(direction);
            }
            else
            {
                //Debug.Log("Not right tag");
            }
        }
        else
        {
            //Debug.Log("Nothing found");
        }
    }

    public void Stun()
    {
        _stunTimer = stunDuration;

        Debug.Log("Player stunned");

        if (_carryingObject != null)
        {
            ThrowGemAway();
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
    private void ThrowGemAway()
    {
        Vector2Int playerGridPos = GameManager.Instance.GemManager.GridGenerateScript.WorldToGridPosition(transform.position);
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int targetGridPos = playerGridPos + dir * 2;
            Vector3 worldPos = GameManager.Instance.GemManager.GridGenerateScript.GridToWorldPosition(targetGridPos);

            if (!GameManager.Instance.GemManager.GridGenerateScript.wallPositions.Contains(worldPos) &&
                !GameManager.Instance.GemManager.gemObjects.Exists(obj => obj.transform.position == worldPos))
            {
                _carryingObject.transform.position = worldPos;
                _carryingObject.GetComponent<Collider>().enabled = true;

                GameManager.Instance.GemManager.gemObjects.Add(_carryingObject);
                _carryingObject = null;
                return;
            }
        }

        _carryingObject.transform.position = transform.position + Vector3.up * 0.5f;
        _carryingObject.GetComponent<Collider>().enabled = true;
        GameManager.Instance.GemManager.gemObjects.Add(_carryingObject);
        _carryingObject = null;
    }


    private void HandleMovement()
    {
        _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);

        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);

        _charController.Move(move * PlayerSpeed * Time.deltaTime);


        if (move.sqrMagnitude > 0.01f && _canTurn)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Only rotate on Y-axis
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }
}
