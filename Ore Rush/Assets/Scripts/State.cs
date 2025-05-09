using UnityEngine;

public abstract class State
{
    protected PlayerHandler Owner;

    public State(PlayerHandler owner)
    {
        this.Owner = owner;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}
