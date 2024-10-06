using Game.Prefabs;
using HarmonyLib;
using LegacyFlavour.Helpers;
using LegacyFlavour.Systems;
using Unity.Entities;

namespace LegacyFlavour.Patches
{
    /// <summary>
    /// Replaces a ui object prefab icon and caches it for updates
    /// </summary>
    [HarmonyPatch( typeof( UIObject ), "LateInitialize" )]
    class UIObject_ConstructorPatch
    {
        static ZoneColourSystem _zoneColourSystem;

        static void Postfix( UIObject __instance )
        {
            LegacyFlavourSystem.EnsureModUIFolder( );

            if ( _zoneColourSystem == null )
            {
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LegacyFlavourSystem>( );
                _zoneColourSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ZoneColourSystem>( );
            }

            var url = __instance.m_Icon;

            if ( _zoneColourSystem == null || string.IsNullOrEmpty(url) || !url.Contains( "Media/Game/Icons/" ) )
                return;

            if ( _zoneColourSystem.DynamicZoneIcons?.CheckForReplacement( url, out var parsedUrl ) == true )
            {
                __instance.m_Icon = parsedUrl;
                DynamicZoneIcons._replacements.Add( (__instance, url) );
            }
        }
    }
}
