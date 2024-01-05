using LegacyFlavour.Configuration.Themes;
using System;
using System.Collections.Generic;

namespace LegacyFlavour.Configuration
{
    public class LegacyFlavourConfig : ConfigBase
    {
        protected override string ConfigFileName => "config.json";

        public bool Enabled
        {
            get;
            set;
        }

        public bool UseStickyWhiteness
        {
            get;
            set;
        } = false;

        public bool WhitenessToggle
        {
            get;
            set;
        }

        public bool UseUnits
        {
            get;
            set;
        } = true;

        public bool UseDynamicCellBorders
        {
            get;
            set;
        } = true;

        public decimal CellOpacity
        {
            get;
            set;
        } = 0.9m;

        public decimal CellBorderOpacity
        {
            get;
            set;
        } = 0.9m;

        public decimal EmptyCellOpacity
        {
            get;
            set;
        } = 0m;

        public decimal EmptyCellBorderOpacity
        {
            get;
            set;
        } = 0.2m;

        public ColourBlindness Mode
        {
            get;
            set;
        } = ColourBlindness.None;

        public TimeOfDayOverride TimeOfDay
        {
            get;
            set;
        } = TimeOfDayOverride.Off;

        public bool FreezeVisualTime
        {
            get;
            set;
        }

        public WeatherOverride Weather
        {
            get;
            set;
        } = WeatherOverride.Off;

        public List<ZoneColourConfig> Zones
        {
            get;
            set;
        }

        public bool OverrideIcons
        {
            get;
            set;
        } = true;

        public string IconsID
        {
            get;
            set;
        }

        public readonly static LegacyFlavourConfig Default = LoadDefault<LegacyFlavourConfig>( );
    }

    public class ZoneColourConfig
    {
        public string Name
        {
            get;
            set;
        }

        public string Colour
        {
            get;
            set;
        }

        public string Deuteranopia
        {
            get;
            set;
        }

        public string Protanopia
        {
            get;
            set;
        }

        public string Tritanopia
        {
            get;
            set;
        }

        public string Custom
        {
            get;
            set;
        }
    }
}
