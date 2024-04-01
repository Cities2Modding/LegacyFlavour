import React from 'react'
import $IconPanel from '../components/_icon-panel';

const $ZoneSettings = ({ locale, data, setData, triggerUpdate }) => {
    const react = window.$_gooee.react;
    const { Grid, Button, CheckBox, Slider, Dropdown } = window.$_gooee.framework;

    const [preset, setPreset] = react.useState('');

    const triggerResetZoneSettingsToDefault = () => {
        engine.trigger("cities2modding_legacyflavour.resetZoneSettingsToDefault");
    };

    const triggerSetZoneSettingsPreset = () => {
        engine.trigger("cities2modding_legacyflavour.setZoneSettingsPreset", preset);
    };

    const updateData = (field, val) => {
        if (field === "Enabled")
            setData({ ...data, Enabled: val });
        else if (field === "UseDynamicCellBorders")
            setData({ ...data, UseDynamicCellBorders: val });
        else if (field === "CellOpacity")
            setData({ ...data, CellOpacity: val });
        else if (field === "CellBorderOpacity")
            setData({ ...data, CellBorderOpacity: val });
        else if (field === "EmptyCellOpacity")
            setData({ ...data, EmptyCellOpacity: val });
        else if (field === "EmptyCellBorderOpacity")
            setData({ ...data, EmptyCellBorderOpacity: val });
        else if (field === "OverrideIcons")
            setData({ ...data, OverrideIcons: val });

        triggerUpdate(field, val);
    };

    const presets = [
        { label: locale["DEFAULT"], value: "" },
        { label: locale["CITY_PLANNER_SPECIAL"], value: "CityPlannerSpecial" },
    ];

    return <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
        <div style={{ flex: 1, width: '50%' }}>
            <div style={{ flex: 1, paddingRight: '5rem' }}>
                <$IconPanel label={locale["CUSTOM_ZONE_COLOURING"]}
                    description={locale["CUSTOM_ZONE_COLOURING_DESC"]}
                    icon="Media/Game/Icons/Zones.svg">
                    <CheckBox style={{ alignSelf: 'center', margin: '10rem' }} checked={data.Enabled} onToggle={(val) => updateData("Enabled", val)} />
                </$IconPanel>
                <$IconPanel label={locale["OVERRIDE_ZONE_ICONS"]}
                    description={locale["OVERRIDE_ZONE_ICONS_DESC"]}
                    icon="Media/Game/Icons/ZoneResidential.svg">
                    <CheckBox style={{ alignSelf: 'center', margin: '10rem' }} checked={data.OverrideIcons} onToggle={(val) => updateData("OverrideIcons", val)} />
                </$IconPanel>
                <$IconPanel label={locale["USE_DYNAMIC_CELL_BORDERS"]}
                    description={locale["USE_DYNAMIC_CELL_BORDERS_DESC"]}
                    icon="Media/Game/Climate/Snow.svg">
                    <CheckBox style={{ alignSelf: 'center', margin: '10rem' }} checked={data.UseDynamicCellBorders} onToggle={(val) => updateData("UseDynamicCellBorders", val)} />
                </$IconPanel>
                <$IconPanel label={locale["CELL_OPACITY"]}
                    description={locale["CELL_OPACITY_DESC"]}
                    icon="Media/Editor/Edit.svg" fitChild="true">
                    <div className="pl-4 pr-4 pb-4">
                        <Slider value={data.CellOpacity} onValueChanged={(val) => updateData("CellOpacity", val)} />
                    </div>
                </$IconPanel>
            </div>
        </div>
        <div style={{ flex: 1, width: '50%', paddingLeft: '5rem' }}> 
            <$IconPanel label={locale["CELL_BORDER_OPACITY"]}
                description={locale["CELL_BORDER_OPACITY_DESC"]}
                icon="Media/Editor/Edit.svg" fitChild="true">
                <div className="pl-4 pr-4 pb-4">
                    <Slider value={data.CellBorderOpacity} onValueChanged={(val) => updateData("CellBorderOpacity", val)} />
                </div>
            </$IconPanel>
            <$IconPanel label={locale["EMPTY_CELL_OPACITY"]}
                description={locale["EMPTY_CELL_OPACITY_DESC"]}
                icon="Media/Editor/Edit.svg" fitChild="true">
                <div className="pl-4 pr-4 pb-4">
                    <Slider value={data.EmptyCellOpacity} onValueChanged={(val) => updateData("EmptyCellOpacity", val)} />
                </div>
            </$IconPanel>
            <$IconPanel label={locale["EMPTY_CELL_BORDER_OPACITY"]}
                description={locale["EMPTY_CELL_BORDER_OPACITY_DESC"]}
                icon="Media/Editor/Edit.svg" fitChild="true">
                <div className="pl-4 pr-4 pb-4">
                    <Slider value={data.EmptyCellBorderOpacity} onValueChanged={(val) => updateData("EmptyCellBorderOpacity", val)} />
                </div>
            </$IconPanel>
            <div className="d-flex flex-row align-items-center">
                <Dropdown className="flex-1 mr-4" selected={preset} options={presets} onSelectionChanged={(val) => setPreset(val)}></Dropdown>
                <Button className="pl-6 pr-6" color="light" size="sm" style="trans" onClick={triggerSetZoneSettingsPreset}>{locale["SET"]}</Button>
            </div>
        </div>
    </div>
}

export default $ZoneSettings