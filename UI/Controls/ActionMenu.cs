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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Prism.Native;
using Prism.UI.Media;
using Windows.UI.Xaml.Controls;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeActionMenu"/>.
    /// </summary>
    [Register(typeof(INativeActionMenu))]
    public class ActionMenu : CommandBar, INativeActionMenu
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<FrameworkPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Gets or sets the background for the menu.
        /// </summary>
        public new Brush Background
        {
            get { return background; }
            set
            {
                if (value != background)
                {
                    background = value;
                    base.Background = background.GetBrush() ?? ThemeResources.ChromeMediumBrush;
                    SetOverflowBackground();
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.BackgroundProperty);
                }
            }
        }
        private Brush background;

        /// <summary>
        /// Gets or sets the title of the menu's Cancel button, if one exists.
        /// </summary>
        public string CancelButtonTitle
        {
            get { return cancelButtonTitle; }
            set
            {
                if (value != cancelButtonTitle)
                {
                    cancelButtonTitle = value;
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.CancelButtonTitleProperty);
                }
            }
        }
        private string cancelButtonTitle;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the foreground content of the menu.
        /// </summary>
        public new Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (value != foreground)
                {
                    foreground = value;
                    base.Foreground = foreground.GetBrush() ?? ThemeResources.ButtonForegroundBrush;
                    foreach (var item in Items.OfType<INativeMenuItem>().Where(i => i.Foreground == null))
                    {
                        var control = item as Control;
                        if (control != null)
                        {
                            control.Foreground = base.Foreground;
                        }
                    }
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.ForegroundProperty);
                }
            }
        }
        private Brush foreground;

        /// <summary>
        /// Gets the amount that the menu is inset on top of its parent view.
        /// </summary>
        public Thickness Insets
        {
            get { return new Thickness(0, 0, 0, ActualHeight); }
        }

        /// <summary>
        /// Gets a collection of the items within the menu.
        /// </summary>
        public IList Items { get; }

        /// <summary>
        /// Gets or sets the maximum number of menu items that can be displayed before they are placed into an overflow menu.
        /// </summary>
        public int MaxDisplayItems
        {
            get { return maxDisplayItems; }
            set
            {
                if (value != maxDisplayItems)
                {
                    maxDisplayItems = value;
                    OrganizeItems();
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.MaxDisplayItemsProperty);
                }
            }
        }
        private int maxDisplayItems;

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> of the image to use for representing the overflow menu when one is present.
        /// </summary>
        public Uri OverflowImageUri
        {
            get { return overflowImageUri; }
            set
            {
                if (value != overflowImageUri)
                {
                    overflowImageUri = value;
                    SetOverflowImage();
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.OverflowImageUriProperty);
                }
            }
        }
        private Uri overflowImageUri;

        private object defaultOverflowImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMenu"/> class.
        /// </summary>
        public ActionMenu()
        {
            ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
            Items = new ObservableCollection<INativeMenuItem>();
            ((ObservableCollection<INativeMenuItem>)Items).CollectionChanged += (o, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = e.NewStartingIndex; i < e.NewItems.Count + e.NewStartingIndex; i++)
                        {
                            if (i < maxDisplayItems)
                            {
                                PrimaryCommands.Insert(i, e.NewItems[i - e.NewStartingIndex] as ICommandBarElement);
                                if (PrimaryCommands.Count > maxDisplayItems)
                                {
                                    var item = PrimaryCommands[PrimaryCommands.Count - 1];
                                    PrimaryCommands.RemoveAt(PrimaryCommands.Count - 1);
                                    SecondaryCommands.Insert(0, item);
                                }
                            }
                            else
                            {
                                SecondaryCommands.Insert(i - maxDisplayItems, e.NewItems[i - e.NewStartingIndex] as ICommandBarElement);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        for (int i = e.OldStartingIndex + e.OldItems.Count; i >= e.OldStartingIndex; i--)
                        {
                            if (i < maxDisplayItems)
                            {
                                PrimaryCommands.RemoveAt(i);
                            }
                            else
                            {
                                SecondaryCommands.RemoveAt(i - maxDisplayItems);
                            }
                        }

                        for (int i = e.NewStartingIndex; i < e.NewItems.Count + e.NewStartingIndex; i++)
                        {
                            if (i < maxDisplayItems)
                            {
                                PrimaryCommands.Insert(i, e.NewItems[i - e.NewStartingIndex] as ICommandBarElement);
                            }
                            else
                            {
                                SecondaryCommands.Insert(i - maxDisplayItems, e.NewItems[i - e.NewStartingIndex] as ICommandBarElement);
                            }
                        }

                        OrganizeItems();
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = e.OldStartingIndex + e.OldItems.Count; i >= e.OldStartingIndex; i--)
                        {
                            if (i < maxDisplayItems)
                            {
                                PrimaryCommands.RemoveAt(i);
                                if (PrimaryCommands.Count < maxDisplayItems && SecondaryCommands.Count > 0)
                                {
                                    var item = SecondaryCommands[0];
                                    PrimaryCommands.Add(item);
                                    SecondaryCommands.RemoveAt(0);
                                }
                            }
                            else
                            {
                                SecondaryCommands.RemoveAt(i - maxDisplayItems);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                        {
                            if (i < maxDisplayItems)
                            {
                                PrimaryCommands[i] = e.NewItems[i] as ICommandBarElement;
                            }
                            else
                            {
                                SecondaryCommands[i - maxDisplayItems] = e.NewItems[i] as ICommandBarElement;
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        PrimaryCommands.Clear();
                        SecondaryCommands.Clear();
                        break;
                }

                if (Foreground != null && e.NewItems != null)
                {
                    foreach (var item in e.NewItems.OfType<INativeMenuItem>().Where(i => i.Foreground == null))
                    {
                        var control = item as Control;
                        if (control != null)
                        {
                            control.Foreground = base.Foreground;
                        }
                    }
                }
            };

            base.SizeChanged += (o, e) =>
            {
                if (e.NewSize.Height != e.PreviousSize.Height)
                {
                    OnPropertyChanged(Prism.UI.Controls.ActionMenu.InsetsProperty);
                }
            };
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// In simplest terms, this means the method is called just before a UI element displays in your app.
        /// Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetOverflowBackground();
            SetOverflowImage();
        }

        /// <summary>
        /// Called when a property value is changed.
        /// </summary>
        /// <param name="pd">A property descriptor describing the property whose value has been changed.</param>
        protected virtual void OnPropertyChanged(PropertyDescriptor pd)
        {
            PropertyChanged(this, new FrameworkPropertyChangedEventArgs(pd));
        }

        private void OrganizeItems()
        {
            if (maxDisplayItems < PrimaryCommands.Count)
            {
                for (int i = PrimaryCommands.Count - 1; i >= maxDisplayItems; i--)
                {
                    var item = PrimaryCommands[i];
                    PrimaryCommands.RemoveAt(i);
                    SecondaryCommands.Insert(0, item);
                }
            }
            else if (maxDisplayItems > PrimaryCommands.Count && SecondaryCommands.Count > 0)
            {
                for (; maxDisplayItems > PrimaryCommands.Count && SecondaryCommands.Count > 0;)
                {
                    var item = SecondaryCommands[0];
                    SecondaryCommands.RemoveAt(0);
                    PrimaryCommands.Add(item);
                }
            }
        }

        private void SetOverflowBackground()
        {
            var presenter = (this.GetChild<global::Windows.UI.Xaml.Controls.Primitives.Popup>(e => e.Name == "OverflowPopup")?
                .Child as global::Windows.UI.Xaml.Controls.Panel)?.GetChild<CommandBarOverflowPresenter>();

            if (presenter != null)
            {
                presenter.Background = base.Background;
            }
        }

        private void SetOverflowImage()
        {
            var button = this.GetChild<ContentControl>(e => e.Name == "MoreButton");
            if (button != null)
            {
                if (defaultOverflowImage == null)
                {
                    defaultOverflowImage = button.Content;
                }
                
                button.Content = overflowImageUri == null ? defaultOverflowImage : new BitmapIcon() { UriSource = overflowImageUri };
            }
        }
    }
}
