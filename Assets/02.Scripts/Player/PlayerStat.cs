using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    // 데이터의 단위를 일치 시켜야함 (예시는 초당 속성 값)
    [Header("Points")]
    public int MaxHealthPoint = 100;
    public float MaxStaminaPoint = 100f;
    public float StaminaRecovery = 10f;
    public float StaminaRecoveryDelay = 2f;

    [Header("Movement")]
    public float MoveSpeed = 7f;
    public float SprintSpeedFactor = 2f;
    public float JumpPower = 2.5f;
    public float RotationSpeed = 200f;
    public float SprintCost = 10f;
    public float JumpCost = 10f;

    [Header("Attack")]
    public float AttackSpeed = 1.2f;    // 초당 1.2번 공격할 수 있다
    public int AttackDamage = 20;
    public float AttackCost = 20f;

}
