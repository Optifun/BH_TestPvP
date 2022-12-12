using System.Linq;
using UnityEngine;

namespace Static
{
    public class LevelStaticData : MonoBehaviour
    {
        public Transform[] SpawnPoints;

        private void Reset()
        {
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(Globals.Tags.SpawnPoint);
            SpawnPoints = spawnPoints.Select(go => go.transform).ToArray();
        }
    }
}