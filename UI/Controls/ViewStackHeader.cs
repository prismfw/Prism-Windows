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
using Windows.UI.Xaml.Controls;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeViewStackHeader"/>.
    /// </summary>
    public sealed class ViewStackHeader : Grid, INativeViewStackHeader
    {
        /// <summary>
        /// Occurs when the value of a property is changed.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Gets or sets the background for the header.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    base.Background = background.GetBrush();
                    OnPropertyChanged(Prism.UI.Controls.ViewStackHeader.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the font to use for displaying the title text.
        /// </summary>
        public object FontFamily
        {
            get { return fontFamily; }
            set
            {
                if (value != fontFamily)
                {
                    fontFamily = value;
                    headerLabel.SetFont(value as Media.FontFamily, fontStyle);
                    OnPropertyChanged(Prism.UI.Controls.ViewStackHeader.FontFamilyProperty);
                }
            }
        }
        private object fontFamily;

        /// <summary>
        /// Gets or sets the size of the title text.
        /// </summary>
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                if (value != fontSize)
                {
                    fontSize = value;
                    headerLabel.FontSize = value;
                    OnPropertyChanged(Prism.UI.Controls.ViewStackHeader.FontSizeProperty);
                }
            }
        }
        private double fontSize;

        /// <summary>
        /// Gets or sets the style with which to render the title text.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                if (value != fontStyle)
                {
                    fontStyle = value;
                    headerLabel.SetFont(fontFamily as Media.FontFamily, value);
                    OnPropertyChanged(Prism.UI.Controls.ViewStackHeader.FontStyleProperty);
                }
            }
        }
        private FontStyle fontStyle;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the foreground content of the header.
        /// </summary>
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;
                    headerLabel.Foreground = value.GetBrush() ?? ThemeResources.TextForegroundBrush;
                    OnPropertyChanged(Prism.UI.Controls.ViewStackHeader.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets or sets the title for the header.
        /// </summary>
        public string Title
        {
            get { return headerLabel.Text; }
            set
            {
                value = value ?? string.Empty;
                if (value != headerLabel.Text)
                {
                    headerLabel.Text = value;
                    OnPropertyChanged(Prism.UI.Controls.ViewStackHeader.TitleProperty);
                }
            }
        }

        private readonly TextBlock headerLabel;

        internal ViewStackHeader()
        {
            headerLabel = new TextBlock()
            {
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center
            };
            Children.Add(headerLabel);

            BorderBrush = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(64, 120, 120, 120));
            BorderThickness = new global::Windows.UI.Xaml.Thickness(0, 0, 0, 1);
            Height = 64;
            HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
        }

        private void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }
    }
}
