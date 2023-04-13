// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraService.UX
{
    /// <summary>
    /// Uses the <see cref="Interfaces.ICameraService.CameraOutOfBounds"/>
    /// event in combination with the <see cref="CameraFade"/> component to
    /// fade the camera out as it progressively goes out of bounds.
    /// </summary>
    [RequireComponent(typeof(CameraFade))]
    public class CameraOutOfBoundsFade : MonoBehaviour
    {
        private CameraFade cameraFade;

        private ICameraService cameraService;
        /// <summary>
        /// Lazy loaded reference to the active <see cref="ICameraService"/>.
        /// </summary>
        private ICameraService CameraService => cameraService ??= ServiceManager.Instance.GetService<ICameraService>();

        private void OnEnable()
        {
            cameraFade = GetComponent<CameraFade>();

            CameraService.CameraOutOfBounds += CameraService_CameraOutOfBounds;
            CameraService.CameraBackInBounds += CameraService_CameraBackInBounds;
        }

        private void OnDisable()
        {
            if (CameraService != null)
            {
                CameraService.CameraOutOfBounds -= CameraService_CameraOutOfBounds;
                CameraService.CameraBackInBounds -= CameraService_CameraBackInBounds;
            }
        }

        private void CameraService_CameraOutOfBounds(float severity, Vector3 returnToBoundsDirection)
        {
            cameraFade.SetFade(severity);
        }

        private void CameraService_CameraBackInBounds()
        {
            cameraFade.SetFade(0f);
        }
    }
}
