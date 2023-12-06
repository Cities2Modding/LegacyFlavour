using Colossal.Serialization.Entities;
using Game;
using Game.Rendering;
using Game.SceneFlow;
using Game.Simulation;
using Game.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LegacyFlavour.Systems
{
    /// <summary>
    /// Core Legacy Flavour system, handles time/weather and other stuff.
    /// </summary>
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

        private bool isInitialised;
        private ZoneColourSystem _zoneColourSystem;
        private PlanetarySystem _planetarySystem;
        private ClimateSystem _climateSystem;
        private TimeSystem _timeSystem;
        private LightingSystem _lightingSystem;
        private Game.Tools.ToolSystem _toolSystem;
        private float targetVisualTime;
        private bool seekGoldenHour;

        protected override void OnCreate( )
        {
            base.OnCreate( );
            SetupKeybinds( );
        }

        protected override void OnGameLoadingComplete( Purpose purpose, GameMode mode )
        {
            base.OnGameLoadingComplete( purpose, mode );

            if ( !mode.IsGameOrEditor( ) )
                return;

            if ( _lightingSystem == null )
            {
                _lightingSystem = World.GetExistingSystemManaged<LightingSystem>( );
                _zoneColourSystem = World.GetOrCreateSystemManaged<ZoneColourSystem>( );
                _toolSystem = World.GetExistingSystemManaged<Game.Tools.ToolSystem>( );
                _climateSystem = World.GetExistingSystemManaged<ClimateSystem>( );
                _timeSystem = World.GetExistingSystemManaged<TimeSystem>( );
                _planetarySystem = World.GetExistingSystemManaged<PlanetarySystem>( );
            }
            UpdateTimeOfDay( );
            UpdateWeather( );
            isInitialised = true;
        }

        /// <summary>
        /// Trigger updates to effects for properties
        /// </summary>
        /// <param name="property"></param>
        public void TriggerUpdate( string property )
        {
            switch ( property )
            {
                case "UseStickyWhiteness":
                    if ( _config.UseStickyWhiteness )
                        Shader.SetGlobalInt( "colossal_InfoviewOn", _toolSystem.activeInfoview?.active == true && _config.WhitenessToggle ? 1 : 0 );
                    break;

                case "WhitenessToggle":
                    if ( _config.UseStickyWhiteness )
                        Shader.SetGlobalInt( "colossal_InfoviewOn", _toolSystem.activeInfoview?.active == true && _config.WhitenessToggle ? 1 : 0 );
                    break;

                case "FreezeVisualTime":
                case "TimeOfDay":
                    UpdateTimeOfDay( );
                    break;

                case "Weather":
                    UpdateWeather( );
                    break;
            }
        }

        /// <summary>
        /// Update the time of day based on our config
        /// </summary>
        private void UpdateTimeOfDay( )
        {
            // When we're sync'ing visual and actual time these options will conflict
            if ( _config.FreezeVisualTime && _config.TimeOfDay == TimeOfDayOverride.Off )
            {
                _planetarySystem.overrideTime = true;
            }
            else
            {
                switch ( _config.TimeOfDay )
                {
                    default:
                    case TimeOfDayOverride.Off:
                        if ( !_config.FreezeVisualTime )
                            _planetarySystem.overrideTime = false;
                        targetVisualTime = 0f;
                        break;

                    case TimeOfDayOverride.Night:
                        _planetarySystem.overrideTime = true;
                        targetVisualTime = 0f;
                        break;

                    case TimeOfDayOverride.GoldenHour:
                        _planetarySystem.overrideTime = true;

                        _planetarySystem.time = 0f;
                        targetVisualTime = 24f;
                        seekGoldenHour = true;
                        break;

                    case TimeOfDayOverride.Day:
                        _planetarySystem.overrideTime = true;
                        targetVisualTime = 12f;
                        break;
                }
            }
        }

        /// <summary>
        /// Check if it's sunrise or sunset
        /// </summary>
        /// <returns></returns>
        private bool IsSunriseOrSunset( )
        {
            return _lightingSystem.state == LightingSystem.State.Sunset || _lightingSystem.state == LightingSystem.State.Sunrise;
        }

        /// <summary>
        /// Update weather based on config
        /// </summary>
        private void UpdateWeather( )
        {
            switch ( _config.Weather )
            {
                case WeatherOverride.Off:
                    _climateSystem.temperature.overrideState = false;
                    _climateSystem.precipitation.overrideState = false;
                    _climateSystem.cloudiness.overrideState = false;
                    break;

                case WeatherOverride.Sun:
                    _climateSystem.temperature.overrideState = true;
                    _climateSystem.temperature.overrideValue = 28f;
                    _climateSystem.precipitation.overrideState = true;
                    _climateSystem.precipitation.overrideValue = 0f;
                    _climateSystem.cloudiness.overrideState = true;
                    _climateSystem.cloudiness.overrideValue = 0f;
                    break;

                case WeatherOverride.Overcast:
                    _climateSystem.temperature.overrideState = true;
                    _climateSystem.temperature.overrideValue = 15f;
                    _climateSystem.precipitation.overrideState = true;
                    _climateSystem.precipitation.overrideValue = 0f;
                    _climateSystem.cloudiness.overrideState = true;
                    _climateSystem.cloudiness.overrideValue = 0.8f;
                    break;

                case WeatherOverride.Rain:
                    _climateSystem.temperature.overrideState = true;
                    _climateSystem.temperature.overrideValue = 15f;
                    _climateSystem.precipitation.overrideState = true;
                    _climateSystem.precipitation.overrideValue = 1f;
                    _climateSystem.cloudiness.overrideState = true;
                    _climateSystem.cloudiness.overrideValue = 0.8f;
                    break;

                case WeatherOverride.Snow:
                    _climateSystem.temperature.overrideState = true;
                    _climateSystem.temperature.overrideValue = -3f;
                    _climateSystem.precipitation.overrideState = true;
                    _climateSystem.precipitation.overrideValue = 1f;
                    _climateSystem.cloudiness.overrideState = true;
                    _climateSystem.cloudiness.overrideValue = 0.8f;
                    break;
            }
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

                TriggerUpdate( "UseStickyWhiteness" );
               
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
                    LegacyFlavourConfig.Save( _config );                    
                    TriggerUpdate( "WhitenessToggle" );
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
                _zoneColourSystem.ForceUpdate( true );
            };
            inputAction.Enable( );
        }

        protected override void OnUpdate( )
        {
            if ( !isInitialised || !_config.FreezeVisualTime || _planetarySystem == null )
                return;

            if ( seekGoldenHour && ( _config.TimeOfDay != TimeOfDayOverride.GoldenHour || IsSunriseOrSunset( ) ))
            {
                seekGoldenHour = false;
                targetVisualTime = _planetarySystem.time;
            }

            _planetarySystem.time = Mathf.Lerp( _planetarySystem.time, targetVisualTime, 1.5f * UnityEngine.Time.deltaTime );
        }
    }
}
