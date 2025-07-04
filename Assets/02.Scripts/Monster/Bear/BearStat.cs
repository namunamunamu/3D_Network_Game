using UnityEngine;

[SerializeField]
public class BearStat
{
    [Header("체력")]
    public int MaxHealthPoint = 250;

    [Header("이동")]
    public float MoveSpeed = 2.5f;
    public float SprintSpeed = 8.5f;
    public float ChaseSpeed = 15f;
    public float IdleTime = 3f;
    public float ChaseTime = 5f;

    [Header("공격")]
    public int AttackDamage = 30;
    public float AttackSpeed = 0.7f;

    [Header("범위")]
    public float FindDistance = 15f;
    public float AttackDistance = 3f;
}
