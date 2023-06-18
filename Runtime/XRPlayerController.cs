// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraService
{
    /// <summary>
    /// Default <see cref="IXRPlayerController"/> implementation.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CharacterController))]
    [System.Runtime.InteropServices.Guid("3ddace9b-b75e-46d0-9b62-2b169e0c35d5")]
    public class XRPlayerController : XRCameraRig, IXRPlayerController
    {
        [SerializeField, Tooltip("The head collider is used to offset the body/character collider.")]
        private XRPlayerHead head = null;

        [SerializeField, Tooltip("The main character collider.")]
        private CharacterController controller;

        [Header("Body Estimation")]
        [SerializeField, Tooltip("The transform defining the body position.")]
        private Transform bodyTransform = null;

        [SerializeField, Tooltip("The diameter of the body collider.")]
        [Range(.5f, 2f)]
        private float bodyDiameter = 1f;

        [SerializeField]
        [Tooltip("Threshold angle between head and body at which to start updating the body orientation to fit head rotation.")]
        private float startRotateBodyAngle = 45f;

        [SerializeField]
        [Tooltip("The body orientation change animation speed.")]
        private float angleRotateBodySpeed = 20f;

        [SerializeField, Header("Gravity")]
        [Tooltip("Controls when gravity begins to take effect.")]
        private GravityMode gravityMode;

        /// <inheritdoc />
        public GravityMode GravityMode
        {
            get => gravityMode;
            set => gravityMode = value;
        }

        /// <inheritdoc />
        public Transform BodyTransform => bodyTransform;

        private Vector3 gravityVelocity;
        private Vector3 motionInput;

        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            UpdateRig();
        }

        /// <inheritdoc />
        protected virtual void OnDrawGizmosSelected()
        {
            var prevoiusColor = Gizmos.color;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(head.transform.position, head.Radius);
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
            direction = controller.transform.TransformDirection(direction);

            var forwardDirection = CameraTransform.forward;
            forwardDirection.y = 0f;

            var rightDirection = CameraTransform.right;
            rightDirection.y = 0f;

            var combinedDirection = Quaternion.Inverse(RigTransform.rotation) * (rightDirection * direction.x + forwardDirection * direction.z).normalized;

            motionInput = speed * combinedDirection;
        }

        private void UpdateRig()
        {
            UpdateControllerBounds();

            if (!Application.isPlaying)
            {
                return;
            }

            UpdateBodyEstimation();
            UpdateGravity();
            ApplyMovement();
        }

        /// <summary>
        /// Updates the <see cref="controller"/> configuration with regard to
        /// the <see cref="ICameraRig.CameraTransform"/> pose.
        /// </summary>
        private void UpdateControllerBounds()
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
            controller.skinWidth = .1f * controller.radius;

            var height = Mathf.Max(bodyDiameter, head.transform.localPosition.y - head.Radius);
            controller.height = height;
            controller.stepOffset = .2f * controller.height;

            var center = CameraTransform.localPosition;
            center.y = height / 2f + controller.skinWidth;
            controller.center = center;
        }

        /// <summary>
        /// Updates the <see cref="gravityVelocity"/> with regard to the
        /// active <see cref="GravityMode"/>.
        /// </summary>
        private void UpdateGravity()
        {
            switch (gravityMode)
            {
                case GravityMode.OnMove:
                case GravityMode.Immediately:
                    {
                        if (controller.isGrounded ||
                            (motionInput == Vector3.zero && gravityMode == GravityMode.OnMove && gravityVelocity == Vector3.zero))
                        {
                            gravityVelocity = Vector3.zero;
                            return;
                        }

                        gravityVelocity = Physics.gravity;
                    }
                    break;
                case GravityMode.Disabled:
                    gravityVelocity = Vector3.zero;
                    return;
                default:
                    Debug.LogError($"{nameof(Definitions.GravityMode)}.{gravityMode} not supported.");
                    break;
            }
        }

        /// <summary>
        /// Uses the head / camera's forward orientation to estimate the orientation
        /// of the <see cref="BodyTransform"/> of the player.
        /// </summary>
        private void UpdateBodyEstimation()
        {
            var bodyForward = BodyTransform.forward;
            var headForward = CameraTransform.forward;
            headForward = new Vector3(headForward.x, 0f, headForward.z);

            var angle = Vector3.SignedAngle(bodyForward, headForward, Vector3.up);
            var delta = Mathf.Abs(angle) - startRotateBodyAngle;

            if (angle > startRotateBodyAngle)
            {
                var step = Vector3.up * Time.deltaTime * angleRotateBodySpeed * delta;
                BodyTransform.Rotate(step, Space.World);
            }
            else if (angle < -startRotateBodyAngle)
            {
                var step = Vector3.up * Time.deltaTime * angleRotateBodySpeed * -delta;
                BodyTransform.Rotate(step, Space.World);
            }
        }

        /// <summary>
        /// Applies current <see cref="motionInput"/> and <see cref="gravityVelocity"/>
        /// to the <see cref="controller"/>.
        /// </summary>
        private void ApplyMovement()
        {
            var motionDirection = motionInput + gravityVelocity;
            controller.Move(motionDirection * Time.deltaTime);
            motionInput = Vector3.zero;
        }
    }
}
