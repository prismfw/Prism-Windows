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
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Prism.Native;
using Prism.UI;
using Prism.Utilities;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for a main <see cref="INativeWindow"/>.
    /// </summary>
    [Register(typeof(INativeWindow), Name = "main")]
    public class MainWindow : INativeWindow
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
        /// Occurs when the size of the window has changed.
        /// </summary>
        public event EventHandler<Prism.UI.WindowSizeChangedEventArgs> SizeChanged;

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
            set { Logger.Warn("Setting window height is not supported on this platform.  Ignoring."); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is currently visible.
        /// </summary>
        public bool IsVisible
        {
            get { return global::Windows.UI.Xaml.Window.Current.Visible; }
        }

        /// <summary>
        /// Gets the width of the window.
        /// </summary>
        public double Width
        {
            get { return global::Windows.UI.Xaml.Window.Current.CoreWindow.Bounds.Width; }
            set { Logger.Warn("Setting window width is not supported on this platform.  Ignoring."); }
        }

        private Size currentSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
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
        public void Close(Animate animate)
        {
            var args = new CancelEventArgs();
            Closing(this, args);

            if (args.Cancel)
                return;

            global::Windows.UI.Xaml.Application.Current.Exit();
        }

        /// <summary>
        /// Displays the window if it is not already visible.
        /// </summary>
        public void Show(Animate animate)
        {
            if (!global::Windows.UI.Xaml.Window.Current.Visible)
            {
                global::Windows.UI.Xaml.Window.Current.Activate();
            }
        }

        /// <summary>
        /// Captures the contents of the window in an image and returns the result.
        /// </summary>
        public async Task<Prism.UI.Media.Imaging.ImageSource> TakeScreenshotAsync()
        {
            var target = new RenderTargetBitmap();
            await target.RenderAsync(null);
            return new Prism.UI.Media.Imaging.ImageSource((await target.GetPixelsAsync()).ToArray());
        }
    }
}
