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
                Debug.Log( "LegacyFlavour switched to mode: " + colourBlindness );
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
        public void ForceUpdate( )
        {
            _dynamicZoneIcons?.ModifyBasedOnColours( );
            _updateAllZoneColors.Invoke( _zoneSystem, null );
        }

        /// <summary>
        /// Update zone prefab colours
        /// </summary>
        /// <param name="zonePrefab"></param>
        /// <param name="zoneData"></param>
        /// <returns></returns>
        public bool UpdateZones( ZonePrefab zonePrefab, ZoneData zoneData )
        {
            var color = zonePrefab.m_Color;

            var colourIndex1 = ZoneUtils.GetColorIndex( CellFlags.Visible, zoneData.m_ZoneType );
            var colourIndex2 = ZoneUtils.GetColorIndex( CellFlags.Visible | CellFlags.Occupied, zoneData.m_ZoneType );
            var colourIndex3 = ZoneUtils.GetColorIndex( CellFlags.Visible | CellFlags.Selected, zoneData.m_ZoneType );
            var vanilla = !enabled;

            if ( !vanilla )
            {
                var overrideColor = color;
                var colors = Colours;
                var nameLessAreaType = zonePrefab.name;

                if ( nameLessAreaType.StartsWith( "EU " ) || nameLessAreaType.StartsWith( "NA " ) )
                    nameLessAreaType = nameLessAreaType[3..];

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
                fillColorArray[colourIndex1] = ( Vector4 ) ( isEmpty ? transparent : ColourHelpers.SetAlpha( overrideColor, fillOpacity2 ) );
                edgeColorArray[colourIndex1] = ( Vector4 ) ( isEmpty ? hasSnow ? borderColor : ColourHelpers.SetAlpha( borderColor, 0.1f ) : ColourHelpers.SetAlpha( borderColor, 0.9f ) );

                fillColorArray[colourIndex2] = ( Vector4 ) ( isEmpty ? transparent : ColourHelpers.SetAlpha( overrideColor, fillOpacity2 ) );
                edgeColorArray[colourIndex2] = ( Vector4 ) ( isEmpty ? ColourHelpers.SetAlpha( borderColor, 0.2f ) : ColourHelpers.SetAlpha( borderColor, 0.9f ) );

                fillColorArray[colourIndex3] = ( Vector4 ) ( isEmpty ? transparent : ColourHelpers.SetAlpha( overrideColor, fillOpacity ) );
                edgeColorArray[colourIndex3] = ( Vector4 ) ( isEmpty ? ColourHelpers.SetAlpha( borderColor, 0.2f ) : borderColor );
                return false;
            }

            return true;
        }
    }
}
