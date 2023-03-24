// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Utilities;
using RealityCollective.Extensions;
using RealityCollective.ServiceFramework;
using RealityCollective.ServiceFramework.Editor;
using RealityCollective.ServiceFramework.Editor.Packages;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.CameraService.Editor
{
    [InitializeOnLoad]
    internal static class CameraPackageInstaller
    {
        private static readonly string destinationPath = Application.dataPath + "/RealityToolkit.Generated/Camera";
        private static readonly string sourcePath = Path.GetFullPath($"{PathFinderUtility.ResolvePath<IPathFinder>(typeof(CameraPackagePathFinder)).ForwardSlashes()}{Path.DirectorySeparatorChar}{"Assets~"}");

        static CameraPackageInstaller()
        {
            EditorApplication.delayCall += CheckPackage;
        }

        [MenuItem(ServiceFrameworkPreferences.Editor_Menu_Keyword + "/Reality Toolkit/Packages/Install Camera Package Assets...", true)]
        private static bool ImportPackageAssetsValidation()
        {
            return !Directory.Exists($"{destinationPath}{Path.DirectorySeparatorChar}");
        }

        [MenuItem(ServiceFrameworkPreferences.Editor_Menu_Keyword + "/Reality Toolkit/Packages/Install Camera Package Assets...")]
        private static void ImportPackageAssets()
        {
            EditorPreferences.Set($"{nameof(CameraPackageInstaller)}.Assets", false);
            EditorApplication.delayCall += CheckPackage;
        }

        private static void CheckPackage()
        {
            if (!EditorPreferences.Get($"{nameof(CameraPackageInstaller)}.Assets", false))
            {
                EditorPreferences.Set($"{nameof(CameraPackageInstaller)}.Assets", AssetsInstaller.TryInstallAssets(sourcePath, destinationPath));
            }
        }
    }
}
