

public class Item_Crystal : ItemObject
{
    public int Score = 10;

    protected override void ItemEffect()
    {
        base.ItemEffect();
        
        ScoreManager.Instance.AddScore(Score);
    }
}
