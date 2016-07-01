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
using System.Linq;
using Prism.Input;
using Prism.Native;
using Prism.UI.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeListBoxItem"/>.
    /// </summary>
    [Register(typeof(INativeListBoxItem))]
    public class ListBoxItem : Canvas, INativeListBoxItem
    {
        private static DependencyProperty IsSelectedProperty { get; } =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ListBoxItem), new global::Windows.UI.Xaml.PropertyMetadata(false, OnDependencyPropertyChanged));

        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when the system loses track of the pointer for some reason.
        /// </summary>
        public new event EventHandler<PointerEventArgs> PointerCanceled;

        /// <summary>
        /// Occurs when the pointer has moved while over the element.
        /// </summary>
        public new event EventHandler<PointerEventArgs> PointerMoved;

        /// <summary>
        /// Occurs when the pointer has been pressed while over the element.
        /// </summary>
        public new event EventHandler<PointerEventArgs> PointerPressed;

        /// <summary>
        /// Occurs when the pointer has been released while over the element.
        /// </summary>
        public new event EventHandler<PointerEventArgs> PointerReleased;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Occurs when this instance has been detached from the visual tree.
        /// </summary>
        public new event EventHandler Unloaded;

        /// <summary>
        /// Gets or sets the accessory for the item.
        /// </summary>
        public Prism.UI.Controls.ListBoxItemAccessory Accessory { get; set; }

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
                        separator.Transitions = separatorTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        childrenTransitions = ChildrenTransitions;
                        separatorTransitions = separator.Transitions;

                        Transitions = null;
                        ChildrenTransitions = null;
                        separator.Transitions = null;
                    }
                    
                    OnPropertyChanged(Prism.UI.Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, childrenTransitions, separatorTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the background for the control.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;

                    if (!IsSelected)
                    {
                        base.Background = background.GetBrush();
                    }

                    OnPropertyChanged(Prism.UI.Controls.ListBoxItem.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the panel containing the content to be displayed by the item.
        /// </summary>
        public INativePanel ContentPanel
        {
            get { return Children.OfType<INativePanel>().FirstOrDefault(); }
            set
            {
                if (value != ContentPanel)
                {
                    if (Children.Count > 1)
                    {
                        Children[0] = (UIElement)value;
                    }
                    else
                    {
                        Children.Insert(0, (UIElement)value);
                    }
                    OnPropertyChanged(Prism.UI.Controls.ListBoxItem.ContentPanelProperty);
                }
            }
        }

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
                    OnPropertyChanged(Prism.UI.Visual.IsHitTestVisibleProperty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has been loaded and is ready for rendering.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
        }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a measurement of itself and its children.
        /// </summary>
        public MeasureRequestHandler MeasureRequest { get; set; }

        /// <summary>
        /// Gets or sets the background of the item when it is selected.
        /// </summary>
        public Brush SelectedBackground
        {
            get { return selectedBackground; }
            set
            {
                if (value != selectedBackground)
                {
                    selectedBackground = value;
                    
                    if (IsSelected)
                    {
                        base.Background = selectedBackground.GetBrush() ?? selectedBackgroundDefault;
                    }

                    OnPropertyChanged(Prism.UI.Controls.ListBoxItem.SelectedBackgroundProperty);
                }
            }
        }
        private Brush selectedBackground;

        /// <summary>
        /// Gets or sets the amount to indent the separator.
        /// </summary>
        public Thickness SeparatorIndentation
        {
            get { return separatorIndentation; }
            set
            {
                if (value != separatorIndentation)
                {
                    separatorIndentation = value;
                    OnPropertyChanged(Prism.UI.Controls.ListBoxItem.SeparatorIndentationProperty);
                    SetSeparatorPosition();
                }
            }
        }
        private Thickness separatorIndentation = new Thickness(12, 0);

        /// <summary>
        /// Gets or sets the display state of the element.
        /// </summary>
        public new Prism.UI.Visibility Visibility
        {
            get { return visibility; }
            set
            {
                if (value != Visibility)
                {
                    visibility = value;

                    base.Visibility = visibility == Prism.UI.Visibility.Visible ?
                        global::Windows.UI.Xaml.Visibility.Visible : global::Windows.UI.Xaml.Visibility.Collapsed;

                    OnPropertyChanged(Prism.UI.Controls.Element.VisibilityProperty);
                }
            }
        }
        private Prism.UI.Visibility visibility;
        
        private readonly Line separator;
        private bool isInitialized;
        private double? parentWidth;
        private global::Windows.UI.Xaml.Media.Brush selectedBackgroundDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxItem"/> class.
        /// </summary>
        public ListBoxItem()
        {
            separator = new Line() { Name = "separator" };
            Children.Add(separator);

            HorizontalAlignment = HorizontalAlignment.Stretch;

            base.Loaded += (o, e) =>
            {
                OnInitialize();

                parentWidth = this.GetParent<INativeListBox>()?.Frame.Width;

                var presenter = this.GetParent<ListViewItemPresenter>();
                if (presenter != null)
                {
                    selectedBackgroundDefault = presenter.SelectedBackground;
                    presenter.SelectedBackground = presenter.SelectedPointerOverBackground = null;
                }

                var lvi = this.GetParent<ListViewItem>();
                if (lvi != null)
                {
                    SetBinding(IsSelectedProperty, new Binding()
                    {
                        Source = lvi,
                        Path = new global::Windows.UI.Xaml.PropertyPath("IsSelected"),
                        Mode = BindingMode.OneWay
                    });
                }

                IsLoaded = true;
                OnPropertyChanged(Prism.UI.Visual.IsLoadedProperty);
                Loaded(this, EventArgs.Empty);
            };

            base.PointerCanceled += (o, e) =>
            {
                e.Handled = true;
                PointerCanceled(this, e.GetPointerEventArgs(this));
            };

            base.PointerMoved += (o, e) =>
            {
                e.Handled = true;
                PointerMoved(this, e.GetPointerEventArgs(this));
            };

            base.PointerPressed += (o, e) =>
            {
                e.Handled = true;
                PointerPressed(this, e.GetPointerEventArgs(this));
            };

            base.PointerReleased += (o, e) =>
            {
                e.Handled = true;
                PointerReleased(this, e.GetPointerEventArgs(this));
            };

            base.SizeChanged += (o, e) =>
            {
                SetSeparatorPosition();
            };

            base.Tapped += (o, e) =>
            {
                this.GetParent<ListBox>()?.OnItemClicked(this);
            };

            base.Unloaded += (o, e) =>
            {
                IsLoaded = false;
                OnPropertyChanged(Prism.UI.Visual.IsLoadedProperty);
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
            return constraints;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.</summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override global::Windows.Foundation.Size ArrangeOverride(global::Windows.Foundation.Size finalSize)
        {
            var width = this.GetParent<INativeListBox>()?.Frame.Width ?? finalSize.Width;
            ArrangeRequest(false, new Rectangle(0, frame.Y, width, finalSize.Height));
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
            var width = this.GetParent<INativeListBox>()?.Frame.Width;
            var desiredSize = MeasureRequest(parentWidth != width, new Size(width ?? double.PositiveInfinity, double.PositiveInfinity)).GetSize();
            parentWidth = width;

            base.MeasureOverride(desiredSize);
            return desiredSize;
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// In simplest terms, this means the method is called just before a UI element displays in your app.
        /// Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnInitialize();
        }

        /// <summary>
        /// Called when a property value is changed.
        /// </summary>
        /// <param name="pd">A property descriptor describing the property whose value has been changed.</param>
        protected virtual void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }

        private static void OnDependencyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null && e.Property == IsSelectedProperty)
            {
                item.SetValue(BackgroundProperty, item.IsSelected ? (item.selectedBackground.GetBrush() ?? item.selectedBackgroundDefault) : item.background.GetBrush());
                item.OnPropertyChanged(Prism.UI.Controls.ListBoxItem.IsSelectedProperty);
            }
        }

        private void OnInitialize()
        {
            if (isInitialized)
            {
                return;
            }

            var presenter = this.GetChild<ListViewItemPresenter>();
            if (presenter == null)
            {
                return;
            }

            isInitialized = true;

            presenter.ContentMargin = new global::Windows.UI.Xaml.Thickness();
        }

        private void SetSeparatorPosition()
        {
            separator.X1 = separatorIndentation.Left;
            separator.Y1 = separator.Y2 = Math.Max(0, (ActualHeight - 1) + separatorIndentation.Top - separatorIndentation.Bottom);
            separator.X2 = ActualWidth - separatorIndentation.Right;
            separator.StrokeThickness = 1;
        }
    }
}
