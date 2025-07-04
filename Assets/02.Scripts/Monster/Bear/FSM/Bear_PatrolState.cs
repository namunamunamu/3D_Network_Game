using UnityEngine;

public class Bear_PatrolState : Bear_BaseState
{
    Transform _currentPatrolPoint;

    public override void OnEnter()
    {
        base.OnEnter();

        Owner.Animator.SetBool("WalkForward", true);

        _currentPatrolPoint = GameManager.Instance.GetRandomPatrolPoint();

        Owner.NavMeshAgent.speed = Owner.Stat.MoveSpeed;
        Owner.NavMeshAgent.SetDestination(_currentPatrolPoint.position);
    }

    public override void Update()
    {
        base.Update();
        float patrolPointDistance = Vector3.Distance(new Vector3(_currentPatrolPoint.position.x, transform.position.y, _currentPatrolPoint.position.z), transform.position);
        if (patrolPointDistance < 1f)
        {
            Machine.ChangeState<Bear_IdleState>();
        }

        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);
        if (distance < Owner.Stat.FindDistance)
        {
            Machine.ChangeState<Bear_ChaseState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Owner.Animator.SetBool("WalkForward", false);
    }
}
