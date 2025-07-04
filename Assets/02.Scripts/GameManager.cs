using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private List<Transform> _spawnPoints;
    public List<Transform> SpawnPoints => _spawnPoints;

    [SerializeField] private List<Transform> _patrolPoints;
    public List<Transform> PatrolPoints => _patrolPoints;

    public Bear Bear;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Init();
    }

    private void Init()
    {

    }

    public Transform GetRandomPatrolPoint()
    {
        return _patrolPoints[Random.Range(0, _patrolPoints.Count)];
    }
}
