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
using System.Collections;
using System.Linq;
using Prism.Native;
using Prism.UI.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Prism.Input;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeSelectList"/>.
    /// </summary>
    [Register(typeof(INativeSelectList))]
    public class SelectList : ComboBox, INativeSelectList
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
        /// Occurs when the selection of the select list is changed.
        /// </summary>
        public new event EventHandler<Prism.UI.Controls.SelectionChangedEventArgs> SelectionChanged;

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
                        ItemContainerTransitions = itemContainerTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        itemContainerTransitions = ItemContainerTransitions;

                        Transitions = null;
                        ItemContainerTransitions = null;
                    }

                    OnPropertyChanged(Prism.UI.Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, itemContainerTransitions;

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
                    base.Background = background.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.BackgroundAltMediumLowBrushId);
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
                    base.BorderBrush = borderBrush.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseMediumLowBrushId);
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
                    Padding = new global::Windows.UI.Xaml.Thickness(value);
                    BorderThickness = new global::Windows.UI.Xaml.Thickness(borderWidth);
                    OnPropertyChanged(Prism.UI.Controls.Control.BorderWidthProperty);
                }
            }
        }
        private double borderWidth;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a display item for the select list.
        /// </summary>
        public SelectListDisplayItemRequestHandler DisplayItemRequest { get; set; }

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
                    var displayObj = this.GetChild<INativeVisual>();
                    var panel = displayObj as INativePanel;
                    if (panel != null)
                    {
                        foreach (var child in panel.Children)
                        {
                            var label = child as INativeLabel;
                            if (label != null && (label.Foreground == null || label.Foreground == foreground))
                            {
                                label.Foreground = value;
                            }
                            else
                            {
                                var control = child as INativeControl;
                                if (control != null && (control.Foreground == null || control.Foreground == foreground))
                                {
                                    control.Foreground = value;
                                }
                            }
                        }
                    }
                    else
                    {
                        var label = displayObj as INativeLabel;
                        if (label != null && (label.Foreground == null || label.Foreground == foreground))
                        {
                            label.Foreground = value;
                        }
                        else
                        {
                            var control = displayObj as INativeControl;
                            if (control != null && (control.Foreground == null || control.Foreground == foreground))
                            {
                                control.Foreground = value;
                            }
                        }
                    }

                    foreground = value;

                    var brush = foreground.GetBrush();
                    base.Foreground = brush ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseHighBrushId);

                    if (glyphForeground == null)
                    {
                        var glyph = this.GetChild<FontIcon>(c => c.Name == "DropDownGlyph");
                        if (glyph != null)
                        {
                            glyph.Foreground = brush ?? Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseMediumHighBrushId);
                        }
                    }

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

                Width =  value.Width;
                Height = value.Height;
                Canvas.SetLeft(this, value.X);
                Canvas.SetTop(this, value.Y);
            }
        }
        private Rectangle frame = new Rectangle();

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the drop down glyph.
        /// </summary>
        public Brush GlyphForeground
        {
            get { return glyphForeground; }
            set
            {
                glyphForeground = value;

                var glyph = this.GetChild<FontIcon>(c => c.Name == "DropDownGlyph");
                if (glyph != null)
                {
                    glyph.Foreground = (glyphForeground ?? foreground).GetBrush() ??
                        Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseMediumHighBrushId);
                }
            }
        }
        private Brush glyphForeground;

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
        /// Gets or sets a value indicating whether the list is open for selection.
        /// </summary>
        public bool IsOpen
        {
            get { return IsDropDownOpen; }
            set { IsDropDownOpen = value; }
        }

        /// <summary>
        /// Gets or sets a list of the items that make up the selection list.
        /// </summary>
        public new IList Items
        {
            get { return ItemsSource as IList; }
            set
            {
                if (value != ItemsSource)
                {
                    ItemsSource = value;
                    OnPropertyChanged(Prism.UI.Controls.SelectList.ItemsProperty);

                    if (SelectedIndex < 0 && ItemsSource != null)
                    {
                        SelectedIndex = 0;
                    }

                    RefreshDisplayItem();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background of the selection list.
        /// </summary>
        public Brush ListBackground
        {
            get { return listBackground; }
            set
            {
                if (value != listBackground)
                {
                    listBackground = value;

                    var popup = GetTemplateChild("PopupBorder") as global::Windows.UI.Xaml.Controls.Border;
                    if (popup != null)
                    {
                        popup.Background = listBackground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.BackgroundChromeMediumLowBrushId);
                    }

                    OnPropertyChanged(Prism.UI.Controls.SelectList.ListBackgroundProperty);
                }
            }
        }
        private Brush listBackground;

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a list item for an object in the select list.
        /// </summary>
        public SelectListListItemRequestHandler ListItemRequest { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the separators in the selection list, if applicable.
        /// </summary>
        public Brush ListSeparatorBrush
        {
            get { return listSeparatorBrush; }
            set
            {
                if (value != listSeparatorBrush)
                {
                    listSeparatorBrush = value;
                    OnPropertyChanged(Prism.UI.Controls.SelectList.ListSeparatorBrushProperty);
                }
            }
        }
        private Brush listSeparatorBrush;

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
                    OnPropertyChanged(Prism.UI.Visual.RenderTransformProperty);
                }
            }
        }
        private INativeTransform renderTransform;

        /// <summary>
        /// Gets or sets the visual theme that should be used by this instance.
        /// </summary>
        public new Prism.UI.Theme RequestedTheme
        {
            get { return base.RequestedTheme.GetTheme(); }
            set { base.RequestedTheme = value.GetElementTheme(); }
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

                    OnPropertyChanged(Prism.UI.Element.VisibilityProperty);
                }
            }
        }
        private Prism.UI.Visibility visibility;

        private bool suppress;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectList"/> class.
        /// </summary>
        public SelectList()
        {
            MinHeight = 0;
            MinWidth = 0;
            Margin = new global::Windows.UI.Xaml.Thickness();
            Padding = new global::Windows.UI.Xaml.Thickness();
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);
            VerticalContentAlignment = global::Windows.UI.Xaml.VerticalAlignment.Top;
            HorizontalContentAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Left;

            base.IsEnabledChanged += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.Controls.Control.IsEnabledProperty);
            };

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

            base.SelectionChanged += (o, e) =>
            {
                if (!suppress)
                {
                    OnPropertyChanged(Prism.UI.Controls.SelectList.SelectedIndexProperty);
                    SelectionChanged(this, new Prism.UI.Controls.SelectionChangedEventArgs(e.AddedItems.ToArray(), e.RemovedItems.ToArray()));
                }
            };

            LoadItemTemplate();
        }

        /// <summary>
        /// Attempts to set focus to the control.
        /// </summary>
        public void Focus()
        {
            base.Focus(FocusState.Programmatic);
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
        /// Forces a refresh of the display item.
        /// </summary>
        public void RefreshDisplayItem()
        {
            suppress = true;
            int index = SelectedIndex;
            SelectedIndex = -1;
            SelectedIndex = index;
            suppress = false;

            Width = frame.Width = double.NaN;
            Height = frame.Height = double.NaN;
            InvalidateMeasure();
        }

        /// <summary>
        /// Forces a refresh of the items in the selection list.
        /// </summary>
        public void RefreshListItems()
        {
            LoadItemTemplate();
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
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// In simplest terms, this means the method is called just before a UI element displays in your app.
        /// Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var presenter = this.GetChild<ContentPresenter>(c => c.Name == "ContentPresenter");
            if (presenter != null)
            {
                Grid.SetColumnSpan(presenter, 2);
            }

            var glyph = this.GetChild<FontIcon>(c => c.Name == "DropDownGlyph");
            if (glyph != null)
            {
                glyph.MinHeight = 8;
                glyph.Foreground = (glyphForeground ?? foreground).GetBrush() ??
                    Windows.Resources.GetBrush(this, Windows.Resources.ForegroundBaseMediumHighBrushId);
            }

            var popup = GetTemplateChild("PopupBorder") as global::Windows.UI.Xaml.Controls.Border;
            if (popup != null)
            {
                popup.Background = listBackground.GetBrush() ?? Windows.Resources.GetBrush(this, Windows.Resources.BackgroundChromeMediumLowBrushId);
            }
        }

        /// <summary>
        /// Invoked when the DropDownClosed event is raised.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnDropDownClosed(object e)
        {
            base.OnDropDownClosed(e);
            OnPropertyChanged(Prism.UI.Controls.SelectList.IsOpenProperty);
        }

        /// <summary>
        /// Invoked when the DropDownOpened event is raised.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnDropDownOpened(object e)
        {
            base.OnDropDownOpened(e);
            OnPropertyChanged(Prism.UI.Controls.SelectList.IsOpenProperty);
        }

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            OnPropertyChanged(Prism.UI.Controls.Control.IsFocusedProperty);
            GotFocus(this, EventArgs.Empty);

            base.OnGotFocus(e);
        }

        /// <summary>
        /// Called before the LostFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
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

        private void LoadItemTemplate()
        {
            ItemTemplateSelector = new SelectListTemplateSelector(this);
        }

        private class SelectListTemplateSelector : DataTemplateSelector
        {
            private readonly DataTemplate displayItemTemplate;
            private readonly DataTemplate listItemTemplate;

            public SelectListTemplateSelector(SelectList selectList)
            {
                // Using application resources because the XAML fails if using local resources.
                object resource = null;
                global::Windows.UI.Xaml.Application.Current.Resources.TryGetValue("SelectListItemConverter", out resource);
                global::Windows.UI.Xaml.Application.Current.Resources["SelectListItemConverter"] = new SelectListItemConverter(selectList);

                displayItemTemplate = (DataTemplate)XamlReader.Load(@"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <ContentPresenter Content=""{Binding Converter={StaticResource SelectListItemConverter}, ConverterParameter=display}"" />
                </DataTemplate>");

                listItemTemplate = (DataTemplate)XamlReader.Load(@"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <ContentPresenter Content=""{Binding Converter={StaticResource SelectListItemConverter}, ConverterParameter=list}""/>
                </DataTemplate>");

                // Restore the resource to its previous state when we're done with it.
                if (resource == null)
                {
                    global::Windows.UI.Xaml.Application.Current.Resources.Remove("SelectListItemConverter");
                }
                else
                {
                    global::Windows.UI.Xaml.Application.Current.Resources["SelectListItemConverter"] = resource;
                }
            }

            protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            {
                var comboItem = container as ComboBoxItem ?? container.GetParent<ComboBoxItem>();
                if (comboItem != null)
                {
                    comboItem.Margin = new global::Windows.UI.Xaml.Thickness();
                    comboItem.Padding = new global::Windows.UI.Xaml.Thickness();
                    return listItemTemplate;
                }

                return displayItemTemplate;
            }
        }

        private class SelectListItemConverter : IValueConverter
        {
            private readonly WeakReference<SelectList> selectList;

            public SelectListItemConverter(SelectList selectList)
            {
                this.selectList = new WeakReference<SelectList>(selectList);
            }

            public object Convert(object value, Type targetType, object parameter, string language)
            {
                SelectList sl;
                if (selectList.TryGetTarget(out sl))
                {
                    if ((parameter as string) != "list")
                    {
                        return sl.DisplayItemRequest();
                    }

                    return new SelectListItemPresenter() { Children = { sl.ListItemRequest(value) as UIElement } };
                }

                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                return value;
            }
        }

        private class SelectListItemPresenter : Grid
        {
            public SelectListItemPresenter()
            {
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Left;
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Top;
            }

            protected override global::Windows.Foundation.Size ArrangeOverride(global::Windows.Foundation.Size finalSize)
            {
                var child = Children.FirstOrDefault();
                if (child == null)
                {
                    return global::Windows.Foundation.Size.Empty;
                }

                var visual = child as INativeVisual;
                if (visual == null)
                {
                    return base.ArrangeOverride(finalSize);
                }

                visual.ArrangeRequest(false, null);
                finalSize = visual.Frame.Size.GetSize();
                base.ArrangeOverride(finalSize);
                return finalSize;
            }

            protected override global::Windows.Foundation.Size MeasureOverride(global::Windows.Foundation.Size availableSize)
            {
                var child = Children.FirstOrDefault();
                if (child == null)
                {
                    return global::Windows.Foundation.Size.Empty;
                }

                var visual = child as INativeVisual;
                if (visual == null)
                {
                    return base.MeasureOverride(availableSize);
                }

                var desiredSize = visual.MeasureRequest(false, null).GetSize();

                base.MeasureOverride(desiredSize);
                return new global::Windows.Foundation.Size(Math.Max(0, desiredSize.Width - Margin.Left - Margin.Right),
                    Math.Max(0, desiredSize.Height - Margin.Top - Margin.Bottom));
            }
        }
    }
}
