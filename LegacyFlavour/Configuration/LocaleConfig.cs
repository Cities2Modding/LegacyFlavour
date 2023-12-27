using LegacyFlavour.UI;
using System.Collections.Generic;

namespace LegacyFlavour.Configuration
{
    public class LocaleConfig : ConfigBase
    {
        public List<LocaleGroup> Locales
        {
            get; 
            set; 
        }

        protected override string ConfigFileName => "locale.json";

        public readonly static LocaleConfig Default = LoadDefault<LocaleConfig>( );
    }

    public class LocaleGroup : ModelWriter
    {
        public List<string> IDs
        {
            get;
            set;
        }

        public Dictionary<string, string> Entries
        {
            get;
            set;
        }
    }
}
