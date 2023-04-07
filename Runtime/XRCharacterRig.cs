// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraService.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// A physics enabled character <see cref="ICameraRig"/> implmentation.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    [System.Runtime.InteropServices.Guid("3ddace9b-b75e-46d0-9b62-2b169e0c35d5")]
    public class XRCharacterRig : XRCameraRig
    {
        private CharacterController characterController;

        [SerializeField, Tooltip("The head rigidbody.")]
        private Rigidbody head = null;

        [SerializeField, Tooltip("The left hand rigidbody.")]
        private Rigidbody leftHand = null;

        [SerializeField, Tooltip("The right hand rigidbody.")]
        private Rigidbody rightHand = null;

        /// <summary>
        /// The left hand tracking space <see cref="Transform"/>.
        /// </summary>
        public Transform LeftHandTransform => leftHand.transform;

        /// <summary>
        /// The right hand tracking space <see cref="Transform"/>.
        /// </summary>
        public Transform RightHandTransform => rightHand.transform;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            SyncControllerCenter();
            SyncHands();
        }

        private void SyncControllerCenter()
        {
            var bodyPosition = BodyTransform.localPosition;
            var cameraHeight = CameraTransform.localPosition.y;

            characterController.center = BodyTransform.localPosition;
            characterController.height = cameraHeight;
        }

        private void SyncHands()
        {

        }
    }
}
