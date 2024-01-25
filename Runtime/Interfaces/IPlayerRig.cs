// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Player.Interfaces
{
    /// <summary>
    /// A basic player rig.
    /// </summary>
    public interface IPlayerRig
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
        /// Rotates the <see cref="IPlayerRig"/> at its current position
        /// around <paramref name="axis"/> by <paramref name="angle"/>.
        /// </summary>
        /// <param name="axis">Axis to rotate around.</param>
        /// <param name="angle">Angle to rotate.</param>
        void RotateAround(Vector3 axis, float angle);

        /// <summary>
        /// Sets the world space position and rotation of the <see cref="IPlayerRig"/>.
        /// </summary>
        /// <param name="position">The world space position.</param>
        /// <param name="rotation">The world space rotation.</param>
        void SetPositionAndRotation(Vector3 position, Quaternion rotation);

        /// <summary>
        /// Sets the world space position and rotation of the <see cref="IPlayerRig"/>.
        /// </summary>
        /// <param name="position">The world space position.</param>
        /// <param name="rotation">The world space rotation.</param>
        void SetPositionAndRotation(Vector3 position, Vector3 rotation);

        /// <summary>
        /// Moves the <see cref="IPlayerRig"/> in <paramref name="direction"/> on the (X,Z) plane.
        /// </summary>
        /// <param name="direction">The direction <see cref="Vector2"/>.</param>
        /// <param name="speed">The speed multiplier for the movement. Defaults to <c>1f</c>.</param>
        void Move(Vector2 direction, float speed = 1f);

        /// <summary>
        /// Moves the <see cref="IPlayerRig"/> in <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">The direction <see cref="Vector3"/>.</param>
        /// <param name="speed">The speed multiplier for the movement. Defaults to <c>1f</c>.</param>
        void Move(Vector3 direction, float speed = 1f);
    }
}
