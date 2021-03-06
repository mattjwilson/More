﻿namespace More.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Security.Permissions;

    /// <summary>
    /// Represents a <see cref="ConfigurationElement"/> that has a name and <see cref="Type">type</see>.
    /// </summary>
    [ConfigurationPermission( SecurityAction.InheritanceDemand, Unrestricted = true )]
    public abstract class NamedTypeConfigurationElement : NamedConfigurationElement
    {
        private const string TypeProperty = "type";

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedTypeConfigurationElement"/> class.
        /// </summary>
        protected NamedTypeConfigurationElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedTypeConfigurationElement"/> class with the supplied <paramref name="name"/> and <paramref name="baseType">base type</paramref>.
        /// </summary>
        /// <param name="name">The element <see cref="P:NamedConfigurationElement.Name">name</see>.</param>
        /// <param name="baseType">The element base <see cref="Type">type</see>.</param>
        protected NamedTypeConfigurationElement( string name, Type baseType )
            : base( name )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrWhiteSpace( name ), "name" );
            this.BaseType = baseType;
        }

        /// <summary>
        /// Gets or sets the configured base <see cref="Type">type</see> element.
        /// </summary>
        /// <value>The configured base <see cref="Type">type</see> element.</value>
        protected Type BaseType
        {
            get
            {
                return (Type) base[TypeProperty];
            }
            set
            {
                base[TypeProperty] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Type">type</see> the element is the configuration for.
        /// </summary>
        /// <value>The <see cref="Type">type</see> the element is the configuration for.</value>
        [ConfigurationProperty( TypeProperty, IsRequired = true )]
        [TypeConverter( typeof( TypeNameConverter ) )]
        [SuppressMessage( "Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "This is the most logical configuration element attribute name." )]
        public virtual Type Type
        {
            get
            {
                return this.BaseType;
            }
            set
            {
                this.Validate( value );
                this.BaseType = value;
            }
        }

        /// <summary>
        /// Override this method to perform your concrete validations.
        /// </summary>
        /// <param name="value">The <see cref="Object">object</see> to validate.</param>
        protected virtual void Validate( object value )
        {
        }
    }
}
