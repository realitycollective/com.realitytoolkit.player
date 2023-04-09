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
            applyQualitySettings = profile.ApplyQualitySettings;

            TrackingType = profile.TrackingType;

            nearClipPlaneOpaqueDisplay = profile.NearClipPlaneOpaqueDisplay;
            cameraClearFlagsOpaqueDisplay = profile.CameraClearFlagsOpaqueDisplay;
            backgroundColorOpaqueDisplay = profile.BackgroundColorOpaqueDisplay;
            opaqueQualityLevel = profile.OpaqueQualityLevel;

            nearClipPlaneTransparentDisplay = profile.NearClipPlaneTransparentDisplay;
            cameraClearFlagsTransparentDisplay = profile.CameraClearFlagsTransparentDisplay;
            backgroundColorTransparentDisplay = profile.BackgroundColorTransparentDisplay;
            transparentQualityLevel = profile.TransparentQualityLevel;
        }

        private readonly ICameraService cameraService;
        private readonly float eyeTextureResolution;
        private readonly bool applyQualitySettings;
        private readonly float nearClipPlaneTransparentDisplay;
        private readonly CameraClearFlags cameraClearFlagsTransparentDisplay;
        private readonly Color backgroundColorTransparentDisplay;
        private readonly int transparentQualityLevel;
        private readonly float nearClipPlaneOpaqueDisplay;
        private readonly CameraClearFlags cameraClearFlagsOpaqueDisplay;
        private readonly Color backgroundColorOpaqueDisplay;
        private readonly int opaqueQualityLevel;
        private bool cameraOpaqueLastFrame;

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

            cameraOpaqueLastFrame = CameraRig.IsOpaque;

            if (applyQualitySettings)
            {
                if (CameraRig.IsOpaque)
                {
                    ApplySettingsForOpaqueDisplay();
                }
                else
                {
                    ApplySettingsForTransparentDisplay();
                }
            }

            XRSettings.eyeTextureResolutionScale = eyeTextureResolution;
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (cameraOpaqueLastFrame != CameraRig.IsOpaque)
            {
                cameraOpaqueLastFrame = CameraRig.IsOpaque;

                if (applyQualitySettings)
                {
                    if (CameraRig.IsOpaque)
                    {
                        ApplySettingsForOpaqueDisplay();
                    }
                    else
                    {
                        ApplySettingsForTransparentDisplay();
                    }
                }
            }
        }

        /// <summary>
        /// Applies opaque settings from camera profile.
        /// </summary>
        protected virtual void ApplySettingsForOpaqueDisplay()
        {
            CameraRig.RigCamera.clearFlags = cameraClearFlagsOpaqueDisplay;
            CameraRig.RigCamera.nearClipPlane = nearClipPlaneOpaqueDisplay;
            CameraRig.RigCamera.backgroundColor = backgroundColorOpaqueDisplay;
            QualitySettings.SetQualityLevel(opaqueQualityLevel, false);
        }

        /// <summary>
        /// Applies transparent settings from camera profile.
        /// </summary>
        protected virtual void ApplySettingsForTransparentDisplay()
        {
            CameraRig.RigCamera.clearFlags = cameraClearFlagsTransparentDisplay;
            CameraRig.RigCamera.backgroundColor = backgroundColorTransparentDisplay;
            CameraRig.RigCamera.nearClipPlane = nearClipPlaneTransparentDisplay;
            QualitySettings.SetQualityLevel(transparentQualityLevel, false);
        }
    }
}