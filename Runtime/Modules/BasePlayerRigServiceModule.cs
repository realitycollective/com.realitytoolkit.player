// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.Player.Definitions;
using RealityToolkit.Player.Interfaces;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Player.Modules
{
    /// <summary>
    /// Base class for <see cref="IPlayerRigServiceModule"/>s to inherit from.
    /// </summary>
    public abstract class BasePlayerRigServiceModule : BaseServiceModule, IPlayerRigServiceModule
    {
        /// <inheritdoc />
        public BasePlayerRigServiceModule(string name, uint priority, BasePlayerRigServiceModuleProfile profile, IPlayerService parentService)
            : base(name, priority, profile, parentService)
        {
            playerService = parentService;
            eyeTextureResolution = profile.EyeTextureResolution;
            TrackingType = profile.TrackingType;
        }

        private readonly IPlayerService playerService;
        private readonly float eyeTextureResolution;

        /// <inheritdoc />
        public TrackingType TrackingType { get; }

        /// <inheritdoc />
        public virtual float HeadHeight => playerRig.CameraTransform.localPosition.y;

        /// <summary>
        /// Internal referrence to the <see cref="IPlayerService.PlayerRig"/>
        /// for ease of use.
        /// </summary>
        private IPlayerRig playerRig => playerService.PlayerRig;

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            XRSettings.eyeTextureResolutionScale = eyeTextureResolution;
        }
    }
}