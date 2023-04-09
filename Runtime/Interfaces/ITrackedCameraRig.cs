// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.SpatialTracking;

namespace RealityToolkit.CameraService.Interfaces
{
    /// <summary>
    /// A <see cref="ICameraRig"/> that is being tracked by hardware sensors.
    /// </summary>
    public interface ITrackedCameraRig : ICameraRig
    {
        /// <summary>
        /// The <see cref="TrackedPoseDriver"/> driving the <see cref="ITrackedCameraRig"/>'s pose.
        /// </summary>
        TrackedPoseDriver PoseDriver { get; }
    }
}