// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraService.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// Default <see cref="ICharacterCameraRig"/> implementation.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    [System.Runtime.InteropServices.Guid("3ddace9b-b75e-46d0-9b62-2b169e0c35d5")]
    public class CharacterCameraRig : TrackedCameraRig, ICharacterCameraRig
    {
        [SerializeField, Tooltip("The head collider is used to offset the body/character collider.")]
        private SphereCollider head = null;

        [Header("Body Estimation")]
        [SerializeField, Tooltip("The main character collider.")]
        private CharacterController controller;

        [SerializeField, Tooltip("The transform defining the body position.")]
        private Transform bodyTransform = null;

        [SerializeField, Tooltip("The diameter of the body collider.")]
        [Range(.5f, 2f)]
        private float bodyDiameter = 1f;

        [SerializeField]
        [Range(0f, 180f)]
        [Tooltip("This is the angle that will be used to adjust the player's body rotation in relation to their head position.")]
        private float bodyAdjustmentAngle = 60f;

        [SerializeField, Range(1f, 100f)]
        [Tooltip("The speed at which the body transform will sync it's rotation with the head transform.")]
        private float bodyAdjustmentSpeed = 1f;

        /// <inheritdoc />
        public Transform BodyTransform => bodyTransform;

        protected override void Update()
        {
            base.Update();
            SyncBody();
            SyncControllerCenter();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            var prevoiusColor = Gizmos.color;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(head.transform.position, head.radius);
            Gizmos.color = prevoiusColor;
        }

        protected override void ResetRig()
        {
            base.ResetRig();
            SyncBody();
            SyncControllerCenter();
        }

        private void SyncBody()
        {
            var cameraLocalPosition = CameraTransform.localPosition;
            var bodyLocalPosition = BodyTransform.localPosition;

            bodyLocalPosition.x = cameraLocalPosition.x;
            bodyLocalPosition.y = 0f;
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

        private void SyncControllerCenter()
        {
            var bodyPosition = BodyTransform.localPosition;
            var headHeight = head.radius;
            var bodyHeight = head.transform.localPosition.y - headHeight;

            controller.radius = bodyDiameter / 2f;
            controller.height = bodyHeight;
            controller.center = new Vector3(bodyPosition.x, bodyPosition.y + (controller.height / 2f), bodyPosition.z);
        }
    }
}
