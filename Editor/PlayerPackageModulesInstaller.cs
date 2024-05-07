// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Editor.Packages;
using RealityCollective.ServiceFramework.Services;
using System.Linq;
using UnityEditor;

namespace RealityToolkit.Player.Editor
{
    /// <summary>
    /// Installs <see cref="IPlayerServiceModule"/>s coming from a third party package
    /// into the <see cref="PlayerServiceProfile"/> in the <see cref="ServiceManager.ActiveProfile"/>.
    /// </summary>
    [InitializeOnLoad]
    public sealed class PlayerPackageModulesInstaller : IPackageModulesInstaller
    {
        /// <summary>
        /// Statis initalizer for the installer instance.
        /// </summary>
        static PlayerPackageModulesInstaller()
        {
            if (Instance == null)
            {
                Instance = new PlayerPackageModulesInstaller();
            }

            PackageInstaller.RegisterModulesInstaller(Instance);
        }

        /// <summary>
        /// Internal singleton instance of the installer.
        /// </summary>
        private static PlayerPackageModulesInstaller Instance { get; }

        /// <inheritdoc/>
        public bool Install(ServiceConfiguration serviceConfiguration)
        {
            if (!typeof(IPlayerServiceModule).IsAssignableFrom(serviceConfiguration.InstancedType.Type))
            {
                // This module installer does not accept the configuration type.
                return false;
            }

            if (!ServiceManager.IsActiveAndInitialized)
            {
                UnityEngine.Debug.LogWarning($"Could not install {serviceConfiguration.InstancedType.Type.Name}.{nameof(ServiceManager)} is not initialized.");
                return false;
            }

            if (!ServiceManager.Instance.HasActiveProfile)
            {
                UnityEngine.Debug.LogWarning($"Could not install {serviceConfiguration.InstancedType.Type.Name}.{nameof(ServiceManager)} has no active profile.");
                return false;
            }

            if (!ServiceManager.Instance.TryGetServiceProfile<IPlayerService, PlayerServiceProfile>(out var playerServiceProfile))
            {
                UnityEngine.Debug.LogWarning($"Could not install {serviceConfiguration.InstancedType.Type.Name}.{nameof(PlayerServiceProfile)} not found.");
                return false;
            }

            // Setup the configuration.
            var typedServiceConfiguration = new ServiceConfiguration<IPlayerServiceModule>(serviceConfiguration.InstancedType.Type, serviceConfiguration.Name, serviceConfiguration.Priority, serviceConfiguration.RuntimePlatforms, serviceConfiguration.Profile);

            // Make sure it's not already in the target profile.
            if (playerServiceProfile.ServiceConfigurations.All(sc => sc.InstancedType.Type != serviceConfiguration.InstancedType.Type))
            {
                playerServiceProfile.AddConfiguration(typedServiceConfiguration);
                UnityEngine.Debug.Log($"Successfully installed the {serviceConfiguration.InstancedType.Type.Name} to {playerServiceProfile.name}.");
            }
            else
            {
                UnityEngine.Debug.Log($"Skipped installing the {serviceConfiguration.InstancedType.Type.Name} to {playerServiceProfile.name}. Already installed.");
            }

            return true;
        }
    }
}
