﻿namespace More.Windows.Input
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a cancelable command<seealso cref="AsyncCancelableCommand"/>.
    /// </summary>
    /// <remarks>This <see cref="INamedCommand">command</see> is a specialized variant of <see cref="NamedCommand{T}"/>
    /// which supports cancelling the execution of the command. This command is useful for scenarios such as cancelling
    /// the close of a window.</remarks>
    public class CancelableCommand : NamedCommand<CancelEventArgs>
    {
        private readonly Action<CancelEventArgs> executeMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableCommand"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        public CancelableCommand( string name, Action<CancelEventArgs> executeMethod )
            : this( null, name, executeMethod, DefaultFunc.CanExecute )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableCommand"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        public CancelableCommand( string id, string name, Action<CancelEventArgs> executeMethod )
            : this( id, name, executeMethod, DefaultFunc.CanExecute )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableCommand"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2}"/> representing the can execute method.</param>
        public CancelableCommand( string name, Action<CancelEventArgs> executeMethod, Func<CancelEventArgs, bool> canExecuteMethod )
            : this( null, name, executeMethod, canExecuteMethod )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
            Contract.Requires<ArgumentNullException>( canExecuteMethod != null, "canExecuteMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableCommand"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2}"/> representing the can execute method.</param>
        public CancelableCommand( string id, string name, Action<CancelEventArgs> executeMethod, Func<CancelEventArgs, bool> canExecuteMethod )
            : base( id, name, executeMethod, canExecuteMethod )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
            Contract.Requires<ArgumentNullException>( canExecuteMethod != null, "canExecuteMethod" );
            this.executeMethod = executeMethod;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The associated parameter with the command.</param>
        public override void Execute( CancelEventArgs parameter )
        {
            parameter = parameter ?? new CancelEventArgs();

            this.executeMethod( parameter );

            if ( !parameter.Cancel )
                this.OnExecuted( parameter );
        }
    }
}
