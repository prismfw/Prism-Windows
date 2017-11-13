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
using System.Threading.Tasks;
using Prism.IO;
using Prism.Native;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.FileProperties;

namespace Prism.Windows.IO
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeDirectoryInfo"/> that specifically handles external drives.
    /// </summary>
    public class ExternalDirectoryInfo : INativeDirectoryInfo
    {
        /// <summary>
        /// Gets or sets the attributes of the directory.
        /// </summary>
        public Prism.IO.FileAttributes Attributes
        {
            get
            {
                return (folder.Attributes == global::Windows.Storage.FileAttributes.Normal ?
                    Prism.IO.FileAttributes.Normal : (Prism.IO.FileAttributes)folder.Attributes);
            }
            set
            {
                new System.IO.DirectoryInfo(folder.Path).Attributes = (System.IO.FileAttributes)value;
            }
        }

        /// <summary>
        /// Gets the date and time that the directory was created.
        /// </summary>
        public DateTime CreationTime
        {
            get { return folder.DateCreated.LocalDateTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was created.
        /// </summary>
        public DateTime CreationTimeUtc
        {
            get { return folder.DateCreated.UtcDateTime; }
        }

        /// <summary>
        /// Gets a value indicating whether the directory exists.
        /// </summary>
        public bool Exists
        {
            get { return !string.IsNullOrEmpty(folder.Path); }
        }

        /// <summary>
        /// Gets the date and time that the directory was last accessed.
        /// </summary>
        public DateTime LastAccessTime
        {
            get { return folderProps.DateModified.LocalDateTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was last accessed.
        /// </summary>
        public DateTime LastAccessTimeUtc
        {
            get { return folderProps.DateModified.UtcDateTime; }
        }

        /// <summary>
        /// Gets the date and time that the directory was last modified.
        /// </summary>
        public DateTime LastWriteTime
        {
            get { return folderProps.DateModified.LocalDateTime; }
        }

        /// <summary>
        /// Gets the date and time, in coordinated universal time (UTC), that the directory was last modified.
        /// </summary>
        public DateTime LastWriteTimeUtc
        {
            get { return folderProps.DateModified.UtcDateTime; }
        }

        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        public string Name
        {
            get { return folder.Name; }
        }

        /// <summary>
        /// Gets the full path to the directory.
        /// </summary>
        public string Path
        {
            get { return folder.Path; }
        }
        
        private readonly StorageFolder folder;
        private readonly BasicProperties folderProps;

        internal ExternalDirectoryInfo(StorageFolder folder, BasicProperties folderProps)
        {
            this.folder = folder;
            this.folderProps = folderProps;
        }

        /// <summary>
        /// Gets information about the subdirectories within the current directory,
        /// optionally getting information about directories in any subdirectories as well.
        /// </summary>
        /// <param name="searchOption">A value indicating whether to search subdirectories or just the top directory.</param>
        /// <returns>An <see cref="Array"/> containing the directory information.</returns>
        public async Task<INativeDirectoryInfo[]> GetDirectoriesAsync(SearchOption searchOption)
        {
            var options = new QueryOptions() { FolderDepth = searchOption == SearchOption.AllDirectories ? FolderDepth.Deep : FolderDepth.Shallow };
            var folders = await folder.CreateFolderQueryWithOptions(options).GetFoldersAsync();
            var retVal = new INativeDirectoryInfo[folders.Count];
            for (int i = 0; i < retVal.Length; i++)
            {
                var folder = folders[i];
                retVal[i] = new ExternalDirectoryInfo(folder, await folder.GetBasicPropertiesAsync());
            }

            return retVal;
        }

        /// <summary>
        /// Gets information about the files in the current directory,
        /// optionally getting information about files in any subdirectories as well.
        /// </summary>
        /// <param name="searchOption">A value indicating whether to search subdirectories or just the top directory.</param>
        /// <returns>An <see cref="Array"/> containing the file information.</returns>
        public async Task<INativeFileInfo[]> GetFilesAsync(SearchOption searchOption)
        {
            var options = new QueryOptions() { FolderDepth = searchOption == SearchOption.AllDirectories ? FolderDepth.Deep : FolderDepth.Shallow };
            var files = await folder.CreateFileQueryWithOptions(options).GetFilesAsync();
            var retVal = new INativeFileInfo[files.Count];
            for (int i = 0; i < retVal.Length; i++)
            {
                var file = files[i];
                var folder = await file.GetParentAsync();
                retVal[i] = new ExternalFileInfo(file, folder == null ? null : new ExternalDirectoryInfo(folder, await folder.GetBasicPropertiesAsync()),
                    await file.GetBasicPropertiesAsync());
            }

            return retVal;
        }

        /// <summary>
        /// Gets information about the parent directory in which the current directory exists.
        /// </summary>
        /// <returns>The directory information.</returns>
        public async Task<INativeDirectoryInfo> GetParentAsync()
        {
            var parent = await folder.GetParentAsync();
            return parent == null ? null : new ExternalDirectoryInfo(parent, await parent.GetBasicPropertiesAsync());
        }
    }
}
