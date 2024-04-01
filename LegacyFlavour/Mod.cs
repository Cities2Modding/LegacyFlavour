using HarmonyLib;
using System;
using Game.Modding;
using Game;
using Colossal.Logging;
using Unity.Entities;
using LegacyFlavour.Systems;

namespace LegacyFlavour
{
    public class Mod : IMod
    {
        private static ILog _log = LogManager.GetLogger( "Cities2Modding" ).SetShowsErrorsInUI( false );

        private Harmony _harmony;
        private World _world;

        public void OnLoad( UpdateSystem updateSystem )
        {
            _world = updateSystem.World;

            updateSystem.UpdateAt<LegacyFlavourUpdateSystem>( SystemUpdatePhase.MainLoop );
            updateSystem.UpdateAt<LegacyFlavourSystem>( SystemUpdatePhase.Rendering );
            _world.GetOrCreateSystemManaged<ZoneColourSystem>( );

            _harmony = new Harmony( "cities2modding_legacyflavour" );
            _harmony.PatchAll( );
            
            _log.Info( Environment.NewLine + @"      :::        :::::::::: ::::::::      :::      ::::::::  :::   :::         
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
        }

        private void SafelyRemove<T>( )
            where T : GameSystemBase
        {
            var system = _world?.GetExistingSystemManaged<T>( );

            if ( system != null )
                _world.DestroySystemManaged( system );
        }

        public void OnDispose( )
        {
            SafelyRemove<LegacyFlavourSystem>( );
            SafelyRemove<LegacyFlavourUpdateSystem>( );
            SafelyRemove<ZoneColourSystem>( );

            _harmony.UnpatchAll( "cities2modding_legacyflavour" );
        }
    }
}
