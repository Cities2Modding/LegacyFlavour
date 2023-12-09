import React from 'react'
import $IconPanel from '../components/_icon-panel';
import $CheckBox from '../components/_checkbox';
import $ToggleGroup from '../components/_toggle-group';

const $Settings = ({ react, data, setData, triggerUpdate }) => {
    const timeOfDayOptions = [
        "Off",
        "Day",
        "Golden Hour",
        "Night"
    ];

    const weatherOptions = [
        "Off",
        "Sun",
        "Overcast",
        "Rain",
        "Snow"
    ];

    const timeOfDayUpdate = (val) => {
        let parsedVal = val;

        if (val === "Golden Hour")
            parsedVal = "GoldenHour";

        triggerUpdate("TimeOfDay", parsedVal);

        if (val == "Off")
            triggerUpdate("FreezeVisualTime", false);
        else
            triggerUpdate("FreezeVisualTime", true);

        // Optimistically update local state
        setData({ ...data, TimeOfDay: parsedVal, FreezeVisualTime: (val != "Off") });
    }

    const freezeTimeUpdate = (val) => {
        if (!val) {
            triggerUpdate("TimeOfDay", "Off");

            triggerUpdate("FreezeVisualTime", val);

            setData({ ...data, TimeOfDay: "Off", FreezeVisualTime: val });
        }
        else {
            triggerUpdate("FreezeVisualTime", val);
            setData({ ...data, FreezeVisualTime: val });
        }
    }

    const updateData = (field, val) => {
        if (field === "UseStickyWhiteness") {
            setData({ ...data, UseStickyWhiteness: val });
        }
        else if (field === "WhitenessToggle") {
            setData({ ...data, WhitenessToggle: val });
        }
        else if (field === "UseUnits") {
            setData({ ...data, UseUnits: val });
        }
        else if (field === "Weather") {
            setData({ ...data, Weather: val });
        }
        triggerUpdate(field, val);
    }

    const checkedTimeItem = data.TimeOfDay == "GoldenHour" ? "Golden Hour" : data.TimeOfDay;

    return <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
        <div style={{ flex: 1, width: '50%' }}>
            <div style={{ flex: 1, paddingRight: '5rem' }}>
                <$IconPanel label="Use Sticky Whiteness"
                    description="Override the games white info-mode switch, using a custom setting. Toggle with ALT+S."
                    icon="Media/Game/Icons/Information.svg">
                    <$CheckBox react={react} style={{ alignSelf: 'center', margin: '10rem' }} checked={data.UseStickyWhiteness} onToggle={(val) => updateData("UseStickyWhiteness", val)} />
                </$IconPanel>
                <$IconPanel label="Whiteness Toggle"
                    description="If 'Use Sticky Whiteness' is enabled, the info-mode white setting will be set to this value when a tool with an info-mode is activated. Toggle with SHIFT+W."
                    icon="Media/Game/Icons/Orbit.svg">
                    <$CheckBox react={react} style={{ alignSelf: 'center', margin: '10rem' }} checked={data.WhitenessToggle} onToggle={(val) => updateData("WhitenessToggle", val)} />
                </$IconPanel>
                <$IconPanel label="Use Units"
                    description="When a tool system with a length measurement is selected, use 'units' instead. Toggle with ALT+U."
                    icon="Media/Game/Icons/Roads.svg">
                    <$CheckBox react={react} style={{ alignSelf: 'center', margin: '10rem' }} checked={data.UseUnits} onToggle={(val) => updateData("UseUnits", val)} />
                </$IconPanel>
            </div>
        </div>
        <div style={{ flex: 1, width: '50%', paddingLeft: '5rem' }}>
            <$IconPanel label="Freeze time visuals"
                description="When enabled, freezes the visual time of day. 'Day/Night Visuals' must be on."
                icon="Media/PhotoMode/Pause.svg">
                <$CheckBox react={react} style={{ alignSelf: 'center', margin: '10rem' }} checked={data.FreezeVisualTime} onToggle={(val) => freezeTimeUpdate(val)} />
            </$IconPanel>
            <$IconPanel label="Set visual time of day"
                description="Override the visual time of day. 'Day/Night Visuals' must be on. If golden hour goes to midnight the map is missing settings."
                icon="Media/Editor/Time.svg" fitChild="true">
                <$ToggleGroup react={react} checked={checkedTimeItem} options={timeOfDayOptions} isHorizontal="true" onChecked={(val) => timeOfDayUpdate(val)} />
            </$IconPanel>
            <$IconPanel label="Weather"
                description="Override the weather"
                icon="Media/Game/Climate/Overcast.svg" fitChild="true">
                <$ToggleGroup react={react} checked={data.Weather} options={weatherOptions} isHorizontal="true" onChecked={(val) => updateData("Weather", val)} />
            </$IconPanel>
        </div>
    </div>
}

export default $Settings