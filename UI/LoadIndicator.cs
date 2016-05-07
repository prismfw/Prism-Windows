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
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeLoadIndicator"/>.
    /// </summary>
    [Register(typeof(INativeLoadIndicator))]
    public class LoadIndicator : Grid, INativeLoadIndicator
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Gets or sets the background of the indicator.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    panel.Background = value.GetBrush() ?? new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(60, 0, 0, 0));
                    OnPropertyChanged(Prism.UI.LoadIndicator.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the font to use for displaying the title text.
        /// </summary>
        public object FontFamily
        {
            get { return textElement.FontFamily; }
            set
            {
                var fontFamily = value as Media.FontFamily;
                if (fontFamily != textElement.FontFamily)
                {
                    textElement.SetFont(fontFamily, FontStyle);
                    OnPropertyChanged(Prism.UI.LoadIndicator.FontFamilyProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the title text.
        /// </summary>
        public double FontSize
        {
            get { return textElement.FontSize; }
            set
            {
                if (value != textElement.FontSize)
                {
                    textElement.FontSize = value;
                    OnPropertyChanged(Prism.UI.LoadIndicator.FontSizeProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style with which to render the title text.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return textElement.GetFontStyle(); }
            set
            {
                var style = textElement.FontStyle;
                var weight = textElement.FontWeight.Weight;
                textElement.SetFont(textElement.FontFamily as Media.FontFamily, value);

                if (textElement.FontStyle != style || textElement.FontWeight.Weight != weight)
                {
                    OnPropertyChanged(Prism.UI.LoadIndicator.FontStyleProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the title text.
        /// </summary>
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;

                    var brush = value.GetBrush();
                    textElement.Foreground = brush ?? ThemeResources.TextForegroundBrush;
                    progressRing.Foreground = brush ?? ThemeResources.AccentColorBrush;
                    OnPropertyChanged(Prism.UI.LoadIndicator.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets a value indicating whether this instance is currently visible.
        /// </summary>
        public bool IsVisible
        {
            get { return popup.IsOpen; }
        }

        /// <summary>
        /// Gets or sets the title text of the indicator.
        /// </summary>
        public string Title
        {
            get { return textElement.Text; }
            set
            {
                if (value != textElement.Text)
                {
                    textElement.Text = value ?? string.Empty;
                    OnPropertyChanged(Prism.UI.LoadIndicator.TitleProperty);
                }
            }
        }

        /// <summary>
        /// Gets the UI element that is displaying the text.
        /// </summary>
        protected TextBlock TextElement
        {
            get { return textElement; }
        }
        private readonly TextBlock textElement;

        /// <summary>
        /// Gets the UI element that is displaying the progress indicator.
        /// </summary>
        protected ProgressRing ProgressElement
        {
            get { return progressRing; }
        }
        private readonly ProgressRing progressRing;

        private readonly Popup popup;
        private readonly StackPanel panel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadIndicator"/> class.
        /// </summary>
        public LoadIndicator()
        {
            textElement = new TextBlock()
            {
                FontSize = Fonts.LoadIndicatorFontSize,
                Foreground = ThemeResources.TextForegroundBrush,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center
            };

            progressRing = new ProgressRing()
            {
                Background = null,
                Foreground = ThemeResources.AccentColorBrush,
                Height = 36,
                Margin = new global::Windows.UI.Xaml.Thickness(0, 0, 12, 0),
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center,
                Width = 36
            };

            panel = new StackPanel()
            {
                Background = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(60, 0, 0, 0)),
                Children = { progressRing, textElement },
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center,
                Orientation = global::Windows.UI.Xaml.Controls.Orientation.Horizontal,
                Padding = new global::Windows.UI.Xaml.Thickness(12),
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center
            };
            Children.Add(panel);

            popup = new Popup()
            {
                Child = this,
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center
            };
        }

        /// <summary>
        /// Removes the indicator from view.
        /// </summary>
        public void Hide()
        {
            popup.IsOpen = false;
            progressRing.IsActive = false;

            popup.Opened += (o, e) => OnPropertyChanged(Prism.UI.LoadIndicator.IsVisibleProperty);
            popup.Closed += (o, e) => OnPropertyChanged(Prism.UI.LoadIndicator.IsVisibleProperty);

            CoreApplication.MainView.CoreWindow.SizeChanged -= OnWindowSizeChanged;
        }

        /// <summary>
        /// Displays the indicator.
        /// </summary>
        public void Show()
        {
            Width = CoreApplication.MainView.CoreWindow.Bounds.Width;
            Height = CoreApplication.MainView.CoreWindow.Bounds.Height;

            CoreApplication.MainView.CoreWindow.SizeChanged -= OnWindowSizeChanged;
            CoreApplication.MainView.CoreWindow.SizeChanged += OnWindowSizeChanged;

            progressRing.IsActive = true;
            popup.IsOpen = true;
        }

        /// <summary>
        /// Called when a property value is changed.
        /// </summary>
        /// <param name="pd">A property descriptor describing the property whose value has been changed.</param>
        protected virtual void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }

        private void OnWindowSizeChanged(CoreWindow window, WindowSizeChangedEventArgs e)
        {
            Width = e.Size.Width;
            Height = e.Size.Height;
        }
    }
}
