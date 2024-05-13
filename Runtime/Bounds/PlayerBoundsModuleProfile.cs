// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using UnityEngine;

namespace RealityToolkit.Player.Bounds
{
    /// <summary>
    /// Configuration profile for the <see cref="PlayerBoundsModule"/>.
    /// </summary>
    public class PlayerBoundsModuleProfile : BaseProfile
    {
        [SerializeField, Tooltip("The distance the head is allowed to move out of bounds before it is considered severely out of bounds.")]
        private float maxSeverityDistanceThreshold = .2f;

        /// <summary>
        /// The distance the head is allowed to move out of bounds before it is considered severely out of bounds.
        /// </summary>
        public float MaxSeverityDistanceThreshold => maxSeverityDistanceThreshold;

        [SerializeField, Tooltip("If set, the player will be reset into bounds if out of bounds for a given period of time.")]
        private bool autoResetEnabled = true;

        /// <summary>
        /// If set, the player will be reset into bounds if out of bounds for a given period of time.
        /// </summary>
        public bool AutoResetEnabled => autoResetEnabled;

        [SerializeField, Tooltip("Duration in seconds tolerated out of bounds until the player is automatically reset into bounds.")]
        private float autoResetTimeout = 5f;

        /// <summary>
        /// Duration in seconds tolerated out of bounds until the player is automatically reset into bounds.
        /// </summary>
        public float AutoResetTimeout => autoResetTimeout;
    }
}
