// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Player.Rigs;
using UnityEngine;

namespace RealityToolkit.Player.Bounds
{
    /// <summary>
    /// Put this component on a <see cref="GameObject"/> with a <see cref="Collider"/>
    /// attached to raise <see cref="IPlayerBoundsModule.PlayerOutOfBounds"/>
    /// and <see cref="IPlayerBoundsModule.PlayerBackInBounds"/> events.
    /// </summary>
    /// <remarks>
    /// This component relies on the <see cref="XRPlayerController"/> rig and specifically
    /// on the <see cref="XRPlayerHead"/> being present on that rig.
    /// </remarks>
    [RequireComponent(typeof(Collider))]
    public class PlayerOutOfBoundsTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("If set, this trigger will raise player bounds events to the player service.")]
        private bool raiseEvents = true;

        private IPlayerBoundsModule playerBoundsModule;

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
        protected virtual async void Awake()
        {
            await ServiceManager.WaitUntilInitializedAsync();

            if (!ServiceManager.Instance.TryGetService(out playerBoundsModule))
            {
                Debug.LogError($"{nameof(PlayerOutOfBoundsTrigger)} requires the {nameof(IPlayerBoundsModule)} to work.", this);
            }
        }

        /// <inheritdoc />
        protected virtual void OnEnable()
        {
            var collider = GetComponent<Collider>();
            if (!collider.isTrigger)
            {
                collider.isTrigger = true;
                Debug.LogWarning($"{nameof(PlayerOutOfBoundsTrigger)} requires the attached {nameof(Collider)} to be a trigger and has auto configured it.", this);
            }
        }

        /// <inheritdoc />
        private void OnTriggerEnter(Collider other)
        {
            if (playerBoundsModule == null || !RaiseEvents ||
                !other.TryGetComponent<XRPlayerHead>(out _))
            {
                return;
            }

            playerBoundsModule.OnTriggerEnter(this);
        }

        /// <inheritdoc />
        protected virtual void OnTriggerStay(Collider other)
        {
            if (playerBoundsModule == null || !RaiseEvents ||
                !other.TryGetComponent<XRPlayerHead>(out _))
            {
                return;
            }

            playerBoundsModule.OnTriggerStay(this);
        }

        /// <inheritdoc />
        protected virtual void OnTriggerExit(Collider other)
        {
            if (playerBoundsModule == null || !RaiseEvents ||
                !other.TryGetComponent<XRPlayerHead>(out _))
            {
                return;
            }

            playerBoundsModule.OnTriggerExit(this);
        }
    }
}
