// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Interfaces;
using RealityToolkit.CameraService.UX;
using UnityEngine;

namespace RealityToolkit.CameraService
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class XRPlayerHead : MonoBehaviour
    {
        [SerializeField]
        private SphereCollider sphereCollider;

        private XRPlayerController controller;
        private ICameraBoundsModule cameraBoundsModule;
        private CameraOutOfBoundsTrigger initialTrigger;

        /// <summary>
        /// The radius of the head measured in the object's local space.
        /// </summary>
        public float Radius => sphereCollider.radius;

        /// <inheritdoc />
        private async void Awake()
        {
            controller = GetComponentInParent<XRPlayerController>();
            if (controller.IsNull())
            {
                Debug.LogError($"{nameof(XRPlayerHead)}must be parented to {nameof(XRPlayerController)}.");
            }

            await ServiceManager.WaitUntilInitializedAsync();
            cameraBoundsModule = ServiceManager.Instance.GetService<ICameraBoundsModule>();
            cameraBoundsModule.CameraBackInBounds += CameraService_CameraBackInBounds;
        }

        /// <inheritdoc />
        private void OnDestroy()
        {
            if (cameraBoundsModule != null)
            {
                cameraBoundsModule.CameraBackInBounds -= CameraService_CameraBackInBounds;
            }
        }

        /// <inheritdoc />
        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent<CameraOutOfBoundsTrigger>(out var outOfBoundsTrigger) &&
                outOfBoundsTrigger.RaiseEvents &&
                initialTrigger.IsNull())
            {
                cameraBoundsModule.RaiseCameraOutOfBounds(1f, Vector3.zero);
                initialTrigger = outOfBoundsTrigger;
            }
        }

        /// <inheritdoc />
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<CameraOutOfBoundsTrigger>(out var outOfBoundsTrigger) &&
                outOfBoundsTrigger == initialTrigger)
            {
                cameraBoundsModule.RaiseCameraBackInBounds();
                initialTrigger = null;
            }
        }

        private void CameraService_CameraBackInBounds()
        {

        }

        //private void CheckCameraBounds()
        //{
        //    const float lowerThresholdFactor = .2f;

        //    var headPosition = transform.position;
        //    headPosition.y = 0f;
        //    var bodyPosition = BodyTransform.position;
        //    bodyPosition.y = 0f;

        //    var headToBodyOffset = Vector3.Distance(headPosition, bodyPosition);
        //    var severity = 0f;
        //    var cameraOutOfBoundsLowerThreshold = lowerThresholdFactor * Radius;
        //    var cameraOutOfBoundsUpperThreshold = Radius;

        //    if (headToBodyOffset >= cameraOutOfBoundsUpperThreshold)
        //    {
        //        severity = 1f;
        //    }
        //    else if (headToBodyOffset >= cameraOutOfBoundsLowerThreshold)
        //    {
        //        var range = cameraOutOfBoundsUpperThreshold - cameraOutOfBoundsLowerThreshold;
        //        var value = headToBodyOffset - cameraOutOfBoundsLowerThreshold;
        //        severity = value / range;
        //    }

        //    if (severity > 0f)
        //    {
        //        cameraService.RaiseCameraOutOfBounds(severity, (bodyPosition - headPosition).normalized);
        //    }
        //    else if (severity <= 0f && wasOutOfBounds)
        //    {
        //        cameraService.RaiseCameraBackInBounds();
        //    }

        //    wasOutOfBounds = severity > 0f;
        //}
    }
}
