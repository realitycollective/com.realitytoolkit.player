// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.CameraService.Interfaces;
using RealityToolkit.CameraService.UX;
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
    [RequireComponent(typeof(CharacterController))]
    [System.Runtime.InteropServices.Guid("3ddace9b-b75e-46d0-9b62-2b169e0c35d5")]
    public class XRPlayerController : TrackedCameraRig, IPlayerCameraRig
    {
        [SerializeField, Tooltip("The head collider is used to offset the body/character collider.")]
        private SphereCollider head = null;

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

        private bool wasOutOfBounds;
        private Vector3 verticalVelocity;
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
            Gizmos.DrawWireSphere(head.transform.position, head.radius);
            Gizmos.color = prevoiusColor;
        }

        /// <inheritdoc />
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<CameraBoundsCollider>(out _))
            {

            }
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

            var combinedDirection = (forwardDirection * direction.z + rightDirection * direction.x).normalized;

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

            var height = Mathf.Max(bodyDiameter, head.transform.localPosition.y - head.radius);
            controller.height = height;
            controller.stepOffset = .2f * controller.height;

            var center = CameraTransform.localPosition;
            center.y = height / 2f + controller.skinWidth;
            controller.center = center;
        }

        private void UpdateGravity()
        {
            switch (gravityMode)
            {
                case GravityMode.OnMove:
                case GravityMode.Immediately:
                    {
                        if (controller.isGrounded ||
                            (motionInput == Vector3.zero && gravityMode == GravityMode.OnMove && verticalVelocity == Vector3.zero))
                        {
                            verticalVelocity = Vector3.zero;
                            return;
                        }

                        verticalVelocity = Physics.gravity;
                    }
                    break;
                case GravityMode.Disabled:
                    verticalVelocity = Vector3.zero;
                    return;
                default:
                    Debug.LogError($"{nameof(RealityToolkit.CameraService.GravityMode)}.{gravityMode} not supported.");
                    break;
            }
        }

        /// <summary>
        /// Uses the head / camera's forward orientation to estimate the orientation
        /// of the body / torso of the player.
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

        private void CheckCameraBounds()
        {
            const float lowerThresholdFactor = .2f;

            var headPosition = CameraTransform.position;
            headPosition.y = 0f;
            var bodyPosition = BodyTransform.position;
            bodyPosition.y = 0f;

            var headToBodyOffset = Vector3.Distance(headPosition, bodyPosition);
            var severity = 0f;
            var cameraOutOfBoundsLowerThreshold = lowerThresholdFactor * controller.radius;
            var cameraOutOfBoundsUpperThreshold = controller.radius;

            if (headToBodyOffset >= cameraOutOfBoundsUpperThreshold)
            {
                severity = 1f;
            }
            else if (headToBodyOffset >= cameraOutOfBoundsLowerThreshold)
            {
                var range = cameraOutOfBoundsUpperThreshold - cameraOutOfBoundsLowerThreshold;
                var value = headToBodyOffset - cameraOutOfBoundsLowerThreshold;
                severity = value / range;
            }

            if (severity > 0f)
            {
                CameraService.RaiseCameraOutOfBounds(severity, (bodyPosition - headPosition).normalized);
            }
            else if (severity <= 0f && wasOutOfBounds)
            {
                CameraService.RaiseCameraBackInBounds();
            }

            wasOutOfBounds = severity > 0f;
        }

        private void ApplyMovement()
        {
            var motionDirection = motionInput + verticalVelocity;
            controller.Move(motionDirection * Time.deltaTime);
            motionInput = Vector3.zero;
        }
    }
}