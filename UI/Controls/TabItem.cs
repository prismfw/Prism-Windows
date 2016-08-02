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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeTabItem"/>.
    /// </summary>
    [Register(typeof(INativeTabItem))]
    public class TabItem : StackPanel, INativeTabItem
    {
        /// <summary>
        /// Occurs when the value of a property is changed.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Gets or sets the object that acts as the content of the item.
        /// This is typically an <see cref="IView"/> or <see cref="INativeViewStack"/> instance.
        /// </summary>
        public object Content
        {
            get { return content; }
            set
            {
                content = value;

                var tabView = this.GetParent<INativeTabView>();
                if (tabView != null && tabView.SelectedIndex == tabView.TabItems.IndexOf(this))
                {
                    var splitView = tabView as global::Windows.UI.Xaml.Controls.SplitView;
                    if (splitView != null)
                    {
                        if (tabView is INativeTabbedSplitView)
                        {
                            splitView = splitView.Content as global::Windows.UI.Xaml.Controls.SplitView;
                            if (splitView != null)
                            {
                                splitView.Pane = content as UIElement;
                            }
                        }
                        else
                        {
                            splitView.Content = content as UIElement;
                        }
                    }
                }
            }
        }
        private object content;

        /// <summary>
        /// Gets or sets the font to use for displaying the title text.
        /// </summary>
        public object FontFamily
        {
            get { return TextBlock.FontFamily; }
            set
            {
                var fontFamily = value as Media.FontFamily;
                if (fontFamily != TextBlock.FontFamily)
                {
                    TextBlock.SetFont(fontFamily, FontStyle);
                    OnPropertyChanged(Prism.UI.Controls.Label.FontFamilyProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the title text.
        /// </summary>
        public double FontSize
        {
            get { return TextBlock.FontSize; }
            set
            {
                if (value != TextBlock.FontSize)
                {
                    TextBlock.FontSize = value;
                    OnPropertyChanged(Prism.UI.Controls.Label.FontSizeProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style with which to render the title text.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return TextBlock.GetFontStyle(); }
            set
            {
                var style = TextBlock.FontStyle;
                var weight = TextBlock.FontWeight.Weight;
                TextBlock.SetFont(TextBlock.FontFamily as Media.FontFamily, value);

                if (TextBlock.FontStyle != style || TextBlock.FontWeight.Weight != weight)
                {
                    OnPropertyChanged(Prism.UI.Controls.Label.FontStyleProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the title.
        /// </summary>
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;
                    TextBlock.Foreground = foreground.GetBrush();
                    OnPropertyChanged(Prism.UI.Controls.TabItem.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets or sets an <see cref="INativeImageSource"/> for an image to display with the item.
        /// </summary>
        public INativeImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                if (value != imageSource)
                {
                    imageSource = value;
                    Image.Source = imageSource.GetImageSource();
                    OnPropertyChanged(Prism.UI.Controls.TabItem.ImageSourceProperty);
                }
            }
        }
        private INativeImageSource imageSource;

        /// <summary>
        /// Gets or sets the title for the item.
        /// </summary>
        public string Title
        {
            get { return TextBlock.Text; }
            set
            {
                if (value != TextBlock.Text)
                {
                    TextBlock.Text = value;
                    OnPropertyChanged(Prism.UI.Controls.TabItem.TitleProperty);
                }
            }
        }

        /// <summary>
        /// Gets the element that displays the image of the tab.
        /// </summary>
        protected global::Windows.UI.Xaml.Controls.Image Image { get; }

        /// <summary>
        /// Gets the element that displays the title text of the tab.
        /// </summary>
        protected TextBlock TextBlock { get; }

        internal static global::Windows.UI.Xaml.Media.Brush SelectedBackgroundDefault { get; set; } =
            new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(102, 0, 120, 215));

        /// <summary>
        /// Initializes a new instance of the <see cref="TabItem"/> class.
        /// </summary>
        public TabItem()
        {
            Children.Add(Image = new global::Windows.UI.Xaml.Controls.Image()
            {
                Height = 48,
                Width = 48,
                Stretch = global::Windows.UI.Xaml.Media.Stretch.Uniform,
                VerticalAlignment = VerticalAlignment.Center
            });
            
            Children.Add(TextBlock = new TextBlock
            {
                MaxLines = 1,
                TextTrimming = TextTrimming.CharacterEllipsis,
                VerticalAlignment = VerticalAlignment.Center
            });

            HorizontalAlignment = HorizontalAlignment.Stretch;
            MinHeight = 48;
            Orientation = Orientation.Horizontal;

            Loaded += (o, e) =>
            {
                var presenter = this.GetParent<ListViewItemPresenter>();
                if (presenter != null)
                {
                    presenter.SelectedBackground = presenter.SelectedPointerOverBackground =
                        presenter.GetParent<INativeTabView>()?.Foreground.GetBrush() ?? SelectedBackgroundDefault;
                }
            };
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

