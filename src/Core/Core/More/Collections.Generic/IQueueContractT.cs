﻿namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IQueue<> ) )]
    internal abstract class IQueueContract<T> : IQueue<T>
    {
        T IQueue<T>.Peek()
        {
            Contract.Requires<InvalidOperationException>( ( (ICollection) this ).Count > 0 );
            return default( T );
        }

        T IQueue<T>.Dequeue()
        {
            Contract.Requires<InvalidOperationException>( ( (ICollection) this ).Count > 0 );
            return default( T );
        }

        void IQueue<T>.Enqueue( T item )
        {
            Contract.Ensures( ( (ICollection) this ).Count > 0 );
        }

        void IQueue<T>.Clear()
        {
            Contract.Ensures( ( (ICollection) this ).Count == 0 );
        }

        bool IQueue<T>.Contains( T item )
        {
            return default( bool );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "This is only a code contract definition." )]
        void IQueue<T>.CopyTo( T[] array, int arrayIndex )
        {
            Contract.Requires<ArgumentNullException>( array != null, "array" );
            Contract.Requires<ArgumentException>( array.Rank == 1, "array.Rank" );
            Contract.Requires<ArgumentOutOfRangeException>( arrayIndex >= 0, "arrayIndex" );
            Contract.Requires<ArgumentOutOfRangeException>( arrayIndex <= ( array.Length + ( (ICollection) this ).Count ), "arrayIndex" );
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return default( IEnumerator<T> );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default( IEnumerator );
        }

        void ICollection.CopyTo( Array array, int index )
        {
        }

        int ICollection.Count
        {
            get
            {
                return default( int );
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return default( bool );
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return default( object );
            }
        }
    }
}
