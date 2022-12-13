﻿using System;
using System.Collections.Generic;
using Game.Lobby.Services;
using Mirror;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Lobby.View
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private PlayerBubble _playerBubblePrefab;

        [SerializeField] private ScrollRect _playerBubblesContainer;
        [SerializeField] private ButtonContainer _readyButton;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _connectButton;
        [SerializeField] private TMP_InputField _addressInput;

        [SerializeField] private GameObject _connectionForm;

        private Dictionary<RoomPlayer, PlayerBubble> _playersList = new();
        private LobbyPresenter _presenter;
        private RoomPlayer _localPlayer;

        public void Initialize(LobbyPresenter presenter)
        {
            _presenter = presenter;
            _hostButton.onClick.AddListener(OnHostButtonClick);
            _connectButton.onClick.AddListener(OnConnectButtonClick);
            _readyButton.Button.onClick.AddListener(OnReadyClick);
        }

        private void OnReadyClick()
        {
            _presenter.ToggleReady();
        }

        public void ShowConnectionForm()
        {
            _connectionForm.SetActive(true);
        }

        public void DisplayPlayers(List<RoomPlayer> players)
        {
            ClearPlayersList();

            foreach (var roomPlayer in players)
                DisplayPlayer(roomPlayer);
        }


        public void DisplayPlayer(RoomPlayer player)
        {
            PlayerBubble playerBubble = SpawnPlayerBubble(player);
            _playersList.Add(player, playerBubble);
        }

        public void RemovePlayer(RoomPlayer player)
        {
            Destroy(_playersList[player].gameObject);
            _playersList.Remove(player);
        }

        public void AttachLocalPlayer(RoomPlayer roomPlayer)
        {
            _localPlayer = roomPlayer;
            _localPlayer.OnReadyChanged += OnLocalPlayerReadyChanged;
        }

        private void OnConnectButtonClick() =>
            _presenter.Connect(_addressInput.text);

        private void OnHostButtonClick() =>
            _presenter.HostGame();

        private void OnLocalPlayerReadyChanged(RoomPlayer _, bool ready) =>
            _readyButton.Text.text = ready ? "Unready" : "Ready";

        private PlayerBubble SpawnPlayerBubble(RoomPlayer player)
        {
            var playerBubble = Instantiate(_playerBubblePrefab, _playerBubblesContainer.content);
            playerBubble.Initialize(player);
            return playerBubble;
        }

        private void ClearPlayersList()
        {
            foreach (var pair in _playersList)
                Destroy(pair.Value.gameObject);
        }

        private void OnDestroy()
        {
            if (_localPlayer)
                _localPlayer.OnReadyChanged -= OnLocalPlayerReadyChanged;

            _hostButton.onClick.RemoveListener(OnHostButtonClick);
            _connectButton.onClick.RemoveListener(OnConnectButtonClick);
            _readyButton.Button.onClick.RemoveListener(OnReadyClick);
        }
    }
}