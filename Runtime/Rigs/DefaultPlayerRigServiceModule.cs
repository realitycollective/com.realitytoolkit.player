// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Definitions.Platforms;

namespace RealityToolkit.Player.Rigs
{
    /// <summary>
    /// Default and general use <see cref="IPlayerRigServiceModule"/> implementation.
    /// </summary>
    [RuntimePlatform(typeof(AllPlatforms))]
    [System.Runtime.InteropServices.Guid("EA4C0C19-E533-4AE8-91A2-6998CB8905BB")]
    public class DefaultPlayerRigServiceModule : BasePlayerRigServiceModule, IPlayerRigServiceModule
    {
        /// <inheritdoc />
        public DefaultPlayerRigServiceModule(string name, uint priority, BasePlayerRigServiceModuleProfile profile, IPlayerService parentService)
            : base(name, priority, profile, parentService) { }
    }
}
