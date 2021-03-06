﻿namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents the base implementation to show a shell view.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="IShellView">shell view</see> to display.</typeparam>
    /// <example>The following demonstrates how to implement the <see cref="ShowShellView{T}"/>.
    /// <code lang="C#">
    /// <![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Composition;
    /// using System.Composition.Hosting;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// 
    /// public class MyShell : UserControl, IShellView
    /// {
    ///     public void Show()
    ///     {
    ///         Application.Current.RootVisual = this;
    ///     }
    /// }
    /// 
    /// public class ShowShellView : ShowShellView<MyShell>
    /// {
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public partial class ShowShellView<T> : Activity where T : class, IShellView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowShellView{T}"/> class.
        /// </summary>
        public ShowShellView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowShellView{T}"/> class.
        /// </summary>
        /// <param name="shellViewTypeName">The name of the <see cref="Type">type</see> of the shell view to show. If
        /// this parameter is null or empty, then the <typeparamref name="T">default shell view type</typeparamref> is assumed.</param>
        protected ShowShellView( string shellViewTypeName )
        {
            this.ShellViewTypeName = shellViewTypeName;
        }

        /// <summary>
        /// Gets or sets the name of the type of shell view to show.
        /// </summary>
        /// <value>The name of the <see cref="Type">type</see> of the shell view to show.</value>
        public string ShellViewTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets localization/globalization language information that applies to the shell view.
        /// </summary>
        /// <value>The language information for the shell view.  This property can be null.</value>
        public string Language
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the direction that text and other user interface (UI) elements flow within the shell view.
        /// </summary>
        /// <value>The direction that text and other UI elements flow within the shell view.</value>
        public string FlowDirection
        {
            get;
            set;
        }

        private Type GetShellViewType( IServiceProvider serviceProvider )
        {
            Contract.Requires( serviceProvider != null );
            Contract.Ensures( Contract.Result<Type>() != null );

            var candidateType = this.ShellViewTypeName;
            Type shellViewType = null;

            // determine whether a specific shell is being specified
            if ( !string.IsNullOrEmpty( candidateType ) )
            {
                // initialization parameter indicates the requested shell
                shellViewType = GetTypeFromTypeName( serviceProvider, candidateType );

                if ( shellViewType == null )
                {
                    // no shell matched the requested type
                    throw new HostException( ExceptionMessage.NoCandidateShellView.FormatDefault( candidateType ) );
                }
                else if ( !typeof( IShellView ).GetTypeInfo().IsAssignableFrom( shellViewType.GetTypeInfo() ) )
                {
                    // the shell does not implement the required type
                    throw new HostException( ExceptionMessage.InvalidShellView.FormatDefault( shellViewType, typeof( IShellView ) ) );
                }
            }
            else
            {
                // use the default shell
                shellViewType = typeof( T );
            }

            return shellViewType;
        }

        private void Show( IServiceProvider serviceProvider, Type shellViewType )
        {
            Contract.Requires( serviceProvider != null );
            Contract.Requires( shellViewType != null );

            IShellView shell = null;

            try
            {
                // use the composition service to build up and export the shell view instance
                shell = (IShellView) serviceProvider.GetService( shellViewType );
            }
            catch ( CompositionFailedException ex )
            {
                throw new HostException( ExceptionMessage.ShellViewLoadException, ex );
            }
            catch ( InvalidOperationException ex )
            {
                throw new HostException( ExceptionMessage.ShellViewLoadException, ex );
            }

            if ( shell == null )
                throw new HostException( ExceptionMessage.ShellViewResolutionFailed.FormatDefault( shellViewType ) );

            if ( !string.IsNullOrEmpty( this.Language ) )
                shell.Language = this.Language;

            if ( !string.IsNullOrEmpty( this.FlowDirection ) )
                shell.FlowDirection = this.FlowDirection;

            shell.Show();
        }

        /// <summary>
        /// Gets or sets the name of the activity.
        /// </summary>
        /// <value>The activity name.</value>
        public override string Name
        {
            get
            {
                if ( string.IsNullOrEmpty( base.Name ) )
                    base.Name = "ShowShellView";

                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the activity description.
        /// </summary>
        /// <value>The activity description.</value>
        public override string Description
        {
            get
            {
                if ( string.IsNullOrEmpty( base.Description ) )
                    base.Description = SR.ShowShellViewDesc;

                return base.Description;
            }
            set
            {
                base.Description = value;
            }
        }

        /// <summary>
        /// Occurs when the activity is executed.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> supplied to the activity.</param>
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Delegated to the OnUnhandledException method." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected override void OnExecute( IServiceProvider serviceProvider )
        {
            var shellViewType = this.GetShellViewType( serviceProvider );

            try
            {
                this.Show( serviceProvider, shellViewType );
            }
            catch ( Exception ex )
            {
                this.OnUnhandledException( serviceProvider, ex );
                return;
            }

            this.IsCompleted = true;
        }
    }
}
