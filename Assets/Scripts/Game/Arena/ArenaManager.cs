﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game.Arena.Character;
using Game.Lobby.Services;
using Mirror;
using Static;

namespace Game.Arena
{
    public class ArenaManager : NetworkBehaviour
    {
        public event Action<NetworkIdentity, int> GameFinished;
        public event Action<CharacterContainer, int> HitRegistered;

        private SyncDictionary<uint, HitProgress> _playerHits = new();
        private Dictionary<uint, CharacterContainer> _characters = new();
        private ArenaStaticData _arenaStaticData;
        private LevelStaticData _levelStaticData;
        private RoomManager _roomManager;

        public override void OnStartClient()
        {
            var roomManager = (RoomManager) NetworkManager.singleton;
            roomManager.OnClientArenaLoaded(this);
        }

        public void Initialize(
            RoomManager roomManager,
            ArenaStaticData arenaStaticData,
            LevelStaticData levelStaticData)
        {
            _roomManager = roomManager;
            _levelStaticData = levelStaticData;
            _arenaStaticData = arenaStaticData;
        }

        public void SetupPlayer(CharacterContainer container)
        {
            _characters.Add(container.Identity.netId, container);
            if (container.Identity.isLocalPlayer)
            {
                AttachCamera(container);
                InitializeLocalClient(container);
            }
            else
            {
                InitializeClient(container);
            }

            if (isServer)
            {
                FillProgress(container);
            }
        }

        [Command]
        public void CmdRegisterHit(uint gainerId, uint targetId)
        {
            var hitProgress = _playerHits[gainerId].Hists;
            var hitId = hitProgress.FindIndex(hits => hits.NetId == targetId);

            var playerHits = hitProgress[hitId];
            playerHits.HitCount++;
            hitProgress[hitId] = playerHits;

            IncrementHitRpc(gainerId, targetId);
            CheckWinner(gainerId);
        }

        [TargetRpc]
        public void IncrementHitRpc(uint gainerId, uint targetId)
        {
        }


        private void CheckWinner(uint gainerId)
        {
            var hitsList = _playerHits[gainerId].Hists;
            if (hitsList.All(hit => hit.HitCount >= _arenaStaticData.RequireHitCount))
            {
                var score = hitsList.Sum(hits => hits.HitCount);
                GameFinished?.Invoke(_characters[gainerId].Identity, score);
            }
        }

        private void FillProgress(CharacterContainer container)
        {
            var playerIdentity = container.Identity;
            var selfProgress = new HitProgress();
            _playerHits.Add(playerIdentity.netId, selfProgress);
            foreach (var pair in _playerHits)
            {
                var enemyProgress = pair.Value;
                var enemyId = pair.Key;

                enemyProgress.Hists.Add(new PlayerHits(playerIdentity.netId, 0));
                selfProgress.Hists.Add(new PlayerHits(enemyId, 0));
            }
        }

        private void InitializeClient(CharacterContainer container)
        {
            container.Character.enabled = false;
            container.Movement.enabled = true;
            container.Rotation.enabled = true;
        }

        private void InitializeLocalClient(CharacterContainer container)
        {
            container.Input.enabled = true;
            container.CameraController.enabled = true;
            container.Movement.enabled = true;
            container.Rotation.enabled = true;

            container.Input.ActivateInput();
            container.ChargeAbility.Initialize(this, _arenaStaticData);
        }

        public void AttachCamera(CharacterContainer container)
        {
            container.CameraController.AttachCamera(_levelStaticData.ThirdPersonCamera);
        }
    }
}