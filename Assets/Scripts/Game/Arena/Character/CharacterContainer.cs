using Game.Lobby.Services;
using Mirror;
using UnityEngine;

namespace Game.Arena.Character
{
    public class CharacterContainer : MonoBehaviour
    {
        [field: SerializeField] public NetworkIdentity Identity { get; private set; }
        [field: SerializeField] public NetworkTransform Transform { get; private set; }

        private RoomManager _roomManager;

        private void Start()
        {
            _roomManager = NetworkManager.singleton as RoomManager;
            _roomManager.SetupCharacter(this);
        }
    }
}