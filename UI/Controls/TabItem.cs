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
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeTabItem"/>.
    /// </summary>
    [Register(typeof(INativeTabItem))]
    public class TabItem : StackPanel, INativeTabItem
    {
        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when the value of a property is changed.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Occurs when this instance has been detached from the visual tree.
        /// </summary>
        public new event EventHandler Unloaded;

        /// <summary>
        /// Gets or sets a value indicating whether animations are enabled for this instance.
        /// </summary>
        public bool AreAnimationsEnabled
        {
            get { return areAnimationsEnabled; }
            set
            {
                if (value != areAnimationsEnabled)
                {
                    areAnimationsEnabled = value;
                    if (areAnimationsEnabled)
                    {
                        Transitions = transitions;
                        ChildrenTransitions = childrenTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        childrenTransitions = ChildrenTransitions;

                        Transitions = null;
                        ChildrenTransitions = null;
                    }

                    OnPropertyChanged(Prism.UI.Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, childrenTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

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
        /// Gets or sets a <see cref="Rectangle"/> that represents the size and position of the object relative to its parent container.
        /// </summary>
        public Rectangle Frame
        {
            get { return frame; }
            set
            {
                frame = value;

                Width = value.Width;
                Height = value.Height;
            }
        }
        private Rectangle frame = new Rectangle();

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
        /// Gets or sets a value indicating whether this instance can be considered a valid result for hit testing.
        /// </summary>
        public new bool IsHitTestVisible
        {
            get { return base.IsHitTestVisible; }
            set
            {
                if (value != base.IsHitTestVisible)
                {
                    base.IsHitTestVisible = value;

                    var presenter = this.GetParent<ListViewItemPresenter>();
                    if (presenter != null)
                    {
                        presenter.IsHitTestVisible = base.IsHitTestVisible;
                    }

                    OnPropertyChanged(Prism.UI.Visual.IsHitTestVisibleProperty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has been loaded and is ready for rendering.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a measurement of itself and its children.
        /// </summary>
        public MeasureRequestHandler MeasureRequest { get; set; }

        /// <summary>
        /// Gets or sets transformation information that affects the rendering position of this instance.
        /// </summary>
        public new INativeTransform RenderTransform
        {
            get { return renderTransform; }
            set
            {
                if (value != renderTransform)
                {
                    renderTransform = value;
                    base.RenderTransform = renderTransform as Media.Transform ?? renderTransform as global::Windows.UI.Xaml.Media.Transform;
                    OnPropertyChanged(Prism.UI.Visual.RenderTransformProperty);
                }
            }
        }
        private INativeTransform renderTransform;

        /// <summary>
        /// Gets or sets the visual theme that should be used by this instance.
        /// </summary>
        public new Prism.UI.Theme RequestedTheme
        {
            get { return base.RequestedTheme.GetTheme(); }
            set { base.RequestedTheme = value.GetElementTheme(); }
        }

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
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            base.Loaded += (o, e) =>
            {
                var presenter = this.GetParent<ListViewItemPresenter>();
                if (presenter != null)
                {
                    presenter.IsHitTestVisible = base.IsHitTestVisible;
                    presenter.SelectedBackground = presenter.SelectedPointerOverBackground =
                        presenter.GetParent<INativeTabView>()?.Foreground.GetBrush() ?? SelectedBackgroundDefault;
                }

                if (!IsLoaded)
                {
                    IsLoaded = true;
                    OnPropertyChanged(Prism.UI.Visual.IsLoadedProperty);
                    Loaded(this, EventArgs.Empty);
                }
            };

            base.Unloaded += (o, e) =>
            {
                if (IsLoaded)
                {
                    IsLoaded = false;
                    OnPropertyChanged(Prism.UI.Visual.IsLoadedProperty);
                    Unloaded(this, EventArgs.Empty);
                }
            };
        }

        /// <summary>
        /// Measures the object and returns its desired size.
        /// </summary>
        /// <param name="constraints">The width and height that the object is not allowed to exceed.</param>
        /// <returns>The desired size as a <see cref="Size"/> instance.</returns>
        public Size Measure(Size constraints)
        {
            return base.MeasureOverride(constraints.GetSize()).GetSize();
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override global::Windows.Foundation.Size ArrangeOverride(global::Windows.Foundation.Size finalSize)
        {
            ArrangeRequest(false, null);
            finalSize = Frame.Size.GetSize();
            base.ArrangeOverride(finalSize);
            return finalSize;
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of the layout cycle. Classes can
        /// override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. 
        /// Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override global::Windows.Foundation.Size MeasureOverride(global::Windows.Foundation.Size availableSize)
        {
            var desiredSize = MeasureRequest(false, null).GetSize();
            base.MeasureOverride(desiredSize);
            return desiredSize;
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

