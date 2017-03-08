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
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeTabItem"/>.
    /// </summary>
    [Register(typeof(INativeTabItem))]
    public class TabItem : PivotItem, INativeTabItem
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
                        ContentTransitions = contentTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        contentTransitions = ContentTransitions;

                        Transitions = null;
                        ContentTransitions = null;
                    }

                    OnPropertyChanged(Prism.UI.Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, contentTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the object that acts as the content of the item.
        /// This is typically an <see cref="IView"/> or <see cref="INativeViewStack"/> instance.
        /// </summary>
        public new object Content
        {
            get { return base.Content; }
            set
            {
                (base.Content as DependencyObject)?.UnregisterPropertyChangedCallback(Control.BackgroundProperty, callbackToken);

                base.Content = value;

                var control = value as Control;
                if (control != null)
                {
                    Background = control.Background;
                    callbackToken = control.RegisterPropertyChangedCallback(Control.BackgroundProperty, (o, e) => { Background = ((Control)o).Background; });
                }
            }
        }

        /// <summary>
        /// Gets or sets the font to use for displaying the title text.
        /// </summary>
        public new object FontFamily
        {
            get { return TextBlock.FontFamily; }
            set
            {
                var fontFamily = value as Media.FontFamily;
                if (fontFamily != TextBlock.FontFamily)
                {
                    TextBlock.SetFont(fontFamily, FontStyle);
                    OnPropertyChanged(Prism.UI.Controls.TabItem.FontFamilyProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the title text.
        /// </summary>
        public new double FontSize
        {
            get { return TextBlock.FontSize; }
            set
            {
                if (value != TextBlock.FontSize)
                {
                    TextBlock.FontSize = value;
                    OnPropertyChanged(Prism.UI.Controls.TabItem.FontSizeProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style with which to render the title text.
        /// </summary>
        public new FontStyle FontStyle
        {
            get { return TextBlock.GetFontStyle(); }
            set
            {
                var style = TextBlock.FontStyle;
                var weight = TextBlock.FontWeight.Weight;
                TextBlock.SetFont(TextBlock.FontFamily as Media.FontFamily, value);

                if (TextBlock.FontStyle != style || TextBlock.FontWeight.Weight != weight)
                {
                    OnPropertyChanged(Prism.UI.Controls.TabItem.FontStyleProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the title.
        /// </summary>
        public new Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;
                    if (this.GetParent<Pivot>()?.SelectedItem != this)
                    {
                        TextBlock.Foreground = foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseMediumLowBrushId);
                    }

                    OnPropertyChanged(Prism.UI.Controls.TabItem.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets or sets a <see cref="Rectangle"/> that represents the size and position of the object relative to its parent container.
        /// </summary>
        public Rectangle Frame { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="INativeImageSource"/> for an image to display with the item.
        /// </summary>
        public INativeImageSource Image
        {
            get { return image; }
            set
            {
                if (value != image)
                {
                    image = value;
                    ImageElement.Source = image.GetImageSource();
                    ImageElement.Visibility = ImageElement.Source == null ? Visibility.Collapsed : Visibility.Visible;
                    OnPropertyChanged(Prism.UI.Controls.TabItem.ImageProperty);
                }
            }
        }
        private INativeImageSource image;

        /// <summary>
        /// Gets or sets a value indicating whether the user can interact with the item.
        /// </summary>
        public new bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;

                    var element = Header as UIElement;
                    if (element != null)
                    {
                        element.Opacity = value ? 1.0 : 0.4;
                    }

                    var item = (Header as DependencyObject)?.GetParent<PivotHeaderItem>();
                    if (item != null)
                    {
                        item.IsEnabled = value;
                    }

                    OnPropertyChanged(Prism.UI.Controls.TabItem.IsEnabledProperty);
                }
            }
        }
        private bool isEnabled = true;

        /// <summary>
        /// Gets or sets a value indicating whether this instance can be considered a valid result for hit testing.
        /// </summary>
        public new bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set
            {
                if (value != isHitTestVisible)
                {
                    isHitTestVisible = value;

                    var item = (Header as DependencyObject)?.GetParent<PivotHeaderItem>();
                    if (item != null)
                    {
                        item.IsHitTestVisible = value;
                    }

                    OnPropertyChanged(Prism.UI.Visual.IsHitTestVisibleProperty);
                }
            }
        }
        private bool isHitTestVisible = true;

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

                    var item = (Header as DependencyObject)?.GetParent<PivotHeaderItem>();
                    if (item != null)
                    {
                        item.RenderTransform = renderTransform as Media.Transform ?? renderTransform as global::Windows.UI.Xaml.Media.Transform;
                    }

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
        protected global::Windows.UI.Xaml.Controls.Image ImageElement { get; }

        /// <summary>
        /// Gets the element that displays the colored rectangle on the bottom when the tab is selected.
        /// </summary>
        protected global::Windows.UI.Xaml.Shapes.Rectangle RectangleElement { get; }

        /// <summary>
        /// Gets the element that displays the title text of the tab.
        /// </summary>
        protected TextBlock TextBlock { get; }

        private long callbackToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabItem"/> class.
        /// </summary>
        public TabItem()
        {
            var stackPanel = new StackPanel()
            {
                MinWidth = 96,
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Header = stackPanel;

            stackPanel.Children.Add(ImageElement = new global::Windows.UI.Xaml.Controls.Image()
            {
                Height = 24,
                Width = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new global::Windows.UI.Xaml.Thickness(0, 0, 0, 6),
                Name = "Icon",
                Stretch = global::Windows.UI.Xaml.Media.Stretch.Uniform,
                Visibility = Visibility.Collapsed
            });
            
            stackPanel.Children.Add(TextBlock = new TextBlock
            {
                FontSize = 1, // The default size is 15, but the rendered size is significantly larger unless we set it ourselves.
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new global::Windows.UI.Xaml.Thickness(0, 0, 0, 6),
                MaxLines = 1,
                Name = "Title",
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            stackPanel.Children.Add(RectangleElement = new global::Windows.UI.Xaml.Shapes.Rectangle()
            {
                Height = 2,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Name = "Rect",
                Opacity = 0
            });

            stackPanel.Tapped += (o, e) =>
            {
                this.GetParent<TabView>()?.OnTabItemClicked(this);
            };

            // Doing this sucks, but not doing it throws a non-sensical exception
            Style = (Style)XamlReader.Load(@"<Style TargetType=""ContentControl"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <Setter Property=""Background"" Value=""Transparent"" />
                <Setter Property=""Margin"" Value=""0""/>
                <Setter Property=""Padding"" Value=""0"" />
                <Setter Property=""HorizontalContentAlignment"" Value=""Stretch"" />
                <Setter Property=""VerticalContentAlignment"" Value=""Stretch"" />
                <Setter Property=""IsTabStop"" Value=""False"" />
                <Setter Property=""Template"">
                  <Setter.Value>
                    <ControlTemplate TargetType=""ContentControl"">
                      <Grid Background=""{TemplateBinding Background}"" HorizontalAlignment=""{TemplateBinding HorizontalAlignment}"" VerticalAlignment=""{TemplateBinding VerticalAlignment}"">
                        <VisualStateManager.VisualStateGroups>
                          <VisualStateGroup x:Name=""Pivot"">
                            <VisualState x:Name=""Right"" />
                            <VisualState x:Name=""Left"" />
                            <VisualState x:Name=""Center"" />
                          </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                        <ContentPresenter HorizontalAlignment=""{TemplateBinding HorizontalContentAlignment}"" VerticalAlignment=""{TemplateBinding VerticalContentAlignment}"" ContentTemplate=""{TemplateBinding ContentTemplate}"" Content=""{TemplateBinding Content}"" Margin=""{TemplateBinding Padding}"" />
                      </Grid>
                    </ControlTemplate>
                  </Setter.Value>
                </Setter>
            </Style>");

            base.Loaded += (o, e) =>
            {
                var item = (Header as DependencyObject)?.GetParent<PivotHeaderItem>();
                if (item != null)
                {
                    item.VerticalContentAlignment = VerticalAlignment.Bottom;

                    item.IsEnabled = isEnabled;
                    item.IsHitTestVisible = isHitTestVisible;
                    item.RenderTransform = renderTransform as Media.Transform ?? renderTransform as global::Windows.UI.Xaml.Media.Transform;
                    item.RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);
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
            base.MeasureOverride(constraints.GetSize());
            return ((Header as FrameworkElement)?.DesiredSize ?? DesiredSize).GetSize();
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override global::Windows.Foundation.Size ArrangeOverride(global::Windows.Foundation.Size finalSize)
        {
            ArrangeRequest(false, null);
            base.ArrangeOverride(finalSize);

            var item = (Header as DependencyObject)?.GetParent<PivotHeaderItem>();
            var panel = item?.GetParent<PivotHeaderPanel>();
            if (panel != null)
            {
                item.Height = panel.Height;
            }

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
            MeasureRequest(false, null);
            base.MeasureOverride(availableSize);
            return availableSize;
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

