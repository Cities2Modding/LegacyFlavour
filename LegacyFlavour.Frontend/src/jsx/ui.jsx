import React from 'react';
import { useDataUpdate } from 'hookui-framework';
import $TabWindow from './components/_tab-window';
import $Settings from './tabs/_settings';
import $ZoneColours from './tabs/_zone-colours';
import $ZoneSettings from './tabs/_zone-settings';
import $UIThemes from './tabs/_ui-themes';
import $About from './tabs/_about';

const $LegacyFlavour = ({ react }) => {

    const [data, setData] = react.useState({})
    const [defaultThemeData, setDefaultThemeData] = react.useState({})
    const [themeData, setThemeData] = react.useState({})
    const [opacity, setOpacity] = react.useState(1)
    const [useTransparency, setUseTransparency] = react.useState(false)

    useDataUpdate(react, "cities2modding_legacyflavour.config", setData)
    useDataUpdate(react, "cities2modding_legacyflavour.themeConfig", setThemeData)
    useDataUpdate(react, "cities2modding_legacyflavour.defaultThemeData", setDefaultThemeData)

    //defaultThemes
    const triggerUpdate = (prop, val) => {
        engine.trigger("cities2modding_legacyflavour.updateProperty", JSON.stringify({ property: prop, value: val }) );
    };

    const toggleVisibility = () => {        
        const visData = { type: "toggle_visibility", id: "cities2modding.legacyflavour" };
        const event = new CustomEvent('hookui', { detail: visData });
        window.dispatchEvent(event);

        engine.trigger("audio.playSound", "close-panel", 1);
    }

    const onChangeOpacity = (val) => {
        if (!useTransparency) {
            setOpacity(1);
            return;
        }
        setOpacity(val);
    };

    const onChangeUseTransparency = ( val ) => {
        setUseTransparency(val);
        onChangeOpacity();
    };

    const tabs = [
        {
            name: 'Settings',
            content: <div style={{ display: 'flex', width: '100%' }}>
                <$Settings react={react} data={data} setData={setData} triggerUpdate={triggerUpdate} />
            </div>
        },
        {
            name: 'Zone Settings',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$ZoneSettings react={react} data={data} setData={setData} triggerUpdate={triggerUpdate} />
            </div>
        },
        {
            name: 'Zone Colours',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$ZoneColours react={react} data={data} setData={setData} triggerUpdate={triggerUpdate} useTransparency={useTransparency} onChangeUseTransparency={onChangeUseTransparency} onChangeWindowOpacity={onChangeOpacity} />
            </div>
        },
        {
            name: 'UI Themes',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$UIThemes react={react} themeData={themeData} setThemeData={setThemeData} defaultThemeData={defaultThemeData} />
            </div>
        },
        {
            name: 'About',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$About />
            </div>
        }
    ];

    return <$TabWindow react={react} tabs={tabs} onClose={toggleVisibility} style={{ opacity: opacity }} />
};

// Registering the panel with HookUI
window._$hookui.registerPanel({
    id: "cities2modding.legacyflavour",
    name: "Legacy Flavour",
    icon: "Media/Game/Icons/GenericVehicle.svg",
    component: $LegacyFlavour
});
