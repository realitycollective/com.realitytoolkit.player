// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;

namespace RealityToolkit.CameraService.UX
{
    /// <summary>
    /// Uses the <see cref="ICameraService.CameraOutOfBounds"/>
    /// event in combination with the <see cref="CameraFade"/> component to
    /// fade the camera out as it progressively goes out of bounds.
    /// </summary>
    [RequireComponent(typeof(CameraFade))]
    public class CameraOutOfBoundsFade : MonoBehaviour
    {
        private CameraFade cameraFade;
        private ICameraService cameraService;

        private async void OnEnable()
        {
            cameraFade = GetComponent<CameraFade>();

            try
            {
                await ServiceManager.WaitUntilInitializedAsync();
                cameraService = await ServiceManager.Instance.GetServiceAsync<ICameraService>();
                cameraService.CameraOutOfBounds += CameraService_CameraOutOfBounds;
                cameraService.CameraBackInBounds += CameraService_CameraBackInBounds;
            }
            catch
            {
                Debug.LogError($"{nameof(CameraOutOfBoundsFade)} was not able to register to {nameof(ICameraService)} events.");
            }
        }

        private void OnDisable()
        {
            if (cameraService != null)
            {
                cameraService.CameraOutOfBounds -= CameraService_CameraOutOfBounds;
                cameraService.CameraBackInBounds -= CameraService_CameraBackInBounds;
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
