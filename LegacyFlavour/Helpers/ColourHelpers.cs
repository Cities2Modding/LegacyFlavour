using System;
using System.Text.RegularExpressions;
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
        /// RGBA to Colour
        /// </summary>
        /// <param name="rgbaString"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Color RGBAToColourVar( string rgbaString, out string alpha )
        {
            alpha = null;

            // Pattern to match rgba values and the opacity variable
            var pattern = @"rgba\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(.*?)\s*\)$";
            var regex = new Regex( pattern );
            var match = regex.Match( rgbaString );

            if ( match.Success )
            {
                // Extract RGBA values and the opacity variable
                var r = float.Parse( match.Groups[1].Value ) / 255.0f;
                var g = float.Parse( match.Groups[2].Value ) / 255.0f;
                var b = float.Parse( match.Groups[3].Value ) / 255.0f;
                alpha = match.Groups[4].Value;

                return new Color( r, g, b, 1f );
            }
            else
            {
                throw new ArgumentException( "Invalid RGBA string format." );
            }
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

        /// <summary>
        /// Try match a hue or multiply
        /// </summary>
        /// <param name="originalHex"></param>
        /// <param name="targetHex"></param>
        /// <returns></returns>
        public static string MatchHueOrMultiply( string originalHex, string targetHex )
        {
            // Convert hex to Color
            ColorUtility.TryParseHtmlString( originalHex, out var originalColour );
            ColorUtility.TryParseHtmlString( targetHex, out var targetColour );

            // Convert to HSV
            Color.RGBToHSV( originalColour, out _, out _, out var oV );
            Color.RGBToHSV( targetColour, out var tH, out var tS, out var _ );

            var hex = ColorUtility.ToHtmlStringRGB( Color.HSVToRGB( tH, tS, oV ) );
            return $"#{hex}";
        }

        /// <summary>
        /// Try to match the hue of a colour
        /// </summary>
        /// <param name="originalColour"></param>
        /// <param name="targetHex"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static string MatchHueRGBAVar( Color originalColour, string targetHex, string alpha )
        {
            // Convert hex to Color
            ColorUtility.TryParseHtmlString( targetHex, out var targetColour );

            // Convert to HSV
            Color.RGBToHSV( originalColour, out _, out _, out var oV );
            Color.RGBToHSV( targetColour, out var tH, out var tS, out var _ );

            var matchedColour = Color.HSVToRGB( tH, tS, oV );

            return $"rgba({matchedColour.r * 255f:0}, {matchedColour.g * 255f:0}, {matchedColour.b * 255f:0}, {alpha})";
        }

        /// <summary>
        /// Try match a hue or multiply
        /// </summary>
        /// <param name="originalColour"></param>
        /// <param name="targetHex"></param>
        /// <returns></returns>
        public static string MatchHueOrMultiplyRGBAVar( Color originalColour, string targetHex, string alpha )
        {
            return MatchHueRGBAVar( originalColour, targetHex, alpha );

            // Convert to HSV
            Color.RGBToHSV( originalColour, out var oH, out var oS, out var oV );

            if ( oS != 0f )
                return MatchHueRGBAVar( originalColour, targetHex, alpha );

            ColorUtility.TryParseHtmlString( targetHex, out var targetColour );

            Color.RGBToHSV( targetColour, out var tH, out var tS, out _ );

            // Blend the hues and saturations
            float blendedHue = oH * ( 1 - tS ) + tH * tS; // Blend hue based on target saturation
            float blendedSaturation = oS * ( 1 - tS ) + tS; // Blend saturation

            // Create a new color using the blended hue and saturation, but the value of the original color
            var matchedColour = Color.HSVToRGB( blendedHue, blendedSaturation, oV );

            return $"rgba({matchedColour.r * 255f:0}, {matchedColour.g * 255f:0}, {matchedColour.b * 255f:0}, {alpha})";
        }

        /// <summary>
        /// Hex to RGBA with opacity var
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static string HexToRGBA( string hex, string alpha )
        {
            // Convert hex to Color
            ColorUtility.TryParseHtmlString( hex, out var hexColour );

            // Combine into RGBA string with opacity variable
            return $"rgba({hexColour.r * 255f:0}, {hexColour.g * 255f:0}, {hexColour.b * 255f:0}, {alpha})";
        }
    }
}
