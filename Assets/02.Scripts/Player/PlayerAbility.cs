using Photon.Pun;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    protected Player _owner { get; private set; }
    protected PhotonView _photonView { get; private set; }
    protected Animator _animator { get; private set; }
    protected CharacterController _characterController { get; private set; }

    protected virtual void Awake()
    {
        _owner = GetComponent<Player>();
        _photonView = _owner.PhotonView;
        _animator = _owner.Animator;
        _characterController = _owner.CharacterController;

        Init();
    }

    protected virtual void Init()
    {

    }
}
