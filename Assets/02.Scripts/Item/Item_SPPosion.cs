

public class Item_SPPosion : ItemObject
{
    public int RecoveryAmount = 50;

    protected override void ItemEffect()
    {
        base.ItemEffect();

        _player.AddCurrentSP(RecoveryAmount);
    }
}
