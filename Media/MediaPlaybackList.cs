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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Prism.Native;

namespace Prism.Windows.Media
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeMediaPlaybackList"/>.
    /// </summary>
    [Register(typeof(INativeMediaPlaybackList))]
    public class MediaPlaybackList : INativeMediaPlaybackList, IMediaPlaybackSource
    {
        /// <summary>
        /// Occurs when the currently playing item has changed.
        /// </summary>
        public event EventHandler<NativeItemChangedEventArgs> CurrentItemChanged;

        /// <summary>
        /// Occurs when a playback item has failed to open.
        /// </summary>
        public event EventHandler<NativeErrorEventArgs> ItemFailed;

        /// <summary>
        /// Occurs when a playback item has been successfully opened.
        /// </summary>
        public event EventHandler<NativeItemEventArgs> ItemOpened;

        /// <summary>
        /// Gets the zero-based index of the current item in the <see cref="Items"/> collection.
        /// </summary>
        public int CurrentItemIndex
        {
            get { return (int)source.CurrentItemIndex; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the playlist should automatically restart after the last item has finished playing.
        /// </summary>
        public bool IsRepeatEnabled
        {
            get { return source.AutoRepeatEnabled; }
            set { source.AutoRepeatEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items in the playlist should be played in random order.
        /// </summary>
        public bool IsShuffleEnabled
        {
            get { return source.ShuffleEnabled; }
            set { source.ShuffleEnabled = value; }
        }

        /// <summary>
        /// Gets a collection of playback items that make up the playlist.
        /// </summary>
        public IList Items { get; }

        /// <summary>
        /// Gets the media playback source object.
        /// </summary>
        public global::Windows.Media.Playback.IMediaPlaybackSource Source
        {
            get { return source; }
        }
        private readonly global::Windows.Media.Playback.MediaPlaybackList source;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlaybackList"/> class.
        /// </summary>
        public MediaPlaybackList()
        {
            source = new global::Windows.Media.Playback.MediaPlaybackList();
            
            source.CurrentItemChanged += (o, e) => CurrentItemChanged(this, new NativeItemChangedEventArgs(GetPlaybackItem(e.OldItem), GetPlaybackItem(e.NewItem)));
            source.ItemFailed += (o, e) => ItemFailed(this, new NativeErrorEventArgs(GetPlaybackItem(e.Item), e.Error.ExtendedError));
            source.ItemOpened += (o, e) => ItemOpened(this, new NativeItemEventArgs(GetPlaybackItem(e.Item)));

            Items = new ObservableCollection<MediaPlaybackItem>();
            ((INotifyCollectionChanged)Items).CollectionChanged += (o, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            source.Items.Insert(e.NewStartingIndex + i, GetPlaybackItem(e.NewItems[i] as IMediaPlaybackSource));
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            source.Items.RemoveAt(e.OldStartingIndex);
                        }

                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            source.Items.Insert(e.NewStartingIndex + i, GetPlaybackItem(e.NewItems[i] as IMediaPlaybackSource));
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            source.Items.RemoveAt(e.OldStartingIndex);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            source.Items[e.NewStartingIndex + i] = GetPlaybackItem(e.NewItems[i] as IMediaPlaybackSource);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        source.Items.Clear();
                        break;
                }
            };
        }

        /// <summary>
        /// Moves to the next item in the playlist.
        /// </summary>
        public void MoveNext()
        {
            source.MoveNext();
        }

        /// <summary>
        /// Moves to the previous item in the playlist.
        /// </summary>
        public void MovePrevious()
        {
            source.MovePrevious();
        }

        /// <summary>
        /// Moves to the item in the playlist that is located at the specified index.
        /// </summary>
        /// <param name="itemIndex">The zero-based index of the item to move to.</param>
        public void MoveTo(int itemIndex)
        {
            source.MoveTo((uint)itemIndex);
        }

        private object GetPlaybackItem(global::Windows.Media.Playback.MediaPlaybackItem item)
        {
            if (item != null)
            {
                foreach (var playbackItem in Items)
                {
                    if (item == playbackItem || (playbackItem as IMediaPlaybackSource)?.Source == item)
                    {
                        return playbackItem;
                    }
                }
            }

            return null;
        }

        private global::Windows.Media.Playback.MediaPlaybackItem GetPlaybackItem(IMediaPlaybackSource item)
        {
            return item?.Source as global::Windows.Media.Playback.MediaPlaybackItem;
        }
    }
}
