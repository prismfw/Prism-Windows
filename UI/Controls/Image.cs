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
using Prism.Input;
using Prism.Native;
using Prism.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeImage"/>.
    /// </summary>
    [Register(typeof(INativeImage))]
    public class Image : ContentControl, INativeImage
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
                
                if (stretch != Stretch.None && stretch != Stretch.UniformToFill)
                {
                    Element.Width = value.Width;
                    Element.Height = value.Height;
                }
                else if (source != null && ((source as INativeBitmapImage)?.IsLoaded ?? true))
                {
                    if (stretch == Stretch.UniformToFill)
                    {
                        double width = source.PixelWidth / source.Scale;
                        double height = source.PixelHeight / source.Scale;
                        double scale = Math.Max(frame.Width / width, frame.Height / height);
                        Element.Width = width * scale;
                        Element.Height = height * scale;
                    }
                    else
                    {
                        Element.Width = source.PixelWidth / source.Scale;
                        Element.Height = source.PixelHeight / source.Scale;
                    }
                }

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
        /// Gets or sets the <see cref="INativeImageSource"/> object that contains the image data for the element.
        /// </summary>
        public INativeImageSource Source
        {
            get { return source; }
            set
            {
                if (value != source)
                {
                    source = value;
                    Element.Source = source.GetImageSource();
                    OnPropertyChanged(Prism.UI.Controls.Image.SourceProperty);
                }
            }
        }
        private INativeImageSource source;

        /// <summary>
        /// Gets or sets the manner in which the image will be stretched to fit its allocated space.
        /// </summary>
        public Stretch Stretch
        {
            get { return stretch; }
            set
            {
                if (value != stretch)
                {
                    stretch = value;
                    if (stretch != Stretch.None && stretch != Stretch.UniformToFill)
                    {
                        Element.Stretch = stretch.GetStretch();
                    }
                    else
                    {
                        Element.Stretch = global::Windows.UI.Xaml.Media.Stretch.Uniform;
                        if (source != null && ((source as INativeBitmapImage)?.IsLoaded ?? true))
                        {
                            if (stretch == Stretch.UniformToFill)
                            {
                                double width = source.PixelWidth / source.Scale;
                                double height = source.PixelHeight / source.Scale;
                                double scale = Math.Max(frame.Width / width, frame.Height / height);
                                Element.Width = width * scale;
                                Element.Height = height * scale;
                            }
                            else
                            {
                                Element.Width = source.PixelWidth / source.Scale;
                                Element.Height = source.PixelHeight / source.Scale;
                            }
                        }
                    }

                    OnPropertyChanged(Prism.UI.Controls.Image.StretchProperty);
                }
            }
        }
        private Stretch stretch;

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
        /// Gets the UI element that is displaying the image.
        /// </summary>
        protected global::Windows.UI.Xaml.Controls.Image Element { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        public Image()
        {
            Element = new global::Windows.UI.Xaml.Controls.Image()
            {
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center
            };
            Content = Element;
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);
            HorizontalContentAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            VerticalContentAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;

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
            if (source == null || !((source as INativeBitmapImage)?.IsLoaded ?? true))
            {
                return Size.Empty;
            }

            double scale = 1;
            double width = source.PixelWidth / source.Scale;
            double height = source.PixelHeight / source.Scale;
            if (constraints.Width < width)
            {
                scale = constraints.Width / width;
            }
            if (constraints.Height < height)
            {
                scale = Math.Min(scale, constraints.Height / height);
            }

            return new Size(width * scale, height * scale);
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
