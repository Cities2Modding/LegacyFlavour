using cohtml.Net;
using Game.SceneFlow;
using Game.Settings;
using LegacyFlavour.Configuration;
using LegacyFlavour.Configuration.Themes;
using LegacyFlavour.Patches;
using LegacyFlavour.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LegacyFlavour.Helpers
{
    /// <summary>
    /// Handles the specifics of building CSS etc.
    /// </summary>
    public class ThemeStylesheetBuilder
    {
        static readonly HashSet<string> ACCENT_KEYS = new HashSet<string>
        {
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
            "--focusedColorDark",
            "--menuControlBorder",
            "--selectedColor",
            "--selectedColorDark",
            "--selectedColor-hover",
            "--selectedColor-active"
        };

        static readonly HashSet<string> BACKGROUND_ACCENT_KEYS = new HashSet<string>
        {
            "--panelColorNormal",
            "--panelColorDark",
            "--panelColorDark-hover",
            "--panelColorDark-active",

            "--pausePanelColorDark",

            "--sectionHeaderColor",
            "--sectionHeaderColorLight",

            "--sectionBackgroundColor",
            "--sectionBackgroundColorLight",

            "--sectionBorderColor",

            "--menuPanel1",
            "--menuPanel2",
            "--menuControl1",
            "--menuControl2"
        };

        static readonly string CSS_FILENAME = Path.Combine( "UI", "themes.css" );
        static readonly string ASSEMBLY_PATH = Path.GetDirectoryName( typeof( ConfigBase ).Assembly.Location );
        static readonly string THEMES_FILE = Path.Combine( ASSEMBLY_PATH, CSS_FILENAME );

        private readonly LegacyFlavourUpdateSystem _updateSystem;
        private readonly List<ThemeEntry> _defaultThemes;
        private readonly View _view;
        private readonly string _script;
        private readonly string _styles;
        
        public ThemeStylesheetBuilder( LegacyFlavourUpdateSystem updateSystem )
        {
            _updateSystem = updateSystem;
            _defaultThemes = ThemeConfig.Default.Themes;
            _view = GameManager.instance.userInterface.view.View;
            _script = LoadResource( "theme-inject.js" );
            _styles = LoadResource( "theme-template.scss" );
        }

        /// <summary>
        /// Build and inject the themes
        /// </summary>
        public void BuildAndInject( )
        {
            ExportStylesheet( );
            InjectStylesheet( );
        }

        /// <summary>
        /// Inject our CSS into the UI
        /// </summary>
        public void InjectStylesheet( )
        {
            _view.ExecuteScript( _script );
        }

        /// <summary>
        /// Load the injection script from embedded resources
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string LoadResource( string name )
        {
            var resourceName = "LegacyFlavour.Resources." + name;
            using ( var stream = typeof( ConfigBase ).Assembly.GetManifestResourceStream( resourceName ) )
            {
                if ( stream == null )
                {
                    UnityEngine.Debug.LogError( $"Embedded default config not found: {resourceName}" );
                    return null;
                }
                using ( var reader = new StreamReader( stream ) )
                {
                    return reader.ReadToEnd( );
                }
            }
        }

        /// <summary>
        /// Generate a theme based on a target value
        /// </summary>
        /// <param name="defaultTheme"></param>
        /// <param name="name"></param>
        /// <param name="targetHex"></param>
        /// <param name="bgAccent"></param>
        /// <param name="textAccentLight"></param>
        /// <param name="textAccentDark"></param>
        /// <param name="textAccentHighLight"></param>
        public void Generate( string defaultTheme, string name, string targetHex, string bgAccent = "#000000", string textAccentLight = "#FFFFFF", string textAccentDark = "#000000", string textAccentHighLight = "default" )
        {
            var copy = GenerateSettingsFromValue( defaultTheme, name, targetHex, bgAccent: bgAccent, textAccentLight: textAccentLight, textAccentDark: textAccentDark, textAccentHighLight: textAccentHighLight );

            if ( copy == null )
                return;

            var existingTheme = _updateSystem.ThemeConfig.Themes.FirstOrDefault( t => t.Name == copy.Name );

            if ( existingTheme != null )
                existingTheme.Settings = copy.Settings;
            else
                _updateSystem.ThemeConfig.Themes.Add( copy );
        }

        /// <summary>
        /// Generate a theme based on a target value
        /// </summary>
        /// <param name="defaultTheme"></param>
        /// <param name="name"></param>
        /// <param name="targetHex"></param>
        /// <param name="instance"></param>
        /// <param name="bgAccent"></param>
        /// <param name="textAccentLight"></param>
        /// <param name="textAccentDark"></param>
        /// <param name="textAccentHighLight"></param>
        public ThemeEntry GenerateSettingsFromValue( string defaultTheme, string name, string targetHex, ThemeEntry instance = null, string bgAccent = "#000000", string textAccentLight = "#FFFFFF", string textAccentDark = "#000000", string textAccentHighLight = "default" )
        {
            var coreTheme = ThemeConfig.Default.Themes?
                .FirstOrDefault( t => t.Name == "Default" );

            var defaultValue = ThemeConfig.Default.Themes?
                .FirstOrDefault( t => t.Name == defaultTheme );

            if ( defaultValue == null || defaultValue.Settings == null )
                return null;

            var defaultSettings = defaultValue.Settings;

            // If this setting doesn't have the default included then include it
            foreach ( var setting in coreTheme.Settings )
            {
                if ( defaultSettings.Count( s => s.Key == setting.Key ) == 0 )
                    defaultSettings.Add( new ThemeSetting { Key = setting.Key, Value = setting.Value } );
            }

            var themeEntry = instance ?? new ThemeEntry { Name = name, Settings = new List<ThemeSetting>( ) };

            themeEntry.Settings.Clear( ); // Clear previous settings

            foreach ( var setting in defaultValue.Settings )
            {
                var dontColour = setting.Key.StartsWith( "--positive" ) || setting.Key.StartsWith( "--warning" ) || setting.Key.StartsWith( "--negative" ) ||
                    setting.Key.StartsWith( "--customTabTextColor" ) || setting.Key.StartsWith( "--customTabSelectedTextColor" ) || setting.Key.StartsWith( "--customPanelTextColor" );

                var targetColour = BACKGROUND_ACCENT_KEYS.Contains( setting.Key ) ? bgAccent : targetHex;

                var value = setting.Value;

                if ( setting.Value.StartsWith( "rgba(" ) )
                {
                    var colour = ColourHelpers.RGBAToColourVar( setting.Value, out var variableName );
                    
                    if ( BACKGROUND_ACCENT_KEYS.Contains( setting.Key ) || ACCENT_KEYS.Contains( setting.Key ) )
                        value = ColourHelpers.MatchHueOrMultiplyRGBAVar( colour, targetColour, variableName );
                    else
                        value = setting.Value;
                }
                else
                {
                    ColorUtility.TryParseHtmlString( setting.Value, out var colour );                    

                    if ( BACKGROUND_ACCENT_KEYS.Contains( setting.Key ) || ACCENT_KEYS.Contains( setting.Key ) )
                    {
                        value = ColourHelpers.MatchHueOrMultiply( setting.Value, targetColour );
                    }
                    else
                        value = setting.Value;
                }

                themeEntry.Settings.Add( new ThemeSetting { Key = setting.Key, Value = value } );
            }

            return themeEntry;
        }

        /// <summary>
        /// Export a config to CSS
        /// </summary>
        /// <param name="config"></param>
        public void ExportStylesheet( )
        {
            if ( _updateSystem.ThemeConfig == null )
                return;

            BuildStylesheet( GetThemes( ) );
        }

        /// <summary>
        /// Export the rules to a CSS file
        /// </summary>
        private void BuildStylesheet( List<ThemeEntry> themes )
        {
            try
            {
                var cssBuilder = new StringBuilder( );
                var settingsBuilder = new StringBuilder( );

                var coreTheme = ThemeConfig.Default.Themes?
                    .FirstOrDefault( t => t.Name == "Default" );

                foreach ( var theme in themes )
                {
                    var mainAccent = theme.Settings.FirstOrDefault( s => s.Key == "--accentColorNormal" );

                    if ( mainAccent == null || InterfaceSettingsPatch.DEFAULT_THEMES.Contains( theme.Name.ToLower().Replace( " ", "-" ) ) )
                        continue;

                    var css = _styles;
                    var themeClassName = theme.Name.Replace( ' ', '-' ).ToLower( );
                    css = css.Replace( "$theme:;", string.Empty );
                    css = css.Replace( "$mainAccent:;", string.Empty );

                    css = css.Replace( "#{$theme}", themeClassName );
                    css = css.Replace( "$theme", themeClassName );
                    css = css.Replace( "$mainAccent", mainAccent.Value );

                    settingsBuilder.Clear( );

                    foreach ( var setting in theme.Settings )
                    {
                        settingsBuilder.AppendLine( $"\t{setting.Key}: {setting.Value};" );
                    }

                    css = css.Replace( "$settings:;", settingsBuilder.ToString() ).Trim();

                    cssBuilder.Append( css );
                    cssBuilder.AppendLine( );
                }

                File.WriteAllText( THEMES_FILE, cssBuilder.ToString( ).Trim( ) );
            }
            catch ( Exception ex )
            {
                // Do error logging
            }
        }

        /// <summary>
        /// Get a list of theme names including defaults
        /// </summary>
        /// <param name="customThemes"></param>
        /// <returns></returns>
        public List<string> GetThemeNames( )
        {
            var customThemes = _updateSystem.ThemeConfig.Themes;

            if ( customThemes == null || customThemes.Count == 0 )
                return _defaultThemes.Select( t => t.Name ).ToList();

            var themes = new List<string>( _defaultThemes.Count + customThemes.Count );
            themes.AddRange( _defaultThemes.Select( t => t.Name ) );
            themes.AddRange( customThemes.Select( t => t.Name ) );

            return themes;
        }

        /// <summary>
        /// Get all themes including default ones
        /// </summary>
        /// <param name="customThemes"></param>
        /// <returns></returns>
        private List<ThemeEntry> GetThemes( )
        {
            var customThemes = _updateSystem.ThemeConfig.Themes;

            // Return a copy of default if no custom themes exist
            if ( customThemes == null || customThemes.Count == 0 )
                return _defaultThemes.ToList( );

            var themes = new List<ThemeEntry>( _defaultThemes.Count + customThemes.Count );
            themes.AddRange( _defaultThemes );
            themes.AddRange( customThemes );

            return themes;
        }
    }
}
