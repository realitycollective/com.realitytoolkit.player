// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.PlayerService.Interfaces;
using System;
using UnityEngine;

namespace RealityToolkit.PlayerService.Modules
{
    /// <summary>
    /// Default implementation for <see cref="IPlayerBoundsModule"/>.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("a68682d3-3e9d-4d56-8a32-9805f58928f8")]
    public class PlayerBoundsModule : BaseServiceModule, IPlayerBoundsModule
    {
        /// <inheritdoc />
        public PlayerBoundsModule(string name, uint priority, BaseProfile profile, IPlayerService parentService)
            : base(name, priority, profile, parentService) { }

        /// <inheritdoc />
        public bool IsPlayerOutOfBounds { get; private set; }

        /// <inheritdoc />
        public Pose LastInBoundsPose { get; private set; }

        /// <inheritdoc />
        public event PlayerOutOfBoundsDelegate PlayerOutOfBounds;

        /// <inheritdoc />
        public event Action PlayerBackInBounds;

        private IPlayerRig playerRig;
        private const float returnToBoundsPoseOffset = .5f;

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            playerRig = (ParentService as IPlayerService).PlayerRig;
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
        public void RaisePlayerOutOfBounds(float severity, Vector3 returnToBoundsDirection)
        {
            IsPlayerOutOfBounds = severity > 0f;
            if (IsPlayerOutOfBounds)
            {
                PlayerOutOfBounds?.Invoke(severity, returnToBoundsDirection);
            }
        }

        /// <inheritdoc />
        public void RaisePlayerBackInBounds()
        {
            IsPlayerOutOfBounds = false;
            PlayerBackInBounds?.Invoke();
        }
    }
}
