// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// A basic camera rig.
    /// </summary>
    public interface ICameraRig
    {
        /// <summary>
        /// The root rig <see cref="Transform"/>.
        /// </summary>
        Transform RigTransform { get; }

        /// <summary>
        /// The <see cref="Transform"/> where the <see cref="Camera"/> component is located.
        /// </summary>
        Transform CameraTransform { get; }

        /// <summary>
        /// The rig's <see cref="Camera"/> reference.
        /// </summary>
        Camera RigCamera { get; }

        /// <summary>
        /// Is the <see cref="RigCamera"/> camera displaying on a traditional 2d screen or a stereoscopic display?
        /// </summary>
        bool IsStereoscopic { get; }

        /// <summary>
        /// Is the <see cref="RigCamera"/> displaying in opaque (VR) or transparent mode (XR)?.
        /// </summary>
        bool IsOpaque { get; }

        /// <summary>
        /// Rotates the <see cref="ICameraRig"/> at its current position
        /// around <paramref name="axis"/> by <paramref name="angle"/>.
        /// </summary>
        /// <param name="axis">Axis to rotate around.</param>
        /// <param name="angle">Angle to rotate.</param>
        void RotateAround(Vector3 axis, float angle);

        /// <summary>
        /// Moves the <see cref="ICameraRig"/> in <paramref name="direction"/> on the (X,Z) plane.
        /// </summary>
        /// <param name="direction">The direction <see cref="Vector2"/>.</param>
        /// <param name="speed">The speed multiplier for the movement. Defaults to <c>1f</c>.</param>
        void Move(Vector2 direction, float speed = 1f);

        /// <summary>
        /// Moves the <see cref="ICameraRig"/> in <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">The direction <see cref="Vector3"/>.</param>
        /// <param name="speed">The speed multiplier for the movement. Defaults to <c>1f</c>.</param>
        void Move(Vector3 direction, float speed = 1f);
    }
}
