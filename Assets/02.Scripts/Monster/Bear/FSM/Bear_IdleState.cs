using UnityEngine;


public class Bear_IdleState : Bear_BaseState
{
    private float _timer;

    public override void OnEnter()
    {
        base.OnEnter();
        Owner.Animator.SetBool("Idle", true);
    }

    public override void Update()
    {
        base.Update();
        _timer += Time.deltaTime;
        if (_timer >= Owner.Stat.IdleTime)
        {
            _timer = 0;
            Machine.ChangeState<Bear_PatrolState>();
            return;
        }

        if (Owner.Target == null) return;

        float distance = Vector3.Distance(transform.position, Owner.Target.transform.position);
        if (distance < Owner.Stat.AttackDistance && Owner.AttackTimer == Owner.AttackCoolDown)
        {
            Machine.ChangeState<Bear_AttackState>();
            return;
        }

        if (distance < Owner.Stat.FindDistance)
        {
            Machine.ChangeState<Bear_ChaseState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        Owner.Animator.SetBool("Idle", false);
    }
}
