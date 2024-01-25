// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using UnityEngine.XR;

namespace RealityToolkit.Player.Interfaces
{
    /// <summary>
    /// The base interface for implementing a mixed reality player system.
    /// </summary>
    public interface IPlayerService : IService
    {
        /// <summary>
        /// The active <see cref="IPlayerRig"/>.
        /// </summary>
        IPlayerRig PlayerRig { get; }

        /// <summary>
        /// The active <see cref="IPlayerRigServiceModule"/> operating the <see cref="PlayerRig"/>.
        /// </summary>
        IPlayerRigServiceModule PlayerRigServiceModule { get; }

        /// <summary>
        /// Gets the active <see cref="XRDisplaySubsystem"/> for the currently loaded
        /// XR plugin / platform.
        /// </summary>
        /// <remarks>The reference is lazy loaded once on first access and then cached for future use.</remarks>
        XRDisplaySubsystem DisplaySubsystem { get; }
    }
}