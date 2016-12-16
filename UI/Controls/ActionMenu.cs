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
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeActionMenu"/>.
    /// </summary>
    [Register(typeof(INativeActionMenu))]
    public class ActionMenu : StackPanel, INativeActionMenu
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

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
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
        /// Gets or sets the background for the menu.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    OverflowMenu.MenuFlyoutPresenterStyle = new Style()
                    {
                        TargetType = typeof(MenuFlyoutPresenter),
                        Setters = { new Setter(Control.BackgroundProperty, background.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.BackgroundChromeMediumLowBrushId)) }
                    };

                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the title of the menu's Cancel button, if one exists.
        /// </summary>
        public string CancelButtonTitle
        {
            get { return cancelButtonTitle; }
            set
            {
                if (value != cancelButtonTitle)
                {
                    cancelButtonTitle = value;
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.CancelButtonTitleProperty);
                }
            }
        }
        private string cancelButtonTitle;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the foreground content of the menu.
        /// </summary>
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;

                    var brush = foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId);
                    OverflowButton.Foreground = brush;
                    foreach (var item in Items.OfType<INativeMenuItem>().Where(i => i.Foreground == null))
                    {
                        var control = item as Control;
                        if (control != null)
                        {
                            control.Foreground = brush;
                        }
                    }

                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.ForegroundProperty);
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
        /// Gets a collection of the items within the menu.
        /// </summary>
        public IList Items { get; }

        /// <summary>
        /// Gets or sets the maximum number of menu items that can be displayed before they are placed into an overflow menu.
        /// </summary>
        public int MaxDisplayItems
        {
            get { return maxDisplayItems; }
            set
            {
                if (value != maxDisplayItems)
                {
                    maxDisplayItems = value;
                    SetButtons();
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.MaxDisplayItemsProperty);
                }
            }
        }
        private int maxDisplayItems;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a measurement of itself and its children.
        /// </summary>
        public MeasureRequestHandler MeasureRequest { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> of the image to use for representing the overflow menu when one is present.
        /// </summary>
        public Uri OverflowImageUri
        {
            get { return overflowImageUri; }
            set
            {
                if (value != overflowImageUri)
                {
                    overflowImageUri = value;
                    OverflowButton.Content = overflowImageUri == null ? (object)(new SymbolIcon(Symbol.More)) : new BitmapIcon() { UriSource = overflowImageUri };
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.OverflowImageUriProperty);
                }
            }
        }
        private Uri overflowImageUri;

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

                    var transform = renderTransform as Media.Transform ?? renderTransform as global::Windows.UI.Xaml.Media.Transform;
                    OverflowButton.RenderTransform = transform;
                    foreach (var child in Items.OfType<UIElement>())
                    {
                        child.RenderTransform = transform;
                    }

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
        /// Gets the button that controls activation of the overflow menu.
        /// </summary>
        protected global::Windows.UI.Xaml.Controls.Button OverflowButton { get; }

        /// <summary>
        /// Gets the control that presents overflow content.
        /// </summary>
        protected MenuFlyout OverflowMenu { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMenu"/> class.
        /// </summary>
        public ActionMenu()
        {
            Orientation = global::Windows.UI.Xaml.Controls.Orientation.Horizontal;

            OverflowButton = new global::Windows.UI.Xaml.Controls.Button()
            {
                Background = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Colors.Transparent),
                Content = new SymbolIcon(Symbol.More),
                Flyout = (OverflowMenu = new MenuFlyout()),
                Padding = new global::Windows.UI.Xaml.Thickness(8),
                RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5)
            };

            OverflowButton.Click += (o, e) =>
            {
                OverflowMenu.Items.Clear();
                for (int i = maxDisplayItems; i < Items.Count; i++)
                {
                    var item = Items[i] as INativeMenuItem;
                    var menuButton = item as INativeMenuButton;
                    if (menuButton != null)
                    {
                        OverflowMenu.Items.Add(new MenuFlyoutItem()
                        {
                            Command = new MenuButtonCommand(),
                            CommandParameter = item,
                            Foreground = menuButton.Foreground.GetBrush() ?? foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId),
                            IsEnabled = menuButton.IsEnabled,
                            IsHitTestVisible = base.IsHitTestVisible,
                            Text = menuButton.Title
                        });
                    }
                    else if (item is INativeMenuSeparator)
                    {
                        OverflowMenu.Items.Add(new MenuFlyoutSeparator()
                        {
                            Background = item.Foreground.GetBrush() ?? foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId),
                        });
                    }
                }
            };

            Items = new ObservableCollection<INativeMenuItem>();
            ((ObservableCollection<INativeMenuItem>)Items).CollectionChanged += (o, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewStartingIndex < maxDisplayItems || Items.Count > maxDisplayItems && (Items.Count - e.NewItems.Count <= maxDisplayItems))
                        {
                            SetButtons();
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        if (e.NewStartingIndex < maxDisplayItems || e.OldStartingIndex < maxDisplayItems)
                        {
                            SetButtons();
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldStartingIndex < maxDisplayItems)
                        {
                            SetButtons();
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (e.NewStartingIndex < maxDisplayItems)
                        {
                            SetButtons();
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Children.Clear();
                        OverflowMenu.Items.Clear();
                        break;
                }

                if (Foreground != null && e.NewItems != null)
                {
                    var brush = Foreground.GetBrush();
                    foreach (var item in e.NewItems.OfType<INativeMenuItem>())
                    {
                        var element = item as UIElement;
                        if (element != null)
                        {
                            element.RenderTransform = renderTransform as Media.Transform ?? renderTransform as global::Windows.UI.Xaml.Media.Transform;
                        }

                        if (item.Foreground == null)
                        {
                            var control = item as Control;
                            if (control != null)
                            {
                                control.Foreground = brush;
                            }
                        }
                    }
                }
            };

            base.Loaded += (o, e) =>
            {
                IsLoaded = true;
                OnPropertyChanged(Visual.IsLoadedProperty);
                Loaded(this, EventArgs.Empty);
            };

            base.Unloaded += (o, e) =>
            {
                IsLoaded = false;
                OnPropertyChanged(Visual.IsLoadedProperty);
                Unloaded(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// Measures the object and returns its desired size.
        /// </summary>
        /// <param name="constraints">The width and height that the object is not allowed to exceed.</param>
        /// <returns>The desired size as a <see cref="Size"/> instance.</returns>
        public Size Measure(Size constraints)
        {
            base.Measure(constraints.GetSize());
            return DesiredSize.GetSize();
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override global::Windows.Foundation.Size ArrangeOverride(global::Windows.Foundation.Size finalSize)
        {
            ArrangeRequest(false, null);
            return base.ArrangeOverride(finalSize);
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
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Called when a property value is changed.
        /// </summary>
        /// <param name="pd">A property descriptor describing the property whose value has been changed.</param>
        protected virtual void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }

        private void SetButtons()
        {
            bool hasOverflow = maxDisplayItems < Items.Count;
            var items = ((ObservableCollection<INativeMenuItem>)Items).Take(hasOverflow ? maxDisplayItems : Items.Count).OfType<UIElement>();
            var itemsEnumerator = items.GetEnumerator();

            Children.Clear();

            int count = items.Count() + (hasOverflow ? 1 : 0);
            for (int i = 0; i < count; i++)
            {
                if (i == count - 1 && hasOverflow)
                {
                    Children.Insert(i, OverflowButton);
                }
                else if (itemsEnumerator.MoveNext())
                {
                    Children.Insert(i, itemsEnumerator.Current);
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
                (parameter as INativeMenuButton).Action();
            }
        }
    }
}
