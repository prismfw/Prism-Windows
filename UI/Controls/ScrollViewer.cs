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
using Prism.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeScrollViewer"/>.
    /// </summary>
    [Register(typeof(INativeScrollViewer))]
    public class ScrollViewer : ContentControl, INativeScrollViewer
    {
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
        /// Occurs when the contents of the scroll viewer has been scrolled.
        /// </summary>
        public event EventHandler Scrolled;

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
                        Element.ContentTransitions = elementContentTransitions;
                        presenter.Transitions = presenterTransitions;
                        presenter.ChildrenTransitions = presenterChildrenTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        contentTransitions = ContentTransitions;
                        elementTransitions = Element.Transitions;
                        elementContentTransitions = Element.ContentTransitions;
                        presenterTransitions = presenter.Transitions;
                        presenterChildrenTransitions = presenter.ChildrenTransitions;

                        Transitions = null;
                        ContentTransitions = null;
                        Element.Transitions = null;
                        Element.ContentTransitions = null;
                        presenter.Transitions = null;
                        presenter.ChildrenTransitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, contentTransitions, elementTransitions, elementContentTransitions, presenterTransitions, presenterChildrenTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the contents can be scrolled horizontally.
        /// </summary>
        public bool CanScrollHorizontally
        {
            get { return Element.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled; }
            set
            {
                if (value != CanScrollHorizontally)
                {
                    Element.HorizontalScrollBarVisibility = value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
                    OnPropertyChanged(Prism.UI.Controls.ScrollViewer.CanScrollHorizontallyProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the contents can be scrolled vertically.
        /// </summary>
        public bool CanScrollVertically
        {
            get { return Element.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled; }
            set
            {
                if (value != CanScrollVertically)
                {
                    Element.VerticalScrollBarVisibility = value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
                    OnPropertyChanged(Prism.UI.Controls.ScrollViewer.CanScrollVerticallyProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the content of the scroll viewer.
        /// </summary>
        public new object Content
        {
            get { return presenter.Children.FirstOrDefault(); }
            set
            {
                var element = value as UIElement;
                if (element != presenter.Children.FirstOrDefault())
                {
                    if (presenter.Children.Count > 0)
                    {
                        presenter.Children[0] = element;
                    }
                    else
                    {
                        presenter.Children.Add(element);
                    }

                    OnPropertyChanged(Prism.UI.Controls.ScrollViewer.ContentProperty);
                }
            }
        }

        /// <summary>
        /// Gets the distance that the contents has been scrolled.
        /// </summary>
        public Point ContentOffset
        {
            get { return new Point(Element.HorizontalOffset, Element.VerticalOffset); }
        }

        /// <summary>
        /// Gets the size of the scrollable area.
        /// </summary>
        public Size ContentSize { get; private set; }

        /// <summary>
        /// Gets or sets a <see cref="Rectangle"/> that represents the size and position of the object relative to its parent container.
        /// </summary>
        public Rectangle Frame
        {
            get { return frame; }
            set
            {
                frame = value;

                Width = Element.Width = value.Width;
                Height = Element.Height = value.Height;
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
        /// Gets or sets the level of opacity for the element.
        /// </summary>
        public new double Opacity
        {
            get { return base.Opacity; }
            set
            {
                if (value != base.Opacity)
                {
                    base.Opacity = value;
                    OnPropertyChanged(Prism.UI.Controls.Element.OpacityProperty);
                }
            }
        }

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

        /// <summary>
        /// Gets the UI element that is scrolling the content.
        /// </summary>
        protected global::Windows.UI.Xaml.Controls.ScrollViewer Element { get; }

        private readonly ScrollViewerContentPresenter presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollViewer"/> class.
        /// </summary>
        public ScrollViewer()
        {
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            Element = new global::Windows.UI.Xaml.Controls.ScrollViewer()
            {
                Content = (presenter = new ScrollViewerContentPresenter()),
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch,
                Padding = new global::Windows.UI.Xaml.Thickness(0, 0, 0, 0),
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            base.Content = Element;

            Element.ViewChanged += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.Controls.ScrollViewer.ContentOffsetProperty);
                Scrolled(this, EventArgs.Empty);
            };

            base.Loaded += (o, e) =>
            {
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
        /// Scrolls the contents to the specified offset.
        /// </summary>
        /// <param name="offset">The position to which to scroll the contents.</param>
        /// <param name="animate">Whether to animate the scrolling.</param>
        public void ScrollTo(Point offset, Animate animate)
        {
            Element.ChangeView(offset.X, offset.Y, null, animate == Animate.Off);
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

            if (Element.ExtentWidth != ContentSize.Width || Element.ExtentHeight != ContentSize.Height)
            {
                ContentSize = new Size(Element.ExtentWidth, Element.ExtentHeight);
                OnPropertyChanged(Prism.UI.Controls.ScrollViewer.ContentSizeProperty);
            }

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

        private class ScrollViewerContentPresenter : Canvas
        {
            public ScrollViewerContentPresenter()
            {
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch;
            }

            protected override global::Windows.Foundation.Size MeasureOverride(global::Windows.Foundation.Size availableSize)
            {
                base.MeasureOverride(availableSize);

                var child = Children.FirstOrDefault();
                if (child == null)
                {
                    return global::Windows.Foundation.Size.Empty;
                }

                return new global::Windows.Foundation.Size(child.DesiredSize.Width + GetLeft(child) - Margin.Left - Margin.Right,
                    child.DesiredSize.Height + GetTop(child) - Margin.Top - Margin.Bottom);
            }
        }
    }
}
