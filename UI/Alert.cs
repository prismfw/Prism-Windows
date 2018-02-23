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
using System.Linq;
using Prism.Native;
using Prism.UI;
using Windows.UI.Popups;

namespace Prism.Windows.UI
{
    /// <summary>
    /// Represents a Windows implementation for an <see cref="INativeAlert"/>.
    /// </summary>
    [Register(typeof(INativeAlert))]
    public class Alert : INativeAlert
    {
        /// <summary>
        /// Gets the number of buttons that have been added to the alert.
        /// </summary>
        public int ButtonCount
        {
            get { return dialog.Commands.Count; }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the button that is mapped to the ESC key on desktop platforms.
        /// </summary>
        public int CancelButtonIndex
        {
            get { return (int)dialog.CancelCommandIndex; }
            set { dialog.CancelCommandIndex = (uint)value; }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the button that is mapped to the Enter key on desktop platforms.
        /// </summary>
        public int DefaultButtonIndex
        {
            get { return (int)dialog.DefaultCommandIndex; }
            set { dialog.DefaultCommandIndex = (uint)value; }
        }

        /// <summary>
        /// Gets the message text for the alert.
        /// </summary>
        public string Message
        {
            get { return dialog.Content; }
        }

        /// <summary>
        /// Gets the title text for the alert.
        /// </summary>
        public string Title
        {
            get { return dialog.Title; }
        }

        private readonly MessageDialog dialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="Alert"/> class.
        /// </summary>
        /// <param name="message">The message text for the alert.</param>
        /// <param name="title">The title text for the alert.</param>
        public Alert(string message, string title)
        {
            dialog = new MessageDialog(message ?? string.Empty, title ?? string.Empty);
        }

        /// <summary>
        /// Adds the specified <see cref="AlertButton"/> to the alert.
        /// </summary>
        /// <param name="button">The button to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="button"/> is <c>null</c>.</exception>
        public void AddButton(AlertButton button)
        {
            if (button == null)
            {
                throw new ArgumentNullException("button");
            }

            dialog.Commands.Add(new UICommand(button.Title, (o) =>
            {
                var b = o.Id as AlertButton;
                if (b != null && b.Action != null)
                {
                    b.Action.Invoke(b);
                }
            }, button));
        }

        /// <summary>
        /// Gets the button at the specified zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the button to retrieve.</param>
        /// <returns>The <see cref="AlertButton"/> at the specified index -or- <c>null</c> if there is no button.</returns>
        public AlertButton GetButton(int index)
        {
            var command = dialog.Commands.ElementAtOrDefault(index);
            return command == null ? null : command.Id as AlertButton;
        }

        /// <summary>
        /// Modally presents the alert.
        /// </summary>
        public void Show()
        {
            // this should not be awaited
#pragma warning disable 4014
            dialog.ShowAsync();
#pragma warning restore 4014
        }
    }
}
