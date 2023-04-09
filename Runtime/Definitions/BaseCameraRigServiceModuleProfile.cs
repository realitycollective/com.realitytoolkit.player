// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using System;
using UnityEngine;

namespace RealityToolkit.CameraService.Definitions
{
    /// <summary>
    /// Provides configuration options for <see cref="Modules.BaseCameraRigServiceModule"/>s.
    /// </summary>
    public class BaseCameraRigServiceModuleProfile : BaseProfile
    {
        [Header("General")]
        [SerializeField]
        [Tooltip("Sets the tracking type of the camera.")]
        private TrackingType trackingType = TrackingType.Auto;

        /// <summary>
        /// The configured tracking type of the camera.
        /// </summary>
        public TrackingType TrackingType => trackingType;

        [SerializeField]
        [Range(1f, 2f)]
        [Tooltip("Rendered eye texture resolution. A value greater than 1 has an impact on performance.")]
        private float eyeTextureResolution = 1f;

        /// <summary>
        /// Rendered eye texture resolution. A value greater than 1 has
        /// an impact on performance.
        /// </summary>
        public float EyeTextureResolution => eyeTextureResolution;

        [SerializeField]
        [Tooltip("Set, if you want XRTK to apply quality settings for the camera.")]
        private bool applyQualitySettings = true;

        /// <summary>
        /// If set, XRTK will update the quality settings for the camera as configured in the profile.
        /// </summary>
        public bool ApplyQualitySettings => applyQualitySettings;

        [Header("Opaque Display Settings")]
        [Min(0.0001f)]
        [SerializeField]
        [Tooltip("The near clipping plane distance for an opaque display.")]
        private float nearClipPlaneOpaqueDisplay = 0.1f;

        /// <summary>
        /// The near clipping plane distance for an opaque display.
        /// </summary>
        public float NearClipPlaneOpaqueDisplay
        {
            get => nearClipPlaneOpaqueDisplay;
            internal set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value must be greater than zero!");
                }

                nearClipPlaneOpaqueDisplay = value;
            }
        }

        [SerializeField]
        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsOpaqueDisplay = CameraClearFlags.Skybox;

        /// <summary>
        /// Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.
        /// </summary>
        public CameraClearFlags CameraClearFlagsOpaqueDisplay => cameraClearFlagsOpaqueDisplay;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorOpaqueDisplay = Color.black;

        /// <summary>
        /// Background color for a transparent display.
        /// </summary>
        public Color BackgroundColorOpaqueDisplay => backgroundColorOpaqueDisplay;

        [SerializeField]
        [Tooltip("Set the desired quality for your application for opaque display.")]
        private int opaqueQualityLevel = 0;

        /// <summary>
        /// Set the desired quality for your application for opaque display.
        /// </summary>
        public int OpaqueQualityLevel => opaqueQualityLevel;

        [Header("Transparent Display Settings")]
        [Min(0.0001f)]
        [SerializeField]
        [Tooltip("The near clipping plane distance for a transparent display.")]
        private float nearClipPlaneTransparentDisplay = 0.85f;

        /// <summary>
        /// The near clipping plane distance for a transparent display.
        /// </summary>
        public float NearClipPlaneTransparentDisplay
        {
            get => nearClipPlaneTransparentDisplay;
            internal set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value must be greater than zero!");
                }

                nearClipPlaneTransparentDisplay = value;
            }
        }

        [SerializeField]
        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsTransparentDisplay = CameraClearFlags.SolidColor;

        /// <summary>
        /// Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.
        /// </summary>
        public CameraClearFlags CameraClearFlagsTransparentDisplay => cameraClearFlagsTransparentDisplay;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorTransparentDisplay = Color.clear;

        /// <summary>
        /// Background color for a transparent display.
        /// </summary>
        public Color BackgroundColorTransparentDisplay => backgroundColorTransparentDisplay;

        [SerializeField]
        [Tooltip("Set the desired quality for your application for transparent display.")]
        private int transparentQualityLevel = 0;

        /// <summary>
        /// Set the desired quality for your application for transparent display.
        /// </summary>
        public int TransparentQualityLevel => transparentQualityLevel;
    }
}
