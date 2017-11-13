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
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Prism.Windows.IO
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeFileInfo"/> that specifically handles files on external drives.
    /// </summary>
    public class ExternalFileInfo : INativeFileInfo
    {
        /// <summary>
        /// Gets or sets the attributes of the directory.
        /// </summary>
        public Prism.IO.FileAttributes Attributes
        {
            get
            {
                return (file.Attributes == FileAttributes.Normal ?
                    Prism.IO.FileAttributes.Normal : (Prism.IO.FileAttributes)file.Attributes);
            }
            set
            {
                new System.IO.FileInfo(file.Path).Attributes = (System.IO.FileAttributes)value;
            }
        }

        /// <summary>
        /// Gets the date and time that the directory was created.
        /// </summary>
        public DateTime CreationTime
        {
            get { return file.DateCreated.LocalDateTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was created.
        /// </summary>
        public DateTime CreationTimeUtc
        {
            get { return file.DateCreated.UtcDateTime; }
        }

        /// <summary>
        /// Gets the directory in which the file exists.
        /// </summary>
        public INativeDirectoryInfo Directory { get; }

        /// <summary>
        /// Gets a value indicating whether the directory exists.
        /// </summary>
        public bool Exists
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the extension of the file.
        /// </summary>
        public string Extension
        {
            get { return file.FileType; }
        }

        /// <summary>
        /// Gets a value indicating whether the file is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (file.Attributes & FileAttributes.ReadOnly) != 0; }
        }

        /// <summary>
        /// Gets the date and time that the directory was last accessed.
        /// </summary>
        public DateTime LastAccessTime
        {
            get { return fileProps.DateModified.LocalDateTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was last accessed.
        /// </summary>
        public DateTime LastAccessTimeUtc
        {
            get { return fileProps.DateModified.UtcDateTime; }
        }

        /// <summary>
        /// Gets the date and time that the directory was last modified.
        /// </summary>
        public DateTime LastWriteTime
        {
            get { return fileProps.DateModified.LocalDateTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was last modified.
        /// </summary>
        public DateTime LastWriteTimeUtc
        {
            get { return fileProps.DateModified.UtcDateTime; }
        }

        /// <summary>
        /// Gets the size of the file, in bytes.
        /// </summary>
        public long Length
        {
            get { return (long)fileProps.Size; }
        }

        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        public string Name
        {
            get { return file.Name; }
        }

        /// <summary>
        /// Gets the full path to the directory.
        /// </summary>
        public string Path
        {
            get { return file.Path; }
        }
        
        private readonly StorageFile file;
        private readonly BasicProperties fileProps;

        internal ExternalFileInfo(StorageFile file, ExternalDirectoryInfo directory, BasicProperties fileProps)
        {
            this.file = file;
            this.fileProps = fileProps;

            Directory = directory;
        }
    }
}
