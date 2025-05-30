using UnityEngine;

public class MineState : State
{
    private float _playerSpeed;

    private Vector3 _direction;

    private float _timer;

    public MineState(PlayerHandler owner) : base(owner)
    {
    }

    public override void Enter()
    {
        _playerSpeed = Owner.PlayerSpeed;
        Owner.PlayerSpeed = 0;
        _direction = Owner.transform.forward;
        Owner._canTurn = false;
        Owner.PickaxeAnimtor.SetBool("IsMining", true);
    }
    public override void Exit()
    {
        Debug.Log("Exited mine state");
        Owner.Mine(_direction);
        Owner.PlayerSpeed = _playerSpeed;
        Owner._canTurn = true;
        Owner.PickaxeAnimtor.SetBool("IsMining", false);
    }
    public override void Update()
    {
        _timer += Time.deltaTime;
        Owner.PickaxeCooldownSlider.value = _timer / Owner.PickaxeStunTime;
        if (_timer > Owner.PickaxeStunTime)
        {
            Owner.ChangeState(new NoneState(Owner));
        }
    }
}