using Game;
using LegacyFlavour.Configuration;
using System;

namespace LegacyFlavour.Systems
{
    /// <summary>
    /// System to limit disk write frequency for config saving
    /// </summary>
    public partial class LegacyFlavourUpdateSystem : GameSystemBase
    {
        private bool hasConfigUpdatePending;
        private float nextConfigUpdate;

        private bool hasZoneColourUpdatePending;
        private bool forceZoneInvalidateCache;
        private float nextZoneColourUpdate;

        private static LegacyFlavourConfig _config = ConfigBase.Load<LegacyFlavourConfig>( );

        public LegacyFlavourConfig Config
        {
            get
            {
                return _config;
            }
        }

        private ZoneColourSystem _zoneColourSystem;

        protected override void OnCreate( )
        {
            base.OnCreate( );
            _zoneColourSystem = World.GetOrCreateSystemManaged<ZoneColourSystem>( );
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
                    UnityEngine.Debug.LogError( "Error writing config files!\n" + ex );
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
        }
    }
}
