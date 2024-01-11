// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;
using RealityToolkit.PlayerService.Definitions;
using RealityToolkit.PlayerService.Interfaces;

namespace RealityToolkit.PlayerService.Modules
{
    /// <summary>
    /// Default and general use <see cref="ICameraRigServiceModule"/> implementation.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("EA4C0C19-E533-4AE8-91A2-6998CB8905BB")]
    public class DefaultCameraRigServiceModule : BaseCameraRigServiceModule, ICameraRigServiceModule
    {
        /// <inheritdoc />
        public DefaultCameraRigServiceModule(string name, uint priority, BaseCameraRigServiceModuleProfile profile, IPlayerService parentService)
            : base(name, priority, profile, parentService) { }
    }
}
