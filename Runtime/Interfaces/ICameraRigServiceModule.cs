// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraService.Definitions;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// Base interface for implementing camera rig service modules to be registered with the <see cref="ICameraService"/>.
    /// </summary>
    public interface ICameraRigServiceModule : ICameraServiceModule
    {
        /// <summary>
        /// The <see cref="Definitions.TrackingType"/> this provider is configured to use.
        /// </summary>
        TrackingType TrackingType { get; }
    }
}
