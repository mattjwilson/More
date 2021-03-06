﻿namespace More.Windows.Data
{
    using global::System;
    using global::System.Collections.Generic;
    using global::Windows.ApplicationModel.DataTransfer;
    using global::Windows.Foundation;
    using global::Windows.Foundation.Metadata;
    using global::Windows.Storage;
    using global::Windows.Storage.Streams;
    using global::Windows.UI;

    /// <summary>
    /// Defines the behavior of a data package that contains the data a user wants to exchange with another application.
    /// </summary>
    [CLSCompliant( false )]
    public interface IDataPackage
    {
        /// <summary>
        /// Allows you to get and set properties like the title of the content being shared.
        /// </summary>
        /// <value>A <see cref="IDataPackagePropertySet">collection</see> of properties that describe the data contained in a <see cref="IDataPackage"/>.</value>
        IDataPackagePropertySet Properties
        {
            get;
        }

        /// <summary>
        /// Specifies the data package operation (none, move, copy, or link).
        /// </summary>
        /// <value>One of the <see cref="DataPackageOperation"/> values.</value>
        DataPackageOperation RequestedOperation
        {
            get;
            set;
        }

        /// <summary>
        /// Maps a URI to a file. 
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}">dictionary</see> that specifies the an HTML path with a
        /// corresponding <see cref="IRandomAccessStreamReference">stream reference</see> object.</value>
        /// <remarks>Used to ensure that referenced content (such as an image) in HTML content is added to the <see cref="IDataPackage"/>.</remarks>
        IDictionary<string, IRandomAccessStreamReference> ResourceMap
        {
            get;
        }

        /// <summary>
        /// Occurs when the <see cref="IDataPackage"/> is destroyed.
        /// </summary>
        event TypedEventHandler<IDataPackage, object> Destroyed;

        /// <summary>
        /// Occurs when a paste operation is completed.
        /// </summary>
        event TypedEventHandler<IDataPackage, OperationCompletedEventArgs> OperationCompleted;

        /// <summary>
        /// Returns a read-only copy of the <see cref="IDataPackage"/>.
        /// </summary>
        /// <returns>A <see cref="IDataPackageView"/> object.</returns>
        IDataPackageView GetView();

        /// <summary>
        /// Sets the application link that a <see cref="IDataPackage"/> contains.
        /// </summary>
        /// <param name="value">A <see cref="Uri">Uniform Resource Identifier (URI)</see> with a scheme that isn't HTTP or HTTPS
        /// that's handled by the source application.</param>
        void SetApplicationLink( Uri value );

        /// <summary>
        /// Sets the bitmap image contained in the <see cref="IDataPackage"/>.
        /// </summary>
        /// <param name="value">A <see cref="IRandomAccessStreamReference">stream</see> that contains the bitmap image.</param>
        void SetBitmap( IRandomAccessStreamReference value );

        /// <summary>
        /// Sets the data contained in the <see cref="IDataPackage"/> in a random access stream.
        /// </summary>
        /// <param name="formatId">Specifies the format of the data.</param>
        /// <param name="value">Specifies the content that the data package contains.</param>
        void SetData( string formatId, object value );

        //
        // Summary:
        //     Sets a delegate to handle requests from the target app.
        //
        // Parameters:
        //   formatId:
        //     Specifies the format of the data. We recommend that you set this value by
        //     using the StandardDataFormats class.
        //
        //   delayRenderer:
        //     A delegate that is responsible for processing requests from a target app.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatId"></param>
        /// <param name="delayRenderer"></param>
        void SetDataProvider( string formatId, DataProviderHandler delayRenderer );

        /// <summary>
        /// Adds HTML content to the <see cref="IDataPackage"/>.
        /// </summary>
        /// <param name="value">The HTML content to set.</param>
        void SetHtmlFormat( string value );

        /// <summary>
        /// Sets the Rich Text Format (RTF) content that is contained in a <see cref="IDataPackage"/>.
        /// </summary>
        /// <param name="value">The Rich Text Format (RTF) content to set.</param>
        void SetRtf( string value );

        /// <summary>
        /// Sets the files and folders contained in a <see cref="IDataPackage"/>.
        /// </summary>
        /// <param name="value">The <see cref="IEnumerable{T}">sequence</see> of <see cref="IStorageItem">files and folders</see> to be added.</param>
        void SetStorageItems( IEnumerable<IStorageItem> value );

        /// <summary>
        /// Adds files and folders to a <see cref="IDataPackage"/>.
        /// </summary>
        /// <param name="value">The <see cref="IEnumerable{T}">sequence</see> of <see cref="IStorageItem">files and folders</see> to be added.</param>
        /// <param name="readOnly">True if the files are read-only; false otherwise.</param>
        void SetStorageItems( IEnumerable<IStorageItem> value, bool readOnly );

        /// <summary>
        /// Sets the text that a <see cref="IDataPackage"/> contains.
        /// </summary>
        /// <param name="value">The text to set.</param>
        void SetText( string value );

        /// <summary>
        /// Sets the web link that a <see cref="IDataPackage"/> contains.
        /// </summary>
        /// <param name="value">A <see cref="Uri">Uniform Resource Identifier (URI)</see> with an HTTP or HTTPS
        /// scheme that corresponds to the content being displayed to the user.</param>
        void SetWebLink( Uri value );
    }
}
