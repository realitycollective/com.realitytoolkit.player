// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Player.Rigs;
using UnityEngine;
using UnityEngine.Events;

namespace RealityToolkit.Player.UX
{
    /// <summary>
    /// A simple utility component that will raise events w
    /// </summary>
    [HelpURL("https://www.realitytoolkit.io/docs/category/player")]
    [RequireComponent(typeof(Collider))]
    public class PlayerTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("Raised, when the player enters the trigger zone.")]
        private UnityEvent onPlayerEnter = null;

        [Space, SerializeField, Tooltip("Raised, while the player is within the trigger zone.")]
        private UnityEvent onPlayerStay = null;

        [Space, SerializeField, Tooltip("Raised, when the player leaves the trigger zone.")]
        private UnityEvent onPlayerExit = null;

        /// <summary>
        /// Raised, when the player enters the trigger zone.
        /// </summary>
        public UnityEvent OnPlayerEnter => onPlayerEnter;

        /// <summary>
        /// Raised, while the player is within the trigger zone.
        /// </summary>
        public UnityEvent OnPlayerStay => onPlayerStay;

        /// <summary>
        /// Raised, when the player leaves the trigger zone.
        /// </summary>
        public UnityEvent OnPlayerExit => onPlayerExit;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IPlayerRig>(out _))
            {
                return;
            }

            OnPlayerEnter?.Invoke();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent<IPlayerRig>(out _))
            {
                return;
            }

            OnPlayerStay?.Invoke();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IPlayerRig>(out _))
            {
                return;
            }

            OnPlayerExit?.Invoke();
        }
    }
}
