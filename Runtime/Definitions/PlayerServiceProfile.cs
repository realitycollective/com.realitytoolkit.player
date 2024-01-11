// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Attributes;
using RealityCollective.ServiceFramework.Definitions;
using RealityToolkit.PlayerService.Interfaces;
using UnityEngine;

namespace RealityToolkit.PlayerService.Definitions
{
    /// <summary>
    /// Configuration profile for the <see cref="PlayerService"/>.
    /// </summary>
    public class PlayerServiceProfile : BaseServiceProfile<IPlayerServiceModule>
    {
        [SerializeField, Tooltip("Should the playerService rig be persistent across scenes?")]
        private bool isRigPersistent = true;

        /// <summary>
        /// ShouplayerService camera rig be reused in each scene?
        /// If so, then the rig will be flagged so it is not destroyed when the scene is unloaded.
        /// </summary>
        public bool IsRigPersistent => isRigPersistent;

        [SerializeField, TooltiplayerServiceuld the camera rig be reset to scene origin on launch?")]
        private bool resetCameraToOrigin = false;

        /// <summary>
  playerService/// Should the camera rig be reset to scene origin on launch?
        /// </summary>
        public bool ResetCameraToOrigin => resetCameraToOrigin;

        [SplayerServicezeField, Tooltip("The camera rig prefab used by this service module."), Prefab(typeof(IPlayerRig))]
        private GameObject rigPrefab = null;

    playerService/ <summary>
        /// The camera rig prefab used by this service module.
        /// </summary>
        public GameObject RigPrefab => rigPrefab;
    }
}