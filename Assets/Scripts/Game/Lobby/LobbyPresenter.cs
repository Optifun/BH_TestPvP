using System;
using System.Collections.Generic;
using Game.Lobby.Services;
using Game.Lobby.View;
using Game.State;
using Mirror;

namespace Game.Lobby
{
    public class LobbyPresenter
    {
        private readonly RoomManager _roomManager;
        private readonly LobbyUI _lobbyUI;
        private readonly GameState _gameState;
        private RoomPlayer _localPlayer;
        public List<NetworkRoomPlayer> Players => _roomManager.roomSlots;

        public LobbyPresenter(RoomManager roomManager, LobbyUI lobbyUI, GameState gameState)
        {
            _lobbyUI = lobbyUI;
            _roomManager = roomManager;
            _gameState = gameState;

            _roomManager.ClientConnected += OnClientConnected;
            _roomManager.ClientDisconnected += OnClientDisconnected;
        }

        public void GotoConnection()
        {
            _lobbyUI.ShowConnectionForm();
        }

        public void HostGame()
        {
            _roomManager.StartHost();
        }

        public void Connect(string host)
        {
            _roomManager.StartClient(new Uri(host));
        }

        public void ToggleReady()
        {
            _localPlayer.CmdChangeReadyState(!_localPlayer.readyToBegin);
        }

        public void SetUsername(string username)
        {
            _gameState.LobbyState.Username = username;
        }

        private void OnClientConnected(NetworkConnection connection, AuthenticationData _)
        {
            var roomPlayer = connection.identity.GetComponent<RoomPlayer>();
            _lobbyUI.DisplayPlayer(roomPlayer);

            if (NetworkClient.localPlayer == connection.identity)
            {
                _localPlayer = roomPlayer;
                _lobbyUI.AttachLocalPlayer(_localPlayer);
            }
        }

        private void OnClientDisconnected(NetworkConnection connection, AuthenticationData _) =>
            _lobbyUI.RemovePlayer(connection.identity.GetComponent<RoomPlayer>());
    }
}