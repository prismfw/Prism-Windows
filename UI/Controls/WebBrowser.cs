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
using Prism.UI.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Canvas = Windows.UI.Xaml.Controls.Canvas;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeWebBrowser"/>.
    /// </summary>
    [Register(typeof(INativeWebBrowser))]
    public class WebBrowser : ContentControl, INativeWebBrowser
    {
        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when the web browser has finished loading the document.
        /// </summary>
        public event EventHandler<WebNavigationCompletedEventArgs> NavigationCompleted;

        /// <summary>
        /// Occurs when the web browser has begun navigating to a document.
        /// </summary>
        public event EventHandler<WebNavigationStartingEventArgs> NavigationStarting;

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
        /// Occurs when a script invocation has completed.
        /// </summary>
        public event EventHandler<WebScriptCompletedEventArgs> ScriptCompleted;

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
        /// Gets a value indicating whether the web browser has at least one document in its back navigation history.
        /// </summary>
        public bool CanGoBack { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the web browser has at least one document in its forward navigation history.
        /// </summary>
        public bool CanGoForward { get; private set; }

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
        /// Gets the title of the current document.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the URI of the current document.
        /// </summary>
        public Uri Uri { get; private set; }

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
        /// Gets the UI element that is displaying the HTML content.
        /// </summary>
        protected WebView Element { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBrowser"/> class.
        /// </summary>
        public WebBrowser()
        {
            Content = Element = new WebView()
            {
                HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch
            };

            Element.NavigationCompleted += (o, e) =>
            {
                if (CanGoBack != o.CanGoBack)
                {
                    CanGoBack = o.CanGoBack;
                    OnPropertyChanged(Prism.UI.Controls.WebBrowser.CanGoBackProperty);
                }

                if (CanGoForward != o.CanGoForward)
                {
                    CanGoForward = o.CanGoForward;
                    OnPropertyChanged(Prism.UI.Controls.WebBrowser.CanGoForwardProperty);
                }

                if (Uri != e.Uri)
                {
                    Uri = e.Uri;
                    OnPropertyChanged(Prism.UI.Controls.WebBrowser.UriProperty);
                }

                if (Title != o.DocumentTitle)
                {
                    Title = o.DocumentTitle;
                    OnPropertyChanged(Prism.UI.Controls.WebBrowser.TitleProperty);
                }

                NavigationCompleted(this, new WebNavigationCompletedEventArgs(e.Uri));
            };

            Element.NavigationFailed += (o, e) =>
            {
                Prism.Utilities.Logger.Error(string.Format("Web navigation failed with error code {0}.", (int)e.WebErrorStatus));
            };

            Element.NavigationStarting += (o, e) =>
            {
                var args = new WebNavigationStartingEventArgs(e.Uri);
                NavigationStarting(this, args);
                e.Cancel = args.Cancel;
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

            base.Unloaded += (o, e) =>
            {
                IsLoaded = false;
                OnPropertyChanged(Visual.IsLoadedProperty);
                Unloaded(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// Navigates to the previous document in the navigation history.
        /// </summary>
        public void GoBack()
        {
            Element.GoBack();
        }

        /// <summary>
        /// Navigates to the next document in the navigation history.
        /// </summary>
        public void GoForward()
        {
            Element.GoForward();
        }

        /// <summary>
        /// Executes a script function that is implemented by the current document.
        /// </summary>
        /// <param name="scriptName">The name of the script function to execute.</param>
        public async void InvokeScript(string scriptName)
        {
            string result = await Element.InvokeScriptAsync(scriptName, null);
            ScriptCompleted(this, new WebScriptCompletedEventArgs(result));
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
        /// Navigates to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The URI to navigate to.</param>
        public void Navigate(Uri uri)
        {
            Element.Navigate(uri);
        }

        /// <summary>
        /// Navigates to the specified <see cref="String"/> containing the content for a document.
        /// </summary>
        /// <param name="html">The string containing the content for a document.</param>
        public void NavigateToString(string html)
        {
            Element.NavigateToString(html);
        }

        /// <summary>
        /// Reloads the current document.
        /// </summary>
        public void Refresh()
        {
            Element.Refresh();
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
    }
}
