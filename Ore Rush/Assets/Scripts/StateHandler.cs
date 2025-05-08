using System;
using TMPro;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    private State currentState;

    [SerializeField]
    private GameObject FrontOfPlayer;


    public GameObject _carryingObject { get; private set; }

    private int _score;

    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private TextMeshProUGUI _cooldownText;
    [SerializeField]
    private float PickaxeCooldown;

    public float _currentPickaxeCooldown { get; private set; }

    [field: SerializeField] public Vector3 CarryingOffset { get; private set; }
    [field: SerializeField] public float mineRange { get; private set; }
    [field: SerializeField] public float PickaxeStunTime { get; private set; }
    
    private void Start()
    {
        currentState = new NoneState(this);
    }
    void Update()
    {
        currentState.Update();
        HandleCooldowns();
    }

    private void HandleCooldowns()
    {
        if (_currentPickaxeCooldown > 0)
        {
            _cooldownText.text = "Pickaxe cooldown:" + _currentPickaxeCooldown.ToString();
            _currentPickaxeCooldown -= Time.deltaTime;
        }
    }

    public void ChangeState(State state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void PickUpGem(GameObject gem)
    {
        if (currentState.GetType() != typeof(NoneState)) return;
        _carryingObject = gem;
        ChangeState(new CarryingState(this));
    }

    internal void CollectGem()
    {
        if (currentState.GetType() == typeof(CarryingState))
        {
            _score++;
            _scoreText.text = "Score: " + _score;
            Destroy(_carryingObject);
            ChangeState(new NoneState(this));
        }
    }


    internal void Mine()
    {
        if (_currentPickaxeCooldown > 0)
        {
            return;
        }


        Ray ray = new Ray(FrontOfPlayer.transform.position, FrontOfPlayer.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, mineRange))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                _currentPickaxeCooldown = PickaxeCooldown;
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
