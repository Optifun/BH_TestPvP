namespace Game.State
{
    public class GameState
    {
        public static GameState Instance { get; } = new GameState();
        
        public GameState()
        {
            LobbyState = new LobbyState();
        }

        public LobbyState LobbyState { get; }
    }
}