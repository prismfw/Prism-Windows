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
using Windows.UI.Xaml.Media;

namespace Prism.Windows
{
    /// <summary>
    /// Provides methods for traversing the Windows view hierarchy.
    /// </summary>
    public static class ViewExtensions
    {
        /// <summary>
        /// Walks the view hierarchy and returns the child that is of type <typeparamref name="T"/> and satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of child to search for.</typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="predicate">The condition to check each child for.</param>
        /// <returns></returns>
        public static T GetChild<T>(this global::Windows.UI.Xaml.DependencyObject parent, Func<T, bool> predicate = null)
            where T : class
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T && (predicate == null || predicate.Invoke((T)(object)child)))
                {
                    return (T)(object)child;
                }

                var c = child.GetChild<T>(predicate);
                if (c != null)
                {
                    return c;
                }
            }

            return null;
        }

        /// <summary>
        /// Walks the view hierarchy and returns the parent that is of type <typeparamref name="T"/> and satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of parent to search for.</typeparam>
        /// <param name="child">The child.</param>
        /// <param name="predicate">The condition to check each parent for.</param>
        /// <returns></returns>
        public static T GetParent<T>(this global::Windows.UI.Xaml.DependencyObject child, Func<T, bool> predicate = null)
            where T : class
        {
            if (typeof(T) == typeof(INativeWindow))
            {
                var window = ObjectRetriever.GetNativeObject(Prism.UI.Window.Current) as INativeWindow;
                if (window != null && window.Content == child && (predicate == null || predicate.Invoke((T)window)))
                {
                    return (T)window;
                }
            }

            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
            {
                return null;
            }

            if (parent is T && (predicate == null || predicate.Invoke((T)(object)parent)))
            {
                return (T)(object)parent;
            }

            return parent.GetParent<T>(predicate);
        }

        internal static INativeElement GetNearestElement(this global::Windows.UI.Xaml.DependencyObject child)
        {
            var element = child as INativeElement;
            if (element != null)
            {
                return element;
            }

            return GetNearestElement(VisualTreeHelper.GetParent(child));
        }
    }
}
