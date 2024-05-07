// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Player.Rigs
{
    /// <summary>
    /// Base interface for implementing player rig service modules to be registered with the <see cref="IPlayerService"/>.
    /// </summary>
    public interface IPlayerRigServiceModule : IPlayerServiceModule
    {
        /// <summary>
        /// The <see cref="Rigs.TrackingType"/> this provider is configured to use.
        /// </summary>
        TrackingType TrackingType { get; }
    }
}
