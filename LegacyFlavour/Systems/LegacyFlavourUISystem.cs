using Colossal.UI.Binding;
using Game.UI;
using LegacyFlavour.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LegacyFlavour.Systems
{
    public class LegacyFlavourUISystem : UISystemBase
    {
        private LegacyFlavourConfig _config;

        private string kGroup = "cities2modding_legacyflavour";
        static GetterValueBinding<LegacyFlavourConfig> _binding;
        static FieldInfo _dirtyField = typeof( GetterValueBinding<LegacyFlavourConfig> ).GetField( "m_ValueDirty", BindingFlags.Instance | BindingFlags.NonPublic );

        static readonly string[] TRIGGER_UPDATE_PROPERTIES = new[] 
        {
            "UseStickyWhiteness",
            "WhitenessToggle",
            "TimeOfDay",
            "Weather",
            "FreezeVisualTime"
        };

        static readonly string[] TRIGGER_COLOUR_UPDATE_PROPERTIES = new[] 
        {
            "Enabled",
            "CellOpacity",
            "CellBorderOpacity",
            "EmptyCellOpacity",
            "EmptyCellBorderOpacity",
            "Mode",
            "UseDynamicCellBorders"
        };

        private LegacyFlavourSystem _legacyFlavourSystem;
        private ZoneColourSystem _zoneColourSystem;

        /// <summary>
        /// Create our bindings
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            _legacyFlavourSystem = World.GetOrCreateSystemManaged<LegacyFlavourSystem>( );
            _zoneColourSystem = World.GetOrCreateSystemManaged<ZoneColourSystem>( );

            _config = LegacyFlavourSystem.Config;

            LegacyFlavourConfig.OnUpdated += ( ) =>
            {
                _dirtyField.SetValue( _binding, true );
            };

            _binding = new GetterValueBinding<LegacyFlavourConfig>( kGroup, "config", ( ) =>
            {
                return _config;
            }, new ValueWriter<LegacyFlavourConfig>( ).Nullable( ) );

            AddUpdateBinding( _binding );
            AddBinding( new TriggerBinding<string>( kGroup, "updateProperty", UpdateProperty ) );            
            AddBinding( new TriggerBinding( kGroup, "regenerateIcons", ( ) =>
            {
                _zoneColourSystem.ForceUpdate( true );
            } ) );
            AddBinding( new TriggerBinding( kGroup, "setColoursToVanilla", _zoneColourSystem.SetCurrentToVanilla ) );
            AddBinding( new TriggerBinding( kGroup, "resetZoneSettingsToDefault", _zoneColourSystem.ResetSettingsToDefault ) );
            AddBinding( new TriggerBinding( kGroup, "resetColoursToDefault", _zoneColourSystem.ResetColoursToDefault ) );
            AddBinding( new TriggerBinding<string>( kGroup, "launchUrl", OpenURL ) );
            AddBinding( new TriggerBinding<string, string>( kGroup, "updateZoneColour", _zoneColourSystem.UpdateZoneColour ) );
        }

        /// <summary>
        /// Open a URL in the web browser
        /// </summary>
        /// <param name="url"></param>
        private void OpenURL( string url )
        {
            if ( string.IsNullOrEmpty( url ) )
                return;

            try
            {
                // Launch the URL in the default browser
                Process.Start( url );
            }
            catch ( Exception ex )
            {
                // Handle exceptions, if any
                Console.WriteLine( "An error occurred: " + ex.Message );
            }
        }

        /// <summary>
        /// Triggered from UI to C# when a property is updated
        /// </summary>
        /// <param name="json"></param>
        private void UpdateProperty( string json )
        {
            if ( string.IsNullOrEmpty( json ) )
                return;

            var properties = ModelWriter._propertiesCache[typeof( LegacyFlavourConfig )];

            if ( properties == null )
                return;

            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>( json );

            if ( dic == null )
                return;

            var property = properties.FirstOrDefault( p => p.Name == ( string ) dic["property"] );
            var val = dic["value"];

            if ( property == null ) 
                return;

            if ( val.GetType( ) != property.PropertyType )
            {
                if ( property.PropertyType == typeof( decimal ) )
                {
                    if ( val is string stringValue && int.TryParse( stringValue, out var intValue ) )
                    {
                        property.SetValue( _config, intValue / 100m ); // Convert back
                    }
                    else if ( val is long longValue )
                        property.SetValue( _config, longValue / 100m ); // Convert back
                    else if ( val is int intValue2 )
                        property.SetValue( _config, intValue2 / 100m ); // Convert back
                }
                else
                {
                    if ( !property.PropertyType.IsEnum || val.GetType( ) != typeof( string ) )
                        return;

                    if ( Enum.TryParse( property.PropertyType, ( string ) val, out var enumValue ) )
                        property.SetValue( _config, enumValue );
                }
            }
            else
                property.SetValue( _config, val );

            LegacyFlavourConfig.Save( _config );

            if ( TRIGGER_UPDATE_PROPERTIES.Contains( property.Name ) )
                _legacyFlavourSystem.TriggerUpdate( property.Name );
            else if ( TRIGGER_COLOUR_UPDATE_PROPERTIES.Contains( property.Name ) )
                _zoneColourSystem.TriggerUpdate( property.Name );
        }
    }
}
