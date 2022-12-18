using System;
using System.Collections;
using System.Collections.Generic;
using Game.Arena.Character;
using Static;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Arena
{
    public class ArenaUI : MonoBehaviour, IDisposable
    {
        [field: SerializeField] private Canvas _endGameCanvas;
        [field: SerializeField] private TMP_Text _winnerText;
        [field: SerializeField] private TMP_Text _timerText;
        private ArenaStaticData _arenaStaticData;
        private ArenaManager _arenaManager;

        private Dictionary<uint, PlayerScoreUI> _scores = new();

        public void Initialize(ArenaStaticData arenaStaticData, ArenaManager arenaManager)
        {
            _arenaStaticData = arenaStaticData;
            _arenaManager = arenaManager;
            _arenaManager.HitRegistered += UpdatePlayerScore;
        }

        public void ShowWinner(CharacterContainer player, int score, int countDownSeconds)
        {
            _endGameCanvas.enabled = true;
            _winnerText.text = $"Winner: {player.RoomPlayer.Username}, Score: {score}";
            StartCoroutine(ShowCountDown(countDownSeconds));
        }

        private IEnumerator ShowCountDown(int countDownSeconds)
        {
            int left = countDownSeconds;
            while (left >= 0)
            {
                _timerText.text = left.ToString();
                yield return new WaitForSecondsRealtime(1);
                left--;
            }
        }

        public void DisplayPlayerScore(CharacterContainer container)
        {
            var scoreGO = Object.Instantiate(_arenaStaticData.PlayerScorePrefab, container.ScorePosition);
            var scoreUI = scoreGO.GetComponent<PlayerScoreUI>();
            scoreGO.transform.rotation = Quaternion.Euler(0, 180, 0);
            scoreUI.SetUsername(container.RoomPlayer.Username);
            scoreUI.SetScore(0);

            Debug.Log($"UI attached to {container.netId}");
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