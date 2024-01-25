// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityCollective.ServiceFramework.Modules;
using RealityToolkit.Player.Interfaces;
using UnityEngine;

namespace RealityToolkit.Player.Modules
{
    /// <summary>
    /// Default <see cref="IBodyPoseProviderModule"/> implementation that provides a very basic
    /// estimation for the body pose.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("92fef470-917e-4d54-9099-6c3e857b2bbc")]
    public class DefaultBodyPoseProviderModule : BaseServiceModule, IBodyPoseProviderModule
    {
        /// <inheritdoc />
        public DefaultBodyPoseProviderModule(string name, uint priority, BaseProfile profile, IPlayerService parentService)
            : base(name, priority, profile, parentService)
        {
            playerService = parentService;
        }

        private readonly IPlayerService playerService;
        private IXRPlayerController playerController;
        private const float rotateTowardsSpeed = 50f;
        private const float thresholdAngle = 30f;
        private const float largeAngleBoostThreshold = 45f;
        private const float largeAngleBoostMultiplier = 2f;
        private bool shouldReturnToIdle;

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            playerController = playerService.PlayerRig as IXRPlayerController;
            if (playerController == null)
            {
                Debug.LogError($"{nameof(DefaultBodyPoseProviderModule)} works only with {nameof(IXRPlayerController)} implementations.");
                IsEnabled = false;
            }
        }

        /// <inheritdoc />
        public Pose Pose
        {
            get
            {
                if (!IsEnabled)
                {
                    return Pose.identity;
                }

                var bodyPosition = playerController.CameraTransform.position;
                bodyPosition.y = playerController.RigTransform.position.y;
                var bodyRotation = playerController.BodyTransform.rotation;

                var angle = Vector3.Angle(playerController.BodyTransform.forward, playerController.CameraTransform.forward);
                if (angle > thresholdAngle || shouldReturnToIdle)
                {
                    var targetRotation = Quaternion.LookRotation(playerController.CameraTransform.forward, Vector3.up).eulerAngles;
                    targetRotation.x = 0f;
                    targetRotation.z = 0f;

                    var speed = angle > largeAngleBoostThreshold ? largeAngleBoostMultiplier * rotateTowardsSpeed : rotateTowardsSpeed;
                    bodyRotation = Quaternion.RotateTowards(playerController.BodyTransform.rotation, Quaternion.Euler(targetRotation), speed * Time.deltaTime);
                    shouldReturnToIdle = angle > 0;
                }

                return new Pose(bodyPosition, bodyRotation);
            }
        }
    }
}
