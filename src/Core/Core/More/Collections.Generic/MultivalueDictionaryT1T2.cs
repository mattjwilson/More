﻿namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Collections.Specialized;
    using global::System.ComponentModel;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;

    /// <summary>
    /// Represents a dictionary which associates multiple values with a single key.
    /// </summary>
    /// <remarks>
    /// When indexing a <see cref="IMultivalueDictionary{TKey,TValue}">multivalue dictionary</see>, each key returns a <see cref="ICollection{T}">collection</see>
    /// of values. This class also ensures that value <see cref="ICollection{T}">collections</see> are <see cref="INotifyCollectionChanged">observable</see>.
    /// </remarks>
    /// <typeparam name="TKey">The <see cref="Type">type</see> of item key.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of item value.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( DictionaryDebugView<,> ) )]
    public class MultivalueDictionary<TKey, TValue> : Dictionary<TKey, ICollection<TValue>>, IMultivalueDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private Func<IEnumerable<TValue>, ICollection<TValue>> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultivalueDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <remarks>This constructor creates a <see cref="Collection{T}">collection</see> for values, which allows
        /// duplicates and is unordered.</remarks>
        public MultivalueDictionary()
            : base( EqualityComparer<TKey>.Default )
        {
            this.factory = CreateFactory( () => new Collection<TValue>() );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultivalueDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="keyComparer">The <see cref="IEqualityComparer{TKey}">comparer</see> used to compare keys.</param>
        /// <remarks>This constructor creates a <see cref="Collection{T}">collection</see> for values, which allows
        /// duplicates and is unordered.</remarks>
        public MultivalueDictionary( IEqualityComparer<TKey> keyComparer )
            : base( keyComparer )
        {
            Contract.Requires<ArgumentNullException>( keyComparer != null, "keyComparer" );
            this.factory = CreateFactory( () => new Collection<TValue>() );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultivalueDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="valueFactory">The factory <see cref="Func{T}">method</see> used to create
        /// <typeparamref name="TValue">value</typeparamref> <see cref="ICollection{T}">collections</see>.</param>
        public MultivalueDictionary( Func<ICollection<TValue>> valueFactory )
            : base( EqualityComparer<TKey>.Default )
        {
            Contract.Requires<ArgumentNullException>( valueFactory != null, "valueFactory" );
            this.factory = CreateFactory( valueFactory );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultivalueDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="keyComparer">The <see cref="IEqualityComparer{TKey}">comparer</see> used to compare keys.</param>
        /// <param name="valueFactory">The factory <see cref="Func{T}">method</see> used to create
        /// <typeparamref name="TValue">value</typeparamref> <see cref="ICollection{T}">collections</see>.</param>
        public MultivalueDictionary( IEqualityComparer<TKey> keyComparer, Func<ICollection<TValue>> valueFactory )
            : base( keyComparer )
        {
            Contract.Requires<ArgumentNullException>( keyComparer != null, "keyComparer" );
            Contract.Requires<ArgumentNullException>( valueFactory != null, "valueFactory" );
            this.factory = CreateFactory( valueFactory );
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.PropertyChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.CollectionChanged;

            if ( handler != null )
                handler( this, e );
        }

        private static Func<IEnumerable<TValue>, ICollection<TValue>> CreateFactory( Func<ICollection<TValue>> valueFactory )
        {
            Contract.Ensures( Contract.Result<Func<IEnumerable<TValue>, ICollection<TValue>>>() != null );
            var @new = valueFactory;
            return e => @new().AddRange( e );
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="MultivalueDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns><see langkeyword="true">True</see> if the key was removed; otherwise, <see langkeyword="false">false</see>.</returns>
        /// <remarks>If the <see cref="MultivalueDictionary{TKey,TValue}"/> does not contain an element with the specified key,
        /// the <seealso cref="MultivalueDictionary{TKey,TValue}"/>"/> remains unchanged.
        /// <para>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public new bool Remove( TKey key )
        {
            if ( !this.ContainsKey( key ) )
                return false;

            var oldItem = new KeyValuePair<TKey, ICollection<TValue>>( key, base[key].ToArray() );
            var index = this.IndexOf( key );

            var removed = base.Remove( key );

            if ( !removed )
                return false;

            this.OnPropertyChanged( "Count" );
            this.OnPropertyChanged( "Item[]" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, oldItem, index ) );

            return true;
        }

        /// <summary>
        /// Gets or sets the collection with the specified key.
        /// </summary>
        /// <remarks>
        /// <para>If the value is <see langkeyword="null">null</see>, then the key is <see cref="Remove(TKey)">removed</see> instead.</para>
        /// <para>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public new virtual ICollection<TValue> this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                if ( value == null )
                {
                    this.Remove( key );
                    return;
                }

                value = this.factory( value );

                if ( this.ContainsKey( key ) )
                {
                    var collection = base[key];
                    var oldItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                    collection.ReplaceAll( value );
                    var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                    this.OnPropertyChanged( "Item[]" );
                    this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, newItem, oldItem, this.IndexOf( key ) ) );
                    return;
                }

                this.AddRange( key, value );
            }
        }

        /// <summary>
        /// Adds a new collection of values to be associated with a key. If duplicate values are permitted, 
        /// this method always add a new key/value pair to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="values">The <see cref="ICollection{T}">collection</see> of values to add.</param>
        /// <remarks>
        /// <para>
        /// If duplicate values are not permitted, and <paramref name="key"/> already has a value
        /// equal to <paramref name="values"/> associated with it, then that value is replaced,
        /// and the number of values associate with <paramref name="key"/> is unchanged.
        /// </para>
        /// <para>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Add"/> or <see cref="T:NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public new virtual void Add( TKey key, ICollection<TValue> values )
        {
            this.AddRange( key, values );
        }

        /// <summary>
        /// Removes all items from the <see cref="MultivalueDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Reset"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public new virtual void Clear()
        {
            base.Clear();
            this.OnPropertyChanged( "Count" );
            this.OnPropertyChanged( "Item[]" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        void ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Clear()
        {
            this.Clear();
        }

        void ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Add( KeyValuePair<TKey, ICollection<TValue>> item )
        {
            this.Add( item.Key, item.Value );
        }

        bool ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Remove( KeyValuePair<TKey, ICollection<TValue>> item )
        {
            return this.Remove( item.Key );
        }

        [SuppressMessage( "Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This an item in the dictionary not a collection property." )]
        ICollection<TValue> IDictionary<TKey, ICollection<TValue>>.this[TKey key]
        {
            get
            {
                return this[key];
            }
            set
            {
                this[key] = value;
            }
        }

        void IDictionary<TKey, ICollection<TValue>>.Add( TKey key, ICollection<TValue> value )
        {
            this.AddRange( key, value );
        }

        bool IDictionary<TKey, ICollection<TValue>>.Remove( TKey key )
        {
            return this.Remove( key );
        }

        /// <summary>
        /// Determines the index of a specific key in the <see cref="IMultivalueDictionary{TKey,TValue}">dictionary</see>.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IMultivalueDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <returns>The index of <paramref name="key"/> if found in the <see cref="IMultivalueDictionary{TKey,TValue}">dictionary</see>; otherwise, -1.</returns>
        public int IndexOf( TKey key )
        {
            return this.Keys.IndexOf( key, this.Comparer );
        }

        /// <summary>
        /// Adds new values to a key.
        /// </summary>
        /// <param name="key">The key to add values to.</param>
        /// <param name="values">The <see cref="IEnumerable{T}">sequence</see> of values to add.</param>
        /// <remarks>
        /// <para>
        /// If duplicate values are permitted, this method always adds new key/value pairs to the dictionary.
        /// If duplicate values are not permitted, and the <paramref name="key"/> already has a value equal to one of the
        /// <paramref name="values"/> associated with it, then that value is replaced, and the number of values associated with the
        /// <paramref name="key"/> is unchanged.
        /// </para>
        /// <para>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Add"/> or <see cref="T:NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public virtual void AddRange( TKey key, IEnumerable<TValue> values )
        {
            if ( this.ContainsKey( key ) )
            {
                var collection = base[key];
                var oldItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                collection.AddRange( values );
                var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, newItem, oldItem, this.IndexOf( key ) ) );
            }
            else
            {
                var collection = this.factory( values );
                base[key] = collection;
                var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                this.OnPropertyChanged( "Count" );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, newItem, this.Count - 1 ) );
            }
        }

        /// <summary>
        /// Adds a new value to a key.
        /// </summary>
        /// <param name="key">The key to add the value to.</param>
        /// <param name="value">The value to add.</param>
        /// <remarks>If duplicate values are permitted, this method always adds a new key/value pair to the dictionary.
        /// If duplicate values are not permitted, and the <paramref name="key"/> already has a value equal to the
        /// <paramref name="value"/> associated with it, then that value is replaced, and the number of values associated with the
        /// <paramref name="key"/> is unchanged.
        /// <para>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Add"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public virtual void Add( TKey key, TValue value )
        {
            var exists = this.ContainsKey( key );
            var item = this.GetOrAdd( key, () => this.factory( new[] { value } ) );

            if ( exists )
                item.Add( value );
        }

        /// <summary>
        /// Removes a sequence of values from a key. If the last value is removed from a key, then the key is also removed.
        /// </summary>
        /// <param name="key">The key to remove values from.</param>
        /// <param name="values">The <see cref="IEnumerable{T}">sequence</see> of values to remove.</param>
        /// <returns>The number of values that were removed.</returns>
        /// <remarks>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Remove"/> or <see cref="T:NotifyCollectionChangedAction.Replace"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public virtual int RemoveRange( TKey key, IEnumerable<TValue> values )
        {
            if ( !this.ContainsKey( key ) )
                return 0;

            var collection = this[key];
            var oldItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
            var count = values.Count( collection.Remove );

            if ( collection.Count == 0 )
            {
                this.Remove( key );
            }
            else
            {
                var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, newItem, oldItem, this.IndexOf( key ) ) );
            }

            return count;
        }

        /// <summary>
        /// Remove all of the specified keys and their associated values.
        /// </summary>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of key values to remove.</param>
        /// <returns>The number of keys that were removed.</returns>
        /// <remarks>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public virtual int RemoveRange( IEnumerable<TKey> keys )
        {
            var removed = new List<KeyValuePair<TKey, ICollection<TValue>>>();
            var items = from key in keys
                        where this.ContainsKey( key )
                        let item = new KeyValuePair<TKey, ICollection<TValue>>( key, base[key].ToArray() )
                        select item;

            foreach ( var item in items )
            {
                if ( base.Remove( item.Key ) )
                    removed.Add( item );
            }

            if ( removed.Any() )
            {
                this.OnPropertyChanged( "Count" );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, removed.AsReadOnly() ) );
            }

            return removed.Count;
        }

        /// <summary>
        /// Replaces all of the values associated with <paramref name="key"/> with a single <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The replacement value.</param>
        /// <returns><see langkeyword="true">True</see> if the value was replaced for the specified <paramref name="key"/>;
        /// otherwise <see langkeyword="false">false</see>.</returns>
        /// <remarks>This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Replace"/> or <see cref="T:NotifyCollectionChangedAction.Add"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public virtual bool Set( TKey key, TValue value )
        {
            var replaced = this.ContainsKey( key );

            if ( replaced )
            {
                var collection = base[key];
                var oldItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                collection.ReplaceAll( new[] { value } );
                var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, newItem, oldItem, this.IndexOf( key ) ) );
            }
            else
            {
                var collection = this.factory( new[] { value } );
                base.Add( key, collection );
                var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                this.OnPropertyChanged( "Count" );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, newItem, this.IndexOf( key ) ) );
            }

            return replaced;
        }

        /// <summary>
        /// Replaces all values associated for the specified <paramref name="key"/> with a new collection of values. If the collection does not
        /// permit duplicate values, and <paramref name="values"/> has duplicate items, then only the distinct list of values is added.
        /// </summary>
        /// <param name="key">The key to set values for.</param>
        /// <param name="values">The <see cref="IEnumerable{T}">sequence</see> of new values set.</param>
        /// <returns><see langkeyword="true">True</see> if any values were replaced for the specified <paramref name="key"/>;
        /// otherwise <see langkeyword="false">false</see>.</returns>
        /// <remarks>This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as <see cref="T:NotifyCollectionChangedAction.Replace"/> or <see cref="T:NotifyCollectionChangedAction.Add"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public virtual bool SetRange( TKey key, IEnumerable<TValue> values )
        {
            var replaced = this.ContainsKey( key );

            if ( replaced )
            {
                var collection = base[key];
                var oldItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                collection.ReplaceAll( this.factory( values ) );
                var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, newItem, oldItem, this.IndexOf( key ) ) );
            }
            else
            {
                var collection = this.factory( values );
                base.Add( key, collection );
                var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                this.OnPropertyChanged( "Count" );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, newItem, this.IndexOf( key ) ) );
            }

            return replaced;
        }

        /// <summary>
        /// Removes a given value from the values associated with a key. If the last value is removed from a key, then the key is also removed.
        /// </summary>
        /// <param name="key">The key to remove a value from.</param>
        /// <param name="value">The value to remove.</param>
        /// <returns><see langkeyword="true">True</see> if the <paramref name="value"/> was removed; otherwise, <see langkeyword="false">false</see>.</returns>
        /// <remarks>This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Action"/> as "Remove" or "Replace"</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public virtual bool Remove( TKey key, TValue value )
        {
            if ( !this.ContainsKey( key ) )
                return false;

            var collection = base[key];
            var oldItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
            var removed = collection.Remove( value );

            if ( removed )
            {
                var newItem = new KeyValuePair<TKey, ICollection<TValue>>( key, collection.ToArray() );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, newItem, oldItem, this.IndexOf( key ) ) );
            }

            if ( collection.Count == 0 )
                this.Remove( key );

            return removed;
        }

        /// <summary>
        /// Determines if this dictionary contains a key/value pair equal to <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">The value to locate.</param>
        /// <returns>True if the dictionary has an associated <paramref name="value"/> with the specified <paramref name="key"/>.</returns>
        public virtual bool Contains( TKey key, TValue value )
        {
            return this.ContainsKey( key ) && this[key].Contains( value );
        }

        /// <summary>
        /// Returns the count of values associated with a key.
        /// </summary>
        /// <param name="key">The key to count values for.</param>
        /// <returns>If the <paramref name="key"/> exists, then the number of values associated with that <paramref name="key"/>.</returns>
        public virtual int CountValues( TKey key )
        {
            return this.ContainsKey( key ) ? this[key].Count : 0;
        }

        /// <summary>
        /// Returns the total count of values in the collection.
        /// </summary>
        /// <returns>The total number of values associated with all keys in the dictionary.</returns>
        public virtual int CountAllValues()
        {
            return this.Sum( i => i.Value.Count );
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have
        /// changed by using either <c>null</c>or <see cref="F:String.Empty"/> as the
        /// property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        /// <remarks>The event handler receives an argument of type <see cref="NotifyCollectionChangedEventArgs"/>,
        /// which contains data that is related to this event.</remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
