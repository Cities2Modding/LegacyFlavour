import React from 'react'
import $IconPanel from '../components/_icon-panel';
import $CheckBox from '../components/_checkbox';
import $Slider from '../components/_slider';
import $Button from '../components/_button';
import $Select from '../components/_select';

const $ZoneSettings = ({ react, locale, data, setData, triggerUpdate }) => {
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
                    <$CheckBox react={react} style={{ alignSelf: 'center', margin: '10rem' }} checked={data.Enabled} onToggle={(val) => updateData("Enabled", val)} />
                </$IconPanel>
                <$IconPanel label={locale["OVERRIDE_ZONE_ICONS"]}
                    description={locale["OVERRIDE_ZONE_ICONS_DESC"]}
                    icon="Media/Game/Icons/ZoneResidential.svg">
                    <$CheckBox react={react} style={{ alignSelf: 'center', margin: '10rem' }} checked={data.OverrideIcons} onToggle={(val) => updateData("OverrideIcons", val)} />
                </$IconPanel>
                <$IconPanel label={locale["USE_DYNAMIC_CELL_BORDERS"]}
                    description={locale["USE_DYNAMIC_CELL_BORDERS_DESC"]}
                    icon="Media/Game/Climate/Snow.svg">
                    <$CheckBox react={react} style={{ alignSelf: 'center', margin: '10rem' }} checked={data.UseDynamicCellBorders} onToggle={(val) => updateData("UseDynamicCellBorders", val)} />
                </$IconPanel>
                <$IconPanel label={locale["CELL_OPACITY"]}
                    description={locale["CELL_OPACITY_DESC"]}
                    icon="Media/Editor/Edit.svg" fitChild="true">
                    <$Slider react={react} value={data.CellOpacity} onValueChanged={(val) => updateData("CellOpacity", val)} />
                </$IconPanel>
            </div>
        </div>
        <div style={{ flex: 1, width: '50%', paddingLeft: '5rem' }}> 
            <$IconPanel label={locale["CELL_BORDER_OPACITY"]}
                description={locale["CELL_BORDER_OPACITY_DESC"]}
                icon="Media/Editor/Edit.svg" fitChild="true">
                <$Slider react={react} value={data.CellBorderOpacity} onValueChanged={(val) => updateData("CellBorderOpacity", val)} />
            </$IconPanel>
            <$IconPanel label={locale["EMPTY_CELL_OPACITY"]}
                description={locale["EMPTY_CELL_OPACITY_DESC"]}
                icon="Media/Editor/Edit.svg" fitChild="true">
                <$Slider react={react} value={data.EmptyCellOpacity} onValueChanged={(val) => updateData("EmptyCellOpacity", val)} />
            </$IconPanel>
            <$IconPanel label={locale["EMPTY_CELL_BORDER_OPACITY"]}
                description={locale["EMPTY_CELL_BORDER_OPACITY_DESC"]}
                icon="Media/Editor/Edit.svg" fitChild="true">
                <$Slider react={react} value={data.EmptyCellBorderOpacity} onValueChanged={(val) => updateData("EmptyCellBorderOpacity", val)} />
            </$IconPanel>
            <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
                <$Select react={react} selected={preset} containerStyle={{ flex: 1, width: 'auto', marginRight: '10rem', }} options={presets} onSelectionChanged={(val) => setPreset(val)}></$Select>
                <$Button style={{ paddingLeft: '30rem', paddingRight: '30rem' }} onClick={triggerSetZoneSettingsPreset}>{locale["SET"]}</$Button>
            </div>
        </div>
    </div>
}

export default $ZoneSettings