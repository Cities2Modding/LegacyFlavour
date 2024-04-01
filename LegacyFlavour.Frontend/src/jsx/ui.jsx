import React from 'react';
import { useDataUpdate } from 'hookui-framework';
import $TabWindow from './components/_tab-window';
import $Settings from './tabs/_settings';
import $ZoneColours from './tabs/_zone-colours';
import $ZoneSettings from './tabs/_zone-settings';
import $About from './tabs/_about';
import ToolWindow from "./windows/_tool-window";

const LegacyFlavourWindow = ({ react, setupController }) => {
    const { Button, Icon, TabModal } = window.$_gooee.framework;
    const { model, update, trigger, _L } = setupController();

    const [data, setData] = react.useState({});
    const [localeData, setLocaleData] = react.useState({});
    const [opacity, setOpacity] = react.useState(1);
    const [useTransparency, setUseTransparency] = react.useState(false);
    const [tabs, setTabs] = react.useState([]);

    useDataUpdate(react, "cities2modding_legacyflavour.config", setData);
    useDataUpdate(react, "cities2modding_legacyflavour.currentLocale", setLocaleData);

    const triggerUpdate = (prop, val) => {
        engine.trigger("cities2modding_legacyflavour.updateProperty", JSON.stringify({ property: prop, value: val }));
    };


    const closeModal = () => {
        trigger("OnToggleVisible");
        engine.trigger("audio.playSound", "close-panel", 1);
    };

    const onChangeOpacity = (val) => {
        if (!useTransparency) {
            setOpacity(1);
            return;
        }
        setOpacity(val);
    };

    const onChangeUseTransparency = (val) => {
        setUseTransparency(val);
        onChangeOpacity();
    };

    react.useEffect(() => {
        if (typeof localeData.Entries !== "undefined") {
            setTabs([
                {
                    name: "SETTINGS",
                    label: localeData.Entries["SETTINGS"],
                    content: <$Settings locale={localeData.Entries} data={data} setData={setData} triggerUpdate={triggerUpdate} />
                    
                },
                {
                    name: "ZONE_SETTINGS",
                    label: localeData.Entries["ZONE_SETTINGS"],
                    content: <$ZoneSettings locale={localeData.Entries} data={data} setData={setData} triggerUpdate={triggerUpdate} />                    
                },
                {
                    name: "ZONE_COLOURS",
                    label: localeData.Entries["ZONE_COLOURS"],
                    content: <$ZoneColours react={react} locale={localeData.Entries} data={data} setData={setData} triggerUpdate={triggerUpdate} useTransparency={useTransparency} onChangeUseTransparency={onChangeUseTransparency} onChangeWindowOpacity={onChangeOpacity} />
                },
                {
                    name: "ABOUT",
                    label: localeData.Entries["ABOUT"],
                    content: <$About react={react} locale={localeData.Entries} />
                }
            ]);
        }
    }, [localeData, data]);

    const title = localeData.Entries ? `Legacy Flavour (${localeData.Entries["LEGACY_FLAVOUR"]})` : "Legacy Flavour";
    return <>
       /* <ToolWindow model={model} trigger={trigger} update={update} _L={_L} />*/
        {model.IsVisible ? <TabModal fixed size="lg" title={title} tabs={tabs} onClose={closeModal} style={{ opacity: opacity }} /> : null}
    </>;
};

window.$_gooee.register("legacyflavour", "LegacyFlavourWindow", LegacyFlavourWindow, "main-container", "legacyflavour");