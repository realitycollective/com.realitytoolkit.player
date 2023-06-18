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

    }
}
