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
using Prism.Native;
using Windows.Media.Playback;

namespace Prism.Windows.Media
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeAudioPlayer"/>.
    /// </summary>
    [Register(typeof(INativeAudioPlayer))]
    public class AudioPlayer : INativeAudioPlayer
    {
        /// <summary>
        /// Occurs when there is an error during loading or playing of the audio track.
        /// </summary>
        public event EventHandler<ErrorEventArgs> AudioFailed;

        /// <summary>
        /// Occurs when buffering of the audio track has finished.
        /// </summary>
        public event EventHandler BufferingEnded;

        /// <summary>
        /// Occurs when buffering of the audio track has begun.
        /// </summary>
        public event EventHandler BufferingStarted;

        /// <summary>
        /// Occurs when playback of the audio track has finished.
        /// </summary>
        public event EventHandler PlaybackEnded;

        /// <summary>
        /// Occurs when playback of the audio track has begun.
        /// </summary>
        public event EventHandler PlaybackStarted;

        /// <summary>
        /// Gets or sets a value indicating whether playback of the audio track should automatically begin once buffering is finished.
        /// </summary>
        public bool AutoPlay
        {
            get { return BackgroundMediaPlayer.Current.AutoPlay; }
            set { BackgroundMediaPlayer.Current.AutoPlay = value; }
        }

        /// <summary>
        /// Gets the duration of the audio track.
        /// </summary>
        public TimeSpan Duration
        {
            get { return BackgroundMediaPlayer.Current.NaturalDuration; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the audio track will automatically begin playing again once it has finished.
        /// </summary>
        public bool IsLooping
        {
            get { return BackgroundMediaPlayer.Current.IsLoopingEnabled; }
            set { BackgroundMediaPlayer.Current.IsLoopingEnabled = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the audio track is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing; }
        }

        /// <summary>
        /// Gets or sets a coefficient of the rate at which the audio track is played back.
        /// </summary>
        public double PlaybackRate
        {
            get { return BackgroundMediaPlayer.Current.PlaybackRate; }
            set { BackgroundMediaPlayer.Current.PlaybackRate = value; }
        }

        /// <summary>
        /// Gets or sets the position of the audio track.
        /// </summary>
        public TimeSpan Position
        {
            get { return BackgroundMediaPlayer.Current.Position; }
            set { BackgroundMediaPlayer.Current.Position = value; }
        }

        /// <summary>
        /// Gets or sets the volume of the audio track as a range between 0.0 (silent) and 1.0 (full).
        /// </summary>
        public double Volume
        {
            get { return BackgroundMediaPlayer.Current.Volume; }
            set { BackgroundMediaPlayer.Current.Volume = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPlayer"/> class.
        /// </summary>
        public AudioPlayer()
        {
            BackgroundMediaPlayer.Current.BufferingEnded += (o, e) => BufferingEnded(this, EventArgs.Empty);

            BackgroundMediaPlayer.Current.BufferingStarted += (o, e) => BufferingStarted(this, EventArgs.Empty);

            BackgroundMediaPlayer.Current.CurrentStateChanged += (o, e) =>
            {
                if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
                {
                    PlaybackStarted(this, EventArgs.Empty);
                }
            };

            BackgroundMediaPlayer.Current.MediaEnded += (o, e) => PlaybackEnded(this, EventArgs.Empty);

            BackgroundMediaPlayer.Current.MediaFailed += (o, e) => AudioFailed(this, new ErrorEventArgs(e.ExtendedErrorCode));
        }

        /// <summary>
        /// Loads the audio track from the file at the specified location.
        /// </summary>
        /// <param name="source">The URI of the source file for the audio track.</param>
        public void Open(Uri source)
        {
            BackgroundMediaPlayer.Current.SetUriSource(source);
        }

        /// <summary>
        /// Pauses playback of the audio track.
        /// </summary>
        public void Pause()
        {
            BackgroundMediaPlayer.Current.Pause();
        }

        /// <summary>
        /// Starts or resumes playback of the audio track.
        /// </summary>
        public void Play()
        {
            BackgroundMediaPlayer.Current.Play();
        }
    }
}
