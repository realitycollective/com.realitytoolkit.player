// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Player.Definitions;
using RealityToolkit.Player.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Player
{
    /// <summary>
    /// The Reality Toolkit's default implementation of the <see cref="IPlayerService"/>.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("5C656EE3-FE7C-4FB3-B3EE-DF3FC0D0973D")]
    public class PlayerService : BaseServiceWithConstructor, IPlayerService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The service display name.</param>
        /// <param name="priority">The service initialization priority.</param>
        /// <param name="profile">The service configuration profile.</param>
        public PlayerService(string name, uint priority, PlayerServiceProfile profile)
            : base(name, priority)
        {
            rigPrefab = profile.RigPrefab;
            resetPlayerToOrigin = profile.ResetPlayerToOrigin;
        }

        private readonly GameObject rigPrefab;
        private readonly bool resetPlayerToOrigin;

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public IPlayerRig PlayerRig { get; private set; }

        /// <inheritdoc />
        public IPlayerRigServiceModule PlayerRigServiceModule { get; private set; }

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
            var PlayerServiceModules = ServiceManager.Instance.GetServices<IPlayerRigServiceModule>();
            Debug.Assert(PlayerServiceModules.Count > 0, $"There must be an active {nameof(IPlayerRigServiceModule)}. Please check your {nameof(PlayerServiceProfile)} configuration.");
            Debug.Assert(PlayerServiceModules.Count < 2, $"There should only ever be one active {nameof(IPlayerRigServiceModule)}. Please check your {nameof(PlayerServiceProfile)} configuration.");
            PlayerRigServiceModule = PlayerServiceModules[0];

            EnsurePlayerRigSetup();
        }

        /// <inheritdoc />
        public override void Start()
        {
            EnsurePlayerRigSetup();
        }

        private void EnsurePlayerRigSetup()
        {
            // If we don't have a rig reference yet...
            if (PlayerRig == null)
            {
                // We first try and lookup an existing rig in the scene...
                if (Camera.main.IsNotNull())
                {
                    PlayerRig = Camera.main.transform.root.GetComponentInChildren<IPlayerRig>();
                    if (PlayerRig == null)
                    {
                        Debug.LogWarning($"There is an existing main {nameof(Camera)} in the scene but it is not parented under a {nameof(IPlayerRig)} object as required by the {GetType().Name} to work." +
                            $" The existing camera is replaced with the {nameof(IPlayerRig)} prefab configured in the {nameof(PlayerServiceProfile)} of {GetType().Name}.");
                        Camera.main.gameObject.Destroy();
                    }
                }

                // If we still don't have a rig, then and only then we create a new rig instance.
                if (PlayerRig == null)
                {
                    if (rigPrefab.IsNotNull())
                    {
#if UNITY_EDITOR
                        if (Application.isPlaying)
                        {
                            PlayerRig = Object.Instantiate(rigPrefab).GetComponentInChildren<IPlayerRig>(true);
                        }
                        else
                        {
                            var go = UnityEditor.PrefabUtility.InstantiatePrefab(rigPrefab) as GameObject;
                            PlayerRig = go.GetComponentInChildren<IPlayerRig>(true);
                        }
#else
                        PlayerRig = Object.Instantiate(rigPrefab).GetComponentInChildren<IPlayerRig>(true);
#endif
                    }
                    else
                    {
                        Debug.LogError($"Failed to instantiate player rig. There is no player rig prefab configured in the {nameof(PlayerServiceProfile)}.");
                    }
                }

                Debug.Assert(PlayerRig != null, $"Failed to set up player rig required by {GetType().Name}");
            }

            if (resetPlayerToOrigin)
            {
                PlayerRig.RigTransform.position = Vector3.zero;
                PlayerRig.CameraTransform.position = Vector3.zero;
            }
        }
    }
}