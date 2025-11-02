// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Utilities.Extensions;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace RealityToolkit.Player.UX
{
    /// <summary>
    /// Fades the camera from and to <see cref="_fadeColor"/>.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFade : MonoBehaviour
    {
        [SerializeField, Tooltip("The color to fade from and to.")]
        private Color _fadeColor = Color.black;

        [SerializeField, Tooltip("Duration in seconds to fully fade in / out.")]
        private float _fullFadeDuration = 1f;

        [SerializeField, Tooltip("The material used to fade. This must be a transparency enabled material.")]
        private Material _fadeMaterial;

        [SerializeField, Tooltip("If set, the camera will fade in on start.")]
        private bool _fadeOnStart = true;

        private MeshRenderer _fadeRenderer;
        private GameObject _fadeCube;
        private bool _isFading;
        private Coroutine _fadeCoroutine;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void Start()
        {
            _fadeCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _fadeCube.EnsureComponentDestroyed<BoxCollider>();
            _fadeCube.transform.SetParent(transform, false);

            _fadeRenderer = _fadeCube.GetComponent<MeshRenderer>();
            _fadeRenderer.material = _fadeMaterial;
            _fadeRenderer.receiveShadows = false;
            _fadeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _fadeRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _fadeRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            if (_fadeOnStart)
            {
                SetFade(1f);
                _ = FadeInAsync();
                return;
            }

            SetFade(0f);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnDestroy()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            if (_fadeCube.IsNotNull())
            {
                _fadeCube.Destroy();
            }
        }

        /// <summary>
        /// Manually fades in the camera.
        /// </summary>
        public async Task FadeInAsync()
        {
            var color = _fadeRenderer.material.color;
            var startAlpha = color.a;
            var duration = Mathf.Abs(startAlpha) * _fullFadeDuration;

            _fadeCoroutine = StartCoroutine(Fade(startAlpha, 0f, duration));

            while (_fadeCoroutine != null)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Manually fades out the camera.
        /// </summary>
        public async Task FadeOutAsync()
        {
            var color = _fadeRenderer.material.color;
            var startAlpha = color.a;
            var duration = Mathf.Abs(startAlpha - 1f) * _fullFadeDuration;

            _fadeCoroutine = StartCoroutine(Fade(startAlpha, 1f, duration));

            while (_fadeCoroutine != null)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Sets the fade alpha value on the camera.
        /// </summary>
        /// <param name="alpha">The fade intensity.</param>
        public void SetFade(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            var color = _fadeColor;
            color.a = alpha;
            _isFading = color.a > 0;

            var material = _fadeRenderer.material;
            material.color = color;
            _fadeRenderer.material = material;
            _fadeRenderer.enabled = _isFading;
        }

        private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var frameAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / duration));
                SetFade(frameAlpha);
                yield return new WaitForEndOfFrame();
            }

            SetFade(endAlpha);
        }
    }
}
