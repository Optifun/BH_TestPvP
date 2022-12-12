namespace Game.State
{
    public class GameState
    {
        public GameState()
        {
            LobbyState = new LobbyState();
        }

        public LobbyState LobbyState { get; }
    }
}