// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Editor;
using System.IO;
using UnityEditor;

namespace RealityToolkit.CameraService.Editor
{
    [InitializeOnLoad]
    internal static class CameraPackageInstaller
    {
        private static readonly string DefaultPath = "/Assets/RealityToolkit.Generated/Camera";
        private static readonly string HiddenPath = Path.GetFullPath($"{PathFinderUtility.ResolvePath<IPathFinder>(typeof(CameraPathFinder)).ForwardSlashes()}{Path.DirectorySeparatorChar}{"Assets~"}");

        static CameraPackageInstaller()
        {
            EditorApplication.delayCall += CheckPackage;
        }

        [MenuItem("Reality Collective/Reality Toolkit/Packages/Install Camera Package Assets...", true)]
        private static bool ImportPackageAssetsValidation()
        {
            return !Directory.Exists($"{DefaultPath}{Path.DirectorySeparatorChar}");
        }

        [MenuItem("Reality Collective/Reality Toolkit/Packages/Install Camera Package Assets...")]
        private static void ImportPackageAssets()
        {
            EditorPreferences.Set($"{nameof(CameraPackageInstaller)}.Assets", false);
            EditorApplication.delayCall += CheckPackage;
        }

        private static void CheckPackage()
        {
            if (!EditorPreferences.Get($"{nameof(CameraPackageInstaller)}.Assets", false))
            {
                EditorPreferences.Set($"{nameof(CameraPackageInstaller)}.Assets", PackageInstaller.TryInstallAssets(HiddenPath, DefaultPath));
            }
        }
    }
}
