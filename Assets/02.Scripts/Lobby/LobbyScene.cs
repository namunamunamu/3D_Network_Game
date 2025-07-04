using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public enum EPlayerType
{
    Male,
    Female
}

public class LobbyScene : MonoBehaviour
{
    public TMP_InputField NicknameInputField;
    public TMP_InputField RoomName;

    public GameObject MalePlayer;
    public GameObject FemalePlayer;

    public EPlayerType SelectedPlayerType;

    public void OnClilckMaleButton()
    {
        MalePlayer.SetActive(true);
        FemalePlayer.SetActive(false);

        SelectedPlayerType = EPlayerType.Male;
    }

    public void OnClickFemaleButton()
    {
        FemalePlayer.SetActive(true);
        MalePlayer.SetActive(false);

        SelectedPlayerType = EPlayerType.Female;
    }


    // 방 만들기 함수를 호출
    public void OnClickMakeRoomButton()
    {
        MakeRoom();
    }

    private void MakeRoom()
    {
        string nickname = NicknameInputField.text;
        string roomName = RoomName.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomName))
        {
            return;
        }

        // 포톤에 닉네임 등록
        PhotonNetwork.NickName = nickname;

        PhtonServerManager._instance.PlayerType = SelectedPlayerType;

        // // Room 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;    // 룸 입장 가능한 최대 인원수
        roomOptions.IsOpen = true;      // 룸 입장 가능 여부
        roomOptions.IsVisible = true;   // 로비(채널) 룸 목록에 노출시킬지 여부

        // // Room 생성
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }


}
