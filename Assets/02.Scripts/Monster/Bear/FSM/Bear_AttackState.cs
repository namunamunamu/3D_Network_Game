using UnityEngine;



public class Bear_AttackState : Bear_BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();

        Owner.Animator.SetTrigger($"Attack{Random.Range(1, 5)}");
    }

    public override void OnExit()
    {
        base.OnExit();
        Owner.AttackTimer = 0;
    }
}
