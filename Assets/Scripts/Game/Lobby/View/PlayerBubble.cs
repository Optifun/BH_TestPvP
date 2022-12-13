using System;
using Game.Lobby.Services;
using TMPro;
using UnityEngine;

namespace Game.Lobby.View
{
    public class PlayerBubble : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private RoomPlayer _player;

        public void Initialize(RoomPlayer player)
        {
            _player = player;
            _player.OnReadyChanged += OnPlayerReadyChanged;
            _player.OnUsernameChanged += OnPlayerUsernameChanged;
        }

        private void OnPlayerUsernameChanged(RoomPlayer sender, string newName) =>
            _text.text = newName;

        private void OnPlayerReadyChanged(RoomPlayer sender, bool ready) =>
            _text.color = sender.readyToBegin ? Color.green : Color.red;


        private void OnDestroy()
        {
            _player.OnReadyChanged -= OnPlayerReadyChanged;
            _player.OnUsernameChanged -= OnPlayerUsernameChanged;
        }
    }
}