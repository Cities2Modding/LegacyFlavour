# LegacyFlavour Mod for Cities Skylines 2

Welcome to the GitHub repository for LegacyFlavour, a Cities Skylines 2 mod designed to blend the best of Cities Skylines 1 with the advanced features of Cities Skylines 2. Our mod aims to enhance your city-building experience by introducing nostalgic elements and improved functionalities.

## Updates for v0.0.9
- **Language support**: Introduced localisation support for: de-DE (German), es-ES (Spanish), fr-FR (French), it-IT (Italian), ja-JP (Japanese), ko-KR (Korean), pl-PL (Polish), pt-BR (Portuguese - Brazil), ru-RU (Russian), zh-HANS (Chinese - Simplified), zh-HANT (Chinese - Traditional).

## Features

LegacyFlavour currently includes the following features:

1. **Unit Display for Road Tools**: This feature introduces a 'Unit' display for road tools, making it easier to plan and build your roads with precision. (Note: This feature requires the unit type to be set to Imperial in the game options.)

2. **Transparent Zone Grid Cells**: We've made zone grid cells transparent for a cleaner, more visually appealing city layout, allowing for a more immersive city planning experience.

3. **Re-worked colour Zone colour scheme**: Changes the colour scheme of zones to better match Cities Skylines 1. (UI icons will be updated too in the near future).

4. **Colour-Blindness Modes**: This is WIP and currently in testing. Cycle zone colours to match a scheme for specific types of colour blindness.

5. **Toggle Shortcuts**: 

- ALT+Z to toggle the zone colours mod. 
- SHIFT+Z to cycle through colour blindness modes.
- ALT+S to toggle Sticky Infomode Whiteness. 
- SHIFT+W to toggle Sticky Infomode Whiteness value override.
- ALT+R to reload the config file
- ALT+U to toggle Units display

6. **Configure Zone colours via JSON**: Config file in mod directory to allow custom values.

7. **Dynamic Zone Borders**: If it snows zone borders will switch colour to be more visible on snow. **UPDATE** this now calculates snow coverage so is more accurate. It may have a delay as updates occur periodically to prevent performance hits.

8. **Dynamic UI Icons**: UI icons for zones now adapt to match the configured colours. These are dynamically generated from the games icons based on config settings.

9. **Sticky Infomode Whiteness**: Shortcuts to override infomode 'whiteness' setting to stop it constantly popping up.

10. **UI System via HookUI**: Integrates with [HookUI](https://github.com/Captain-Of-Coit/hookui/releases) to offer an advanced UI system, elevating your control and customization options.

11. **Configurable Zoning Configuration**: Customize zoning options like never before. Adjust colours, borders, and more, directly from the UI.

12. **Enhanced Control over Weather and Time**: Control weather and time settings with new features, including saving these states between game reloads.

13. **UI Theme Editor**: Introducing a new UI theme editor with a variety of built-in themes. Customize your game's look with ease.

14. **Custom Theme Creation**: Now you can create and save your own custom themes using our advanced color pickers.

15. **Accent and Background Accent Customization**: Enhance your game's UI with personalized accent and background colors that change dynamically.

16. **Integrated Themes in UI Options**: Newly created themes will be available for preview and selection directly in the UI options screen.

## Installation

### Requirements

- **BepInEx v5**: LegacyFlavour requires BepInEx version 5 to be installed in your Cities Skylines 2 game. Ensure you have this version before proceeding with the installation. Also install HookUI as this mod now depends on it.

### Downloading

To ensure you get the genuine LegacyFlavour mod, please download it from either of these two sources:

- **This GitHub Repository**
- **[ThunderStore.io](https://thunderstore.io)**

### Steps

1. Download the LegacyFlavour mod from either this GitHub repository or ThunderStore.io.
2. Extract the downloaded file.
3. Place the extracted files into the `BepInEx/plugins` folder in your Cities Skylines 2 directory.
4. Launch Cities Skylines 2, and the mod should be active!

## Contribution

Feel free to contribute to the development of LegacyFlavour! If you have ideas or suggestions, please open an issue or a pull request in this repository.

## Support

If you encounter any issues or have questions, please open an issue in this repository. We'll do our best to assist you.

## Disclaimer

LegacyFlavour is a fan-made mod and is not affiliated with the official Cities Skylines game or its creators.

## License

LegacyFlavour is released under the GNU General Public License v2.0. For more details, see the [LICENSE](LICENSE) file included in this repository.

## Acknowledgements

A big thank you to the Cities Skylines modding community for their ongoing support and inspiration.

---

Enjoy building your city with a taste of nostalgia with LegacyFlavour! 🏙️🌉