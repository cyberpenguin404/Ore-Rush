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
        Owner.PickaxeAnimtor.SetBool("IsMining", true);
    }
    public override void Exit()
    {
        Debug.Log("Exited mine state");
        Owner.Mine(Owner.transform.forward);
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