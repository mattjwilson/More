﻿namespace System.ComponentModel
{
    using global::System;
    using global::System.ComponentModel.Design;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceContainer"/> interface.
    /// </summary>
    public static class IServiceContainerExtensions
    {
        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to add.</typeparam>
        /// <typeparam name="TInstance">The <see cref="Type">type</see> of service created.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="callback">A <see cref="ServiceCreatorCallback">callback</see> object that is used to create the service.
        /// This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void AddService<TService, TInstance>( this IServiceContainer serviceContainer, ServiceCreatorCallback callback ) where TInstance : class, TService
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( callback != null, "callback" );
            serviceContainer.AddService( typeof( TService ), callback, false );
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to add.</typeparam>
        /// <typeparam name="TInstance">The <see cref="Type">type</see> of service created.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="callback">A <see cref="ServiceCreatorCallback">callback</see> object that is used to create the service.
        /// This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void AddService<TService, TInstance>( this IServiceContainer serviceContainer, ServiceCreatorCallback callback, bool promote ) where TInstance : class, TService
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( callback != null, "callback" );
            serviceContainer.AddService( typeof( TService ), callback, promote );
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to add.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceInstance">An instance of the <typeparamref name="TService">service</typeparamref> to add.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void AddService<TService>( this IServiceContainer serviceContainer, TService serviceInstance ) where TService : class
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );
            serviceContainer.AddService( typeof( TService ), serviceInstance, false );
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to add.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceInstance">An instance of the <typeparamref name="TService">service</typeparamref> to add.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void AddService<TService>( this IServiceContainer serviceContainer, TService serviceInstance, bool promote ) where TService : class
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );
            serviceContainer.AddService( typeof( TService ), serviceInstance, promote );
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to add.</typeparam>
        /// <typeparam name="TInstance">The instance <see cref="Type">type</see> of service added.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceInstance">An instance of the <typeparamref name="TInstance">service type</typeparamref> to add.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void AddService<TService, TInstance>( this IServiceContainer serviceContainer, TInstance serviceInstance ) where TInstance : class, TService
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );
            serviceContainer.AddService( typeof( TService ), serviceInstance, false );
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to add.</typeparam>
        /// <typeparam name="TInstance">The instance <see cref="Type">type</see> of service added.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceInstance">An instance of the <typeparamref name="TInstance">service type</typeparamref> to add.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void AddService<TService, TInstance>( this IServiceContainer serviceContainer, TInstance serviceInstance, bool promote ) where TInstance : class, TService
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );
            serviceContainer.AddService( typeof( TService ), serviceInstance, promote );
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to add.</param>
        /// <param name="callback">A <see cref="ServiceCreatorCallback">callback</see> object that is used to create the service.
        /// This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void AddService( this IServiceContainer serviceContainer, Type serviceType, ServiceCreatorCallback callback )
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceType != null, "serviceType" );
            Contract.Requires<ArgumentNullException>( callback != null, "callback" );
            serviceContainer.AddService( serviceType, callback, false );
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to add.</param>
        /// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type
        /// indicated by the <paramref name="serviceType">service type</paramref>.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static void AddService( this IServiceContainer serviceContainer, Type serviceType, object serviceInstance )
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceType != null, "serviceType" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );
            serviceContainer.AddService( serviceType, serviceInstance, false );
        }

        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to remove.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void RemoveService<TService>( this IServiceContainer serviceContainer )
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            serviceContainer.RemoveService( typeof( TService ), false );
        }

        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to remove.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void RemoveService<TService>( this IServiceContainer serviceContainer, bool promote )
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            serviceContainer.RemoveService( typeof( TService ), promote );
        }

        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to remove.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void RemoveService( this IServiceContainer serviceContainer, Type serviceType )
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceType != null, "serviceType" );
            serviceContainer.RemoveService( serviceType, false );
        }

        /// <summary>
        /// Replaces the specified service in the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to replace.</typeparam>
        /// <typeparam name="TInstance">The <see cref="Type">type</see> of service created.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="callback">A <see cref="ServiceCreatorCallback">callback</see> object that is used to create the service.
        /// This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void ReplaceService<TService, TInstance>( this IServiceContainer serviceContainer, ServiceCreatorCallback callback ) where TInstance : class, TService
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( callback != null, "callback" );

            serviceContainer.RemoveService( typeof( TService ), false );
            serviceContainer.AddService( typeof( TService ), callback, false );
        }

        /// <summary>
        /// Replaces the specified service in the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to replace.</typeparam>
        /// <typeparam name="TInstance">The <see cref="Type">type</see> of service created.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="callback">A <see cref="ServiceCreatorCallback">callback</see> object that is used to create the service.
        /// This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void ReplaceService<TService, TInstance>( this IServiceContainer serviceContainer, ServiceCreatorCallback callback, bool promote ) where TInstance : class, TService
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( callback != null, "callback" );

            serviceContainer.RemoveService( typeof( TService ), promote );
            serviceContainer.AddService( typeof( TService ), callback, promote );
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to replace.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceInstance">An instance of the <typeparamref name="TService">service</typeparamref> to replace.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void ReplaceService<TService>( this IServiceContainer serviceContainer, TService serviceInstance ) where TService : class
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );

            serviceContainer.RemoveService( typeof( TService ), false );
            serviceContainer.AddService( typeof( TService ), serviceInstance, false );
        }

        /// <summary>
        /// Replaces the specified service in the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to replace.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceInstance">An instance of the <typeparamref name="TService">service</typeparamref> to replace.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void ReplaceService<TService>( this IServiceContainer serviceContainer, TService serviceInstance, bool promote ) where TService : class
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );

            serviceContainer.RemoveService( typeof( TService ), promote );
            serviceContainer.AddService( typeof( TService ), serviceInstance, promote );
        }

        /// <summary>
        /// Replaces the specified service in the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to replace.</typeparam>
        /// <typeparam name="TInstance">The instance <see cref="Type">type</see> of service replaceed.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceInstance">An instance of the <typeparamref name="TInstance">service type</typeparamref> to replace.</param>
        /// <remarks>This method does not promote requests to parent containers.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void ReplaceService<TService, TInstance>( this IServiceContainer serviceContainer, TInstance serviceInstance ) where TInstance : class, TService
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );

            serviceContainer.RemoveService( typeof( TService ), false );
            serviceContainer.AddService( typeof( TService ), serviceInstance, false );
        }

        /// <summary>
        /// Replaces the specified service in the service container.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service to replace.</typeparam>
        /// <typeparam name="TInstance">The instance <see cref="Type">type</see> of service replaceed.</typeparam>
        /// <param name="serviceContainer">The extended <see cref="IServiceContainer">service container</see>.</param>
        /// <param name="serviceInstance">An instance of the <typeparamref name="TInstance">service type</typeparamref> to replace.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reduces common programming mistakes." )]
        public static void ReplaceService<TService, TInstance>( this IServiceContainer serviceContainer, TInstance serviceInstance, bool promote ) where TInstance : class, TService
        {
            Contract.Requires<ArgumentNullException>( serviceContainer != null, "serviceContainer" );
            Contract.Requires<ArgumentNullException>( serviceInstance != null, "serviceInstance" );

            serviceContainer.RemoveService( typeof( TService ), promote );
            serviceContainer.AddService( typeof( TService ), serviceInstance, promote );
        }
    }
}
