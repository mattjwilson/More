﻿namespace More.Windows.Media
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the base implementation for media content.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of media content.</typeparam>
    [ContractClass( typeof( MediaContentContract<> ) )]
    public abstract partial class MediaContent<T>
    {
        /// <summary>
        /// Occurs when the stream is read for reading.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing media content to be read.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing an object of type <typeparamref name="T"/>.</returns>
        protected abstract Task<T> OnReadStreamAsync( Stream stream );

        /// <summary>
        /// Returns the media content from the specified embedded resource asynchronously.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly">assembly</see> that contains the embedded resource.</param>
        /// <param name="resourceName">The name of the embedded resource to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing an object of type <typeparamref name="T"/>.</returns>
        public virtual async Task<T> FromEmbeddedResourceAsync( Assembly assembly, string resourceName )
        {
            Contract.Requires<ArgumentNullException>( assembly != null, "assembly" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( resourceName ), "resourceName" );
            Contract.Ensures( Contract.Result<Task<T>>() != null );

            using ( var stream = assembly.GetManifestResourceStream( resourceName ) )
                return await this.OnReadStreamAsync( stream );
        }

        /// <summary>
        /// Downloads the media contents and returns a sequence that can be observed when complete.
        /// </summary>
        /// <param name="resourceUri">The <see cref="Uri"/> for the resource to download.</param>
        /// <returns>A <see cref="Task{T}">task</see> representing the download operation.</returns>
        public virtual async Task<T> DownloadAsync( Uri resourceUri )
        {
            Contract.Requires<ArgumentNullException>( resourceUri != null, "resourceUri" );
            Contract.Ensures( Contract.Result<Task<T>>() != null );

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseDefaultCredentials = true
            };

            using ( var client = new HttpClient( handler, true ) )
            {
                using ( var stream = await client.GetStreamAsync( resourceUri ) )
                    return await this.OnReadStreamAsync( stream );
            }
        }
    }
}
