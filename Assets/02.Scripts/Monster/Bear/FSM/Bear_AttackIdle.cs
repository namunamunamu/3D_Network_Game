using UnityEngine;

public class Bear_AttackIdle : Bear_BaseState
{

    public override void Update()
    {
        base.Update();

        transform.LookAt(Owner.Target.transform);
        
        float distance = Vector3.Distance(Owner.transform.position, Owner.Target.transform.position);

        if (distance < Owner.Stat.AttackDistance && Owner.AttackTimer == Owner.AttackCoolDown)
        {
            Machine.ChangeState<Bear_AttackState>();
            return;
        }

        if (distance < Owner.Stat.FindDistance)
        {
            Machine.ChangeState<Bear_ChaseState>();
        }
        else
        {
            Machine.ChangeState<Bear_PatrolState>();
        }
    }
        
}
