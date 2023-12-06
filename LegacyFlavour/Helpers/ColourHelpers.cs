using UnityEngine;

namespace LegacyFlavour.Helpers
{
    /// <summary>
    /// Colour modification helpers
    /// </summary>
    public static class ColourHelpers
    {
        /// <summary>
        /// Convert a hex colour string to a Unity colour
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor( string hex )
        {
            if ( ColorUtility.TryParseHtmlString( hex, out var color ) )
                return color;

            return Color.white;
        }

        /// <summary>
        /// Convert a color to hex code
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static string ColorToHex( Color color )
        {
            return "#" + ColorUtility.ToHtmlStringRGB( color );
        }

        /// <summary>
        /// Darken a colour
        /// </summary>
        /// <param name="originalColor"></param>
        /// <param name="darkenAmount"></param>
        /// <returns></returns>
        public static Color Darken( Color originalColor, float darkenAmount )
        {
            // Convert the color to HSV
            Color.RGBToHSV( originalColor, out float H, out float S, out float V );

            // Decrease the V (value/brightness) by the darken amount, clamping it between 0 and 1
            V = Mathf.Clamp01( V - darkenAmount );

            // Convert back to RGB and return the new color
            return Color.HSVToRGB( H, S, V );
        }

        /// <summary>
        /// Set a colour's alpha
        /// </summary>
        /// <param name="originalColor"></param>
        /// <param name="opacity"></param>
        /// <returns></returns>
        public static Color SetAlpha( Color originalColor, float opacity )
        {
            return new Color( originalColor.r, originalColor.g, originalColor.b, opacity );
        }

        /// <summary>
        /// Try to match the hue of a colour
        /// </summary>
        /// <param name="originalHex"></param>
        /// <param name="targetHex"></param>
        /// <returns></returns>
        public static string MatchHue( string originalHex, string targetHex )
        {
            // Convert hex to Color
            ColorUtility.TryParseHtmlString( originalHex, out var originalColour );
            ColorUtility.TryParseHtmlString( targetHex, out var targetColour );

            // Convert to HSV
            Color.RGBToHSV( originalColour, out _, out _, out float oV );
            Color.RGBToHSV( targetColour, out float tH, out float tS, out _ );

            // Create a new color with the hue and saturation of the target color, but with the value of the original color
            var matchedColour = Color.HSVToRGB( tH, tS, oV );

            // Convert back to hex and return
            var matchedHex = ColorUtility.ToHtmlStringRGB( matchedColour );
            return $"#{matchedHex}";
        }
    }
}
