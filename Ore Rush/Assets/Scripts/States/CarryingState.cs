using System;
using UnityEngine;
using static PlayerHandler;

public class CarryingState : State
{

    public CarryingState(PlayerHandler owner) : base(owner)
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
