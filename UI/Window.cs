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
using Prism.Native;
using Prism.UI;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

using DisplayOrientations = Windows.Graphics.Display.DisplayOrientations;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeWindow"/>.
    /// </summary>
    [Register(typeof(INativeWindow))]
    public class Window : INativeWindow
    {
        /// <summary>
        /// Occurs when the window is brought to the foreground.
        /// </summary>
        public event EventHandler Activated;

        /// <summary>
        /// Occurs when the <see cref="M:Close"/> method is called or a user uses the back button to back out of the application.
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        /// <summary>
        /// Occurs when the window is pushed to the background.
        /// </summary>
        public event EventHandler Deactivated;

        /// <summary>
        /// Occurs when the orientation of the rendered content has changed.
        /// </summary>
        public event EventHandler<DisplayOrientationChangedEventArgs> OrientationChanged;

        /// <summary>
        /// Occurs when the size of the window has changed.
        /// </summary>
        public event EventHandler<Prism.UI.WindowSizeChangedEventArgs> SizeChanged;

        /// <summary>
        /// Gets or sets the preferred orientations in which to automatically rotate the window in response to orientation changes of the physical device.
        /// </summary>
        public Prism.UI.DisplayOrientations AutorotationPreferences
        {
            get { return DisplayInformation.AutoRotationPreferences.GetDisplayOrientations(); }
            set { DisplayInformation.AutoRotationPreferences = value.GetDisplayOrientations(); }
        }

        /// <summary>
        /// Gets or sets the object that acts as the content of the window.
        /// </summary>
        public object Content
        {
            get { return global::Windows.UI.Xaml.Window.Current.Content; }
            set { global::Windows.UI.Xaml.Window.Current.Content = value as UIElement; }
        }

        /// <summary>
        /// Gets the height of the window.
        /// </summary>
        public double Height
        {
            get { return global::Windows.UI.Xaml.Window.Current.Bounds.Height; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is currently visible.
        /// </summary>
        public bool IsVisible
        {
            get { return global::Windows.UI.Xaml.Window.Current.Visible; }
        }

        /// <summary>
        /// Gets the current orientation of the rendered content within the window.
        /// </summary>
        public Prism.UI.DisplayOrientations Orientation
        {
            get { return DisplayInformation.GetForCurrentView().CurrentOrientation.GetDisplayOrientations(); }
        }

        /// <summary>
        /// Gets or sets the style for the window.
        /// </summary>
        public WindowStyle Style
        {
            get
            {
                if (ApplicationView.GetForCurrentView().IsFullScreenMode)
                {
                    return WindowStyle.FullScreen;
                }

                return CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar ? WindowStyle.Chromeless : WindowStyle.Normal;
            }
            set
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = value == WindowStyle.Chromeless;

                var view = ApplicationView.GetForCurrentView();
                if (value == WindowStyle.FullScreen && !view.IsFullScreenMode)
                {
                    view.TryEnterFullScreenMode();
                }
                else if (value != WindowStyle.FullScreen && view.IsFullScreenMode)
                {
                    view.ExitFullScreenMode();
                }
            }
        }

        /// <summary>
        /// Gets the width of the window.
        /// </summary>
        public double Width
        {
            get { return global::Windows.UI.Xaml.Window.Current.CoreWindow.Bounds.Width; }
        }

        private Size currentSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window()
        {
            global::Windows.UI.Xaml.Window.Current.CoreWindow.Activated += (o, e) =>
            {
                if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
                {
                    Deactivated(this, EventArgs.Empty);
                }
                else
                {
                    Activated(this, EventArgs.Empty);
                }
            };

            DisplayInformation.GetForCurrentView().OrientationChanged += (o, e) =>
            {
                OrientationChanged(this, new DisplayOrientationChangedEventArgs(o.CurrentOrientation.GetDisplayOrientations()));
            };

            global::Windows.UI.Xaml.Window.Current.CoreWindow.SizeChanged += (o, e) =>
            {
                SizeChanged(this, new Prism.UI.WindowSizeChangedEventArgs(currentSize, e.Size.GetSize()));
                currentSize.Width = e.Size.Width;
                currentSize.Height = e.Size.Height;
            };

            currentSize = new Size(global::Windows.UI.Xaml.Window.Current.CoreWindow.Bounds.Width, global::Windows.UI.Xaml.Window.Current.CoreWindow.Bounds.Height);
        }

        /// <summary>
        /// Attempts to close the window.
        /// </summary>
        public void Close()
        {
            var args = new CancelEventArgs();
            Closing(this, args);

            if (args.Cancel)
                return;

            global::Windows.UI.Xaml.Application.Current.Exit();
        }

        /// <summary>
        /// Sets the preferred minimum size of the window.
        /// </summary>
        /// <param name="minSize">The preferred minimum size.</param>
        public void SetPreferredMinSize(Size minSize)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(minSize.GetSize());
        }

        /// <summary>
        /// Displays the window if it is not already visible.
        /// </summary>
        public void Show()
        {
            if (!global::Windows.UI.Xaml.Window.Current.Visible)
            {
                global::Windows.UI.Xaml.Window.Current.Activate();
            }
        }

        /// <summary>
        /// Attempts to resize the window to the specified size.
        /// </summary>
        /// <param name="newSize">The width and height at which to size the window.</param>
        /// <returns><c>true</c> if the window was successfully resized; otherwise, <c>false</c>.</returns>
        public bool TryResize(Size newSize)
        {
            return ApplicationView.GetForCurrentView().TryResizeView(newSize.GetSize());
        }
    }
}
