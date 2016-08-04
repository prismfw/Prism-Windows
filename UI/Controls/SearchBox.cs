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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeSearchBox"/>.
    /// </summary>
    [Register(typeof(INativeSearchBox))]
    public class SearchBox : global::Windows.UI.Xaml.Controls.SearchBox, INativeSearchBox
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
        /// Occurs when the value of the <see cref="P:QueryText"/> property has changed.
        /// </summary>
        public new event EventHandler<Prism.UI.Controls.QueryChangedEventArgs> QueryChanged;

        /// <summary>
        /// Occurs when the user submits a search query.
        /// </summary>
        public new event EventHandler<Prism.UI.Controls.QuerySubmittedEventArgs> QuerySubmitted;

        /// <summary>
        /// Occurs when this instance has been detached from the visual tree.
        /// </summary>
        public new event EventHandler Unloaded;

        /// <summary>
        /// Gets or sets the type of action key to use for the soft keyboard when the control has focus.
        /// </summary>
        public ActionKeyType ActionKeyType
        {
            get { return actionKeyType; }
            set
            {
                if (value != actionKeyType)
                {
                    actionKeyType = value;
                    OnPropertyChanged(Prism.UI.Controls.SearchBox.ActionKeyTypeProperty);
                }
            }
        }
        private ActionKeyType actionKeyType;

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
                    base.Background = background.GetBrush() ?? ThemeResources.AltMediumBrush;
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
                    base.BorderBrush = borderBrush.GetBrush();
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
                    base.BorderThickness = new global::Windows.UI.Xaml.Thickness(borderWidth);
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
                    base.Foreground = foreground.GetBrush() ?? ThemeResources.TextForegroundBrush;
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
            get { return FocusState != global::Windows.UI.Xaml.FocusState.Unfocused; }
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
        /// Gets or sets the text to display when the control does not have a value.
        /// </summary>
        public string Placeholder
        {
            get { return base.PlaceholderText; }
            set
            {
                value = value ?? string.Empty;
                if (value != base.PlaceholderText)
                {
                    base.PlaceholderText = value;
                    OnPropertyChanged(Prism.UI.Controls.SearchBox.PlaceholderProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the query text value of the control.
        /// </summary>
        public new string QueryText
        {
            get { return base.QueryText; }
            set
            {
                value = value ?? string.Empty;
                if (value != base.QueryText)
                {
                    base.QueryText = value;
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

                    OnPropertyChanged(Prism.UI.Controls.Element.VisibilityProperty);
                }
            }
        }
        private Visibility visibility;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchBox"/> class.
        /// </summary>
        public SearchBox()
        {
            Margin = new global::Windows.UI.Xaml.Thickness();
            MinHeight = 0;
            MinWidth = 0;

            base.IsEnabledChanged += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.Controls.Control.IsEnabledProperty);
            };

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

            base.SizeChanged += (o, e) =>
            {
                var sv = this.GetChild<Grid>();
                if (sv != null)
                {
                    // failing to do this causes the control to expand when typing
                    sv.Width = e.NewSize.Width;
                    sv.Height = e.NewSize.Height;
                }
            };

            base.QueryChanged += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.Controls.SearchBox.QueryTextProperty);
                QueryChanged(this, new Prism.UI.Controls.QueryChangedEventArgs(e.QueryText));
            };

            base.QuerySubmitted += (o, e) =>
            {
                QuerySubmitted(this, new Prism.UI.Controls.QuerySubmittedEventArgs(e.QueryText));
            };

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
            Focus(global::Windows.UI.Xaml.FocusState.Programmatic);
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

                var desiredSize = base.MeasureOverride(constraints.GetSize()).GetSize();
                desiredSize.Height = Math.Min(constraints.Height, FontSize * 1.33 + Padding.Top + Padding.Bottom + BorderThickness.Top + BorderThickness.Bottom);
                return desiredSize;
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
                bool tabStop = IsTabStop;
                bool enabled = IsEnabled;
                IsTabStop = false;
                IsEnabled = false;
                IsEnabled = enabled;
                IsTabStop = tabStop;
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
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            OnPropertyChanged(Prism.UI.Controls.Control.IsFocusedProperty);
            GotFocus(this, EventArgs.Empty);

            base.OnGotFocus(e);
        }

        /// <summary>
        /// Called before the LostFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            OnPropertyChanged(Prism.UI.Controls.Control.IsFocusedProperty);
            LostFocus(this, EventArgs.Empty);

            base.OnLostFocus(e);
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
