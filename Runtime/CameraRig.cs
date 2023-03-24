﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// The default <see cref="ICameraRig"/> implmentation.
    /// Use it as it is or use it as starting point for your own implementation.
    /// </summary>
    [System.Runtime.InteropServices.Guid("8E0EE4FC-C8A5-4B10-9FCA-EE55B6D421FF")]
    public class CameraRig : MonoBehaviour, ICameraRig
    {
        [SerializeField]
        private Transform rigTransform = null;

        [SerializeField]
        private Camera playerCamera = null;

        [SerializeField]
        private Transform bodyTransform = null;

        [SerializeField]
        private TrackedPoseDriver cameraPoseDriver = null;

        /// <inheritdoc />
        public GameObject GameObject => gameObject;

        /// <inheritdoc />
        public Transform RigTransform => rigTransform;

        /// <inheritdoc />
        public Camera PlayerCamera => playerCamera;

        /// <inheritdoc />
        public Transform CameraTransform => PlayerCamera == null ? null : PlayerCamera.transform;

        /// <inheritdoc />
        public Transform BodyTransform => bodyTransform;

        /// <inheritdoc />
        public TrackedPoseDriver CameraPoseDriver => cameraPoseDriver;

        /// <inheritdoc />
        public virtual bool IsStereoscopic => PlayerCamera.stereoEnabled;

        /// <inheritdoc />
        public virtual bool IsOpaque
        {
            get
            {
                if (!ServiceManager.IsActiveAndInitialized ||
                    !ServiceManager.Instance.TryGetService<ICameraService>(out var cameraService) ||
                    cameraService.DisplaySubsystem == null)
                {
                    // When no device is attached we are assuming the display
                    // device is the computer's display, which should be opaque.
                    return true;
                }

                return cameraService.DisplaySubsystem.displayOpaque;
            }
        }

        /// <summary>
        /// Called just before any of the update callbacks is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            if (CameraPoseDriver.IsNull())
            {
                cameraPoseDriver = PlayerCamera.gameObject.EnsureComponent<TrackedPoseDriver>();
                cameraPoseDriver.UseRelativeTransform = false;
            }

            if (ServiceManager.Instance != null &&
                ServiceManager.Instance.TryGetService<ICameraService>(out var cameraService)
                && CameraPoseDriver.IsNotNull())
            {
                switch (cameraService.CameraRigServiceModule.TrackingType)
                {
                    case TrackingType.SixDegreesOfFreedom:
                        CameraPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                        break;
                    case TrackingType.ThreeDegreesOfFreedom:
                        CameraPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
                        break;
                    case TrackingType.Auto:
                    default:
                        // For now, leave whatever the user has configured manually on the component. Once we
                        // have APIs in place to query platform capabilities, we might use that for auto.
                        break;
                }
            }
        }
    }
}