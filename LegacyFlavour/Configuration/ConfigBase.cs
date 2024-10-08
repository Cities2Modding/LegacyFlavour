﻿using LegacyFlavour.UI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LegacyFlavour.Configuration
{
    /// <summary>
    /// Abstract base class for configuration handling.
    /// </summary>
    public abstract class ConfigBase : ModelWriter
    {
        const string DATA_FOLDER = "Cities2Modding";

        public static readonly string MOD_PATH = Path.Combine( Application.persistentDataPath, DATA_FOLDER, "LegacyFlavour" );

        static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented  
        };

        /// <summary>
        /// Action triggered on configuration update.
        /// </summary>
        public static Action OnUpdated;

        /// <summary>
        /// Abstract property to get the configuration file name.
        /// Must be implemented by derived classes.
        /// </summary>
        protected abstract string ConfigFileName { get; }

        /// <summary>
        /// Saves the current configuration to a file.
        /// </summary>
        public virtual void Save( )
        {
            var json = JsonConvert.SerializeObject( this, _serializerSettings );
            var filePath = Path.Combine( MOD_PATH, ConfigFileName );
            File.WriteAllText( filePath, json );
            OnUpdated?.Invoke( );
        }

        /// <summary>
        /// Loads the configuration from a file.
        /// </summary>
        /// <param name="useDefaultAsTemplate"></param>
        /// <returns>The loaded configuration object.</returns>
        public static T Load<T>( bool useDefaultAsTemplate = true ) where T : ConfigBase, new()
        {
            var instance = new T( );
            var filePath = Path.Combine( MOD_PATH, instance.ConfigFileName );
            var json = "";

            if ( File.Exists( filePath ) )
            {
                json = File.ReadAllText( filePath );
            }
            else if ( useDefaultAsTemplate  )
            {
                json =  LoadDefaultAndSave<T>( );
            }
            else
                return Activator.CreateInstance<T>( );

            return ( T ) JsonConvert.DeserializeObject( json, typeof( T ), _serializerSettings );
        }

        /// <summary>
        /// Loads the default configuration and saves it to a file.
        /// </summary>
        /// <returns>The default configuration.</returns>
        public static T LoadDefault<T>( ) where T : ConfigBase, new()
        {
            var instance = new T( );
            var json = instance.LoadDefaultJson( );

            return JsonConvert.DeserializeObject<T>( json, _serializerSettings );
        }

        /// <summary>
        /// Loads the default configuration and saves it to a file.
        /// </summary>
        /// <returns>The default configuration JSON string.</returns>
        private static string LoadDefaultAndSave<T>( ) where T : ConfigBase, new()
        {
            var instance = new T( );
            var json = instance.LoadDefaultJson( );
            var filePath = Path.Combine( MOD_PATH, instance.ConfigFileName );
            if (!Directory.Exists( MOD_PATH ))
                Directory.CreateDirectory( MOD_PATH );
            File.WriteAllText( filePath, json );
            return json;
        }

        /// <summary>
        /// Loads the default JSON configuration.
        /// Can be overridden in derived classes to provide custom default configurations.
        /// </summary>
        /// <returns>The default configuration JSON string.</returns>
        protected virtual string LoadDefaultJson( )
        {
            var resourceName = "LegacyFlavour.Resources.default" + UppercaseFirst( ConfigFileName );
            var assembly = Assembly.GetExecutingAssembly( );
            using ( var stream = assembly.GetManifestResourceStream( resourceName ) )
            {
                if ( stream == null )
                {
                    Debug.LogError( $"Embedded default config not found: {resourceName}" );
                    return null;
                }
                using ( var reader = new StreamReader( stream ) )
                {
                    return reader.ReadToEnd( );
                }
            }
        }

        /// <summary>
        /// Helper method to uppercase the first character of a string.
        /// </summary>
        /// <param name="s">The string to modify.</param>
        /// <returns>The modified string with the first character in uppercase.</returns>
        private static string UppercaseFirst( string s )
        {
            if ( string.IsNullOrEmpty( s ) )
            {
                return string.Empty;
            }
            return char.ToUpper( s[0] ) + s.Substring( 1 );
        }
    }
}
