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
        [SerializeField, Tooltip("Should the camera rig be persistent across scenes?")]
        private bool isRigPersistent = true;

        /// <summary>
        /// Should the camera rig be reused in each scene?
        /// If so, then the rig will be flagged so it is not destroyed when the scene is unloaded.
        /// </summary>
        public bool IsRigPersistent => isRigPersistent;

        [SerializeField, Tooltip("The camera rig prefab used by this service module."), Prefab(typeof(ICameraRig))]
        private GameObject rigPrefab = null;

        /// <summary>
        /// The camera rig prefab used by this service module.
        /// </summary>
        public GameObject RigPrefab => rigPrefab;
    }
}