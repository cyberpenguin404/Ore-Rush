using UnityEngine;

public class MineState : State
{
    private float _playerSpeed;

    private float _timer;

    public MineState(StateHandler owner) : base(owner)
    {
    }

    public override void Enter()
    {
        _playerSpeed = Owner.GetComponent<PlayerMovement>().PlayerSpeed;
        Owner.GetComponent<PlayerMovement>().PlayerSpeed = 0;
    }
    public override void Exit()
    {
        Owner.Mine();
        Owner.GetComponent<PlayerMovement>().PlayerSpeed = _playerSpeed;
    }
    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > Owner.PickaxeStunTime)
        {
            Owner.ChangeState(new NoneState(Owner));
        }
    }
}