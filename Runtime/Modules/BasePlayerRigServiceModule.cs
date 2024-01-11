// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.PlayerService.Definitions;
using RealityToolkit.PlayerService.Interfaces;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.PlayerService.Modules
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
            PlayerService = parentService;
            eyeTextureResolution = profile.EyeTextureResolution;
            TrackingType = profile.TrackingType;
        }

        private readonly IPlayerService PlayerService;
        private readonly float eyeTextureResolution;

        /// <inheritdoc />
        public TrackingType TrackingType { get; }

        /// <inheritdoc />
        public virtual float HeadHeight => PlayerRig.CameraTransform.localPosition.y;

        /// <summary>
        /// Internal referrence to the <see cref="IPlayerService.PlayerRig"/>
        /// for ease of use.
        /// </summary>
        private IPlayerRig PlayerRig => PlayerService.PlayerRig;

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