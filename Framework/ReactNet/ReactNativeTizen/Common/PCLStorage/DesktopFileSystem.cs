﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tizen.Applications;

namespace PCLStorage
{
    /// <summary>
    /// Implementation of <see cref="IFileSystem"/> over classic .NET file I/O APIs
    /// </summary>
    public class DesktopFileSystem : IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        public IFolder LocalStorage
        {
            get
            {
                return new FileSystemFolder(Application.Current.DirectoryInfo.Data);
            }
        }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user
        /// </summary>
        public IFolder RoamingStorage
        {
            get
            {

                return null;
                //  SpecialFolder.ApplicationData is not app-specific, so use the Windows Forms API to get the app data path
                //var roamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                //var roamingAppData = System.Windows.Forms.Application.UserAppDataPath;
                //return new FileSystemFolder(roamingAppData);
            }
        }

        /// <summary>
        /// Gets a file, given its path.  Returns null if the file does not exist.
        /// </summary>
        /// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A file for the given path, or null if it does not exist.</returns>
        public async Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(path, "path");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (File.Exists(path))
            {
                return new FileSystemFile(path);
            }

            return null;
        }

        /// <summary>
        /// Gets a folder, given its path.  Returns null if the folder does not exist.
        /// </summary>
        /// <param name="path">The path to a folder, as returned from the <see cref="IFolder.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A folder for the specified path, or null if it does not exist.</returns>
        public async Task<IFolder> GetFolderFromPathAsync(string path, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(path, "path");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            if (Directory.Exists(path))
            {
                return new FileSystemFolder(path, true);
            }

            return null;
        }
    }
}
