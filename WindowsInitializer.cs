/*
Copyright (C) 2018  Prism Framework Team

This file is part of the Prism Framework.

The Prism Framework is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

The Prism Framework is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Native;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration.Pnp;
using Windows.Storage;

namespace Prism.Windows
{
    /// <summary>
    /// Represents a platform initializer for Windows.
    /// </summary>
    public sealed class WindowsInitializer : PlatformInitializer
    {
        private WindowsInitializer()
        {
        }

        /// <summary>
        /// Initializes the platform and loads the specified <see cref="Prism.Application"/> instance.
        /// </summary>
        /// <param name="appInstance">The application instance to be loaded.</param>
        public static async void Initialize(Prism.Application appInstance)
        {
            List<Assembly> assemblies = null;
            if (!HasInitialized)
            {
                CoreApplication.MainView.CoreWindow.Activate();

                assemblies = new List<Assembly>();
                var folder = Package.Current.InstalledLocation;
                var exclusions = new string[]
                {
                    "System",
                    "Microsoft",
                    "Internal",
                    "ClrCompression",
                    "PrismFramework"
                };

                var task = folder.GetFilesAsync().AsTask();
                var files = task.Result;

                foreach (StorageFile file in files)
                {
                    if (file.FileType != ".dll" && file.FileType != ".exe")
                    {
                        continue;
                    }
                    
                    if (file.Name.StartsWith("PrismFramework.Windows"))
                    {
                        AssemblyName name = new AssemblyName() { Name = file.DisplayName };
                        assemblies.Insert(0, Assembly.Load(name));
                    }
                    else if (!exclusions.Any(e => file.Name.StartsWith(e)))
                    {
                        try
                        {
                            AssemblyName name = new AssemblyName() { Name = file.DisplayName };
                            assemblies.Add(Assembly.Load(name));
                        }
                        catch { }
                    }
                }

                // UWP doesn't provide an API for reading the OS version, so the best we can do is gather up all of the
                // current MS devices and guess the version based on the frequency of version numbers from the devices.
                try
                {
                    string versionId = "{A8B865DD-2E3D-4094-AD97-E593A70C75D6} 3";
                    string ownerId = "{A8B865DD-2E3D-4094-AD97-E593A70C75D6} 9";

                    var version = (await PnpObject.FindAllAsync(PnpObjectType.Device, new[]
                    {
                        versionId, ownerId
                    }, @"System.Devices.ContainerId:=""{00000000-0000-0000-FFFF-FFFFFFFFFFFF}"""))
                    .Select(d =>
                    {
                        object o;
                        d.Properties.TryGetValue(ownerId, out o);

                        object v;
                        d.Properties.TryGetValue(versionId, out v);

                        return new
                        {
                            Owner = o as string,
                            Version = v as string
                        };
                    })
                    .Where(d => d.Owner == "Microsoft" && d.Version != null)
                    .Select(d => d.Version)
                    .GroupBy(v => v.Substring(0, v.IndexOf('.', v.IndexOf('.') + 1)))
                    .OrderByDescending(d => d.Count())
                    .First().Key;

                    Systems.Device.SystemVersion = new Version(version);
                }
                catch { }
            }

            Initialize(appInstance, assemblies?.ToArray());
        }
    }
}