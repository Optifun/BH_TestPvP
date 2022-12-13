using Game.Arena.Character;
using Static;
using UnityEngine;

namespace Game.Arena
{
    public class CharacterFactory
    {
        private readonly LevelStaticData _levelStaticData;
        private readonly GameObject _playerPrefab;

        public CharacterFactory(LevelStaticData levelStaticData, GameObject playerPrefab)
        {
            _playerPrefab = playerPrefab;
            _levelStaticData = levelStaticData;
        }


        public CharacterContainer SpawnCharacter()
        {
            var point = PickRandomPoint();
            var go = Object.Instantiate(_playerPrefab, point.position + Vector3.up * 2, point.rotation);

            var container = go.GetComponent<CharacterContainer>();

            return container;
        }

        public CharacterContainer SetupCharacter(CharacterContainer container)
        {
            _levelStaticData.ThirdPersonCamera.Follow = container.transform;
            return container;
        }

        private Transform PickRandomPoint()
        {
            var id = Random.Range(0, _levelStaticData.SpawnPoints.Length);
            var spawnPoint = _levelStaticData.SpawnPoints[id];
            return spawnPoint;
        }
    }
}