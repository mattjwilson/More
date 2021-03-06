﻿namespace More.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a configuration file based service Uniform Resource Locator (URL) locator.
    /// </summary>
    public class ConfigurationFileUrlLocator : IConfigurationSettingLocator<Uri>
    {
        private readonly ConcurrentDictionary<string, Uri> urls = new ConcurrentDictionary<string, Uri>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileUrlLocator"/> class.
        /// </summary>
        public ConfigurationFileUrlLocator()
            : this( null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileUrlLocator"/> class.
        /// </summary>
        /// <param name="nextLocator">The next <see cref="IConfigurationSettingLocator{T}">locator</see> in the chain.</param>
        public ConfigurationFileUrlLocator( IConfigurationSettingLocator<Uri> nextLocator )
        {
            this.DefaultEnvironment = DeploymentEnvironment.Unspecified;
            this.NextLocator = nextLocator;
        }

        private Uri LocateUrl( string key )
        {
            Contract.Requires( !string.IsNullOrEmpty( key ) );
            Uri url = null;

            // if parsed url exists, return it
            if ( this.urls.TryGetValue( key, out url ) )
                return url;

            // get url from configuration file
            var value = ConfigurationManager.AppSettings[key];

            if ( string.IsNullOrEmpty( value ) )
                return null;

            // parse and cache url as necessary
            if ( Uri.TryCreate( value, UriKind.RelativeOrAbsolute, out url ) )
                this.urls[key] = url;

            return url;
        }

        /// <summary>
        /// Gets or sets the default deployment environment used by the locator.
        /// </summary>
        /// <value>One of the <see cref="DeploymentEnvironment"/> values.</value>
        public DeploymentEnvironment DefaultEnvironment
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the next locator for the current instance.
        /// </summary>
        /// <value>An <see cref="IConfigurationSettingLocator{T}"/> object.</value>
        public IConfigurationSettingLocator<Uri> NextLocator
        {
            get;
            protected set;
        }

        /// <summary>
        /// Clears any URLs cached by the locator.
        /// </summary>
        public virtual void ClearCache()
        {
            this.urls.Clear();

            if ( this.NextLocator != null )
                this.NextLocator.ClearCache();
        }

        /// <summary>
        /// Locates a URL with the specified key.
        /// </summary>
        /// <param name="key">The key for the URL to locate.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="Uri">URL</see> or null if no match is found.</returns>
        public Task<Uri> LocateAsync( string key )
        {
            return this.LocateAsync( key, DeploymentEnvironment.Unspecified );
        }

        /// <summary>
        /// Locates a URL with the specified key and environment.
        /// </summary>
        /// <param name="key">The key for the URL to locate.</param>
        /// <param name="environment">One of the <see cref="DeploymentEnvironment"/> values.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="Uri">URL</see> or null if no match is found.</returns>
        public virtual Task<Uri> LocateAsync( string key, DeploymentEnvironment environment )
        {
            var url = this.LocateUrl( key );

            if ( url != null || this.NextLocator == null )
            {
                var source = new TaskCompletionSource<Uri>();
                source.SetResult( url );
                return source.Task;
            }

            return this.NextLocator.LocateAsync( key, environment );
        }
    }
}
