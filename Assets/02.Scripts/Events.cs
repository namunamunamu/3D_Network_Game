public static class Events
{
    public static PlayerSpawnEvent PlayerSpawnEvent = new PlayerSpawnEvent();
}

public class PlayerSpawnEvent : GameEvent
{
    public Player Player;
}

public class GameEvent
{
}
