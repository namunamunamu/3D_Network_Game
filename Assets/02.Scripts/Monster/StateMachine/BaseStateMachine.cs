using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateMachine<OwnerType> : MonoBehaviour, IFSM where OwnerType : MonoBehaviour
{
    private OwnerType _owner;
    public OwnerType Owner => _owner;

    public string MachineName { get; set; }

    private Dictionary<Type, BaseState> _states = new Dictionary<Type, BaseState>();

    protected BaseState CurrentState { get; set; }
    protected BaseState InitialState { get; set; }
    protected BaseState NextState { get; set; }
    protected BaseState PreviousState { get; set; }

    public abstract void AddStates();

    public void AddState<T>() where T : BaseState, new()
    {
        if (ContainState<T>()) return;

        BaseState baseState = gameObject.AddComponent<T>();
        baseState.enabled = false;
        baseState.Machine = this;
        baseState.Initialize();

        _states.Add(typeof(T), baseState);
    }

    public void SetInitialState<T>() where T : BaseState
    {
        InitialState = _states[typeof(T)];
    }

    public virtual void Initialize()
    {
        if (string.IsNullOrEmpty(MachineName))
        {
            MachineName = GetType().Name;

            AddStates();

            CurrentState = InitialState;

            CurrentState.enabled = true;
            CurrentState.OnEnter();
        }
    }

    public virtual void Start()
    {
        _owner = gameObject.GetComponent<OwnerType>();
        Initialize();
    }

    public void ChangeState<T>() where T : BaseState
    {
        Type t = typeof(T);

        PreviousState = CurrentState;
        NextState = _states[t];

        CurrentState.OnExit();
        CurrentState.enabled = false;
        CurrentState = NextState;
        NextState = null;

        CurrentState.enabled = true;
        CurrentState.OnEnter();
    }

    public bool ContainState<T>() where T : BaseState
    {
        return _states.ContainsKey(typeof(T));
    }

    public T GetState<T>() where T : BaseState
    {
        return (T)_states[typeof(T)];
    }

    public bool IsCurrentState<T>() where T : BaseState
    {
        return (CurrentState.GetType() == typeof(T)) ? true : false;
    }

    public T GetCurrentState<T>() where T : BaseState
    {
        return (T)CurrentState;
    }

    public void RemoveState<T>() where T : BaseState
    {
        Type t = typeof(T);
        if (_states.ContainsKey(t))
        {
            _states.Remove(t);
        }
    }

    public void RemoveAllStates()
    {
        _states.Clear();
    }
}
