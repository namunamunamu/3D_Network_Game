

using UnityEngine;

public class Bear_DeadState : Bear_BaseState
{
    float _timer;

    public override void OnEnter()
    {
        base.OnEnter();

        Owner.NavMeshAgent.enabled = false;
        Owner.Animator.SetBool("Death", true);
    }

    public override void Update()
    {
        base.Update();

        _timer += Time.deltaTime;
        if (_timer >= 30)
        {
            Machine.ChangeState<Bear_IdleState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _timer = 0;
        Owner.Revive();
        Owner.NavMeshAgent.enabled = true;
        Owner.Animator.SetBool("Death", false);
    }
}
