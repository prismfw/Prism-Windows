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


using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Prism.Windows
{
    /// <summary>
    /// Provides convenience properties for accessing theme-specific brushes.
    /// </summary>
    public static class ThemeResources
    {
        /// <summary>
        /// Gets a brush with the system-defined accent color.
        /// </summary>
        public static Brush AccentColorBrush
        {
            get { return GetBrush("SystemAccentColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific alternate high color.
        /// </summary>
        public static Brush AltHighBrush
        {
            get { return GetBrush("SystemAltHighColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific alternate medium color.
        /// </summary>
        public static Brush AltMediumBrush
        {
            get { return GetBrush("SystemAltMediumColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific generic background color.
        /// </summary>
        public static Brush BackgroundBrush
        {
            get { return GetBrush("SystemColorWindowColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific high color.
        /// </summary>
        public static Brush BaseHighBrush
        {
            get { return GetBrush("SystemBaseHighColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific low color.
        /// </summary>
        public static Brush BaseLowBrush
        {
            get { return GetBrush("SystemBaseLowColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific medium color.
        /// </summary>
        public static Brush BaseMediumBrush
        {
            get { return GetBrush("SystemBaseMediumColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific medium high color.
        /// </summary>
        public static Brush BaseMediumHighBrush
        {
            get { return GetBrush("SystemBaseMediumHighColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific medium low color.
        /// </summary>
        public static Brush BaseMediumLowBrush
        {
            get { return GetBrush("SystemBaseMediumLowColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific color for button text.
        /// </summary>
        public static Brush ButtonForegroundBrush
        {
            get { return GetBrush("SystemColorWindowTextColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific color for high chrome.
        /// </summary>
        public static Brush ChromeHighBrush
        {
            get { return GetBrush("SystemChromeHighColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific color for medium chrome.
        /// </summary>
        public static Brush ChromeMediumBrush
        {
            get { return GetBrush("SystemChromeMediumColor"); }
        }

        /// <summary>
        /// Gets a brush with the theme-specific color for text.
        /// </summary>
        public static Brush TextForegroundBrush
        {
            get { return GetBrush("SystemColorWindowTextColor"); }
        }

        /// <summary>
        /// Gets a brush for the specified color resource.
        /// </summary>
        /// <param name="key">The color resource for which to return a brush.</param>
        public static Brush GetBrush(string key)
        {
            var resource = global::Windows.UI.Xaml.Application.Current.Resources[key];
            if (resource is Color)
            {
                return new SolidColorBrush((Color)resource);
            }

            return resource as Brush;
        }
    }
}
