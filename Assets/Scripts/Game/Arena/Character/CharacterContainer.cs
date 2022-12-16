using Game.Lobby.Services;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Arena.Character
{
    public class CharacterContainer : NetworkBehaviour
    {
        [SyncVar] public RoomPlayer RoomPlayer;

        [field: SerializeField] public NetworkIdentity Identity { get; private set; }
        [field: SerializeField] public NetworkTransform Transform { get; private set; }
        [field: SerializeField] public Transform ScorePosition { get; private set; }
        [field: SerializeField] public PlayerInput Input { get; private set; }
        [field: SerializeField] public CharacterComponent Character { get; private set; }
        [field: SerializeField] public CameraController CameraController { get; private set; }
        [field: SerializeField] public CharacterMovement Movement { get; private set; }
        [field: SerializeField] public CharacterRotation Rotation { get; private set; }
        [field: SerializeField] public Invincibility Invincibility { get; private set; }
        [field: SerializeField] public ChargeAbility ChargeAbility { get; private set; }

        private RoomManager _roomManager;

        public override void OnStartClient()
        {
            _roomManager = NetworkManager.singleton as RoomManager;
            if (_roomManager != null)
                _roomManager.SetupCharacter(this);
        }
    }
}