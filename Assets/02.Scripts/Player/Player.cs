using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public enum EPlayerState
{
    Live,
    Dead
}

[RequireComponent(typeof(PlayerMoveAbility))]
public class Player : MonoBehaviour, IDamageable
{
    [Header("Current Status")]
    public int CurrentHP;
    public float CurrentSP;

    [Header("Stat Data")]
    public PlayerStat Stat;

    [Header("UI")]
    public GameObject PlayerIcon;
    public GameObject EnemyIcon;
    public Slider HealthSlider;

    public int Score = 0;


    [SerializeField] private bool _isSPComsumed = false;

    private EPlayerState _state = EPlayerState.Live;
    public EPlayerState State => _state;


    private Dictionary<Type, PlayerAbility> _cachedAbilities;
    public PhotonView PhotonView { get; private set; }
    public Animator Animator { get; private set; }
    public CharacterController CharacterController { get; private set; }

    [SerializeField] private float _spRecoveryTimer;
    // private bool _isDead = false;

    public event Action OnSPChanged;
    public event Action OnHPChanged;
    public event Action OnDeadEvent;
    public event Action OnHitEvent;


    private void Awake()
    {
        Stat = new PlayerStat();
        _cachedAbilities = new Dictionary<Type, PlayerAbility>();
        PhotonView = GetComponent<PhotonView>();
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();

        CurrentHP = Stat.MaxHealthPoint;
        CurrentSP = Stat.MaxStaminaPoint;

        if (!PhotonView.IsMine)
        {
            PlayerIcon.SetActive(false);
            EnemyIcon.SetActive(true);
            return;
        }

        PlayerIcon.SetActive(true);
        EnemyIcon.SetActive(false);

        PlayerSpawnEvent playerSpawnEvent = Events.PlayerSpawnEvent;
        playerSpawnEvent.Player = this;
        EventManger.Broadcast(playerSpawnEvent);
    }

    private void Update()
    {
        if (_isSPComsumed)
        {
            _spRecoveryTimer += Time.deltaTime;
            if (_spRecoveryTimer >= Stat.StaminaRecoveryDelay)
            {
                _isSPComsumed = false;
                _spRecoveryTimer = 0;
            }
        }
        else
        {
            RecoverySP();
        }
    }

    public void AddCurrentHP(int amount, int actorNumber = 0)
    {
        if (amount < 0)
        {
            OnHitEvent?.Invoke();
            if (CurrentHP > amount && PhotonView.IsMine)
            {
                PhotonView.RPC(nameof(PlayHitAnimation), RpcTarget.All);
            }
        }

        if (CurrentHP + amount > Stat.MaxHealthPoint)
        {
            CurrentHP = Stat.MaxHealthPoint;
        }
        else if (CurrentHP + amount <= 0)
        {
            CurrentHP = 0;
            OnDead(actorNumber);
        }

        CurrentHP += amount;
        OnHPChanged?.Invoke();
    }

    public void AddCurrentSP(float amount)
    {
        if (amount < 0)
        {
            _isSPComsumed = true;
        }

        if (CurrentSP + amount > Stat.MaxStaminaPoint)
        {
            CurrentSP = Stat.MaxStaminaPoint;
        }
        else if (CurrentSP + amount < 0)
        {
            CurrentSP = 0;
        }

        CurrentSP += amount;
        OnSPChanged?.Invoke();
    }

    private void RecoverySP()
    {
        if (CurrentSP >= Stat.MaxStaminaPoint)
        {
            CurrentSP = Stat.MaxStaminaPoint;
            return;
        }

        AddCurrentSP(1f / Stat.StaminaRecovery);
    }

    [PunRPC]
    public void Damaged(int damage, int actorNumber)
    {
        if (_state == EPlayerState.Dead)
        {
            return;
        }

        AddCurrentHP(-damage, actorNumber);
    }

    private void OnDead(int actorNumber)
    {
        _state = EPlayerState.Dead;
        Animator.SetBool("IsDead", true);

        CharacterController.enabled = false;
        GetAbility<PlayerRotateAbility>().enabled = false;

        if (PhotonView.IsMine)
        {
            MakeItems(UnityEngine.Random.Range(1, 4));
        }

        StartCoroutine(RespawnCoroutine());


        RoomManager.Instance.OnPlayerDeath(PhotonView.Owner.ActorNumber, actorNumber);
        OnDeadEvent?.Invoke();
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(5f);

        _state = EPlayerState.Live;
        Animator.SetBool("IsDead", false);

        if (PhotonView.IsMine)
        {
            PhotonView.RPC(nameof(Spawn), RpcTarget.All);            
        }

        AddCurrentHP(Stat.MaxHealthPoint * 2);
        AddCurrentSP(Stat.MaxStaminaPoint);

        CharacterController.enabled = true;
        GetAbility<PlayerRotateAbility>().enabled = true;
    }

    [PunRPC]
    private void Spawn()
    {
        int randomInt = UnityEngine.Random.Range(0, GameManager.Instance.SpawnPoints.Count);
        transform.position = GameManager.Instance.SpawnPoints[randomInt].position;
    }

    [PunRPC]
    public void PlayHitAnimation()
    {
        Animator.SetTrigger("Hit");
    }

    private void MakeItems(int count)
    {
        Vector3 dropPosition = transform.position + new Vector3(0, 2, 0);
        for (int i = 0; i < count; ++i)
        {
            // 포톤의 네트워크 객체의 생명 주기
            // Player   : 플레이어가 생성하고, 플레이어가 나가면 자동삭제(PhotonNetwork.Instantinate/Destroy)
            // Room     : 룸이 생성하고, 룸이 없어지면 삭제.. (PhotonNetwork.InstantinateRoomObject/Destroy)
            // PhotonNetwork.InstantiateRoomObject("Item", transform.position + new Vector3(0, 2, 0), Quaternion.identity, 0);

            ItemObjectFactory.Instnace.RequestCreate(EItemType.Score, dropPosition);
        }

        if (UnityEngine.Random.Range(0, 1) <= 0.3f)
        {
            ItemObjectFactory.Instnace.RequestCreate(EItemType.Stamina, dropPosition);
        }

        if (UnityEngine.Random.Range(0, 1) <= 0.2f)
        {
            ItemObjectFactory.Instnace.RequestCreate(EItemType.Health, dropPosition);
        }
    }



    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_cachedAbilities.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        // 게으른 초기화: 미리 초기화 하지 않고, 필요할 때 할당하여 초기화. => 메모리 절약가능 | 초기화 될 때 시간 리소스를 사용함
        ability = GetComponent<T>();
        if (ability != null)
        {
            _cachedAbilities.Add(type, ability);

            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }
}
