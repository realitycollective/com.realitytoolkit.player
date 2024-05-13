// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Utilities.Extensions;
using UnityEngine;

namespace RealityToolkit.Player.Rigs
{
    /// <summary>
    /// This component represents the player's head on the <see cref="XRPlayerController"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class XRPlayerHead : MonoBehaviour, IXRPlayerHead
    {
        [SerializeField, Tooltip("The sphere collider wrapping the player's head.")]
        private SphereCollider sphereCollider;

        private XRPlayerController controller;

        /// <inheritdoc />
        public Pose Pose => new Pose(transform.position, transform.rotation);

        /// <inheritdoc />
        public float Radius => sphereCollider.radius;

        /// <inheritdoc />
        protected virtual void Awake()
        {
            controller = GetComponentInParent<XRPlayerController>();
            if (controller.IsNull())
            {
                Debug.LogError($"{nameof(XRPlayerHead)} must be parented to {nameof(XRPlayerController)}.");
            }
        }
    }
}
