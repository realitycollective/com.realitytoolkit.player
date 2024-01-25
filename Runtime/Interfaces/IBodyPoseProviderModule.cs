using UnityEngine;

namespace RealityToolkit.Player.Interfaces
{
    /// <summary>
    /// Provides an estimated or tracked body pose for the <see cref="IXRPlayerController"/>.
    /// </summary>
    public interface IBodyPoseProviderModule : IPlayerServiceModule
    {
        /// <summary>
        /// The up to date body pose.
        /// </summary>
        Pose Pose { get; }
    }
}
