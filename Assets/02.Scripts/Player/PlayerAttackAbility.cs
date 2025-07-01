using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;

public class PlayerAttackAbility : PlayerAbility
{
    private float _attackCoolDown;
    private float _timer;

    public Collider WeaponCollider;


    protected override void Init()
    {
        _attackCoolDown = 1f / _owner.Stat.AttackSpeed;
        WeaponCollider.enabled = false;
    }

    // - '위치/회전' 처럼 상시로 확인이 필요한 데이터 동기화: IPunObservable(OnPhotonSerializeView)
    // - '트리거/공격/피격' 처럼 간헐적으로 특정한 이벤트가 발생했을 때의 변화된 데이터 동기화: RPC
    // 
    // RPC: Remote Procedure Call
    //      ㄴ물리적으로 떨어져 있는 다른 디바이스의 함수를 호출하는 기능
    //      ㄴRPC 함수를 호출하면 네트워크를 통해 다른 사용자의 스크립트에서 해당 함수가 호출된다.

    private void Update()
    {
        if (!_photonView.IsMine || _owner.State == EPlayerState.Dead) return;

        _timer += Time.deltaTime;
        if (_timer >= _attackCoolDown)
        {
            _timer = _attackCoolDown;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && _timer >= _attackCoolDown)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (_owner.CurrentSP < 0)
        {
            return;
        }

        // 1. 일반 메서드 호출 방식
        // PlayAttackAnimation(Random.Range(1, 4));

        // 2. RPC 메서드 호출 방식
        _photonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, UnityEngine.Random.Range(1, 4));

        _owner.AddCurrentSP(-_owner.Stat.AttackCost);
        _timer = 0;
    }

    public void ActiveCollider()
    {
        WeaponCollider.enabled = true;
    }

    public void DeactiveCollider()
    {
        WeaponCollider.enabled = false;
    }

    [PunRPC]
    private void PlayAttackAnimation(int randomInt)
    {
        _animator.SetTrigger($"Attack{randomInt}");
    }

    public void Hit(Collider other)
    {
        if (_photonView.IsMine == false || other.GetComponent<IDamageable>() == null)
        {
            return;
        }

        DeactiveCollider();

        // RPC로 호출해야지 다른 사람의 게임 오브젝트들도 이 함수가 실행된다.
        // damageableObject.Damaged(_owner.Stat.AttackDamage);

        PhotonView otherPhoton = other.GetComponent<PhotonView>();
        otherPhoton.RPC(nameof(Player.Damaged), RpcTarget.AllBuffered, _owner.Stat.AttackDamage);
    }
}
