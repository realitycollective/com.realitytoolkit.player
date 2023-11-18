// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.CameraService.Modules
{
    /// <summary>
    /// Base class for <see cref="ICameraRigServiceModule"/>s to inherit from.
    /// </summary>
    public abstract class BaseCameraRigServiceModule : BaseServiceModule, ICameraRigServiceModule
    {
        /// <inheritdoc />
        public BaseCameraRigServiceModule(string name, uint priority, BaseCameraRigServiceModuleProfile profile, ICameraService parentService)
            : base(name, priority, profile, parentService)
        {
            cameraService = parentService;
            eyeTextureResolution = profile.EyeTextureResolution;
            TrackingType = profile.TrackingType;
        }

        private readonly ICameraService cameraService;
        private readonly float eyeTextureResolution;

        /// <inheritdoc />
        public TrackingType TrackingType { get; }

        /// <inheritdoc />
        public virtual float HeadHeight => CameraRig.CameraTransform.localPosition.y;

        /// <summary>
        /// Internal referrence to the <see cref="ICameraService.CameraRig"/>
        /// for ease of use.
        /// </summary>
        private ICameraRig CameraRig => cameraService.CameraRig;

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