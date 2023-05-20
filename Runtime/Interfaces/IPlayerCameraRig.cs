// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// A <see cref="ICameraRig"/> for player character simulation.
    /// </summary>
    public interface IPlayerCameraRig : ICameraRig
    {
        /// <summary>
        /// Controls when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="RealityToolkit.CameraService.GravityMode"/>
        GravityMode GravityMode { get; set; }

        /// <summary>
        /// The character's body transform.
        /// </summary>
        Transform BodyTransform { get; }
    }
}
