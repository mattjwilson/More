﻿namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents the metadata for an application setting.
    /// </summary>
    [CLSCompliant( false )]
    [MetadataAttribute]
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Parameter )]
    public sealed class SettingAttribute : ImportAttribute
    {
        private readonly static Lazy<IDictionary<Type, ITypeConverter>> mappedConverters = new Lazy<IDictionary<Type, ITypeConverter>>( CreateDefaultConverters );
        private readonly static object syncRoot = new object();
        private object defaultValue = NullValue;

        /// <summary>
        /// Gets a value that can be used to represent null for the <see cref="P:DefaultValue">default value</see>.
        /// </summary>
        /// <value>An <see cref="Object">object</see> that represents <c>null</c>.</value>
        /// <remarks>This field is used to ensure the <see cref="P:DefaultValue"/> property is not actually null,
        /// which can cause an exception in the Managed Extensibility Framework under certain conditions.</remarks>
        public static readonly object NullValue = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingAttribute" /> class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        public SettingAttribute( string key )
            : base( key )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );
        }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <value>The setting key.</value>
        public string Key
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return this.ContractName;
            }
        }

        /// <summary>
        /// Gets or sets the default value of the setting.
        /// </summary>
        /// <value>The setting default value.</value>
        public object DefaultValue
        {
            get
            {
                Contract.Ensures( this.defaultValue != null );
                return this.defaultValue;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.defaultValue = value;
            }
        }

        private static IDictionary<Type, ITypeConverter> CreateDefaultConverters()
        {
            return new Dictionary<Type, ITypeConverter>()
            {
                { typeof( object ), new DefaultConverter() },
                { typeof( Guid ), new GuidConverter() },
                { typeof( Uri ), new UriConverter() },
                { typeof( TimeSpan ), new TimeSpanConverter() },
                { typeof( Enum ), new EnumConverter() }
            };
        }

        /// <summary>
        /// Adds or replaces a conversion function for the specified value.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value to convert to.</typeparam>
        /// <param name="converter">The converter <see cref="Func{T1,T2,T3,TResult}">function</see>.</param>
        /// <remarks>Default converters are provided for the following types: <see cref="Guid"/>, <see cref="Uri"/>,
        /// <see cref="TimeSpan"/>, and <see cref="Enum"/>. A default converter is also provided to convert all
        /// primitive types. If a <paramref name="converter"/> is provided that maps to <see cref="Enum"/>, it will
        /// become the default converter for all enumerations. If a <paramref name="converter"/> is provided that
        /// maps to <see cref="Object"/>, it will become the fallback converter for all conversions.</remarks>
        public static void SetConverter<TValue>( Func<object, Type, IFormatProvider, TValue> converter )
        {
            Contract.Requires<ArgumentNullException>( converter != null, "converter" );

            lock ( syncRoot )
                mappedConverters.Value[typeof( TValue )] = new UserDefinedConverter<TValue>( converter );
        }

        /// <summary>
        /// Converts the specified value to the requested target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target <see cref="Type">type</see> to convert the value to.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider">format provider</see> used to convert the value.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>Default converters are provided for the following types: <see cref="Guid"/>, <see cref="Uri"/>,
        /// <see cref="TimeSpan"/>, and <see cref="Enum"/>. A default converter is also provided to convert all
        /// primitive types. To register additional conversion methods, use the <see cref="SetConverter{T}"/> method.</remarks>
        public static object Convert( object value, Type targetType, IFormatProvider formatProvider )
        {
            Contract.Requires<ArgumentNullException>( targetType != null, "targetType" );

            IDictionary<Type, ITypeConverter> converters;
            ITypeConverter converter;

            lock ( syncRoot )
            {
                converters = mappedConverters.Value;

                // try to resolve a defined converter
                if ( converters.TryGetValue( targetType, out converter ) )
                    return converter.Convert( value, targetType, formatProvider );
            }

            // special handling for enumerations
            if ( targetType.GetTypeInfo().IsEnum )
            {
                // there is a single type converter that handles all enumerations
                lock ( syncRoot )
                    converter = converters[typeof( Enum )];

                return converter.Convert( value, targetType, formatProvider );
            }

            // default, catch-all converter
            lock ( syncRoot )
                converter = converters[typeof( object )];

            return converter.Convert( value, targetType, formatProvider );
        }
    }
}
