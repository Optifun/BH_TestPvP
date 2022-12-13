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

            _roomManager.ClientConnected += OnClientConnected;
            _roomManager.ClientDisconnected += OnClientDisconnected;
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

        private void OnClientConnected(NetworkConnection connection, AuthenticationData _)
        {
            _lobbyUI.StartCoroutine(HandlePlayerConnection(connection));
        }

        private void OnClientDisconnected(NetworkConnection connection, AuthenticationData _) =>
            _lobbyUI.RemovePlayer(connection.identity.GetComponent<RoomPlayer>());

        private IEnumerator HandlePlayerConnection(NetworkConnection connection)
        {
            yield return WaitTillPlayerConnected(connection, 3);

            if (connection.identity == null)
            {
                yield break;
            }

            var roomPlayer = connection.identity.GetComponent<RoomPlayer>();
            _lobbyUI.DisplayPlayer(roomPlayer);

            if (NetworkClient.localPlayer == connection.identity)
            {
                _localPlayer = roomPlayer;
                _lobbyUI.AttachLocalPlayer(_localPlayer);
            }
        }

        private IEnumerator WaitTillPlayerConnected(NetworkConnection connection, float seconds)
        {
            var stopwatch = Stopwatch.StartNew();
            while (connection.identity == null && stopwatch.ElapsedMilliseconds / 1000f < seconds)
            {
                seconds--;
                yield return new WaitForEndOfFrame();
            }
            stopwatch.Stop();
        }
    }
}