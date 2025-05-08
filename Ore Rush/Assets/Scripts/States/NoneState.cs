using Unity.VisualScripting;
using UnityEngine;
using static StateHandler;

public class NoneState : State
{
    public NoneState(StateHandler owner) : base(owner)
    {
    }
    public override void Update()
    {
        if (Input.GetButtonDown("Fire1") && Owner._currentPickaxeCooldown <= 0)
        {
            Owner.ChangeState(new MineState(Owner));
        }
    }
}
