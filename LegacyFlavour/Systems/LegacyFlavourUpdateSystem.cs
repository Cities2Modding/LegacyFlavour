using Game;
using LegacyFlavour.Configuration;
using LegacyFlavour.Configuration.Themes;
using System;
using LegacyFlavour.Helpers;
using Game.UI;

namespace LegacyFlavour.Systems
{
    /// <summary>
    /// System to limit disk write frequency for config saving
    /// </summary>
    public class LegacyFlavourUpdateSystem : GameSystemBase
    {
        private bool hasConfigUpdatePending;
        private float nextConfigUpdate;

        private bool hasThemeConfigUpdatePending;
        private float nextThemeConfigUpdate;

        private bool hasZoneColourUpdatePending;
        private bool forceZoneInvalidateCache;
        private float nextZoneColourUpdate;

        private static LegacyFlavourConfig _config = ConfigBase.Load<LegacyFlavourConfig>( );
        private static ThemeConfig _themeConfig = ThemeConfig.Load( );

        public LegacyFlavourConfig Config
        {
            get
            {
                return _config;
            }
        }

        public ThemeConfig ThemeConfig
        {
            get
            {
                return _themeConfig;
            }
        }

        public ThemeGenerator ThemeGenerator
        {
            get
            {
                return _themeGenerator;
            }
        }

        private ZoneColourSystem _zoneColourSystem;
        private ThemeGenerator _themeGenerator;

        protected override void OnCreate( )
        {
            base.OnCreate( );
            _zoneColourSystem = World.GetOrCreateSystemManaged<ZoneColourSystem>( );
            _themeGenerator = new ThemeGenerator( this );
        }

        protected override void OnUpdate( )
        {
            if ( hasConfigUpdatePending && UnityEngine.Time.time >= nextConfigUpdate )
            {
                hasConfigUpdatePending = false;

                try
                {
                    _config.Save( );
                }
                catch ( Exception ex )
                {
                    // Do error logging
                }
            }

            if ( hasThemeConfigUpdatePending && UnityEngine.Time.time >= nextThemeConfigUpdate )
            {
                hasThemeConfigUpdatePending = false;

                try
                {
                    _themeGenerator.SetDefaultConfig( );
                    _themeConfig.Save( );
                    _themeGenerator.Export( );
                    _themeGenerator.Inject( );
                }
                catch ( Exception ex )
                {
                    // Do error logging
                }
            }

            if ( hasZoneColourUpdatePending && UnityEngine.Time.time >= nextZoneColourUpdate )
            {
                hasZoneColourUpdatePending = false;
                _zoneColourSystem.ForceUpdate( forceZoneInvalidateCache );
            }
        }

        /// <summary>
        /// Enqueue a config update
        /// </summary>
        /// <param name="immediate"></param>
        public void EnqueueConfigUpdate( bool immediate = false )
        {
            nextConfigUpdate = immediate ? 0f : UnityEngine.Time.time + 5f;
            hasConfigUpdatePending = true;
        }

        /// <summary>
        /// Enqueue a theme config update
        /// </summary>
        /// <param name="immediate"></param>
        public void EnqueueThemeConfigUpdate( bool immediate = false )
        {
            nextThemeConfigUpdate = immediate ? 0f : UnityEngine.Time.time + 1f;
            hasThemeConfigUpdatePending = true;
        }

        /// <summary>
        /// Enqueue a zone colours update
        /// </summary>
        /// <param name="immediate"></param>
        /// <param name="invalidateCache"></param>
        public void EnqueueColoursUpdate( bool immediate = false, bool invalidateCache = false )
        {
            nextZoneColourUpdate = immediate ? 0f : UnityEngine.Time.time + 0.2f;
            hasZoneColourUpdatePending = true;

            if ( invalidateCache )
                forceZoneInvalidateCache = true;
        }

        /// <summary>
        /// Reload the config file
        /// </summary>
        public void Reload( )
        {
            _config = ConfigBase.Load<LegacyFlavourConfig>( );
            _themeConfig = ThemeConfig.Load( );
            _themeGenerator.Export( );
            _themeGenerator.Inject( );
        }
    }
}
