using LegacyFlavour.Systems;
using System.Collections.Generic;
using System.IO;
using Unity.Entities;

namespace LegacyFlavour.Configuration.Themes
{
    /// <summary>
    /// Adds options for more UI themes
    /// </summary>
    public class ThemeConfig : ConfigBase
    {
        public List<ThemeEntry> Themes
        {
            get;
            set;
        }

        public readonly static ThemeConfig Default = LoadDefault<ThemeConfig>( );
        protected override string ConfigFileName => "themes.json";

        /// <summary>
        /// Load the config file
        /// </summary>
        /// <returns></returns>
        public static ThemeConfig Load( )
        {
            var config = Load<ThemeConfig>( useDefaultAsTemplate: false );

            // We're creating a new instance!
            if ( !Exists( ) )
            {
                UnityEngine.Debug.Log( "Current themes.json does not exist. Force generating!" );

                var updateSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LegacyFlavourUpdateSystem>( );

                updateSystem?.EnqueueThemeConfigUpdate( true );
            }

            return config;
        }

        /// <summary>
        /// Save the theme config and export the CSS
        /// </summary>
        public override void Save( )
        {
            base.Save( );
        }

        /// <summary>
        /// Check if the config exists
        /// </summary>
        /// <returns></returns>
        public static bool Exists( )
        {
            var filePath = Path.Combine( GetAssemblyDirectory( ), "themes.json" );

            return File.Exists( filePath );
        }
    }
}