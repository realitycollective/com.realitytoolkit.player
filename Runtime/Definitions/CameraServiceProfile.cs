// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Attributes;
using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraService.Definitions
{
    /// <summary>
    /// Configuration profile for the <see cref="CameraService"/>.
    /// </summary>
    public class CameraServiceProfile : BaseServiceProfile<ICameraServiceModule>
    {
        [SerializeField, Tooltip("The camera rig prefab used by this service module."), Prefab(typeof(ICameraRig))]
        private GameObject rigPrefab = null;

        /// <summary>
        /// The camera rig prefab used by this service module.
        /// </summary>
        public GameObject RigPrefab => rigPrefab;
    }
}