﻿namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Globalization;

    /// <summary>
    /// Specifies the minimum and maximum length of characters that are allowed in a data field.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.StringLengthAttribute.</remarks>
    [AttributeUsage( AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false )]
    public class StringLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthAttribute"/> class by using a specified maximum length.
        /// </summary>
        /// <param name="maximumLength">The maximum length of a string.</param>
        public StringLengthAttribute( int maximumLength )
            : base( (Func<string>) ( () => DataAnnotationsResources.StringLengthAttribute_ValidationError ) )
        {
            this.MaximumLength = maximumLength;
        }

        /// <summary>
        /// Gets or sets the maximum length of a string.
        /// </summary>
        /// <value>The maximum length a string.</value>
        public int MaximumLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the minimum length of a string.
        /// </summary>
        /// <value>The minimum length of a string.</value>
        public int MinimumLength
        {
            get;
            set;
        }

        private void EnsureLegalLengths()
        {
            if ( this.MaximumLength < 0 )
            {
                throw new InvalidOperationException( DataAnnotationsResources.StringLengthAttribute_InvalidMaxLength );
            }

            if ( this.MaximumLength < this.MinimumLength )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.RangeAttribute_MinGreaterThanMax, new object[] { this.MaximumLength, this.MinimumLength } ) );
            }
        }

        /// <summary>
        /// Applies formatting to a specified error message.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted error message.</returns>
        public override string FormatErrorMessage( string name )
        {
            this.EnsureLegalLengths();
            string format = ( ( this.MinimumLength != 0 ) && !base.CustomErrorMessageSet ) ? DataAnnotationsResources.StringLengthAttribute_ValidationErrorIncludingMinimum : base.ErrorMessageString;
            return string.Format( CultureInfo.CurrentCulture, format, new object[] { name, this.MaximumLength, this.MinimumLength } );
        }

        /// <summary>
        /// Determines whether a specified object is valid.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns>True if the specified object is valid; otherwise, false.</returns>
        public override bool IsValid( object value )
        {
            this.EnsureLegalLengths();
            int num = ( value == null ) ? 0 : ( (string) value ).Length;
            return ( ( value == null ) || ( ( num >= this.MinimumLength ) && ( num <= this.MaximumLength ) ) );
        }
    }
}
