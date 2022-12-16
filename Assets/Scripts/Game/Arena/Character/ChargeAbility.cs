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

        private ArenaManager _arenaManager;
        private ArenaStaticData _arenaStaticData;

        private float _chargeVelocity;
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
                var distance = _arenaStaticData.ChargeDistance;
                var duration = _arenaStaticData.ChargingTime;
                CmdCharge(movementNormalized, distance, duration);
            }
        }

        private void Update()
        {
            if (!IsCharging && !isOwned) return;

            _controller.Move(ChargingDirection * _chargeVelocity);
        }

        [Command]
        private void CmdCharge(Vector3 forward, float distance, float duration)
        {
            IsCharging = true;
            ChargingDirection = forward.normalized;
            _chargeVelocity = distance / duration;
            StartCoroutine(DisableCharge(duration));
        }


        private IEnumerator DisableCharge(float duration)
        {
            yield return new WaitForSeconds(duration);
            IsCharging = false;
            ChargingDirection = Vector3.zero;
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