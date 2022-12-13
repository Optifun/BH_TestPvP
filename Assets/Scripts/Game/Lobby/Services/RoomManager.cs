using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Game.Lobby.Services
{
    public class RoomManager : NetworkRoomManager
    {
        public event Action<NetworkConnection, AuthenticationData> ClientConnected;
        public event Action<NetworkConnection, AuthenticationData> ClientDisconnected;
        public Action<NetworkConnection, RoomPlayer> ClientEnterRoom;
        public Action<NetworkConnection, RoomPlayer> ClientExitRoom;
        public event Action PlayersReady;

        public Dictionary<NetworkConnection, AuthenticationData> Clients { get; } = new();

        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            var data = conn.authenticationData as AuthenticationData;
            Clients.Add(conn, data);
            ClientConnected?.Invoke(conn, data);
        }

        public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
        {
            if (Clients.TryGetValue(conn, out var data))
                ClientDisconnected?.Invoke(conn, data);
        }

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
        {
            RoomPlayer roomPlayer = Instantiate(roomPlayerPrefab, Vector3.zero, Quaternion.identity) as RoomPlayer;
            if (roomPlayer == null)
            {
                Debug.LogError("Room prefab missing RoomPlayer script");
                return null;
            }

            var authData = conn.authenticationData as AuthenticationData;
            roomPlayer.Username = authData.Username;
            return roomPlayer.gameObject;
        }

        public override void OnRoomServerPlayersReady()
        {
            PlayersReady?.Invoke();
        }
    }
}