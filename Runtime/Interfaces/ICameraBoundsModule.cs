// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// Event delegate for handling the camera out of bounds situation.
    /// </summary>
    /// <param name="severity">A percentage in range <c>[0f, 1f]</c> specifying how far of bounds the camera is. Where <c>1f</c> means the camera
    /// is definitely going places it should not be at.</param>
    /// <param name="returnToBoundsDirection">A <see cref="Vector3"/> specifying the direction the camera needs to take to return to into bounds.</param>
    public delegate void CameraOutOfBoundsDelegate(float severity, Vector3 returnToBoundsDirection);

    /// <summary>
    /// The camera bounds module is used for room scale XR applications
    /// where it is necessary to monitor, whether the user has moved physically
    /// outside of allowed application bounds.
    /// </summary>
    public interface ICameraBoundsModule : ICameraServiceModule
    {
        /// <summary>
        /// Raised while the <see cref="ICameraRig.RigCamera"/> is out of bounds.
        /// </summary>
        event CameraOutOfBoundsDelegate CameraOutOfBounds;

        /// <summary>
        /// Raised when the <see cref="ICameraRig.RigCamera"/> is back in bounds.
        /// </summary>
        event Action CameraBackInBounds;

        /// <summary>
        /// Raises the <see cref="CameraOutOfBounds"/> event to subsribed
        /// <see cref="CameraOutOfBoundsDelegate"/>s.
        /// </summary>
        /// <param name="severity">A percentage in range <c>[0f, 1f]</c> specifying how far of bounds the camera is. Where <c>1f</c> means the camera
        /// is definitely going places it should not be at.</param>
        /// <param name="returnToBoundsDirection">A <see cref="Vector3"/> specifying the direction the camera needs to take to return to into bounds.</param>
        void RaiseCameraOutOfBounds(float severity, Vector3 returnToBoundsDirection);

        /// <summary>
        /// Raises the <see cref="CameraBackInBounds"/> event to subscirbed delegates.
        /// </summary>
        void RaiseCameraBackInBounds();
    }
}
