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
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for a modal <see cref="INativeWindow"/>.
    /// </summary>
    [Register(typeof(INativeWindow), Name = "modal")]
    public class ModalWindow : Grid, INativeWindow
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
        public new event EventHandler<Prism.UI.WindowSizeChangedEventArgs> SizeChanged;

        /// <summary>
        /// Gets or sets the object that acts as the content of the window.
        /// This is typically an <see cref="IView"/> or <see cref="INativeViewStack"/> instance.
        /// </summary>
        public object Content
        {
            get { return content; }
            set
            {
                content = value;
                Children.Clear();
                Children.Add(value as UIElement);
            }
        }
        private object content;

        /// <summary>
        /// Gets the height of the window.
        /// </summary>
        public new double Height
        {
            get { return popup.ActualHeight; }
            set { Logger.Warn("Setting window height is not supported on this platform.  Ignoring."); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is currently visible.
        /// </summary>
        public bool IsVisible
        {
            get { return popup.IsOpen; }
        }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value ?? string.Empty; }
        }
        private string title;

        /// <summary>
        /// Gets the width of the window.
        /// </summary>
        public new double Width
        {
            get { return popup.ActualWidth; }
            set { Logger.Warn("Setting window width is not supported on this platform.  Ignoring."); }
        }

        private readonly Popup popup;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModalWindow"/> class.
        /// </summary>
        public ModalWindow()
        {
            Background = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Colors.Black);

            popup = new Popup() { Child = this };

            popup.Opened += (o, e) =>
            {
                Activated(this, EventArgs.Empty);
            };

            popup.Closed += (o, e) =>
            {
                Deactivated(this, EventArgs.Empty);
            };

            popup.SizeChanged += (o, e) =>
            {
                SizeChanged?.Invoke(this, new Prism.UI.WindowSizeChangedEventArgs(e.PreviousSize.GetSize(), e.NewSize.GetSize()));
            };
        }

        /// <summary>
        /// Attempts to close the window.
        /// </summary>
        public void Close(Animate animate)
        {
            var args = new CancelEventArgs();
            Closing(this, args);

            if (args.Cancel)
            {
                return;
            }

            CoreApplication.MainView.CoreWindow.SizeChanged -= OnWindowSizeChanged;

            popup.IsOpen = false;
        }

        /// <summary>
        /// Displays the window if it is not already visible.
        /// </summary>
        public void Show(Animate animate)
        {
            base.Height = CoreApplication.MainView.CoreWindow.Bounds.Height;
            base.Width = CoreApplication.MainView.CoreWindow.Bounds.Width;

            CoreApplication.MainView.CoreWindow.SizeChanged -= OnWindowSizeChanged;
            CoreApplication.MainView.CoreWindow.SizeChanged += OnWindowSizeChanged;

            popup.IsOpen = true;
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

        private void OnWindowSizeChanged(CoreWindow window, global::Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            base.Width = e.Size.Width;
            base.Height = e.Size.Height;
        }
    }
}
