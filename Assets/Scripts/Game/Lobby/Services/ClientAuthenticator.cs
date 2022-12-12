using System.Collections;
using System.Collections.Generic;
using Game.State;
using Mirror;
using UnityEngine;

namespace Game.Lobby.Services
{
    public class ClientAuthenticator : NetworkAuthenticator
    {
        public struct AuthRequest : NetworkMessage
        {
            public string Username;
        }

        public struct AuthResponse : NetworkMessage
        {
            public byte code;
            public string message;
        }

        private readonly HashSet<NetworkConnection> _connectionsPendingDisconnect = new();
        private readonly Dictionary<string, NetworkConnection> _successfullyConnected = new(6);
        private LobbyState _lobbyState;

        public void Initialize(LobbyState lobbyState)
        {
            _lobbyState = lobbyState;
        }

        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<AuthRequest>(OnAuthRequestMessage, false);
        }

        public override void OnStopServer()
        {
            NetworkServer.UnregisterHandler<AuthRequest>();
        }

        public override void OnStartClient()
        {
            NetworkClient.RegisterHandler<AuthResponse>(OnAuthResponseMessage, false);
        }

        public override void OnStopClient()
        {
            NetworkClient.UnregisterHandler<AuthResponse>();
        }

        public override void OnClientAuthenticate()
        {
            AuthRequest authRequestMessage = new AuthRequest
            {
                Username = _lobbyState.Username
            };

            NetworkClient.connection.Send(authRequestMessage);
        }

        public override void OnServerAuthenticate(NetworkConnectionToClient conn)
        {
        }

        public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequest msg)
        {
            if (_connectionsPendingDisconnect.Contains(conn)) return;

            if (msg.Username == "" || _successfullyConnected.ContainsKey(msg.Username))
            {
                _connectionsPendingDisconnect.Add(conn);

                AuthResponse errorResponse = new AuthResponse
                {
                    code = 201,
                    message = "Invalid or duplicated name"
                };

                conn.Send(errorResponse);

                conn.isAuthenticated = false;
                StartCoroutine(DelayedDisconnect(conn, 1f));
                return;
            }

            AuthResponse authResponse = new AuthResponse
            {
                code = 100,
                message = "Success"
            };

            conn.authenticationData = new AuthenticationData(msg.Username);
            conn.Send(authResponse);
            ServerAccept(conn);
        }

        public void OnAuthResponseMessage(AuthResponse msg)
        {
            if (msg.code != 100)
            {
                Debug.LogError($"Authentication Response: {msg.message}");
                ClientReject();
                return;
            }

            ClientAccept();
        }

        IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            ServerReject(conn);

            yield return null;

            _connectionsPendingDisconnect.Remove(conn);
        }
    }
}