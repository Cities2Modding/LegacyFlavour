import React from 'react';
import { useDataUpdate } from 'hookui-framework';
import $TabWindow from './components/_tab-window';
import $Settings from './tabs/_settings';
import $ZoneColours from './tabs/_zone-colours';
import $ZoneSettings from './tabs/_zone-settings';
import $About from './tabs/_about';

const $LegacyFlavour = ({ react }) => {

    const [data, setData] = react.useState({})

    useDataUpdate(react, "cities2modding_legacyflavour.config", setData)

    const triggerUpdate = (prop, val) => {
        engine.trigger("cities2modding_legacyflavour.updateProperty", JSON.stringify({ property: prop, value: val }) );
    };

    const toggleVisibility = () => {        
        const data = { type: "toggle_visibility", id: "cities2modding.legacyflavour" };
        const event = new CustomEvent('hookui', { detail: data });
        window.dispatchEvent(event);
    }

    const tabs = [
        {
            name: 'Settings',
            content: <div style={{ display: 'flex', width: '100%' }}>
                <$Settings react={react} data={data} triggerUpdate={triggerUpdate} />
            </div>
        },
        {
            name: 'Zone Settings',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$ZoneSettings react={react} data={data} triggerUpdate={triggerUpdate} />
            </div>
        },
        {
            name: 'Zone Colours',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$ZoneColours react={react} data={data} triggerUpdate={triggerUpdate} />
            </div>
        },
        {
            name: 'About',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$About />
            </div>
        }
    ];

    return <$TabWindow react={react} tabs={tabs} onClose={toggleVisibility} />
};

// Registering the panel with HookUI
window._$hookui.registerPanel({
    id: "cities2modding.legacyflavour",
    name: "Legacy Flavour",
    icon: "Media/Game/Icons/GenericVehicle.svg",
    component: $LegacyFlavour
});
