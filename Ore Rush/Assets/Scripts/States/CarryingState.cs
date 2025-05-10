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
        Owner._carryingObject.GetComponent<Collider>().enabled = false;
    }
    public override void Exit()
    {
        if (Owner._carryingObject != null)
        {
            Owner._carryingObject.GetComponent<Collider>().enabled = true;
        }
    }
    public override void Update()
    {
        Owner._carryingObject.transform.position = Owner.transform.position + Owner.CarryingOffset;
    }
}
