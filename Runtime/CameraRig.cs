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
#if RTK_LOCOMOTION
        , Locomotion.Interfaces.ILocomotionTarget
#endif
    {
        [SerializeField, Tooltip("The camera component on the rig.")]
        private Camera rigCamera = null;

        /// <inheritdoc />
        public GameObject GameObject => gameObject;

        /// <inheritdoc />
        public Transform RigTransform => transform;

#if RTK_LOCOMOTION
        /// <inheritdoc />
        public Transform MoveTransform => RigTransform;

        /// <inheritdoc />
        public Transform OrientationTransform => CameraTransform;
#endif

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

#if RTK_LOCOMOTION
        private async void Awake()
        {
            await ServiceManager.WaitUntilInitializedAsync();
            if (ServiceManager.Instance.TryGetService<Locomotion.Interfaces.ILocomotionService>(out var locomotionService))
            {
                locomotionService.LocomotionTarget = this;
            }
        }
#endif

        /// <summary>
        /// Resets the <see cref="ICameraRig.RigTransform"/>, <see cref="ICameraRig.CameraTransform"/>.
        /// </summary>
        protected virtual void ResetRig()
        {
            RigTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            CameraTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        /// <inheritdoc />
        public virtual void Move(Vector3 direction, float speed = 1f)
        {
            RigTransform.Translate(speed * Time.deltaTime * direction);
        }
    }
}