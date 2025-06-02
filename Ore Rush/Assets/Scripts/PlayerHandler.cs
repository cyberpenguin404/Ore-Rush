using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.ShaderData;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerHandler : MonoBehaviour
{
    public bool spawnWall;

    private State _currentState;

    public string PlayerName;

    public int PlayerIndex;
    public GameObject _carryingObject { get; private set; }

    [SerializeField]
    private GameObject _arrowIndicator;

    private float _stunTimer = 0f;
    [SerializeField] private float stunDuration = 2f;
    public int Score
    {
        get => GameManager.Instance.PlayerScores.ContainsKey(PlayerIndex) ? GameManager.Instance.PlayerScores[PlayerIndex] : 0;
        set
        {
            GameManager.Instance.ChangeScore(value, PlayerIndex);
            ScoreText.text = value.ToString();
        }
    }
    [HideInInspector]
    public Image DynamiteIcon;
    [HideInInspector]
    public GameObject RJoystickIcon;
    private TextMeshProUGUI ScoreText => PlayerIndex == 1 ? GameManager.Instance.ScoreTextPlayer1 : GameManager.Instance.ScoreTextPlayer2;

    public Slider PickaxeCooldownSlider;
    public Slider DynamiteCooldownSlider;

    public Animator PickaxeAnimtor;

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

    [SerializeField]
    private float _knockbackForce;

    [SerializeField]
    private GameObject _popup;

    private Vector2 _moveInput;

    private Vector3 _knockBack;
    private float _mass = 5;

    [SerializeField]
    private GameObject _pickaxeIndicator;
    private GameObject _currentPickaxeIndicator;
    [SerializeField]
    private int _maxDropDistance;

    public void OnMovement(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        _currentState = new NoneState(this);

        GameManager.Instance.ConnectPlayer(this);
        PlayerIndex = GameManager.Instance._playerCount;
        PlayerName = "Player " + PlayerIndex;

        GameManager.Instance.PlayerScores[PlayerIndex] = 0;

        GetComponent<Renderer>().material = PlayerIndex == 1 ? player1Material : player2Material;
        PickaxeCooldownSlider = PlayerIndex == 1 ? GameManager.Instance.PickaxeCooldownSlider1 : GameManager.Instance.PickaxeCooldownSlider2;
        PickaxeCooldownSlider.value = 1;
        DynamiteCooldownSlider = PlayerIndex == 1 ? GameManager.Instance.DynamiteCooldownSlider1 : GameManager.Instance.DynamiteCooldownSlider2;
        DynamiteIcon = PlayerIndex == 1 ? GameManager.Instance.DynamiteIcon1 : GameManager.Instance.DynamiteIcon2;
        RJoystickIcon = PlayerIndex == 1 ? GameManager.Instance.RJoystickIcon1 : GameManager.Instance.RJoystickIcon2;

        SetSpawnpoint();
    }

    private void SetSpawnpoint()
    {
        _charController.enabled = false;
        gameObject.transform.position = PlayerIndex == 1 ? GameManager.Instance.SpawnPointPlayer1 : GameManager.Instance.SpawnPointPlayer2;
        _charController.enabled = true;
    }

    void Update()
    {
        if (GameManager.Instance.MainGameRunning)
        HandlePlayer();
    }

    private void HandlePlayer()
    {
        if (spawnWall)
        {
            GameManager.Instance.DropWall(transform.position + Vector3.up * 10);
            spawnWall = false;
        }
        HandleArrow();
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
        HandleMineIndicator();
        _currentState.Update();
        HandleCooldowns();
        HandleCarrying();
    }

    private void HandleMineIndicator()
    {
        if (_currentPickaxeCooldown > 0)
        {
            return;
        }


        Ray ray = new Ray(new Vector3(transform.position.x, 0, transform.position.z), transform.forward);
        RaycastHit hit;

        int wallLayerMask = (1 << LayerMask.NameToLayer("Pickaxeable")) | (1 << LayerMask.NameToLayer("Player"));

        if (Physics.Raycast(ray, out hit, mineRange, wallLayerMask))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                ChangeIndicator(hit);
            }
            else if (hit.collider.CompareTag("Scaffholding"))
            {
                ChangeIndicator(hit);
            }
        }
        else
        {
            Destroy(_currentPickaxeIndicator);
            _currentPickaxeIndicator = null;
        }
    }

    private void ChangeIndicator(RaycastHit hit)
    {
        Vector3 targetLocation = new Vector3(hit.transform.position.x, -1.2f, hit.transform.position.z);
        if (_currentPickaxeIndicator != null)
        {
            if (targetLocation != _currentPickaxeIndicator.transform.position)
            {
                Destroy(_currentPickaxeIndicator);
                _currentPickaxeIndicator = Instantiate(_pickaxeIndicator, targetLocation, _pickaxeIndicator.transform.rotation);
            }
        }
        else
        {
            _currentPickaxeIndicator = Instantiate(_pickaxeIndicator, targetLocation, _pickaxeIndicator.transform.rotation);
        }
    }

    private void HandleArrow()
    {
        if (_carryingObject != null)
        {
            _arrowIndicator.SetActive(true);
            RotateArrow();
        }
        else
        {
            _arrowIndicator.SetActive(false);
        }
    }

    private void RotateArrow()
    {
        Vector3 directionToCollection = GameManager.Instance.GridGenerate.collectionZone.position - transform.position;
        directionToCollection.y = 0;
        _arrowIndicator.transform.rotation = Quaternion.LookRotation(directionToCollection, Vector3.up);
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
            GameManager.Instance.SpawnManager.PickupGem(gem);
            _carryingObject = gem;
            _carryingObject.GetComponent<Collider>().enabled = false;
        }
    }

    internal void CollectGem()
    {
        if (_carryingObject != null)
        {
            int value = _carryingObject.GetComponent<Gem>().Value;
            Score += value;
            GameObject newPopup = Instantiate(_popup, _carryingObject.transform.position, _popup.transform.rotation);
            newPopup.GetComponent<PopupScript>().Text.text = "+" + value;
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

        int wallLayerMask = (1 << LayerMask.NameToLayer("Pickaxeable")) | (1 << LayerMask.NameToLayer("Player"));

        if (Physics.Raycast(ray, out hit, mineRange, wallLayerMask))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                _currentPickaxeCooldown = PickaxeCooldown;

                GameManager.Instance.GridGenerate.wallPositions.Remove(hit.transform.position);
                GameManager.Instance.GridGenerate.wallObjects.Remove(hit.collider.gameObject);
                GameManager.Instance.SpawnManager.EmptyWalls.Remove(hit.collider.gameObject);
                GameManager.Instance.SpawnManager.EmptyTiles.Add(hit.transform.position);

                if (hit.collider.GetComponent<WallScript>().gemInsideMe != null)
                {
                    PickUpGem(hit.collider.GetComponent<WallScript>().gemInsideMe);
                }

                Destroy(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Scaffholding"))
            {
                _currentPickaxeCooldown = PickaxeCooldown;
                hit.collider.GetComponent<ScaffholdingScript>().CollapseScaffholding(direction);
            }
            else if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("hit player");
                hit.collider.GetComponent<PlayerHandler>().ApplyKnockBack(transform.forward, _knockbackForce);
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
    public void ApplyKnockBack(Vector3 dir, float force)
    {
        dir.Normalize();
        _knockBack += dir.normalized * force / _mass;
    }
    public void Stun()
    {
        _stunTimer = stunDuration;

        Debug.Log("Player stunned");

        if (_carryingObject != null)
        {
            StartCoroutine(ThrowGemNextFrame());
        }
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }
    private IEnumerator ThrowGemNextFrame()
    {
        yield return null;
        ThrowGem();
    }
    private void ThrowGem()
    {
        if (_carryingObject == null)
        {
            return;
        }
        Vector2Int playerGridPos = GameManager.Instance.SpawnManager.GridGenerateScript.WorldToGridPosition(transform.position);
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        for (int distance = _maxDropDistance; distance >= 0; distance--)
        {
            foreach (var dir in directions)
            {
                Vector2Int targetGridPos = playerGridPos + dir * distance;
                Vector3 worldPos = GameManager.Instance.SpawnManager.GridGenerateScript.GridToWorldPosition(targetGridPos);
                Debug.Log(worldPos);

                if (GameManager.Instance.SpawnManager.EmptyTiles.Contains(worldPos))
                {
                    _carryingObject.transform.position = worldPos;
                    _carryingObject.GetComponent<Collider>().enabled = true;

                    GameManager.Instance.SpawnManager.GemObjects.Add(_carryingObject);
                    _carryingObject = null;
                    return;
                }
            }
        }
        _carryingObject.transform.position = transform.position + Vector3.up * 0.5f;
        _carryingObject.GetComponent<Collider>().enabled = true;
        GameManager.Instance.SpawnManager.GemObjects.Add(_carryingObject);
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
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);

        if (_knockBack.magnitude > 0.2f)
        {
            _charController.Move(_knockBack * Time.deltaTime);
            _knockBack = Vector3.Lerp(_knockBack,Vector3.zero, 5*Time.deltaTime);
        }
        else
        {
            _knockBack = Vector3.zero;
        }
    }
}
