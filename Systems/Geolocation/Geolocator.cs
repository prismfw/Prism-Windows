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
using System.Threading.Tasks;
using Prism.Native;
using Prism.Systems.Geolocation;
using Windows.Devices.Geolocation;

namespace Prism.Windows.Systems.Geolocation
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeGeolocator"/>.
    /// </summary>
    [Register(typeof(INativeGeolocator), IsSingleton = true)]
    public class Geolocator : INativeGeolocator
    {
        /// <summary>
        /// Occurs when the location is updated.
        /// </summary>
        public event EventHandler<GeolocationUpdatedEventArgs> LocationUpdated;

        /// <summary>
        /// Gets or sets the desired level of accuracy when reading geographic coordinates.
        /// </summary>
        public GeolocationAccuracy DesiredAccuracy
        {
            get { return locator.DesiredAccuracy == PositionAccuracy.High ? GeolocationAccuracy.Precise : GeolocationAccuracy.Approximate; }
            set { locator.DesiredAccuracy = value == GeolocationAccuracy.Precise ? PositionAccuracy.High : PositionAccuracy.Default; }
        }

        /// <summary>
        /// Gets or sets the minimum distance, in meters, that should be covered before the location is updated again.
        /// </summary>
        public double DistanceThreshold
        {
            get { return locator.MovementThreshold; }
            set { locator.MovementThreshold = value; }
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, that should pass before the location is updated again.
        /// </summary>
        public double UpdateInterval
        {
            get { return updateInterval; }
            set
            {
                updateInterval = value;
                locator.ReportInterval = (uint)updateInterval;
            }
        }
        private double updateInterval;

        private readonly global::Windows.Devices.Geolocation.Geolocator locator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Geolocator"/> class.
        /// </summary>
        public Geolocator()
        {
            locator = new global::Windows.Devices.Geolocation.Geolocator();
            updateInterval = locator.ReportInterval;
        }

        /// <summary>
        /// Signals the geolocation service to begin listening for location updates.
        /// </summary>
        public void BeginLocationUpdates()
        {
            locator.PositionChanged -= OnPositionChanged;
            locator.PositionChanged += OnPositionChanged;
        }

        /// <summary>
        /// Signals the geolocation service to stop listening for location updates.
        /// </summary>
        public void EndLocationUpdates()
        {
            locator.PositionChanged -= OnPositionChanged;
        }

        /// <summary>
        /// Makes a singular request to the geolocation service for the current location.
        /// </summary>
        /// <returns>A <see cref="Coordinate"/> representing the current location.</returns>
        public async Task<Coordinate> GetCoordinateAsync()
        {
            var coordinate = (await locator.GetGeopositionAsync()).Coordinate;
            return new Coordinate(coordinate.Timestamp, coordinate.Point.Position.Latitude, coordinate.Point.Position.Longitude,
                coordinate.Point.Position.Altitude, coordinate.Heading, coordinate.Speed, coordinate.Accuracy, coordinate.AltitudeAccuracy);
        }

        /// <summary>
        /// Requests access to the device's geolocation service.
        /// </summary>
        /// <returns><c>true</c> if access is granted; otherwise, <c>false</c>.</returns>
        public async Task<bool> RequestAccessAsync()
        {
            return (await global::Windows.Devices.Geolocation.Geolocator.RequestAccessAsync()) == GeolocationAccessStatus.Allowed;
        }

        private void OnPositionChanged(global::Windows.Devices.Geolocation.Geolocator sender, PositionChangedEventArgs e)
        {
            var coordinate = e.Position.Coordinate;
            LocationUpdated(this, new GeolocationUpdatedEventArgs(new Coordinate(coordinate.Timestamp,
                coordinate.Point.Position.Latitude, coordinate.Point.Position.Longitude, coordinate.Point.Position.Altitude,
                coordinate.Heading, coordinate.Speed, coordinate.Accuracy, coordinate.AltitudeAccuracy)));
        }
    }
}
