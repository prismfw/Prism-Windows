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


using Prism.Native;
using Prism.UI.Media;

namespace Prism.Windows.UI.Media
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeTransform"/>.
    /// </summary>
    [Register(typeof(INativeTransform))]
    public class Transform : INativeTransform
    {
        /// <summary>
        /// Gets the name of the font family.
        /// </summary>
        public Matrix Value
        {
            get { return matrixTransform.Matrix.GetMatrix(); }
            set { matrixTransform.Matrix = value.GetMatrix(); }
        }

        private readonly global::Windows.UI.Xaml.Media.MatrixTransform matrixTransform;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class.
        /// </summary>
        public Transform()
        {
            matrixTransform = new global::Windows.UI.Xaml.Media.MatrixTransform();
        }

        /// <summary>
        /// Returns a <see cref="global::Windows.UI.Xaml.Media.Transform"/> from a <see cref="Transform"/>.
        /// </summary>
        /// <param name="transform">The transform.</param>
        public static implicit operator global::Windows.UI.Xaml.Media.Transform(Transform transform)
        {
            return transform.matrixTransform;
        }
    }
}
