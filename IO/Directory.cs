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
using System.Linq;
using System.Threading.Tasks;
using Prism.IO;
using Prism.Native;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Search;

namespace Prism.Windows.IO
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeDirectory"/>.
    /// </summary>
    [Register(typeof(INativeDirectory), IsSingleton = true)]
    public class Directory : INativeDirectory
    {
        /// <summary>
        /// Gets the directory path to a folder with read-only access that contains the application's bundled assets.
        /// </summary>
        public string AssetDirectory
        {
            get { return "ms-appx:///Assets/"; }
        }

        /// <summary>
        /// Gets the directory path to a folder with read/write access for storing persisted application data.
        /// </summary>
        public string DataDirectory
        {
            get { return ApplicationData.Current.LocalFolder.Path + "\\AppData\\"; }
        }

        /// <summary>
        /// Gets the character that is used to separate directories.
        /// </summary>
        public char SeparatorChar
        {
            get { return '\\'; }
        }

        /// <summary>
        /// Gets the directory path to a folder with read/write access for storing temporary application data.
        /// </summary>
        public string TempDirectory
        {
            get { return ApplicationData.Current.TemporaryFolder.Path + '\\'; }
        }

        /// <summary>
        /// Copies the directory from the source path to the destination path, including all subdirectories and files
        /// within it.
        /// </summary>
        /// <param name="sourceDirectoryPath">The path of the directory to be copied.</param>
        /// <param name="destinationDirectoryPath">The path to where the copied directory should be placed.</param>
        /// <param name="overwrite">Whether to overwrite any subdirectories or files at the destination path that have identical names to
        /// subdirectories or files at the source path.</param>
        public async Task CopyAsync(string sourceDirectoryPath, string destinationDirectoryPath, bool overwrite)
        {
            var sourceFolder = await StorageFolder.GetFolderFromPathAsync(sourceDirectoryPath);
            
            StorageFolder destFolder = null;
            try
            {
                destFolder = await StorageFolder.GetFolderFromPathAsync(destinationDirectoryPath);
            }
            catch { }

            if (destFolder == null)
            {
                await CreateAsync(destinationDirectoryPath);
                destFolder = await StorageFolder.GetFolderFromPathAsync(destinationDirectoryPath);
            }

            foreach (var file in await sourceFolder.GetFilesAsync())
            {
                await file.CopyAsync(destFolder, file.Name, overwrite ? NameCollisionOption.ReplaceExisting : NameCollisionOption.FailIfExists);
            }

            foreach (var folder in await sourceFolder.GetFoldersAsync())
            {
                var newFolder = await destFolder.CreateFolderAsync(folder.Name, CreationCollisionOption.OpenIfExists);
                await CopyAsync(folder.Path, newFolder.Path, overwrite);
            }
        }

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path at which to create the directory.</param>
        public async Task CreateAsync(string directoryPath)
        {
            StorageFolder folder = null;
            if (directoryPath.StartsWith(ApplicationData.Current.LocalFolder.Path))
            {
                directoryPath = directoryPath.Remove(0, ApplicationData.Current.LocalFolder.Path.Length + 1);
                folder = ApplicationData.Current.LocalFolder;
            }
            else if (directoryPath.StartsWith(ApplicationData.Current.TemporaryFolder.Path))
            {
                directoryPath = directoryPath.Remove(0, ApplicationData.Current.TemporaryFolder.Path.Length + 1);
                folder = ApplicationData.Current.TemporaryFolder;
            }

            string[] directories = directoryPath.Split('\\');
            foreach (var dir in directories)
            {
                folder = await folder.CreateFolderAsync(dir, CreationCollisionOption.OpenIfExists);
            }
        }

        /// <summary>
        /// Deletes the directory at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to delete.</param>
        /// <param name="recursive">Whether to delete all subdirectories and files within the directory.</param>
        public async Task DeleteAsync(string directoryPath, bool recursive)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            if (recursive || !(await folder.GetItemsAsync()).Any())
            {
                await folder.DeleteAsync();
            }
        }

        /// <summary>
        /// Gets the names of the subdirectories in the directory at the specified path,
        /// optionally getting the names of directories in any subdirectories as well.
        /// </summary>
        /// <param name="directoryPath">The path of the directory whose subdirectories are to be retrieved.</param>
        /// <param name="searchOption">A value indicating whether to search subdirectories or just the top directory.</param>
        /// <returns>The directories.</returns>
        public async Task<string[]> GetDirectoriesAsync(string directoryPath, Prism.IO.SearchOption searchOption)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            if (searchOption == SearchOption.AllDirectories)
            {
                return (await folder.GetFoldersAsync(CommonFolderQuery.DefaultQuery)).Select(f => f.Path).ToArray();
            }

            return (await folder.GetFoldersAsync()).Select(f => f.Path).ToArray();
        }

        /// <summary>
        /// Gets the names of the files in the directory at the specified path,
        /// optionally getting the names of files in any subdirectories as well.
        /// </summary>
        /// <param name="directoryPath">The path of the directory whose files are to be retrieved.</param>
        /// <param name="searchOption">A value indicating whether to search subdirectories or just the top directory.</param>
        /// <returns>The files.</returns>
        public async Task<string[]> GetFilesAsync(string directoryPath, Prism.IO.SearchOption searchOption)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            if (searchOption == SearchOption.AllDirectories)
            {
                return (await folder.GetFilesAsync(CommonFileQuery.DefaultQuery)).Select(f => f.Path).ToArray();
            }

            return (await folder.GetFilesAsync()).Select(f => f.Path).ToArray();
        }

        /// <summary>
        /// Moves the directory at the source path to the destination path.
        /// </summary>
        /// <param name="sourceDirectoryPath">The path of the directory to be moved.</param>
        /// <param name="destinationDirectoryPath">The path to where the directory should be moved.</param>
        /// <param name="overwrite">Whether to overwrite any subdirectories or files at the destination path that have identical names to
        /// subdirectories or files at the source path.</param>
        public async Task MoveAsync(string sourceDirectoryPath, string destinationDirectoryPath, bool overwrite)
        {
            var sourceFolder = await StorageFolder.GetFolderFromPathAsync(sourceDirectoryPath);

            StorageFolder destFolder = null;
            try
            {
                destFolder = await StorageFolder.GetFolderFromPathAsync(destinationDirectoryPath);
            }
            catch { }

            if (destFolder == null)
            {
                await CreateAsync(destinationDirectoryPath);
                destFolder = await StorageFolder.GetFolderFromPathAsync(destinationDirectoryPath);
            }

            foreach (var file in await sourceFolder.GetFilesAsync())
            {
                await file.MoveAsync(destFolder, file.Name, overwrite ? NameCollisionOption.ReplaceExisting : NameCollisionOption.FailIfExists);
            }

            foreach (var folder in await sourceFolder.GetFoldersAsync())
            {
                var newFolder = await destFolder.CreateFolderAsync(folder.Name, CreationCollisionOption.OpenIfExists);
                await MoveAsync(folder.Path, newFolder.Path, overwrite);
            }

            await sourceFolder.DeleteAsync();
        }
    }
}
