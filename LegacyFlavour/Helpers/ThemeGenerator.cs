using LegacyFlavour.Configuration.Themes;
using LegacyFlavour.Systems;
using System.Collections.Generic;
using System.Linq;

namespace LegacyFlavour.Helpers
{
    public class ThemeGenerator
    {
        private ThemeEntry _themeCache;
        private Dictionary<string, ThemeSetting> _settingsCache = new Dictionary<string, ThemeSetting>( );

        private readonly LegacyFlavourUpdateSystem _updateSystem;
        private readonly ThemeStylesheetBuilder _builder;

        public ThemeGenerator( LegacyFlavourUpdateSystem updateSystem )
        {
            _updateSystem = updateSystem;
            _builder = new ThemeStylesheetBuilder( _updateSystem );
        }

        public void SetDefaultConfig( )
        {
            if ( _updateSystem.ThemeConfig == null || _updateSystem.ThemeConfig.Themes?.Count > 0 )
                    return;

            _updateSystem.ThemeConfig.Themes = new List<ThemeEntry>( );
            _builder.Generate( "Default", "Custom", "#a0784d" ); // Just fill a default one
        }

        /// <summary>
        /// Get a list of all themes
        /// </summary>
        /// <returns></returns>
        public List<string> GetThemeNames( )
        {
            return _builder.GetThemeNames( );
        }

        /// <summary>
        /// Export the current stylesheet
        /// </summary>
        public void Export( )
        {
            _builder.ExportStylesheet( );
        }

        /// <summary>
        /// Inject the current stylesheet
        /// </summary>
        public void Inject( )
        {
            _builder.InjectStylesheet( );
        }

        /// <summary>
        /// Generate a style from accents
        /// </summary>
        /// <param name="defaultTheme"></param>
        /// <param name="theme"></param>
        /// <param name="accent"></param>
        /// <param name="backgrounds"></param>
        public void GenerateFromAccent( string defaultTheme, string theme, string accent, string backgrounds )
        {
            var themeEntry = _updateSystem.ThemeConfig.Themes?.FirstOrDefault( t => t.Name == theme );

            if ( themeEntry == null )
                return;

            _builder.GenerateSettingsFromValue( defaultTheme, theme, accent, themeEntry, bgAccent: backgrounds );
            _updateSystem.EnqueueThemeConfigUpdate( true );
        }

        private void CheckForSettingsCache( string theme, bool invalidate = false )
        {
            if ( _updateSystem.ThemeConfig == null || _updateSystem.ThemeConfig.Themes == null )
                return;

            if ( invalidate || _themeCache == null || _themeCache.Name != theme )
            {
                _themeCache = _updateSystem.ThemeConfig.Themes
                    .FirstOrDefault( t => t.Name == theme );
            }

            if ( _themeCache == null || _themeCache.Settings == null )
                return;

            if ( invalidate || _settingsCache.Count != _themeCache.Settings.Count )
            {
                foreach ( var setting in _themeCache.Settings )
                    _settingsCache.Add( setting.Key, setting );
            }
        }

        /// <summary>
        /// Update a setting on the current theme
        /// </summary>
        /// <param name="theme"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateSetting( string theme, string key, string value )
        {
            CheckForSettingsCache( theme );

            if ( !_settingsCache.TryGetValue( key, out var setting ) ||
                string.IsNullOrEmpty( value ) )
                return;

            setting.Value = value;
            _updateSystem.EnqueueThemeConfigUpdate( );
        }
    }
}
