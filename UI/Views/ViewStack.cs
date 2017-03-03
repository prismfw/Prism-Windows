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
using Prism.Native;
using Prism.UI;
using Prism.Windows.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeViewStack"/>.
    /// </summary>
    [Register(typeof(INativeViewStack))]
    public class ViewStack : StackPanel, INativeViewStack
    {
        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when a view is being popped off of the view stack.
        /// </summary>
        public event EventHandler<NativeViewStackPoppingEventArgs> Popping;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Occurs when this instance has been detached from the visual tree.
        /// </summary>
        public new event EventHandler Unloaded;

        /// <summary>
        /// Occurs when the current view of the view stack has changed.
        /// </summary>
        public event EventHandler ViewChanged;

        /// <summary>
        /// Occurs when the current view of the view stack is being replaced by a different view.
        /// </summary>
        public event EventHandler<NativeViewStackViewChangingEventArgs> ViewChanging;

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
                        contentControl.Transitions = containerTransitions;
                        contentControl.ContentTransitions = containerContentTransitions;

                        var header = Header as global::Windows.UI.Xaml.Controls.Panel;
                        if (header != null)
                        {
                            header.Transitions = headerTransitions;
                            header.ChildrenTransitions = headerChildrenTransitions;
                        }
                    }
                    else
                    {
                        transitions = Transitions;
                        childrenTransitions = ChildrenTransitions;
                        containerTransitions = contentControl.Transitions;
                        containerContentTransitions = contentControl.ContentTransitions;

                        var header = Header as global::Windows.UI.Xaml.Controls.Panel;
                        if (header != null)
                        {
                            headerTransitions = header.Transitions;
                            headerChildrenTransitions = header.ChildrenTransitions;

                            header.Transitions = null;
                            header.ChildrenTransitions = null;
                        }

                        Transitions = null;
                        ChildrenTransitions = null;
                        contentControl.Transitions = null;
                        contentControl.ContentTransitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, childrenTransitions, containerTransitions, containerContentTransitions, headerTransitions, headerChildrenTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets the view that is currently on top of the stack.
        /// </summary>
        public object CurrentView
        {
            get { return views.LastOrDefault(); }
        }

        /// <summary>
        /// Gets or sets a <see cref="Rectangle"/> that represents
        /// the size and position of the object relative to its parent container.
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
        /// Gets the header for the view stack.
        /// </summary>
        public INativeViewStackHeader Header { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the back button is enabled.
        /// </summary>
        public bool IsBackButtonEnabled
        {
            get { return ((UIElement)Header).GetChild<FrameworkElement>(e => e.Name == "BackButton")?.Visibility == global::Windows.UI.Xaml.Visibility.Visible; }
            set
            {
                var backButton = ((UIElement)Header).GetChild<FrameworkElement>(e => e.Name == "BackButton");
                if (backButton != null)
                {
                    backButton.Visibility = value ? global::Windows.UI.Xaml.Visibility.Visible : global::Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the header is hidden.
        /// </summary>
        public bool IsHeaderHidden
        {
            get { return ((UIElement)Header).Visibility == global::Windows.UI.Xaml.Visibility.Collapsed; }
            set
            {
                if (value != IsHeaderHidden)
                {
                    ((UIElement)Header).Visibility = value ? global::Windows.UI.Xaml.Visibility.Collapsed : global::Windows.UI.Xaml.Visibility.Visible;
                    OnPropertyChanged(Prism.UI.ViewStack.IsHeaderHiddenProperty);
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
                    OnPropertyChanged(Prism.UI.Visual.IsHitTestVisibleProperty);
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
        /// Gets a collection of the views that are currently a part of the stack.
        /// </summary>
        public IEnumerable<object> Views
        {
            get { return views.AsReadOnly(); }
        }

        private readonly List<object> views;
        private readonly ContentControl contentControl;
        private long callbackToken;
        private bool isPopping;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewStack"/> class.
        /// </summary>
        public ViewStack()
        {
            views = new List<object>();

            contentControl = new ViewStackContentControl()
            {
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch
            };

            Children.Add((Header = new ViewStackHeader()) as UIElement);
            Children.Add(contentControl);
            Orientation = global::Windows.UI.Xaml.Controls.Orientation.Vertical;
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

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
        /// Inserts the specified view into the stack at the specified index.
        /// </summary>
        /// <param name="view">The view to be inserted.</param>
        /// <param name="index">The zero-based index of the location in the stack in which to insert the view.</param>
        /// <param name="animate">Whether to use any system-defined transition animation.</param>
        public void InsertView(object view, int index, Animate animate)
        {
            views.Insert(index, view);

            var vsc = view as IViewStackChild;
            if (vsc != null)
            {
                vsc.ViewStack = this;
            }

            if (views.Last() == view)
            {
                ViewChanging(this, new NativeViewStackViewChangingEventArgs(contentControl.Content, view));
                contentControl.Content = view;
            }
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
        /// Removes the top view from the stack.
        /// </summary>
        /// <param name="animate">Whether to use any system-defined transition animation.</param>
        /// <returns>The view that was removed from the stack.</returns>
        public object PopView(Animate animate)
        {
            if (views.Count < 2)
            {
                return null;
            }

            var last = views.Last();
            if (!isPopping)
            {
                isPopping = true;
                var args = new NativeViewStackPoppingEventArgs(last);
                Popping(this, args);
                isPopping = false;
                if (args.Cancel)
                {
                    return null;
                }
            }
            
            views.RemoveAt(views.Count - 1);

            var vsc = last as IViewStackChild;
            if (vsc != null)
            {
                vsc.ViewStack = null;
            }

            ViewChanging(this, new NativeViewStackViewChangingEventArgs(contentControl.Content, views.Last()));
            contentControl.Content = views.Last();
            return last;
        }

        /// <summary>
        /// Removes every view from the stack except for the root view.
        /// </summary>
        /// <param name="animate">Whether to use any system-defined transition animation.</param>
        /// <returns>An <see cref="Array"/> containing the views that were removed from the stack.</returns>
        public object[] PopToRoot(Animate animate)
        {
            if (views.Count < 2)
            {
                return null;
            }

            var last = views.Last();
            if (!isPopping)
            {
                isPopping = true;
                var args = new NativeViewStackPoppingEventArgs(last);
                Popping(this, args);
                isPopping = false;
                if (args.Cancel)
                {
                    return null;
                }
            }

            var popped = views.Skip(1);
            views.RemoveRange(1, views.Count - 1);

            foreach (var vsc in popped.OfType<IViewStackChild>())
            {
                vsc.ViewStack = null;
            }

            ViewChanging(this, new NativeViewStackViewChangingEventArgs(contentControl.Content, views.Last()));
            contentControl.Content = views.Last();
            return popped.ToArray();
        }

        /// <summary>
        /// Removes from the stack every view on top of the specified view.
        /// </summary>
        /// <param name="view">The view to pop to.</param>
        /// <param name="animate">Whether to use any system-defined transition animation.</param>
        /// <returns>An <see cref="Array"/> containing the views that were removed from the stack.</returns>
        public object[] PopToView(object view, Animate animate)
        {
            if (contentControl.Content == view)
            {
                return null;
            }

            var last = views.Last();
            if (!isPopping)
            {
                isPopping = true;
                var args = new NativeViewStackPoppingEventArgs(last);
                Popping(this, args);
                isPopping = false;
                if (args.Cancel)
                {
                    return null;
                }
            }

            int index = views.IndexOf(view);

            var popped = views.Skip(++index);
            views.RemoveRange(index, views.Count - index);

            foreach (var vsc in popped.OfType<IViewStackChild>())
            {
                vsc.ViewStack = null;
            }

            ViewChanging(this, new NativeViewStackViewChangingEventArgs(contentControl.Content, view));
            contentControl.Content = view;
            return popped.ToArray();
        }

        /// <summary>
        /// Pushes the specified view onto the top of the stack.
        /// </summary>
        /// <param name="view">The view to push to the top of the stack.</param>
        /// <param name="animate">Whether to use any system-defined transition animation.</param>
        public void PushView(object view, Animate animate)
        {
            views.Add(view);

            var vsc = view as IViewStackChild;
            if (vsc != null)
            {
                vsc.ViewStack = this;
            }

            ViewChanging(this, new NativeViewStackViewChangingEventArgs(contentControl.Content, view));
            contentControl.Content = view;
        }

        /// <summary>
        /// Replaces a view that is currently on the stack with the specified view.
        /// </summary>
        /// <param name="oldView">The view to be replaced.</param>
        /// <param name="newView">The view with which to replace the old view.</param>
        /// <param name="animate">Whether to use any system-defined transition animation.</param>
        public void ReplaceView(object oldView, object newView, Animate animate)
        {
            int index = views.IndexOf(oldView);
            views[index] = newView;

            var vsc = oldView as IViewStackChild;
            if (vsc != null)
            {
                vsc.ViewStack = null;
            }

            vsc = newView as IViewStackChild;
            if (vsc != null)
            {
                vsc.ViewStack = this;
            }

            if (index == views.Count - 1)
            {
                ViewChanging(this, new NativeViewStackViewChangingEventArgs(oldView, newView));
                contentControl.Content = newView;
            }
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

        /// <summary>
        /// Called when the current view is changed.
        /// </summary>
        /// <param name="oldView">The old view.</param>
        /// <param name="newView">The new view.</param>
        protected virtual void OnViewChanged(object oldView, object newView)
        {
            if (oldView != null || newView != null)
            {
                (oldView as DependencyObject)?.UnregisterPropertyChangedCallback(Control.BackgroundProperty, callbackToken);

                var control = newView as Control;
                if (control != null)
                {
                    Background = control.Background;
                    callbackToken = control.RegisterPropertyChangedCallback(Control.BackgroundProperty, (o, e) => { Background = ((Control)o).Background; });
                }

                var contentView = newView as INativeContentView;
                if (contentView != null)
                {
                    Header.Title = contentView.Title;
                }

                ViewChanged(this, EventArgs.Empty);
            }
        }

        private class ViewStackContentControl : ContentControl
        {
            public ViewStackContentControl()
            {
                Loaded += (o, e) => { (Parent as ViewStack)?.OnViewChanged(null, Content); };
            }
            
            protected override void OnContentChanged(object oldContent, object newContent)
            {
                base.OnContentChanged(oldContent, newContent);
                (Parent as ViewStack)?.OnViewChanged(oldContent, newContent);
            }
        }
    }
}
