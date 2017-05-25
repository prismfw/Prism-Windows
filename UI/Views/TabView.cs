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
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeTabView"/>.
    /// </summary>
    [Register(typeof(INativeTabView))]
    public class TabView : ContentControl, INativeTabView
    {
        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Occurs when a tab item is selected.
        /// </summary>
        public event EventHandler<NativeItemChangedEventArgs> TabItemSelected;

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
                        Element.Transitions = elementTransitions;
                        Element.ItemContainerTransitions = itemContainerTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        contentTransitions = ContentTransitions;
                        elementTransitions = Element.Transitions;
                        itemContainerTransitions = Element.ItemContainerTransitions;

                        Transitions = null;
                        ContentTransitions = null;
                        Element.Transitions = null;
                        Element.ItemContainerTransitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, contentTransitions, elementTransitions, itemContainerTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the background for the view.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;

                    var headerPanel = this.GetChild<PivotHeaderPanel>(c => c.Visibility != global::Windows.UI.Xaml.Visibility.Collapsed)?.Parent as Panel;
                    if (headerPanel != null)
                    {
                        headerPanel.Background = background.GetBrush();
                    }

                    OnPropertyChanged(Prism.UI.TabView.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the selected tab item.
        /// </summary>
        public new Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;
                    
                    var tabHeader = (Element.SelectedItem as PivotItem)?.Header as DependencyObject;
                    if (tabHeader != null)
                    {
                        var brush = foreground.GetBrush();

                        var textBlock = tabHeader.GetChild<TextBlock>(c => c.Name == "title");
                        if (textBlock != null)
                        {
                            textBlock.Foreground = brush;
                        }

                        // TODO: Tint the tab image.

                        var rect = tabHeader.GetChild<global::Windows.UI.Xaml.Shapes.Rectangle>(c => c.Name == "rect");
                        if (rect != null)
                        {
                            rect.Fill = brush;
                            rect.Opacity = 1;
                        }
                    }
                    
                    OnPropertyChanged(Prism.UI.TabView.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets or sets a <see cref="Rectangle"/> that represents
        /// the size and position of the object relative to its parent container.
        /// </summary>
        public Rectangle Frame
        {
            get { return frame; }
            set
            {
                frame = value;

                Width = value.Width;
                Height = value.Height;
                Canvas.SetLeft(this, value.X);
                Canvas.SetTop(this, value.Y);
            }
        }
        private Rectangle frame = new Rectangle();

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
                    OnPropertyChanged(Visual.IsHitTestVisibleProperty);
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
                    OnPropertyChanged(Visual.RenderTransformProperty);
                }
            }
        }
        private INativeTransform renderTransform;

        /// <summary>
        /// Gets or sets the visual theme that should be used by this instance.
        /// </summary>
        public new Theme RequestedTheme
        {
            get { return base.RequestedTheme.GetTheme(); }
            set { base.RequestedTheme = value.GetElementTheme(); }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the selected tab item.
        /// </summary>
        public int SelectedIndex
        {
            get { return Element.SelectedIndex; }
            set
            {
                if (value != Element.SelectedIndex)
                {
                    Element.SelectedIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets the size and location of the bar that contains the tab items.
        /// </summary>
        public Rectangle TabBarFrame
        {
            get { return new Rectangle(0, 0, ActualWidth, TabBarHeight); }
        }

        /// <summary>
        /// Gets or sets the height of the tab bar.
        /// </summary>
        public double TabBarHeight { get; set; } = 72;

        /// <summary>
        /// Gets a list of the tab items that are a part of the view.
        /// </summary>
        public IList TabItems
        {
            get { return Element.ItemsSource as IList; }
        }

        /// <summary>
        /// Gets the UI element that displays the tab items.
        /// </summary>
        protected Pivot Element { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabView"/> class.
        /// </summary>
        public TabView()
        {
            Content = Element = new Pivot() { ItemsSource = new ObservableCollection<object>() };
            Element.SelectionChanged += (o, e) =>
            {
                var newTab = e.AddedItems.FirstOrDefault();
                var oldTab = e.RemovedItems.FirstOrDefault();

                if ((newTab as INativeTabItem)?.IsEnabled ?? true)
                {
                    SetSelectedForeground();

                    OnPropertyChanged(Prism.UI.TabView.SelectedIndexProperty);
                    TabItemSelected(this, new NativeItemChangedEventArgs(oldTab, newTab));
                }
                else
                {
                    int newIndex = TabItems.IndexOf(newTab);
                    int oldIndex = TabItems.IndexOf(oldTab);

                    if (newIndex > oldIndex)
                    {
                        Element.SelectedIndex = (Element.SelectedIndex == TabItems.Count - 1) ? 0 : ++newIndex;
                    }
                    else
                    {
                        Element.SelectedIndex = (Element.SelectedIndex == 0) ? TabItems.Count - 1 : --newIndex;
                    }
                }
            };
            
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            base.Loaded += (o, e) =>
            {
                var headerPanel = this.GetChild<PivotHeaderPanel>(c => c.Visibility != global::Windows.UI.Xaml.Visibility.Collapsed)?.Parent as Panel;
                if (headerPanel != null)
                {
                    headerPanel.Background = background.GetBrush();
                }

                if (Element.SelectedIndex < 0 && TabItems.Count > 0)
                {
                    Element.SelectedIndex = 0;
                }

                if (!IsLoaded)
                {
                    IsLoaded = true;
                    OnPropertyChanged(Visual.IsLoadedProperty);
                    Loaded(this, EventArgs.Empty);
                }
            };

            base.Unloaded += (o, e) =>
            {
                if (IsLoaded)
                {
                    IsLoaded = false;
                    OnPropertyChanged(Visual.IsLoadedProperty);
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
            return constraints;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.</summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override global::Windows.Foundation.Size ArrangeOverride(global::Windows.Foundation.Size finalSize)
        {
            ArrangeRequest(false, null);
            finalSize = Frame.Size.GetSize();
            base.ArrangeOverride(finalSize);

            var panel = this.GetChild<PivotHeaderPanel>(c => c.Visibility != global::Windows.UI.Xaml.Visibility.Collapsed);
            if (panel != null)
            {
                panel.Height = TabBarHeight;
                panel.HorizontalAlignment = panel.Name == "StaticHeader" && finalSize.Width < 720 ?
                    global::Windows.UI.Xaml.HorizontalAlignment.Center : global::Windows.UI.Xaml.HorizontalAlignment.Left;
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
            var desiredSize = MeasureRequest(false, null).GetSize();
            base.MeasureOverride(desiredSize);
            return desiredSize;
        }

        /// <summary>
        /// Called when a tab item in the view is clicked or tapped.
        /// </summary>
        /// <param name="item">The tab item that was clicked.</param>
        protected internal void OnTabItemClicked(INativeTabItem item)
        {
            // This should only fire if the tab is already the active one.
            if (Element.SelectedIndex == TabItems.IndexOf(item))
            {
                TabItemSelected(this, new NativeItemChangedEventArgs(item, item));
            }
        }

        /// <summary>
        /// Called when a property value is changed.
        /// </summary>
        /// <param name="pd">A property descriptor describing the property whose value has been changed.</param>
        protected virtual void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }

        private void SetSelectedForeground()
        {
            var panel = this.GetChild<PivotHeaderPanel>(p => p.Visibility != global::Windows.UI.Xaml.Visibility.Collapsed);
            if (panel != null)
            {
                for (int i = 0; i < panel.Children.Count; i++)
                {
                    var item = panel.Children[i];

                    var textBlock = item.GetChild<TextBlock>(c => c.Name == "Title");
                    if (textBlock != null)
                    {
                        textBlock.Foreground = i == Element.SelectedIndex ? foreground.GetBrush() :
                            ((TabItems[i] as INativeTabItem)?.Foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseMediumLowBrushId));
                    }

                    // TODO: Tint the tab image.

                    var rect = item.GetChild<global::Windows.UI.Xaml.Shapes.Rectangle>(c => c.Name == "Rect");
                    if (rect != null)
                    {
                        if (i == Element.SelectedIndex)
                        {
                            rect.Fill = foreground.GetBrush();
                            rect.Opacity = 1;
                        }
                        else
                        {
                            rect.Opacity = 0;
                        }
                    }
                }
            }
        }
    }
}
