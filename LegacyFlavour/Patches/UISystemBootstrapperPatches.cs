using Game;
using Game.UI;
using HarmonyLib;
using LegacyFlavour.Configuration.Themes;
using LegacyFlavour.Systems;
using Unity.Entities;

namespace LegacyFlavour.Patches
{
    [HarmonyPatch( typeof( UISystemBootstrapper ), "Awake" )]
    public static class UISystemBootstrapper_AwakePatch
    {
        static LegacyFlavourUpdateSystem _updateSystem;

        public static void Postfix( )
        {
            if ( _updateSystem == null )
                _updateSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LegacyFlavourUpdateSystem>( );

            if ( _updateSystem == null || _updateSystem.ThemeGenerator == null )
                return;

            _updateSystem.EnqueueThemeConfigUpdate( true );
        }
    }
}
