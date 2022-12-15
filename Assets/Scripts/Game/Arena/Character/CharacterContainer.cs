using Cinemachine;
using Game.Lobby.Services;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Arena.Character
{
    public class CharacterContainer : MonoBehaviour
    {
        [field: SerializeField] public NetworkIdentity Identity { get; private set; }
        [field: SerializeField] public NetworkTransform Transform { get; private set; }
        [field: SerializeField] public PlayerInput Input { get; private set; }
        [field: SerializeField] public CameraController CameraController { get; private set; }
        [field: SerializeField] public CharacterMovement Movement { get; private set; }
        [field: SerializeField] public Invincibility Invincibility { get; private set; }
        [field: SerializeField] public ChargeAbility ChargeAbility { get; private set; }

        private RoomManager _roomManager;

        private void Start()
        {
            _roomManager = NetworkManager.singleton as RoomManager;
            _roomManager.SetupCharacter(this);
        }
    }
}