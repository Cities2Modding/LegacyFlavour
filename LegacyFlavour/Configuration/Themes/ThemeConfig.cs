using LegacyFlavour.Helpers;
using System.Collections.Generic;
using static UnityEngine.InputSystem.Layouts.InputControlLayout;

namespace LegacyFlavour.Configuration.Themes
{
    /// <summary>
    /// Adds options for more UI themes
    /// </summary>
    public class ThemeConfig : ConfigBase
    {
        public List<ThemeEntry> Themes
        {
            get;
            set;
        }

        public readonly static ThemeConfig Default = LoadDefault<ThemeConfig>( );
        protected override string ConfigFileName => "themes.json";

        /// <summary>
        /// Load the config file
        /// </summary>
        /// <returns></returns>
        public static ThemeConfig Load( )
        {
            return Load<ThemeConfig>( useDefaultAsTemplate: false );
        }

        /// <summary>
        /// Save the theme config and export the CSS
        /// </summary>
        public override void Save( )
        {
            base.Save( );
        }
    }
}