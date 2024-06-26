﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Player.Rigs
{
    /// <summary>
    /// Available tracking types for the camera operated by the <see cref="IPlayerService"/>.
    /// </summary>
    public enum TrackingType
    {
        /// <summary>
        /// Use platform defaults or if applicable, auto determine the supported
        /// <see cref="TrackingType"/> by the device running the application.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Positional and orientational tracking.
        /// </summary>
        SixDegreesOfFreedom,
        /// <summary>
        /// Orientational tracking only.
        /// </summary>
        ThreeDegreesOfFreedom
    }
}
