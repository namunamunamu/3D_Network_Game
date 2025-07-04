using UnityEngine;
using UnityEngine.AI;


public class Bear_ChaseState : Bear_BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();

        Owner.Animator.SetBool("Run Forward", true);

        Owner.NavMeshAgent.speed = Owner.Stat.SprintSpeed;
    }

    public override void Update()
    {
        base.Update();
        if (Owner.NavMeshAgent.enabled)
        {
            Owner.NavMeshAgent.SetDestination(Owner.Target.transform.position);
        }

        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        if (distance < Owner.Stat.AttackDistance && Owner.AttackTimer == Owner.AttackCoolDown)
        {
            Machine.ChangeState<Bear_AttackState>();
            return;
        }

        if (distance > Owner.Stat.FindDistance)
        {
            Machine.ChangeState<Bear_PatrolState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        Owner.Animator.SetBool("Run Forward", false);
    }

}
