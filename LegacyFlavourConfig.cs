using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LegacyFlavour
{
    public class LegacyFlavourConfig
    {
        public ColourBlindness Mode
        {
            get;
            set;
        } = ColourBlindness.None;

        public List<ZoneColourConfig> Zones
        { 
            get; 
            set; 
        }

        public static void Save( LegacyFlavourConfig config )
        {
            var json = JsonConvert.SerializeObject( config );
            var filePath = Path.Combine( GetAssemblyDirectory( ), "config.json" );
            File.WriteAllText( filePath, json );
        }

        public static LegacyFlavourConfig Load( )
        {
            var filePath = Path.Combine( GetAssemblyDirectory( ), "config.json" );
            var json = "";

            if ( File.Exists( filePath ) )
            {
                json = File.ReadAllText( filePath );
            }
            else
            {
                json = LoadDefaultAndSave( );
            }
            return JsonConvert.DeserializeObject<LegacyFlavourConfig>( json );
        }

        private static string LoadDefaultAndSave( )
        {
            var assembly = Assembly.GetExecutingAssembly( );

            using ( var stream = assembly.GetManifestResourceStream( "LegacyFlavour.Resources.defaultConfig.json" ) )
            {
                if ( stream == null )
                {
                    Debug.LogError( "Embedded default config not found." );
                    return null;
                }

                using ( var reader = new StreamReader( stream ) )
                {
                    var json = reader.ReadToEnd( );
                    var defaultConfig = JsonConvert.DeserializeObject<LegacyFlavourConfig>( json );
                    var filePath = Path.Combine( GetAssemblyDirectory( ), "config.json" );
                    File.WriteAllText( filePath, json );
                    return json;
                }
            }
        }

        private static string GetAssemblyDirectory( )
        {
            return Path.GetDirectoryName( typeof( LegacyFlavourConfig ).Assembly.Location );
        }
    }

    public class ZoneColourConfig
    {
        public string Name 
        {
            get; 
            set;
        }

        public string Colour
        {
            get;
            set;
        }

        public string Deuteranopia
        {
            get;
            set;
        }

        public string Protanopia
        {
            get;
            set;
        }

        public string Tritanopia
        {
            get;
            set;
        }

        public string Custom
        {
            get;
            set;
        }
    }
}
