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
using System.Collections.Generic;
using System.Linq;
using Prism.Input;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media.Inking;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeInkCanvas"/>.
    /// </summary>
    [Register(typeof(INativeInkCanvas))]
    public class InkCanvas : global::Windows.UI.Xaml.Controls.InkCanvas, INativeInkCanvas
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
                    }
                    else
                    {
                        transitions = Transitions;
                        Transitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions;

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
                Canvas.SetLeft(this, value.X);
                Canvas.SetTop(this, value.Y);
            }
        }
        private Rectangle frame = new Rectangle();

        /// <summary>
        /// Gets or sets how the ink canvas handles input.
        /// </summary>
        public InkInputMode InputMode
        {
            get { return InkPresenter.InputProcessingConfiguration.Mode == InkInputProcessingMode.Erasing ? InkInputMode.Erasing : InkInputMode.Inking; }
            set
            {
                if (value != InputMode)
                {
                    InkPresenter.InputProcessingConfiguration.Mode = value == InkInputMode.Erasing ? InkInputProcessingMode.Erasing : InkInputProcessingMode.Inking;
                    OnPropertyChanged(Prism.UI.Controls.InkCanvas.InputModeProperty);
                }
            }
        }

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
        /// Gets the ink strokes that are on the canvas.
        /// </summary>
        public IEnumerable<INativeInkStroke> Strokes
        {
            get { return InkPresenter.StrokeContainer.GetStrokes().Select(s => new Media.Inking.InkStroke(s)); }
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

                    OnPropertyChanged(Prism.UI.Controls.Element.VisibilityProperty);
                }
            }
        }
        private Visibility visibility;

        /// <summary>
        /// Initializes a new instance of the <see cref="InkCanvas"/> class.
        /// </summary>
        public InkCanvas()
        {
            InkPresenter.InputDeviceTypes = (global::Windows.UI.Core.CoreInputDeviceTypes)7;

            base.Loaded += (o, e) =>
            {
                IsLoaded = true;
                OnPropertyChanged(Visual.IsLoadedProperty);
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
                OnPropertyChanged(Visual.IsLoadedProperty);
                Unloaded(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// Adds the specified ink stroke to the canvas.
        /// </summary>
        /// <param name="stroke">The ink stroke to add.</param>
        public void AddStroke(INativeInkStroke stroke)
        {
            InkPresenter.StrokeContainer.AddStroke((global::Windows.UI.Input.Inking.InkStroke)(stroke as Media.Inking.InkStroke));
        }

        /// <summary>
        /// Adds the specified ink strokes to the canvas.
        /// </summary>
        /// <param name="strokes">The ink strokes to add.</param>
        public void AddStrokes(IEnumerable<INativeInkStroke> strokes)
        {
            InkPresenter.StrokeContainer.AddStrokes(strokes.OfType<Media.Inking.InkStroke>().Select(s => (global::Windows.UI.Input.Inking.InkStroke)s));
        }

        /// <summary>
        /// Removes all ink strokes from the canvas.
        /// </summary>
        public void ClearStrokes()
        {
            InkPresenter.StrokeContainer.Clear();
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
        /// Updates the drawing attributes to apply to new ink strokes on the canvas.
        /// </summary>
        /// <param name="attributes">The drawing attributes to apply to new ink strokes.</param>
        public void UpdateDrawingAttributes(Prism.UI.Media.Inking.InkDrawingAttributes attributes)
        {
            InkPresenter.UpdateDefaultDrawingAttributes(attributes.GetInkDrawingAttributes());
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
