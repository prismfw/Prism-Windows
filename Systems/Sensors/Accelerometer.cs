/*
Copyright (C) 2017  Prism Framework Team

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
using Prism.Systems.Sensors;
using Windows.Graphics.Display;

namespace Prism.Windows.Systems.Sensors
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeAccelerometer"/>.
    /// </summary>
    [Register(typeof(INativeAccelerometer), IsSingleton = true)]
    public class Accelerometer : INativeAccelerometer
    {
        /// <summary>
        /// Occurs when the reading of the accelerometer has changed.
        /// </summary>
        public event EventHandler<AccelerometerReadingChangedEventArgs> ReadingChanged;

        /// <summary>
        /// Gets a value indicating whether an accelerometer is available for the current device.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                if (accelerometer == null)
                {
                    Initialize();
                }

                return accelerometer != null;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, that should pass between readings.
        /// </summary>
        public double UpdateInterval
        {
            get { return updateInterval; }
            set
            {
                updateInterval = value;

                if (double.IsNaN(updateInterval) || updateInterval == 0)
                {
                    accelerometer.ReadingChanged -= OnReadingChanged;
                    accelerometer.ReportInterval = 0;
                }
                else
                {
                    accelerometer.ReadingChanged -= OnReadingChanged;
                    accelerometer.ReportInterval = Math.Max((uint)updateInterval, accelerometer.MinimumReportInterval);
                    accelerometer.ReadingChanged += OnReadingChanged;
                }
            }
        }
        private double updateInterval = double.NaN;

        private global::Windows.Devices.Sensors.Accelerometer accelerometer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Accelerometer"/> class.
        /// </summary>
        public Accelerometer()
        {
            Initialize();
        }

        /// <summary>
        /// Gets the current reading of the accelerometer.
        /// </summary>
        /// <returns>The current reading of the accelerometer as an <see cref="AccelerometerReading"/> instance.</returns>
        public AccelerometerReading GetCurrentReading()
        {
            var reading = accelerometer.GetCurrentReading();
            if (reading == null)
            {
                return null;
            }

            return new AccelerometerReading(GetTimestamp(reading.Timestamp),
                reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ);
        }

        private double GetTimestamp(DateTimeOffset dto)
        {
            return Prism.Systems.Device.Current.SystemUptime -
                (DateTimeOffset.UtcNow.UtcDateTime - dto.UtcDateTime).TotalMilliseconds;
        }

        private void Initialize()
        {
            accelerometer = global::Windows.Devices.Sensors.Accelerometer.GetDefault();
            if (accelerometer != null)
            {
                accelerometer.ReadingTransform = DisplayOrientations.Portrait;
            }
        }

        private void OnReadingChanged(global::Windows.Devices.Sensors.Accelerometer o, global::Windows.Devices.Sensors.AccelerometerReadingChangedEventArgs e)
        {
            ReadingChanged(this, new AccelerometerReadingChangedEventArgs(new AccelerometerReading(GetTimestamp(e.Reading.Timestamp),
                e.Reading.AccelerationX, e.Reading.AccelerationY, e.Reading.AccelerationZ)));
        }
    }
}
