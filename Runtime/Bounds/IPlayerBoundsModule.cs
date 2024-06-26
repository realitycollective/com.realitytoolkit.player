﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace RealityToolkit.Player.Bounds
{
    /// <summary>
    /// Event delegate for handling the playerService out of bounds situation.
    /// </summary>
    /// <param name="severity">A percentage in range <c>[0f, 1f]</c> specifying how far of bounds the player is. Where <c>1f</c> means the player
    /// is definitely going places it should not be at.</param>
    /// <param name="returnToBoundsDirection">A <see cref="Vector3"/> specifying the direction the player needs to take to return to into bounds.</param>
    public delegate void PlayerOutOfBoundsDelegate(float severity, Vector3 returnToBoundsDirection);

    /// <summary>
    /// The player bounds module is used for room scale XR applications
    /// where it is necessary to monitor, whether the user has moved physically
    /// outside of allowed application bounds.
    /// </summary>
    public interface IPlayerBoundsModule : IPlayerServiceModule
    {
        /// <summary>
        /// Is the active <see cref="IPlayerRig"/> currently considered out of bounds?
        /// </summary>
        bool IsPlayerOutOfBounds { get; }

        /// <summary>
        /// The last saved known <see cref="Pose"/> where the <see cref="IPlayerRig"/> was still in bounds.
        /// </summary>
        Pose LastInBoundsPose { get; }

        /// <summary>
        /// Raised while the <see cref="IPlayerRig.RigCamera"/> is out of bounds.
        /// </summary>
        event PlayerOutOfBoundsDelegate PlayerOutOfBounds;

        /// <summary>
        /// Raised when the <see cref="IPlayerRig.RigCamera"/> is back in bounds.
        /// </summary>
        event Action PlayerBackInBounds;

        /// <summary>
        /// Force resets the <see cref="IPlayerRig"/> into the last known pose
        /// before it went out of bounds.
        /// </summary>
        void ResetPlayerIntoBounds();

        /// <summary>
        /// Raises the <see cref="PlayerOutOfBounds"/> event to subsribed
        /// <see cref="PlayerOutOfBoundsDelegate"/>s.
        /// </summary>
        /// <param name="severity">A percentage in range <c>[0f, 1f]</c> specifying how far of bounds the player is. Where <c>1f</c> means the player
        /// is definitely going places it should not be at.</param>
        /// <param name="returnToBoundsDirection">A <see cref="Vector3"/> specifying the direction the player needs to take to return to into bounds.</param>
        void RaisePlayerOutOfBounds(float severity, Vector3 returnToBoundsDirection);

        /// <summary>
        /// Raises the <see cref="PlayerBackInBounds"/> event to subscirbed delegates.
        /// </summary>
        void RaisePlayerBackInBounds();
    }
}
