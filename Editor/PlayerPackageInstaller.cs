// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Editor;
using RealityCollective.ServiceFramework.Editor.Packages;
using RealityCollective.Utilities.Editor;
using RealityCollective.Utilities.Extensions;
using RealityToolkit.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Player.Editor
{
    [InitializeOnLoad]
    internal static class PlayerPackageInstaller
    {
        private static readonly string destinationPath = Application.dataPath + "/RealityToolkit.Generated/Player";
        private static readonly string sourcePath = Path.GetFullPath($"{PathFinderUtility.ResolvePath<IPathFinder>(typeof(PlayerPackagePathFinder)).ForwardSlashes()}{Path.DirectorySeparatorChar}{"Assets~"}");

        static PlayerPackageInstaller()
        {
            EditorApplication.delayCall += CheckPackage;
        }

        [MenuItem(RealityToolkitPreferences.Editor_Menu_Keyword + "/Packages/Install Player Package Assets...", true)]
        private static bool ImportPackageAssetsValidation()
        {
            return !Directory.Exists($"{destinationPath}{Path.DirectorySeparatorChar}");
        }

        [MenuItem(RealityToolkitPreferences.Editor_Menu_Keyword + "/Packages/Install Player Package Assets...")]
        private static void ImportPackageAssets()
        {
            EditorPreferences.Set($"{nameof(PlayerPackageInstaller)}.Assets", false);
            EditorApplication.delayCall += CheckPackage;
        }

        private static void CheckPackage()
        {
            if (!EditorPreferences.Get($"{nameof(PlayerPackageInstaller)}.Assets", false))
            {
                EditorPreferences.Set($"{nameof(PlayerPackageInstaller)}.Assets", AssetsInstaller.TryInstallAssets(sourcePath, destinationPath));
            }
        }
    }
}
