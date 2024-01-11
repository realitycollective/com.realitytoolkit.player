// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.PlayerService.Definitions;

namespace RealityToolkit.PlayerService.Interfaces
{
    /// <summary>
    /// Base interface for implementing camera rig service modules to be registered with the <see cref="IPlayerService"/>.
    /// </summary>
    public interface ICameraRigServiceModule : IPlayerServiceModule
    {
        /// <summary>
        /// The <see cref="Definitions.TrackingType"/> this provider is configured to use.
        /// </summary>
        TrackingType TrackingType { get; }
    }
}
