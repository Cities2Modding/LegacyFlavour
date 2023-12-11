using Game.UI.Localization;
using Game.UI.Widgets;
using HarmonyLib;
using LegacyFlavour.Systems;
using System.Collections.Generic;
using Unity.Entities;

namespace LegacyFlavour.Patches
{
    [HarmonyPatch( typeof( Game.Settings.InterfaceSettings ), "GetInterfaceStyleValues" )]
    public static class InterfaceSettingsPatch
    {
        static LegacyFlavourUpdateSystem _updateSystem;
        static List<DropdownItem<string>> _dropdownItems = new List<DropdownItem<string>>( );
        public static HashSet<string> DEFAULT_THEMES = new HashSet<string>{ "default", "bright-blue", "dark-grey-orange" };
        
        static void Postfix( ref DropdownItem<string>[] __result )
        {
            _updateSystem ??= World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystemManaged<LegacyFlavourUpdateSystem>( );

            if ( _updateSystem.ThemeConfig == null || _updateSystem.ThemeGenerator == null )
                return;

            var themes = _updateSystem.ThemeGenerator.GetThemeNames( );

            if ( themes?.Count > 0 )
            {
                // Only reintialise it if the count changes
                if ( _dropdownItems.Count != __result.Length + themes.Count )
                {
                    if ( _dropdownItems.Count > 0 ) // Ensures previous list is cleared
                        _dropdownItems.Clear( );

                    _dropdownItems = new List<DropdownItem<string>>( __result );

                    foreach ( var theme in themes )
                    {
                        var styleKey = theme.ToLower( ).Replace( ' ', '-' );

                        if ( DEFAULT_THEMES.Contains( styleKey ) )
                            continue;

                        _dropdownItems.Add( new DropdownItem<string>
                        {
                            value = styleKey,
                            displayName = LocalizedString.Value( theme )
                        } );
                    }
                }

                // Convert the list back to an array and set it as the result
                __result = _dropdownItems.ToArray( );
            }
        }
    }
}
