// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// The Reality Toolkit's default implementation of the <see cref="ICameraService"/>.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("5C656EE3-FE7C-4FB3-B3EE-DF3FC0D0973D")]
    public class CameraService : BaseServiceWithConstructor, ICameraService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The service display name.</param>
        /// <param name="priority">The service initialization priority.</param>
        /// <param name="profile">The service configuration profile.</param>
        public CameraService(string name, uint priority, CameraServiceProfile profile)
            : base(name, priority)
        {
            rigPrefab = profile.RigPrefab;
            resetCameraToOrigin = profile.ResetCameraToOrigin;
        }

        private readonly GameObject rigPrefab;
        private readonly bool resetCameraToOrigin;

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public ICameraRig CameraRig { get; private set; }

        /// <inheritdoc />
        public ICameraRigServiceModule CameraRigServiceModule { get; private set; }

        private static readonly List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        private static XRDisplaySubsystem displaySubsystem = null;
        /// <inheritdoc />
        public XRDisplaySubsystem DisplaySubsystem
        {
            get
            {
                if (displaySubsystem != null && displaySubsystem.running)
                {
                    return displaySubsystem;
                }

                displaySubsystem = null;

#if UNITY_2023_2_OR_NEWER
                SubsystemManager.GetSubsystems(xrDisplaySubsystems);
#else
                SubsystemManager.GetInstances(xrDisplaySubsystems);
#endif

                for (var i = 0; i < xrDisplaySubsystems.Count; i++)
                {
                    var xrDisplaySubsystem = xrDisplaySubsystems[i];
                    if (xrDisplaySubsystem.running)
                    {
                        displaySubsystem = xrDisplaySubsystem;
                        break;
                    }
                }

                return displaySubsystem;
            }
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            var cameraServiceModules = ServiceManager.Instance.GetServices<ICameraRigServiceModule>();
            Debug.Assert(cameraServiceModules.Count > 0, $"There must be an active {nameof(ICameraRigServiceModule)}. Please check your {nameof(CameraServiceProfile)} configuration.");
            Debug.Assert(cameraServiceModules.Count < 2, $"There should only ever be one active {nameof(ICameraRigServiceModule)}. Please check your {nameof(CameraServiceProfile)} configuration.");
            CameraRigServiceModule = cameraServiceModules[0];

            EnsureCameraRigSetup();
        }

        /// <inheritdoc />
        public override void Start()
        {
            EnsureCameraRigSetup();
        }

        private void EnsureCameraRigSetup()
        {
            // If we don't have a rig reference yet...
            if (CameraRig == null)
            {
                // We first try and lookup an existing rig in the scene...
                if (Camera.main.IsNotNull())
                {
                    CameraRig = Camera.main.transform.root.GetComponentInChildren<ICameraRig>();
                    if (CameraRig == null)
                    {
                        Debug.LogWarning($"There is an existing main {nameof(Camera)} in the scene but it is not parented under a {nameof(ICameraRig)} object as required by the {GetType().Name} to work." +
                            $" The existing camera is replaced with the {nameof(ICameraRig)} prefab configured in the {nameof(CameraServiceProfile)} of {GetType().Name}.");
                        Camera.main.gameObject.Destroy();
                    }
                }

                // If we still don't have a rig, then and only then we create a new rig instance.
                if (CameraRig == null)
                {
                    if (rigPrefab.IsNotNull())
                    {
#if UNITY_EDITOR
                        if (Application.isPlaying)
                        {
                            CameraRig = UnityEngine.Object.Instantiate(rigPrefab).GetComponent<ICameraRig>();
                        }
                        else
                        {
                            var go = UnityEditor.PrefabUtility.InstantiatePrefab(rigPrefab) as GameObject;
                            CameraRig = go.GetComponent<ICameraRig>();
                        }
#else
                    CameraRig = UnityEngine.Object.Instantiate(rigPrefab).GetComponent<ICameraRig>();
#endif
                    }
                    else
                    {
                        Debug.LogError($"Failed to instantiate camera rig. There is no camera rig prefab configured in the {nameof(CameraServiceProfile)}.");
                    }
                }

                Debug.Assert(CameraRig != null, $"Failed to set up camera rig required by {GetType().Name}");
            }

            if (resetCameraToOrigin)
            {
                CameraRig.RigTransform.position = Vector3.zero;
                CameraRig.CameraTransform.position = Vector3.zero;
            }
        }
    }
}