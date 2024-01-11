// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.PlayerService.Interfaces;
using RealityToolkit.PlayerService.UX;
using UnityEngine;

namespace RealityToolkit.PlayerService
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class XRPlayerHead : MonoBehaviour
    {
        [SerializeField, Tooltip("The sphere collider wrapping the player's head.")]
        private SphereCollider sphereCollider;

        [SerializeField, Tooltip("The distance the head is allowed to move out of bounds before it is considered severely out of bounds.")]
        private float maxSeverityDistanceThreshold = .2f;

        private XRPlayerController controller;
        private IPlayerBoundsModule playerBoundsModule;
        private PlayerOutOfBoundsTrigger initialTrigger;
        private Vector3 enterPosition;

        /// <summary>
        /// The radius of the head measured in the object's local space.
        /// </summary>
        public float Radius => sphereCollider.radius;

        /// <inheritdoc />
        protected virtual async void Awake()
        {
            controller = GetComponentInParent<XRPlayerController>();
            if (controller.IsNull())
            {
                Debug.LogError($"{nameof(XRPlayerHead)} must be parented to {nameof(XRPlayerController)}.");
            }

            await ServiceManager.WaitUntilInitializedAsync();

            if (ServiceManager.Instance.TryGetService(out playerBoundsModule))
            {
                playerBoundsModule.PlayerBackInBounds += PlayerService_PlayerBackInBounds;
            }
        }

        /// <inheritdoc />
        protected virtual void OnDestroy()
        {
            if (playerBoundsModule != null)
            {
                playerBoundsModule.PlayerBackInBounds -= PlayerService_PlayerBackInBounds;
            }
        }

        /// <inheritdoc />
        private void OnTriggerEnter(Collider other)
        {
            if (playerBoundsModule == null)
            {
                return;
            }

            if (initialTrigger.IsNull() &&
                other.TryGetComponent<PlayerOutOfBoundsTrigger>(out var outOfBoundsTrigger) &&
                outOfBoundsTrigger.RaiseEvents)
            {
                initialTrigger = outOfBoundsTrigger;
                enterPosition = transform.position;
            }
        }

        /// <inheritdoc />
        protected virtual void OnTriggerStay(Collider other)
        {
            if (playerBoundsModule == null)
            {
                return;
            }

            if (initialTrigger.IsNotNull() &&
                initialTrigger.RaiseEvents)
            {
                var distance = Vector3.Distance(enterPosition, transform.position);
                var severity = Mathf.Clamp01(distance / maxSeverityDistanceThreshold);
                var direction = (enterPosition - transform.position).normalized;

                playerBoundsModule.RaisePlayerOutOfBounds(severity, direction);
            }
        }

        /// <inheritdoc />
        protected virtual void OnTriggerExit(Collider other)
        {
            if (playerBoundsModule == null)
            {
                return;
            }

            if (other.TryGetComponent<PlayerOutOfBoundsTrigger>(out var outOfBoundsTrigger) &&
                outOfBoundsTrigger == initialTrigger)
            {
                playerBoundsModule.RaisePlayerBackInBounds();
                initialTrigger = null;
            }
        }

        protected virtual void PlayerService_PlayerBackInBounds()
        {
            // We may have been force moved back into bounds externally, so we
            // we got to reset internal state.
            initialTrigger = null;
        }
    }
}
