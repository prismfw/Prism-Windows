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
    /// Represents a Windows implementation for an <see cref="INativeLabel"/>.
    /// </summary>
    [Register(typeof(INativeLabel))]
    public class Label : ContentControl, INativeLabel
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
        /// Gets or sets the font to use for displaying the text in the label.
        /// </summary>
        public new object FontFamily
        {
            get { return Element.FontFamily; }
            set
            {
                var fontFamily = value as Media.FontFamily;
                if (fontFamily != Element.FontFamily)
                {
                    Element.SetFont(fontFamily, FontStyle);
                    OnPropertyChanged(Prism.UI.Controls.Label.FontFamilyProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the text in the label.
        /// </summary>
        public new double FontSize
        {
            get { return Element.FontSize; }
            set
            {
                if (value != Element.FontSize)
                {
                    Element.FontSize = value;
                    OnPropertyChanged(Prism.UI.Controls.Label.FontSizeProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style with which to render the text in the label.
        /// </summary>
        public new FontStyle FontStyle
        {
            get { return Element.GetFontStyle(); }
            set
            {
                var style = Element.FontStyle;
                var weight = Element.FontWeight.Weight;
                Element.SetFont(Element.FontFamily as Media.FontFamily, value);

                if (Element.FontStyle != style || Element.FontWeight.Weight != weight)
                {
                    OnPropertyChanged(Prism.UI.Controls.Label.FontStyleProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the text contents of the label.
        /// </summary>
        public new Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;

                    var lvi = this.GetParent<ListViewItem>();
                    if (lvi == null || !lvi.IsSelected || highlightBrush == null)
                    {
                        Element.Foreground = foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.PageTextBaseHighBrushId);
                    }

                    OnPropertyChanged(Prism.UI.Controls.Label.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets or sets a <see cref="Rectangle"/> that represents the size and position of the object relative to its parent container.
        /// </summary>
        public Rectangle Frame
        {
            get { return frame; }
            set
            {
                frame = value;

                Element.Width = Width = value.Width;
                Element.Height = Height = value.Height;
                Canvas.SetLeft(this, value.X);
                Canvas.SetTop(this, value.Y);
            }
        }
        private Rectangle frame = new Rectangle();

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the text contents when the label resides within a highlighted element.
        /// </summary>
        public Brush HighlightBrush
        {
            get { return highlightBrush; }
            set
            {
                if (value != highlightBrush)
                {
                    highlightBrush = value;

                    var lvi = this.GetParent<ListViewItem>();
                    if (lvi != null && lvi.IsSelected)
                    {
                        Element.Foreground = highlightBrush.GetBrush() ?? foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.PageTextBaseHighBrushId);
                    }

                    OnPropertyChanged(Prism.UI.Controls.Label.HighlightBrushProperty);
                }
            }
        }
        private Brush highlightBrush;

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
        /// Gets or sets the maximum number of lines of text that the label can show.
        /// A value of 0 means there is no limit.
        /// </summary>
        public int Lines
        {
            get { return Element.MaxLines; }
            set
            {
                if (value != Element.MaxLines)
                {
                    Element.MaxLines = value;
                    OnPropertyChanged(Prism.UI.Controls.Label.LinesProperty);
                }
            }
        }

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
        /// Gets or sets the visual theme that should be used by this instance.
        /// </summary>
        public new Theme RequestedTheme
        {
            get { return base.RequestedTheme.GetTheme(); }
            set { base.RequestedTheme = value.GetElementTheme(); }
        }

        /// <summary>
        /// Gets or sets the text of the label.
        /// </summary>
        public string Text
        {
            get { return Element.Text; }
            set
            {
                value = value ?? string.Empty;
                if (value != Element.Text)
                {
                    Element.Text = value;
                    OnPropertyChanged(Prism.UI.Controls.Label.TextProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text within the label.
        /// </summary>
        public Prism.UI.TextAlignment TextAlignment
        {
            get { return Element.TextAlignment.GetTextAlignment(); }
            set
            {
                if (value != Element.TextAlignment.GetTextAlignment())
                {
                    Element.TextAlignment = value.GetTextAlignment();
                    OnPropertyChanged(Prism.UI.Controls.Label.TextAlignmentProperty);
                }
            }
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

                    OnPropertyChanged(Prism.UI.Controls.Element.VisibilityProperty);
                }
            }
        }
        private Prism.UI.Visibility visibility;

        /// <summary>
        /// Gets the UI element that is displaying the text.
        /// </summary>
        protected TextBlock Element { get; }

        private long callbackToken;
        private ListViewItem parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        public Label()
        {
            HorizontalContentAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);
            Content = Element = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.CharacterEllipsis
            };

            base.Loaded += (o, e) =>
            {
                parent = this.GetParent<ListViewItem>();
                if (parent != null)
                {
                    callbackToken = parent.RegisterPropertyChangedCallback(ListViewItem.IsSelectedProperty, OnParentPropertyChanged);
                }

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
                if (callbackToken > 0)
                {
                    parent?.UnregisterPropertyChangedCallback(ListViewItem.IsSelectedProperty, callbackToken);
                }

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
            if (string.IsNullOrEmpty(Text))
            {
                return Size.Empty;
            }
            
            try
            {
                Element.Width = double.NaN;
                Element.Height = double.NaN;

                base.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                Element.Measure(constraints.GetSize());
                return Element.DesiredSize.GetSize();
            }
            finally
            {
                base.Visibility = visibility == Prism.UI.Visibility.Visible ?
                    global::Windows.UI.Xaml.Visibility.Visible : global::Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
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

        private void OnParentPropertyChanged(DependencyObject obj, DependencyProperty property)
        {
            var lvi = obj as ListViewItem;
            if (lvi.IsSelected && highlightBrush != null)
            {
                Element.Foreground = highlightBrush.GetBrush();
            }
            else
            {
                Element.Foreground = foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.PageTextBaseHighBrushId);
            }
        }
    }
}
