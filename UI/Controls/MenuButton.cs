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
    /// Represents a Windows implementation for an <see cref="INativeMenuButton"/>.
    /// </summary>
    [Register(typeof(INativeMenuButton))]
    public class MenuButton : AppBarButton, INativeMenuButton
    {
        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the foreground content of the menu item.
        /// </summary>
        public new Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;
                    base.Foreground = foreground.GetBrush() ?? this.GetParent<AppBar>()?.Foreground ?? ThemeResources.ButtonForegroundBrush;
                    OnPropertyChanged(Prism.UI.Controls.MenuItem.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> of the image to display within the button.
        /// </summary>
        public Uri ImageUri
        {
            get { return (Icon as BitmapIcon)?.UriSource; }
            set
            {
                if (value != ImageUri)
                {
                    Icon = new BitmapIcon() { UriSource = value };
                    OnPropertyChanged(Prism.UI.Controls.MenuButton.ImageUriProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the title of the button.
        /// </summary>
        public string Title
        {
            get { return Label; }
            set
            {
                value = value ?? string.Empty;
                if (value != Label)
                {
                    Label = value;
                    OnPropertyChanged(Prism.UI.Controls.MenuButton.TitleProperty);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuButton"/> class.
        /// </summary>
        public MenuButton()
        {
            Click += (o, e) => Clicked(this, EventArgs.Empty);
            IsEnabledChanged += (o, e) => OnPropertyChanged(Prism.UI.Controls.MenuButton.IsEnabledProperty);
        }

        /// <summary>
        /// Called when a property value is changed.
        /// </summary>
        /// <param name="pd">A property descriptor describing the property whose value has been changed.</param>
        protected virtual void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }
    }
}
