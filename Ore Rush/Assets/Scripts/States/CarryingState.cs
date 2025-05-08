using System;
using UnityEngine;
using static StateHandler;

public class CarryingState : State
{

    public CarryingState(StateHandler owner) : base(owner)
    {
    }

    public override void Enter()
    {
    }
    public override void Exit()
    {
    }
    public override void Update()
    {
        Owner._carryingObject.transform.position = Owner.transform.position + Owner.CarryingOffset;
    }
}
