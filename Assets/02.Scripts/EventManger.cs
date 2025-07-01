using System;
using System.Collections.Generic;

public static class EventManger
{
    private static readonly Dictionary<Type, Action<GameEvent>> _events = new Dictionary<Type, Action<GameEvent>>();
    private static readonly Dictionary<Delegate, Action<GameEvent>> _eventLoockups = new Dictionary<Delegate, Action<GameEvent>>();


    public static void AddListener<T>(Action<T> listener) where T : GameEvent
    {
        if (!_eventLoockups.ContainsKey(listener))
        {
            Action<GameEvent> newAction = (e) => listener((T)e);
            _eventLoockups[listener] = newAction;

            if (_events.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
            {
                _events[typeof(T)] = internalAction += newAction;
            }
            else
            {
                _events[typeof(T)] = newAction;
            }
        }
    }

    public static void RemoveListener<T>(Action<T> listener)
    {
        if (_eventLoockups.TryGetValue(listener, out var action))
        {
            if (_events.TryGetValue(typeof(T), out var internalAction))
            {
                internalAction -= action;
                if (internalAction == null)
                {
                    _events.Remove(typeof(T));
                }
                else
                {
                    _events[typeof(T)] = internalAction;
                }
            }
            _eventLoockups.Remove(listener);
        }
    }

    public static void Broadcast(GameEvent gameEvent)
    {
        if (_events.TryGetValue(gameEvent.GetType(), out var listeners))
        {
            listeners?.Invoke(gameEvent);
        }
    }

    public static void Clear()
    {
        _events.Clear();
        _eventLoockups.Clear();
    }
}
