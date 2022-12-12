using System;
using System.Collections.Generic;
using Mirror;

namespace Game.Lobby.Services
{
    public class RoomManager : NetworkRoomManager
    {
        public event Action<NetworkConnection, AuthenticationData> ClientConnected;
        public event Action<NetworkConnection, AuthenticationData> ClientDisconnected;
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

        public override void OnRoomServerPlayersReady()
        {
            PlayersReady?.Invoke();
        }
    }
}