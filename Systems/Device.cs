/*
Copyright (C) 2016  Prism Framework Team

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
using Prism.Native;
using Prism.Systems;
using Windows.Graphics.Display;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.System.Profile;
using Windows.System.Power;
using Windows.UI.ViewManagement;

namespace Prism.Windows.Systems
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeDevice"/>.
    /// </summary>
    [Register(typeof(INativeDevice), IsSingleton = true)]
    public class Device : INativeDevice
    {
        // temporary storage for OS version
        internal static Version SystemVersion { get; set; }

        /// <summary>
        /// Occurs when the battery level of the device has changed by at least 1 percent.
        /// </summary>
        public event EventHandler BatteryLevelChanged;

        /// <summary>
        /// Occurs when the orientation of the device has changed.
        /// </summary>
        public event EventHandler OrientationChanged;

        /// <summary>
        /// Occurs when the power source of the device has changed.
        /// </summary>
        public event EventHandler PowerSourceChanged;

        /// <summary>
        /// Gets the battery level of the device as a percentage value between 0 (empty) and 100 (full).
        /// </summary>
        public int BatteryLevel
        {
            get { return PowerManager.RemainingChargePercent; }
        }

        /// <summary>
        /// Gets the scaling factor of the display monitor.
        /// </summary>
        public double DisplayScale
        {
            get { return DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel; }
        }

        /// <summary>
        /// Gets the form factor of the device on which the application is running.
        /// </summary>
        public FormFactor FormFactor
        {
            get
            {
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    return FormFactor.Phone;
                }

                return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse ? FormFactor.Desktop : FormFactor.Tablet;
            }
        }

        /// <summary>
        /// Gets a unique identifier for the device.
        /// </summary>
        public string Id
        {
            get
            {
                var token = HardwareIdentification.GetPackageSpecificToken(null);
                var hasher = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
                var hashed = hasher.HashData(token.Id);
                return CryptographicBuffer.EncodeToHexString(hashed);

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the orientation of the device should be monitored.
        /// This affects the ability to read the orientation of the device.
        /// </summary>
        public bool IsOrientationMonitoringEnabled
        {
            get { return isOrientationMonitoringEnabled; }
            set
            {
                isOrientationMonitoringEnabled = value;

                var displayInfo = DisplayInformation.GetForCurrentView();
                displayInfo.OrientationChanged -= OnOrientationChanged;

                if (isOrientationMonitoringEnabled)
                {
                    displayInfo.OrientationChanged += OnOrientationChanged;
                }
            }
        }
        private bool isOrientationMonitoringEnabled;

        /// <summary>
        /// Gets or sets a value indicating whether the power state of the device should be monitored.
        /// This affects the ability to read the power source and battery level of the device.
        /// </summary>
        public bool IsPowerMonitoringEnabled
        {
            get { return isPowerMonitoringEnabled; }
            set
            {
                isPowerMonitoringEnabled = value;

                PowerManager.BatteryStatusChanged -= OnPowerSourceChanged;
                PowerManager.RemainingChargePercentChanged -= OnBatteryLevelChanged;

                if (isPowerMonitoringEnabled)
                {
                    PowerManager.BatteryStatusChanged += OnPowerSourceChanged;
                    PowerManager.RemainingChargePercentChanged += OnBatteryLevelChanged;
                }
            }
        }
        private bool isPowerMonitoringEnabled;

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string Name
        {
            get { return global::Windows.Networking.Proximity.PeerFinder.DisplayName; }
        }

        /// <summary>
        /// Gets the operating system that is running on the device.
        /// </summary>
        public OperatingSystem OperatingSystem
        {
            get { return OperatingSystem.Windows; }
        }

        /// <summary>
        /// Gets the physical orientation of the device.
        /// </summary>
        public DeviceOrientation Orientation
        {
            get
            {
                var displayInfo = DisplayInformation.GetForCurrentView();
                switch (displayInfo.CurrentOrientation)
                {
                    case DisplayOrientations.Portrait:
                        return DeviceOrientation.PortraitUp;
                    case DisplayOrientations.Landscape:
                        return DeviceOrientation.LandscapeLeft;
                    case DisplayOrientations.LandscapeFlipped:
                        return DeviceOrientation.LandscapeRight;
                    case DisplayOrientations.PortraitFlipped:
                        return DeviceOrientation.PortraitDown;
                    default:
                        return DeviceOrientation.Unknown;
                }
            }
        }

        /// <summary>
        /// Gets the version of the operating system that is running on the device.
        /// </summary>
        public Version OSVersion { get; }

        /// <summary>
        /// Gets the source from which the device is receiving its power.
        /// </summary>
        public PowerSource PowerSource
        {
            get { return PowerManager.BatteryStatus.GetPowerSource(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device()
        {
            OSVersion = SystemVersion ?? new Version(0, 0);
            SystemVersion = null;
        }

        private void OnBatteryLevelChanged(object sender, object e)
        {
            BatteryLevelChanged(this, EventArgs.Empty);
        }

        private void OnOrientationChanged(DisplayInformation sender, object e)
        {
            OrientationChanged(this, EventArgs.Empty);
        }

        private void OnPowerSourceChanged(object sender, object e)
        {
            PowerSourceChanged(this, EventArgs.Empty);
        }
    }
}
