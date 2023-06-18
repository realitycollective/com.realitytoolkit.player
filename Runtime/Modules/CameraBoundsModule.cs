// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.CameraService.Interfaces;
using System;
using UnityEngine;

namespace RealityToolkit.CameraService.Modules
{
    /// <summary>
    /// Default implementation for <see cref="ICameraBoundsModule"/>.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("a68682d3-3e9d-4d56-8a32-9805f58928f8")]
    public class CameraBoundsModule : BaseServiceModule, ICameraBoundsModule
    {
        /// <inheritdoc />
        public CameraBoundsModule(string name, uint priority, BaseProfile profile, ICameraService parentService)
            : base(name, priority, profile, parentService) { }

        /// <inheritdoc />
        public event CameraOutOfBoundsDelegate CameraOutOfBounds;

        /// <inheritdoc />
        public event Action CameraBackInBounds;

        /// <inheritdoc />
        public void RaiseCameraOutOfBounds(float severity, Vector3 returnToBoundsDirection) => CameraOutOfBounds?.Invoke(severity, returnToBoundsDirection);

        /// <inheritdoc />
        public void RaiseCameraBackInBounds() => CameraBackInBounds?.Invoke();
    }
}
