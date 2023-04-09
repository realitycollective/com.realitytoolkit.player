// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// A <see cref="ICameraRig"/> for player character simuation.
    /// </summary>
    public interface ICharacterCameraRig : ICameraRig
    {
        /// <summary>
        /// The <see cref="CharacterController"/> component on the <see cref="ICameraRig"/>.
        /// </summary>
        CharacterController Controller { get; }

        /// <summary>
        /// The character's body transform, located at the character's feet.
        /// </summary>
        /// <remarks>
        /// This <see cref="Transform"/> is synced to the <see cref="ICameraRig.CameraTransform"/>'s X & Z position values.
        /// Y value is set using current <see cref="ICameraRigServiceModule.HeadHeight"/>.
        /// </remarks>
        Transform BodyTransform { get; }
    }
}
