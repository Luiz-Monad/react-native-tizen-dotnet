﻿using System;

namespace PCLStorage
{
    /// <summary>
    /// Provides access to an implementation of <see cref="IFileSystem"/> for the current platform
    /// </summary>
    public static class FileSystem
    {
        static Lazy<IFileSystem> _fileSystem = new Lazy<IFileSystem>(() => CreateFileSystem(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// The implementation of <see cref="IFileSystem"/> for the current platform
        /// </summary>
        public static IFileSystem Current
        {
            get
            {
                IFileSystem ret = _fileSystem.Value;
                if (ret == null)
                {
                    throw FileSystem.NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IFileSystem CreateFileSystem()
        {
            return new DesktopFileSystem();
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the PCLStorage NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
