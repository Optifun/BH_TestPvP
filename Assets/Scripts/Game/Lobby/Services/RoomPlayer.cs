﻿using System;
using Mirror;

namespace Game.Lobby.Services
{
    public class RoomPlayer : NetworkRoomPlayer
    {
        public event EventHandler<string> OnUsernameChanged;
        public event EventHandler<bool> OnReadyChanged;

        [SyncVar(hook = nameof(UsernameUpdatedCallback))]
        public string Username;

        private void UsernameUpdatedCallback(string oldValue, string newValue) =>
            OnUsernameChanged?.Invoke(this, newValue);

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState) =>
            OnReadyChanged?.Invoke(this, newReadyState);

        [Command]
        public void CmdSetUsername(string value)
        {
            Username = value;
            OnUsernameChanged?.Invoke(this, value);
        }
    }
}