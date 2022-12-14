using System;
using System.Collections.Generic;
using Game.Arena;
using Game.Arena.Character;
using Game.Lobby.View;
using Mirror;
using Static;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace Game.Lobby.Services
{
    public class RoomManager : NetworkRoomManager
    {
        public event Action<NetworkConnection, AuthenticationData> ClientConnected;
        public event Action<NetworkConnection, AuthenticationData> ClientDisconnected;
        public Action<NetworkConnection, RoomPlayer> ClientEnterRoom;
        public Action<NetworkConnection, RoomPlayer> ClientExitRoom;

        public ArenaManager ArenaManagerPrefab;
        private CharacterFactory _characterFactory;
        private LevelStaticData _levelData;
        private ArenaManager _arenaManager;
        private LobbyPresenter _lobbyPresenter;
        
        public event Action<bool> PlayersReady;

        public Dictionary<NetworkConnection, AuthenticationData> Clients { get; } = new();

        public void Initialize(LobbyPresenter lobbyPresenter)
        {
            _lobbyPresenter = lobbyPresenter;
        }

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

        public void SetupCharacter(CharacterContainer container) =>
            _arenaManager.SetupPlayer(container);

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            if (sceneName == GameplayScene)
            {
                _lobbyPresenter.Hide();
                _levelData = FindObjectOfType<LevelStaticData>();
                _characterFactory = new CharacterFactory(_levelData, playerPrefab);
                _arenaManager = GameObject.Instantiate(ArenaManagerPrefab);
                _arenaManager.Initialize(this, _characterFactory);
            }
        }

        public override void OnRoomClientSceneChanged()
        {
            if (SceneManager.GetActiveScene().name == GameplayScene)
            {
                _levelData = FindObjectOfType<LevelStaticData>();
                _characterFactory = new CharacterFactory(_levelData, playerPrefab);
                _arenaManager = FindObjectOfType<ArenaManager>();
                _arenaManager.Initialize(this, _characterFactory);
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