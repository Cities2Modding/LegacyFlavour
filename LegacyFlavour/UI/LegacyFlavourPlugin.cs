using Gooee.Plugins.Attributes;
using Gooee.Plugins;

namespace LegacyFlavour.UI
{
    [ControllerTypes( typeof( LegacyFlavourController ) )]
    [PluginToolbar( typeof( LegacyFlavourController ), "OnToggleVisible", "Legacy Flavour", "Media/Game/Icons/GenericVehicle.svg" )]
    public class LegacyFlavourPlugin : IGooeePluginWithControllers
    {
        public string Name => "LegacyFlavour";
        public string Version => "1.1.2";
        public string ScriptResource => "LegacyFlavour.Resources.ui.js";

        public IController[] Controllers
        {
            get;
            set;
        }
    }
}
