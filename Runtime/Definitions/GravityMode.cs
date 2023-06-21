namespace RealityToolkit.CameraService.Definitions
{
    /// <summary>
    /// Defines when gravity begins to take effect on <see cref="Interfaces.IXRPlayerController"/>'s.
    /// </summary>
    public enum GravityMode
    {
        /// <summary>
        /// Only begin to apply gravity when a move input occurs. Gravity continues applying each frame until the rig is grounded, even if input is stopped.
        /// </summary>
        /// <remarks>
        /// Use this style when you don't want gravity to apply when the player physically walks away and off a ground surface.
        /// Gravity will only begin to move the player back down to the ground when they try to use input to move.
        /// </remarks>
        OnMove = 0,
        /// <summary>
        /// Apply gravity every frame, even without move input.
        /// </summary>
        /// <remarks>
        /// Use this style when you want gravity to apply when the player physically walks away and off a ground surface,
        /// even when there is no input to move.
        /// </remarks>
        Immediately,
        /// <summary>
        /// Do not apply gravity at all.
        /// </summary>
        Disabled
    }
}
