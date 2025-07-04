using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public enum EBearState
{
    Live,
    Dead
}

public class Bear : MonoBehaviour, IDamageable
{
    public Slider HPSlider;

    public GameObject HitVFX;
    public float VFXOffset;

    public EBearState State = EBearState.Live;

    public Collider LeftAttackCollider;
    public Collider RightAttackCollider;
    public Collider BiteAttackCollider;


    public BearStat Stat { get; private set; }

    public int CurrentHP { get; private set; }
    public Transform PatrolTransform { get; private set; }

    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public BearStateMachine FSM { get; private set; }
    private PhotonView _photonView;

    public float AttackTimer { get; set; }
    public float AttackCoolDown { get; private set; }

    public GameObject Target { get; private set; }

    private Vector3 _initPosition;


    private void Awake()
    {
        Stat = new BearStat();
        AttackCoolDown = 1f / Stat.AttackSpeed;
        Animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        FSM = GetComponent<BearStateMachine>();
        _photonView = GetComponent<PhotonView>();

        HPSlider.maxValue = Stat.MaxHealthPoint;

        Init();
    }

    private void Init()
    {
        CurrentHP = Stat.MaxHealthPoint;
    }

    private void Update()
    {
        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackCoolDown)
        {
            AttackTimer = AttackCoolDown;
        }

        FindTarget();

        HPSlider.value = CurrentHP;
    }

    public Transform GetPatrolPoint()
    {
        return GameManager.Instance.GetRandomPatrolPoint();
    }

    private void FindTarget()
    {
        StartCoroutine(FindTargetCoroutine());
    }

    private IEnumerator FindTargetCoroutine()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = 9999f;
        GameObject nearestPlayer = null;
        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlayer = player;
            }
        }

        if (nearestPlayer != null) Target = nearestPlayer;

        yield return new WaitForSeconds(1f);
    }

    public void ActiveCollider()
    {
        Debug.Log("Attack Activate");
        LeftAttackCollider.enabled = true;
        RightAttackCollider.enabled = true;
        BiteAttackCollider.enabled = true;
    }

    public void DeactiveCollider()
    {
        Debug.Log("Attack Deactivate");
        LeftAttackCollider.enabled = false;
        RightAttackCollider.enabled = false;
        BiteAttackCollider.enabled = false;
    }

    public void EndAttack()
    {
        Debug.Log("End Attack");
        FSM.ChangeState<Bear_AttackIdle>();
    }

    public void Revive()
    {
        CurrentHP = Stat.MaxHealthPoint;
        State = EBearState.Live;
    }


    [PunRPC]
    public void Damaged(int damage, int actorNumber)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0)
        {
            State = EBearState.Dead;
        }
    }

    public void Hit(Collider other)
    {
        if (_photonView.IsMine == false || other.GetComponent<IDamageable>() == null)
        {
            return;
        }
        DeactiveCollider();

        PhotonView otherPhoton = other.GetComponent<PhotonView>();


        if (other.tag == "Player")
        {
            otherPhoton.RPC(nameof(Player.Damaged), RpcTarget.AllBuffered, Stat.AttackDamage, 99);
        }
    }
}
