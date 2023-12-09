using Colossal.Serialization.Entities;
using Game;
using Game.Rendering;
using Game.SceneFlow;
using Game.Simulation;
using Game.UI;
using LegacyFlavour.Configuration;
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

        public static void EnsureModUIFolder( )
        {
            var resourceHandler = ( GameUIResourceHandler ) GameManager.instance.userInterface.view.uiSystem.resourceHandler;

            if ( resourceHandler == null || resourceHandler.HostLocationsMap.ContainsKey( "legacyflavourui" ) )
                return;

            resourceHandler.HostLocationsMap.Add( "legacyflavourui", new List<string> { UIPath } );
        }

        private LegacyFlavourConfig Config
        {
            get
            {
                return _updateSystem.Config;
            }
        }

        private bool isInitialised;
        private LegacyFlavourUpdateSystem _updateSystem;
        private PlanetarySystem _planetarySystem;
        private ClimateSystem _climateSystem;
        private LightingSystem _lightingSystem;
        private Game.Tools.ToolSystem _toolSystem;
        private float targetVisualTime;
        private bool seekGoldenHour;

        protected override void OnCreate( )
        {
            base.OnCreate( );

            _updateSystem = World.GetExistingSystemManaged<LegacyFlavourUpdateSystem>( );

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
                _toolSystem = World.GetExistingSystemManaged<Game.Tools.ToolSystem>( );
                _climateSystem = World.GetExistingSystemManaged<ClimateSystem>( );
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
                    if ( Config.UseStickyWhiteness )
                        Shader.SetGlobalInt( "colossal_InfoviewOn", _toolSystem.activeInfoview?.active == true &&
                            Config.WhitenessToggle ? 1 : 0 );
                    break;

                case "WhitenessToggle":
                    if ( Config.UseStickyWhiteness )
                        Shader.SetGlobalInt( "colossal_InfoviewOn", _toolSystem.activeInfoview?.active == true &&
                            Config.WhitenessToggle ? 1 : 0 );
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
            if ( Config.FreezeVisualTime && Config.TimeOfDay == TimeOfDayOverride.Off )
            {
                _planetarySystem.overrideTime = true;
            }
            else
            {
                switch ( Config.TimeOfDay )
                {
                    default:
                    case TimeOfDayOverride.Off:
                        if ( !Config.FreezeVisualTime )
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
            switch ( Config.Weather )
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
                Config.UseUnits = !Config.UseUnits;
                _updateSystem.EnqueueConfigUpdate( );
            };
            inputAction.Enable( );

            inputAction = new InputAction( "LegacyFlavour_StickyWhiteness" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/alt" )
                .With( "Button", "<Keyboard>/s" );
            inputAction.performed += ( a ) =>
            {
                Config.UseStickyWhiteness = !Config.UseStickyWhiteness;

                _updateSystem.EnqueueConfigUpdate( );

                TriggerUpdate( "UseStickyWhiteness" );
               
            };
            inputAction.Enable( );

            inputAction = new InputAction( "LegacyFlavour_WhitenessToggle" );
            inputAction.AddCompositeBinding( "ButtonWithOneModifier" )
                .With( "Modifier", "<Keyboard>/shift" )
                .With( "Button", "<Keyboard>/w" );
            inputAction.performed += ( a ) =>
            {
                if ( Config.UseStickyWhiteness )
                {
                    Config.WhitenessToggle = !Config.WhitenessToggle;
                    _updateSystem.EnqueueConfigUpdate( );
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
                _updateSystem.Reload( );
                _updateSystem.EnqueueColoursUpdate( invalidateCache: true );
            };
            inputAction.Enable( );
        }

        protected override void OnUpdate( )
        {
            if ( !isInitialised || !Config.FreezeVisualTime || _planetarySystem == null )
                return;

            if ( seekGoldenHour && ( Config.TimeOfDay != TimeOfDayOverride.GoldenHour || IsSunriseOrSunset( ) ))
            {
                seekGoldenHour = false;
                targetVisualTime = _planetarySystem.time;
            }

            _planetarySystem.time = Mathf.Lerp( _planetarySystem.time, targetVisualTime, 1.5f * UnityEngine.Time.deltaTime );
        }
    }
}
