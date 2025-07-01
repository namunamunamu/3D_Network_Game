using UnityEngine.UI;

public class PlayerHealthBarAblility : PlayerAbility
{
    public Slider HealthSlider;

    protected override void Init()
    {
        if (_photonView.IsMine)
        {
            HealthSlider.gameObject.SetActive(false);
            return;
        }
        HealthSlider.maxValue = _owner.Stat.MaxHealthPoint;
        Refresh();
        _owner.OnHPChanged += Refresh;
    }

    public void Refresh()
    {
        if (_photonView.IsMine)
        {
            return;
        }

        HealthSlider.value = _owner.CurrentHP;
    }
}
