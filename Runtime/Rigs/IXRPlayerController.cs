// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Player.Rigs
{
    /// <summary>
    /// A <see cref="IXRPlayerRig"/> for player character simulation.
    /// </summary>
    public interface IXRPlayerController : IXRPlayerRig
    {
        /// <summary>
        /// Controls when gravity begins to take effect.
        /// </summary>
        /// <seealso cref="Rigs.GravityMode"/>
        GravityMode GravityMode { get; set; }

        /// <summary>
        /// The player's head on the <see cref="IXRPlayerController"/>.
        /// </summary>
        IXRPlayerHead Head { get; }

        /// <summary>
        /// The character's body transform.
        /// </summary>
        Transform BodyTransform { get; }
    }
}
