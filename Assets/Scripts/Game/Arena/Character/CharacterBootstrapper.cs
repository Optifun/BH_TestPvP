using Game.Lobby.Services;
using Mirror;
using UnityEngine;

namespace Game.Arena.Character
{
    public class CharacterBootstrapper : NetworkBehaviour
    {
        [SerializeField] private CharacterContainer container;
        private RoomManager _roomManager;

        public override void OnStartClient()
        {
            _roomManager = NetworkManager.singleton as RoomManager;
            if (_roomManager != null)
                _roomManager.SetupCharacter(container);
        }
    }
}