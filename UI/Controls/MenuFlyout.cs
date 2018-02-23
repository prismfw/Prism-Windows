/*
Copyright (C) 2018  Prism Framework Team

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
using System.Collections.Specialized;
using System.Windows.Input;
using Prism.Native;
using Prism.UI;
using Prism.UI.Controls;
using Prism.UI.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeMenuFlyout"/>.
    /// </summary>
    [Register(typeof(INativeMenuFlyout))]
    public class MenuFlyout : global::Windows.UI.Xaml.Controls.MenuFlyout, INativeMenuFlyout
    {
        /// <summary>
        /// Occurs when the flyout has been closed.
        /// </summary>
        public new event EventHandler Closed;

        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public event EventHandler Loaded;

        /// <summary>
        /// Occurs when the flyout has been opened.
        /// </summary>
        public new event EventHandler Opened;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Occurs when this instance has been detached from the visual tree.
        /// </summary>
        public event EventHandler Unloaded;

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
                    if (Presenter != null)
                    {
                        SetTransitions();
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = false;
        private TransitionCollection transitions, itemContainerTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the background for the flyout.
        /// </summary>
        public Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    if (Presenter != null)
                    {
                        Presenter.Background = background.GetBrush();
                    }

                    OnPropertyChanged(FlyoutBase.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the background for the flyout.
        /// </summary>
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;
                    if (IsLoaded)
                    {
                        for (int i = 0; i < base.Items.Count; i++)
                        {
                            SetMenuItem(base.Items[i], Items[i]);
                        }
                    }

                    OnPropertyChanged(Prism.UI.Controls.MenuFlyout.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets or sets a <see cref="Rectangle"/> that represents the size and position of the element relative to its parent container.
        /// </summary>
        public Rectangle Frame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can be considered a valid result for hit testing.
        /// </summary>
        public bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set
            {
                if (value != isHitTestVisible)
                {
                    isHitTestVisible = value;
                    if (Presenter != null)
                    {
                        Presenter.IsHitTestVisible = value;
                    }

                    OnPropertyChanged(Visual.IsHitTestVisibleProperty);
                }
            }
        }
        private bool isHitTestVisible = true;

        /// <summary>
        /// Gets a value indicating whether this instance has been loaded and is ready for rendering.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets a collection of the items within the menu.
        /// </summary>
        public new IList Items { get; }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a measurement of itself and its children.
        /// </summary>
        public MeasureRequestHandler MeasureRequest { get; set; }

        /// <summary>
        /// Gets or sets the placement of the flyout in relation to its placement target.
        /// </summary>
        public new FlyoutPlacement Placement
        {
            get { return placement; }
            set
            {
                if (value != placement)
                {
                    placement = value;
                    base.Placement = value.GetFlyoutPlacementMode();
                    OnPropertyChanged(FlyoutBase.PlacementProperty);
                }
            }
        }
        private FlyoutPlacement placement;

        /// <summary>
        /// Gets the presenter that is presenting the content of the flyout.
        /// </summary>
        public MenuFlyoutPresenter Presenter { get; private set; }

        /// <summary>
        /// Gets or sets transformation information that affects the rendering position of this instance.
        /// </summary>
        public INativeTransform RenderTransform
        {
            get { return renderTransform; }
            set
            {
                if (value != renderTransform)
                {
                    renderTransform = value;
                    if (Presenter != null)
                    {
                        Presenter.RenderTransform = renderTransform as Media.Transform ?? renderTransform as global::Windows.UI.Xaml.Media.Transform;
                    }

                    OnPropertyChanged(Visual.RenderTransformProperty);
                }
            }
        }
        private INativeTransform renderTransform;

        /// <summary>
        /// Gets or sets the visual theme that should be used by this instance.
        /// </summary>
        public Theme RequestedTheme
        {
            get { return requestedTheme; }
            set
            {
                requestedTheme = value;
                if (Presenter != null)
                {
                    Presenter.RequestedTheme = requestedTheme.GetElementTheme();
                }
            }
        }
        private Theme requestedTheme;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuFlyout"/> class.
        /// </summary>
        public MenuFlyout()
        {
            Items = new ObservableCollection<INativeMenuItem>();
            ((ObservableCollection<INativeMenuItem>)Items).CollectionChanged += (o, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                        {
                            var newItem = Items[i];
                            if (newItem is INativeMenuButton)
                            {
                                base.Items.Insert(i, new MenuFlyoutItem()
                                {
                                    Command = new MenuButtonCommand(),
                                    CommandParameter = newItem
                                });
                            }
                            else if (newItem is INativeMenuSeparator)
                            {
                                base.Items.Insert(i, new MenuFlyoutSeparator());
                            }

                            if (IsLoaded)
                            {
                                SetMenuItem(base.Items[i], newItem);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = e.OldStartingIndex + e.OldItems.Count - 1; i >= e.OldStartingIndex; i--)
                        {
                            base.Items.RemoveAt(i);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                        {
                            var newItem = Items[i];
                            var baseItem = base.Items[i];
                            if (newItem is INativeMenuButton && !(baseItem is MenuFlyoutItem))
                            {
                                base.Items[i] = new MenuFlyoutItem()
                                {
                                    Command = new MenuButtonCommand(),
                                    CommandParameter = newItem
                                };
                            }
                            else if (newItem is INativeMenuSeparator && !(baseItem is MenuFlyoutSeparator))
                            {
                                base.Items[i] = new MenuFlyoutSeparator();
                            }

                            if (IsLoaded)
                            {
                                SetMenuItem(baseItem, newItem);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        base.Items.Clear();
                        break;
                }
            };

            base.Closed += (o, e) => Closed(this, EventArgs.Empty);

            base.Opened += (o, e) =>
            {
                if (base.Items.Count > 0)
                {
                    var presenter = base.Items[0].GetParent<MenuFlyoutPresenter>();
                    if (presenter != Presenter)
                    {
                        if (Presenter != null)
                        {
                            Presenter.LayoutUpdated -= OnLayoutUpdated;
                            Presenter.Loaded -= OnLoaded;
                            Presenter.Unloaded -= OnUnloaded;
                        }

                        Presenter = presenter;
                        Presenter.Background = background.GetBrush();
                        Presenter.IsHitTestVisible = isHitTestVisible;
                        Presenter.Padding = new global::Windows.UI.Xaml.Thickness();
                        Presenter.RenderTransform = renderTransform as Media.Transform ?? renderTransform as global::Windows.UI.Xaml.Media.Transform;
                        Presenter.RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);
                        Presenter.RequestedTheme = requestedTheme.GetElementTheme();
                        Presenter.Tag = this;

                        transitions = Presenter.Transitions;
                        itemContainerTransitions = Presenter.ItemContainerTransitions;
                        SetTransitions();

                        Presenter.LayoutUpdated -= OnLayoutUpdated;
                        Presenter.Loaded -= OnLoaded;
                        Presenter.Unloaded -= OnUnloaded;

                        Presenter.LayoutUpdated += OnLayoutUpdated;
                        Presenter.Loaded += OnLoaded;
                        Presenter.Unloaded += OnUnloaded;
                    }
                }

                for (int i = 0; i < base.Items.Count; i++)
                {
                    SetMenuItem(base.Items[i], Items[i]);
                }

                Opened(this, EventArgs.Empty);
                OnLoaded(null, null);
            };
        }

        /// <summary>
        /// Invalidates the arrangement of this instance's children.
        /// </summary>
        public void InvalidateArrange()
        {
            Presenter?.InvalidateArrange();
        }

        /// <summary>
        /// Invalidates the measurement of this instance and its children.
        /// </summary>
        public void InvalidateMeasure()
        {
            Presenter?.InvalidateMeasure();
        }

        /// <summary>
        /// Measures the object and returns its desired size.
        /// </summary>
        /// <param name="constraints">The width and height that the object is not allowed to exceed.</param>
        /// <returns>The desired size as a <see cref="Size"/> instance.</returns>
        public Size Measure(Size constraints)
        {
            if (Presenter != null)
            {
                Presenter.Measure(constraints.GetSize());
                return Presenter.DesiredSize.GetSize();
            }

            return Size.Empty;
        }

        /// <summary>
        /// Presents the flyout and positions it relative to the specified placement target.
        /// </summary>
        /// <param name="placementTarget">The object to use as the flyout's placement target.</param>
        public void ShowAt(object placementTarget)
        {
            base.ShowAt(placementTarget as FrameworkElement);
        }

        /// <summary>
        /// Called when a property value is changed.
        /// </summary>
        /// <param name="pd">A property descriptor describing the property whose value has been changed.</param>
        protected virtual void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }

        private void OnLayoutUpdated(object sender, object e)
        {
            MeasureRequest(false, null);
            ArrangeRequest(false, null);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                IsLoaded = true;
                OnPropertyChanged(Visual.IsLoadedProperty);
                Loaded(this, EventArgs.Empty);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                IsLoaded = false;
                OnPropertyChanged(Visual.IsLoadedProperty);
                Unloaded(this, EventArgs.Empty);
            }
        }

        private void SetTransitions()
        {
            if (areAnimationsEnabled)
            {
                Presenter.Transitions = transitions;
                Presenter.ItemContainerTransitions = itemContainerTransitions;
            }
            else
            {
                transitions = Presenter.Transitions;
                itemContainerTransitions = Presenter.ItemContainerTransitions;

                Presenter.Transitions = null;
                Presenter.ItemContainerTransitions = null;
            }
        }

        private void SetMenuItem(MenuFlyoutItemBase baseItem, object menuItem)
        {
            var item = baseItem as MenuFlyoutItem;
            if (item != null)
            {
                var button = menuItem as INativeMenuButton;
                if (button != null)
                {
                    item.Foreground = button.Foreground.GetBrush() ?? foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId);
                    item.IsEnabled = button.IsEnabled;
                    item.Text = button.Title;
                }
            }
            else
            {
                var separator = baseItem as MenuFlyoutSeparator;
                if (separator != null)
                {
                    separator.Background = (menuItem as INativeMenuSeparator)?.Foreground.GetBrush() ?? foreground.GetBrush() ??
                        Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId);
                }
            }
        }

        private class MenuButtonCommand : ICommand
        {
            event EventHandler ICommand.CanExecuteChanged { add { } remove { } }

            public bool CanExecute(object parameter)
            {
                return (parameter as INativeMenuButton)?.IsEnabled ?? true;
            }

            public void Execute(object parameter)
            {
                (parameter as INativeMenuButton)?.Action();
            }
        }
    }
}
