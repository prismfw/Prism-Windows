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
using Prism.UI.Media;
using Windows.UI;
using Windows.UI.Xaml;

using Brush = global::Windows.UI.Xaml.Media.Brush;

namespace Prism.Windows
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeResources"/>.
    /// </summary>
    [Register(typeof(INativeResources), IsSingleton = true)]
    public class Resources : INativeResources
    {
        internal const string BackgroundAltHighBrushId = "SystemControlBackgroundAltHighBrush";
        internal const string BackgroundAltMediumLowBrushId = "SystemControlBackgroundAltMediumLowBrush";
        internal const string BackgroundBaseLowBrushId = "SystemControlBackgroundBaseLowBrush";
        internal const string BackgroundChromeMediumLowBrushId = "SystemControlBackgroundChromeMediumLowBrush";
        internal const string ForegroundAccentBrushId = "SystemControlForegroundAccentBrush";
        internal const string ForegroundBaseHighBrushId = "SystemControlForegroundBaseHighBrush";
        internal const string ForegroundBaseLowBrushId = "SystemControlForegroundBaseLowBrush";
        internal const string ForegroundBaseMediumHighBrushId = "SystemControlForegroundBaseMediumHighBrush";
        internal const string ForegroundBaseMediumLowBrushId = "SystemControlForegroundBaseMediumLowBrush";
        internal const string ForegroundChromeDisabledLowBrushId = "SystemControlForegroundChromeDisabledLowBrush";
        internal const string ForegroundTransparentBrushId = "SystemControlForegroundTransparentBrush";
        internal const string HighlightAccentBrushId = "SystemControlHighlightAccentBrush";
        internal const string HighlightAltChromeWhiteBrushId = "SystemControlHighlightAltChromeWhiteBrush";
        internal const string HighlightListAccentLowBrushId = "SystemControlHighlightListAccentLowBrush";
        internal const string PageBackgroundChromeLowBrushId = "SystemControlPageBackgroundChromeLowBrush";
        internal const string PageTextBaseHighBrushId = "SystemControlPageTextBaseHighBrush";

        /// <summary>
        /// Gets the brush associated with the specified key.
        /// </summary>
        /// <param name="owner">The object that owns the resource, or <c>null</c> if the resource is not owned by a specified object.</param>
        /// <param name="key">The key associated with the brush resource to get.</param>
        public static Brush GetBrush(object owner, object key)
        {
            object retval;
            TryGetResource(owner, key, out retval, false);
            return retval is Color ? new global::Windows.UI.Xaml.Media.SolidColorBrush((Color)retval) : retval as Brush;
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
            return TryGetResource(owner, key, out value, true);
        }

        private static bool TryGetResource(object owner, object key, out object value, bool convert)
        {
            string id = null;

            var resourceKey = key as ResourceKey;
            if (resourceKey != null)
            {
                switch ((SystemResourceKeyId)resourceKey.Id)
                {
                    case SystemResourceKeyId.ActionMenuMaxDisplayItems:
                        value = 2;
                        return true;
                    case SystemResourceKeyId.ButtonBorderWidth:
                    case SystemResourceKeyId.DateTimePickerBorderWidth:
                    case SystemResourceKeyId.SearchBoxBorderWidth:
                    case SystemResourceKeyId.SelectListBorderWidth:
                    case SystemResourceKeyId.TextBoxBorderWidth:
                        value = 2.5;
                        return true;
                    case SystemResourceKeyId.ButtonPadding:
                        value = new Thickness(9.5, 3.5);
                        return true;
                    case SystemResourceKeyId.ListBoxItemDetailHeight:
                        value = 64.0;
                        return true;
                    case SystemResourceKeyId.ListBoxItemStandardHeight:
                        value = 48.0;
                        return true;
                    case SystemResourceKeyId.ListBoxItemIndicatorSize:
                    case SystemResourceKeyId.ListBoxItemInfoButtonSize:
                    case SystemResourceKeyId.ListBoxItemInfoIndicatorSize:
                        value = new Size();
                        return true;
                    case SystemResourceKeyId.PopupSize:
                        value = new Size(540, 620);
                        return true;
                    case SystemResourceKeyId.SelectListDisplayItemPadding:
                        value = new Thickness(4, 2, 32, 4);
                        return true;
                    case SystemResourceKeyId.SelectListListItemPadding:
                        value = new Thickness(4, 2, 4, 4);
                        return true;
                    case SystemResourceKeyId.ShouldAutomaticallyIndentSeparators:
                        value = false;
                        return true;
                    case SystemResourceKeyId.HorizontalScrollBarHeight:
                    case SystemResourceKeyId.VerticalScrollBarWidth:
                        value = 12.0;
                        return true;
                    case SystemResourceKeyId.BaseFontFamily:
                        value = new FontFamily("Segoe UI");
                        return true;
                    case SystemResourceKeyId.ButtonFontSize:
                    case SystemResourceKeyId.DateTimePickerFontSize:
                    case SystemResourceKeyId.DetailLabelFontSize:
                    case SystemResourceKeyId.GroupedSectionHeaderFontSize:
                    case SystemResourceKeyId.LabelFontSize:
                    case SystemResourceKeyId.SearchBoxFontSize:
                    case SystemResourceKeyId.SectionHeaderFontSize:
                    case SystemResourceKeyId.SelectListFontSize:
                    case SystemResourceKeyId.TabItemFontSize:
                    case SystemResourceKeyId.TextBoxFontSize:
                    case SystemResourceKeyId.ValueLabelFontSize:
                    case SystemResourceKeyId.ViewHeaderFontSize:
                        value = 15.0;
                        return true;
                    case SystemResourceKeyId.LoadIndicatorFontSize:
                        value = 20.26;
                        return true;
                    case SystemResourceKeyId.ButtonFontStyle:
                    case SystemResourceKeyId.DateTimePickerFontStyle:
                    case SystemResourceKeyId.DetailLabelFontStyle:
                    case SystemResourceKeyId.GroupedSectionHeaderFontStyle:
                    case SystemResourceKeyId.LabelFontStyle:
                    case SystemResourceKeyId.LoadIndicatorFontStyle:
                    case SystemResourceKeyId.SearchBoxFontStyle:
                    case SystemResourceKeyId.SectionHeaderFontStyle:
                    case SystemResourceKeyId.SelectListFontStyle:
                    case SystemResourceKeyId.TabItemFontStyle:
                    case SystemResourceKeyId.TextBoxFontStyle:
                    case SystemResourceKeyId.ValueLabelFontStyle:
                    case SystemResourceKeyId.ViewHeaderFontStyle:
                        value = FontStyle.Normal;
                        return true;
                    case SystemResourceKeyId.AccentBrush:
                    case SystemResourceKeyId.ActivityIndicatorForegroundBrush:
                    case SystemResourceKeyId.ProgressBarForegroundBrush:
                    case SystemResourceKeyId.SliderForegroundBrush:
                    case SystemResourceKeyId.TabViewForegroundBrush:
                    case SystemResourceKeyId.ToggleSwitchForegroundBrush:
                        id = HighlightAccentBrushId;
                        break;
                    case SystemResourceKeyId.ActionMenuBackgroundBrush:
                    case SystemResourceKeyId.SelectListListBackgroundBrush:
                        id = BackgroundChromeMediumLowBrushId;
                        break;
                    case SystemResourceKeyId.ActionMenuForegroundBrush:
                    case SystemResourceKeyId.ButtonForegroundBrush:
                    case SystemResourceKeyId.DateTimePickerForegroundBrush:
                    case SystemResourceKeyId.DetailLabelForegroundBrush:
                    case SystemResourceKeyId.SearchBoxForegroundBrush:
                    case SystemResourceKeyId.SelectListForegroundBrush:
                    case SystemResourceKeyId.TextBoxForegroundBrush:
                    case SystemResourceKeyId.ToggleSwitchBorderBrush:
                    case SystemResourceKeyId.ValueLabelForegroundBrush:
                        id = ForegroundBaseHighBrushId;
                        break;
                    case SystemResourceKeyId.ButtonBackgroundBrush:
                    case SystemResourceKeyId.DateTimePickerBackgroundBrush:
                    case SystemResourceKeyId.ProgressBarBackgroundBrush:
                        id = BackgroundBaseLowBrushId;
                        break;
                    case SystemResourceKeyId.ButtonBorderBrush:
                    case SystemResourceKeyId.DateTimePickerBorderBrush:
                        id = ForegroundTransparentBrushId;
                        break;
                    case SystemResourceKeyId.GroupedListBoxItemBackgroundBrush:
                    case SystemResourceKeyId.GroupedSectionHeaderBackgroundBrush:
                    case SystemResourceKeyId.ListBoxBackgroundBrush:
                    case SystemResourceKeyId.ListBoxItemBackgroundBrush:
                    case SystemResourceKeyId.SectionHeaderBackgroundBrush:
                        value = new SolidColorBrush(new Prism.UI.Color(0, 255, 255, 255));
                        return true;
                    case SystemResourceKeyId.GroupedSectionHeaderForegroundBrush:
                    case SystemResourceKeyId.LabelForegroundBrush:
                    case SystemResourceKeyId.LoadIndicatorForegroundBrush:
                    case SystemResourceKeyId.SectionHeaderForegroundBrush:
                    case SystemResourceKeyId.ViewHeaderForegroundBrush:
                        id = PageTextBaseHighBrushId;
                        break;
                    case SystemResourceKeyId.ListBoxItemSelectedBackgroundBrush:
                        id = HighlightListAccentLowBrushId;
                        break;
                    case SystemResourceKeyId.ListBoxSeparatorBrush:
                        id = ForegroundBaseLowBrushId;
                        break;
                    case SystemResourceKeyId.SearchBoxBackgroundBrush:
                    case SystemResourceKeyId.TextBoxBackgroundBrush:
                    case SystemResourceKeyId.ToggleSwitchBackgroundBrush:
                        id = BackgroundAltHighBrushId;
                        break;
                    case SystemResourceKeyId.SearchBoxBorderBrush:
                    case SystemResourceKeyId.TextBoxBorderBrush:
                        id = ForegroundChromeDisabledLowBrushId;
                        break;
                    case SystemResourceKeyId.SelectListBackgroundBrush:
                        id = BackgroundAltMediumLowBrushId;
                        break;
                    case SystemResourceKeyId.SelectListBorderBrush:
                    case SystemResourceKeyId.SliderBackgroundBrush:
                    case SystemResourceKeyId.TabItemForegroundBrush:
                        id = ForegroundBaseMediumLowBrushId;
                        break;
                    case SystemResourceKeyId.SliderThumbBrush:
                        id = ForegroundAccentBrushId;
                        break;
                    case SystemResourceKeyId.ToggleSwitchThumbOffBrush:
                        id = ForegroundBaseMediumHighBrushId;
                        break;
                    case SystemResourceKeyId.ToggleSwitchThumbOnBrush:
                        id = HighlightAltChromeWhiteBrushId;
                        break;
                    case SystemResourceKeyId.LoadIndicatorBackgroundBrush:
                    case SystemResourceKeyId.TabViewBackgroundBrush:
                    case SystemResourceKeyId.ViewBackgroundBrush:
                    case SystemResourceKeyId.ViewHeaderBackgroundBrush:
                        id = PageBackgroundChromeLowBrushId;
                        break;
                    case SystemResourceKeyId.SelectListListSeparatorBrush:
                        value = null;
                        return true;
                }
            }

            if (id == null)
            {
                id = key.ToString();
            }

            var element = owner as FrameworkElement;
            if (!(element?.Resources != null && element.Resources.TryGetValue(id, out value)) &&
                !global::Windows.UI.Xaml.Application.Current.Resources.TryGetValue(id, out value))
            {
                return false;
            }

            if (convert)
            {
                if (value is Color)
                {
                    value = ((Color)value).GetColor();
                }
                else if (value is Brush)
                {
                    // We would like to convert SolidColorBrushes, but we can't do that without losing the
                    // internal bindings those brushes have that are triggered upon changes to system settings.
                    value = new DataBrush(value);
                }
            }

            return true;
        }
    }
}
