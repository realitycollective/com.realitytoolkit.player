// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Player.Definitions;
using RealityToolkit.Player.Interfaces;
using UnityEngine;

namespace RealityToolkit.Player
{
    /// <summary>
    /// The default <see cref="IPlayerRig"/> implmentation.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [System.Runtime.InteropServices.Guid("8E0EE4FC-C8A5-4B10-9FCA-EE55B6D421FF")]
    public class PlayerRig : MonoBehaviour, IPlayerRig
    {
        [SerializeField, Tooltip("The camera component on the rig.")]
        private Camera rigCamera = null;

        /// <inheritdoc />
        public Transform RigTransform => transform;

        /// <inheritdoc />
        public Camera RigCamera => rigCamera;

        /// <inheritdoc />
        public Transform CameraTransform => RigCamera.IsNull() ? null : RigCamera.transform;

        private IPlayerService playerService;
        /// <summary>
        /// Lazy loaded reference to the active <see cref="IPlayerService"/>.
        /// </summary>
        protected IPlayerService PlayerService => playerService ??= ServiceManager.Instance.GetService<IPlayerService>();

        /// <inheritdoc />
        protected virtual async void Start()
        {
            await ServiceManager.WaitUntilInitializedAsync();

            if (ServiceManager.Instance.TryGetServiceProfile<IPlayerService, PlayerServiceProfile>(out var profile) &&
                profile.IsRigPersistent)
            {
                RigTransform.gameObject.DontDestroyOnLoad();
            }
        }

        /// <summary>
        /// Resets the <see cref="IPlayerRig.RigTransform"/>, <see cref="IPlayerRig.CameraTransform"/>.
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
        public virtual void SetPositionAndRotation(Vector3 position, Quaternion rotation)
            => SetPositionAndRotation(position, rotation.eulerAngles);

        /// <inheritdoc />
        public virtual void SetPositionAndRotation(Vector3 position, Vector3 rotation)
        {
            var height = position.y;
            position -= CameraTransform.position - RigTransform.position;
            position.y = height;

            RigTransform.position = position;
            RotateAround(Vector3.up, rotation.y - CameraTransform.eulerAngles.y);
        }

        /// <inheritdoc />
        public virtual void Move(Vector2 direction, float speed = 1f)
        => Move(new Vector3(direction.x, 0f, direction.y), speed);

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