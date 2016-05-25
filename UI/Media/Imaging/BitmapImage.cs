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
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Prism.UI.Media.Imaging;
using Prism.Native;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Prism.Windows.UI.Media.Imaging
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeBitmapImage"/>.
    /// </summary>
    [Register(typeof(INativeBitmapImage))]
    public class BitmapImage : INativeBitmapImage, IImageSource
    {
        /// <summary>
        /// Occurs when the image fails to load.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ImageFailed;

        /// <summary>
        /// Occurs when the image has been loaded into memory.
        /// </summary>
        public event EventHandler ImageLoaded;

        /// <summary>
        /// Gets a value indicating whether the image has encountered an error during loading.
        /// </summary>
        public bool IsFaulted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image has been loaded into memory.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets the number of pixels along the image's Y-axis.
        /// </summary>
        public int PixelHeight
        {
            get { return bitmapImage.PixelHeight; }
        }

        /// <summary>
        /// Gets the number of pixels along the image's X-axis.
        /// </summary>
        public int PixelWidth
        {
            get { return bitmapImage.PixelWidth; }
        }

        /// <summary>
        /// Gets the image source instance.
        /// </summary>
        public global::Windows.UI.Xaml.Media.ImageSource Source
        {
            get { return bitmapImage; }
        }

        /// <summary>
        /// Gets the URI of the source file containing the image data.
        /// </summary>
        public Uri SourceUri { get; private set; }

        private readonly global::Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapImage"/> class.
        /// </summary>
        /// <param name="sourceUri">The URI of the source file containing the image data.</param>
        /// <param name="cachedImage">The image that was pulled from the image cache, or <c>null</c> if nothing was pulled from the cache.</param>
        public BitmapImage(Uri sourceUri, INativeBitmapImage cachedImage)
        {
            SourceUri = sourceUri;

            var cached = cachedImage as BitmapImage;
            if (cached != null)
            {
                bitmapImage = cached.bitmapImage;
                IsLoaded = cached.IsLoaded;
            }

            if (bitmapImage == null)
            {
                bitmapImage = new global::Windows.UI.Xaml.Media.Imaging.BitmapImage();
            }

            bitmapImage.ImageOpened -= OnImageLoaded;
            bitmapImage.ImageOpened += OnImageLoaded;
            bitmapImage.ImageFailed -= OnImageFailed;
            bitmapImage.ImageFailed += OnImageFailed;
            bitmapImage.UriSource = sourceUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapImage"/> class.
        /// </summary>
        /// <param name="imageData">The byte array containing the data for the image.</param>
        public BitmapImage(byte[] imageData)
        {
            bitmapImage = new global::Windows.UI.Xaml.Media.Imaging.BitmapImage();

            bitmapImage.ImageOpened -= OnImageLoaded;
            bitmapImage.ImageOpened += OnImageLoaded;
            bitmapImage.ImageFailed -= OnImageFailed;
            bitmapImage.ImageFailed += OnImageFailed;
            
            // attempting to wait on this causes a deadlock
            var task = bitmapImage.SetSourceAsync(imageData.AsBuffer().AsStream().AsRandomAccessStream());
        }

        /// <summary>
        /// Saves the image data to a file at the specified path using the specified file format.
        /// </summary>
        /// <param name="filePath">The path to the file in which to save the image data.</param>
        /// <param name="fileFormat">The file format in which to save the image data.</param>
        public async Task SaveAsync(string filePath, ImageFileFormat fileFormat)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            StorageFolder folder = null;
            StorageFile file = null;

            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }
            catch { }

            if (folder == null)
            {
                await Prism.IO.Directory.CreateAsync(directoryPath);
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }

            // BitmapImage appears to be write-only, so we can't read the pixel data in order to write it to disk.
            // To get around this, we're rendering the image to a RenderTargetBitmap, which lets us get at the pixels.
            // However, RenderTargetBitmap only works if it's a part of the view hierarchy.
            // So, we set the MainWindow's content to the bitmap, render it, and then reset the content, hopefully fast enough that no one notices.
            // If anyone knows of a better way, please let us know!
            var content = Prism.UI.Window.Current.Content;
            try
            {
                file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var image = new global::Windows.UI.Xaml.Controls.Image() { Source = bitmapImage };

                    Prism.UI.Window.Current.Content = image;

                    var bitmap = new global::Windows.UI.Xaml.Media.Imaging.RenderTargetBitmap();
                    await bitmap.RenderAsync(image, bitmapImage.PixelWidth, bitmapImage.PixelHeight);

                    // Reset the content as soon as possible.
                    Prism.UI.Window.Current.Content = content;

                    var pixels = await bitmap.GetPixelsAsync();

                    var encorder = await BitmapEncoder.CreateAsync(fileFormat == ImageFileFormat.Jpeg ? BitmapEncoder.JpegEncoderId : BitmapEncoder.PngEncoderId, stream);
                    encorder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmapImage.PixelWidth, (uint)bitmapImage.PixelHeight,
                        96, 96, pixels.ToArray());
                    await encorder.FlushAsync();

                    await stream.FlushAsync();
                }
            }
            finally
            {
                // This is to ensure that the content is reset even if an error is thrown.
                if (Prism.UI.Window.Current.Content != content)
                {
                    Prism.UI.Window.Current.Content = content;
                }
            }
        }

        /// <summary>
        /// Raises the image failed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The exception that describes the failure.</param>
        protected void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            IsFaulted = true;
            ImageFailed(this, new ErrorEventArgs(new Exception(e.ErrorMessage)));
        }

        /// <summary>
        /// Raises the image loaded event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected void OnImageLoaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;
            ImageLoaded(this, EventArgs.Empty);
        }
    }
}
