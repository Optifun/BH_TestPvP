﻿using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Lobby.Services;
using Mirror;
using Static;
using UnityEngine;

namespace Game.Arena.Character
{
    public class ChargeAbility : NetworkBehaviour
    {
        [field: SyncVar] public bool IsCharging { get; private set; }
        public event Action<ChargeAbility> Completed;
        public Vector3 ChargingDirection { get; private set; }

        [SerializeField] private CollisionDetector _detector;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private CharacterMovement _movement;
        [SerializeField] private Rigidbody _rigidbody;

        private ArenaManager _arenaManager;
        private ArenaStaticData _arenaStaticData;

        private float _chargeVelocity;
        private float _rigidbodyDrag;
        private RigidbodyParams _rigidbodyParams;
        private RigidbodyParams _chargeRigidbodyParams = new() {Drag = 0, Mass = 100, AngularDrag = 0};
        private Vector3 _chargePosition;
        private const float PushForce = 4;

        private void Awake() =>
            _detector.CollisionEnter += OnCollided;

        public void Initialize(ArenaManager manager, ArenaStaticData staticData)
        {
            _arenaStaticData = staticData;
            _arenaManager = manager;
        }

        public void TriggerCharge(Vector3 movementNormalized)
        {
            if (isLocalPlayer)
            {
                Debug.Log("Trigger charge");
                var distance = _arenaStaticData.ChargeDistance;
                var duration = _arenaStaticData.ChargingTime;
                CmdCharge(movementNormalized, distance, duration);
            }
        }

        private void FixedUpdate()
        {
            if (!IsCharging || !isOwned) return;

            _movement.SetVelocity(ChargingDirection * _chargeVelocity);
        }

        [Command]
        private void CmdCharge(Vector3 forward, float distance, float duration)
        {
            Debug.Log("Charging...");
            _movement.Move(Vector2.zero);
            IsCharging = true;
            ChargingDirection = forward.normalized;
            _chargePosition = transform.position;

            _chargeVelocity = distance / duration;
            _rigidbodyParams = new RigidbodyParams(_rigidbody);
            _chargeRigidbodyParams.Apply(_rigidbody);
            StartCoroutine(DisableCharge(duration));
        }


        private IEnumerator DisableCharge(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            _rigidbodyParams.Apply(_rigidbody);
            _movement.SetVelocity(Vector3.zero);

            var translation = transform.position - _chargePosition;
            Debug.Log($"Travelled distance = {translation.magnitude}, required {_chargeVelocity * duration}");

            IsCharging = false;
            ChargingDirection = Vector3.zero;
            Debug.Log("Charge completed");
            Completed?.Invoke(this);
        }

        private void OnCollided(GameObject _, Collision target)
        {
            if (IsCharging == false || !isServer)
                return;

            if (target.gameObject.TryGetComponent(out CharacterContainer container))
            {
                var pushImpulse = target.GetContact(0).normal * PushForce;
                var invincibilityDuration = _arenaStaticData.InvincibilityDuration;
                _arenaManager.CmdRegisterHit(netIdentity.netId, container.Identity.netId);
                container.Invincibility.ApplyInvincibility(pushImpulse, invincibilityDuration);
            }
        }

        private void OnDestroy() =>
            _detector.CollisionEnter += OnCollided;
    }
}