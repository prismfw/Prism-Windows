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
using Prism.Input;
using Prism.Native;
using Prism.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Windows.UI.Controls
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeMediaElement"/>.
    /// </summary>
    [Register(typeof(INativeMediaElement))]
    public class MediaElement : ContentControl, INativeMediaElement
    {
        /// <summary>
        /// Occurs when this instance has been attached to the visual tree and is ready to be rendered.
        /// </summary>
        public new event EventHandler Loaded;

        /// <summary>
        /// Occurs when playback of a media source has finished.
        /// </summary>
        public event EventHandler MediaEnded;

        /// <summary>
        /// Occurs when a media source has failed to open.
        /// </summary>
        public event EventHandler<ErrorEventArgs> MediaFailed;

        /// <summary>
        /// Occurs when a media source has been successfully opened.
        /// </summary>
        public event EventHandler MediaOpened;

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
        /// Occurs when a seek operation has been completed.
        /// </summary>
        public event EventHandler SeekCompleted;

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
                        Element.TransportControls.Transitions = transportTransitions;
                    }
                    else
                    {
                        transitions = Transitions;
                        contentTransitions = ContentTransitions;
                        elementTransitions = Element.Transitions;
                        transportTransitions = Element.TransportControls.Transitions;

                        Transitions = null;
                        ContentTransitions = null;
                        Element.Transitions = null;
                        Element.TransportControls.Transitions = null;
                    }

                    OnPropertyChanged(Visual.AreAnimationsEnabledProperty);
                }
            }
        }
        private bool areAnimationsEnabled = true;
        private TransitionCollection transitions, contentTransitions, elementTransitions, transportTransitions;

        /// <summary>
        /// Gets or sets a value indicating whether to show the default playback controls (play, pause, etc).
        /// </summary>
        public bool ArePlaybackControlsEnabled
        {
            get { return Element.AreTransportControlsEnabled; }
            set
            {
                if (value != Element.AreTransportControlsEnabled)
                {
                    Element.AreTransportControlsEnabled = value;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.ArePlaybackControlsEnabledProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the method to invoke when this instance requests an arrangement of its children.
        /// </summary>
        public ArrangeRequestHandler ArrangeRequest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether playback of a media source should automatically begin once buffering is finished.
        /// </summary>
        public bool AutoPlay
        {
            get { return Element.AutoPlay; }
            set
            {
                if (value != Element.AutoPlay)
                {
                    Element.AutoPlay = value;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.AutoPlayProperty);
                }
            }
        }

        /// <summary>
        /// Gets the amount that the current playback item has buffered as a value between 0.0 and 1.0.
        /// </summary>
        public double BufferingProgress
        {
            get { return Element.BufferingProgress; }
        }

        /// <summary>
        /// Gets the duration of the current playback item.
        /// </summary>
        public TimeSpan Duration { get; private set; }

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
        /// Gets or sets a value indicating whether the current media source will automatically begin playback again once it has finished.
        /// </summary>
        public bool IsLooping
        {
            get { return Element.IsLooping; }
            set
            {
                if (value != Element.IsLooping)
                {
                    Element.IsLooping = value;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.IsLoopingProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the media content is muted.
        /// </summary>
        public bool IsMuted
        {
            get { return Element.IsMuted; }
            set
            {
                if (value != Element.IsMuted)
                {
                    Element.IsMuted = value;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.IsMutedProperty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether a playback item is currently playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

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
        /// Gets or sets a coefficient of the rate at which media content is played back.  A value of 1.0 is a normal playback rate.
        /// </summary>
        public double PlaybackRate
        {
            get { return Element.PlaybackRate; }
            set
            {
                if (value != Element.PlaybackRate)
                {
                    Element.PlaybackRate = value;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.PlaybackRateProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the playback item.
        /// </summary>
        public TimeSpan Position
        {
            get { return Element.Position; }
            set { Element.Position = value; }
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
        /// Gets or sets the source of the media content to be played.
        /// </summary>
        public object Source
        {
            get { return source; }
            set
            {
                if (value != source)
                {
                    source = value;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.SourceProperty);

                    var playbackSource = ((source as Windows.Media.IMediaPlaybackSource)?.Source ?? source) as global::Windows.Media.Playback.IMediaPlaybackSource;
                    if (playbackSource == null)
                    {
                        Element.Source = null;
                    }
                    else
                    {
                        Element.SetPlaybackSource(playbackSource);
                    }
                }
            }
        }
        private object source;

        /// <summary>
        /// Gets or sets the manner in which video content is stretched within its allocated space.
        /// </summary>
        public Stretch Stretch
        {
            get { return Element.Stretch.GetStretch(); }
            set
            {
                var stretch = value.GetStretch();
                if (stretch != Element.Stretch)
                {
                    Element.Stretch = stretch;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.StretchProperty);
                }
            }
        }

        /// <summary>
        /// Gets the size of the video content, or Size.Empty if there is no video content.
        /// </summary>
        public Size VideoSize { get; private set; }

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

        /// <summary>
        /// Gets or sets the volume of the media content as a range between 0.0 (silent) and 1.0 (full).
        /// </summary>
        public double Volume
        {
            get { return Element.Volume; }
            set
            {
                if (value != Element.Volume)
                {
                    Element.Volume = value;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.VolumeProperty);
                }
            }
        }

        /// <summary>
        /// Gets the UI element that is displaying the text.
        /// </summary>
        protected global::Windows.UI.Xaml.Controls.MediaElement Element { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaElement"/> class.
        /// </summary>
        public MediaElement()
        {
            RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);
            Content = Element = new global::Windows.UI.Xaml.Controls.MediaElement();

            Element.BufferingProgressChanged += (o, e) =>
            {
                OnPropertyChanged(Prism.UI.Controls.MediaElement.BufferingProgressProperty);
            };

            Element.CurrentStateChanged += (o, e) =>
            {
                var isPlaying = Element.CurrentState == global::Windows.UI.Xaml.Media.MediaElementState.Playing;
                if (isPlaying != IsPlaying)
                {
                    IsPlaying = isPlaying;
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.IsPlayingProperty);
                }
            };

            Element.MediaEnded += (o, e) =>
            {
                MediaEnded(this, EventArgs.Empty);
            };

            Element.MediaFailed += (o, e) =>
            {
                MediaFailed(this, new ErrorEventArgs(new Exception(e.ErrorMessage)));
            };

            Element.MediaOpened += (o, e) =>
            {
                MediaOpened(this, EventArgs.Empty);
            };

            Element.SeekCompleted += (o, e) =>
            {
                SeekCompleted(this, EventArgs.Empty);
            };

            Element.RegisterPropertyChangedCallback(global::Windows.UI.Xaml.Controls.MediaElement.NaturalDurationProperty, (o, e) =>
            {
                var element = o as global::Windows.UI.Xaml.Controls.MediaElement;
                if (element != null)
                {
                    var duration = element.NaturalDuration.HasTimeSpan ? element.NaturalDuration.TimeSpan : TimeSpan.Zero;
                    if (duration != Duration)
                    {
                        Duration = duration;
                        OnPropertyChanged(Prism.UI.Controls.MediaElement.DurationProperty);
                    }
                }
            });

            Element.RegisterPropertyChangedCallback(global::Windows.UI.Xaml.Controls.MediaElement.IsAudioOnlyProperty, OnDependencyPropertyChanged);
            Element.RegisterPropertyChangedCallback(global::Windows.UI.Xaml.Controls.MediaElement.NaturalVideoHeightProperty, OnDependencyPropertyChanged);
            Element.RegisterPropertyChangedCallback(global::Windows.UI.Xaml.Controls.MediaElement.NaturalVideoWidthProperty, OnDependencyPropertyChanged);

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
                Element.Width = double.NaN;
                Element.Height = double.NaN;

                base.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                Element.Measure(constraints.GetSize());
                return Element.DesiredSize.GetSize();
            }
            finally
            {
                base.Visibility = visibility == Prism.UI.Visibility.Visible ?
                    global::Windows.UI.Xaml.Visibility.Visible : global::Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Pauses playback of the current media source.
        /// </summary>
        public void PausePlayback()
        {
            Element.Pause();
        }

        /// <summary>
        /// Starts or resumes playback of the current media source.
        /// </summary>
        public void StartPlayback()
        {
            Element.Play();
        }

        /// <summary>
        /// Stops playback of the current media source.
        /// </summary>
        public void StopPlayback()
        {
            Element.Stop();
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
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

        private void OnDependencyPropertyChanged(DependencyObject o, DependencyProperty dp)
        {
            var element = o as global::Windows.UI.Xaml.Controls.MediaElement;
            if (element != null)
            {
                if (element.IsAudioOnly)
                {
                    if (VideoSize != Size.Empty)
                    {
                        VideoSize = Size.Empty;
                        OnPropertyChanged(Prism.UI.Controls.MediaElement.VideoSizeProperty);
                    }
                }
                else if (element.NaturalVideoWidth != VideoSize.Width || element.NaturalVideoHeight != VideoSize.Height)
                {
                    VideoSize = new Size(element.NaturalVideoWidth, element.NaturalVideoHeight);
                    OnPropertyChanged(Prism.UI.Controls.MediaElement.VideoSizeProperty);
                }
            }
        }
    }
}
