using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : MonoBehaviour
{
    public Slider HPSlider;
    public Slider SPSlider;

    private Player _player;

    public void Awake()
    {
        EventManger.AddListener<PlayerSpawnEvent>(OnPlayerSpawn);
    }

    public void OnPlayerSpawn(PlayerSpawnEvent gameEvent)
    {
        _player = gameEvent.Player;
        HPSlider.maxValue = _player.Stat.MaxHealthPoint;
        SPSlider.maxValue = _player.Stat.MaxStaminaPoint;

        _player.OnSPChanged += Refresh;
        _player.OnHPChanged += Refresh;

        Refresh();
    }


    public void Refresh()
    {
        Debug.Log("Refresh!!");
        HPSlider.value = _player.CurrentHP;
        SPSlider.value = _player.CurrentSP;
    }
}
