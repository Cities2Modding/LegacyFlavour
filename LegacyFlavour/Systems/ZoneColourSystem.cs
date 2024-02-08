using Game;
using Game.Prefabs;
using Game.Zones;
using LegacyFlavour.Configuration;
using LegacyFlavour.Helpers;
using LegacyFlavour.MonoBehaviours;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LegacyFlavour.Systems
{
    /// <summary>
    /// Handles updating zone colours
    /// </summary>
    public class ZoneColourSystem : GameSystemBase
    {
        static FieldInfo m_FillColorArray = typeof( ZoneSystem ).GetFields( BindingFlags.Instance | BindingFlags.NonPublic )
            .FirstOrDefault( m => m.Name == "m_FillColorArray" );

        static FieldInfo m_EdgeColorArray = typeof( ZoneSystem ).GetFields( BindingFlags.Instance | BindingFlags.NonPublic )
            .FirstOrDefault( m => m.Name == "m_EdgeColorArray" );

        static MethodInfo _updateAllZoneColors = typeof( ZoneSystem ).GetMethods( BindingFlags.Instance | BindingFlags.NonPublic )
            .FirstOrDefault( m => m.Name == "UpdateZoneColors" && m.GetParameters( ).Length == 0 );


        /// <summary>
        /// All colours
        /// </summary>
        public (ColourBlindness, Dictionary<string, Color>)[] AllColours
        {
            get
            {
                return new[]
                {
                    (ColourBlindness.None, GetColourSet( ColourBlindness.None )),
                    (ColourBlindness.Deuteranopia, GetColourSet( ColourBlindness.Deuteranopia )),
                    (ColourBlindness.Protanopia, GetColourSet( ColourBlindness.Protanopia )),
                    (ColourBlindness.Tritanopia, GetColourSet( ColourBlindness.Tritanopia )),
                    (ColourBlindness.Custom, GetColourSet( ColourBlindness.Custom )),
                };
            }
        }

        public Dictionary<string, Color> Colours
        {
            get
            {
                return GetColourSet( colourBlindness );
            }
        }

        /// <summary>
        /// Dynamically changes the colour of zone icons
        /// </summary>
        public DynamicZoneIcons DynamicZoneIcons
        {
            get
            {
                return _dynamicZoneIcons;
            }
        }

        private static Dictionary<string, string> _vanillaColours = new Dictionary<string, string>( );

        private LegacyFlavourConfig Config
        {
            get
            {
                return _updateSystem.Config;
            }
        }

        public bool enabled = true;
        public ColourBlindness colourBlindness;

        private bool hasSnow;
        private LegacyFlavourUpdateSystem _updateSystem;
        private ZoneSystem _zoneSystem;
        private SnowDetector _snowDetector;
        private DynamicZoneIcons _dynamicZoneIcons;

        protected override void OnCreate( )
        {
            base.OnCreate( );

            _updateSystem = World.GetOrCreateSystemManaged<LegacyFlavourUpdateSystem>( );

            _dynamicZoneIcons = new DynamicZoneIcons( );
            _zoneSystem = World.GetExistingSystemManaged<ZoneSystem>( );

            SetupSnowDetector( );
            Configure( );
            SetupKeybinds( );

            _updateSystem.EnqueueColoursUpdate( invalidateCache: true );
        }

        /// <summary>
        /// Not used
        /// </summary>
        protected override void OnUpdate( )
        {
        }

        /// <summary>
        /// Configure defaults
        /// </summary>
        private void Configure( )
        {
            colourBlindness = Config.Mode;
            enabled = Config.Enabled;
            _snowDetector.Disable = !enabled;
            colourBlindness = Config.Mode;
        }

        /// <summary>
        /// Get a colour set
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private Dictionary<string, Color> GetColourSet( ColourBlindness mode )
        {
            switch ( mode )
            {
                case ColourBlindness.Deuteranopia:
                    return Config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Deuteranopia == "default" ? v.Colour : v.Deuteranopia ) );
                case ColourBlindness.Protanopia:
                    return Config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Protanopia == "default" ? v.Colour : v.Protanopia ) );
                case ColourBlindness.Tritanopia:
                    return Config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Tritanopia == "default" ? v.Colour : v.Tritanopia ) );
                case ColourBlindness.Custom:
                    return Config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Custom == "default" ? v.Colour : v.Custom ) );
                default:
                    return Config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Colour ) );
            }
        }

        /// <summary>
        /// Trigger an update on a config property
        /// </summary>
        /// <param name="property"></param>
        public void TriggerUpdate( string property )
        {
            switch ( property )
            {
                case "Enabled":
                    UpdateEnabledStatus( );
                    break;

                case "Mode":
                    colourBlindness = Config.Mode;
                    _updateSystem.EnqueueColoursUpdate( );
                    break;

                case "OverrideIcons":
                case "CellOpacity":
                case "EmptyCellOpacity":
                case "CellBorderOpacity":
                case "EmptyCellBorderOpacity":
                    _updateSystem.EnqueueColoursUpdate( );
                    break;

                case "UseDynamicCellBorders":
                    _snowDetector.Disable = !Config.UseDynamicCellBorders;
                    break;
            }
        }

        private void UpdateEnabledStatus( bool invalidateIcons = false )
        {
            enabled = Config.Enabled;
            _snowDetector.Disable = !enabled;
            _updateSystem.EnqueueColoursUpdate( invalidateCache: invalidateIcons );
        }

        /// <summary>
        /// Setup zone colour keybinds
        /// </summary>
        private void SetupKeybinds( )
        {
            var inputAction = new InputAction( "LegacyFlavour_ColourToggle" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/alt" )
                .With( "Button", "<Keyboard>/z" );

            inputAction.performed += ( a ) =>
            {
                enabled = !enabled;
                Config.Enabled = enabled;
                _snowDetector.Disable = !enabled;
                _updateSystem.EnqueueConfigUpdate( );
                _updateSystem.EnqueueColoursUpdate( );
            };

            inputAction.Enable( );

            inputAction = new InputAction( "LegacyFlavour_Cycle" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/shift" )
                .With( "Button", "<Keyboard>/z" );
            inputAction.performed += ( a ) =>
            {
                var next = ( int ) colourBlindness + 1;

                if ( next >= System.Enum.GetValues( typeof( ColourBlindness ) ).Length )
                    next = 0;

                colourBlindness = ( ColourBlindness ) next;
                Config.Mode = colourBlindness;
                _updateSystem.EnqueueConfigUpdate( );
                _updateSystem.EnqueueColoursUpdate( );
            };

            inputAction.Enable( );
        }

        /// <summary>
        /// Setup the snow detector that adjusts border colours dynamically
        /// </summary>
        private void SetupSnowDetector( )
        {
            _snowDetector = SnowDetector.Create( );
            _snowDetector.OnUpdate += ( coverage ) =>
            {
                if ( coverage >= 20f )
                    hasSnow = true;
                else
                    hasSnow = false;

                _updateSystem.EnqueueColoursUpdate( );
            };
        }

        /// <summary>
        /// Force a zone colours update
        /// </summary>
        /// <param name="invalidateCache"></param>
        public void ForceUpdate( bool invalidateCache = false )
        {
            _dynamicZoneIcons?.ModifyBasedOnColours( invalidateCache );
            _updateAllZoneColors.Invoke( _zoneSystem, null );
        }

        /// <summary>
        /// Set the current mode to vanilla colours
        /// </summary>
        public void SetCurrentToVanilla( )
        {
            foreach ( var zone in Config.Zones )
            {
                if ( !_vanillaColours.ContainsKey( zone.Name ) )
                    continue;

                var vanilla = _vanillaColours[zone.Name];

                switch ( Config.Mode )
                {
                    case ColourBlindness.Deuteranopia:
                        zone.Deuteranopia = vanilla;
                        break;

                    case ColourBlindness.Protanopia:
                        zone.Protanopia = vanilla;
                        break;

                    case ColourBlindness.Tritanopia:
                        zone.Tritanopia = vanilla;
                        break;

                    case ColourBlindness.Custom:
                        zone.Custom = vanilla;
                        break;

                    case ColourBlindness.None:
                        zone.Colour = vanilla;
                        break;
                }
            }

            _updateSystem.EnqueueConfigUpdate( );
            _updateSystem.EnqueueColoursUpdate( invalidateCache: true );
        }

        /// <summary>
        /// Reset zone settings to default
        /// </summary>
        public void ResetSettingsToDefault( )
        {
            var defaultConfig = LegacyFlavourConfig.Default;
            Config.Enabled = defaultConfig.Enabled;
            Config.UseDynamicCellBorders = defaultConfig.UseDynamicCellBorders;
            Config.CellOpacity = defaultConfig.CellOpacity;
            Config.CellBorderOpacity = defaultConfig.CellBorderOpacity;
            Config.EmptyCellOpacity = defaultConfig.EmptyCellOpacity;
            Config.EmptyCellBorderOpacity = defaultConfig.EmptyCellBorderOpacity;

            _updateSystem.EnqueueConfigUpdate( );
            _updateSystem.EnqueueColoursUpdate( );
        }

        /// <summary>
        /// Set the settings profile to a preset
        /// </summary>
        /// <param name="name"></param>
        public void SetToPreset( string name )
        {
            var defaultConfig = LegacyFlavourConfig.Default;
            Config.Enabled = true;
            Config.UseDynamicCellBorders = true;

            if ( !string.IsNullOrEmpty( name ) && name == "CityPlannerSpecial" )
            {
                Config.CellOpacity = 0.1m;
                Config.CellBorderOpacity = 0.95m;
                Config.EmptyCellOpacity = 0m;
                Config.EmptyCellBorderOpacity = 0.5m;
            }
            else
            {
                Config.CellOpacity = defaultConfig.CellOpacity;
                Config.CellBorderOpacity = defaultConfig.CellBorderOpacity;
                Config.EmptyCellOpacity = defaultConfig.EmptyCellOpacity;
                Config.EmptyCellBorderOpacity = defaultConfig.EmptyCellBorderOpacity;
            }

            _updateSystem.EnqueueConfigUpdate( );
            _updateSystem.EnqueueColoursUpdate( );
        }

        /// <summary>
        /// Reset the colours to default config
        /// </summary>
        public void ResetColoursToDefault( )
        {
            foreach ( var zone in Config.Zones )
            {
                var defaultZone = LegacyFlavourConfig.Default.Zones
                    .FirstOrDefault( z => z.Name == zone.Name );

                if ( defaultZone == null )
                    continue;

                zone.Deuteranopia = defaultZone.Deuteranopia;
                zone.Protanopia = defaultZone.Protanopia;
                zone.Tritanopia = defaultZone.Tritanopia;
                zone.Custom = defaultZone.Custom;
                zone.Colour = defaultZone.Colour;
            }

            _updateSystem.EnqueueConfigUpdate( );
            _updateSystem.EnqueueColoursUpdate( invalidateCache: true );
        }

        /// <summary>
        /// Update a zone colour
        /// </summary>
        /// <param name="name"></param>
        /// <param name="colour"></param>
        public void UpdateZoneColour( string name, string colour )
        {
            var zone = Config.Zones.FirstOrDefault( z => z.Name == name );

            if ( zone == null )
                return;

            switch ( Config.Mode )
            {
                case ColourBlindness.Deuteranopia:
                    zone.Deuteranopia = colour;
                    break;

                case ColourBlindness.Protanopia:
                    zone.Protanopia = colour;
                    break;

                case ColourBlindness.Tritanopia:
                    zone.Tritanopia = colour;
                    break;

                case ColourBlindness.Custom:
                    zone.Custom = colour;
                    break;

                case ColourBlindness.None:
                    zone.Colour = colour;
                    break;
            }

            _updateSystem.EnqueueConfigUpdate( );
            _updateSystem.EnqueueColoursUpdate( invalidateCache: true );
        }

        /// <summary>
        /// Update zone prefab colours
        /// </summary>
        /// <param name="zonePrefab"></param>
        /// <param name="zoneData"></param>
        /// <returns></returns>
        public bool UpdateZones( ZonePrefab zonePrefab, ZoneData zoneData )
        {
            var vanilla = !enabled;

            if ( !vanilla )
            {
                var color = zonePrefab.m_Color;

                var colourIndex1 = ZoneUtils.GetColorIndex( CellFlags.Visible, zoneData.m_ZoneType );
                var colourIndex2 = ZoneUtils.GetColorIndex( CellFlags.Visible | CellFlags.Occupied, zoneData.m_ZoneType );
                var colourIndex3 = ZoneUtils.GetColorIndex( CellFlags.Visible | CellFlags.Selected, zoneData.m_ZoneType );

                var nameLessAreaType = zonePrefab.name;

                if ( nameLessAreaType.StartsWith( "EU " ) || nameLessAreaType.StartsWith( "NA " ) )
                    nameLessAreaType = nameLessAreaType[3..];

                if ( !_vanillaColours.ContainsKey( nameLessAreaType ) )
                    _vanillaColours[nameLessAreaType] = ColourHelpers.ColorToHex( color );

                var overrideColor = color;
                var colors = Colours;
                    
                if ( colors.ContainsKey( nameLessAreaType ) )
                    overrideColor = colors[nameLessAreaType];

                var borderColor = ColourHelpers.Darken( overrideColor, 0.23f );
                var isEmpty = zonePrefab.m_AreaType == AreaType.None;

                var fillColorArray = ( Vector4[] ) m_FillColorArray.GetValue( _zoneSystem );
                var edgeColorArray = ( Vector4[] ) m_EdgeColorArray.GetValue( _zoneSystem );
                var transparent = new Color( 0f, 0f, 0f, ( float ) Config.EmptyCellOpacity );

                if ( isEmpty && hasSnow )
                    borderColor = new Color( 0f, 0f, 0f, 0.75f );

                var fillOpacity = ( float ) Config.CellOpacity;
                var fillOpacity2 = Mathf.Clamp01( fillOpacity - 0.1f );
                var borderOpacity = ( float ) Config.CellBorderOpacity;
                var emptyBorderOpacity = ( float ) Config.EmptyCellBorderOpacity;

                fillColorArray[colourIndex1] = ( Vector4 ) ( isEmpty ? transparent : ColourHelpers.SetAlpha( overrideColor, fillOpacity2 ) );
                edgeColorArray[colourIndex1] = ( Vector4 ) ( isEmpty ? hasSnow ? borderColor : ColourHelpers.SetAlpha( borderColor, emptyBorderOpacity ) : ColourHelpers.SetAlpha( borderColor, borderOpacity ) );

                fillColorArray[colourIndex2] = ( Vector4 ) ( isEmpty ? transparent : ColourHelpers.SetAlpha( overrideColor, fillOpacity2 ) );
                edgeColorArray[colourIndex2] = ( Vector4 ) ( isEmpty ? ColourHelpers.SetAlpha( borderColor, emptyBorderOpacity ) : ColourHelpers.SetAlpha( borderColor, borderOpacity ) );

                fillColorArray[colourIndex3] = ( Vector4 ) ( isEmpty ? transparent : ColourHelpers.SetAlpha( overrideColor, fillOpacity ) );
                edgeColorArray[colourIndex3] = ( Vector4 ) ( isEmpty ? ColourHelpers.SetAlpha( borderColor, emptyBorderOpacity ) : borderColor );
                return false;
            }

            return true;
        }
    }
}
