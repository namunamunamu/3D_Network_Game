
public class BearStateMachine : BaseStateMachine<Bear>
{
    public override void AddStates()
    {
        AddState<Bear_IdleState>();
        AddState<Bear_AttackIdle>();
        AddState<Bear_PatrolState>();
        AddState<Bear_ChaseState>();
        AddState<Bear_AttackState>();
        AddState<Bear_DeadState>();

        SetInitialState<Bear_IdleState>();
    }
}
