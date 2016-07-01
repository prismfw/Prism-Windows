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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Input;
using Prism.Native;
using Prism.UI;
using Prism.UI.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeListBox"/>.
    /// </summary>
    [Register(typeof(INativeListBox))]
    public class ListBox : ListView, INativeListBox
    {
        /// <summary>
        /// Occurs when an accessory in a list box item is clicked or tapped.
        /// </summary>
        public event EventHandler<Prism.UI.Controls.AccessoryClickedEventArgs> AccessoryClicked;

        /// <summary>
        /// Occurs when an item in the list box is clicked or tapped.
        /// </summary>
        public event EventHandler<Prism.UI.Controls.ItemClickedEventArgs> ItemClicked;

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
        /// Occurs when the selection of the list box is changed.
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
                        HeaderTransitions = headerTransitions;
                        FooterTransitions = footerTransitions;
                        ItemContainerTransitions = itemContainerTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        headerTransitions = HeaderTransitions;
                        footerTransitions = FooterTransitions;
                        itemContainerTransitions = ItemContainerTransitions;

                        Transitions = null;
                        HeaderTransitions = null;
                        FooterTransitions = null;
                        ItemContainerTransitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, headerTransitions, footerTransitions, itemContainerTransitions;

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
                    base.Background = background.GetBrush();
                    OnPropertyChanged(Prism.UI.Controls.ListBox.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets a value indicating whether the contents can be scrolled horizontally.
        /// </summary>
        public bool CanScrollHorizontally
        {
            get { return global::Windows.UI.Xaml.Controls.ScrollViewer.GetHorizontalScrollBarVisibility(this) != ScrollBarVisibility.Disabled; }
            set
            {
                if (value != CanScrollHorizontally)
                {
                    global::Windows.UI.Xaml.Controls.ScrollViewer.SetHorizontalScrollBarVisibility(this, value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the contents can be scrolled vertically.
        /// </summary>
        public bool CanScrollVertically
        {
            get { return global::Windows.UI.Xaml.Controls.ScrollViewer.GetVerticalScrollBarVisibility(this) != ScrollBarVisibility.Disabled; }
            set
            {
                if (value != CanScrollVertically)
                {
                    global::Windows.UI.Xaml.Controls.ScrollViewer.SetVerticalScrollBarVisibility(this, value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled);
                }
            }
        }

        /// <summary>
        /// Gets the distance that the contents has been scrolled.
        /// </summary>
        public Point ContentOffset
        {
            get
            {
                var sv = GetTemplateChild("ScrollViewer") as global::Windows.UI.Xaml.Controls.ScrollViewer;
                return sv == null ? new Point() : new Point(sv.HorizontalOffset, sv.VerticalOffset);
            }
        }

        /// <summary>
        /// Gets the size of the scrollable area.
        /// </summary>
        public Size ContentSize { get; private set; }

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
        /// Gets a value indicating whether this instance has been loaded and is ready for rendering.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether each object in the <see cref="P:Items"/> collection represents a different section in the list.
        /// When <c>true</c>, objects that implement <see cref="IList"/> will have each of their items represent a different entry in the same section.
        /// </summary>
        public bool IsSectioningEnabled
        {
            get { return collectionSource.IsSourceGrouped; }
            set
            {
                if (value != collectionSource.IsSourceGrouped)
                {
                    collectionSource.IsSourceGrouped = value;
                    OnPropertyChanged(Prism.UI.Controls.ListBox.IsSectioningEnabledProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a reuse identifier for an object in the list box.
        /// </summary>
        public ItemIdRequestHandler ItemIdRequest { get; set; }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a display item for an object in the list box.
        /// </summary>
        public ListBoxItemRequestHandler ItemRequest { get; set; }

        /// <summary>
        /// Gets or sets the items that make up the contents of the list box.
        /// Items that implement the <see cref="IList"/> interface will be treated as different sections if <see cref="P:IsSectioningEnabled"/> is <c>true</c>.
        /// </summary>
        public new IList Items
        {
            get { return collectionSource.Source as IList; }
            set
            {
                if (value != collectionSource.Source)
                {
                    collectionSource.Source = value;
                    OnPropertyChanged(Prism.UI.Controls.ListBox.ItemsProperty);
                    Reload();
                }
            }
        }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a measurement of itself and its children.
        /// </summary>
        public MeasureRequestHandler MeasureRequest { get; set; }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a section header in the list box.
        /// </summary>
        public ListBoxSectionHeaderRequestHandler SectionHeaderRequest { get; set; }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests a reuse identifier for a section header.
        /// </summary>
        public ItemIdRequestHandler SectionHeaderIdRequest { get; set; }

        /// <summary>
        /// Gets the currently selected items.
        /// To programmatically select and deselect items, use the <see cref="M:Select"/> and <see cref="M:Deselect"/> methods.
        /// </summary>
        public new IList SelectedItems
        {
            get { return new ReadOnlyCollection<object>(base.SelectedItems); }
        }

        /// <summary>
        /// Gets or sets the selection behavior for the list box.
        /// </summary>
        public new Prism.UI.Controls.SelectionMode SelectionMode
        {
            get { return (Prism.UI.Controls.SelectionMode)base.SelectionMode; }
            set
            {
                if (value != SelectionMode)
                {
                    base.SelectionMode = (ListViewSelectionMode)value;
                    OnPropertyChanged(Prism.UI.Controls.ListBox.SelectionModeProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the separators between each item in the list.
        /// </summary>
        public Brush SeparatorBrush
        {
            get { return separatorBrush; }
            set
            {
                if (value != separatorBrush)
                {
                    separatorBrush = value;

                    var child = this.GetChild<ItemsStackPanel>();
                    if (child != null)
                    {
                        int count = global::Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(child);
                        for (int i = 0; i < count; i++)
                        {
                            var item = global::Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(child, i);
                            var separator = (Shape)item.GetChild<Line>(c => c.Name == "separator") ?? item.GetChild<global::Windows.UI.Xaml.Shapes.Rectangle>();
                            if (separator != null)
                            {
                                separator.Stroke = separatorBrush.GetBrush() ?? new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(50, 0, 0, 0));
                            }
                        }
                    }

                    OnPropertyChanged(Prism.UI.Controls.ListBox.SeparatorBrushProperty);
                }
            }
        }
        private Brush separatorBrush;

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

        private string HeaderConverterId
        {
            get { return Name + "ListBoxHeaderConverter"; }
        }

        private string ItemConverterId
        {
            get { return Name + "ListBoxItemConverter"; }
        }

        private readonly CollectionViewSource collectionSource;
        private readonly Dictionary<INativeListBoxItem, object> itemMap = new Dictionary<INativeListBoxItem, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBox"/> class.
        /// </summary>
        public ListBox()
        {
            collectionSource = new CollectionViewSource();
            BindingOperations.SetBinding(this, ItemsSourceProperty, new Binding() { Source = collectionSource });

            IsItemClickEnabled = true;
            GroupStyle.Add(new GroupStyle() { HidesIfEmpty = false });
            ItemContainerStyle = (Style)XamlReader.Load(@"
                <Style xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" TargetType=""ListViewItem"">
                    <Setter Property=""FontFamily"" Value=""{ThemeResource ContentControlThemeFontFamily}""/>
                    <Setter Property=""FontSize"" Value=""{ThemeResource ControlContentThemeFontSize}""/>
                    <Setter Property=""Background"" Value=""Transparent""/>
                    <Setter Property=""Foreground"" Value=""{ThemeResource SystemControlForegroundBaseHighBrush}""/>
                    <Setter Property=""TabNavigation"" Value=""Local""/>
                    <Setter Property=""IsHoldingEnabled"" Value=""True""/>
                    <Setter Property=""Padding"" Value=""0""/>
                    <Setter Property=""HorizontalContentAlignment"" Value=""Left""/>
                    <Setter Property=""VerticalContentAlignment"" Value=""Center""/>
                    <Setter Property=""MinWidth"" Value=""{ThemeResource ListViewItemMinWidth}""/>
                    <Setter Property=""MinHeight"" Value=""{ThemeResource ListViewItemMinHeight}""/>
                    <Setter Property=""Template"">
                        <Setter.Value>
                            <ControlTemplate TargetType=""ListViewItem"">
                                <ListViewItemPresenter
                                    CheckBrush=""{ThemeResource SystemControlForegroundBaseMediumHighBrush}""
                                    ContentMargin=""{TemplateBinding Padding}""
                                    CheckMode=""Inline""
                                    ContentTransitions=""{TemplateBinding ContentTransitions}""
                                    CheckBoxBrush=""{ThemeResource SystemControlForegroundBaseMediumHighBrush}""
                                    DragForeground=""{ThemeResource ListViewItemDragForegroundThemeBrush}""
                                    DragOpacity=""{ThemeResource ListViewItemDragThemeOpacity}""
                                    DragBackground=""{ThemeResource ListViewItemDragBackgroundThemeBrush}""
                                    DisabledOpacity=""{ThemeResource ListViewItemDisabledThemeOpacity}""
                                    FocusBorderBrush=""{ThemeResource SystemControlForegroundAltHighBrush}""
                                    FocusSecondaryBorderBrush=""{ThemeResource SystemControlForegroundBaseHighBrush}""
                                    HorizontalContentAlignment=""{TemplateBinding HorizontalContentAlignment}""
                                    PointerOverForeground=""{ThemeResource SystemControlHighlightAltBaseHighBrush}""
                                    PressedBackground=""{ThemeResource SystemControlHighlightListMediumBrush}""
                                    PlaceholderBackground=""{ThemeResource ListViewItemPlaceholderBackgroundThemeBrush}""
                                    PointerOverBackground=""{ThemeResource SystemControlHighlightListLowBrush}""
                                    ReorderHintOffset=""{ThemeResource ListViewItemReorderHintThemeOffset}""
                                    SelectedPressedBackground=""{ThemeResource SystemControlHighlightListAccentHighBrush}""
                                    SelectionCheckMarkVisualEnabled=""False""
                                    SelectedForeground=""{ThemeResource SystemControlHighlightAltBaseHighBrush}""
                                    SelectedPointerOverBackground=""{ThemeResource SystemControlHighlightListAccentMediumBrush}""
                                    SelectedBackground=""{ThemeResource SystemControlHighlightListAccentLowBrush}""
                                    VerticalContentAlignment=""{TemplateBinding VerticalContentAlignment}""
                                />
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </Style> ");

            base.ChoosingGroupHeaderContainer += (o, e) =>
            {
                var separator = e.GroupHeaderContainer?.GetChild<global::Windows.UI.Xaml.Shapes.Rectangle>();
                if (separator != null)
                {
                    separator.Stroke = separatorBrush.GetBrush() ?? new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(50, 0, 0, 0));
                }
            };

            // This only fires for new selections.  Similarly, each item's Tapped event seems to only fire for items that are already selected.
            // Using both of these events together gives us the behavior we want, but it's a fragile setup, so be vigilant of possible problems.
            base.ItemClick += (o, e) =>
            {
                if (SelectionMode != Prism.UI.Controls.SelectionMode.None)
                {
                    ItemClicked(this, new Prism.UI.Controls.ItemClickedEventArgs(e.ClickedItem));
                }
            };

            base.Loaded += (o, e) =>
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Reload();
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

            base.SelectionChanged += (o, e) =>
            {
                if (IsLoaded)
                {
                    OnPropertyChanged(Prism.UI.Controls.ListBox.SelectedItemsProperty);
                    SelectionChanged(this, new Prism.UI.Controls.SelectionChangedEventArgs(e.AddedItems.ToArray(), e.RemovedItems.ToArray()));
                }
            };

            base.SizeChanged += (o, e) =>
            {
                var sv = GetTemplateChild("ScrollViewer") as global::Windows.UI.Xaml.Controls.ScrollViewer;
                if (sv != null)
                {
                    sv.Height = e.NewSize.Height;
                    sv.Width = e.NewSize.Width;
                }
            };

            base.Unloaded += (o, e) =>
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    global::Windows.UI.Xaml.Application.Current.Resources.Remove(ItemConverterId);
                    global::Windows.UI.Xaml.Application.Current.Resources.Remove(HeaderConverterId);
                    Name = string.Empty;
                }

                IsLoaded = false;
                OnPropertyChanged(Prism.UI.Visual.IsLoadedProperty);
                Unloaded(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// Deselects the specified item.
        /// </summary>
        /// <param name="item">The item within the <see cref="P:Items"/> collection to deselect.</param>
        /// <param name="animate">Whether to animate the deselection.</param>
        public void DeselectItem(object item, Animate animate)
        {
            if (base.SelectedItems.Contains(item))
            {
                if (base.SelectionMode == ListViewSelectionMode.Single)
                {
                    base.SelectedItem = null;
                }
                else if (base.SelectionMode != ListViewSelectionMode.None)
                {
                    base.SelectedItems.Remove(item);
                }
            }
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
        /// Forces a reload of the list box's entire contents.
        /// </summary>
        public void Reload()
        {
            // Using application resources because the XAML fails if using local resources.
            if (string.IsNullOrEmpty(Name))
            {
                do
                {
                    Name = Guid.NewGuid().ToString();
                }
                while (global::Windows.UI.Xaml.Application.Current.Resources.ContainsKey(ItemConverterId));
            }
            
            object resource = null;
            global::Windows.UI.Xaml.Application.Current.Resources.TryGetValue(ItemConverterId, out resource);
            if (!(resource is ListBoxItemConverter))
            {
                global::Windows.UI.Xaml.Application.Current.Resources[ItemConverterId] = new ListBoxItemConverter(this);
            }

            ItemTemplate = (DataTemplate)XamlReader.Load(string.Format(@"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <ContentPresenter Content=""{{Binding Converter={{StaticResource {0}}}}}"" HorizontalAlignment=""Stretch""/>
                </DataTemplate>", ItemConverterId));

            global::Windows.UI.Xaml.Application.Current.Resources.TryGetValue(HeaderConverterId, out resource);
            if (!(resource is ListBoxHeaderConverter))
            {
                global::Windows.UI.Xaml.Application.Current.Resources[HeaderConverterId] = new ListBoxHeaderConverter(this);
            }

            GroupStyle[0].HeaderTemplate = (DataTemplate)XamlReader.Load(string.Format(@"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <ContentPresenter Content=""{{Binding Converter={{StaticResource {0}}}}}"" HorizontalAlignment=""Stretch"" Padding=""-12,0,0,0""/>
                </DataTemplate>", HeaderConverterId));
        }

        /// <summary>
        /// Scrolls to the specified item.
        /// </summary>
        /// <param name="item">The item within the <see cref="P:Items"/> collection to which the list box should scroll.</param>
        /// <param name="animate">Whether to animate the scrolling.</param>
        public void ScrollTo(object item, Animate animate)
        {
            base.ScrollIntoView(item);
        }

        /// <summary>
        /// Scrolls the contents to the specified offset.
        /// </summary>
        /// <param name="offset">The position to which to scroll the contents.</param>
        /// <param name="animate">Whether to animate the scrolling.</param>
        public void ScrollTo(Point offset, Animate animate)
        {
            var sv = GetTemplateChild("ScrollViewer") as global::Windows.UI.Xaml.Controls.ScrollViewer;
            if (sv != null)
            {
                sv.ChangeView(offset.X, offset.Y, null, animate == Animate.Off);
            }
        }

        /// <summary>
        /// Selects the specified item.
        /// </summary>
        /// <param name="item">The item within the <see cref="P:Items"/> collection to select.</param>
        /// <param name="animate">Whether to animate the selection.</param>
        public void SelectItem(object item, Animate animate)
        {
            if (!base.SelectedItems.Contains(item))
            {
                if (base.SelectionMode == ListViewSelectionMode.Single)
                {
                    base.SelectedItem = item;
                }
                else if (base.SelectionMode != ListViewSelectionMode.None)
                {
                    base.SelectedItems.Add(item);
                }
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

            var sv = GetTemplateChild("ScrollViewer") as global::Windows.UI.Xaml.Controls.ScrollViewer;
            if (sv != null && sv.ExtentWidth != ContentSize.Width || sv.ExtentHeight != ContentSize.Height)
            {
                ContentSize = new Size(sv.ExtentWidth, sv.ExtentHeight);
                OnPropertyChanged(Prism.UI.Controls.ListBox.ContentSizeProperty);
            }

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

            var sv = GetTemplateChild("ScrollViewer") as global::Windows.UI.Xaml.Controls.ScrollViewer;
            if (sv != null)
            {
                sv.ViewChanged += (o, e) =>
                {
                    OnPropertyChanged(Prism.UI.Controls.ListBox.ContentOffsetProperty);
                };
            }

            SelectedIndex = -1;
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
        /// Called when an item in the list box is clicked or tapped.
        /// </summary>
        /// <param name="item">The item that was clicked.</param>
        protected internal void OnItemClicked(INativeListBoxItem item)
        {
            object value;
            if (SelectionMode != Prism.UI.Controls.SelectionMode.None && itemMap.TryGetValue(item, out value))
            {
                ItemClicked(this, new Prism.UI.Controls.ItemClickedEventArgs(value));
            }
        }

        private void OnItemUnloaded(object sender, EventArgs e)
        {
            var item = sender as INativeListBoxItem;
            if (item != null)
            {
                item.Unloaded -= OnItemUnloaded;
                itemMap.Remove(item);
            }
        }

        private class ListBoxHeaderConverter : IValueConverter
        {
            private readonly ListBox listBox;

            public ListBoxHeaderConverter(ListBox listBox)
            {
                this.listBox = listBox;
            }

            public object Convert(object value, Type targetType, object parameter, string language)
            {
                return listBox.SectionHeaderRequest(value, null);
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                return value;
            }
        }

        private class ListBoxItemConverter : IValueConverter
        {
            private readonly ListBox listBox;

            public ListBoxItemConverter(ListBox listBox)
            {
                this.listBox = listBox;
            }

            public object Convert(object value, Type targetType, object parameter, string language)
            {
                var item = listBox.ItemRequest(value, null);
                if (item != null)
                {
                    // This is not an ideal setup, but the ListView's ItemClick event does not behave the way we need it to.
                    // To get around it, we're relying on each item to tell the ListView when it's tapped and keeping track of which item corresponds to which value.
                    listBox.itemMap[item] = value;
                    item.Unloaded -= listBox.OnItemUnloaded;
                    item.Unloaded += listBox.OnItemUnloaded;

                    var separator = (item as DependencyObject)?.GetChild<Line>(c => c.Name == "separator");
                    if (separator != null)
                    {
                        separator.Stroke = listBox.separatorBrush.GetBrush() ?? new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(50, 0, 0, 0));
                    }
                }

                return item;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                return value;
            }
        }
    }
}
