using Photon.Pun;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public float Interval;
    private float _intervalTimer = 0;
    public float Range;

    private void Start()
    {
        Interval = Random.Range(5f, 20f);
        Range = Random.Range(1f, 10f);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        _intervalTimer += Time.deltaTime;

        if (_intervalTimer >= Interval)
        {
            _intervalTimer = 0;

            Vector3 randomPosition = transform.position + Random.insideUnitSphere * Range;
            randomPosition.y = 3f;

            ItemObjectFactory.Instnace.RequestCreate(EItemType.Score, randomPosition);
        }
    }
}
