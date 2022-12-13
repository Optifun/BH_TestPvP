using System;
using System.Collections.Generic;
using Game.Arena;
using Game.Arena.Character;
using Mirror;
using Static;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Lobby.Services
{
    public class RoomManager : NetworkRoomManager
    {
        public event Action<NetworkConnection, AuthenticationData> ClientConnected;
        public event Action<NetworkConnection, AuthenticationData> ClientDisconnected;
        public Action<NetworkConnection, RoomPlayer> ClientEnterRoom;
        public Action<NetworkConnection, RoomPlayer> ClientExitRoom;
        private CharacterFactory _characterFactory;
        private LevelStaticData _levelData;
        public event Action<bool> PlayersReady;

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

        public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
        {
            var container = _characterFactory.SpawnCharacter();
            NetworkServer.AddPlayerForConnection(conn, container.gameObject);
        }

        public void SetupCharacter(CharacterContainer container)
        {
            _characterFactory.SetupCharacter(container);
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            if (sceneName == GameplayScene)
            {
                _levelData = FindObjectOfType<LevelStaticData>();
                _characterFactory = new CharacterFactory(_levelData, playerPrefab);
            }
        }

        public override void OnRoomClientSceneChanged()
        {
            if (SceneManager.GetActiveScene().name == GameplayScene)
            {
                _levelData = FindObjectOfType<LevelStaticData>();
                _characterFactory = new CharacterFactory(_levelData, playerPrefab);
            }
        }

        public void SwitchToArena() =>
            ServerChangeScene(GameplayScene);

        public override void OnRoomServerPlayersReady() =>
            PlayersReady?.Invoke(true);

        public override void OnRoomServerPlayersNotReady() =>
            PlayersReady?.Invoke(false);
    }
}