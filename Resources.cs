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
using Windows.UI.Xaml;

namespace Prism.Windows
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeResources"/>.
    /// </summary>
    [Register(typeof(INativeResources), IsSingleton = true)]
    public class Resources : INativeResources
    {
        static Resources()
        {
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ActionMenuMaxDisplayItemsKey.Id] = 4;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.AutomaticallyIndentSeparatorsKey.Id] = false;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.BaseFontFamilyKey.Id] = new Prism.UI.Media.FontFamily("Segoe UI");
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.BaseFontSizeKey.Id] = 15.0;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ButtonPaddingKey.Id] = new Thickness(9.5, 3.5);
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ControlBorderWidthKey.Id] = 2.5;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ControlFontSizeKey.Id] = 15.0;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.HorizontalScrollBarHeightKey.Id] = 12.0;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ListBoxItemDetailHeightKey.Id] = 64.0;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ListBoxItemIndicatorSizeKey.Id] = new Size();
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ListBoxItemInfoButtonSizeKey.Id] = new Size();
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ListBoxItemInfoIndicatorSizeKey.Id] = new Size();
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.ListBoxItemStandardHeightKey.Id] = 48.0;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.LoadIndicatorFontSizeKey.Id] = 20.26;
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.PopupSizeKey.Id] = new Size(540, 620);
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.SelectListDisplayItemPaddingKey.Id] = new Thickness(4, 2, 32, 4);
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.SelectListListItemPaddingKey.Id] = new Thickness(4, 2, 4, 4);
            global::Windows.UI.Xaml.Application.Current.Resources[SystemResources.VerticalScrollBarWidthKey.Id] = 12.0;
        }

        /// <summary>
        /// Gets the names of all available fonts.
        /// </summary>
        /// <returns>An <see cref="Array"/> containing the names of all available fonts.</returns>
        public string[] GetAvailableFontNames()
        {
            return DWriteFactory.GetFontNames();
        }

        /// <summary>
        /// Gets the system resource associated with the specified key.
        /// </summary>
        /// <param name="owner">The object that owns the resource, or <c>null</c> if the resource is not owned by a specified object.</param>
        /// <param name="key">The key associated with the resource to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the system resources contain a resource with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetResource(object owner, object key, out object value)
        {
            var id = (key as ResourceKey)?.Id ?? key.ToString();

            var element = owner as FrameworkElement;
            if (element?.Resources != null && element.Resources.TryGetValue(id, out value))
            {
                return true;
            }

            return global::Windows.UI.Xaml.Application.Current.Resources.TryGetValue(id, out value);
        }
    }
}
