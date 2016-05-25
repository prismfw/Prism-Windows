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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Prism.Windows.UI.Media.Imaging
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeImageCompositor"/>.
    /// </summary>
    [Register(typeof(INativeImageCompositor), IsSingleton = true)]
    public class ImageCompositor : INativeImageCompositor
    {
        /// <summary>
        /// Composites the provided images into one image with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the composited image.</param>
        /// <param name="height">The height of the composited image.</param>
        /// <param name="images">The images that are to be composited. The first image will be drawn first and each subsequent image will be drawn on top.</param>
        public async Task<Prism.UI.Media.Imaging.ImageSource> CompositeAsync(int width, int height, params INativeImageSource[] images)
        {
            if (images.Length == 0)
            {
                return new Prism.UI.Media.Imaging.BitmapImage(new byte[0]);
            }

            var bitmaps = images.Select(i => i.GetImageSource());
            var grid = new Grid();
            foreach (var bitmap in bitmaps)
            {
                grid.Children.Add(new Image()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Source = bitmap,
                    VerticalAlignment = VerticalAlignment.Center
                });
            }

            var content = Prism.UI.Window.Current.Content;
            try
            {
                var target = new RenderTargetBitmap();

                Prism.UI.Window.Current.Content = grid;
                await target.RenderAsync(grid, width, height);
                Prism.UI.Window.Current.Content = content;

                return new Prism.UI.Media.Imaging.BitmapImage((await target.GetPixelsAsync()).ToArray());
            }
            finally
            {
                if (Prism.UI.Window.Current.Content != content)
                {
                    Prism.UI.Window.Current.Content = content;
                }
            }
        }
    }
}
