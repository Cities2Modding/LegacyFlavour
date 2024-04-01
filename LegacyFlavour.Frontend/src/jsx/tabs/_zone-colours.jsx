import React from 'react'
import $IconPanel from '../components/_icon-panel';
import $ColorPicker from '../components/_colorpicker';
import $Button from '../components/_button';

const $ZoneColours = ({ locale, data, setData, triggerUpdate, useTransparency, onChangeUseTransparency, onChangeWindowOpacity }) => {
    const { Grid, Button, CheckBox, FormCheckBox, Slider, Dropdown } = window.$_gooee.framework;

    const react = window.$_gooee.react;
    const colourModes = [
        { label: locale["DEFAULT_COLOURS"], value: "None" },
        { label: locale["DEUTERANOPIA"], value: "Deuteranopia" },
        { label: locale["PROTANOPIA"], value: "Protanopia" },
        { label: locale["TRITANOPIA"], value: "Tritanopia" },
        { label: locale["CUSTOM"], value: "Custom" }
    ];

    const updateData = (field, val) => {
        if (field === "Mode") {
            setData({ ...data, Mode: val });
        }
        triggerUpdate(field, val);
    };

    const onModeChanged = (selected) => {
        updateData("Mode", selected);
    };

    const triggerZoneColourUpdate = (zoneName, colour) => {
        engine.trigger("cities2modding_legacyflavour.updateZoneColour", zoneName, colour);
    };

    let zoneGroups = [
        { name: "Residential", label: locale["RESIDENTIAL"], icon: "ZoneResidential", desc: locale["RESIDENTIAL_DESC"], items: [] },
        { name: "Commercial", label: locale["COMMERCIAL"], icon: "ZoneCommercial", desc: locale["COMMERCIAL_DESC"], items: [] },
        { name: "Office", label: locale["OFFICE"], icon: "ZoneOffice", desc: locale["OFFICE_DESC"], items: [] },
        { name: "Industrial", label: locale["INDUSTRIAL"], icon: "ZoneIndustrial", desc: locale["INDUSTRIAL_DESC"], items: [] }
    ];

    if (data.Zones) {
        data.Zones.map((zone, index) => {
            var name = zone.Name;

            for (var i = 0; i < zoneGroups.length; i++) {
                if (name.startsWith(zoneGroups[i].name)) {
                    zoneGroups[i].items.push(zone);
                    break;
                }
            }
        });
    }

    const changeWindowOpacity = (visible) => {
        if (!onChangeWindowOpacity)
            return;
        onChangeWindowOpacity(visible ? 1 : 0.55);
    };

    const onColourDropdown = (visible) => {
        changeWindowOpacity(!visible);
    };

    const getZoneColours = (zoneGroup) => {
        let icon = "coui://legacyflavourui/Icons/" + data.IconsID + "/" + zoneGroup.icon + "_" + data.Mode + ".svg"; // Cache busting via querystring causes flickers so meh! We need a game restart for icon changes

        return (<$IconPanel key={zoneGroup.name} label={zoneGroup.label} style={{ flex: 1 }}
            description={zoneGroup.desc}
            icon={icon} fitChild="true">
            <div className="d-flex flex-column">
                {
                    zoneGroup.items.map((zone, index) => {
                        let colour = zone.Colour;

                        if (data.Mode == "Deuteranopia" && zone.Deuteranopia != "default")
                            colour = zone.Deuteranopia;
                        else if (data.Mode == "Protanopia" && zone.Protanopia != "default")
                            colour = zone.Protanopia;
                        else if (data.Mode == "Tritanopia" && zone.Tritanopia != "default")
                            colour = zone.Tritanopia;
                        else if (data.Mode == "Custom" && zone.Custom != "default")
                            colour = zone.Custom;

                        const onChanged = (newColour) => {
                            triggerZoneColourUpdate(zone.Name, newColour);
                        };

                        const zoneLabel = locale[zone.Name.replace(/ /g, "_").toUpperCase()];
                        return (<$ColorPicker key={zone.Name} react={react} label={zoneLabel} color={colour} onChanged={onChanged} onDropdown={onColourDropdown} />);
                    })
                }
            </div>
        </$IconPanel>)
    };

    const renderZoneColours = (index) => {
        return getZoneColours(zoneGroups[index]);
    };

    const triggerRegenerateIcons = () => {
        engine.trigger("cities2modding_legacyflavour.regenerateIcons");
    };

    const triggerSetColoursToVanilla = () => {
        engine.trigger("cities2modding_legacyflavour.setColoursToVanilla");
    };

    const triggerResetColoursToDefault = () => {
        engine.trigger("cities2modding_legacyflavour.resetColoursToDefault");
    };

    const modeString = data.Mode;

    return <div>
        <div style={{ display: 'flex', flexDirection: 'row' }}>
            <div style={{ width: '66.666666666%', paddingRight: '5rem' }}>
                <$IconPanel label={locale["COLOUR_BLINDNESS_MODE"]}
                    description={locale["COLOUR_BLINDNESS_MODE_DESC"]}
                    icon="Media/Editor/Edit.svg" fitChild="true">
                    <div className="flex-1 p-4">
                        <Dropdown selected={modeString} options={colourModes} onSelectionChanged={onModeChanged}></Dropdown>
                    </div>
                </$IconPanel>
            </div>
            <div style={{ width: '33.33333333333%' }}>
                <$Button onClick={triggerRegenerateIcons}>{locale["REGENERATE_ICONS"]}</$Button>
                <$Button className="mt-2" onClick={triggerSetColoursToVanilla}>{locale["SET_TO_VANILLA_COLOURS"]}</$Button>
                <$Button className="mt-2" onClick={triggerResetColoursToDefault}>{locale["RESET"]} {modeString}</$Button>
                {/*<FormCheckBox className="mt-4 mb-4" label={locale["MAKE_WINDOW_TRANSPARENT"]} checked={useTransparency} onToggle={onChangeUseTransparency} />*/}
            </div>
        </div>
        <Grid className="align-items-center justify-content-center" auto>
            <div>
                {renderZoneColours(0)}
            </div>
            <div>
                {renderZoneColours(1)}
                {renderZoneColours(2)}
            </div>
            <div>
                {renderZoneColours(3)}
            </div>
        </Grid>
    </div>
}

export default $ZoneColours