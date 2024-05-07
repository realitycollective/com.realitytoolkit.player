// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.Player.Rigs
{
    /// <summary>
    /// Provides configuration options for <see cref="Modules.BasePlayerRigServiceModule"/>s.
    /// </summary>
    public class BasePlayerRigServiceModuleProfile : BaseProfile
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
    }
}
