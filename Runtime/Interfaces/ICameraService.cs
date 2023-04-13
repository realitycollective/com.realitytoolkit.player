// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// Event delegate for handling the camera out of bounds situation.
    /// </summary>
    /// <param name="returnToBoundsDirection">A <see cref="Vector3"/> specifying the direction the camera needs to take to return to into bounds.</param>
    public delegate void CameraOutOfBoundsDelegate(Vector3 returnToBoundsDirection);

    /// <summary>
    /// The base interface for implementing a mixed reality camera system.
    /// </summary>
    public interface ICameraService : IService
    {
        /// <summary>
        /// The active <see cref="ICameraRig"/>.
        /// </summary>
        ICameraRig CameraRig { get; }

        /// <summary>
        /// The active <see cref="ICameraRigServiceModule"/> operating the <see cref="CameraRig"/>.
        /// </summary>
        ICameraRigServiceModule CameraRigServiceModule { get; }

        /// <summary>
        /// Gets the active <see cref="XRDisplaySubsystem"/> for the currently loaded
        /// XR plugin / platform.
        /// </summary>
        /// <remarks>The reference is lazy loaded once on first access and then cached for future use.</remarks>
        XRDisplaySubsystem DisplaySubsystem { get; }

        /// <summary>
        /// Raised while the <see cref="ICameraRig.RigCamera"/> is out of bounds.
        /// </summary>
        event CameraOutOfBoundsDelegate CameraOutOfBounds;

        /// <summary>
        /// Raises the <see cref="CameraOutOfBounds"/> event to subsribed
        /// <see cref="CameraOutOfBoundsDelegate"/>s.
        /// </summary>
        /// <param name="returnToBoundsDirection">A <see cref="Vector3"/> specifying the direction the camera needs to take to return to into bounds.</param>
        void RaiseCameraOutOfBounds(Vector3 returnToBoundsDirection);
    }
}