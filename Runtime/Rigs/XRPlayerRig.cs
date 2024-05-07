// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityCollective.Utilities.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

namespace RealityToolkit.Player.Rigs
{
    /// <summary>
    /// The default <see cref="IXRPlayerRig"/> implementation.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [System.Runtime.InteropServices.Guid("968e05fa-de5d-4502-9123-8fb83fdea695")]
    public class XRPlayerRig : PlayerRig, IXRPlayerRig
#if RTK_LOCOMOTION
        , Locomotion.ILocomotionTarget
#endif
    {
        [SerializeField]
        private TrackedPoseDriver poseDriver = null;

        [SerializeField]
        [Tooltip("Sets the type of tracking origin to use for this Rig. Tracking origins identify where 0,0,0 is in the world of tracking.")]
        private TrackingOriginModeFlags trackingOriginMode = TrackingOriginModeFlags.Device;

        [Range(0f, 3f)]
        [SerializeField]
        [Tooltip("The default vertical camera offset on the player rig. Used until tracking sensors provider a tracked value for the first time.")]
        private float defaultVerticalOffset = 1.6f;

        private bool trackingOriginInitialized = false;
        private bool trackingOriginInitializing = false;
        private static List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();

        /// <inheritdoc />
        public TrackedPoseDriver PoseDriver => poseDriver;

        /// <inheritdoc />
        public virtual bool IsStereoscopic => RigCamera.stereoEnabled;

        /// <inheritdoc />
        public virtual bool IsOpaque
        {
            get
            {
                if (PlayerService.DisplaySubsystem == null)
                {
                    // When no device is attached we are assuming the display
                    // device is the computer's display, which should be opaque.
                    return true;
                }

                return PlayerService.DisplaySubsystem.displayOpaque;
            }
        }

#if RTK_LOCOMOTION
        /// <inheritdoc />
        public Pose Pose => new Pose(RigTransform.position, RigTransform.rotation);
#endif

        /// <inheritdoc />
        protected override async void Start()
        {
            base.Start();

            await ServiceManager.WaitUntilInitializedAsync();

            if (PoseDriver.IsNull())
            {
                poseDriver = RigCamera.gameObject.EnsureComponent<TrackedPoseDriver>();
                poseDriver.UseRelativeTransform = false;
            }

            if (PoseDriver.IsNotNull())
            {
                switch (PlayerService.PlayerRigServiceModule.TrackingType)
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

#if RTK_LOCOMOTION
            if (ServiceManager.Instance.TryGetService<Locomotion.ILocomotionService>(out var locomotionService))
            {
                locomotionService.LocomotionTarget = this;
            }
#endif
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        protected virtual void OnValidate()
        {
            ResetRig();
        }

        protected virtual void UpdatePlayerHeightOffset(float heightOffset = 0f)
        {
            CameraTransform.localPosition = new Vector3(
                CameraTransform.localPosition.x,
                heightOffset,
                CameraTransform.localPosition.z);
        }

        private bool SetupTrackingOrigin()
        {
#if UNITY_2023_2_OR_NEWER
            SubsystemManager.GetSubsystems(inputSubsystems);
#else
            SubsystemManager.GetInstances(inputSubsystems);
#endif

            // We assume the tracking mode to be set, that way
            // when in editor and no subsystems are connected / running
            // we can still keep going and assume everything is ready.
            var trackingOriginModeSet = true;

            if (inputSubsystems.Count != 0)
            {
                for (int i = 0; i < inputSubsystems.Count; i++)
                {
#if UNITY_2023_2_OR_NEWER
                    if (string.Equals(inputSubsystems[i].subsystemDescriptor.id, "MockHMD Head Tracking"))
#else
                    if (string.Equals(inputSubsystems[i].SubsystemDescriptor.id, "MockHMD Head Tracking"))
#endif
                    {
                        UpdatePlayerHeightOffset(defaultVerticalOffset);
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
                UpdatePlayerHeightOffset(defaultVerticalOffset);
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
                    Debug.LogWarning("Attempting to set the player service tracking origin to device, but the device does not support it.");
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

            UpdatePlayerHeightOffset(trackingOriginMode == TrackingOriginModeFlags.Device ? defaultVerticalOffset : 0.0f);
            ResetRig();
        }

        /// <inheritdoc />
        protected override void ResetRig()
        {
            base.ResetRig();
            CameraTransform.position = IsStereoscopic ? Vector3.zero : new Vector3(0f, defaultVerticalOffset, 0f);
        }
    }
}
