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

    public void AddCurrentHP(int amount)
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
            OnDead();
            OnDeadEvent?.Invoke();
        }

        CurrentHP += amount;
        OnHPChanged?.Invoke();
    }

    public void AddCurrentSP(float amount)
    {
        Debug.Log(amount);
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
    public void Damaged(int damage)
    {
        if (_state == EPlayerState.Dead)
        {
            return;
        }

        AddCurrentHP(-damage);
    }

    private void OnDead()
    {
        _state = EPlayerState.Dead;
        Animator.SetBool("IsDead", true);

        CharacterController.enabled = false;
        GetAbility<PlayerRotateAbility>().enabled = false;

        StartCoroutine(RespawnCoroutine());
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
