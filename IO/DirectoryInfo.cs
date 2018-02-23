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
using System.Threading.Tasks;
using Prism.IO;
using Prism.Native;

namespace Prism.Windows.IO
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeDirectoryInfo"/>.
    /// </summary>
    [Register(typeof(INativeDirectoryInfo))]
    public class DirectoryInfo : INativeDirectoryInfo
    {
        /// <summary>
        /// Gets or sets the attributes of the directory.
        /// </summary>
        public Prism.IO.FileAttributes Attributes
        {
            get { return (Prism.IO.FileAttributes)(info.Attributes); }
            set { info.Attributes = (System.IO.FileAttributes)value; }
        }

        /// <summary>
        /// Gets the date and time that the directory was created.
        /// </summary>
        public DateTime CreationTime
        {
            get { return info.CreationTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was created.
        /// </summary>
        public DateTime CreationTimeUtc
        {
            get { return info.CreationTimeUtc; }
        }

        /// <summary>
        /// Gets a value indicating whether the directory exists.
        /// </summary>
        public bool Exists
        {
            get { return info.Exists; }
        }

        /// <summary>
        /// Gets the date and time that the directory was last accessed.
        /// </summary>
        public DateTime LastAccessTime
        {
            get { return info.LastAccessTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was last accessed.
        /// </summary>
        public DateTime LastAccessTimeUtc
        {
            get { return info.LastAccessTimeUtc; }
        }

        /// <summary>
        /// Gets the date and time that the directory was last modified.
        /// </summary>
        public DateTime LastWriteTime
        {
            get { return info.LastWriteTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was last modified.
        /// </summary>
        public DateTime LastWriteTimeUtc
        {
            get { return info.LastWriteTimeUtc; }
        }

        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        public string Name
        {
            get { return info.Name; }
        }

        /// <summary>
        /// Gets the full path to the directory.
        /// </summary>
        public string Path
        {
            get { return info.FullName; }
        }
        
        private readonly System.IO.DirectoryInfo info;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryInfo"/> class.
        /// </summary>
        /// <param name="directoryPath">The path to the directory.</param>
        public DirectoryInfo(string directoryPath)
            : this(new System.IO.DirectoryInfo(directoryPath))
        {
        }

        internal DirectoryInfo(System.IO.DirectoryInfo info)
        {
            this.info = info;
        }

        /// <summary>
        /// Gets information about the subdirectories within the current directory,
        /// optionally getting information about directories in any subdirectories as well.
        /// </summary>
        /// <param name="searchOption">A value indicating whether to search subdirectories or just the top directory.</param>
        /// <returns>An <see cref="Array"/> containing the directory information.</returns>
        public Task<INativeDirectoryInfo[]> GetDirectoriesAsync(SearchOption searchOption)
        {
            return Task.Run<INativeDirectoryInfo[]>(() =>
            {
                return info.GetFileSystemInfos("*", searchOption == SearchOption.AllDirectories ?
                    System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly)
                    .OfType<System.IO.DirectoryInfo>().Select(d => new DirectoryInfo(d)).ToArray();
            });
        }

        /// <summary>
        /// Gets information about the files in the current directory,
        /// optionally getting information about files in any subdirectories as well.
        /// </summary>
        /// <param name="searchOption">A value indicating whether to search subdirectories or just the top directory.</param>
        /// <returns>An <see cref="Array"/> containing the file information.</returns>
        public Task<INativeFileInfo[]> GetFilesAsync(SearchOption searchOption)
        {
            return Task.Run<INativeFileInfo[]>(() =>
            {
                return info.GetFileSystemInfos("*", searchOption == SearchOption.AllDirectories ?
                    System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly)
                    .OfType<System.IO.FileInfo>().Select(f => new FileInfo(f)).ToArray();
            });
        }

        /// <summary>
        /// Gets information about the parent directory in which the current directory exists.
        /// </summary>
        /// <returns>The directory information.</returns>
        public Task<INativeDirectoryInfo> GetParentAsync()
        {
            return Task.Run<INativeDirectoryInfo>(() =>
            {
                return info.Parent == null ? null : new DirectoryInfo(info.Parent);
            });
        }
    }
}
