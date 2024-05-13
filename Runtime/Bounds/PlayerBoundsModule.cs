// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Modules;
using RealityCollective.Utilities.Extensions;
using RealityToolkit.Player.Rigs;
using UnityEngine;

namespace RealityToolkit.Player.Bounds
{
    /// <summary>
    /// Default implementation for <see cref="IPlayerBoundsModule"/>.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("a68682d3-3e9d-4d56-8a32-9805f58928f8")]
    public class PlayerBoundsModule : BaseServiceModule, IPlayerBoundsModule
    {
        /// <inheritdoc />
        public PlayerBoundsModule(string name, uint priority, PlayerBoundsModuleProfile profile, IPlayerService parentService)
            : base(name, priority, profile, parentService)
        {
            maxSeverityDistanceThreshold = profile.MaxSeverityDistanceThreshold;
        }

        private readonly float maxSeverityDistanceThreshold;
        private XRPlayerController playerRig;
        private const float returnToBoundsPoseOffset = .5f;
        private PlayerOutOfBoundsTrigger initialTrigger;
        private Vector3 enterPosition;

        /// <inheritdoc />
        public bool IsPlayerOutOfBounds { get; private set; }

        /// <inheritdoc />
        public Pose LastInBoundsPose { get; private set; }

        /// <inheritdoc />
        public event PlayerOutOfBoundsDelegate PlayerOutOfBounds;

        /// <inheritdoc />
        public event PlayerBackInBoundsDelegate PlayerBackInBounds;

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            playerRig = (ParentService as IPlayerService).PlayerRig as XRPlayerController;
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (IsPlayerOutOfBounds)
            {
                return;
            }

            var position = playerRig.CameraTransform.position;
            position.y = playerRig.RigTransform.position.y;
            LastInBoundsPose = new Pose(position, playerRig.RigTransform.rotation);
        }

        /// <inheritdoc />
        public void ResetPlayerIntoBounds()
        {
            if (!IsPlayerOutOfBounds)
            {
                return;
            }

            var position = LastInBoundsPose.position;
            var direction = playerRig.CameraTransform.position - position;
            direction.y = 0f;
            direction.Normalize();
            position -= returnToBoundsPoseOffset * direction;

            playerRig.SetPositionAndRotation(position, Quaternion.identity);
            RaisePlayerBackInBounds();
        }

        /// <inheritdoc />
        public void OnTriggerEnter(PlayerOutOfBoundsTrigger trigger)
        {
            if (initialTrigger.IsNull())
            {
                initialTrigger = trigger;
                enterPosition = playerRig.Head.Pose.position;
            }
        }

        /// <inheritdoc />
        public void OnTriggerStay(PlayerOutOfBoundsTrigger trigger)
        {
            if (initialTrigger.IsNotNull())
            {
                var distance = Vector3.Distance(enterPosition, playerRig.Head.Pose.position);
                var severity = Mathf.Clamp01(distance / maxSeverityDistanceThreshold);
                var direction = (enterPosition - playerRig.Head.Pose.position).normalized;

                IsPlayerOutOfBounds = severity > 0f;
                if (IsPlayerOutOfBounds)
                {
                    PlayerOutOfBounds?.Invoke(severity, direction);
                }
            }
        }

        /// <inheritdoc />
        public void OnTriggerExit(PlayerOutOfBoundsTrigger trigger)
        {
            if (trigger == initialTrigger)
            {
                RaisePlayerBackInBounds();
                initialTrigger = null;
            }
        }

        private void RaisePlayerBackInBounds()
        {
            IsPlayerOutOfBounds = false;
            PlayerBackInBounds?.Invoke(false);
        }
    }
}
