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
using Prism.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeSplitView"/>.
    /// </summary>
    [Register(typeof(INativeSplitView))]
    public class SplitView : global::Windows.UI.Xaml.Controls.SplitView, INativeSplitView
    {
        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Occurs when this instance has been detached from the visual tree.
        /// </summary>
        public new event EventHandler Unloaded;

        /// <summary>
        /// Gets the actual width of the detail pane.
        /// </summary>
        public double ActualDetailWidth { get; private set; }

        /// <summary>
        /// Gets the actual width of the master pane.
        /// </summary>
        public double ActualMasterWidth { get; private set; }

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
        /// Gets or sets the object that acts as the content for the detail pane.
        /// </summary>
        public object DetailContent
        {
            get { return Content; }
            set
            {
                var element = value as UIElement;
                if (element != Content)
                {
                    Content?.UnregisterPropertyChangedCallback(Control.BackgroundProperty, callbackToken2);

                    Content = element;
                    
                    var control = Content as Control;
                    if (control != null)
                    {
                        Background = control.Background;
                        callbackToken2 = control.RegisterPropertyChangedCallback(Control.BackgroundProperty, (o2, e2) => { Background = ((Control)o2).Background; });
                    }
                }
            }
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
        /// Gets or sets the object that acts as the content for the master pane.
        /// </summary>
        public object MasterContent
        {
            get { return Pane; }
            set
            {
                var element = value as UIElement;
                if (element != Pane)
                {
                    Pane?.UnregisterPropertyChangedCallback(Control.BackgroundProperty, callbackToken1);

                    Pane = element;

                    var control = Pane as Control;
                    if (control != null)
                    {
                        PaneBackground = control.Background;
                        callbackToken1 = control.RegisterPropertyChangedCallback(Control.BackgroundProperty, (o2, e2) => { PaneBackground = ((Control)o2).Background; });
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of the master pane.
        /// </summary>
        public double MaxMasterWidth
        {
            get { return maxMasterWidth; }
            set
            {
                if (value != maxMasterWidth)
                {
                    maxMasterWidth = value;
                    OnPropertyChanged(Prism.UI.SplitView.MaxMasterWidthProperty);
                    ResizePanes(new Size(ActualWidth, ActualHeight));
                }
            }
        }
        private double maxMasterWidth;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a measurement of itself and its children.
        /// </summary>
        public MeasureRequestHandler MeasureRequest { get; set; }

        /// <summary>
        /// Gets or sets the minimum width of the master pane.
        /// </summary>
        public double MinMasterWidth
        {
            get { return minMasterWidth; }
            set
            {
                if (value != minMasterWidth)
                {
                    minMasterWidth = value;
                    OnPropertyChanged(Prism.UI.SplitView.MinMasterWidthProperty);
                    ResizePanes(new Size(ActualWidth, ActualHeight));
                }
            }
        }
        private double minMasterWidth;

        /// <summary>
        /// Gets or sets the preferred width of the master pane as a percentage of the width of the split view.
        /// Valid values are between 0.0 and 1.0.
        /// </summary>
        public double PreferredMasterWidthRatio
        {
            get { return preferredMasterWidthRatio; }
            set
            {
                if (value != preferredMasterWidthRatio)
                {
                    preferredMasterWidthRatio = value;
                    OnPropertyChanged(Prism.UI.SplitView.PreferredMasterWidthRatioProperty);
                    ResizePanes(new Size(ActualWidth, ActualHeight));
                }
            }
        }
        private double preferredMasterWidthRatio;

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

        private long callbackToken1, callbackToken2;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitView"/> class.
        /// </summary>
        public SplitView()
        {
            maxMasterWidth = minMasterWidth = 480;
            preferredMasterWidthRatio = 0.3;

            DisplayMode = SplitViewDisplayMode.Inline;
            IsPaneOpen = true;
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            SizeChanged += (o, e) => ResizePanes(e.NewSize.GetSize());

            base.Loaded += (o, e) =>
            {
                if (!IsLoaded)
                {
                    IsLoaded = true;
                    OnPropertyChanged(Visual.IsLoadedProperty);
                    Loaded(this, EventArgs.Empty);
                }
            };

            base.Unloaded += (o, e) =>
            {
                if (IsLoaded)
                {
                    IsLoaded = false;
                    OnPropertyChanged(Visual.IsLoadedProperty);
                    Unloaded(this, EventArgs.Empty);
                }
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

        private void ResizePanes(Size totalSize)
        {
            OpenPaneLength = Math.Max(minMasterWidth, Math.Min(maxMasterWidth, totalSize.Width * preferredMasterWidthRatio));

            if (OpenPaneLength != ActualMasterWidth)
            {
                ActualMasterWidth = OpenPaneLength;
                OnPropertyChanged(Prism.UI.SplitView.ActualMasterWidthProperty);
            }

            double detailWidth = Math.Max(totalSize.Width - ((DisplayMode & SplitViewDisplayMode.Overlay) != 0 ? 0 : OpenPaneLength), 0);
            if (detailWidth != ActualDetailWidth)
            {
                ActualDetailWidth = detailWidth;
                OnPropertyChanged(Prism.UI.SplitView.ActualDetailWidthProperty);
            }
        }
    }
}
