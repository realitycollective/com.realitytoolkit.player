// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraService.Interfaces;
using System;
using UnityEngine;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// Default <see cref="ICharacterCameraRig"/> implementation.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    [System.Runtime.InteropServices.Guid("3ddace9b-b75e-46d0-9b62-2b169e0c35d5")]
    public class CharacterCameraRig : TrackedCameraRig, ICharacterCameraRig
    {
        private CharacterController characterController;

        [SerializeField, Tooltip("The head rigidbody.")]
        private Rigidbody head = null;

        [SerializeField]
        private Transform bodyTransform = null;

        [Header("Body Estimation")]
        [SerializeField]
        [Range(0f, 180f)]
        [Tooltip("This is the angle that will be used to adjust the player's body rotation in relation to their head position.")]
        private float bodyAdjustmentAngle = 60f;

        [SerializeField]
        [Tooltip("The speed at which the body transform will sync it's rotation with the head transform.")]
        private float bodyAdjustmentSpeed = 1f;

        /// <inheritdoc />
        public CharacterController Controller => characterController;

        /// <inheritdoc />
        public Transform BodyTransform => bodyTransform;

        protected override void Start()
        {
            base.Start();
            characterController = GetComponent<CharacterController>();
        }

        protected override void Update()
        {
            base.Update();
            SyncControllerCenter();
            SyncBody();
        }

        private void SyncControllerCenter()
        {
            var bodyPosition = BodyTransform.localPosition;
            var cameraHeight = CameraTransform.localPosition.y;

            characterController.center = BodyTransform.localPosition;
            characterController.height = cameraHeight;
        }

        private void SyncBody()
        {
            var cameraLocalPosition = CameraTransform.localPosition;
            var bodyLocalPosition = BodyTransform.localPosition;

            bodyLocalPosition.x = cameraLocalPosition.x;
            bodyLocalPosition.y = cameraLocalPosition.y - Math.Abs(VerticalCameraOffset);
            bodyLocalPosition.z = cameraLocalPosition.z;

            BodyTransform.localPosition = bodyLocalPosition;

            var bodyRotation = BodyTransform.rotation;
            var headRotation = CameraTransform.rotation;
            var currentAngle = Mathf.Abs(Quaternion.Angle(bodyRotation, headRotation));

            if (currentAngle > bodyAdjustmentAngle)
            {
                BodyTransform.rotation = Quaternion.Slerp(bodyRotation, headRotation, Time.deltaTime * bodyAdjustmentSpeed);
            }
        }
    }
}
