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
using System.Threading;
using Prism.Native;

namespace Prism.Windows.Utilities
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeTimer"/>.
    /// </summary>
    [Register(typeof(INativeTimer))]
    public class Timer : INativeTimer, IDisposable
    {
        /// <summary>
        /// Occurs when the number of milliseconds specified by <see cref="Interval"/> have passed.
        /// </summary>
        public event EventHandler Elapsed;

        /// <summary>
        /// Gets or sets a value indicating whether the timer should restart after each interval.
        /// </summary>
        public bool AutoReset { get; set; }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, before the <see cref="Elapsed"/> event is fired.
        /// </summary>
        public double Interval
        {
            get { return interval; }
            set
            {
                if (value != interval)
                {
                    interval = value;
                    if (IsRunning)
                    {
                        timer.Change((int)interval, Timeout.Infinite);
                    }
                }
            }
        }
        private double interval;

        /// <summary>
        /// Gets a value indicating whether the timer is current running.
        /// </summary>
        public bool IsRunning { get; private set; }

        private readonly System.Threading.Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        public Timer()
        {
            timer = new System.Threading.Timer((o) =>
            {
                IsRunning = false;

                Elapsed(this, EventArgs.Empty);

                if (AutoReset)
                {
                    StartTimer();
                }
            }, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            timer.Dispose();
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void StartTimer()
        {
            IsRunning = true;
            timer.Change((int)interval, Timeout.Infinite);
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            IsRunning = false;
        }
    }
}
