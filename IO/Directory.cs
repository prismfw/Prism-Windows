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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Prism.IO;
using Prism.Native;
using Prism.Utilities;
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
        /// Gets the directory path to the folder that contains the application's bundled assets.
        /// </summary>
        public string AssetDirectoryPath
        {
            get { return global::Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\"; }
        }

        /// <summary>
        /// Gets the directory path to a folder for storing persisted application data.
        /// </summary>
        public string DataDirectoryPath
        {
            get { return ApplicationData.Current.LocalFolder.Path + "\\"; }
        }

        /// <summary>
        /// Gets the character that is used to separate directories.
        /// </summary>
        public char SeparatorChar
        {
            get { return '\\'; }
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
        public async Task<string[]> GetDirectoriesAsync(string directoryPath, SearchOption searchOption)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            if (searchOption == SearchOption.AllDirectories)
            {
                var queryOptions = new QueryOptions() { FolderDepth = FolderDepth.Deep };
                return (await folder.CreateFolderQueryWithOptions(queryOptions).GetFoldersAsync()).Select(f => f.Path).ToArray();
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
        public async Task<string[]> GetFilesAsync(string directoryPath, SearchOption searchOption)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            if (searchOption == SearchOption.AllDirectories)
            {
                var queryOptions = new QueryOptions() { FolderDepth = FolderDepth.Deep };
                return (await folder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync()).Select(f => f.Path).ToArray();
            }

            return (await folder.GetFilesAsync()).Select(f => f.Path).ToArray();
        }

        /// <summary>
        /// Gets the number of free bytes that are available on the drive that contains the directory at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of a directory on the drive.  If <c>null</c>, the current drive is used.</param>
        /// <returns>The free bytes.</returns>
        public Task<long> GetFreeBytesAsync(string directoryPath)
        {
            return Task.Run(() =>
            {
                ulong freeBytes;
                ulong totalBytes;
                GetDiskSpace(directoryPath, out freeBytes, out totalBytes);

                return (long)freeBytes;
            });
        }

        /// <summary>
        /// Gets information about the specified system directory.
        /// </summary>
        /// <param name="directory">The system directory whose information is to be retrieved.</param>
        /// <returns>Information about the system directory.</returns>
        public async Task<INativeDirectoryInfo> GetSystemDirectoryInfoAsync(SystemDirectory directory)
        {
            return await Task.Run<INativeDirectoryInfo>(async () =>
            {
                switch (directory)
                {
                    case SystemDirectory.Assets:
                        var assetDir = await global::Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                        return new DirectoryInfo(assetDir.Path);
                    case SystemDirectory.Local:
                        return new DirectoryInfo(ApplicationData.Current.LocalFolder.Path);
                    case SystemDirectory.Shared:
                        return ApplicationData.Current.SharedLocalFolder == null ?
                            null : (new DirectoryInfo(ApplicationData.Current.SharedLocalFolder.Path));
                    case SystemDirectory.Temp:
                        return new DirectoryInfo(ApplicationData.Current.TemporaryFolder.Path);
                    case SystemDirectory.External:
                        return new ExternalDirectoryInfo(KnownFolders.RemovableDevices, await KnownFolders.RemovableDevices.GetBasicPropertiesAsync());
                    case SystemDirectory.Music:
                        return new DirectoryInfo((await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music)).SaveFolder.Path);
                    case SystemDirectory.Photos:
                        return new DirectoryInfo((await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures)).SaveFolder.Path);
                    case SystemDirectory.Videos:
                        return new DirectoryInfo((await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos)).SaveFolder.Path);
                    default:
                        // Extras that are not explicitly identified by the core framework library.
                        var value = (int)(directory - Enum.GetValues(typeof(SystemDirectory)).Length);
                        switch (value)
                        {
                            case 0:
                                return new DirectoryInfo(ApplicationData.Current.LocalCacheFolder.Path);
                            case 1:
                                return new DirectoryInfo(ApplicationData.Current.RoamingFolder.Path);
                            case 2:
                                return new DirectoryInfo((await StorageLibrary.GetLibraryAsync(KnownLibraryId.Documents)).SaveFolder.Path);
                            case 3:
                                return new DirectoryInfo(KnownFolders.AppCaptures.Path);
                            case 4:
                                return new DirectoryInfo(KnownFolders.CameraRoll.Path);
                            case 5:
                                return new DirectoryInfo(KnownFolders.HomeGroup.Path);
                            case 6:
                                return new DirectoryInfo(KnownFolders.MediaServerDevices.Path);
                            case 7:
                                return new DirectoryInfo(KnownFolders.Objects3D.Path);
                            case 8:
                                return new DirectoryInfo(KnownFolders.Playlists.Path);
                            case 9:
                                return new DirectoryInfo(KnownFolders.RecordedCalls.Path);
                            case 10:
                                return new DirectoryInfo(KnownFolders.SavedPictures.Path);
                            default:
                                return null;
                        }
                }
            });
        }

        /// <summary>
        /// Gets the total number of bytes on the drive that contains the directory at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of a directory on the drive.  If <c>null</c>, the current drive is used.</param>
        /// <returns>The total bytes.</returns>
        public Task<long> GetTotalBytesAsync(string directoryPath)
        {
            return Task.Run(() =>
            {
                ulong freeBytes;
                ulong totalBytes;
                GetDiskSpace(directoryPath, out freeBytes, out totalBytes);

                return (long)totalBytes;
            });
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

        private static void GetDiskSpace(string directoryPath, out ulong freeBytes, out ulong totalBytes)
        {
            ulong totalFreeBytes;
            if (!GetDiskFreeSpaceEx(directoryPath, out freeBytes, out totalBytes, out totalFreeBytes))
            {
                int errorCode = Marshal.GetLastWin32Error();
                Logger.Error("Disk space query resulted in error code {0} for path {1}", errorCode, directoryPath);

                if (errorCode == 3) // path not found
                {
                    throw new System.IO.FileNotFoundException(null, directoryPath);
                }
                else if (errorCode == 123) // invalid path
                {
                    throw new ArgumentException(nameof(directoryPath));
                }
            }
        }

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static bool GetDiskFreeSpaceEx(string directoryPath, out UInt64 freeBytesAvailable, out UInt64 totalBytes, out UInt64 totalFreeBytes);
    }
}
