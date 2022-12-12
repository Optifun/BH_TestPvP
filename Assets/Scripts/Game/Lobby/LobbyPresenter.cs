using System;
using Game.Lobby.Services;
using Game.Lobby.View;
using Game.State;

namespace Game.Lobby
{
    public class LobbyPresenter
    {
        private readonly RoomManager _roomManager;
        private readonly LobbyUI _lobbyUI;
        private readonly GameState _gameState;

        public LobbyPresenter(RoomManager roomManager, LobbyUI lobbyUI, GameState gameState)
        {
            _lobbyUI = lobbyUI;
            _roomManager = roomManager;
            _gameState = gameState;
        }

        public void GotoConnection()
        {
            _lobbyUI.ShowConnectionForm();
        }

        public void SetUsername(string username)
        {
            _gameState.LobbyState.Username = username;
        }

        public void Connect(string host)
        {
            _roomManager.StartClient(new Uri(host));
        }
    }
}