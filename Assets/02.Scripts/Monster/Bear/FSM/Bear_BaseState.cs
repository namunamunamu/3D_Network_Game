

public class Bear_BaseState : BaseState
{
    public Bear Owner
    {
        get
        {
            return ((BearStateMachine)Machine).Owner;
        }
    }

    public virtual void Update()
    {
        if (Owner.CurrentHP <= 0 && !Machine.IsCurrentState<Bear_DeadState>())
        {
            Machine.ChangeState<Bear_DeadState>();
            return;
        }
    }
}
