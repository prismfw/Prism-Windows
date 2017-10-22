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
using Prism.Native;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Prism.Windows.Input
{
    /// <summary>
    /// Represents the base class for Windows gesture recognizer implementations.
    /// </summary>
    public abstract class GestureRecognizerBase : INativeGestureRecognizer
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Gets the underlying Windows gesture recognizer.
        /// </summary>
        protected GestureRecognizer Recognizer { get; }

        /// <summary>
        /// Gets the target element of the gesture recognizer.
        /// </summary>
        protected UIElement Target { get; private set; }

        private readonly PointerEventHandler pointerCanceledHandler;
        private readonly PointerEventHandler pointerMovedHandler;
        private readonly PointerEventHandler pointerPressedHandler;
        private readonly PointerEventHandler pointerReleasedHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureRecognizerBase"/> class.
        /// </summary>
        protected GestureRecognizerBase()
        {
            Recognizer = new GestureRecognizer();

            pointerCanceledHandler = new PointerEventHandler(OnElementPointerCanceled);
            pointerMovedHandler = new PointerEventHandler(OnElementPointerMoved);
            pointerPressedHandler = new PointerEventHandler(OnElementPointerPressed);
            pointerReleasedHandler = new PointerEventHandler(OnElementPointerReleased);
        }

        /// <summary>
        /// Removes the specified object as the target of the gesture recognizer.
        /// </summary>
        /// <param name="target">The object to clear as the target.</param>
        public void ClearTarget(object target)
        {
            var element = target as UIElement;
            if (element != null)
            {
                element.RemoveHandler(UIElement.PointerCanceledEvent, pointerCanceledHandler);
                element.RemoveHandler(UIElement.PointerMovedEvent, pointerMovedHandler);
                element.RemoveHandler(UIElement.PointerPressedEvent, pointerPressedHandler);
                element.RemoveHandler(UIElement.PointerReleasedEvent, pointerReleasedHandler);

                Target = null;
            }
        }

        /// <summary>
        /// Sets the specified object as the target of the gesture recognizer.
        /// </summary>
        /// <param name="target">The object to set as the target.</param>
        public void SetTarget(object target)
        {
            var element = target as UIElement;
            if (element != null)
            {
                Target = element;

                element.RemoveHandler(UIElement.PointerCanceledEvent, pointerCanceledHandler);
                element.AddHandler(UIElement.PointerCanceledEvent, pointerCanceledHandler, true);

                element.RemoveHandler(UIElement.PointerMovedEvent, pointerMovedHandler);
                element.AddHandler(UIElement.PointerMovedEvent, pointerMovedHandler, true);

                element.RemoveHandler(UIElement.PointerPressedEvent, pointerPressedHandler);
                element.AddHandler(UIElement.PointerPressedEvent, pointerPressedHandler, true);

                element.RemoveHandler(UIElement.PointerReleasedEvent, pointerReleasedHandler);
                element.AddHandler(UIElement.PointerReleasedEvent, pointerReleasedHandler, true);
            }
        }

        /// <summary>
        /// Transforms the specified point from screen coordinates to the target element's local space.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        protected global::Windows.Foundation.Point TransformToTarget(global::Windows.Foundation.Point point)
        {
            return Target == null ? point : Window.Current.Content.TransformToVisual(Target).TransformPoint(point);
        }

        /// <summary>
        /// Called when a property value is changed.
        /// </summary>
        /// <param name="pd">A property descriptor describing the property whose value has been changed.</param>
        protected virtual void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }

        private void OnElementPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            Recognizer.CompleteGesture();

            var element = sender as UIElement;
            if (element != null)
            {
                element.ReleasePointerCapture(e.Pointer);
            }
            else
            {
                Prism.Utilities.Logger.Warn("Unable to release pointer!");
            }
        }

        private void OnElementPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var reference = (sender as DependencyObject)?.GetParent<UIElement>();
            if (reference != null)
            {
                Recognizer.ProcessMoveEvents(e.GetIntermediatePoints(reference));
            }
            else
            {
                Prism.Utilities.Logger.Warn("No reference for move gesture!");
            }
        }

        private void OnElementPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.CapturePointer(e.Pointer);
                Recognizer.ProcessDownEvent(e.GetCurrentPoint(element.GetParent<UIElement>()));
            }
            else
            {
                Prism.Utilities.Logger.Warn("No reference for press gesture!");
            }
        }

        private void OnElementPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                Recognizer.ProcessUpEvent(e.GetCurrentPoint(element.GetParent<UIElement>()));
                element.ReleasePointerCapture(e.Pointer);
            }
            else
            {
                Prism.Utilities.Logger.Warn("No reference for release gesture!");
            }
        }
    }
}
