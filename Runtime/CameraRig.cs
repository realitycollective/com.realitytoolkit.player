// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// The default <see cref="ICameraRig"/> implmentation.
    /// Use it as it is or use it as starting point for customization.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [System.Runtime.InteropServices.Guid("8E0EE4FC-C8A5-4B10-9FCA-EE55B6D421FF")]
    public class CameraRig : MonoBehaviour, ICameraRig
    {
        [SerializeField, Tooltip("The camera component on the rig.")]
        private Camera rigCamera = null;

        /// <inheritdoc />
        public GameObject GameObject => gameObject;

        /// <inheritdoc />
        public Transform RigTransform => transform;

        /// <inheritdoc />
        public Camera RigCamera => rigCamera;

        /// <inheritdoc />
        public Transform CameraTransform => RigCamera.IsNull() ? null : RigCamera.transform;

        /// <inheritdoc />
        public virtual bool IsStereoscopic => RigCamera.stereoEnabled;

        /// <inheritdoc />
        public virtual bool IsOpaque
        {
            get
            {
                if (!ServiceManager.IsActiveAndInitialized ||
                    !ServiceManager.Instance.TryGetService<ICameraService>(out var cameraService) ||
                    cameraService.DisplaySubsystem == null)
                {
                    // When no device is attached we are assuming the display
                    // device is the computer's display, which should be opaque.
                    return true;
                }

                return cameraService.DisplaySubsystem.displayOpaque;
            }
        }

        /// <summary>
        /// Resets the <see cref="ICameraRig.RigTransform"/>, <see cref="ICameraRig.CameraTransform"/>.
        /// </summary>
        protected virtual void ResetRig()
        {
            RigTransform.position = Vector3.zero;
            RigTransform.rotation = Quaternion.identity;
            CameraTransform.position = Vector3.zero;
            CameraTransform.rotation = Quaternion.identity;
        }

        /// <inheritdoc />
        public virtual void Move(Vector3 direction, float speed = 1f)
        {
            RigTransform.Translate(speed * Time.deltaTime * direction);
        }
    }
}