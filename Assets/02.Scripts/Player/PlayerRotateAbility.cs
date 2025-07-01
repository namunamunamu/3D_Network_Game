using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerRotateAbility : PlayerAbility
{
    // 목표: 마우스를 조작하면 캐릭터/카메라를 그 방향으로 회전시키고 싶다
    public Transform CameraRoot;
    private CinemachineCamera _followCamera;


    // 마우스 입력값을 누적할 변수
    private float _mx;
    private float _my;

    protected override void Init()
    {
        if (_photonView.IsMine)
        {
            _followCamera = GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineCamera>();
            _followCamera.Follow = CameraRoot;
        }

        _owner.OnHitEvent += ShakeCamera;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;

        // 1. 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        // 2. 회전 방향 결정하기
        // 3. 회전하기

        _mx += mouseX * _owner.Stat.RotationSpeed * Time.deltaTime;
        _my += mouseY * _owner.Stat.RotationSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -90f, 90f);

        // y축 회전은 캐릭터만 한다.
        transform.eulerAngles = new Vector3(0f, _mx, 0f);

        // x축 회전은 캐릭터는 하지 않는다(카메라만 x축으로 회전)
        CameraRoot.localEulerAngles = new Vector3(-_my, 0f, 0f);
    }

    private void ShakeCamera()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        StartCoroutine(CameraShakeCoroutine(0.2f));
    }

    private IEnumerator CameraShakeCoroutine(float time)
    {
        _followCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise).enabled = true;
        yield return new WaitForSeconds(time);
        _followCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise).enabled = false;
    }
}
