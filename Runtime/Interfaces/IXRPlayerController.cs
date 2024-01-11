// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.PlayerService.Definitions;
using UnityEngine;

namespace RealityToolkit.PlayerService.Interfaces
{
    /// <summary>
    /// A <see cref="IXRCameraRig"/> for player character simulation.
    /// </summary>
    public interface IXRPlayerController : IXRCameraRig
    {
        /// <summary>
        /// Controls when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="RealityToolkit.PlayerService.GravityMode"/>
        GravityMode GravityMode { get; set; }

        /// <summary>
        /// The character's body transform.
        /// </summary>
        Transform BodyTransform { get; }
    }
}
