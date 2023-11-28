using Game.Tools;
using HarmonyLib;
using LegacyFlavour.Systems;
using UnityEngine;

namespace LegacyFlavour.Patches
{
    /// <summary>
    /// Incorporate sticky whiteness support
    /// </summary>
    [HarmonyPatch( typeof( ToolSystem ), "UpdateInfoviewColors" )]
    class ToolSystem_UpdateInfoviewColorsPatch
    {
        static void Postfix( ToolSystem __instance )
        {
            var config = LegacyFlavourSystem.Config;

            if ( config.UseStickyWhiteness && __instance.activeInfoview?.active == true )
                Shader.SetGlobalInt( "colossal_InfoviewOn", config.WhitenessToggle ? 1 : 0 );
        }
    }
}
