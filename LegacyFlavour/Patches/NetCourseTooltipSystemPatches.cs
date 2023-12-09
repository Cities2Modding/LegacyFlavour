using Game.UI.Localization;
using Game.UI.Tooltip;
using HarmonyLib;
using LegacyFlavour.Configuration;
using LegacyFlavour.Systems;
using Unity.Entities;

namespace LegacyFlavour.Patches
{
    // The user must have their unit type set to Metric currently,
    // improvements to handle the various settings and do the relevant
    // conversion would be a good improvement to make.

    /// <summary>
    /// Change length display to Units like Cities Skylines 1
    /// </summary>
    [HarmonyPatch( typeof( NetCourseTooltipSystem ), "OnUpdate" )]
    class NetCourseTooltipSystem_OnUpdatePatch
    {
        static LegacyFlavourUpdateSystem _updateSystem;

        static void Postfix( NetCourseTooltipSystem __instance )
        {
            if ( _updateSystem == null )
                _updateSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LegacyFlavourUpdateSystem>( );

            var m_Length = Traverse.Create( __instance ).Field( "m_Length" ).GetValue<FloatTooltip>( );
            if ( m_Length != null )
            {
                if ( _updateSystem.Config.UseUnits )
                {
                    m_Length.value /= 8f; // Convert to Cities 1 units
                    m_Length.unit = "floatTwoFractions"; // Change to a generic unit type to stop showing m/ft
                    m_Length.label = LocalizedString.Value( "U" ); // Adjust the label to say 'U'
                }
                else
                {
                    m_Length.unit = "length";
                    m_Length.label = default;
                }
            }
        }
    }
}
