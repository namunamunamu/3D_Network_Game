

public class Item_HPPosion : ItemObject
{
    public int HealAmount = 30;

    protected override void ItemEffect()
    {
        base.ItemEffect();

        _player.AddCurrentHP(HealAmount);
    }
}
