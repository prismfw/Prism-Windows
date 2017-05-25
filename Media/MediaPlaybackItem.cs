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
using System.Collections.ObjectModel;
using Prism.Media;
using Prism.Native;
using Windows.Foundation.Collections;
using Windows.Media.Core;

namespace Prism.Windows.Media
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeMediaPlaybackItem"/>.
    /// </summary>
    [Register(typeof(INativeMediaPlaybackItem))]
    public class MediaPlaybackItem : INativeMediaPlaybackItem, IMediaPlaybackSource
    {
        /// <summary>
        /// Gets the duration of the playback item.
        /// </summary>
        public TimeSpan Duration
        {
            get { return source.Source.Duration.HasValue ? source.Source.Duration.Value : TimeSpan.Zero; }
        }

        /// <summary>
        /// Gets a collection of the individual media tracks that contain the playback data.
        /// </summary>
        public ReadOnlyCollection<MediaTrack> Tracks
        {
            get
            {
                if (tracks == null)
                {
                    var trackArray = new MediaTrack[source.AudioTracks.Count + source.VideoTracks.Count + source.TimedMetadataTracks.Count];
                    for (int i = 0; i < trackArray.Length; i++)
                    {
                        if (i < source.AudioTracks.Count)
                        {
                            var audioTrack = source.AudioTracks[i];
                            trackArray[i] = new MediaTrack(audioTrack.Label ?? audioTrack.Id, MediaTrackType.Audio, audioTrack.Language);
                        }
                        else if (i < source.AudioTracks.Count + source.VideoTracks.Count)
                        {
                            var videoTrack = source.VideoTracks[i - source.AudioTracks.Count];
                            trackArray[i] = new MediaTrack(videoTrack.Label ?? videoTrack.Id, MediaTrackType.Video, videoTrack.Language);
                        }
                        else
                        {
                            var timedTrack = source.TimedMetadataTracks[i - (source.AudioTracks.Count + source.VideoTracks.Count)];
                            trackArray[i] = new MediaTrack(timedTrack.Label ?? timedTrack.Id, MediaTrackType.TimedMetadata, timedTrack.Language);
                        }
                    }

                    tracks = new ReadOnlyCollection<MediaTrack>(trackArray);
                }

                return tracks;
            }
        }
        private ReadOnlyCollection<MediaTrack> tracks;

        /// <summary>
        /// Gets the URI of the playback item.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the media playback source object.
        /// </summary>
        public global::Windows.Media.Playback.IMediaPlaybackSource Source
        {
            get { return source; }
        }
        private readonly global::Windows.Media.Playback.MediaPlaybackItem source;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlaybackItem"/> class.
        /// </summary>
        /// <param name="uri">The URI of the playback item.</param>
        public MediaPlaybackItem(Uri uri)
        {
            source = new global::Windows.Media.Playback.MediaPlaybackItem(MediaSource.CreateFromUri(uri));
            Uri = uri;

            source.AudioTracksChanged += OnTracksChanged;
            source.TimedMetadataTracksChanged += OnTracksChanged;
            source.VideoTracksChanged += OnTracksChanged;
        }

        /// <summary>
        /// Releases any resources that are being used by the playback item.
        /// </summary>
        public void Dispose()
        {
        }

        private void OnTracksChanged(global::Windows.Media.Playback.MediaPlaybackItem o, IVectorChangedEventArgs e)
        {
            tracks = null;
        }
    }
}
