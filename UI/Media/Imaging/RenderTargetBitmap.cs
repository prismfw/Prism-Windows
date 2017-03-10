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
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Prism.UI.Media.Imaging;
using Prism.Native;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.Storage.Streams;
using Windows.Graphics.Display;

namespace Prism.Windows.UI.Media.Imaging
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeRenderTargetBitmap"/>.
    /// </summary>
    [Register(typeof(INativeRenderTargetBitmap))]
    public class RenderTargetBitmap : INativeRenderTargetBitmap, IImageSource
    {
        /// <summary>
        /// Gets the number of pixels along the image's Y-axis.
        /// </summary>
        public int PixelHeight
        {
            get { return renderTarget.PixelHeight; }
        }

        /// <summary>
        /// Gets the number of pixels along the image's X-axis.
        /// </summary>
        public int PixelWidth
        {
            get { return renderTarget.PixelWidth; }
        }

        /// <summary>
        /// Gets the scaling factor of the image.
        /// </summary>
        public double Scale
        {
            get { return DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel; }
        }

        /// <summary>
        /// Gets the image source instance.
        /// </summary>
        public global::Windows.UI.Xaml.Media.ImageSource Source
        {
            get { return renderTarget; }
        }

        private readonly global::Windows.UI.Xaml.Media.Imaging.RenderTargetBitmap renderTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetBitmap"/> class.
        /// </summary>
        public RenderTargetBitmap()
        {
            renderTarget = new global::Windows.UI.Xaml.Media.Imaging.RenderTargetBitmap();
        }

        /// <summary>
        /// Gets the data for the captured image as a byte array.
        /// </summary>
        /// <returns>The image data as an <see cref="Array"/> of bytes.</returns>
        public async Task<byte[]> GetPixelsAsync()
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)renderTarget.PixelWidth, (uint)renderTarget.PixelHeight,
                    96, 96, (await renderTarget.GetPixelsAsync()).ToArray());

                await encoder.FlushAsync();
                
                byte[] pixels = new byte[stream.Size];
                await stream.ReadAsync(pixels.AsBuffer(), (uint)stream.Size, InputStreamOptions.None);
                return pixels;
            }
        }

        /// <summary>
        /// Renders a snapshot of the specified visual object.
        /// </summary>
        /// <param name="target">The visual object to render.    This value can be <c>null</c> to render the entire visual tree.</param>
        /// <param name="width">The width of the snapshot.</param>
        /// <param name="height">The height of the snapshot.</param>
        public async Task RenderAsync(INativeVisual target, int width, int height)
        {
            await renderTarget.RenderAsync((UIElement)target, width, height);
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

            file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encorder = await BitmapEncoder.CreateAsync(fileFormat == ImageFileFormat.Jpeg ? BitmapEncoder.JpegEncoderId : BitmapEncoder.PngEncoderId, stream);
                encorder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)renderTarget.PixelWidth, (uint)renderTarget.PixelHeight,
                    96, 96, (await renderTarget.GetPixelsAsync()).ToArray());
                await encorder.FlushAsync();

                await stream.FlushAsync();
            }
        }
    }
}
