// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using RealityToolkit.CameraService.Definitions;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// Base interface for implementing camera service modules to be registered with the <see cref="ICameraService"/>
    /// </summary>
    public interface ICameraServiceModule : IServiceModule
    {
        /// <summary>
        /// The <see cref="Definitions.TrackingType"/> this provider is configured to use.
        /// </summary>
        TrackingType TrackingType { get; }

        /// <summary>
        /// The current head height of the player
        /// </summary>
        float HeadHeight { get; }
    }
}