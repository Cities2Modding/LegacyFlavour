using Game.Prefabs;
using Game.Simulation;
using HarmonyLib;
using LegacyFlavour.Systems;
using UnityEngine;

namespace LegacyFlavour.Patches
{
    [HarmonyPatch( typeof( ZoneSystem ), "OnCreate" )]
    class ZoneSystem_OnCreatePatch
    {
        static void Postfix( ZoneSystem __instance )
        {
            __instance.World.GetOrCreateSystemManaged<ZoneColourSystem>();
        }
    }

    [HarmonyPatch( typeof( ZoneSystem ), "OnUpdate" )]
    class ZoneSystem_UpdatePatch
    {
        static ZoneColourSystem _zoneColourSystem;
        static ClimateSystem _climateSystem;
        public static bool isSnowing = false;
        static float lastCheckTime;

        static void Postfix( ZoneSystem __instance )
        {
            if ( _zoneColourSystem == null )
                _zoneColourSystem = __instance.World.GetOrCreateSystemManaged<ZoneColourSystem>( );

            if ( Time.time < lastCheckTime + 1f )
                return;

            if ( _climateSystem == null )
                _climateSystem = __instance.World.GetOrCreateSystemManaged<ClimateSystem>( );

            if ( _climateSystem == null )
                return;

            if ( _climateSystem.isSnowing && !isSnowing )
            {
                isSnowing = true;
                _zoneColourSystem.ForceUpdate( );
            }
            else if ( !_climateSystem.isSnowing && isSnowing )
            {
                isSnowing = false;
                _zoneColourSystem.ForceUpdate( );
            }

            lastCheckTime = Time.time;
        }
    }

    [HarmonyPatch( typeof( ZoneSystem ), "UpdateZoneColors", typeof( ZonePrefab ), typeof( ZoneData ) )]
    class ZoneSystem_UpdateZoneColorsPatch
    {
        static ZoneColourSystem _zoneColourSystem;

        static bool Prefix( ZoneSystem __instance, ZonePrefab zonePrefab, ZoneData zoneData )
        {
            if ( _zoneColourSystem == null )
                _zoneColourSystem = __instance.World.GetOrCreateSystemManaged<ZoneColourSystem>( );

            return _zoneColourSystem.UpdateZones( zonePrefab, zoneData ); // Skip original function if returns false
        }
    }
}
