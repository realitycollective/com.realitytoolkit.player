// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraService.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// Default <see cref="ICharacterCameraRig"/> implementation.
    /// A player / character rig that supports collisions and has an estimated body pose.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
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

        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            UpdateRig();
        }

        /// <inheritdoc />
        protected virtual void LateUpdate()
        {
            CheckCameraBounds();
        }

        /// <inheritdoc />
        protected virtual void OnDrawGizmosSelected()
        {
            var prevoiusColor = Gizmos.color;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(head.transform.position, head.radius);
            Gizmos.color = prevoiusColor;
        }

        /// <inheritdoc />
        protected override void ResetRig()
        {
            base.ResetRig();
            UpdateRig();
        }

        /// <inheritdoc />
        public override void Move(Vector3 direction, float speed = 1)
        {
            controller.Move(speed * Time.deltaTime * direction);
        }

        private void UpdateRig()
        {
            UpdateBody();
            UpdateBodyControllerCenter();
            UpdateBodyController();
        }

        private void UpdateBody()
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

        private void UpdateBodyControllerCenter()
        {
            if (Application.isPlaying && !controller.enabled)
            {
                // If the controller is disabled, e.g. because the camera
                // is out of bounds, do not update it. If we are in edit-mode
                // we want to update it no matter what to keep it in sync with inspector
                // changes.
                return;
            }

            controller.radius = bodyDiameter / 2f;
            controller.height = Mathf.Max(0f, head.transform.localPosition.y - head.radius);
            controller.center = new Vector3(0f, controller.height / 2f, 0f);
            controller.skinWidth = .1f * controller.radius;
            controller.stepOffset = .1f * controller.height;
        }

        private void UpdateBodyController()
        {
            if (!controller.enabled || !Application.isPlaying)
            {
                return;
            }

            var headOffset = CameraTransform.localPosition;
            headOffset.y = 0f;
            controller.Move(headOffset);
        }

        private void CheckCameraBounds()
        {
            var headPosition = CameraTransform.position;
            headPosition.y = 0f;
            var bodyPosition = BodyTransform.position;
            bodyPosition.y = 0f;

            var headToBodyOffset = Vector3.Distance(headPosition, bodyPosition);
            if (headToBodyOffset > controller.radius)
            {
                controller.enabled = false;
                CameraService.RaiseCameraOutOfBounds((bodyPosition - headPosition).normalized);
                return;
            }

            controller.enabled = true;
        }
    }
}
