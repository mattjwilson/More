﻿namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.ComponentModel;
    using global::System.Diagnostics;    
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;

    /// <summary>
    /// Represents a node collection.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of node value.</typeparam>
    [DebuggerDisplay( "Depth = {Depth}, Count = {Count}, Value = {Value}" )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is a special case where the node is an item and a collection." )]
    public class Node<T> : ObservableCollection<Node<T>>
    {
        private readonly IComparer<T> valueComparer;
        private readonly IEqualityComparer<Node<T>> itemComparer;
        private T currentValue;
        private int depth;
        private Node<T> parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class with the supplied <see cref="IEqualityComparer{T}">comparer</see>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}">comparer</see> used to evaluate item values.</param>
        public Node( IComparer<T> comparer )
            : this( default( T ), comparer )
        {
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class.
        /// </summary>
        public Node()
            : this( default( T ), Comparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class.
        /// </summary>
        /// <param name="value">The node value.</param>
        public Node( T value )
            : this( value, Comparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class.
        /// </summary>
        /// <param name="value">The node value.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> used to evaluate item values.</param>
        public Node( T value, IComparer<T> comparer )
        {
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );
            this.valueComparer = comparer;
            this.itemComparer = new ComparerAdapter<Node<T>, T>( comparer, i => i.Value );
            this.currentValue = value;
        }

        /// <summary>
        /// Gets the value comparer for the current node.
        /// </summary>
        /// <value>The <see cref="IComparer{T}"/> used to compare node values.</value>
        protected IComparer<T> ValueComparer
        {
            get
            {
                Contract.Ensures( this.valueComparer != null );
                return this.valueComparer;
            }
        }

        /// <summary>
        /// Gets the item comparer for the current node.
        /// </summary>
        /// <value>The <see cref="IEqualityComparer{T}"/> used to compare nodes.</value>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generic support" )]
        protected virtual IEqualityComparer<Node<T>> ItemComparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<Node<T>>>() != null );
                return this.itemComparer;
            }
        }

        /// <summary>
        /// Gets or sets the node value.
        /// </summary>
        /// <value>The node value.</value>
        public T Value
        {
            get
            {
                return this.currentValue;
            }
            set
            {
                if ( this.ValueComparer.Compare( this.currentValue, value ) == 0 )
                    return;

                this.currentValue = value;
                this.OnPropertyChanged( "Value" );
            }
        }

        /// <summary>
        /// Gets the depth of the current node.
        /// </summary>
        /// <value>The node depth.  The default value is 0.</value>
        public int Depth
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return this.depth;
            }
            private set
            {
                Contract.Requires( value >= 0, "value" );

                if ( this.depth == value )
                    return;

                this.depth = value;
                this.OnPropertyChanged( "Depth" );
            }
        }

        /// <summary>
        /// Gets the parent of the current node.
        /// </summary>
        /// <value>A <see cref="Node{T}"/> object.  The default value is null.</value>
        public Node<T> Parent
        {
            get
            {
                return this.parent;
            }
            private set
            {
                if ( this.ItemComparer.Equals( this.parent, value ) )
                    return;

                this.parent = value;
                this.OnPropertyChanged( "Parent" );
                UpdateDepth( this );
            }
        }

        private static void UpdateDepth( Node<T> current )
        {
            Contract.Requires( current != null, "current" );
            var queue = new Queue<Node<T>>();

            queue.Enqueue( current );

            while ( queue.Count > 0 )
            {
                current = queue.Dequeue();
                Contract.Assume( current != null );
                current.Depth = current.Parent == null ? 0 : current.Parent.Depth + 1;
                current.ForEach( queue.Enqueue );
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged( string propertyName )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Inserts a value to the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the insertion takes place.</param>
        /// <param name="value">The value of type <typeparamref name="T"/> to insert.</param>
        public virtual void Insert( int index, T value )
        {
            this.Insert( index, new Node<T>( value, this.ValueComparer ) );
        }

        /// <summary>
        /// Adds a value to the collection.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="T"/> to add.</param>
        public virtual void Add( T value )
        {
            this.Add( new Node<T>( value, this.ValueComparer ) );
        }

        /// <summary>
        /// Adds a sequence of values to the collection.
        /// </summary>
        /// <param name="values">The <see cref="IEnumerable{T}">sequence</see> of values to add.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public void AddRange( IEnumerable<T> values )
        {
            Contract.Requires<ArgumentNullException>( values != null, "values" );
            this.AddRange( values.ToList().Select( value => new Node<T>( value, this.ValueComparer ) ) );
        }

        /// <summary>
        /// Removes a value from the collection.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="T"/> to remove.</param>
        /// <returns>True if the value is removed; otherwise, false.</returns>
        public virtual bool Remove( T value )
        {
            var index = this.FindIndex( item => this.ValueComparer.Compare( item.Value, value ) == 0 );

            if ( index < 0 )
                return false;

            this.RemoveAt( index );
            return true;
        }

        /// <summary>
        /// Overrides the default behavior when an item is removed from the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the removal takes place.</param>
        protected override void RemoveItem( int index )
        {
            Contract.Assume( index >= 0 && index < this.Count );
            var item = this[index];
            base.RemoveItem( index );

            Contract.Assume( item != null );
            item.Parent = null;
        }

        /// <summary>
        /// Overrides the default behavior when an item is inserted into the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the insertion takes place.</param>
        /// <param name="item">The <see cref="Node{T}"/> to insert.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Facilitated by sub and super types." )]
        protected override void InsertItem( int index, Node<T> item )
        {
            base.InsertItem( index, item );
            Contract.Assume( item != null );
            item.Parent = this;
        }

        /// <summary>
        /// Overrides the default behavior when an item is replaced in the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the replacement takes place.</param>
        /// <param name="item">The new <see cref="Node{T}"/> item.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Facilitated by sub and super types." )]
        protected override void SetItem( int index, Node<T> item )
        {
            base.SetItem( index, item );
            Contract.Assume( item != null );
            item.Parent = this;
        }
    }
}
