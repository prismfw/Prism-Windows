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
using Prism.Input;
using Prism.Native;

using GestureSettings = global::Windows.UI.Input.GestureSettings;

namespace Prism.Windows.Input
{
    /// <summary>
    /// Represents a Windows implementation for a <see cref="INativeTapGestureRecognizer"/>.
    /// </summary>
    [Register(typeof(INativeTapGestureRecognizer))]
    public class TapGestureRecognizer : GestureRecognizerBase, INativeTapGestureRecognizer
    {
        /// <summary>
        /// Occurs when a double tap gesture is recognized.
        /// </summary>
        public event EventHandler<TappedEventArgs> DoubleTapped;

        /// <summary>
        /// Occurs when a right tap gesture (or long press gesture for touch input) is recognized.
        /// </summary>
        public event EventHandler<TappedEventArgs> RightTapped;

        /// <summary>
        /// Occurs when a single tap gesture is recognized.
        /// </summary>
        public event EventHandler<TappedEventArgs> Tapped;

        /// <summary>
        /// Gets or sets a value indicating whether double tap gestures should be recognized.
        /// </summary>
        public bool IsDoubleTapEnabled
        {
            get { return Recognizer.GestureSettings.HasFlag(GestureSettings.DoubleTap); }
            set
            {
                if (value != IsDoubleTapEnabled)
                {
                    Recognizer.GestureSettings ^= GestureSettings.DoubleTap;
                    OnPropertyChanged(Prism.Input.TapGestureRecognizer.IsDoubleTapEnabledProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether right tap gestures (long press gestures for touch input) should be recognized.
        /// </summary>
        public bool IsRightTapEnabled
        {
            get { return Recognizer.GestureSettings.HasFlag(GestureSettings.RightTap); }
            set
            {
                if (value != IsRightTapEnabled)
                {
                    Recognizer.GestureSettings ^= GestureSettings.RightTap;
                    OnPropertyChanged(Prism.Input.TapGestureRecognizer.IsRightTapEnabledProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether single tap gestures should be recognized.
        /// </summary>
        public bool IsTapEnabled
        {
            get { return Recognizer.GestureSettings.HasFlag(GestureSettings.Tap); }
            set
            {
                if (value != IsTapEnabled)
                {
                    Recognizer.GestureSettings ^= GestureSettings.Tap;
                    OnPropertyChanged(Prism.Input.TapGestureRecognizer.IsTapEnabledProperty);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TapGestureRecognizer"/> class.
        /// </summary>
        public TapGestureRecognizer()
        {
            Recognizer.RightTapped += (o, e) =>
            {
                RightTapped(this, new TappedEventArgs(e.PointerDeviceType.GetPointerType(), TransformToTarget(e.Position).GetPoint(), 1));
            };

            Recognizer.Tapped += (o, e) =>
            {
                if (e.TapCount == 1 && IsTapEnabled)
                {
                    Tapped(this, new TappedEventArgs(e.PointerDeviceType.GetPointerType(), TransformToTarget(e.Position).GetPoint(), 1));
                }
                else if (e.TapCount == 2 && IsDoubleTapEnabled)
                {
                    DoubleTapped(this, new TappedEventArgs(e.PointerDeviceType.GetPointerType(), TransformToTarget(e.Position).GetPoint(), 2));
                }
            };
        }
    }
}
