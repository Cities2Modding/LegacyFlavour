using BepInEx;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using LegacyFlavour.Patches;
using HookUILib.Core;
using Colossal.Localization;
using Game.SceneFlow;
using UnityEngine;
using System;

#if BEPINEX_V6
    using BepInEx.Unity.Mono;
#endif

namespace LegacyFlavour
{
    [BepInPlugin( MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION )]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake( )
        {
            var harmony = Harmony.CreateAndPatchAll( Assembly.GetExecutingAssembly( ), MyPluginInfo.PLUGIN_GUID + "_Cities2Harmony" );

            var patchedMethods = harmony.GetPatchedMethods( ).ToArray( );

            Logger.LogInfo( Environment.NewLine + @"      :::        :::::::::: ::::::::      :::      ::::::::  :::   :::         
      :+:        :+:       :+:    :+:   :+: :+:   :+:    :+: :+:   :+:         
      +:+        +:+       +:+         +:+   +:+  +:+         +:+ +:+          
      +#+        +#++:++#  :#:        +#++:++#++: +#+          +#++:           
      +#+        +#+       +#+   +#+# +#+     +#+ +#+           +#+            
      #+#        #+#       #+#    #+# #+#     #+# #+#    #+#    #+#            
      ########## ########## ########  ###     ###  ########     ###            
:::::::::: :::            :::     :::     :::  ::::::::  :::    ::: :::::::::  
:+:        :+:          :+: :+:   :+:     :+: :+:    :+: :+:    :+: :+:    :+: 
+:+        +:+         +:+   +:+  +:+     +:+ +:+    +:+ +:+    +:+ +:+    +:+ 
:#::+::#   +#+        +#++:++#++: +#+     +:+ +#+    +:+ +#+    +:+ +#++:++#:  
+#+        +#+        +#+     +#+  +#+   +#+  +#+    +#+ +#+    +#+ +#+    +#+ 
#+#        #+#        #+#     #+#   #+#+#+#   #+#    #+# #+#    #+# #+#    #+# 
###        ########## ###     ###     ###      ########   ########  ###    ### " );

            // Plugin startup logic
            Logger.LogDebug( $"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded! Patched methods: " + patchedMethods.Length );

            foreach ( var patchedMethod in patchedMethods )
            {
                Logger.LogDebug( $"Patched method: {patchedMethod.Module.Name}:{patchedMethod.Name}" );
            }
        }
    }

    public class LegacyFlavourUI : UIExtension
    {
        public new readonly ExtensionType extensionType = ExtensionType.Panel;
        public new readonly string extensionID = "cities2modding.legacyflavour";
        public new readonly string extensionContent;

        public LegacyFlavourUI()
        {
            extensionContent = LoadEmbeddedResource( "LegacyFlavour.Resources.ui.js" );
        }
    }
}
