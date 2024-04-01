using Colossal.UI.Binding;
using Game.Audio;
using Game.Prefabs;
using Game.SceneFlow;
using Gooee.Plugins;
using LegacyFlavour.Configuration;
using LegacyFlavour.Systems;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using Gooee.Plugins.Attributes;

namespace LegacyFlavour.UI
{
    public partial class LegacyFlavourController : Controller<LegacyFlavourViewModel>
    {
        private string kGroup = "cities2modding_legacyflavour";
        static GetterValueBinding<LegacyFlavourConfig> _binding;
        static GetterValueBinding<LocaleGroup> _currentLocaleBinding;
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
            "UseDynamicCellBorders",
            "OverrideIcons",
        };

        private LegacyFlavourConfig Config
        {
            get
            {
                return _updateSystem.Config;
            }
        }

        private LegacyFlavourUpdateSystem _updateSystem;
        private LegacyFlavourSystem _legacyFlavourSystem;
        private ZoneColourSystem _zoneColourSystem;
        private EntityQuery _soundQuery;

        public override LegacyFlavourViewModel Configure( )
        {
            _legacyFlavourSystem = World.GetOrCreateSystemManaged<LegacyFlavourSystem>( );
            _zoneColourSystem = World.GetOrCreateSystemManaged<ZoneColourSystem>( );

            _soundQuery = GetEntityQuery( ComponentType.ReadOnly<ToolUXSoundSettingsData>( ) );
            _updateSystem = World.GetExistingSystemManaged<LegacyFlavourUpdateSystem>( );

            _binding = new GetterValueBinding<LegacyFlavourConfig>( kGroup, "config", ( ) =>
            {
                return Config;
            }, new ValueWriter<LegacyFlavourConfig>( ).Nullable( ) );

            _currentLocaleBinding = new GetterValueBinding<LocaleGroup>( kGroup, "currentLocale", ( ) =>
            {
                var localeManager = GameManager.instance.localizationManager;

                if ( localeManager == null )
                    return LocaleConfig.Default.Locales[0];

                var localConfig = LocaleConfig.Default.Locales
                    .FirstOrDefault( l => l.IDs.Contains( localeManager.activeLocaleId ) );

                if ( localConfig == null )
                    return LocaleConfig.Default.Locales[0];

                return localConfig;
            } );

            ConfigBase.OnUpdated += ( ) =>
            {
                _dirtyField.SetValue( _binding, true );
            };

            AddUpdateBinding( _binding );
            AddUpdateBinding( _currentLocaleBinding );

            AddBinding( new TriggerBinding<string>( kGroup, "updateProperty", UpdateProperty ) );
            AddBinding( new TriggerBinding( kGroup, "regenerateIcons", ( ) =>
            {
                _updateSystem.EnqueueColoursUpdate( invalidateCache: true );
            } ) );
            AddBinding( new TriggerBinding( kGroup, "setColoursToVanilla", _zoneColourSystem.SetCurrentToVanilla ) );
            AddBinding( new TriggerBinding( kGroup, "triggerSound", TriggerUISound ) );
            AddBinding( new TriggerBinding( kGroup, "resetZoneSettingsToDefault", _zoneColourSystem.ResetSettingsToDefault ) );
            AddBinding( new TriggerBinding<string>( kGroup, "setZoneSettingsPreset", _zoneColourSystem.SetToPreset ) );
            AddBinding( new TriggerBinding( kGroup, "resetColoursToDefault", _zoneColourSystem.ResetColoursToDefault ) );
            AddBinding( new TriggerBinding<string>( kGroup, "launchUrl", OpenURL ) );
            AddBinding( new TriggerBinding<string, string>( kGroup, "updateZoneColour", _zoneColourSystem.UpdateZoneColour ) );

            return new LegacyFlavourViewModel( );
        }

        [OnTrigger]
        private void OnToggleVisible( )
        {
            Model.IsVisible = !Model.IsVisible;

            TriggerUpdate( );
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
        /// Trigger a UI sound
        /// </summary>
        private void TriggerUISound( )
        {
            AudioManager.instance.PlayUISoundIfNotPlaying( _soundQuery.GetSingleton<ToolUXSoundSettingsData>( ).m_SnapSound );
        }

        /// <summary>
        /// Triggered from UI to C# when a property is updated
        /// </summary>
        /// <param name="json"></param>
        private void UpdateProperty( string json )
        {
            if ( string.IsNullOrEmpty( json ) )
                return;

            // Assuming that propertiesCache is a Dictionary and already populated
            var properties = ModelWriter._propertiesCache[typeof( LegacyFlavourConfig )];
            if ( properties == null )
                return;

            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>( json );
            if ( dic == null || !dic.TryGetValue( "property", out var propertyName ) )
                return;

            var property = properties.FirstOrDefault( p => p.Name == ( string ) propertyName );
            if ( property == null )
                return;

            if ( !dic.TryGetValue( "value", out var val ) )
                return;

            // Optimize type checks and conversions
            if ( val.GetType( ) != property.PropertyType )
            {
                if ( TryConvertValue( property.PropertyType, val, out var convertedValue ) )
                {
                    property.SetValue( Config, convertedValue );
                }
            }
            else
            {
                property.SetValue( Config, val );
            }

            _updateSystem.EnqueueConfigUpdate( );

            if ( TRIGGER_UPDATE_PROPERTIES.Contains( property.Name ) )
                _legacyFlavourSystem.TriggerUpdate( property.Name );
            else if ( TRIGGER_COLOUR_UPDATE_PROPERTIES.Contains( property.Name ) )
                _zoneColourSystem.TriggerUpdate( property.Name );
        }


        /// <summary>
        /// Try to convert a property value
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="val"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryConvertValue( Type propertyType, object val, out object result )
        {
            result = null;
            if ( propertyType == typeof( decimal ) )
            {
                if ( val is string stringValue && int.TryParse( stringValue, out var intValue ) )
                {
                    result = intValue / 100m;
                    return true;
                }
                else if ( val is long longValue )
                {
                    result = longValue / 100m;
                    return true;
                }
                else if ( val is int intValue2 )
                {
                    result = intValue2 / 100m;
                    return true;
                }
            }
            else if ( propertyType.IsEnum && val is string strVal )
            {
                if ( TryParseEnum( propertyType, strVal, out var enumValue ) )
                {
                    result = enumValue;
                    return true;
                }
            }
            return false;
        }

        public bool TryParseEnum( Type enumType, string value, out object result )
        {
            if ( !enumType.IsEnum )
            {
                result = null;
                return false;
            }

            try
            {
                result = Enum.Parse( enumType, value );
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }

}
