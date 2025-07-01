using Photon.Pun;
using UnityEngine;

public class PlayerMoveAbility : PlayerAbility, IPunObservable
{
    private Vector3 _moveDir;
    private float _yVelocity;
    private const float GRAVITY = -9.8f;

    private Vector3 _receivedPosition = Vector3.zero;
    private Quaternion _receivedRotation = Quaternion.identity;


    protected override void Init()
    {
        _moveDir = Vector3.zero;
    }

    // 데이터 동기화를 위한 데이터 전송 및 수신 기능
    // stream : 서버에서 주고받을 데이터가 담겨있는 변수
    // info   : 송수신 성공/실패 여부에 대한 로그
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (_photonView.IsMine && stream.IsWriting)
        {
            // 데이터를 전송하는 상황 -> 데이터를 보내주면 되고,
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (!_photonView.IsMine && stream.IsReading)
        {
            // 데이터를 수신하는 상황 => 받은 데이터를 세팅하면 된다.
            _receivedPosition = (Vector3)stream.ReceiveNext();   // transform.position (보내준 순서대로 받는다)
            _receivedRotation = (Quaternion)stream.ReceiveNext();   // transform.rotation
        }
    }

    private void Update()
    {
        if (_owner.State == EPlayerState.Dead)
        {
            return;
        }
        
        if (!_photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * 20f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _receivedRotation, Time.deltaTime * 20f);

            return;
        }

        if (!_characterController.isGrounded)
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }

        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        _moveDir = new Vector3(h, 0, v);
        _moveDir.Normalize();
        _animator.SetFloat("Move", _moveDir.magnitude);
        _moveDir = Camera.main.transform.TransformDirection(_moveDir);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Sprint();
        }
        else
        {
            _animator.SetBool("IsRun", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
        {
            Jump();
        }

        _moveDir.y = _yVelocity;

        _characterController.Move(_moveDir * _owner.Stat.MoveSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (_owner.CurrentSP < 0)
        {
            return;
        }

        _yVelocity = _owner.Stat.JumpPower;
        _owner.AddCurrentSP(-_owner.Stat.JumpCost);
    }

    private void Sprint()
    {
        if (_owner.CurrentSP < 0)
        {
            _animator.SetBool("IsRun", false);
            return;
        }

        float staminaConsume = 1f / _owner.Stat.SprintCost;
        _moveDir *= _owner.Stat.SprintSpeedFactor;
        _animator.SetBool("IsRun", true);
        _owner.AddCurrentSP(-staminaConsume);
    }
}

