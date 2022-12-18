using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Arena.Character;
using Game.Arena.Progress;
using Game.Lobby.Services;
using Mirror;
using Static;
using UnityEngine;

namespace Game.Arena
{
    public class ArenaManager : NetworkBehaviour
    {
        public event Action<CharacterContainer, int> GameFinished;
        public event Action<CharacterContainer, int> HitRegistered;

        [field: SerializeField] private ArenaUI _arenaUI;

        private readonly SyncDictionary<uint, HitProgress> _playerHits = new();
        private Dictionary<uint, CharacterContainer> _characters = new();
        private ArenaStaticData _arenaStaticData;
        private LevelStaticData _levelStaticData;
        private RoomManager _roomManager;
        private CharacterContainer _localPlayer;

        public override void OnStartClient()
        {
            var roomManager = (RoomManager) NetworkManager.singleton;
            roomManager.OnClientArenaLoaded(this);
            GameFinished += ShowWinner;
        }

        public void Initialize(
            RoomManager roomManager,
            ArenaStaticData arenaStaticData,
            LevelStaticData levelStaticData)
        {
            _roomManager = roomManager;
            _levelStaticData = levelStaticData;
            _arenaStaticData = arenaStaticData;
            _arenaUI.Initialize(_arenaStaticData, this);
        }

        public void SetupPlayer(CharacterContainer container)
        {
            Debug.Log($"Setup player {container.netId}");
            _characters.Add(container.netId, container);
            var localPlayer = container.Identity.isLocalPlayer;
            if (localPlayer)
            {
                AttachCamera(container);
                InitializeLocalClient(container);
            }
            else
            {
                InitializeClient(container);
            }

            if (!localPlayer)
                AttachUI(container);

            if (isServer) FillProgress(container);
        }

        [Server]
        public void RegisterHit(uint gainerId, uint targetId)
        {
            var gainerIdentity = _characters[gainerId].Identity;
            var hitProgress = _playerHits[gainerId].Hits;
            var hitId = hitProgress.FindIndex(hits => hits.NetId == targetId);

            var playerHits = hitProgress[hitId];
            playerHits.HitCount++;
            hitProgress[hitId] = playerHits;
            _playerHits[gainerId] = new HitProgress {Hits = hitProgress};

            TargetIncrementHitRpc(gainerIdentity.connectionToClient, gainerId, targetId, playerHits.HitCount);
            CheckWinner(gainerId);
        }

        [TargetRpc]
        private void TargetIncrementHitRpc(NetworkConnection connection, uint gainerId, uint targetId, int score)
        {
            var playerContainer = _characters[targetId];
            HitRegistered?.Invoke(playerContainer, score);
        }

        [ClientRpc]
        private void FinishMatchRpc(CharacterContainer container, int score) =>
            GameFinished?.Invoke(container, score);


        private void CheckWinner(uint gainerId)
        {
            var hitsList = _playerHits[gainerId].Hits;
            if (hitsList.All(hit => hit.HitCount >= _arenaStaticData.RequireHitCount))
            {
                var score = hitsList.Sum(hits => hits.HitCount);
                Debug.Log($"Got winner [{gainerId}]:{score}");
                FinishMatchRpc(_characters[gainerId], score);
                StartCoroutine(RestartMatch(_arenaStaticData.MatchReloadDelay));
            }
        }

        private IEnumerator RestartMatch(float restartTime)
        {
            yield return new WaitForSecondsRealtime(restartTime);
            _roomManager.SwitchToArena();
        }

        private void AttachUI(CharacterContainer container) =>
            _arenaUI.DisplayPlayerScore(container);

        private void ShowWinner(CharacterContainer player, int score) =>
            _arenaUI.ShowWinner(player, score, _arenaStaticData.MatchReloadDelay);

        private void FillProgress(CharacterContainer container)
        {
            var playerIdentity = container.Identity;
            var selfProgress = new HitProgress {Hits = new List<PlayerHits>()};
            _playerHits.Add(playerIdentity.netId, selfProgress);
            foreach (var pair in _playerHits)
            {
                if (pair.Key == playerIdentity.netId) continue;

                var enemyProgress = pair.Value;
                var enemyId = pair.Key;

                enemyProgress.Hits.Add(new PlayerHits(playerIdentity.netId, 0));
                selfProgress.Hits.Add(new PlayerHits(enemyId, 0));
            }
        }

        private void InitializeClient(CharacterContainer container)
        {
            container.Character.enabled = false;
            container.Movement.enabled = true;
            container.Rotation.enabled = true;
            container.ChargeAbility.Initialize(this, _arenaStaticData);
        }

        private void InitializeLocalClient(CharacterContainer container)
        {
            _localPlayer = container;
            container.Input.enabled = true;
            container.CameraController.enabled = true;
            container.Movement.enabled = true;
            container.Rotation.enabled = true;

            container.Input.ActivateInput();
            container.ChargeAbility.Initialize(this, _arenaStaticData);
        }

        public void AttachCamera(CharacterContainer container) =>
            container.CameraController.AttachCamera(_levelStaticData.ThirdPersonCamera);
    }
}