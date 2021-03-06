﻿namespace More.Windows.Interactivity
{
    using More.Windows.Controls;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Represents an <see cref="T:System.Windows.Interactivity.TriggerAction{T}">interactivity action</see> that can be used to show the
    /// <see cref="T:Interaction">interaction</see> from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class MessageDialogAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Prompts a user with an alert for the specified <see cref="T:Interaction">interaction</see> using a <see cref="MessageBox">message box</see>.
        /// </summary>
        /// <param name="interaction">The <see cref="T:Interaction">interaction</see> to display.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Alert( Interaction interaction )
        {
            Contract.Requires<ArgumentNullException>( interaction != null, "interaction" );

            var dialog = new MessageDialog();

            dialog.Title = interaction.Title;
            dialog.Content = interaction.Content;
            dialog.DefaultCommandIndex = 0;
            dialog.Commands.Add( new NamedCommand<object>( SR.OKCaption, DefaultAction.None ) );
            dialog.Owner = Window.GetWindow( this.AssociatedObject );
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Prompts a user with the specified <see cref="T:Interaction">interaction</see> using a <see cref="MessageBox">message box</see>.
        /// </summary>
        /// <param name="interaction">The <see cref="T:Interaction">interaction</see> to display.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Prompt( Interaction interaction )
        {
            Contract.Requires<ArgumentNullException>( interaction != null, "interaction" );

            var dialog = new MessageDialog();
            var commands = interaction.Commands.DelayAll().ToArray();

            // add behavior which can support binding the cancel command to the window close button
            var behavior = new WindowCloseBehavior();
            behavior.CloseCommand = interaction.CancelCommand;
            System.Windows.Interactivity.Interaction.GetBehaviors( dialog ).Add( behavior );

            dialog.Owner = Window.GetWindow( this.AssociatedObject );
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Title = interaction.Title;
            dialog.Content = interaction.Content;
            dialog.DefaultCommandIndex = interaction.DefaultCommandIndex;
            dialog.CancelCommandIndex = interaction.CancelCommandIndex;
            dialog.Commands.AddRange( commands );
            dialog.ShowDialog();

            commands.InvokeExecuted();
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="args">The <see cref="InteractionRequestedEventArgs"/> event data provided by the corresponding trigger.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Invoke( InteractionRequestedEventArgs args )
        {
            Contract.Requires<ArgumentNullException>( args != null, "args" );

            var interaction = args.Interaction;

            if ( interaction.Commands.Any() )
                this.Prompt( interaction );
            else
                this.Alert( interaction );
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="parameter">The parameter supplied from the corresponding trigger.</param>
        /// <remarks>This method is not meant to be called directly by your code.</remarks>
        protected sealed override void Invoke( object parameter )
        {
            var args = parameter as InteractionRequestedEventArgs;

            if ( args != null )
                this.Invoke( args );
        }
    }
}
