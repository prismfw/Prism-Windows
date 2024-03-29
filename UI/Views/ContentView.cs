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
using System.Linq;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeContentView"/>.
    /// </summary>
    [Register(typeof(INativeContentView))]
    public class ContentView : Page, INativeContentView, IViewStackChild
    {
        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when the value of a property is changed.
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
                        contentCanvas.Transitions = panelTransitions;
                        contentCanvas.ChildrenTransitions = panelChildrenTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        panelTransitions = contentCanvas.Transitions;
                        panelChildrenTransitions = contentCanvas.ChildrenTransitions;

                        Transitions = null;
                        contentCanvas.Transitions = null;
                        contentCanvas.ChildrenTransitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, panelTransitions, panelChildrenTransitions;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the background for the view.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    base.Background = value.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.PageBackgroundChromeLowBrushId);
                    OnPropertyChanged(Prism.UI.ContentView.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the content to be displayed by the view.
        /// </summary>
        public new object Content
        {
            get { return contentCanvas.Children.FirstOrDefault(); }
            set
            {
                contentCanvas.Children.Clear();

                var element = value as UIElement;
                if (element != null)
                {
                    contentCanvas.Children.Add(element);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Rectangle"/> that represents
        /// the size and position of the object relative to its parent container.
        /// </summary>
        public new Rectangle Frame
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
        /// Gets or sets the action menu for the view.
        /// </summary>
        public INativeActionMenu Menu
        {
            get { return menu; }
            set
            {
                if (value != menu)
                {
                    var element = menu as UIElement;
                    if (element != null)
                    {
                        (this.GetParent<INativeViewStack>()?.Header as Panel)?.Children.Remove(element);
                    }

                    menu = value;
                    AttachMenu();
                    
                    OnPropertyChanged(Prism.UI.ContentView.MenuProperty);
                }
            }
        }
        private INativeActionMenu menu;

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
        /// Gets or sets the title of the view.
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;

                    var stack = this.GetParent<INativeViewStack>();
                    if (stack != null)
                    {
                        stack.Header.Title = title;
                    }

                    OnPropertyChanged(Prism.UI.ContentView.TitleProperty);
                }
            }
        }
        private string title;

        /// <summary>
        /// Gets or sets the view stack of which this instance is a child.
        /// </summary>
        public DependencyObject ViewStack { get; set; }

        private readonly Canvas contentCanvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentView"/> class.
        /// </summary>
        public ContentView()
        {
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);
            HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch;

            base.Loaded += (o, e) =>
            {
                if (!IsLoaded)
                {
                    IsLoaded = true;
                    OnPropertyChanged(Visual.IsLoadedProperty);
                    Loaded(this, EventArgs.Empty);
                }

                AttachMenu();
            };

            base.Unloaded += (o, e) =>
            {
                if (IsLoaded)
                {
                    IsLoaded = false;
                    OnPropertyChanged(Visual.IsLoadedProperty);
                    Unloaded(this, EventArgs.Empty);
                }

                var element = menu as UIElement;
                if (element != null)
                {
                    element.GetParent<Panel>()?.Children.Remove(element);
                }
            };

            contentCanvas = new Canvas()
            {
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch
            };

            base.Content = contentCanvas;
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

        private void AttachMenu()
        {
            var element = menu as UIElement;
            if (element != null)
            {
                element.GetParent<Panel>()?.Children.Remove(element);
                (this.GetParent<INativeViewStack>()?.Header as Panel)?.Children.Add(element);
            }
        }
    }
}
