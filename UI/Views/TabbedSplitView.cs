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
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeTabbedSplitView"/>.
    /// </summary>
    [Register(typeof(INativeTabbedSplitView))]
    public class TabbedSplitView : global::Windows.UI.Xaml.Controls.SplitView, INativeTabbedSplitView
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
        public event EventHandler<NativeItemSelectedEventArgs> TabItemSelected;

        /// <summary>
        /// Occurs when this instance has been detached from the visual tree.
        /// </summary>
        public new event EventHandler Unloaded;

        /// <summary>
        /// Gets or sets the foreground for the button that activates the side pane.
        /// </summary>
        public Brush ActivationButtonForeground
        {
            get { return activationButtonForeground; }
            set
            {
                activationButtonForeground = value;
                if (ActivationButton != null)
                {
                    ActivationButton.Foreground = value.GetBrush();
                }
            }
        }
        private Brush activationButtonForeground;

        /// <summary>
        /// Gets the actual width of the detail pane.
        /// </summary>
        public double ActualDetailWidth { get; private set; }

        /// <summary>
        /// Gets the actual width of the master pane.
        /// </summary>
        public double ActualMasterWidth { get; private set; }

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
                        SplitView.Transitions = splitTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        splitTransitions = SplitView.Transitions;

                        Transitions = null;
                        SplitView.Transitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, splitTransitions;

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
                    PaneBackground = background.GetBrush();
                    OnPropertyChanged(Prism.UI.TabView.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the object that acts as the content for the detail pane.
        /// </summary>
        public object DetailContent
        {
            get { return SplitView.Content; }
            set
            {
                var element = value as UIElement;
                if (element != SplitView.Content)
                {
                    SplitView.Content?.UnregisterPropertyChangedCallback(Control.BackgroundProperty, callbackToken2);

                    SplitView.Content = element;
                    
                    var control = SplitView.Content as Control;
                    if (control != null)
                    {
                        SplitView.Background = control.Background;
                        callbackToken2 = control.RegisterPropertyChangedCallback(Control.BackgroundProperty, (o2, e2) => { SplitView.Background = ((Control)o2).Background; });
                    }
                }
            }
        }

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
                    foreach (var presenter in ListView.Items.Select(i => (i as DependencyObject).GetParent<ListViewItemPresenter>()).Where(p => p != null))
                    {
                        presenter.SelectedBackground = presenter.SelectedPointerOverBackground = foreground.GetBrush() ?? Controls.TabItem.SelectedBackgroundDefault;
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
        /// Gets or sets the maximum width of the master pane.
        /// </summary>
        public double MaxMasterWidth
        {
            get { return maxMasterWidth; }
            set
            {
                if (value != maxMasterWidth)
                {
                    maxMasterWidth = value;
                    OnPropertyChanged(Prism.UI.SplitView.MaxMasterWidthProperty);
                    ResizePanes(new Size(ActualWidth, ActualHeight));
                }
            }
        }
        private double maxMasterWidth;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a measurement of itself and its children.
        /// </summary>
        public MeasureRequestHandler MeasureRequest { get; set; }

        /// <summary>
        /// Gets or sets the minimum width of the master pane.
        /// </summary>
        public double MinMasterWidth
        {
            get { return minMasterWidth; }
            set
            {
                if (value != minMasterWidth)
                {
                    minMasterWidth = value;
                    OnPropertyChanged(Prism.UI.SplitView.MinMasterWidthProperty);
                    ResizePanes(new Size(ActualWidth, ActualHeight));
                }
            }
        }
        private double minMasterWidth;

        /// <summary>
        /// Gets or sets the preferred width of the master pane as a percentage of the width of the split view.
        /// Valid values are between 0.0 and 1.0.
        /// </summary>
        public double PreferredMasterWidthRatio
        {
            get { return preferredMasterWidthRatio; }
            set
            {
                if (value != preferredMasterWidthRatio)
                {
                    preferredMasterWidthRatio = value;
                    OnPropertyChanged(Prism.UI.SplitView.PreferredMasterWidthRatioProperty);
                    ResizePanes(new Size(ActualWidth, ActualHeight));
                }
            }
        }
        private double preferredMasterWidthRatio;

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
            get { return Math.Max(ListView.SelectedIndex, 0); }
            set
            {
                if (value != ListView.SelectedIndex)
                {
                    ListView.SelectedIndex = value;
                    OnPropertyChanged(Prism.UI.TabView.SelectedIndexProperty);
                }
            }
        }

        /// <summary>
        /// Gets the size and location of the bar that contains the tab items.
        /// </summary>
        public Rectangle TabBarFrame
        {
            get
            {
                double width = 0;
                if ((DisplayMode == SplitViewDisplayMode.Inline || DisplayMode == SplitViewDisplayMode.CompactInline) && IsPaneOpen)
                {
                    width = OpenPaneLength;
                }
                else if (DisplayMode == SplitViewDisplayMode.CompactInline || DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    width = CompactPaneLength;
                }

                return new Rectangle(PanePlacement == SplitViewPanePlacement.Right ? ActualWidth - width : 0, 0, width, ActualHeight);
            }
        }

        /// <summary>
        /// Gets a list of the tab items that are a part of the view.
        /// </summary>
        public IList TabItems
        {
            get { return tabItems; }
        }
        private readonly ObservableCollection<object> tabItems = new ObservableCollection<object>();

        /// <summary>
        /// Gets the button element that opens the tab menu when clicked.
        /// </summary>
        protected Button ActivationButton { get; }

        /// <summary>
        /// Gets the element that contains the tab items.
        /// </summary>
        protected ListView ListView { get; }

        /// <summary>
        /// Gets the element that contains the master and detail content.
        /// </summary>
        protected global::Windows.UI.Xaml.Controls.SplitView SplitView { get; }

        private long callbackToken1, callbackToken2;
        private object currentTab;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabbedSplitView"/> class.
        /// </summary>
        public TabbedSplitView()
        {
            maxMasterWidth = minMasterWidth = 480;
            preferredMasterWidthRatio = 0.3;

            Pane = new StackPanel()
            {
                Orientation = global::Windows.UI.Xaml.Controls.Orientation.Vertical,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch,
                Children =
                {
                    (ActivationButton = new Button()
                    {
                        Background = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Colors.Transparent),
                        Content = new SymbolIcon(Symbol.List), // temporary icon
                        Height = 48,
                        Width = 48
                    }),
                    (ListView = new ListView()
                    {
                        IsItemClickEnabled = true,
                        ItemsSource = tabItems
                    })
                }
            };

            ActivationButton.Click += (o, e) =>
            {
                IsPaneOpen = !IsPaneOpen;
            };

            ListView.ItemClick += (o, e) =>
            {
                if (TabItems.IndexOf(e.ClickedItem) == ListView.SelectedIndex)
                {
                    TabItemSelected(this, new NativeItemSelectedEventArgs(currentTab, e.ClickedItem));
                    currentTab = e.ClickedItem;
                }
            };

            ListView.SelectionChanged += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.TabView.SelectedIndexProperty);

                TabItemSelected(this, new NativeItemSelectedEventArgs(currentTab, ListView.SelectedItem));
                currentTab = ListView.SelectedItem;

                ChangePane((e.RemovedItems.FirstOrDefault() as INativeTabItem)?.Content, (ListView.SelectedItem as INativeTabItem)?.Content);
            };

            Content = SplitView = new global::Windows.UI.Xaml.Controls.SplitView()
            {
                DisplayMode = SplitViewDisplayMode.Inline,
                IsPaneOpen = true,
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch
            };

            SplitView.SizeChanged += (o, e) => ResizePanes(e.NewSize.GetSize());

            DisplayMode = SplitViewDisplayMode.CompactOverlay;
            HorizontalContentAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            OpenPaneLength = 280;
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            base.Loaded += (o, e) =>
            {
                if (ListView.SelectedIndex < 0 && tabItems.Count > 0)
                {
                    ListView.SelectedIndex = 0;
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

        private void ChangePane(object oldContent, object newContent)
        {
            (oldContent as DependencyObject)?.UnregisterPropertyChangedCallback(Control.BackgroundProperty, callbackToken1);

            SplitView.Pane = newContent as UIElement;

            var control = newContent as Control;
            if (control != null)
            {
                SplitView.PaneBackground = control.Background;
                callbackToken1 = control.RegisterPropertyChangedCallback(Control.BackgroundProperty, (o, e) => { SplitView.PaneBackground = ((Control)o).Background; });
            }
        }

        private void ResizePanes(Size totalSize)
        {
            SplitView.OpenPaneLength = Math.Max(minMasterWidth, Math.Min(maxMasterWidth, totalSize.Width * preferredMasterWidthRatio));

            if (SplitView.OpenPaneLength != ActualMasterWidth)
            {
                ActualMasterWidth = SplitView.OpenPaneLength;
                OnPropertyChanged(Prism.UI.SplitView.ActualMasterWidthProperty);
            }

            double detailWidth = Math.Max(totalSize.Width - ((SplitView.DisplayMode & SplitViewDisplayMode.Overlay) != 0 ? 0 : SplitView.OpenPaneLength), 0);
            if (detailWidth != ActualDetailWidth)
            {
                ActualDetailWidth = detailWidth;
                OnPropertyChanged(Prism.UI.SplitView.ActualDetailWidthProperty);
            }
        }
    }
}
