using System.Collections;
using UnityEngine;

public class PlayerShakingAbility : PlayerAbility
{
    // 무엇을 어떤 힘으로 몇초동안 흔들것인가
    public Transform Target;
    public float Strength;
    public float Duration;

    public void Start()
    {
        _owner.OnHitEvent += Shake;
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTimed = 0f;

        // 초기값 저장
        Vector3 startPosition = Target.localPosition;

        while (elapsedTimed <= Duration)
        {
            elapsedTimed += Time.deltaTime;

            Vector3 randomPosition = Random.insideUnitSphere.normalized * Strength;
            randomPosition.y = startPosition.y;
            Target.localPosition = randomPosition;

            yield return null;
        }

        // 원위치로 이동
        Target.localPosition = startPosition;
    }
}
