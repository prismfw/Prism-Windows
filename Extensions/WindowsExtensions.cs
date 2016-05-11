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


#pragma warning disable 419

using System.Linq;
using Prism.Native;
using Prism.Systems;
using Prism.Windows.UI.Media;
using Windows.System.Power;
using Windows.UI.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace Prism.Windows
{
    /// <summary>
    /// Provides methods for converting Prism objects to Windows objects and vice versa.
    /// </summary>
    public static class WindowsExtensions
    {
        /// <summary>
        /// Gets a <see cref="global::Windows.UI.Xaml.Media.Brush"/> from a <see cref="Prism.UI.Media.Brush"/>.
        /// </summary>
        /// <param name="brush">The brush.</param>
        public static global::Windows.UI.Xaml.Media.Brush GetBrush(this Prism.UI.Media.Brush brush)
        {
            var solidColor = brush as Prism.UI.Media.SolidColorBrush;
            if (solidColor != null)
            {
                return new global::Windows.UI.Xaml.Media.SolidColorBrush(solidColor.Color.GetColor());
            }

            var imageBrush = brush as Prism.UI.Media.ImageBrush;
            if (imageBrush != null)
            {
                return new global::Windows.UI.Xaml.Media.ImageBrush()
                {
                    ImageSource = ((INativeImageSource)ObjectRetriever.GetNativeObject(imageBrush.Image)).GetImage(),
                    Stretch = imageBrush.Stretch.GetStretch()
                };
            }

            var linearBrush = brush as Prism.UI.Media.LinearGradientBrush;
            if (linearBrush != null)
            {
                var retval = new global::Windows.UI.Xaml.Media.LinearGradientBrush()
                {
                    StartPoint = linearBrush.StartPoint.GetPoint(),
                    EndPoint = linearBrush.EndPoint.GetPoint()
                };

                for (int i = 0; i < linearBrush.Colors.Count; i++)
                {
                    var color = linearBrush.Colors[i];
                    retval.GradientStops.Add(new global::Windows.UI.Xaml.Media.GradientStop()
                    {
                        Color = color.GetColor(),
                        Offset = i / (linearBrush.Colors.Count - 1d)
                    });
                }

                return retval;
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="Prism.UI.Media.Brush"/> from a <see cref="global::Windows.UI.Xaml.Media.Brush"/>.
        /// </summary>
        /// <param name="brush">The brush.</param>
        public static Prism.UI.Media.Brush GetBrush(this global::Windows.UI.Xaml.Media.Brush brush)
        {
            var solidColor = brush as global::Windows.UI.Xaml.Media.SolidColorBrush;
            if (solidColor != null)
            {
                return new Prism.UI.Media.SolidColorBrush(solidColor.Color.GetColor());
            }

            var imageBrush = brush as global::Windows.UI.Xaml.Media.ImageBrush;
            if (imageBrush != null)
            {
                var bmp = imageBrush.ImageSource as BitmapImage;
                return bmp == null ? null : new Prism.UI.Media.ImageBrush(bmp.UriSource, imageBrush.Stretch.GetStretch());
            }

            var linearBrush = brush as global::Windows.UI.Xaml.Media.LinearGradientBrush;
            if (linearBrush != null)
            {
                return new Prism.UI.Media.LinearGradientBrush(linearBrush.StartPoint.GetPoint(), linearBrush.EndPoint.GetPoint(),
                    linearBrush.GradientStops.Select(gs => gs.Color.GetColor()).ToArray());
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="global::Windows.UI.Color"/> from a <see cref="Prism.UI.Color"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        public static global::Windows.UI.Color GetColor(this Prism.UI.Color color)
        {
            return global::Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Gets a <see cref="Prism.UI.Color"/> from a <see cref="global::Windows.UI.Color"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        public static Prism.UI.Color GetColor(this global::Windows.UI.Color color)
        {
            return new Prism.UI.Color(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Gets a <see cref="Prism.UI.Media.FontStyle"/> from a <see cref="global::Windows.UI.Xaml.Controls.Control"/>.
        /// </summary>
        /// <param name="control">The control.</param>
        public static Prism.UI.Media.FontStyle GetFontStyle(this global::Windows.UI.Xaml.Controls.Control control)
        {
            return (Prism.UI.Media.FontStyle)((control.FontStyle == FontStyle.Italic ? 2 : 0) +
                (control.FontWeight.Weight > FontWeights.Normal.Weight ? 1 : 0));
        }

        /// <summary>
        /// Gets a <see cref="Prism.UI.Media.FontStyle"/> from a <see cref="global::Windows.UI.Xaml.Controls.TextBlock"/>.
        /// </summary>
        /// <param name="textBlock">The text block.</param>
        public static Prism.UI.Media.FontStyle GetFontStyle(this global::Windows.UI.Xaml.Controls.TextBlock textBlock)
        {
            return (Prism.UI.Media.FontStyle)((textBlock.FontStyle == FontStyle.Italic ? 2 : 0) +
                (textBlock.FontWeight.Weight > FontWeights.Normal.Weight ? 1 : 0));
        }

        /// <summary>
        /// Gets a <see cref="BitmapImage"/> from an <see cref="INativeImageSource"/>.
        /// </summary>
        /// <param name="source">The image.</param>
        public static BitmapImage GetImage(this INativeImageSource source)
        {
            var image = source as Prism.Windows.UI.Media.Imaging.ImageSource;
            return image == null ? (object)source as BitmapImage : image.BitmapImage;
        }

        /// <summary>
        /// Gets a <see cref="global::Windows.Foundation.Point"/> from a <see cref="Point"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        public static global::Windows.Foundation.Point GetPoint(this Point point)
        {
            return new global::Windows.Foundation.Point(point.X, point.Y);
        }

        /// <summary>
        /// Gets a <see cref="Point"/> from a <see cref="global::Windows.Foundation.Point"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        public static Point GetPoint(this global::Windows.Foundation.Point point)
        {
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Gets a <see cref="PowerSource"/> from a <see cref="BatteryStatus"/>.
        /// </summary>
        /// <param name="status">The point.</param>
        public static PowerSource GetPowerSource(this BatteryStatus status)
        {
            switch (status)
            {
                case BatteryStatus.Charging:
                case BatteryStatus.Idle:
                case BatteryStatus.NotPresent:
                    return PowerSource.External;
                case BatteryStatus.Discharging:
                    return PowerSource.Battery;
                default:
                    return PowerSource.Unknown;
            }
        }

        /// <summary>
        /// Gets a <see cref="global::Windows.Foundation.Rect"/> from a <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        public static global::Windows.Foundation.Rect GetRectangle(this Rectangle rect)
        {
            return new global::Windows.Foundation.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Gets a <see cref="Rectangle"/> from a <see cref="global::Windows.Foundation.Rect"/>.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        public static Rectangle GetRectangle(this global::Windows.Foundation.Rect rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Gets a <see cref="global::Windows.Foundation.Size"/> from a <see cref="Size"/>.
        /// </summary>
        /// <param name="size">The size.</param>
        public static global::Windows.Foundation.Size GetSize(this Size size)
        {
            return new global::Windows.Foundation.Size(size.Width, size.Height);
        }

        /// <summary>
        /// Gets a <see cref="Size"/> from a <see cref="global::Windows.Foundation.Size"/>.
        /// </summary>
        /// <param name="size">The size.</param>
        public static Size GetSize(this global::Windows.Foundation.Size size)
        {
            return new Size(size.Width, size.Height);
        }

        /// <summary>
        /// Gets a <see cref="global::Windows.UI.Xaml.Media.Stretch"/> from a <see cref="Prism.UI.Stretch"/>.
        /// </summary>
        /// <param name="stretch">The stretch.</param>
        public static global::Windows.UI.Xaml.Media.Stretch GetStretch(this Prism.UI.Stretch stretch)
        {
            switch (stretch)
            {
                case Prism.UI.Stretch.Fill:
                    return global::Windows.UI.Xaml.Media.Stretch.Fill;
                case Prism.UI.Stretch.Uniform:
                    return global::Windows.UI.Xaml.Media.Stretch.Uniform;
                case Prism.UI.Stretch.UniformToFill:
                    return global::Windows.UI.Xaml.Media.Stretch.UniformToFill;
                default:
                    return global::Windows.UI.Xaml.Media.Stretch.None;
            }
        }

        /// <summary>
        /// Gets a <see cref="Prism.UI.Stretch"/> from a <see cref="global::Windows.UI.Xaml.Media.Stretch"/>.
        /// </summary>
        /// <param name="stretch">The stretch.</param>
        public static Prism.UI.Stretch GetStretch(this global::Windows.UI.Xaml.Media.Stretch stretch)
        {
            switch (stretch)
            {
                case global::Windows.UI.Xaml.Media.Stretch.Fill:
                    return Prism.UI.Stretch.Fill;
                case global::Windows.UI.Xaml.Media.Stretch.Uniform:
                    return Prism.UI.Stretch.Uniform;
                case global::Windows.UI.Xaml.Media.Stretch.UniformToFill:
                    return Prism.UI.Stretch.UniformToFill;
                default:
                    return Prism.UI.Stretch.None;
            }
        }

        /// <summary>
        /// Gets a <see cref="global::Windows.UI.Xaml.TextAlignment"/> from a <see cref="Prism.UI.TextAlignment"/>.
        /// </summary>
        /// <param name="textAlignment">The text alignment.</param>
        public static global::Windows.UI.Xaml.TextAlignment GetTextAlignment(this Prism.UI.TextAlignment textAlignment)
        {
            switch (textAlignment)
            {
                case Prism.UI.TextAlignment.Center:
                    return global::Windows.UI.Xaml.TextAlignment.Center;
                case Prism.UI.TextAlignment.Right:
                    return global::Windows.UI.Xaml.TextAlignment.Right;
                case Prism.UI.TextAlignment.Justified:
                    return global::Windows.UI.Xaml.TextAlignment.Justify;
                default:
                    return global::Windows.UI.Xaml.TextAlignment.Left;
            }
        }

        /// <summary>
        /// Gets a <see cref="Prism.UI.TextAlignment"/> from a <see cref="global::Windows.UI.Xaml.TextAlignment"/>.
        /// </summary>
        /// <param name="textAlignment">The text alignment.</param>
        public static Prism.UI.TextAlignment GetTextAlignment(this global::Windows.UI.Xaml.TextAlignment textAlignment)
        {
            switch (textAlignment)
            {
                case global::Windows.UI.Xaml.TextAlignment.Center:
                    return Prism.UI.TextAlignment.Center;
                case global::Windows.UI.Xaml.TextAlignment.Right:
                    return Prism.UI.TextAlignment.Right;
                case global::Windows.UI.Xaml.TextAlignment.Justify:
                    return Prism.UI.TextAlignment.Justified;
                default:
                    return Prism.UI.TextAlignment.Left;
            }
        }

        /// <summary>
        /// Gets a <see cref="global::Windows.UI.Xaml.Thickness"/> from a <see cref="Thickness"/>.
        /// </summary>
        /// <param name="thickness">The thickness.</param>
        public static global::Windows.UI.Xaml.Thickness GetThickness(this Thickness thickness)
        {
            return new global::Windows.UI.Xaml.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        /// <summary>
        /// Gets a <see cref="Thickness"/> from a <see cref="global::Windows.UI.Xaml.Thickness"/>.
        /// </summary>
        /// <param name="thickness">The thickness.</param>
        public static Thickness GetThickness(this global::Windows.UI.Xaml.Thickness thickness)
        {
            return new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        /// <summary>
        /// Sets the font family and style for a <see cref="global::Windows.UI.Xaml.Controls.Control"/>.
        /// </summary>
        /// <param name="control">The control on which to set the font style.</param>
        /// <param name="fontFamily">The font family to apply to the control.</param>
        /// <param name="fontStyle">The font style to apply to the control.</param>
        public static void SetFont(this global::Windows.UI.Xaml.Controls.Control control, FontFamily fontFamily, Prism.UI.Media.FontStyle fontStyle)
        {
            control.FontFamily = fontFamily;
            control.FontStyle = (fontStyle & Prism.UI.Media.FontStyle.Italic) == 0 ? FontStyle.Normal : FontStyle.Italic;
            control.FontWeight = fontFamily == null || fontFamily.Weight.Weight == FontWeights.Normal.Weight ?
                (fontStyle & Prism.UI.Media.FontStyle.Bold) == 0 ? FontWeights.Normal : FontWeights.Bold : fontFamily.Weight;
        }

        /// <summary>
        /// Sets the font family and style for a <see cref="global::Windows.UI.Xaml.Controls.TextBlock"/>.
        /// </summary>
        /// <param name="textBlock">The text block on which to set the font.</param>
        /// <param name="fontFamily">The font family to apply to the text block.</param>
        /// <param name="fontStyle">The font style to apply to the text block.</param>
        public static void SetFont(this global::Windows.UI.Xaml.Controls.TextBlock textBlock, FontFamily fontFamily, Prism.UI.Media.FontStyle fontStyle)
        {
            textBlock.FontFamily = fontFamily;
            textBlock.FontStyle = (fontStyle & Prism.UI.Media.FontStyle.Italic) == 0 ? FontStyle.Normal : FontStyle.Italic;
            textBlock.FontWeight = fontFamily == null || fontFamily.Weight.Weight == FontWeights.Normal.Weight ?
                (fontStyle & Prism.UI.Media.FontStyle.Bold) == 0 ? FontWeights.Normal : FontWeights.Bold : fontFamily.Weight;
        }
    }
}
