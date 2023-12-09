using Game.Common;
using Game;
using HarmonyLib;
using LegacyFlavour.Systems;
using LegacyFlavour.Configuration;

namespace LegacyFlavour.Patches
{
    [HarmonyPatch( typeof( SystemOrder ) )]
    public static class SystemOrderPatch
    {
        [HarmonyPatch( "Initialize" )]
        [HarmonyPostfix]
        public static void Postfix( UpdateSystem updateSystem )
        {            
            updateSystem.UpdateAt<LegacyFlavourUpdateSystem>( SystemUpdatePhase.MainLoop );
            updateSystem.UpdateAt<LegacyFlavourUISystem>( SystemUpdatePhase.UIUpdate );
            updateSystem.UpdateAt<LegacyFlavourSystem>( SystemUpdatePhase.Rendering );
        }
    }
}
