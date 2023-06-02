// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.SpatialTracking;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// A XR <see cref="ICameraRig"/> that is being tracked by hardware sensors.
    /// </summary>
    public interface IXRCameraRig : ICameraRig
    {
        /// <summary>
        /// Is the <see cref="ICameraRig.RigCamera"/> camera displaying on a traditional 2d screen or a stereoscopic display?
        /// </summary>
        bool IsStereoscopic { get; }

        /// <summary>
        /// Is the <see cref="ICameraRig.RigCamera"/> displaying in opaque (VR) or transparent mode (XR)?.
        /// </summary>
        bool IsOpaque { get; }

        /// <summary>
        /// The <see cref="TrackedPoseDriver"/> driving the <see cref="IXRCameraRig"/>'s pose.
        /// </summary>
        TrackedPoseDriver PoseDriver { get; }
    }
}