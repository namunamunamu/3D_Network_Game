using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;


    private Room _room;
    public Room Room => _room;

    public event Action OnRoomDataChanged;
    public event Action<string> OnPlayerEntered;
    public event Action<string> OnPlayerLeft;
    public event Action<string, string> OnPlayerDead;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public override void OnJoinedRoom()
    {
        GeneratePlayer();

        SetRoom();

        OnRoomDataChanged?.Invoke();
    }

    public void OnPlayerDeath(int actorNumber, int otherNumber)
    {
        string deadPlayerName = _room.Players[actorNumber].NickName + "_" + actorNumber;
        string attackPlayerName = _room.Players[otherNumber].NickName + "_" + otherNumber;

        OnPlayerDead?.Invoke(deadPlayerName, attackPlayerName);
    }

    // 새로운 플레이어가 방에 입장하면 자동으로 호출되는 함수
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        OnRoomDataChanged?.Invoke();

        string nickname = newPlayer.NickName;

        OnPlayerEntered?.Invoke(nickname + "_" + newPlayer.ActorNumber);
    }

    // 플레이어가 방에서 퇴장하면 자동으로 호출되는 함수
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        OnRoomDataChanged?.Invoke();

        string nickname = otherPlayer.NickName;

        OnPlayerLeft?.Invoke(nickname + "_" + otherPlayer.ActorNumber);
    }

    private void GeneratePlayer()
    {
        int randomInt = UnityEngine.Random.Range(0, GameManager.Instance.SpawnPoints.Count);
        Vector3 playerPosition = GameManager.Instance.SpawnPoints[randomInt].position;
        PhotonNetwork.Instantiate("Player", playerPosition, Quaternion.identity);
    }

    private void SetRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
    }
}
