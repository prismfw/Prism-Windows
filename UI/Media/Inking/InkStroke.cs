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
using System.Collections.Generic;
using System.Linq;
using Prism.Native;
using Prism.UI.Media.Inking;

namespace Prism.Windows.UI.Media.Inking
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeInkStroke"/>.
    /// </summary>
    [Register(typeof(INativeInkStroke))]
    public class InkStroke : INativeInkStroke
    {
        /// <summary>
        /// Gets a rectangle that encompasses all of the points in the ink stroke.
        /// </summary>
        public Rectangle BoundingBox
        {
            get { return inkStroke.BoundingRect.GetRectangle(); }
        }

        /// <summary>
        /// Gets a collection of points that make up the ink stroke.
        /// </summary>
        public IEnumerable<Point> Points
        {
            get { return inkStroke.GetInkPoints().Select(p => p.Position.GetPoint()); }
        }

        private readonly global::Windows.UI.Input.Inking.InkStroke inkStroke;

        /// <summary>
        /// Initializes a new instance of the <see cref="InkStroke"/> class.
        /// </summary>
        /// <param name="points">A collection of <see cref="Point"/> objects that defines the shape of the stroke.</param>
        public InkStroke(IEnumerable<Point> points)
        {
            inkStroke = new global::Windows.UI.Input.Inking.InkStrokeBuilder().CreateStrokeFromInkPoints(points.Select(p =>
                new global::Windows.UI.Input.Inking.InkPoint(p.GetPoint(), 0.5f)), System.Numerics.Matrix3x2.Identity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InkStroke"/> class.
        /// </summary>
        internal InkStroke(global::Windows.UI.Input.Inking.InkStroke stroke)
        {
            inkStroke = stroke;
        }

        /// <summary>
        /// Returns a deep-copy clone of this instance.
        /// </summary>
        public INativeInkStroke Clone()
        {
            var builder = new global::Windows.UI.Input.Inking.InkStrokeBuilder();
            builder.SetDefaultDrawingAttributes(inkStroke.DrawingAttributes);
            return new InkStroke(builder.CreateStrokeFromInkPoints(inkStroke.GetInkPoints(), inkStroke.PointTransform));
        }

        /// <summary>
        /// Returns a copy of the ink stroke's drawing attributes.
        /// </summary>
        public InkDrawingAttributes CopyDrawingAttributes()
        {
            return new InkDrawingAttributes()
            {
                Color = inkStroke.DrawingAttributes.Color.GetColor(),
                Size = Math.Max(inkStroke.DrawingAttributes.Size.Width, inkStroke.DrawingAttributes.Size.Height),
                PenTip = inkStroke.DrawingAttributes.PenTip == global::Windows.UI.Input.Inking.PenTipShape.Rectangle ? PenTipShape.Square : PenTipShape.Circle
            };
        }

        /// <summary>
        /// Updates the drawing attributes of the ink stroke.
        /// </summary>
        /// <param name="attributes">The drawing attributes to apply to the ink stroke.</param>
        public void UpdateDrawingAttributes(InkDrawingAttributes attributes)
        {
            inkStroke.DrawingAttributes = attributes.GetInkDrawingAttributes();
        }

        /// <summary>
        /// Implicitly converts an <see cref="InkStroke"/> to an <see cref="global::Windows.UI.Input.Inking.InkStroke"/>.
        /// </summary>
        /// <param name="stroke">The object to be converted.</param>
        public static explicit operator global::Windows.UI.Input.Inking.InkStroke(InkStroke stroke)
        {
            return stroke.inkStroke;
        }

        /// <summary>
        /// Implicitly converts an <see cref="global::Windows.UI.Input.Inking.InkStroke"/> to an <see cref="InkStroke"/>.
        /// </summary>
        /// <param name="stroke">The object to be converted.</param>
        public static implicit operator InkStroke(global::Windows.UI.Input.Inking.InkStroke stroke)
        {
            return new InkStroke(stroke);
        }
    }
}
