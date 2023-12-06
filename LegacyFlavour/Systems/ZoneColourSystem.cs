using Game;
using Game.Prefabs;
using Game.Zones;
using LegacyFlavour.Helpers;
using LegacyFlavour.MonoBehaviours;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static ATL.AudioData.FileStructureHelper;
using static System.Net.Mime.MediaTypeNames;

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

        private static LegacyFlavourConfig _config = LegacyFlavourSystem.Config;

        public Dictionary<string, Color> Colours
        {
            get
            {
                switch ( colourBlindness )
                {
                    case ColourBlindness.Deuteranopia:
                        return _config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Deuteranopia == "default" ? v.Colour : v.Deuteranopia ) );
                    case ColourBlindness.Protanopia:
                        return _config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Protanopia == "default" ? v.Colour : v.Protanopia ) );
                    case ColourBlindness.Tritanopia:
                        return _config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Tritanopia == "default" ? v.Colour : v.Tritanopia ) );
                    case ColourBlindness.Custom:
                        return _config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Custom == "default" ? v.Colour : v.Custom ) );
                    default:
                        return _config.Zones.ToDictionary( k => k.Name, v => ColourHelpers.HexToColor( v.Colour ) );
                }
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

        public bool enabled = true;
        public ColourBlindness colourBlindness = _config.Mode;

        private bool hasSnow;
        private ZoneSystem _zoneSystem;
        private SnowDetector _snowDetector;
        private DynamicZoneIcons _dynamicZoneIcons;

        protected override void OnCreate( )
        {
            base.OnCreate( );

            _dynamicZoneIcons = new DynamicZoneIcons( );
            _zoneSystem = World.GetExistingSystemManaged<ZoneSystem>( );

            SetupSnowDetector( );
            Configure( );
            SetupKeybinds( );
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
            enabled = _config.Enabled;
            _snowDetector.Disable = !enabled;
            colourBlindness = _config.Mode;
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
                    colourBlindness = _config.Mode;
                    ForceUpdate( );
                    break;

                case "CellOpacity":
                case "EmptyCellOpacity":
                case "CellBorderOpacity":
                case "EmptyCellBorderOpacity":
                    ForceUpdate( );
                    break;

                case "UseDynamicCellBorders":
                    _snowDetector.Disable = !_config.UseDynamicCellBorders;
                    break;
            }
        }

        private void UpdateEnabledStatus( bool invalidateIcons = false )
        {
            enabled = _config.Enabled;
            _snowDetector.Disable = !enabled;
            ForceUpdate( invalidateIcons );
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
                _config.Enabled = enabled;
                _snowDetector.Disable = !enabled;
                LegacyFlavourConfig.Save( _config );
                ForceUpdate( );
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
                _config.Mode = colourBlindness;
                LegacyFlavourConfig.Save( _config );
                ForceUpdate( );
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

                ForceUpdate( );
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
            foreach ( var zone in _config.Zones )
            {
                if ( !_vanillaColours.ContainsKey( zone.Name ) )
                    continue;

                var vanilla = _vanillaColours[zone.Name];

                switch ( _config.Mode )
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

            LegacyFlavourConfig.Save( _config );

            ForceUpdate( true );
        }

        /// <summary>
        /// Reset zone settings to default
        /// </summary>
        public void ResetSettingsToDefault( )
        {
            var defaultConfig = LegacyFlavourConfig.Default;
            _config.Enabled = defaultConfig.Enabled;
            _config.UseDynamicCellBorders = defaultConfig.UseDynamicCellBorders;
            _config.CellOpacity = defaultConfig.CellOpacity;
            _config.CellBorderOpacity = defaultConfig.CellBorderOpacity;
            _config.EmptyCellOpacity = defaultConfig.EmptyCellOpacity;
            _config.EmptyCellBorderOpacity = defaultConfig.EmptyCellBorderOpacity;

            LegacyFlavourConfig.Save( _config );

            ForceUpdate( );
        }

        /// <summary>
        /// Reset the colours to default config
        /// </summary>
        public void ResetColoursToDefault( )
        {
            foreach ( var zone in _config.Zones )
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

            LegacyFlavourConfig.Save( _config );

            ForceUpdate( true );
        }

        /// <summary>
        /// Update a zone colour
        /// </summary>
        /// <param name="name"></param>
        /// <param name="colour"></param>
        public void UpdateZoneColour( string name, string colour )
        {
            var zone = _config.Zones.FirstOrDefault( z => z.Name == name );

            if ( zone == null )
                return;

            switch ( _config.Mode )
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

            LegacyFlavourConfig.Save( _config );

            ForceUpdate( true );
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
                var transparent = new Color( 0f, 0f, 0f, ( float )_config.EmptyCellOpacity );

                if ( isEmpty && hasSnow )
                    borderColor = new Color( 0f, 0f, 0f, 0.75f );

                var fillOpacity = ( float ) _config.CellOpacity;
                var fillOpacity2 = Mathf.Clamp01( fillOpacity - 0.1f );
                var borderOpacity = ( float ) _config.CellBorderOpacity;
                var emptyBorderOpacity = ( float ) _config.EmptyCellBorderOpacity;

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
