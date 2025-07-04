using UnityEngine;

public abstract class BaseState : MonoBehaviour, IState
{
    public string StateName { get; internal set;}
    public IFSM Machine { get; internal set; }

    public virtual void Initialize()
    {
        if (string.IsNullOrEmpty(StateName))
        {
            StateName = GetType().Name;
        }
    }

    public virtual string GetStateName()
    {
        return StateName;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }
}
