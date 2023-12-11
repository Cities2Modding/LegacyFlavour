import React from 'react'
import $Section from '../components/_section';
import $Button from '../components/_button';
import $IconPanel from '../components/_icon-panel';
import $ColorPicker from '../components/_colorpicker';
import $Select from '../components/_select';
import $TabControl from '../components/_tab-control';
import $CheckBox from '../components/_checkbox';
import $Slider from '../components/_slider';

const $UIThemes = ({ react, themeData, setThemeData, defaultThemeData }) => {
    const [selectedDefaultTheme, setSelectedDefaultTheme] = react.useState('Default');
    const [selectedTheme, setSelectedTheme] = react.useState('Custom');
    const [accentColour, setAccentColour] = react.useState('#ff0000');
    const [backgroundAccentColour, setBackgroundAccentColour] = react.useState('#000000');
    const [usingSelectedTheme, setUsingSelectedTheme] = react.useState(false);

    let defaultThemeList = [];
    let themeList = [];

    if (themeData.Themes) {
        for (var i = 0; i < themeData.Themes.length; i++) {
            themeList.push(themeData.Themes[i].Name);
        }
    }

    if (defaultThemeData.Themes) {
        for (var i = 0; i < defaultThemeData.Themes.length; i++) {
            defaultThemeList.push(defaultThemeData.Themes[i].Name);
        }
    }

    let selectedIndex = themeList.indexOf(selectedTheme);

    const onSelectedThemeChanged = (selected) => {
        setSelectedTheme(selected);
        selectedIndex = themeList.indexOf(selected);

        engine.trigger("cities2modding_legacyflavour.useSelectedTheme", selected);
    };

    const onDefaultSelectedThemeChanged = (selected) => {
        setSelectedDefaultTheme(selected);
        //selectedIndex = themeList.indexOf(selected);

        //engine.trigger("cities2modding_legacyflavour.useSelectedTheme", selected);
    };

    const chunkArray = (array, size) => {
        const result = [];
        for (let i = 0; i < array.length; i += size) {
            result.push(array.slice(i, i + size));
        }
        return result;
    }

    const formatString = (str) => {
        // Remove leading dashes
        let formattedStr = str.replace(/^[-]+/, '');

        // Replace remaining dashes with spaces
        formattedStr = formattedStr.replace(/-/g, ' ');

        // Split into words at uppercase letters, keeping the uppercase letter as part of the word
        formattedStr = formattedStr.replace(/([A-Z])/g, ' $1').trim();

        // Capitalize the first letter of each word
        formattedStr = formattedStr.split(' ').map(word =>
            word.charAt(0).toUpperCase() + word.slice(1)
        ).join(' ');

        return formattedStr;
    };

    const accentGroups = [
        {
            name: "Accent",
            keys: [
                "--accentColorNormal",
                "--accentColorNormal-hover",
                "--accentColorNormal-pressed",
                "--accentColorDark",
                "--accentColorDark-hover",
                "--accentColorDark-pressed",
                "--accentColorDark-focused",
                "--accentColorLight",
                "--accentColorLighter",
                "--focusedColor",
                "--focusedColorDark"
            ],
        },
        {
            name: "Panel",
            keys: [                
                "--panelColorNormal",
                "--panelColorDark",
                "--panelColorDark-hover",
                "--panelColorDark-active",
                "--pausePanelColorDark",

                "--customPanelTextColor"
            ],
        },
        {
            name: "Section",
            keys: [
                "--sectionHeaderColor",
                "--sectionHeaderColorLight",
                "--sectionHeaderLockedColor",
                "--sectionBackgroundColor",
                "--sectionBackgroundColorLight",
                "--sectionBorderColor",
                "--sectionBackgroundLockedColor"
            ],
        },
        {
            name: "Selected",
            keys: [
                "--selectedTextColor",
                "--selectedTextColorDim",
                "--selectedTextColorDimmer",
                "--selectedTextColorDimmest",
                "--selectedColor",
                "--selectedColorDark",
                "--selectedColor-hover",
                "--selectedColor-active"
            ],
        },
        {
            name: "Text",
            keys: [
                "--normalTextColor",
                "--normalTextColorDim",
                "--normalTextColorDimmer",
                "--normalTextColorDimmest",

                "--normalTextColorDark",
                "--normalTextColorDarkDim",
                "--normalTextColorDarkDimmer",
                "--normalTextColorDarkDimmest",

                "--normalTextColorHighlight",
                "--normalTextColorHighlightDim",
                "--normalTextColorHighlightDimmer",
                "--normalTextColorHighlightDimmest"
            ]
        },
        {
            name: "Menu ",
            keys: [                
                "--menuText1Inverted",
                "--menuText2Inverted",

                "--menuPanel1",
                "--menuPanel2",

                "--menuTitleNormal",
                "--menuText1Normal",
                "--menuText2Normal",
                "--menuText1Disabled",

                "--menuControlBorder"
            ]
        },
        {
            name: "Other",
            keys: [
                "--positiveColor",
                "--warningColor",
                "--negativeColor",

                "--customTabTextColor",
                "--customTabSelectedTextColor",

                "--customChirperPanelTextColor",
                "--customChirperPanelColor",
                "--customChirperItemTextColor",
                "--customChirperItemColor",
            ]
        }
    ];

    const filterSettings = (settings, accentGroup) => {
        var filteredSettings = [];
        for (var i = 0; i < settings.length; i++) {
            if (accentGroup.keys.includes(settings[i].Key))
                filteredSettings.push(settings[i]);
        }

        return filteredSettings;
    };

    const getTheme = (theme, accentGroup) => {
        const filteredSettings = filterSettings(theme.Settings, accentGroup);

        // Split the settings into chunks of 6
        const settingChunks = chunkArray(filteredSettings, 6);

        // Filter out chunks that don't have any settings with a value starting with '#'
        const filteredChunks = settingChunks.filter(chunk =>
            chunk.some(setting => setting.Value.indexOf('#') !== -1 || setting.Value.indexOf('rgba(') !== -1)
        );

        return (
            <div style={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
                {filteredChunks.map((chunk, chunkIndex) => (
                    <div key={chunk.name + '-container'} style={{ flex: 1, width: '33.33333333333%', maxWidth: '50%', paddingLeft: chunkIndex > 0 ? '5rem' : '0', paddingRight: chunkIndex < filteredChunks.length - 1 ? '5rem' : '0' }}>
                        <$Section contentStyle={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
                            {chunk.map((setting) => (
                                ((setting.Value.indexOf('#') !== -1 || setting.Value.indexOf('rgba(') !== -1)) && (
                                    <div key={chunk.name + '-' + setting.Key + '-cp-container'}>
                                        <$ColorPicker key={chunk.name + '-' + setting.Key + '-cp'} react={react} label={formatString(setting.Key)} color={setting.Value} onChanged={(newColour) => { doUpdateThemeValue(setting.Key, newColour); /* triggerZoneColourUpdate(setting.Key, newColour); */ }} />
                                    </div>
                                )
                            ))}
                        </$Section>
                    </div>
                ))}
            </div>
        );
    };

    const updateAccent = (colour) => {
        setAccentColour(colour);
    };

    const updateBackgroundAccent = (colour) => {
        setBackgroundAccentColour(colour);
    };

    const doUpdateThemeValue = (key, colour) => {
        let json = JSON.stringify({
            key: key,
            value: colour
        });

        engine.trigger("cities2modding_legacyflavour.updateThemeValue", selectedTheme, json);
    };

    const generateAccent = (visible) => {
        // If we close the colour drop down
        if (!visible) {
            let json = JSON.stringify({
                accent: accentColour,
                backgroundAccent: backgroundAccentColour,
                defaultTheme: selectedDefaultTheme
            });
            engine.trigger("cities2modding_legacyflavour.generateThemeAccent", selectedTheme, json);

            if (usingSelectedTheme) {
                setTimeout(function () {
                    engine.trigger("cities2modding_legacyflavour.useSelectedTheme", selectedTheme);
                }, 250);
            }
        }
    };

    const useSelectedTheme = () => {
        setUsingSelectedTheme(true);
        setTimeout(function () {
            engine.trigger("cities2modding_legacyflavour.useSelectedTheme", selectedTheme);
        }, 150);
    };

    const tabs = [
        {
            name: 'Accent',
            content: <div style={{ display: 'flex', width: '100%' }}>
                {selectedIndex !== -1 && getTheme(themeData.Themes[selectedIndex], accentGroups[0])}
            </div>
        },
        {
            name: 'Panel',
            content: <div style={{ display: 'flex', width: '100%' }}>
                {selectedIndex !== -1 && getTheme(themeData.Themes[selectedIndex], accentGroups[1])}
            </div>
        },
        {
            name: 'Section',
            content: <div style={{ display: 'flex', width: '100%' }}>
                {selectedIndex !== -1 && getTheme(themeData.Themes[selectedIndex], accentGroups[2])}
            </div>
        },
        {
            name: 'Selected',
            content: <div style={{ display: 'flex', width: '100%' }}>
                {selectedIndex !== -1 && getTheme(themeData.Themes[selectedIndex], accentGroups[3])}
            </div>
        },
        {
            name: 'Text',
            content: <div style={{ display: 'flex', width: '100%' }}>
                {selectedIndex !== -1 && getTheme(themeData.Themes[selectedIndex], accentGroups[4])}
            </div>
        },
        {
            name: 'Menu',
            content: <div style={{ display: 'flex', width: '100%' }}>
                {selectedIndex !== -1 && getTheme(themeData.Themes[selectedIndex], accentGroups[5])}
            </div>
        },
        {
            name: 'Other',
            content: <div style={{ display: 'flex', width: '100%' }}>
                {selectedIndex !== -1 && getTheme(themeData.Themes[selectedIndex], accentGroups[6])}
            </div>
        }
    ];

    return <div>
        <div style={{ display: 'flex', flexDirection: 'row' }}>
            <div style={{ width: '50%', paddingRight: '5rem' }}>
                <$IconPanel label="Theme"
                    description="Select a theme to edit"
                    icon="Media/Editor/Edit.svg" fitChild="true">
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
                        <$Select react={react} selected={selectedTheme} options={themeList} style={{ margin: '10rem', flex: '1' }} onSelectionChanged={onSelectedThemeChanged}></$Select>
                        <$Button onClick={useSelectedTheme}>Use selected theme</$Button>
                    </div>
                </$IconPanel>
            </div>
            <div style={{ width: '50%', paddingLeft: '5rem' }}>
                <$IconPanel label="Generate from colour"
                    description="Select a base theme, main accent colour and generate a theme."
                    icon="Media/Editor/Edit.svg" fitChild="true">
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
                        <$Select react={react} selected={selectedDefaultTheme} options={defaultThemeList} style={{ margin: '10rem', flex: '1' }} onSelectionChanged={onDefaultSelectedThemeChanged}></$Select>

                        <div style={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
                            <$ColorPicker style={{ width: '50%' }} key="lf-accent" react={react} label="Primary Accent" color={accentColour} onChanged={(newColour) => { updateAccent(newColour); }} onDropdown={generateAccent} />
                            <$ColorPicker style={{ width: '50%' }} key="lf-bg-accent" react={react} label="Background Accent" color={backgroundAccentColour} onChanged={(newColour) => { updateBackgroundAccent(newColour); }} onDropdown={generateAccent} />
                        </div>
                    </div>
                </$IconPanel>
            </div>
        </div>
        <div style={{ display: 'flex', width: '100%', flexDirection: 'row', alignItems: 'center', justifyContent: 'center' }}>
            <$TabControl react={react} tabs={tabs} />
        </div>
    </div>
}

export default $UIThemes