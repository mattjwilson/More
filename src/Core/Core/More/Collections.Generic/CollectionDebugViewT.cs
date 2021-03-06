﻿namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;

    /// <summary>
    /// Represents debugging class used to visualize an instance of the <see cref="ICollection{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of items in the collection.</typeparam>
    public sealed class CollectionDebugView<T>
    {
        private readonly ICollection<T> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionDebugView{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="ICollection{T}">collection</see> to debug.</param>
        public CollectionDebugView( ICollection<T> collection )
        {
            Contract.Requires<ArgumentNullException>( collection != null, "collection" );
            this.source = collection;
        }


        /// <summary>
        /// Gets the debugger view of a specific <see cref="ICollection{T}"/> instance.
        /// </summary>
        /// <value>The debugger view of a specific <see cref="ICollection{T}"/> instance.</value>
        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is the convention for the debugger visualization system." )]
        public T[] Items
        {
            get
            {
                Contract.Ensures( this.source != null );
                return this.source.ToArray();
            }
        }
    }
}
