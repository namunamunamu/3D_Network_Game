using TMPro;
using UnityEngine;
using Photon.Realtime;

public class UI_RoomInfo : MonoBehaviour
{
    public TextMeshProUGUI NameTextUI;
    public TextMeshProUGUI CountTextUI;

    private void Start()
    {
        RoomManager.Instance.OnRoomDataChanged += Refresh;
        
        Refresh();
    }

    private void Refresh()
    {
        Room room = RoomManager.Instance.Room;

        if (room == null) return;

        NameTextUI.text = room.Name;
        CountTextUI.text = $"{room.PlayerCount}/{room.MaxPlayers}";
    }

    public void OnClickExitButton()
    {
        Exit();
    }

    public void Exit()
    {

    }

}
