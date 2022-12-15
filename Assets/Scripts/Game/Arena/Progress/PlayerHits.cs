namespace Game.Arena
{
    public struct PlayerHits
    {
        public uint NetId;
        public int HitCount;

        public PlayerHits(uint netId, int hits)
        {
            NetId = netId;
            HitCount = hits;
        }
    }
}