// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if RTK_LOCOMOTION
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
#endif

namespace RealityToolkit.Player.UX
{
    /// <summary>
    /// Produces a footstep sound effect when the player rig moves.
    /// Attach to the <see cref="Rigs.IPlayerRig"/> <see cref="GameObject"/>.
    /// </summary>
    public class PlayerFootstepSFX : MonoBehaviour
#if RTK_LOCOMOTION
        , ILocomotionServiceHandler
#endif
    {
        [SerializeField]
        private AudioSource audioSource = null;

        [SerializeField]
        private float stepSize = 1f;

        private float previousStepTime;
        private Vector2 previousStepPosition;
        private const float timeout = .5f;

#if RTK_LOCOMOTION
        private ILocomotionService locomotionService;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private async void Awake()
        {
            await ServiceManager.WaitUntilInitializedAsync();

            locomotionService = ServiceManager.Instance.GetService<ILocomotionService>();
            locomotionService.Register(gameObject);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnDestroy()
        {
            if (locomotionService != null)
            {
                locomotionService.Unregister(gameObject);
            }
        }

        /// <inheritdoc />
        public void OnMoving(LocomotionEventData eventData) => CheckStep();

        /// <inheritdoc />
        public void OnTeleportCanceled(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public void OnTeleportCompleted(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public void OnTeleportStarted(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public void OnTeleportTargetRequested(LocomotionEventData eventData) { }
#else
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void LateUpdate() => CheckStep();
#endif

        private void CheckStep()
        {
            var time = Time.time;
            var position = transform.position;
            var stepPosition = new Vector2(position.x, position.z);

            if (previousStepTime > 0f && time - previousStepTime > timeout)
            {
                previousStepTime = time;
                previousStepPosition = stepPosition;
                return;
            }

            var delta = (previousStepPosition - stepPosition).magnitude;
            if (delta > stepSize)
            {
                previousStepTime = time;
                previousStepPosition = stepPosition;
                PlaySFX();
            }
        }

        private void PlaySFX() => audioSource.Play();
    }
}
