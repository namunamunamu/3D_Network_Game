
public interface IState
{
    public string StateName { get; }
    public IFSM Machine { get; }
    public string GetStateName();
    public void Initialize();
    public void OnEnter();
    public void OnExit();
}