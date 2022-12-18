using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Game.Lobby.Services;
using Game.Lobby.View;
using Game.State;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
            _roomManager.PlayersReady += OnPlayersReady;
        }

        public void GotoConnection() =>
            _lobbyUI.GotoConnectionForm();

        public void HostGame()
        {
            _gameState.LobbyState.IsServer = true;
            _roomManager.StartHost();
            _lobbyUI.GotoLobby(true);
        }

        public void Connect(string host)
        {
            _gameState.LobbyState.IsServer = false;
            if (_gameState.LobbyState.Username == "") return;

            IPAddress ipAddress;
            if (host == "localhost" || host == "127.0.0.1")
                ipAddress = IPAddress.Loopback;
            else
                ipAddress = IPAddress.Parse(host);

            _roomManager.Connect(ipAddress);
            _lobbyUI.GotoLobby(false);
        }

        public void StartMatch() =>
            _roomManager.StartMatch();

        public void TogglePlayerReady() =>
            _localPlayer.CmdChangeReadyState(!_localPlayer.readyToBegin);

        public void SetUsername(string username) =>
            _gameState.LobbyState.Username = username;

        private void OnClientConnected(NetworkIdentity identity, RoomPlayer player)
        {
            _lobbyUI.DisplayPlayer(player);

            if (identity.isLocalPlayer)
            {
                _localPlayer = player;
                _lobbyUI.AttachLocalPlayer(_localPlayer);
            }
        }

        private void OnClientDisconnected(NetworkIdentity identity, RoomPlayer player) =>
            _lobbyUI.RemovePlayer(player);

        private void OnPlayersReady(bool value)
        {
            if (value)
                Debug.Log("All players are ready");
            _lobbyUI.ToggleStartMatchBtn(value);
        }

        public void Hide()
        {
            _lobbyUI.HideMenu();
        }
    }
}