using Game;
using Game.Prefabs;
using Game.Zones;
using LegacyFlavour.Patches;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LegacyFlavour.Systems
{
    public class ZoneColourSystem : GameSystemBase
    {
        static FieldInfo m_FillColorArray = typeof( ZoneSystem ).GetFields( BindingFlags.Instance | BindingFlags.NonPublic )
            .FirstOrDefault( m => m.Name == "m_FillColorArray" );

        static FieldInfo m_EdgeColorArray = typeof( ZoneSystem ).GetFields( BindingFlags.Instance | BindingFlags.NonPublic )
            .FirstOrDefault( m => m.Name == "m_EdgeColorArray" );

        static MethodInfo _updateAllZoneColors = typeof( ZoneSystem ).GetMethods( BindingFlags.Instance | BindingFlags.NonPublic )
            .FirstOrDefault( m => m.Name == "UpdateZoneColors" && m.GetParameters( ).Length == 0 );

        static LegacyFlavourConfig _config = LegacyFlavourConfig.Load( );

        private Dictionary<string, Color> Colours
        {
            get
            {
                switch ( colourBlindness )
                {
                    case ColourBlindness.Deuteranopia:
                        return _config.Zones.ToDictionary( k => k.Name, v => HexToColor( v.Deuteranopia == "default" ? v.Colour : v.Deuteranopia ) );
                    case ColourBlindness.Protanopia:
                        return _config.Zones.ToDictionary( k => k.Name, v => HexToColor( v.Protanopia == "default" ? v.Colour : v.Protanopia ) );
                    case ColourBlindness.Tritanopia:
                        return _config.Zones.ToDictionary( k => k.Name, v => HexToColor( v.Tritanopia == "default" ? v.Colour : v.Tritanopia ) );
                    case ColourBlindness.Custom:
                        return _config.Zones.ToDictionary( k => k.Name, v => HexToColor( v.Custom == "default" ? v.Colour : v.Custom ) );
                    default:
                        return _config.Zones.ToDictionary( k => k.Name, v => HexToColor( v.Colour ) );
                }
            }
        }

        public bool enabled = true;
        public ColourBlindness colourBlindness = ColourBlindness.None;

        private ZoneSystem _zoneSystem;

        protected override void OnCreate( )
        {
            base.OnCreate( );

            colourBlindness = _config.Mode;
            _zoneSystem = World.GetExistingSystemManaged<ZoneSystem>( );

            var inputAction = new InputAction( "LegacyFlavour_Toggle" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/alt" )
                .With( "Button", "<Keyboard>/z" );

            inputAction.performed += ( a ) =>
            {
                enabled = !enabled;
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
                Debug.Log( "Switched to mode: " + colourBlindness );
                ForceUpdate( );
            };

            inputAction.Enable( );
        }

        protected override void OnUpdate( )
        {
            throw new System.NotImplementedException( );
        }

        public void ForceUpdate( )
        {
            _updateAllZoneColors.Invoke( _zoneSystem, null );
        }

        private Color HexToColor( string hex )
        {
            if ( ColorUtility.TryParseHtmlString( hex, out var color ) )
                return color;

            return Color.white;
        }

        private Color Darken( Color originalColor, float darkenAmount )
        {
            // Convert the color to HSV
            Color.RGBToHSV( originalColor, out float H, out float S, out float V );

            // Decrease the V (value/brightness) by the darken amount, clamping it between 0 and 1
            V = Mathf.Clamp01( V - darkenAmount );

            // Convert back to RGB and return the new color
            return Color.HSVToRGB( H, S, V );
        }

        private Color SetAlpha( Color originalColor, float opacity )
        {
            return new Color( originalColor.r, originalColor.g, originalColor.b, opacity );
        }

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

                var borderColor = Darken( overrideColor, 0.23f );
                var isEmpty = zonePrefab.m_AreaType == AreaType.None;

                var fillColorArray = ( Vector4[] ) m_FillColorArray.GetValue( _zoneSystem );
                var edgeColorArray = ( Vector4[] ) m_EdgeColorArray.GetValue( _zoneSystem );
                var transparent = new Color( 0f, 0f, 0f, 0f );
                var isSnowing = ZoneSystem_UpdatePatch.isSnowing;

                if ( isEmpty && isSnowing )
                    borderColor = new Color( 0f, 0f, 0f, 0.75f );

                fillColorArray[colourIndex1] = ( Vector4 ) ( isEmpty ? transparent : SetAlpha( overrideColor, 0.9f ) );
                edgeColorArray[colourIndex1] = ( Vector4 ) ( isEmpty ? isSnowing ? borderColor : SetAlpha( borderColor, 0.1f ) : SetAlpha( borderColor, 0.9f ) );

                fillColorArray[colourIndex2] = ( Vector4 ) ( isEmpty ? transparent : SetAlpha( overrideColor, 0.9f ) );
                edgeColorArray[colourIndex2] = ( Vector4 ) ( isEmpty ? SetAlpha( borderColor, 0.2f ) : SetAlpha( borderColor, 0.9f ) );

                fillColorArray[colourIndex3] = ( Vector4 ) ( isEmpty ? transparent : overrideColor );
                edgeColorArray[colourIndex3] = ( Vector4 ) ( isEmpty ? SetAlpha( borderColor, 0.2f ) : borderColor );
                return false;
            }

            return true;
        }
    }
}
