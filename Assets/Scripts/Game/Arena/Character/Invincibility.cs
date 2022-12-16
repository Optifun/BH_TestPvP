using System.Collections;
using Mirror;
using UnityEngine;

namespace Game.Arena.Character
{
    public class Invincibility : NetworkBehaviour
    {
        [SerializeField] private MeshRenderer modelRenderer;
        [SerializeField] private CharacterMovement movement;

        [field: SyncVar] public bool IsInvincible { get; private set; }


        public void ApplyInvincibility(Vector3 pushImpulse, float duration)
        {
            if (!isServer) return;

            CmdSetInvincibility(true);
            RpcActivateInvincibility(pushImpulse, duration);
        }

        [ClientRpc]
        private void RpcActivateInvincibility(Vector3 pushImpulse, float duration)
        {
            if (isOwned)
                movement.AddImpulse(pushImpulse);

            StartCoroutine(ShowInvincibleState(duration));
        }

        [Command]
        private void CmdSetInvincibility(bool value)
        {
            IsInvincible = value;
        }

        private IEnumerator ShowInvincibleState(float duration)
        {
            var material = modelRenderer.material;
            var oldColor = material.color;

            material.color = Color.green;
            yield return new WaitForSeconds(duration);
            material.color = oldColor;

            if (isServer)
                CmdSetInvincibility(false);
        }
    }
}