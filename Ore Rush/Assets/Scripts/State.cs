using UnityEngine;

public abstract class State
{
    protected StateHandler Owner;

    public State(StateHandler owner)
    {
        this.Owner = owner;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}
