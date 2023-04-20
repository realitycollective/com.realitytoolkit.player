// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Definitions;
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
                if (CameraService.DisplaySubsystem == null)
                {
                    // When no device is attached we are assuming the display
                    // device is the computer's display, which should be opaque.
                    return true;
                }

                return CameraService.DisplaySubsystem.displayOpaque;
            }
        }

        private ICameraService cameraService;
        /// <summary>
        /// Lazy loaded reference to the active <see cref="ICameraService"/>.
        /// </summary>
        protected ICameraService CameraService => cameraService ??= ServiceManager.Instance.GetService<ICameraService>();

        /// <inheritdoc />
        protected virtual async void Start()
        {
            await ServiceManager.WaitUntilInitializedAsync();

            if (ServiceManager.Instance.TryGetServiceProfile<ICameraService, CameraServiceProfile>(out var profile) &&
                profile.IsRigPersistent)
            {
                RigTransform.gameObject.DontDestroyOnLoad();
            }
        }

        /// <summary>
        /// Resets the <see cref="ICameraRig.RigTransform"/>, <see cref="ICameraRig.CameraTransform"/>.
        /// </summary>
        protected virtual void ResetRig()
        {
            RigTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            CameraTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        /// <inheritdoc />
        public virtual void RotateAround(Vector3 axis, float angle)
        {
            RigTransform.RotateAround(CameraTransform.position, axis, angle);
        }

        /// <inheritdoc />
        public virtual void Move(Vector2 direction, float speed = 1f)
        => Move(new Vector3(direction.x, 0f, direction.y));

        /// <inheritdoc />
        public virtual void Move(Vector3 direction, float speed = 1f)
        {
            var forwardDirection = CameraTransform.forward;
            forwardDirection.y = 0f;

            var rightDirection = CameraTransform.right;
            rightDirection.y = 0f;

            var combinedDirection = (forwardDirection * direction.z + rightDirection * direction.x).normalized;

            RigTransform.Translate(speed * Time.deltaTime * combinedDirection, Space.World);
        }
    }
}