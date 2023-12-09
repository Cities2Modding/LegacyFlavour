using LegacyFlavour.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LegacyFlavour.Configuration
{
    public class LegacyFlavourConfig : ModelWriter
    {
        public bool Enabled
        {
            get;
            set;
        }

        public bool UseStickyWhiteness
        {
            get;
            set;
        } = false;

        public bool WhitenessToggle
        {
            get;
            set;
        }

        public bool UseUnits
        {
            get;
            set;
        } = true;

        public bool UseDynamicCellBorders
        {
            get;
            set;
        } = true;

        public decimal CellOpacity
        {
            get;
            set;
        } = 0.9m;

        public decimal CellBorderOpacity
        {
            get;
            set;
        } = 0.9m;

        public decimal EmptyCellOpacity
        {
            get;
            set;
        } = 0m;

        public decimal EmptyCellBorderOpacity
        {
            get;
            set;
        } = 0.2m;

        public ColourBlindness Mode
        {
            get;
            set;
        } = ColourBlindness.None;

        public TimeOfDayOverride TimeOfDay
        {
            get;
            set;
        } = TimeOfDayOverride.Off;

        public bool FreezeVisualTime
        {
            get;
            set;
        }

        public WeatherOverride Weather
        {
            get;
            set;
        } = WeatherOverride.Off;

        public List<ZoneColourConfig> Zones
        {
            get;
            set;
        }

        public static Action OnUpdated;
        public readonly static LegacyFlavourConfig Default = LoadDefault( );

        public static void Save( LegacyFlavourConfig config )
        {
            var json = JsonConvert.SerializeObject( config );
            var filePath = Path.Combine( GetAssemblyDirectory( ), "config.json" );
            File.WriteAllText( filePath, json );
            OnUpdated?.Invoke( );
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

        public static LegacyFlavourConfig LoadDefault( )
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

                    return JsonConvert.DeserializeObject<LegacyFlavourConfig>( json );
                }
            }
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
