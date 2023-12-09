using Game.Tools;
using HarmonyLib;
using LegacyFlavour.Configuration;
using LegacyFlavour.Systems;
using Unity.Entities;
using UnityEngine;

namespace LegacyFlavour.Patches
{
    /// <summary>
    /// Incorporate sticky whiteness support
    /// </summary>
    [HarmonyPatch( typeof( ToolSystem ), "UpdateInfoviewColors" )]
    class ToolSystem_UpdateInfoviewColorsPatch
    {
        static LegacyFlavourUpdateSystem _updateSystem;

        static void Postfix( ToolSystem __instance )
        {
            if ( _updateSystem == null )
                _updateSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LegacyFlavourUpdateSystem>( );

            var config = _updateSystem.Config;

            if ( config.UseStickyWhiteness && __instance.activeInfoview?.active == true )
                Shader.SetGlobalInt( "colossal_InfoviewOn", config.WhitenessToggle ? 1 : 0 );
        }
    }
}
