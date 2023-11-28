using Game;
using Game.SceneFlow;
using Game.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LegacyFlavour.Systems
{
    public class LegacyFlavourSystem : GameSystemBase
    {
        private static string AssemblyPath = Path.GetDirectoryName( typeof( LegacyFlavourSystem ).Assembly.Location );
        public static string UIPath = AssemblyPath + "\\UI\\";

        public static LegacyFlavourConfig Config
        {
            get
            {
                return _config;
            }
        }

        public static void EnsureModUIFolder( )
        {
            var resourceHandler = ( GameUIResourceHandler ) GameManager.instance.userInterface.view.uiSystem.resourceHandler;

            if ( resourceHandler == null || resourceHandler.HostLocationsMap.ContainsKey( "legacyflavourui" ) )
                return;

            resourceHandler.HostLocationsMap.Add( "legacyflavourui", new List<string> { UIPath } );
        }

        private static LegacyFlavourConfig _config = LegacyFlavourConfig.Load( );

        private ZoneColourSystem _zoneColourSystem;
        private Game.Tools.ToolSystem _toolSystem;

        protected override void OnCreate( )
        {
            base.OnCreate( );
            _zoneColourSystem = World.GetOrCreateSystemManaged<ZoneColourSystem>( );
            _toolSystem = World.GetExistingSystemManaged<Game.Tools.ToolSystem>( );
            SetupKeybinds( );
        }

        /// <summary>
        /// Setup mod keybinds
        /// </summary>
        private void SetupKeybinds( )
        {
            var inputAction = new InputAction( "LegacyFlavour_ToggleUnits" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/alt" )
                .With( "Button", "<Keyboard>/u" );
            inputAction.performed += ( a ) =>
            {
                _config.UseUnits = !_config.UseUnits;
                LegacyFlavourConfig.Save( _config );
            };
            inputAction.Enable( );

            inputAction = new InputAction( "LegacyFlavour_StickyWhiteness" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/alt" )
                .With( "Button", "<Keyboard>/s" );
            inputAction.performed += ( a ) =>
            {
                _config.UseStickyWhiteness = !_config.UseStickyWhiteness;

                LegacyFlavourConfig.Save( _config );

                Debug.Log( "LegacyFlavour Sticky whiteness on: " + _config.UseStickyWhiteness );

                if ( _config.UseStickyWhiteness )
                    Shader.SetGlobalInt( "colossal_InfoviewOn", _toolSystem.activeInfoview?.active == true && _config.WhitenessToggle ? 1 : 0 );
            };
            inputAction.Enable( );

            inputAction = new InputAction( "LegacyFlavour_WhitenessToggle" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/shift" )
                .With( "Button", "<Keyboard>/w" );
            inputAction.performed += ( a ) =>
            {
                if ( _config.UseStickyWhiteness )
                {
                    _config.WhitenessToggle = !_config.WhitenessToggle;
                    Shader.SetGlobalInt( "colossal_InfoviewOn", _toolSystem.activeInfoview?.active == true && _config.WhitenessToggle ? 1 : 0 );
                    LegacyFlavourConfig.Save( _config );
                    Debug.Log("LegacyFlavour Sticky whiteness VALUE = " + _config.WhitenessToggle );
                }
            };
            inputAction.Enable( );


            inputAction = new InputAction( "LegacyFlavour_Reload" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/alt" )
                .With( "Button", "<Keyboard>/r" );
            inputAction.performed += ( a ) =>
            {
                Debug.Log( "LegacyFlavour Config file reloaded!" );
                _config = LegacyFlavourConfig.Load( );
                _zoneColourSystem.ForceUpdate( );
            };
            inputAction.Enable( );


        }

        protected override void OnUpdate( )
        {
        }
    }
}
