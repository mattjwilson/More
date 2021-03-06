﻿namespace More.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a mediator that can coordinate columns between a
    /// <see cref="DataGrid">data grid</see> and a <see cref="IList">list</see>.
    /// </summary>
    internal sealed class ColumnsMediator : IDisposable
    {
        private volatile bool disposed;
        private volatile bool suppressCollectionChanged;
        private volatile bool suppressColumnsChanged;
        private DataGrid dataGrid;
        private ICollection<DataGridColumn> columns;
        private INotifyCollectionChanged columnEvents;
        internal readonly long Key;

        internal ColumnsMediator( DataGrid dataGrid, IEnumerable<DataGridColumn> sequence, INotifyCollectionChanged columnEvents )
        {
            Contract.Requires( dataGrid != null );
            Contract.Requires( sequence != null );
            Contract.Requires( columnEvents != null );

            this.Key = CreateKey( dataGrid, sequence );
            this.columns = sequence as ICollection<DataGridColumn>;
            this.columnEvents = columnEvents;
            this.columnEvents.CollectionChanged += this.OnCollectionChanged;
            this.dataGrid = dataGrid;

            // if the provided sequence isn't a collection, then mediation is one-way.
            // the sequence can notify the data grid, but not the other way around.
            if ( this.columns != null )
                this.dataGrid.Columns.CollectionChanged += this.OnColumnsChanged;
        }

        private bool IsSuppressingEvents
        {
            get
            {
                return this.suppressColumnsChanged || this.suppressCollectionChanged;
            }
        }

        private void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;

            if ( !disposing )
                return;

            if ( this.dataGrid != null )
            {
                this.dataGrid.Columns.CollectionChanged -= this.OnColumnsChanged;
                this.dataGrid = null;
            }

            if ( this.columnEvents != null )
            {
                this.columnEvents.CollectionChanged -= this.OnCollectionChanged;
                this.columnEvents = null;
            }

            this.columns = null;
        }

        internal static long CreateKey( DataGrid dataGrid, object items )
        {
            Contract.Requires( dataGrid != null );
            var hash = ( dataGrid.GetHashCode() << 32 ) | ( items == null ? 0 : items.GetHashCode() );
            return hash;
        }

        private static void SynchronizeCollection( ICollection<DataGridColumn> columns, NotifyCollectionChangedEventArgs e, bool fastClear )
        {
            Contract.Requires( columns != null, "columns" );
            Contract.Requires( e != null, "e" );

            if ( e.Action == NotifyCollectionChangedAction.Reset )
            {
                if ( fastClear )
                {
                    // clear all columns
                    columns.Clear();
                }
                else
                {
                    // HACK: yet another blunder in the DataGrid (at least for Silverlight).  I'm making an assuming this will hold true in WPF
                    // as well.  Calling Clear() on the DataGrid.Columns property does not correctly calculate it's internal indexes.  An
                    // ArgumentOutOfRangeException is thrown from an internal List<T>, but it's difficult to say what the exact problem. Through
                    // trial and error, removing columns one at a time proved to produce reliable results; therefore, this branch of the code
                    // uses Remove() or RemoveAt() instead of Clear().

                    var list = columns as IList<DataGridColumn>;

                    if ( list != null )
                    {
                        for ( var i = list.Count - 1; i > -1; i-- )
                            list.RemoveAt( i );
                    }
                    else
                    {
                        list = new List<DataGridColumn>( columns );

                        for ( var i = list.Count - 1; i > -1; i-- )
                            columns.Remove( list[i] );
                    }
                }

                return;
            }

            // remove old columns
            if ( e.OldItems != null )
            {
                foreach ( DataGridColumn column in e.OldItems )
                    columns.Remove( column );
            }

            // add new columns
            if ( e.NewItems != null )
            {
                foreach ( DataGridColumn column in e.NewItems )
                    if ( !columns.Contains( column ) )
                        columns.Add( column );
            }
        }

        private void OnColumnsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            // exit if events are suppressed
            if ( this.IsSuppressingEvents )
                return;

            // suppress collection changed events
            this.suppressCollectionChanged = true;

            SynchronizeCollection( this.columns, e, true );

            // enable events
            this.suppressCollectionChanged = false;
        }

        private void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            // exit if events are suppressed
            if ( this.IsSuppressingEvents )
                return;

            // suppress column changed events
            this.suppressColumnsChanged = true;

            SynchronizeCollection( this.dataGrid.Columns, e, false );

            // enable events
            this.suppressColumnsChanged = false;
        }

        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
