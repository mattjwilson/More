﻿namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// Provides extension methods to convert <see cref="IStorageItem">storage items</see> to
    /// their platform-specific implementations.
    /// </summary>
    public static class IStorageItemExtensions
    {
        /// <summary>
        /// Returns the platform-specific file information.
        /// </summary>
        /// <param name="file">The <see cref="IFile">file</see> to convert.</param>
        /// <returns>The platform-specific <see cref="FileInfo">file information</see>.</returns>
        public static FileInfo AsFileInfo( this IFile file )
        {
            Contract.Requires<ArgumentNullException>( file != null, "file" );
            Contract.Ensures( Contract.Result<FileInfo>() != null );

            var platform = file as IPlatformStorageItem<FileInfo>;

            if ( platform == null )
                throw new NotSupportedException( ExceptionMessage.NativeFileNotSupported.FormatDefault( file.GetType(), typeof( IPlatformStorageItem<FileInfo> ) ) );

            return platform.NativeStorageItem;
        }

        /// <summary>
        /// Returns the platform-specific folder information.
        /// </summary>
        /// <param name="folder">The <see cref="IFolder">folder</see> to convert.</param>
        /// <returns>The platform-specific <see cref="DirectoryInfo">folder information</see>.</returns>
        public static DirectoryInfo AsDirectoryInfo( this IFolder folder )
        {
            Contract.Requires<ArgumentNullException>( folder != null, "folder" );
            Contract.Ensures( Contract.Result<DirectoryInfo>() != null );

            var platform = folder as IPlatformStorageItem<DirectoryInfo>;

            if ( platform == null )
                throw new NotSupportedException( ExceptionMessage.NativeFolderNotSupported.FormatDefault( folder.GetType(), typeof( IPlatformStorageItem<DirectoryInfo> ) ) );

            return platform.NativeStorageItem;
        }
    }
}
