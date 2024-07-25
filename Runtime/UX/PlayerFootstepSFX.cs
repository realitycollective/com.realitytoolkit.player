// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Player.Rigs;
using UnityEngine;

namespace RealityToolkit.Player.UX
{
    /// <summary>
    /// Produces a footstep sound effect when the player rig moves.
    /// Attach to the <see cref="IPlayerRig"/> <see cref="GameObject"/>.
    /// </summary>
    [HelpURL(RealityToolkitRuntimePreferences.Toolkit_Docs_BaseUrl + "docs/category/player")]
    [RequireComponent(typeof(IPlayerRig))]
    public class PlayerFootstepSFX : MonoBehaviour
#if RTK_LOCOMOTION
        , Locomotion.ILocomotionServiceHandler
#endif
    {
        [SerializeField]
        private AudioSource audioSource = null;

        [SerializeField]
        private float stepSize = 1f;

        private IPlayerRig playerRig;
        private float previousStepTime;
        private Vector2 previousStepPosition;
        private const float timeout = .5f;

#if RTK_LOCOMOTION
        private Locomotion.ILocomotionService locomotionService;
#endif

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private async void Awake()
        {
            await ServiceManager.WaitUntilInitializedAsync();
            playerRig = GetComponent<IPlayerRig>();

#if RTK_LOCOMOTION
            locomotionService = ServiceManager.Instance.GetService<Locomotion.ILocomotionService>();
            locomotionService.Register(gameObject);
#endif
        }

#if RTK_LOCOMOTION
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
        public void OnMoving(Locomotion.LocomotionEventData eventData) => CheckStep();

        /// <inheritdoc />
        public void OnTeleportCanceled(Locomotion.LocomotionEventData eventData) { }

        /// <inheritdoc />
        public void OnTeleportCompleted(Locomotion.LocomotionEventData eventData) { }

        /// <inheritdoc />
        public void OnTeleportStarted(Locomotion.LocomotionEventData eventData) { }

        /// <inheritdoc />
        public void OnTeleportTargetRequested(Locomotion.LocomotionEventData eventData) { }
#else
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void LateUpdate() => CheckStep();
#endif

        private void CheckStep()
        {
            if (playerRig == null)
            {
                return;
            }

            var time = Time.time;
            var position = playerRig.RigTransform.position;
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
