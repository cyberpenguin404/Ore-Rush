using System;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerHandler : MonoBehaviour
{
    private State _currentState;

    public string PlayerName;

    public int PlayerIndex;
    public GameObject _carryingObject { get; private set; }

    public int Score;

    private TextMeshProUGUI ScoreText => PlayerIndex == 1 ? GameManager.Instance.ScoreTextPlayer1 : GameManager.Instance.ScoreTextPlayer2;

    private TextMeshProUGUI PickaxeCooldownText => PlayerIndex == 1 ? GameManager.Instance.PickaxeCooldownText1 : GameManager.Instance.PickaxeCooldownText2;
    [HideInInspector]
    public TextMeshProUGUI DynamiteCooldownText => PlayerIndex == 1 ? GameManager.Instance.DynamiteCooldownText1 : GameManager.Instance.DynamiteCooldownText2;
    [SerializeField]
    private float PickaxeCooldown;

    [Header: Materials]
    [SerializeField]
    private Material player1Material;
    [SerializeField]
    private Material player2Material;

    public float _currentPickaxeCooldown { get; private set; }

    [field: SerializeField] public Vector3 CarryingOffset { get; private set; }
    [field: SerializeField] public float mineRange { get; private set; }
    [field: SerializeField] public float PickaxeStunTime { get; private set; }

    
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
        _currentState.Update();
        HandleCooldowns();
        HandleCarrying();
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
        ChangeState(new MineState(this));
    }

    internal void Mine()
    {

        Debug.Log("Mine called");
        if (_currentPickaxeCooldown > 0)
        {
            Debug.Log("Cooldown");
            return;
        }


        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, mineRange))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                _currentPickaxeCooldown = PickaxeCooldown;
                Destroy(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("Not right tag");
            }
        }
        else
        {
            Debug.Log("Nothing found");
        }
    }
}
