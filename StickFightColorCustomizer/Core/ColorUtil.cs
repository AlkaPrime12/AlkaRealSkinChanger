using System;
using System.Globalization;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class ColorUtil
    {
        public static string ToHex(Color color)
        {
            Color32 c = color;
            return "#" + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");
        }

        public static bool TryParseHex(string hex, out Color color)
        {
            color = Color.white;
            if (string.IsNullOrEmpty(hex))
            {
                return false;
            }

            string s = hex.Trim();
            if (s.StartsWith("#"))
            {
                s = s.Substring(1);
            }

            if (s.Length != 6 && s.Length != 8)
            {
                return false;
            }

            try
            {
                int r = int.Parse(s.Substring(0, 2), NumberStyles.HexNumber);
                int g = int.Parse(s.Substring(2, 2), NumberStyles.HexNumber);
                int b = int.Parse(s.Substring(4, 2), NumberStyles.HexNumber);
                float a = 1f;
                if (s.Length == 8)
                {
                    int ai = int.Parse(s.Substring(6, 2), NumberStyles.HexNumber);
                    a = ai / 255f;
                }

                color = new Color(r / 255f, g / 255f, b / 255f, a);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Color FromRgbBytes(int r, int g, int b)
        {
            return new Color(
                Mathf.Clamp(r, 0, 255) / 255f,
                Mathf.Clamp(g, 0, 255) / 255f,
                Mathf.Clamp(b, 0, 255) / 255f,
                1f);
        }

        public static void RgbToBytes(Color color, out int r, out int g, out int b)
        {
            r = Mathf.RoundToInt(color.r * 255f);
            g = Mathf.RoundToInt(color.g * 255f);
            b = Mathf.RoundToInt(color.b * 255f);
        }

        public static Color HsvCycle(float hue01)
        {
            float h = hue01 - Mathf.Floor(hue01);
            return Color.HSVToRGB(h, 1f, 1f);
        }

        public static bool ColorsNear(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.02f
                && Mathf.Abs(a.g - b.g) < 0.02f
                && Mathf.Abs(a.b - b.b) < 0.02f;
        }
    }
}
