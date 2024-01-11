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
        private ICameraBoundsModule cameraBoundsModule;
        private CameraOutOfBoundsTrigger initialTrigger;
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

            if (ServiceManager.Instance.TryGetService(out cameraBoundsModule))
            {
                cameraBoundsModule.CameraBackInBounds += PlayerService_CameraBackInBounds;
            }
        }

        /// <inheritdoc />
        protected virtual void OnDestroy()
        {
            if (cameraBoundsModule != null)
            {
                cameraBoundsModule.CameraBackInBounds -= PlayerService_CameraBackInBounds;
            }
        }

        /// <inheritdoc />
        private void OnTriggerEnter(Collider other)
        {
            if (cameraBoundsModule == null)
            {
                return;
            }

            if (initialTrigger.IsNull() &&
                other.TryGetComponent<CameraOutOfBoundsTrigger>(out var outOfBoundsTrigger) &&
                outOfBoundsTrigger.RaiseEvents)
            {
                initialTrigger = outOfBoundsTrigger;
                enterPosition = transform.position;
            }
        }

        /// <inheritdoc />
        protected virtual void OnTriggerStay(Collider other)
        {
            if (cameraBoundsModule == null)
            {
                return;
            }

            if (initialTrigger.IsNotNull() &&
                initialTrigger.RaiseEvents)
            {
                var distance = Vector3.Distance(enterPosition, transform.position);
                var severity = Mathf.Clamp01(distance / maxSeverityDistanceThreshold);
                var direction = (enterPosition - transform.position).normalized;

                cameraBoundsModule.RaiseCameraOutOfBounds(severity, direction);
            }
        }

        /// <inheritdoc />
        protected virtual void OnTriggerExit(Collider other)
        {
            if (cameraBoundsModule == null)
            {
                return;
            }

            if (other.TryGetComponent<CameraOutOfBoundsTrigger>(out var outOfBoundsTrigger) &&
                outOfBoundsTrigger == initialTrigger)
            {
                cameraBoundsModule.RaiseCameraBackInBounds();
                initialTrigger = null;
            }
        }

        protected virtual void PlayerService_CameraBackInBounds()
        {
            // We may have been force moved back into bounds externally, so we
            // we got to reset internal state.
            initialTrigger = null;
        }
    }
}
