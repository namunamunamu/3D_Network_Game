

public interface IFSM
{
    string MachineName { get; set; }

    void SetInitialState<T>() where T : BaseState;
    bool ContainState<T>() where T : BaseState;
    bool IsCurrentState<T>() where T : BaseState;
    T GetCurrentState<T>() where T : BaseState;

    T GetState<T>() where T : BaseState;

    void AddState<T>() where T : BaseState, new();

    void ChangeState<T>() where T : BaseState;

    void RemoveState<T>() where T : BaseState;

    void RemoveAllStates();
}
