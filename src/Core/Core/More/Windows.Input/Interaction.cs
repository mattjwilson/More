﻿namespace More.Windows.Input
{
    using More.Collections.Generic;
    using More.ComponentModel;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Collections.Specialized;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Windows.Input;

    /// <summary>
    /// Represents a user interaction.
    /// </summary>
    public class Interaction : ObservableObject
    {
        private readonly ObservableKeyedCollection<string, INamedCommand> commands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        private readonly Lazy<IDictionary<string, object>> continuationData = new Lazy<IDictionary<string, object>>( () => new Dictionary<string, object>() );
        private string title;
        private object content;
        private int defaultCommandIndex = -1;
        private int cancelCommandIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interaction"/> class.
        /// </summary>
        public Interaction()
            : this( string.Empty, null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interaction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        public Interaction( string title )
            : this( title, null )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interaction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="content">The content associated with the interaction.</param>
        public Interaction( string title, object content )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );

            this.title = title;
            this.content = content;
        }

        /// <summary>
        /// Gets or sets the title of the interaction.
        /// </summary>
        /// <value>The interaction title.</value>
        public string Title
        {
            get
            {
                Contract.Ensures( this.title != null );
                return this.title;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.SetProperty( ref this.title, value );
            }
        }

        /// <summary>
        /// Gets or sets the interaction content associated with the request.
        /// </summary>
        /// <value>The interaction content.</value>
        public object Content
        {
            get
            {
                return this.content;
            }
            set
            {
                this.SetProperty( ref this.content, value );
            }
        }

        /// <summary>
        /// Gets the commands associated with the interaction.
        /// </summary>
        /// <value>A <see cref="KeyedCollection{TKey,TValue}">list</see> of <see cref="INamedCommand">commands</see>.</value>
        /// <remarks>The default implementation supports <see cref="INotifyCollectionChanged"/>.</remarks>
        public virtual KeyedCollection<string, INamedCommand> Commands
        {
            get
            {
                Contract.Ensures( this.commands != null );
                return this.commands;
            }
        }

        /// <summary>
        /// Gets or sets the default command index.
        /// </summary>
        /// <value>The default command index. The default value is -1, which indicates there is no default command.</value>
        public int DefaultCommandIndex
        {
            get
            {
                return this.defaultCommandIndex;
            }
            set
            {
                this.SetProperty( ref this.defaultCommandIndex, value );
            }
        }

        /// <summary>
        /// Gets or sets the cancel command index.
        /// </summary>
        /// <value>The cancel command index. The default value is -1, which indicates there is no default command.</value>
        public int CancelCommandIndex
        {
            get
            {
                return this.cancelCommandIndex;
            }
            set
            {
                this.SetProperty( ref this.cancelCommandIndex, value );
            }
        }

        /// <summary>
        /// Gets the default command based on available commands and specified default command index.
        /// </summary>
        /// <value>A <see cref="INamedCommand">command</see> based on the <see cref="P:DefaultCommandIndex">default command index</see>
        /// in the available <see cref="P:Commands">commands</see>.</value>
        public virtual INamedCommand DefaultCommand
        {
            get
            {
                return this.Commands.ElementAtOrDefault( this.DefaultCommandIndex );
            }
        }

        /// <summary>
        /// Gets the cancel command based on available commands and specified cancel command index.
        /// </summary>
        /// <value>A <see cref="INamedCommand">command</see> based on the <see cref="P:CancelCommandIndex">cancel command index</see>
        /// in the available <see cref="P:Commands">commands</see>. If the <see cref="P:CancelCommandIndex">cancel command index</see>
        /// is less than zero, the cancel command is assumed to be the last command; otherwise, null is returned.</value>
        public virtual INamedCommand CancelCommand
        {
            get
            {
                return this.Commands.ElementAtOrDefault( this.CancelCommandIndex );
            }
        }

        /// <summary>
        /// Gets a collection of continuation data.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}">collection</see> containing continuation data.</value>
        /// <remarks>This property is typically only used on platforms that support interactions with
        /// continuations such as Windows Phone.</remarks>
        public virtual IDictionary<string, object> ContinuationData
        {
            get
            {
                Contract.Ensures( this.continuationData != null );
                return this.continuationData.Value;
            }
        }
    }
}
