using Unity.VisualScripting;
using UnityEngine;
using static PlayerHandler;

public class NoneState : State
{
    public NoneState(PlayerHandler owner) : base(owner)
    {
    }
    public override void Update()
    {
    }
}
