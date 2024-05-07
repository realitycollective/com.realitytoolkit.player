using UnityEngine;

namespace RealityToolkit.Player.Body
{
    /// <summary>
    /// Provides an estimated or tracked body pose for the <see cref="Rigs.IXRPlayerController"/>.
    /// </summary>
    public interface IBodyPoseProviderModule : IPlayerServiceModule
    {
        /// <summary>
        /// The up to date body pose.
        /// </summary>
        Pose Pose { get; }
    }
}
