using System.Linq;
using Cinemachine;
using UnityEngine;

namespace Static
{
    public class LevelStaticData : MonoBehaviour
    {
        public Transform[] SpawnPoints;
        public Camera MainCamera;
        public CinemachineVirtualCamera ThirdPersonCamera;

        private void Reset()
        {
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(Globals.Tags.SpawnPoint);
            SpawnPoints = spawnPoints.Select(go => go.transform).ToArray();
        }
    }
}