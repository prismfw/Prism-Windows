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
using Windows.System;
using Windows.UI.Core;

namespace Prism.Windows
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeApplication"/>.
    /// </summary>
    [Register(typeof(INativeApplication), IsSingleton = true)]
    public class Application : INativeApplication
    {
        /// <summary>
        /// Occurs when the application is shutting down.
        /// </summary>
        public event EventHandler Exiting;

        /// <summary>
        /// Occurs when the application is resuming from suspension.
        /// </summary>
        public event EventHandler Resuming;

        /// <summary>
        /// Occurs when the application is suspending.
        /// </summary>
        public event EventHandler Suspending;

        /// <summary>
        /// Occurs when an unhandled exception is encountered.
        /// </summary>
        public event EventHandler<ErrorEventArgs> UnhandledException;

        /// <summary>
        /// Gets the default theme that is used by the application.
        /// </summary>
        public Theme DefaultTheme
        {
            get { return global::Windows.UI.Xaml.Application.Current.RequestedTheme == global::Windows.UI.Xaml.ApplicationTheme.Dark ? Theme.Dark : Theme.Light; }
        }

        /// <summary>
        /// Gets the platform on which the application is running.
        /// </summary>
        public Platform Platform
        {
            get { return Platform.UniversalWindows; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public Application()
        {
            CoreApplication.Exiting += (o, e) => Exiting(this, EventArgs.Empty);
            CoreApplication.Resuming += (o, e) => Resuming(this, EventArgs.Empty);
            CoreApplication.Suspending += (o, e) =>
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                Suspending(this, EventArgs.Empty);
                deferral.Complete();
            };
            
            global::Windows.UI.Xaml.Application.Current.UnhandledException += (o, e) => UnhandledException(this, new ErrorEventArgs(e.Exception));
        }

        /// <summary>
        /// Signals the system to begin ignoring any user interactions within the application.
        /// </summary>
        public void BeginIgnoringUserInput()
        {
            CoreApplication.MainView.CoreWindow.IsInputEnabled = false;
        }

        /// <summary>
        /// Asynchronously invokes the specified delegate on the platform's main thread.
        /// </summary>
        /// <param name="action">The action to invoke on the main thread.</param>
        public async void BeginInvokeOnMainThread(Action action)
        {
            if (CoreApplication.MainView != null)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action.Invoke());
            }
        }

        /// <summary>
        /// Asynchronously invokes the specified delegate on the platform's main thread.
        /// </summary>
        /// <param name="d">A delegate to a method that takes multiple parameters.</param>
        /// <param name="parameters">The parameters for the delegate method.</param>
        public async void BeginInvokeOnMainThreadWithParameters(Delegate d, params object[] parameters)
        {
            if (CoreApplication.MainView != null)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => d.DynamicInvoke(parameters));
            }
        }

        /// <summary>
        /// Signals the system to stop ignoring user interactions within the application.
        /// </summary>
        public void EndIgnoringUserInput()
        {
            CoreApplication.MainView.CoreWindow.IsInputEnabled = true;
        }

        /// <summary>
        /// Launches the specified URL in an external application, most commonly a web browser.
        /// </summary>
        /// <param name="url">The URL to launch to.</param>
        public async void LaunchUrl(Uri url)
        {
            await Launcher.LaunchUriAsync(url);
        }
    }
}
