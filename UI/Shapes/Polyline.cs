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


using Prism.Native;
using Prism.UI.Media;

namespace Prism.Windows.UI.Shapes
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativePolyline"/>.
    /// </summary>
    [Register(typeof(INativePolyline))]
    public class Polyline : PolyShapeBase, INativePolyline
    {
        /// <summary>
        /// Gets or sets the rule to use for determining the interior fill of the shape.
        /// </summary>
        public override FillRule FillRule
        {
            get { return (FillRule)polyline.FillRule; }
            set
            {
                polyline.FillRule = (global::Windows.UI.Xaml.Media.FillRule)value;
                OnPropertyChanged(Prism.UI.Shapes.Polyline.FillRuleProperty);
            }
        }

        private readonly global::Windows.UI.Xaml.Shapes.Polyline polyline;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polyline"/> class.
        /// </summary>
        public Polyline()
            : base(new global::Windows.UI.Xaml.Shapes.Polyline())
        {
            polyline = (global::Windows.UI.Xaml.Shapes.Polyline)Element;
        }
    }
}
