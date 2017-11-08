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
using System.Linq;
using Prism.Input;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeBorder"/>.
    /// </summary>
    [Register(typeof(INativeBorder))]
    public class Border : Canvas, INativeBorder
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
                        Canvas.Transitions = canvasTransitions;
                        Canvas.ChildrenTransitions = canvasChildrenTransitions;
                        Element.Transitions = elementTransitions;
                        Element.ChildTransitions = elementChildTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        canvasTransitions = Canvas.Transitions;
                        canvasChildrenTransitions = Canvas.ChildrenTransitions;
                        elementTransitions = Element.Transitions;
                        elementChildTransitions = Element.ChildTransitions;

                        Transitions = null;
                        Canvas.Transitions = null;
                        Canvas.ChildrenTransitions = null;
                        Element.Transitions = null;
                        Element.ChildTransitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, canvasTransitions, canvasChildrenTransitions, elementTransitions, elementChildTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the background for this instance.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    Element.Background = background.GetBrush();
                    OnPropertyChanged(Prism.UI.Controls.Border.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> with which to paint the border.
        /// </summary>
        public Brush BorderBrush
        {
            get { return borderBrush; }
            set
            {
                if (value != borderBrush)
                {
                    borderBrush = value;
                    Element.BorderBrush = borderBrush.GetBrush();
                    OnPropertyChanged(Prism.UI.Controls.Border.BorderBrushProperty);
                }
            }
        }
        private Brush borderBrush;

        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return Element.BorderThickness.GetThickness(); }
            set
            {
                var thickness = value.GetThickness();
                if (thickness != Element.BorderThickness)
                {
                    Element.BorderThickness = thickness;
                    OnPropertyChanged(Prism.UI.Controls.Border.BorderThicknessProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the child element around which to draw the border.
        /// </summary>
        public object Child
        {
            get { return Canvas.Children.FirstOrDefault(); }
            set
            {
                var child = value as UIElement;
                if (child != Canvas.Children.FirstOrDefault())
                {
                    Canvas.Children.Clear();
                    Canvas.Children.Add(child);
                    OnPropertyChanged(Prism.UI.Controls.Border.ChildProperty);
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

                Width = Element.Width = Canvas.Width = value.Width;
                Height = Element.Height = Canvas.Height = value.Height;
                SetLeft(this, value.X);
                SetTop(this, value.Y);
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
                    OnPropertyChanged(Prism.UI.Element.OpacityProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the padding between the border and the child element.
        /// </summary>
        public Thickness Padding
        {
            get { return Element.Padding.GetThickness(); }
            set
            {
                var thickness = value.GetThickness();
                if (thickness != Element.Padding)
                {
                    Element.Padding = thickness;
                    OnPropertyChanged(Prism.UI.Controls.Border.PaddingProperty);
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
        /// Gets or sets the visual theme that should be used by this instance.
        /// </summary>
        public new Theme RequestedTheme
        {
            get { return base.RequestedTheme.GetTheme(); }
            set { base.RequestedTheme = value.GetElementTheme(); }
        }

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

                    OnPropertyChanged(Prism.UI.Element.VisibilityProperty);
                }
            }
        }
        private Prism.UI.Visibility visibility;

        /// <summary>
        /// Gets the UI element that displays the border.
        /// </summary>
        protected global::Windows.UI.Xaml.Controls.Border Element { get; }

        /// <summary>
        /// Gets the UI element that contains the child of the border.
        /// </summary>
        protected Canvas Canvas { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Border"/> class.
        /// </summary>
        public Border()
        {
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            Children.Add(Element = new global::Windows.UI.Xaml.Controls.Border()
            {
                Background = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Colors.Transparent),
                IsHitTestVisible = false,
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch
            });

            Children.Add(Canvas = new Canvas()
            {
                Background = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Colors.Transparent),
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch
            });

            base.Loaded += (o, e) =>
            {
                IsLoaded = true;
                OnPropertyChanged(Visual.IsLoadedProperty);
                Loaded(this, EventArgs.Empty);
            };

            base.PointerCanceled += (o, e) =>
            {
                if ((e.OriginalSource as DependencyObject).GetNearestElement() == this)
                {
                    PointerCanceled(this, e.GetPointerEventArgs(this));
                }
            };

            base.PointerMoved += (o, e) =>
            {
                if ((e.OriginalSource as DependencyObject).GetNearestElement() == this)
                {
                    PointerMoved(this, e.GetPointerEventArgs(this));
                }
            };

            base.PointerPressed += (o, e) =>
            {
                if ((e.OriginalSource as DependencyObject).GetNearestElement() == this)
                {
                    PointerPressed(this, e.GetPointerEventArgs(this));
                }
            };

            base.PointerReleased += (o, e) =>
            {
                if ((e.OriginalSource as DependencyObject).GetNearestElement() == this)
                {
                    PointerReleased(this, e.GetPointerEventArgs(this));
                }
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
    }
}
