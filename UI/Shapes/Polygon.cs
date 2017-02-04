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

namespace Prism.Windows.UI.Shapes
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativePolygon"/>.
    /// </summary>
    [Register(typeof(INativePolygon))]
    public class Polygon : PolyShapeBase, INativePolygon
    {
        /// <summary>
        /// Gets or sets the rule to use for determining the interior fill of the shape.
        /// </summary>
        public override FillRule FillRule
        {
            get { return (FillRule)polygon.FillRule; }
            set
            {
                polygon.FillRule = (global::Windows.UI.Xaml.Media.FillRule)value;
                OnPropertyChanged(Prism.UI.Shapes.Polygon.FillRuleProperty);
            }
        }

        private readonly global::Windows.UI.Xaml.Shapes.Polygon polygon;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        public Polygon()
            : base(new global::Windows.UI.Xaml.Shapes.Polygon())
        {
            polygon = (global::Windows.UI.Xaml.Shapes.Polygon)Element;
        }
    }
}
