using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "Arena Config", fileName = "ArenaConfig", order = 0)]
    public class ArenaStaticData : ScriptableObject
    {
        [field: Tooltip("Charging distance"), SerializeField]
        public float ChargeDistance { get; private set; }

        [field: Tooltip("Duration of charging state"), SerializeField]
        public float ChargingTime { get; private set; }

        [field: Tooltip("Duration of player invincible state"), SerializeField]
        public float InvincibilityDuration { get; private set; }

        [field: Tooltip("Amount of required hits for each player"), SerializeField]
        public float RequireHitCount { get; private set; }

        [field: Tooltip("Delay before next match"), SerializeField]
        public float MatchReloadDelay { get; private set; }

        public GameObject PlayerScorePrefab;
    }
}