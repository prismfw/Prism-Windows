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
    public class MenuButton : global::Windows.UI.Xaml.Controls.Button, INativeMenuButton
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Gets or sets the action to perform when the button is pressed by the user.
        /// </summary>
        public Action Action { get; set; }

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
                    base.Foreground = foreground.GetBrush() ?? this.GetParent<ActionMenu>()?.Foreground.GetBrush() ??
                        Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId);

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
            get { return icon?.UriSource; }
            set
            {
                if (value != ImageUri)
                {
                    if (value == null)
                    {
                        Content = title;
                    }
                    else
                    {
                        if (icon == null)
                        {
                            icon = new BitmapIcon();
                        }

                        icon.UriSource = value;
                        Content = icon;
                    }

                    OnPropertyChanged(Prism.UI.Controls.MenuButton.ImageUriProperty);
                }
            }
        }
        private BitmapIcon icon;

        /// <summary>
        /// Gets or sets the title of the button.
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    if (ImageUri == null)
                    {
                        Content = title;
                    }

                    OnPropertyChanged(Prism.UI.Controls.MenuButton.TitleProperty);
                }
            }
        }
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuButton"/> class.
        /// </summary>
        public MenuButton()
        {
            Background = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Colors.Transparent);
            base.Foreground = Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId);
            Padding = new global::Windows.UI.Xaml.Thickness(8);
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            Click += (o, e) => Action();
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
