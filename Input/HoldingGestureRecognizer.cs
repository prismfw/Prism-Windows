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
    /// Represents a Windows implementation for a <see cref="INativeHoldingGestureRecognizer"/>.
    /// </summary>
    [Register(typeof(INativeHoldingGestureRecognizer))]
    public class HoldingGestureRecognizer : GestureRecognizerBase, INativeHoldingGestureRecognizer
    {
        /// <summary>
        /// Occurs when a holding gesture is started, completed, or canceled.
        /// </summary>
        public event EventHandler<HoldingEventArgs> Holding;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoldingGestureRecognizer"/> class.
        /// </summary>
        public HoldingGestureRecognizer()
        {
            Recognizer.GestureSettings = GestureSettings.Hold | GestureSettings.HoldWithMouse;

            Recognizer.Holding += (o, e) =>
            {
                Holding(this, new HoldingEventArgs(e.PointerDeviceType.GetPointerType(),
                    TransformToTarget(e.Position).GetPoint(), e.HoldingState.GetHoldingState()));
            };
        }
    }
}
