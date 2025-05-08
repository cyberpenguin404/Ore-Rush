using System;
using TMPro;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    private State currentState;

    public GameObject _carryingObject { get; private set; }

    private int _score;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [field: SerializeField] public Vector3 CarryingOffset { get; private set; }
private void Start()
    {
        currentState = new NoneState(this);
    }
    void Update()
    {
        currentState.Update();
    }

    public void ChangeState(State state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void PickUpGem(GameObject gem)
    {
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
}
