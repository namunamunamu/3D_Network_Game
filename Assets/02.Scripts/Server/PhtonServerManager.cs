using UnityEngine;

// Photon API 네임스페이스
using Photon.Pun;


public class PhtonServerManager : MonoBehaviourPunCallbacks // MonoBehaviourPunCallbacks: 유니티 이벤트 말고도 PUN 서버 이벤트를 받을 수 있다.
{
    public static PhtonServerManager _instance;


    private readonly string _gameVersion = "1.0.0";
    // private string _nickname = "Ken";

    public EPlayerType PlayerType = EPlayerType.Male;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 설정
        // 0. 데이터 송수신 빈도를 매 초당 30회로 설정한다.(기본은 10 | 선호하는 값이며 보장은 X )
        PhotonNetwork.SendRate = 60;   // 데이터 전송 빈도
        PhotonNetwork.SerializationRate = 60;   // 데이터 직렬화 빈도

        // 1. 버전: 버전이 다르면 다른 서버로 접속이 된다.
        PhotonNetwork.GameVersion = _gameVersion;

        // 2. 닉네임: 게임에서 사용할 사용자의 별명(중복 가능 => 유저 판별을 위해서 ActorID를 사용)
        // PhotonNetwork.NickName = _nickname;

        // 방장이 로드한 씬으로 다른 참여자가 똑같이 이동하게끔 동기화 해주는 옵션
        // 방장: 방을 만든 소유자이자 "마스터 클라이언트" (방마다 한명의 마스터 클라이언트가 존재)
        PhotonNetwork.AutomaticallySyncScene = true;

        // 설정 값들을 이용해 서버 접속 시도
        // 네임서버 접속 -> 방 목록이 있는 마스터 서버까지 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤서버에 접속 후 호출되는 콜백(이벤트) 함수
    public override void OnConnected()
    {
        base.OnConnected();

        Debug.Log("네임 서버 접속 완료");
        Debug.Log(PhotonNetwork.CloudRegion);
    }

    // 포톤 마스터 서버에 접속하면 호출되는 함수
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("마스터 서버 접속 완료");
        Debug.Log($"InLobby: {PhotonNetwork.InLobby}");   // 로비 입장 여부

        // 디폴트 로비(채널)입장
        PhotonNetwork.JoinLobby();
        // PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    // 로비에 접속하면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("로비(채널) 입장 완료");
        Debug.Log($"InLobby: {PhotonNetwork.InLobby}"); // 로비 입장 여부

        // 랜덤한 방에 들어간다.
        // PhotonNetwork.JoinRandomRoom();
    }

    // 방에 입장한 후 호출되는 함수
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log($"방 입장 완료: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"플레이어: {PhotonNetwork.CurrentRoom.PlayerCount} 명");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleScene");
        }

        // // 룸에 접속한 사용자 정보
        // Dictionary<int, Photon.Realtime.Player> roomPlayer = PhotonNetwork.CurrentRoom.Players;
        // foreach (KeyValuePair<int, Photon.Realtime.Player> player in roomPlayer)
        // {
        //     Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber} || {player.Value.UserId}");
        //     // ActorNumber는 Room 안에서의 플레이어에 대한 판별 ID
        //     // UserID는 진짜 플레이어의 고유ID => 친구 기능, 귓속말 등등....사용
        // }
    }

    // 방 입장에 실패하면 호출되는 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        Debug.Log($"랜덤 방 입장에 실패하였습니다: {returnCode} | {message}");
        return;
    }

    // Room 생성에 실패하였을때 호출되는 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        Debug.Log($"룸 생성에 실패하였습니다: {returnCode} | {message}");
    }

    // Room 생성에 성공하였을때 호출되는 함수
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log($"룸 생성에 성공하였습니다: {PhotonNetwork.CurrentRoom.Name}");
    }
}
