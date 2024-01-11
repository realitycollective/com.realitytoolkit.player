// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.PlayerService.Interfaces;
using UnityEngine;

namespace RealityToolkit.PlayerService.UX
{
    /// <summary>
    /// Uses the <see cref="IPlayerService.CameraOutOfBounds"/>
    /// event in combination with the <see cref="CameraFade"/> component to
    /// fade the camera out as it progressively goes out of bounds.
    /// </summary>
    [RequireComponent(typeof(CameraFade))]
    public class CameraOutOfBoundsFade : MonoBehaviour
    {
        private CameraFade cameraFade;
        private ICameraBoundsModule cameraBoundsModule;

        private async void OnEnable()
        {
            cameraFade = GetComponent<CameraFade>();

            await ServiceManager.WaitUntilInitializedAsync();

            if (ServiceManager.Instance.TryGetService(out cameraBoundsModule))
            {
                cameraBoundsModule.CameraOutOfBounds += PlayerService_CameraOutOfBounds;
                cameraBoundsModule.CameraBackInBounds += PlayerService_CameraBackInBounds;
            }
        }

        private void OnDisable()
        {
            if (cameraBoundsModule != null)
            {
                cameraBoundsModule.CameraOutOfBounds -= PlayerService_CameraOutOfBounds;
                cameraBoundsModule.CameraBackInBounds -= PlayerService_CameraBackInBounds;
            }
        }

        private void PlayerService_CameraOutOfBounds(float severity, Vector3 returnToBoundsDirection)
        {
            cameraFade.SetFade(severity);
        }

        private void PlayerService_CameraBackInBounds()
        {
            cameraFade.SetFade(0f);
        }
    }
}
