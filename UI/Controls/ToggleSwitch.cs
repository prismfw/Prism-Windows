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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeToggleSwitch"/>.
    /// </summary>
    [Register(typeof(INativeToggleSwitch))]
    public class ToggleSwitch : ContentControl, INativeToggleSwitch
    {
        /// <summary>
        /// Occurs when the control receives focus.
        /// </summary>
        public new event EventHandler GotFocus;

        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when the control loses focus.
        /// </summary>
        public new event EventHandler LostFocus;

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
        /// Occurs when the value of the toggle switch is changed.
        /// </summary>
        public event EventHandler ValueChanged;

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
        /// Gets or sets the background for the control.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    Element.Background = background.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.BackgroundAltHighBrushId);
                    OnPropertyChanged(Prism.UI.Controls.Control.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the border of the control.
        /// </summary>
        public new Brush BorderBrush
        {
            get { return borderBrush; }
            set
            {
                if (value != borderBrush)
                {
                    borderBrush = value;
                    Element.BorderBrush = borderBrush.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId);
                    OnPropertyChanged(Prism.UI.Controls.Control.BorderBrushProperty);
                }
            }
        }
        private Brush borderBrush;

        /// <summary>
        /// Gets or sets the width of the border around the control.
        /// </summary>
        public double BorderWidth
        {
            get { return borderWidth; }
            set
            {
                if (value != borderWidth)
                {
                    borderWidth = value;
                    Element.BorderThickness = new global::Windows.UI.Xaml.Thickness(borderWidth);
                    OnPropertyChanged(Prism.UI.Controls.Control.BorderWidthProperty);
                }
            }
        }
        private double borderWidth;

        /// <summary>
        /// Gets or sets the font to use for displaying the text in the control.
        /// </summary>
        public new object FontFamily
        {
            get { return base.FontFamily; }
            set
            {
                var fontFamily = value as Media.FontFamily;
                if (fontFamily != base.FontFamily)
                {
                    this.SetFont(fontFamily, FontStyle);
                    OnPropertyChanged(Prism.UI.Controls.Control.FontFamilyProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the text in the control.
        /// </summary>
        public new double FontSize
        {
            get { return base.FontSize; }
            set
            {
                if (value != base.FontSize)
                {
                    base.FontSize = value;
                    OnPropertyChanged(Prism.UI.Controls.Control.FontSizeProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style with which to render the text in the control.
        /// </summary>
        public new FontStyle FontStyle
        {
            get { return this.GetFontStyle(); }
            set
            {
                var style = base.FontStyle;
                var weight = base.FontWeight.Weight;
                this.SetFont(base.FontFamily as Media.FontFamily, value);

                if (base.FontStyle != style || base.FontWeight.Weight != weight)
                {
                    OnPropertyChanged(Prism.UI.Controls.Control.FontStyleProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the foreground content of the control.
        /// </summary>
        public new Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;
                    Element.Foreground = foreground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.HighlightAccentBrushId);
                    OnPropertyChanged(Prism.UI.Controls.Control.ForegroundProperty);
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
        /// Gets a value indicating whether the control has focus.
        /// </summary>
        public bool IsFocused
        {
            get { return Element.FocusState != global::Windows.UI.Xaml.FocusState.Unfocused; }
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
        /// Gets or sets the <see cref="Brush"/> to apply to the thumb of the control when the control's value is false.
        /// </summary>
        public Brush ThumbOffBrush
        {
            get { return thumbOffBrush; }
            set
            {
                if (value != thumbOffBrush)
                {
                    thumbOffBrush = value;

                    var thumb = Element.GetChild<Ellipse>(c => c.Name == "SwitchKnobOff");
                    if (thumb != null)
                    {
                        thumb.Fill = thumbOffBrush.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseMediumHighBrushId);
                    }

                    OnPropertyChanged(Prism.UI.Controls.ToggleSwitch.ThumbOffBrushProperty);
                }
            }
        }
        private Brush thumbOffBrush;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the thumb of the control when the control's value is true.
        /// </summary>
        public Brush ThumbOnBrush
        {
            get { return thumbOnBrush; }
            set
            {
                if (value != thumbOnBrush)
                {
                    thumbOnBrush = value;

                    var thumb = Element.GetChild<Ellipse>(c => c.Name == "SwitchKnobOn");
                    if (thumb != null)
                    {
                        thumb.Fill = thumbOnBrush.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.HighlightAltChromeWhiteBrushId);
                    }

                    OnPropertyChanged(Prism.UI.Controls.ToggleSwitch.ThumbOnBrushProperty);
                }
            }
        }
        private Brush thumbOnBrush;

        /// <summary>
        /// Gets or sets the value of the toggle switch.
        /// </summary>
        public bool Value
        {
            get { return Element.IsOn; }
            set { Element.IsOn = value; }
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
        /// Gets the UI element that is displaying the switch.
        /// </summary>
        protected global::Windows.UI.Xaml.Controls.ToggleSwitch Element { get; }

        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitch"/> class.
        /// </summary>
        public ToggleSwitch()
        {
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);

            Content = Element = new global::Windows.UI.Xaml.Controls.ToggleSwitch()
            {
                Margin = new global::Windows.UI.Xaml.Thickness(),
                OffContent = null,
                OnContent = null,
                Padding = new global::Windows.UI.Xaml.Thickness()
            };

            Element.GotFocus += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.Controls.Control.IsFocusedProperty);
                GotFocus(this, EventArgs.Empty);
            };

            Element.LostFocus += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.Controls.Control.IsFocusedProperty);
                LostFocus(this, EventArgs.Empty);
            };

            Element.Toggled += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.Controls.ToggleSwitch.ValueProperty);
                ValueChanged(this, EventArgs.Empty);
            };

            base.IsEnabledChanged += (o, e) =>
            {
                Element.IsEnabled = IsEnabled;
                OnPropertyChanged(Prism.UI.Controls.Control.IsEnabledProperty);
            };

            base.Loaded += (o, e) =>
            {
                OnInitialize();
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
        /// Attempts to set focus to the control.
        /// </summary>
        public void Focus()
        {
            Element.Focus(global::Windows.UI.Xaml.FocusState.Programmatic);
        }

        /// <summary>
        /// Measures the object and returns its desired size.
        /// </summary>
        /// <param name="constraints">The width and height that the object is not allowed to exceed.</param>
        /// <returns>The desired size as a <see cref="Size"/> instance.</returns>
        public Size Measure(Size constraints)
        {
            try
            {
                Width = double.NaN;
                Height = double.NaN;

                base.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                return base.MeasureOverride(constraints.GetSize()).GetSize();
            }
            finally
            {
                base.Visibility = visibility == Prism.UI.Visibility.Visible ?
                    global::Windows.UI.Xaml.Visibility.Visible : global::Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Attempts to remove focus from the control.
        /// </summary>
        public void Unfocus()
        {
            if (IsFocused)
            {
                bool tabStop = Element.IsTabStop;
                bool enabled = Element.IsEnabled;
                Element.IsTabStop = false;
                Element.IsEnabled = false;
                Element.IsEnabled = enabled;
                Element.IsTabStop = tabStop;
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// In simplest terms, this means the method is called just before a UI element displays in your app.
        /// Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnInitialize();
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

        private void OnInitialize()
        {
            if (isInitialized)
            {
                return;
            }

            // Some people may look at this and wonder why I don't just make a xaml template.
            // To those people, I say, "...nah"
            var grid = Element.GetChild<Grid>();
            if (grid == null)
            {
                return;
            }

            grid.Background = null;
            grid.BorderBrush = null;
            grid.BorderThickness = new global::Windows.UI.Xaml.Thickness(0);

            isInitialized = true;
            
            var knob = Element.GetChild<global::Windows.UI.Xaml.Shapes.Rectangle>(c => c.Name == "SwitchKnobBounds");
            if (knob != null)
            {
                knob.SetBinding(global::Windows.UI.Xaml.Shapes.Rectangle.FillProperty, new global::Windows.UI.Xaml.Data.Binding()
                {
                    Source = Element,
                    Path = new global::Windows.UI.Xaml.PropertyPath("Foreground")
                });
            }

            var border = Element.GetChild<global::Windows.UI.Xaml.Shapes.Rectangle>(c => c.Name == "OuterBorder");
            if (border != null)
            {
                border.SetBinding(global::Windows.UI.Xaml.Shapes.Rectangle.FillProperty, new global::Windows.UI.Xaml.Data.Binding()
                {
                    Source = Element,
                    Path = new global::Windows.UI.Xaml.PropertyPath("Background")
                });

                border.SetBinding(global::Windows.UI.Xaml.Shapes.Rectangle.StrokeProperty, new global::Windows.UI.Xaml.Data.Binding()
                {
                    Source = Element,
                    Path = new global::Windows.UI.Xaml.PropertyPath("BorderBrush")
                });
            }

            var thumb = Element.GetChild<Ellipse>(c => c.Name == "SwitchKnobOn");
            if (thumb != null)
            {
                thumb.Fill = thumbOnBrush.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.HighlightAltChromeWhiteBrushId);
            }

            thumb = Element.GetChild<Ellipse>(c => c.Name == "SwitchKnobOff");
            if (thumb != null)
            {
                thumb.Fill = thumbOffBrush.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseMediumHighBrushId);
            }

            var visualStates = global::Windows.UI.Xaml.VisualStateManager.GetVisualStateGroups(grid).FirstOrDefault(vg => vg.Name == "CommonStates");
            if (visualStates != null)
            {
                var disabled = visualStates.States.FirstOrDefault(vs => vs.Name == "Disabled");
                if (disabled != null)
                {
                    var anim = new ObjectAnimationUsingKeyFrames()
                    {
                        KeyFrames =
                        {
                            new DiscreteObjectKeyFrame()
                            {
                                KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)),
                                Value = null
                            }
                        }
                    };

                    Storyboard.SetTarget(anim, border);
                    Storyboard.SetTargetProperty(anim, "Fill");

                    disabled.Storyboard.Children.Add(anim);
                }

                var pointerOver = visualStates.States.FirstOrDefault(vs => vs.Name == "PointerOver");
                if (pointerOver != null)
                {
                    pointerOver.Storyboard.Children.Remove(pointerOver.Storyboard.Children.FirstOrDefault(t => Storyboard.GetTargetName(t) == "SwitchKnobOff"));
                }
            }
        }
    }
}
