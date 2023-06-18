using UnityEngine;

namespace RealityToolkit.CameraService.UX
{
    /// <summary>
    /// Put this component on a <see cref="GameObject"/> with a <see cref="Collider"/>
    /// attached to signal to the <see cref="Interfaces.ICameraService.CameraRig"/> that the camera
    /// is not supposed to penetrate the collider, so it can raise <see cref="Interfaces.ICameraService.CameraOutOfBounds"/>
    /// and <see cref="Interfaces.ICameraService.CameraBackInBounds"/> events.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CameraOutOfBoundsTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("If set, this trigger will raise camera bounds events to the camera service.")]
        private bool raiseEvents = true;

        /// <summary>
        /// If set, this trigger will raise camera bounds events to the camera service.
        /// </summary>
        /// <remarks>
        /// You can use this property to programmatically decide when a trigger should be blocking
        /// or not to the camera, e.g. when the player has unlocked a new area, set this value to <c>false</c>,
        /// to allow the camera to pass the trigger without being considered out of bounds.
        /// </remarks>
        public bool RaiseEvents
        {
            get => raiseEvents;
            set => raiseEvents = value;
        }
    }
}
