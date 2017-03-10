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
using Windows.UI.Text;

namespace Prism.Windows.UI.Media
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeFontFamily"/>.
    /// </summary>
    [Register(typeof(INativeFontFamily))]
    public class FontFamily : global::Windows.UI.Xaml.Media.FontFamily, INativeFontFamily
    {
        /// <summary>
        /// Gets the name of the font family.
        /// </summary>
        public string Name
        {
            get { return Source; }
        }

        /// <summary>
        /// Gets the weight (boldness) of the font.
        /// </summary>
        public FontWeight Weight { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamily"/> class.
        /// </summary>
        /// <param name="familyName">The family name of the font.</param>
        /// <param name="traits">Any special traits to assist in defining the font.</param>
        public FontFamily(string familyName, string traits)
            : base(familyName)
        {
            Weight = FontWeights.Normal;

            if (traits != null)
            {
                string[] traitArray = traits.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < traitArray.Length; i++)
                {
                    switch (traitArray[i].ToLower())
                    {
                        case "black":
                            Weight = FontWeights.Black;
                            break;
                        case "bold":
                            Weight = FontWeights.Bold;
                            break;
                        case "extrablack":
                            Weight = FontWeights.ExtraBlack;
                            break;
                        case "extrabold":
                            Weight = FontWeights.ExtraBold;
                            break;
                        case "extralight":
                            Weight = FontWeights.ExtraLight;
                            break;
                        case "light":
                            Weight = FontWeights.Light;
                            break;
                        case "medium":
                            Weight = FontWeights.Medium;
                            break;
                        case "semibold":
                            Weight = FontWeights.SemiBold;
                            break;
                        case "semilight":
                            Weight = FontWeights.SemiLight;
                            break;
                        case "thin":
                            Weight = FontWeights.Thin;
                            break;
                    }
                }
            }
        }
    }
}
