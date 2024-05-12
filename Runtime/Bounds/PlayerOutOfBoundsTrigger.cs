// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Player.Bounds
{
    /// <summary>
    /// Put this component on a <see cref="GameObject"/> with a <see cref="Collider"/>
    /// attached to raise <see cref="IPlayerBoundsModule.PlayerOutOfBounds"/>
    /// and <see cref="IPlayerBoundsModule.PlayerBackInBounds"/> events.
    /// </summary>
    /// <remarks>
    /// This component relies on the <see cref="Rigs.XRPlayerController"/> rig and specifically
    /// on the <see cref="Rigs.XRPlayerHead"/> being present on that rig.
    /// </remarks>
    [RequireComponent(typeof(Collider))]
    public class PlayerOutOfBoundsTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("If set, this trigger will raise player bounds events to the player service.")]
        private bool raiseEvents = true;

        /// <summary>
        /// If set, this trigger will raise player bounds events to the player service.
        /// </summary>
        /// <remarks>
        /// You can use this property to programmatically decide when a trigger should be blocking
        /// or not to the player, e.g. when the player has unlocked a new area, set this value to <c>false</c>,
        /// to allow the player to pass the trigger without being considered out of bounds.
        /// </remarks>
        public bool RaiseEvents
        {
            get => raiseEvents;
            set => raiseEvents = value;
        }

        /// <inheritdoc />
        protected virtual void OnEnable()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }
    }
}
