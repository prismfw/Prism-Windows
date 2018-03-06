/*
Copyright (C) 2018  Prism Framework Team

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

namespace Prism.Windows.UI.Media.Imaging
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeWritableBitmap"/>.
    /// </summary>
    [Register(typeof(INativeWritableBitmap))]
    public class WritableBitmap : INativeWritableBitmap, IImageSource
    {
        /// <summary>
        /// Gets the number of pixels along the image's Y-axis.
        /// </summary>
        public int PixelHeight
        {
            get { return bitmap.PixelHeight; }
        }

        /// <summary>
        /// Gets the number of pixels along the image's X-axis.
        /// </summary>
        public int PixelWidth
        {
            get { return bitmap.PixelWidth; }
        }

        /// <summary>
        /// Gets the scaling factor of the image.
        /// </summary>
        public double Scale
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the image source instance.
        /// </summary>
        public global::Windows.UI.Xaml.Media.ImageSource Source
        {
            get { return bitmap; }
        }

        private readonly global::Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="WritableBitmap"/> class.
        /// </summary>
        /// <param name="pixelWidth">The number of pixels along the image's X-axis.</param>
        /// <param name="pixelHeight">The number of pixels along the image's Y-axis.</param>
        public WritableBitmap(int pixelWidth, int pixelHeight)
        {
            bitmap = new global::Windows.UI.Xaml.Media.Imaging.WriteableBitmap(pixelWidth, pixelHeight);
        }

        /// <summary>
        /// Gets the data of the bitmap as a byte array.
        /// </summary>
        /// <returns>The image data as an <see cref="Array"/> of bytes.</returns>
        public Task<byte[]> GetPixelsAsync()
        {
            var array = bitmap.PixelBuffer.ToArray();
            return Task.Run(() =>
            {
                // Convert from BGRA to ARGB
                for (int i = 0; i < array.Length; i += 4)
                {
                    byte swap = array[i];
                    array[i] = array[i + 3];
                    array[i + 3] = swap;

                    swap = array[i + 1];
                    array[i + 1] = array[i + 2];
                    array[i + 2] = swap;
                }

                return array;
            });
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
                encorder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight,
                    96, 96, bitmap.PixelBuffer.ToArray());
                await encorder.FlushAsync();

                await stream.FlushAsync();
            }
        }

        /// <summary>
        /// Sets the pixel data of the bitmap to the specified byte array.
        /// </summary>
        /// <param name="pixelData">The byte array containing the pixel data.</param>
        public async Task SetPixelsAsync(byte[] pixelData)
        {
            // Convert from ARGB to BGRA
            var array = new byte[pixelData.Length];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                array[i] = pixelData[i + 3];
                array[i + 1] = pixelData[i + 2];
                array[i + 2] = pixelData[i + 1];
                array[i + 3] = pixelData[i];
            }

            using (var stream = bitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(array, 0, array.Length);
            }

            bitmap.Invalidate();
        }
    }
}
