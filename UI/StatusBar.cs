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
using Prism.UI;
using Windows.Foundation.Metadata;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeStatusBar"/>.
    /// </summary>
    [Register(typeof(INativeStatusBar))]
    public class StatusBar : INativeStatusBar
    {
        /// <summary>
        /// Gets or sets the background color for the status bar.
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                if (!ApiInformation.IsTypePresent(typeof(global::Windows.UI.ViewManagement.StatusBar).FullName))
                {
                    return new Color();
                }

                var statusBar = global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                return new Color((byte)(statusBar.BackgroundOpacity * 255), statusBar.BackgroundColor?.R ?? 0,
                    statusBar.BackgroundColor?.G ?? 0, statusBar.BackgroundColor?.B ?? 0);
            }
            set
            {
                if (ApiInformation.IsTypePresent(typeof(global::Windows.UI.ViewManagement.StatusBar).FullName))
                {
                    var statusBar = global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    statusBar.BackgroundColor = value.GetColor();
                    statusBar.BackgroundOpacity = value.A / 255d;
                }
            }
        }

        /// <summary>
        /// Gets a rectangle describing the area that the status bar is consuming.
        /// </summary>
        public Rectangle Frame
        {
            get
            {
                if (!ApiInformation.IsTypePresent(typeof(global::Windows.UI.ViewManagement.StatusBar).FullName))
                {
                    return new Rectangle();
                }

                return global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.GetRectangle();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the status bar is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                if (!ApiInformation.IsTypePresent(typeof(global::Windows.UI.ViewManagement.StatusBar).FullName))
                {
                    return false;
                }

                var rect = global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect;
                return rect.Width > 0 || rect.Height > 0;
            }
        }

        /// <summary>
        /// Gets or sets the style of the status bar.
        /// </summary>
        public StatusBarStyle Style
        {
            get { return style; }
            set
            {
                style = value;

                if (ApiInformation.IsTypePresent(typeof(global::Windows.UI.ViewManagement.StatusBar).FullName))
                {
                    switch (style)
                    {
                        case StatusBarStyle.Dark:
                            global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = global::Windows.UI.Colors.White;
                            break;
                        case StatusBarStyle.Light:
                            global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = global::Windows.UI.Colors.Black;
                            break;
                        default:
                            global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = null;
                            break;
                    }
                }
            }
        }
        private StatusBarStyle style;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        public StatusBar()
        {
        }

        /// <summary>
        /// Hides the status bar from view.
        /// </summary>
        public async Task HideAsync()
        {
            if (ApiInformation.IsTypePresent(typeof(global::Windows.UI.ViewManagement.StatusBar).FullName))
            {
                await global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }
        }

        /// <summary>
        /// Shows the status bar if it is not visible.
        /// </summary>
        public async Task ShowAsync()
        {
            if (ApiInformation.IsTypePresent(typeof(global::Windows.UI.ViewManagement.StatusBar).FullName))
            {
                await global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            }
        }
    }
}
