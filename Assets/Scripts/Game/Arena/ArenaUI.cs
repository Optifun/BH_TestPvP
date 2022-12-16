using System;
using System.Collections.Generic;
using Game.Arena.Character;
using Static;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Arena
{
    public class ArenaUI : IDisposable
    {
        private readonly ArenaStaticData _arenaStaticData;
        private readonly ArenaManager _arenaManager;

        private readonly Dictionary<uint, PlayerScoreUI> _scores = new();

        public ArenaUI(ArenaStaticData arenaStaticData, ArenaManager arenaManager)
        {
            _arenaStaticData = arenaStaticData;
            _arenaManager = arenaManager;
            _arenaManager.HitRegistered += UpdatePlayerScore;
        }

        public void DisplayPlayerScore(CharacterContainer container)
        {
            var scoreGO = Object.Instantiate(_arenaStaticData.PlayerScorePrefab, container.ScorePosition);
            var scoreUI = scoreGO.GetComponent<PlayerScoreUI>();
            scoreGO.transform.rotation = Quaternion.Euler(0, 180, 0);
            scoreUI.SetUsername(container.RoomPlayer.Username);
            scoreUI.SetScore(0);

            _scores.Add(container.netId, scoreUI);
        }

        private void UpdatePlayerScore(CharacterContainer container, int score) =>
            _scores[container.netId].SetScore(score);

        public void Dispose()
        {
            _arenaManager.HitRegistered -= UpdatePlayerScore;
        }
    }
}