using System;
using System.Collections.Generic;
using System.Globalization;

namespace RotatingCameraLCD.Shared
{
    /// <summary>
    /// Helper-Class for storing CustomData Configs
    /// </summary>
    public sealed class CustomDataConfig
    {
        /// <summary> 0-based index of the surface to display the camera on </summary>
        public int LcdSurfaceIndex { get; set; } = 0;

        /// <summary>how many seconds a camera will be displayed before the lcd switches to the next one </summary>
        public double RotateEverySeconds { get; set; } = 5.0;

        /// <summary>Font size of the camera name to be displayed </summary>
        public float NameFontSize { get; set; } = 0.9f;

        /// <summary> List of camera definitions (Title and blockname)</summary>
        public IReadOnlyList<(string DisplayName, string CameraBlockName)> CameraMappings { get; set; }
            = Array.Empty<(string, string)>();
    }

    /// <summary>
    /// Read config from a LCD panel for camera display
    /// Is parsed from the [MultiCamera2LCD]-Section of CustomData.
    /// </summary>
    public static class CustomDataParser
    {
        private const string SectionName = "MultiCamera2LCD";

        /// <summary> Parse the LCD's CustomData and return a new CustomDataConfig object</summary>
        public static CustomDataConfig Parse(string? customData)
        {
            if (string.IsNullOrWhiteSpace(customData))
                return new CustomDataConfig();

            if (customData == null)
                return new CustomDataConfig();

            var lines = customData.Replace("\r\n", "\n").Split('\n');
            bool inSection = false;

            int lcdSurfaceIndex = 0;
            double rotateSec = 5.0;
            float nameFontSize = 0.9f;
            var map = new List<(string, string)>();

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("//"))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    var section = line.Substring(1, line.Length - 2).Trim();
                    inSection = string.Equals(section, SectionName, StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (!inSection)
                    continue;

                var eq = line.IndexOf('=');
                if (eq > 0)
                {
                    var key = line.Substring(0, eq).Trim();
                    var val = line.Substring(eq + 1).Trim();

                    if (key.Equals("LcdSurfaceIndex", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(val, NumberStyles.Integer, CultureInfo.InvariantCulture, out var idx))
                            lcdSurfaceIndex = Math.Max(0, idx);
                        continue;
                    }

                    if (key.Equals("RotateEveryXSeconds", StringComparison.OrdinalIgnoreCase))
                    {
                        if (double.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                            rotateSec = Math.Max(0.1, d);
                        continue;
                    }

                    if (key.Equals("NameFontSize", StringComparison.OrdinalIgnoreCase))
                    {
                        val = val.TrimEnd('f','F');
                        if (float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
                            nameFontSize = Math.Max(0.1f, f);
                        continue;
                    }
                }
                else
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 2)
                    {
                        var display = parts[0].Trim();
                        var block = parts[1].Trim();
                        if (!string.IsNullOrWhiteSpace(block))
                            map.Add((display, block));
                    }
                }
            }

            return new CustomDataConfig
            {
                LcdSurfaceIndex = lcdSurfaceIndex,
                RotateEverySeconds = rotateSec,
                NameFontSize = nameFontSize,
                CameraMappings = map
            };
        }
    }
}