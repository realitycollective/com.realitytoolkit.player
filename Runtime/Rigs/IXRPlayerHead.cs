using UnityEngine;

namespace RealityToolkit.Player.Rigs
{
    /// <summary>
    /// The player's head on the <see cref="IXRPlayerController"/>.
    /// </summary>
    public interface IXRPlayerHead
    {
        /// <summary>
        /// The head's pose in world space.
        /// </summary>
        Pose Pose { get; }

        /// <summary>
        /// The head's radius in meters.
        /// </summary>
        float Radius { get; }
    }
}
