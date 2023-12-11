using System.Collections.Generic;

namespace LegacyFlavour.Configuration.Themes
{
    public class ThemeEntry
    {
        public string Name
        {
            get;
            set;
        }

        public List<ThemeSetting> Settings
        {
            get;
            set;
        }
    }
}
