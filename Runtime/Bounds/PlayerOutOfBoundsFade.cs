// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Player.UX;
using UnityEngine;

namespace RealityToolkit.Player.Bounds
{
    /// <summary>
    /// Uses the <see cref="IPlayerService.PlayerOutOfBounds"/>
    /// event in combination with the <see cref="CameraFade"/> component to
    /// fade the camera out as it progressively goes out of bounds.
    /// </summary>
    [RequireComponent(typeof(CameraFade))]
    public class PlayerOutOfBoundsFade : MonoBehaviour
    {
        private CameraFade cameraFade;
        private IPlayerBoundsModule playerBoundsModule;

        private async void OnEnable()
        {
            cameraFade = GetComponent<CameraFade>();

            await ServiceManager.WaitUntilInitializedAsync();

            if (ServiceManager.Instance.TryGetService(out playerBoundsModule))
            {
                playerBoundsModule.PlayerOutOfBounds += PlayerService_PlayerOutOfBounds;
                playerBoundsModule.PlayerBackInBounds += PlayerService_PlayerBackInBounds;
            }
        }

        private void OnDisable()
        {
            if (playerBoundsModule != null)
            {
                playerBoundsModule.PlayerOutOfBounds -= PlayerService_PlayerOutOfBounds;
                playerBoundsModule.PlayerBackInBounds -= PlayerService_PlayerBackInBounds;
            }
        }

        private void PlayerService_PlayerOutOfBounds(float severity, Vector3 returnToBoundsDirection)
        {
            cameraFade.SetFade(severity);
        }

        private void PlayerService_PlayerBackInBounds(bool didAutoReset)
        {
            cameraFade.SetFade(0f);
        }
    }
}
