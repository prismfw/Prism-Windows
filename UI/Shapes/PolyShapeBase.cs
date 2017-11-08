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
using System.Collections.Generic;
using Prism.Input;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Shapes
{
    /// <summary>
    /// Represents the base class for the <see cref="Polygon"/> and <see cref="Polyline"/> classes.
    /// This class is abstract.
    /// </summary>
    public abstract class PolyShapeBase : ContentControl, INativePolyShape
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
                        ContentTransitions = contentTransitions;
                        Element.Transitions = elementTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        contentTransitions = ContentTransitions;
                        elementTransitions = Element.Transitions;

                        Transitions = null;
                        ContentTransitions = null;
                        Element.Transitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, contentTransitions, elementTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the interior of the shape.
        /// </summary>
        public Brush Fill
        {
            get { return fill; }
            set
            {
                if (value != fill)
                {
                    fill = value;
                    Element.Fill = fill.GetBrush();
                    OnPropertyChanged(Prism.UI.Shapes.Shape.FillProperty);
                }
            }
        }
        private Brush fill;

        /// <summary>
        /// Gets or sets the rule to use for determining the interior fill of the shape.
        /// </summary>
        public abstract FillRule FillRule { get; set; }

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
                Clip = new global::Windows.UI.Xaml.Media.RectangleGeometry()
                {
                    Rect = new global::Windows.Foundation.Rect(0, 0, Width, Height)
                };

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
                    OnPropertyChanged(Prism.UI.Element.OpacityProperty);
                }
            }
        }

        /// <summary>
        /// Gets a collection of the points that describe the vertices of the shape.
        /// </summary>
        public IList<Point> Points
        {
            get { return points; }
        }
        private PointList points;

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
        /// Gets or sets the <see cref="Brush"/> to apply to the outline of the shape.
        /// </summary>
        public Brush Stroke
        {
            get { return stroke; }
            set
            {
                if (value != stroke)
                {
                    stroke = value;
                    Element.Stroke = stroke.GetBrush();
                    OnPropertyChanged(Prism.UI.Shapes.Shape.StrokeProperty);
                }
            }
        }
        private Brush stroke;

        /// <summary>
        /// Gets or sets the manner in which the ends of a line are drawn.
        /// </summary>
        public LineCap StrokeLineCap
        {
            get { return (LineCap)Element.StrokeDashCap; }
            set
            {
                if (value != (LineCap)Element.StrokeDashCap)
                {
                    Element.StrokeDashCap = Element.StrokeEndLineCap = Element.StrokeStartLineCap = (global::Windows.UI.Xaml.Media.PenLineCap)value;
                    OnPropertyChanged(Prism.UI.Shapes.Shape.StrokeLineCapProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the manner in which the connections between different lines are drawn.
        /// </summary>
        public LineJoin StrokeLineJoin
        {
            get { return (LineJoin)Element.StrokeLineJoin; }
            set
            {
                if (value != (LineJoin)Element.StrokeLineJoin)
                {
                    Element.StrokeLineJoin = (global::Windows.UI.Xaml.Media.PenLineJoin)value;
                    OnPropertyChanged(Prism.UI.Shapes.Shape.StrokeLineJoinProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the miter limit for connecting lines.
        /// </summary>
        public double StrokeMiterLimit
        {
            get { return Element.StrokeMiterLimit; }
            set
            {
                if (value != Element.StrokeMiterLimit)
                {
                    Element.StrokeMiterLimit = value;
                    OnPropertyChanged(Prism.UI.Shapes.Shape.StrokeMiterLimitProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the shape's outline.
        /// </summary>
        public double StrokeThickness
        {
            get { return Element.StrokeThickness; }
            set
            {
                if (value != Element.StrokeThickness)
                {
                    Element.StrokeThickness = value;
                    SetDashArray();
                    OnPropertyChanged(Prism.UI.Shapes.Shape.StrokeThicknessProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the display state of the element.
        /// </summary>
        public new Visibility Visibility
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
        private Visibility visibility;

        /// <summary>
        /// Gets the UI element that is displaying the shape.
        /// </summary>
        protected global::Windows.UI.Xaml.Shapes.Shape Element { get; private set; }

        private double[] strokeDashArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyShapeBase"/> class.
        /// </summary>
        public PolyShapeBase(global::Windows.UI.Xaml.Shapes.Shape shape)
        {
            points = new PointList(new global::Windows.UI.Xaml.Media.PointCollection());

            var polygon = shape as global::Windows.UI.Xaml.Shapes.Polygon;
            if (polygon != null)
            {
                polygon.Points = points.GetCollection();
                Content = Element = polygon;
            }
            else
            {
                var polyline = shape as global::Windows.UI.Xaml.Shapes.Polyline;
                if (polyline != null)
                {
                    polyline.Points = points.GetCollection();
                    Content = Element = polyline;
                }
                else
                {
                    throw new ArgumentException("Shape must be a Polygon or Polyline.", nameof(shape));
                }
            }

            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            base.Loaded += (o, e) =>
            {
                IsLoaded = true;
                OnPropertyChanged(Visual.IsLoadedProperty);
                Loaded(this, EventArgs.Empty);
            };

            base.PointerCanceled += (o, e) => PointerCanceled(this, e.GetPointerEventArgs(this));
            base.PointerMoved += (o, e) => PointerMoved(this, e.GetPointerEventArgs(this));
            base.PointerPressed += (o, e) => PointerPressed(this, e.GetPointerEventArgs(this));
            base.PointerReleased += (o, e) => PointerReleased(this, e.GetPointerEventArgs(this));

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
        /// Sets the dash pattern to be used when drawing the outline of the shape.
        /// </summary>
        /// <param name="pattern">An array of values that defines the dash pattern.  Each value represents the length of a dash, alternating between "on" and "off".</param>
        /// <param name="offset">The distance within the dash pattern where dashes begin.</param>
        public void SetStrokeDashPattern(double[] pattern, double offset)
        {
            strokeDashArray = pattern;
            Element.StrokeDashOffset = offset;

            SetDashArray();
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.</summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override global::Windows.Foundation.Size ArrangeOverride(global::Windows.Foundation.Size finalSize)
        {
            ArrangeRequest(false, null);
            finalSize = Frame.Size.GetSize();
            Element.Arrange(new global::Windows.Foundation.Rect(0, 0, finalSize.Width, finalSize.Height));
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

        private void SetDashArray()
        {
            if (strokeDashArray == null)
            {
                Element.StrokeDashArray = null;
            }
            else
            {
                Element.StrokeDashArray = new global::Windows.UI.Xaml.Media.DoubleCollection();
                for (int i = 0; i < strokeDashArray.Length; i++)
                {
                    Element.StrokeDashArray.Add(strokeDashArray[i] / Element.StrokeThickness);
                }
            }
        }

        private class PointList : IList<Point>
        {
            public int Count
            {
                get { return collection.Count; }
            }

            public bool IsFixedSize
            {
                get { return false; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool IsSynchronized
            {
                get { return false; }
            }

            public object SyncRoot
            {
                get { return null; }
            }

            public Point this[int index]
            {
                get
                {
                    var point = collection[index];
                    return new Point(point.X, point.Y);
                }
                set
                {
                    collection[index] = new global::Windows.Foundation.Point(value.X, value.Y);
                }
            }

            private readonly global::Windows.UI.Xaml.Media.PointCollection collection;

            public PointList(global::Windows.UI.Xaml.Media.PointCollection collection)
            {
                this.collection = collection;
            }

            public void Add(Point value)
            {
                collection.Add(new global::Windows.Foundation.Point(value.X, value.Y));
            }

            public void Clear()
            {
                collection.Clear();
            }

            public bool Contains(Point value)
            {
                return collection.Contains(new global::Windows.Foundation.Point(value.X, value.Y));
            }

            public int IndexOf(Point value)
            {
                return collection.IndexOf(new global::Windows.Foundation.Point(value.X, value.Y));
            }

            public void Insert(int index, Point value)
            {
                collection.Insert(index, new global::Windows.Foundation.Point(value.X, value.Y));
            }

            public bool Remove(Point value)
            {
                return collection.Remove(new global::Windows.Foundation.Point(value.X, value.Y));
            }

            public void RemoveAt(int index)
            {
                collection.RemoveAt(index);
            }

            public void CopyTo(Point[] array, int index)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    var point = collection[i];
                    array[i + index] = new Point(point.X, point.Y);
                }
            }

            public global::Windows.UI.Xaml.Media.PointCollection GetCollection()
            {
                return collection;
            }

            public IEnumerator<Point> GetEnumerator()
            {
                return new PointEnumerator(((IEnumerable<global::Windows.Foundation.Point>)collection).GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new PointEnumerator(((IEnumerable<global::Windows.Foundation.Point>)collection).GetEnumerator());
            }

            private class PointEnumerator : IEnumerator<Point>, IEnumerator
            {
                public Point Current
                {
                    get
                    {
                        var point = (global::Windows.Foundation.Point)elementEnumerator.Current;
                        return new Point(point.X, point.Y);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        var point =  (global::Windows.Foundation.Point)elementEnumerator.Current;
                        return new Point(point.X, point.Y);
                    }
                }

                private readonly IEnumerator elementEnumerator;

                public PointEnumerator(IEnumerator elementEnumerator)
                {
                    this.elementEnumerator = elementEnumerator;
                }

                public void Dispose()
                {
                    var disposable = elementEnumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                public bool MoveNext()
                {
                    do
                    {
                        if (!elementEnumerator.MoveNext())
                        {
                            return false;
                        }
                    }
                    while (!(elementEnumerator.Current is INativeElement));

                    return true;
                }

                public void Reset()
                {
                    elementEnumerator.Reset();
                }
            }
        }
    }
}
