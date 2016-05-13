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
using System.Collections.Generic;
using System.Linq;
using Prism.Input;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using UIElement = Windows.UI.Xaml.UIElement;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativePanel"/>.
    /// </summary>
    [Register(typeof(INativePanel))]
    public class Panel : Canvas, INativePanel
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
        /// Gets or sets the background for the panel.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    base.Background = background.GetBrush();
                    OnPropertyChanged(Prism.UI.Controls.Panel.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets a list of the UI elements that reside within the panel.
        /// </summary>
        public new IList Children
        {
            get { return children; }
        }
        private readonly PanelChildrenList children;

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

                    OnPropertyChanged(Prism.UI.Controls.Element.VisibilityProperty);
                }
            }
        }
        private Visibility visibility;

        /// <summary>
        /// Initializes a new instance of the <see cref="Panel"/> class.
        /// </summary>
        public Panel()
        {
            children = new PanelChildrenList(this);

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

        private class PanelChildrenList : IList
        {
            public int Count
            {
                get { return parent.Children.Count(c => c is INativeElement); }
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

            public object this[int index]
            {
                get { return parent.Children.OfType<INativeElement>().ElementAt(index); }
                set
                {
                    index = parent.Children.IndexOf(parent.Children.Where(c => c is INativeElement).ElementAt(index));
                    var child = value as UIElement;
                    if (child == null)
                    {
                        throw new ArgumentException("Value must be an object of type UIElement.", "value");
                    }

                    parent.Children[index] = child;
                }
            }

            private readonly global::Windows.UI.Xaml.Controls.Panel parent;

            public PanelChildrenList(global::Windows.UI.Xaml.Controls.Panel parent)
            {
                this.parent = parent;
            }

            public int Add(object value)
            {
                int count = parent.Children.Count;
                var child = value as UIElement;
                if (child == null)
                {
                    throw new ArgumentException("Value must be an object of type UIElement.", "value");
                }

                parent.Children.Add(child);
                return parent.Children.Count - count;
            }

            public void Clear()
            {
                for (int i = parent.Children.Count - 1; i >= 0; i--)
                {
                    if (parent.Children[i] is INativeElement)
                    {
                        parent.Children.RemoveAt(i);
                    }
                }
            }

            public bool Contains(object value)
            {
                var child = value as UIElement;
                return child != null && parent.Children.Contains(child);
            }

            public int IndexOf(object value)
            {
                var children = parent.Children.Where(c => c is INativeElement).ToList();
                return children.IndexOf(value as UIElement);
            }

            public void Insert(int index, object value)
            {
                var child = value as UIElement;
                if (child == null)
                {
                    throw new ArgumentException("Value must be an object of type UIElement.", "value");
                }

                index = parent.Children.IndexOf(parent.Children.Where(c => c is INativeElement).ElementAt(index));
                parent.Children.Insert(index, child);
            }

            public void Remove(object value)
            {
                var child = value as UIElement;
                if (child != null)
                {
                    parent.Children.Remove(child);
                }
            }

            public void RemoveAt(int index)
            {
                parent.Children.Remove(parent.Children.Where(c => c is INativeElement).ElementAt(index));
            }

            public void CopyTo(Array array, int index)
            {
                parent.Children.OfType<INativeElement>().ToArray().CopyTo(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return new PanelChildrenEnumerator(parent.Children.OfType<INativeElement>().GetEnumerator());
            }

            private class PanelChildrenEnumerator : IEnumerator<INativeElement>, IEnumerator
            {
                public INativeElement Current
                {
                    get { return elementEnumerator.Current as INativeElement; }
                }

                object IEnumerator.Current
                {
                    get { return elementEnumerator.Current; }
                }

                private readonly IEnumerator elementEnumerator;

                public PanelChildrenEnumerator(IEnumerator elementEnumerator)
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
