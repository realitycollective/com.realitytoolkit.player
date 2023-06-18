// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using UnityEngine;

namespace RealityToolkit.CameraService
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class XRPlayerHead : MonoBehaviour
    {
        private XRPlayerController controller;
        private SphereCollider sphereCollider;

        /// <summary>
        /// The radius of the head measured in the object's local space.
        /// </summary>
        public float Radius => sphereCollider.radius;

        /// <inheritdoc />
        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
            controller = GetComponentInParent<XRPlayerController>();
            if (controller.IsNull())
            {
                Debug.LogError($"{nameof(XRPlayerHead)}must be parented to {nameof(XRPlayerController)}.");
            }
        }

        /// <inheritdoc />
        private void OnValidate()
        {
            sphereCollider = GetComponent<SphereCollider>();
        }

        /// <inheritdoc />
        private void OnTriggerEnter(Collider other)
        {

        }
    }
}
