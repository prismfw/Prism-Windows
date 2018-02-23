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
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Prism.Native;
using Windows.Storage;
using Windows.Storage.Streams;

using FileMode = Prism.IO.FileMode;

namespace Prism.Windows.IO
{
    /// <summary>
    /// Represents a Windows implementation of an <see cref="INativeFile"/>.
    /// </summary>
    [Register(typeof(INativeFile), IsSingleton = true)]
    public class File : INativeFile
    {
        /// <summary>
        /// Opens the file at the specified path, appends the specified bytes to the end of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The path of the file in which to append the bytes.</param>
        /// <param name="bytes">The bytes to append to the end of the file.</param>
        public async Task AppendBytesAsync(string filePath, byte[] bytes)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(filePath));
            var file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.OpenIfExists);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                stream.Seek(stream.Size);
                using (var dataWriter = new DataWriter(stream))
                {
                    dataWriter.WriteBytes(bytes);
                    await dataWriter.StoreAsync();
                    await dataWriter.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Opens the file at the specified path, appends the specified bytes to the end of the file, and then closes the file.
        /// If the file does not exist, one is created.
        /// </summary>
        /// <param name="filePath">The path of the file in which to append the bytes.</param>
        /// <param name="bytes">The bytes to append to the end of the file.</param>
        public async Task AppendAllBytesAsync(string filePath, byte[] bytes)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            StorageFolder folder = null;
            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }
            catch { }

            if (folder == null)
            {
                await Prism.IO.Directory.CreateAsync(directoryPath);
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }

            var file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.OpenIfExists);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                stream.Seek(stream.Size);
                using (var dataWriter = new DataWriter(stream))
                {
                    dataWriter.WriteBytes(bytes);
                    await dataWriter.StoreAsync();
                    await dataWriter.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Opens the file at the specified path, appends the specified text to the end of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The path of the file in which to append the text.</param>
        /// <param name="text">The text to append to the end of the file.</param>
        public async Task AppendTextAsync(string filePath, string text)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(filePath));
            var file = await folder.GetFileAsync(Path.GetFileName(filePath));
            await FileIO.AppendTextAsync(file, text);
        }

        /// <summary>
        /// Opens the file at the specified path, appends the specified text to the end of the file, and then closes the file.
        /// If the file does not exist, one is created.
        /// </summary>
        /// <param name="filePath">The path of the file in which to append the text.</param>
        /// <param name="text">The text to append to the end of the file.</param>
        public async Task AppendAllTextAsync(string filePath, string text)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            StorageFolder folder = null;
            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }
            catch { }

            if (folder == null)
            {
                await Prism.IO.Directory.CreateAsync(directoryPath);
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }

            var file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(file, text);
        }

        /// <summary>
        /// Copies the file at the source path to the destination path, overwriting any existing file.
        /// </summary>
        /// <param name="sourceFilePath">The path of the file to be copied.</param>
        /// <param name="destinationFilePath">The path to where the copied file should be placed.</param>
        public async Task CopyAsync(string sourceFilePath, string destinationFilePath)
        {
            var sourceFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(sourceFilePath));
            var sourceFile = await sourceFolder.GetFileAsync(Path.GetFileName(sourceFilePath));

            string destDirectory = Path.GetDirectoryName(destinationFilePath);
            StorageFolder destFolder = null;
            try
            {
                destFolder = await StorageFolder.GetFolderFromPathAsync(destDirectory);
            }
            catch { }

            if (destFolder == null)
            {
                await Prism.IO.Directory.CreateAsync(destDirectory);
                destFolder = await StorageFolder.GetFolderFromPathAsync(destDirectory);
            }

            await sourceFile.CopyAsync(destFolder, Path.GetFileName(destinationFilePath), NameCollisionOption.ReplaceExisting);
        }

        /// <summary>
        /// Creates a file at the specified path, overwriting any existing file.
        /// </summary>
        /// <param name="filePath">The path at which to create the file.</param>
        /// <param name="bufferSize">The number of bytes buffered for reading and writing to the file.</param>
        public async Task<Stream> CreateAsync(string filePath, int bufferSize)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            StorageFolder folder = null;
            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }
            catch { }

            if (folder == null)
            {
                await Prism.IO.Directory.CreateAsync(directoryPath);
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }

            var file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.ReplaceExisting);
            return (await file.OpenAsync(FileAccessMode.ReadWrite)).AsStream(bufferSize);
        }

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="filePath">The path of the file to delete.</param>
        public async Task DeleteAsync(string filePath)
        {
            try
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(filePath));
                var file = await folder.GetFileAsync(Path.GetFileName(filePath));
                await file.DeleteAsync();
            }
            catch { }
        }

        /// <summary>
        /// Moves the file at the source path to the destination path, overwriting any existing file.
        /// </summary>
        /// <param name="sourceFilePath">The path of the file to be moved.</param>
        /// <param name="destinationFilePath">The path to where the file should be moved.</param>
        public async Task MoveAsync(string sourceFilePath, string destinationFilePath)
        {
            var sourceFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(sourceFilePath));
            var sourceFile = await sourceFolder.GetFileAsync(Path.GetFileName(sourceFilePath));

            string destDirectory = Path.GetDirectoryName(destinationFilePath);
            StorageFolder destFolder = null;
            try
            {
                destFolder = await StorageFolder.GetFolderFromPathAsync(destDirectory);
            }
            catch { }

            if (destFolder == null)
            {
                await Prism.IO.Directory.CreateAsync(destDirectory);
                destFolder = await StorageFolder.GetFolderFromPathAsync(destDirectory);
            }

            await sourceFile.MoveAsync(destFolder, Path.GetFileName(destinationFilePath), NameCollisionOption.ReplaceExisting);
        }

        /// <summary>
        /// Opens the file at the specified path, optionally creating one if it doesn't exist.
        /// </summary>
        /// <param name="filePath">The path of the file to be opened.</param>
        /// <param name="mode">The manner in which the file should be opened.</param>
        public async Task<Stream> OpenAsync(string filePath, FileMode mode)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            StorageFolder folder = null;
            StorageFile file = null;

            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }
            catch { }

            if (folder == null)
            {
                if (mode == FileMode.Open)
                {
                    throw new FileNotFoundException();
                }

                await Prism.IO.Directory.CreateAsync(directoryPath);
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }

            switch (mode)
            {
                case FileMode.Create:
                    file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.ReplaceExisting);
                    break;
                case FileMode.Open:
                    file = await folder.GetFileAsync(Path.GetFileName(filePath));
                    break;
                case FileMode.OpenOrCreate:
                    file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.OpenIfExists);
                    break;
            }

            return (await file.OpenAsync(FileAccessMode.ReadWrite)).AsStream();
        }

        /// <summary>
        /// Opens the file at the specified path, reads all of the bytes in the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The path of the file from which to read the bytes.</param>
        /// <returns>The all bytes.</returns>
        public async Task<byte[]> ReadAllBytesAsync(string filePath)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(filePath));
            var file = await folder.GetFileAsync(Path.GetFileName(filePath));
            return (await FileIO.ReadBufferAsync(file)).ToArray();
        }

        /// <summary>
        /// Opens the file at the specified path, reads all of the text in the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">The path of the file from which to read the text.</param>
        /// <returns>The all text.</returns>
        public async Task<string> ReadAllTextAsync(string filePath)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(filePath));
            var file = await folder.GetFileAsync(Path.GetFileName(filePath));
            return await FileIO.ReadTextAsync(file);
        }

        /// <summary>
        /// Creates a new file at the specified path, writes the specified bytes to the file, and then closes the file.
        /// If a file already exists, it is overwritten.
        /// </summary>
        /// <param name="filePath">The path of the file in which to write the bytes.</param>
        /// <param name="bytes">The bytes to write to the file.</param>
        public async Task WriteAllBytesAsync(string filePath, byte[] bytes)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            StorageFolder folder = null;
            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }
            catch { }

            if (folder == null)
            {
                await Prism.IO.Directory.CreateAsync(directoryPath);
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }

            var file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file, bytes);
        }

        /// <summary>
        /// Creates a new file at the specified path, writes the specified text to the file, and then closes the file.
        /// If a file already exists, it is overwritten.
        /// </summary>
        /// <param name="filePath">The path of the file in which to write the text.</param>
        /// <param name="text">The text to write to the file.</param>
        public async Task WriteAllTextAsync(string filePath, string text)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            StorageFolder folder = null;
            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }
            catch { }

            if (folder == null)
            {
                await Prism.IO.Directory.CreateAsync(directoryPath);
                folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            }

            var file = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, text);
        }
    }
}
