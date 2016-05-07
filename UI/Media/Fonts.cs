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
using Prism.UI.Media;

namespace Prism.Windows.UI.Media
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeFonts"/>.
    /// </summary>
    [Register(typeof(INativeFonts), IsSingleton = true)]
    public class Fonts : INativeFonts
    {
        /// <summary>
        /// Gets the preferred font size for a button.
        /// </summary>
        public double ButtonFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a button.
        /// </summary>
        public FontStyle ButtonFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a date picker.
        /// </summary>
        public double DatePickerFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a date picker.
        /// </summary>
        public FontStyle DatePickerFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the default font family for UI elements that do not have a font family preference.
        /// </summary>
        public Prism.UI.Media.FontFamily DefaultFontFamily { get; } = new Prism.UI.Media.FontFamily("Segoe UI");

        /// <summary>
        /// Gets the preferred font size for the detail label of a list box item.
        /// </summary>
        public double DetailLabelFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for the detail label of a list box item.
        /// </summary>
        public FontStyle DetailLabelFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a section footer in a list box that uses a grouped style.
        /// </summary>
        public double GroupedSectionFooterFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a section footer in a list box that uses a grouped style.
        /// </summary>
        public FontStyle GroupedSectionFooterFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a section header in a list box that uses a grouped style.
        /// </summary>
        public double GroupedSectionHeaderFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a section header in a list box that uses a grouped style.
        /// </summary>
        public FontStyle GroupedSectionHeaderFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for the header of a view.
        /// </summary>
        public double HeaderFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for the header of a view.
        /// </summary>
        public FontStyle HeaderFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for the title text of a load indicator.
        /// </summary>
        public double LoadIndicatorFontSize => 20.26;

        /// <summary>
        /// Gets the preferred font style for the title text of a load indicator.
        /// </summary>
        public FontStyle LoadIndicatorFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a search box.
        /// </summary>
        public double SearchBoxFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a search box.
        /// </summary>
        public FontStyle SearchBoxFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a section footer in a list box that uses the default style.
        /// </summary>
        public double SectionFooterFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a section footer in a list box that uses the default style.
        /// </summary>
        public FontStyle SectionFooterFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a section header in a list box that uses the default style.
        /// </summary>
        public double SectionHeaderFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a section header in a list box that uses the default style.
        /// </summary>
        public FontStyle SectionHeaderFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for the display item of a select list.
        /// </summary>
        public double SelectListFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for the display item of a select list.
        /// </summary>
        public FontStyle SelectListFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a standard text label.
        /// </summary>
        public double StandardLabelFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a standard text label.
        /// </summary>
        public FontStyle StandardLabelFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a tab item.
        /// </summary>
        public double TabItemFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a tab item.
        /// </summary>
        public FontStyle TabItemFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a text area.
        /// </summary>
        public double TextAreaFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a text area.
        /// </summary>
        public FontStyle TextAreaFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a text box.
        /// </summary>
        public double TextBoxFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a text box.
        /// </summary>
        public FontStyle TextBoxFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for a time picker.
        /// </summary>
        public double TimePickerFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for a time picker.
        /// </summary>
        public FontStyle TimePickerFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the preferred font size for the value label of a list box item.
        /// </summary>
        public double ValueLabelFontSize => 15;

        /// <summary>
        /// Gets the preferred font style for the value label of a list box item.
        /// </summary>
        public FontStyle ValueLabelFontStyle => FontStyle.Normal;

        /// <summary>
        /// Gets the names of all available fonts.
        /// </summary>
        /// <returns>An <see cref="Array"/> containing the names of all available fonts.</returns>
        public string[] GetAvailableFontNames()
        {
            return DWriteFactory.GetFontNames();
        }
    }
}
