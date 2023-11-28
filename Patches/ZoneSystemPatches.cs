using Game.Prefabs;
using HarmonyLib;
using LegacyFlavour.Systems;

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

        static void Postfix( ZoneSystem __instance )
        {
            if ( _zoneColourSystem == null )
                _zoneColourSystem = __instance.World.GetOrCreateSystemManaged<ZoneColourSystem>( );            
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
