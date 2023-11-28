using Game.Prefabs;
using HarmonyLib;
using LegacyFlavour.Helpers;
using LegacyFlavour.Systems;
using Unity.Entities;

namespace LegacyFlavour.Patches
{
    /// <summary>
    /// Replaces an infoview prefab icon and caches it for updates
    /// </summary>
    [HarmonyPatch( typeof( InfoviewPrefab ), "Initialize" )]
    class InfoviewPrefab_ConstructorPatch
    {
        static ZoneColourSystem _zoneColourSystem;

        static void Prefix( InfoviewPrefab __instance )
        {
            LegacyFlavourSystem.EnsureModUIFolder( );

            if ( _zoneColourSystem == null )
            {
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LegacyFlavourSystem>( );
                _zoneColourSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ZoneColourSystem>( );
            }

            var url = __instance.m_IconPath;

            if ( _zoneColourSystem == null || !url.Contains( "Media/Game/Icons/" ) )
                return;

            if ( _zoneColourSystem.DynamicZoneIcons?.CheckForReplacement( url, out var parsedUrl ) == true )
            {
                __instance.m_IconPath = parsedUrl;
                DynamicZoneIcons._infoViewReplacements.Add( (__instance, url) );
            }
        }
    }
}
