using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Lobby.Services;
using Game.Lobby.View;
using Game.State;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

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

            _roomManager.ClientEnterRoom += OnClientConnected;
            _roomManager.ClientExitRoom += OnClientDisconnected;
        }

        public void GotoConnection()
        {
            _lobbyUI.GotoConnectionForm();
        }

        public void HostGame()
        {
            _roomManager.StartHost();
            _lobbyUI.GotoLobby();
        }

        public void Connect(string host)
        {
            if (_gameState.LobbyState.Username == "") return;
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

        private void OnClientConnected(NetworkConnection conn, RoomPlayer player)
        {
            var roomPlayer = conn.identity.GetComponent<RoomPlayer>();
            _lobbyUI.DisplayPlayer(roomPlayer);

            if (NetworkClient.localPlayer == conn.identity)
            {
                _localPlayer = roomPlayer;
                _lobbyUI.AttachLocalPlayer(_localPlayer);
            }
        }

        private void OnClientDisconnected(NetworkConnection conn, RoomPlayer player) =>
            _lobbyUI.RemovePlayer(player);
    }
}