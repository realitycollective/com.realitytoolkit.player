// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Extensions;
using System.Collections;
using UnityEngine;

namespace RealityToolkit.CameraService.UX
{
    /// <summary>
    /// Fades the camera from and to <see cref="fadeColor"/>.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFade : MonoBehaviour
    {
        [SerializeField, Tooltip("The color to fade from and to.")]
        private Color fadeColor = Color.black;

        [SerializeField, Tooltip("Duration in seconds to fully fade in / out.")]
        private float fadeDuration = 1f;

        [SerializeField, Tooltip("The material used to fade. This must be a transparency enabled material.")]
        private Material fadeMaterial;

        [SerializeField, Tooltip("If set, the camera will fade in on start.")]
        private bool fadeOnStart = true;

        private MeshRenderer fadeRenderer;
        private MeshFilter fadeMesh;
        private bool isFading;

        private void Start()
        {
            fadeMesh = gameObject.AddComponent<MeshFilter>();
            fadeMesh.mesh = CreateMesh();

            fadeRenderer = gameObject.AddComponent<MeshRenderer>();
            fadeRenderer.material = fadeMaterial;
            fadeRenderer.receiveShadows = false;
            fadeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            fadeRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            fadeRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            if (fadeOnStart)
            {
                FadeIn();
            }
        }

        private void OnDestroy()
        {
            if (fadeRenderer.IsNotNull())
            {
                fadeRenderer.Destroy();
            }

            if (fadeMesh.IsNotNull())
            {
                fadeMesh.Destroy();
            }
        }

        /// <summary>
        /// Manually fades in the camera.
        /// </summary>
        public void FadeIn()
        {
            StartCoroutine(Fade(1f, 0f));
        }

        /// <summary>
        /// Manually fades out the camera.
        /// </summary>
        public void FadeOut()
        {
            StartCoroutine(Fade(0f, 1f));
        }

        /// <summary>
        /// Sets the fade alpha value on the camera.
        /// </summary>
        /// <param name="alpha">The fade intensity.</param>
        public void SetFade(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            var color = fadeColor;
            color.a = alpha;
            isFading = color.a > 0;

            var material = fadeRenderer.material;
            material.color = color;
            fadeRenderer.material = material;
            fadeRenderer.enabled = isFading;
        }

        private Mesh CreateMesh()
        {
            var mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];

            float width = 2f;
            float height = 2f;
            float depth = 1f;

            vertices[0] = new Vector3(-width, -height, depth);
            vertices[1] = new Vector3(width, -height, depth);
            vertices[2] = new Vector3(-width, height, depth);
            vertices[3] = new Vector3(width, height, depth);

            mesh.vertices = vertices;

            int[] tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            mesh.uv = uv;

            return mesh;
        }

        private IEnumerator Fade(float startAlpha, float endAlpha)
        {
            var elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                var frameAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeDuration));
                SetFade(frameAlpha);
                yield return new WaitForEndOfFrame();
            }

            SetFade(endAlpha);
        }
    }
}
