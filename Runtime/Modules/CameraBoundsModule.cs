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
        public bool IsCameraOutOfBounds { get; private set; }

        /// <inheritdoc />
        public Pose LastInBoundsPose { get; private set; }

        /// <inheritdoc />
        public event CameraOutOfBoundsDelegate CameraOutOfBounds;

        /// <inheritdoc />
        public event Action CameraBackInBounds;

        private ICameraRig cameraRig;
        private const float returnToBoundsPoseOffset = .5f;

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            cameraRig = (ParentService as ICameraService).CameraRig;
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetCameraIntoBounds();
            }

            if (IsCameraOutOfBounds)
            {
                return;
            }

            var position = cameraRig.CameraTransform.position;
            position.y = cameraRig.RigTransform.position.y;
            LastInBoundsPose = new Pose(position, cameraRig.RigTransform.rotation);
        }

        /// <inheritdoc />
        public void ResetCameraIntoBounds()
        {
            if (!IsCameraOutOfBounds)
            {
                return;
            }

            var position = LastInBoundsPose.position;
            var direction = cameraRig.CameraTransform.position - position;
            direction.y = 0f;
            direction.Normalize();
            position -= returnToBoundsPoseOffset * direction;

            cameraRig.SetPositionAndRotation(position, Quaternion.identity);
            RaiseCameraBackInBounds();
        }

        /// <inheritdoc />
        public void RaiseCameraOutOfBounds(float severity, Vector3 returnToBoundsDirection)
        {
            IsCameraOutOfBounds = severity > 0f;
            if (IsCameraOutOfBounds)
            {
                CameraOutOfBounds?.Invoke(severity, returnToBoundsDirection);
            }
        }

        /// <inheritdoc />
        public void RaiseCameraBackInBounds()
        {
            IsCameraOutOfBounds = false;
            CameraBackInBounds?.Invoke();
        }
    }
}
