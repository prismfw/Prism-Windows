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


using System.Linq;
using Prism.Native;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeVisualTreeHelper"/>.
    /// </summary>
    [Register(typeof(INativeVisualTreeHelper), IsSingleton = true)]
    public class VisualTreeHelper : INativeVisualTreeHelper
    {
        /// <summary>
        /// Returns the number of children in the specified object's child collection.
        /// </summary>
        /// <param name="reference">The parent object.</param>
        /// <returns>The number of children in the parent object's child collection.</returns>
        public int GetChildrenCount(object reference)
        {
            var window = reference as INativeWindow;
            if (window != null)
            {
                return window.Content == null ? 0 : 1;
            }

            var dep = reference as DependencyObject;
            if (dep == null)
            {
                return 0;
            }

            var flyout = reference as Controls.Flyout;
            if (flyout != null)
            {
                return flyout.Presenter == null ? 0 : 1;
            }

            var menuflyout = reference as Controls.MenuFlyout;
            if (menuflyout != null)
            {
                return menuflyout.Presenter == null ? 0 : 1;
            }

            int count = 0;
            var viewStack = reference as INativeViewStack;
            if (viewStack != null)
            {
                count = viewStack.Views.Count(o => o != viewStack.CurrentView);
            }

            return global::Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(dep) + count;
        }

        /// <summary>
        /// Returns the child that is located at the specified index in the child collection of the specified object.
        /// </summary>
        /// <param name="reference">The parent object.</param>
        /// <param name="childIndex">The zero-based index of the child to return.</param>
        /// <returns>The child at the specified index.</returns>
        public object GetChild(object reference, int childIndex)
        {
            var window = reference as INativeWindow;
            if (window != null && childIndex == 0)
            {
                return window.Content;
            }

            var dep = reference as DependencyObject;
            if (dep == null)
            {
                return null;
            }

            var flyout = reference as Controls.Flyout;
            if (flyout != null)
            {
                return childIndex == 0 ? flyout.Presenter : null;
            }

            var menuflyout = reference as Controls.MenuFlyout;
            if (menuflyout != null)
            {
                return childIndex == 0 ? menuflyout.Presenter : null;
            }

            int childCount = global::Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(dep);
            if (childIndex >= childCount)
            {
                var viewStack = reference as INativeViewStack;
                return viewStack == null ? null : viewStack.Views.Where(o => o != viewStack.CurrentView).ElementAtOrDefault(childIndex - childCount);
            }

            return global::Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(dep, childIndex);
        }

        /// <summary>
        /// Returns the parent of the specified object.
        /// </summary>
        /// <param name="reference">The child object.</param>
        /// <returns>The parent object of the child, or <c>null</c> if no parent is found.</returns>
        public object GetParent(object reference)
        {
            var window = ObjectRetriever.GetNativeObject(Prism.UI.Window.Current) as INativeWindow;
            if (window != null && window.Content == reference)
            {
                return window;
            }

            FrameworkElement presenter = ((FrameworkElement)(reference as FlyoutPresenter) ?? reference as MenuFlyoutPresenter);
            if (presenter?.Tag != null)
            {
                return presenter.Tag;
            }

            var dep = reference as DependencyObject;
            return (dep == null ? null : global::Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(dep)) ?? (reference as IViewStackChild)?.ViewStack;
        }
    }
}
