import React from 'react';
import { useDataUpdate } from 'hookui-framework';
import $TabWindow from './components/_tab-window';
import $Settings from './tabs/_settings';
import $ZoneColours from './tabs/_zone-colours';
import $ZoneSettings from './tabs/_zone-settings';
import $About from './tabs/_about';

const $LegacyFlavour = ({ react }) => {
    const [data, setData] = react.useState({});
    const [localeData, setLocaleData] = react.useState({});
    const [opacity, setOpacity] = react.useState(1);
    const [useTransparency, setUseTransparency] = react.useState(false);
    const [tabs, setTabs] = react.useState([]);

    useDataUpdate(react, "cities2modding_legacyflavour.config", setData);
    useDataUpdate(react, "cities2modding_legacyflavour.currentLocale", setLocaleData);
    
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

    react.useEffect(() => {
        if (typeof localeData.Entries !== "undefined") {
            setTabs([
                {
                    name: "SETTINGS",
                    label: localeData.Entries["SETTINGS"],
                    content: <div style={{ display: 'flex', width: '100%' }}>
                        <$Settings react={react} locale={localeData.Entries} data={data} setData={setData} triggerUpdate={triggerUpdate} />
                    </div>
                },
                {
                    name: "ZONE_SETTINGS",
                    label: localeData.Entries["ZONE_SETTINGS"],
                    content: <div style={{ height: '100%', width: '100%' }}>
                        <$ZoneSettings react={react} locale={localeData.Entries} data={data} setData={setData} triggerUpdate={triggerUpdate} />
                    </div>
                },
                {
                    name: "ZONE_COLOURS",
                    label: localeData.Entries["ZONE_COLOURS"],
                    content: <div style={{ height: '100%', width: '100%' }}>
                        <$ZoneColours react={react} locale={localeData.Entries} data={data} setData={setData} triggerUpdate={triggerUpdate} useTransparency={useTransparency} onChangeUseTransparency={onChangeUseTransparency} onChangeWindowOpacity={onChangeOpacity} />
                    </div>
                },
                {
                    name: "ABOUT",
                    label: localeData.Entries["ABOUT"],
                    content: <div style={{ height: '100%', width: '100%' }}>
                        <$About react={react} locale={localeData.Entries} />
                    </div>
                }
            ]);
        }
    }, [localeData, data]);

    const title = localeData.Entries ? `Legacy Flavour (${localeData.Entries["LEGACY_FLAVOUR"]})` : "Legacy Flavour";
    return <$TabWindow react={react} title={title} tabs={tabs} onClose={toggleVisibility} style={{ opacity: opacity }} />
};

// Registering the panel with HookUI
window._$hookui.registerPanel({
    id: "cities2modding.legacyflavour",
    name: "Legacy Flavour",
    icon: "Media/Game/Icons/GenericVehicle.svg",
    component: $LegacyFlavour
});
