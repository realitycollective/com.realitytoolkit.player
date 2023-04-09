// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

namespace RealityToolkit.CameraService
{
    public class TrackedCameraRig : CameraRig, ITrackedCameraRig
    {
        [SerializeField]
        private TrackedPoseDriver poseDriver = null;

        [SerializeField]
        [Tooltip("Sets the type of tracking origin to use for this Rig. Tracking origins identify where 0,0,0 is in the world of tracking.")]
        private TrackingOriginModeFlags trackingOriginMode = TrackingOriginModeFlags.Device;

        [Range(0f, 3f)]
        [SerializeField]
        [Tooltip("The default vertical camera offset on the rig. Used until tracking sensors provider a tracked value for the first time.")]
        private float defaultVerticalOffset = 1.6f;

        private bool trackingOriginInitialized = false;
        private bool trackingOriginInitializing = false;
        private static List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();

        /// <inheritdoc />
        public TrackedPoseDriver PoseDriver => poseDriver;

        protected virtual void Start()
        {
            if (PoseDriver.IsNull())
            {
                poseDriver = RigCamera.gameObject.EnsureComponent<TrackedPoseDriver>();
                poseDriver.UseRelativeTransform = false;
            }

            if (ServiceManager.Instance != null &&
                ServiceManager.Instance.TryGetService<ICameraService>(out var cameraService)
                && PoseDriver.IsNotNull())
            {
                switch (cameraService.CameraRigServiceModule.TrackingType)
                {
                    case TrackingType.SixDegreesOfFreedom:
                        PoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                        break;
                    case TrackingType.ThreeDegreesOfFreedom:
                        PoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
                        break;
                    case TrackingType.Auto:
                    default:
                        // For now, leave whatever the user has configured manually on the component. Once we
                        // have APIs in place to query platform capabilities, we might use that for auto.
                        break;
                }
            }

            // We attempt to initialize the camera tracking origin, which might
            // fail at this point if the subsytems are not ready, in which case,
            // we set a flag to keep trying.
            trackingOriginInitialized = SetupTrackingOrigin();
            trackingOriginInitializing = !trackingOriginInitialized;
        }

        protected virtual void Update()
        {
            // We keep trying to initialize the tracking origin,
            // until it worked, because at application launch the
            // subsytems might not be ready yet.
            if (trackingOriginInitializing && !trackingOriginInitialized)
            {
                trackingOriginInitialized = SetupTrackingOrigin();
                trackingOriginInitializing = !trackingOriginInitialized;
            }
        }

        protected virtual void OnValidate()
        {
            ResetRig();
        }

        protected virtual void UpdateCameraHeightOffset(float heightOffset = 0f)
        {
            CameraTransform.localPosition = new Vector3(
                CameraTransform.localPosition.x,
                heightOffset,
                CameraTransform.localPosition.z);
        }

        private bool SetupTrackingOrigin()
        {
            SubsystemManager.GetInstances(inputSubsystems);

            // We assume the tracking mode to be set, that way
            // when in editor and no subsystems are connected / running
            // we can still keep going and assume everything is ready.
            var trackingOriginModeSet = true;

            if (inputSubsystems.Count != 0)
            {
                for (int i = 0; i < inputSubsystems.Count; i++)
                {
                    if (inputSubsystems[i].SubsystemDescriptor.id == "MockHMD Head Tracking")
                    {
                        UpdateCameraHeightOffset(defaultVerticalOffset);
                    }
                    else
                    {
                        var result = SetupTrackingOrigin(inputSubsystems[i]);
                        if (result)
                        {
                            inputSubsystems[i].trackingOriginUpdated -= XRInputSubsystem_OnTrackingOriginUpdated;
                            inputSubsystems[i].trackingOriginUpdated += XRInputSubsystem_OnTrackingOriginUpdated;
                        }
                        trackingOriginModeSet &= result;
                    }
                }
            }
            else
            {
                // No subsystems available, we are probably running in editor without a device
                // connected, position the camera at the configured default offset.
                UpdateCameraHeightOffset(defaultVerticalOffset);
            }

            return trackingOriginModeSet;
        }

        private bool SetupTrackingOrigin(XRInputSubsystem subsystem)
        {
            if (subsystem == null)
            {
                return false;
            }

            var trackingOriginModeSet = false;
            var supportedModes = subsystem.GetSupportedTrackingOriginModes();
            var requestedMode = trackingOriginMode;

            if (requestedMode == TrackingOriginModeFlags.Floor)
            {
                if ((supportedModes & (TrackingOriginModeFlags.Floor | TrackingOriginModeFlags.Unknown)) == 0)
                {
                    Debug.LogWarning("Attempting to set the tracking origin to floor, but the device does not support it.");
                }
                else
                {
                    trackingOriginModeSet = subsystem.TrySetTrackingOriginMode(requestedMode);
                }
            }
            else if (requestedMode == TrackingOriginModeFlags.Device)
            {
                if ((supportedModes & (TrackingOriginModeFlags.Device | TrackingOriginModeFlags.Unknown)) == 0)
                {
                    Debug.LogWarning("Attempting to set the camera system tracking origin to device, but the device does not support it.");
                }
                else
                {
                    trackingOriginModeSet = subsystem.TrySetTrackingOriginMode(requestedMode) && subsystem.TryRecenter();
                }
            }

            if (trackingOriginModeSet)
            {
                UpdateTrackingOrigin(subsystem.GetTrackingOriginMode());
            }

            return trackingOriginModeSet;
        }

        private void XRInputSubsystem_OnTrackingOriginUpdated(XRInputSubsystem subsystem) => UpdateTrackingOrigin(subsystem.GetTrackingOriginMode());

        private void UpdateTrackingOrigin(TrackingOriginModeFlags trackingOriginModeFlags)
        {
            trackingOriginMode = trackingOriginModeFlags;

            UpdateCameraHeightOffset(trackingOriginMode == TrackingOriginModeFlags.Device ? defaultVerticalOffset : 0.0f);
            ResetRig();
        }

        protected override void ResetRig()
        {
            base.ResetRig();
            CameraTransform.position = IsStereoscopic ? Vector3.zero : new Vector3(0f, defaultVerticalOffset, 0f);
        }
    }
}
