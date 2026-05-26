using StickFightColorCustomizer.Models;
using StickFightColorCustomizer.Network;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Código compartible: cuerpo + mitades distales + glow (+ uniform skin).
    /// </summary>
    public static class SkinShareCodec
    {
        public const string Prefix = "SFCCSKIN4|";

        public static string Encode(ColorConfig config)
        {
            if (config == null || config.Colors == null)
            {
                return string.Empty;
            }

            BodyColors c = config.Colors;
            GlowSettings g = config.Glow ?? new GlowSettings();

            string body = ColorSyncCodec.Encode(c);
            int zones = 0;
            if (g.OnHead) zones |= 1;
            if (g.OnTorso) zones |= 2;
            if (g.OnArms) zones |= 4;
            if (g.OnLegs) zones |= 8;
            if (g.OnWings) zones |= 16;

            return Prefix + body
                + "|" + ColorUtil.ToHex(c.SpineDistal)
                + "|" + ColorUtil.ToHex(c.LegLeftDistal)
                + "|" + ColorUtil.ToHex(c.LegRightDistal)
                + "|" + ColorUtil.ToHex(c.HandLeftDistal)
                + "|" + ColorUtil.ToHex(c.HandRightDistal)
                + "|" + (g.Enabled ? "1" : "0")
                + "|" + ColorUtil.ToHex(g.Color)
                + "|" + g.Strength.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                + "|" + g.AuraWidth.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                + "|" + g.AuraAlpha.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                + "|" + zones
                + "|" + (g.MaintainInLobby ? "1" : "0")
                + "|" + (config.UseUniformSkin ? "1" : "0");
        }

        public static bool TryDecode(string raw, out ColorConfig config)
        {
            config = null;
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }

            string s = raw.Trim();
            if (!s.StartsWith(Prefix))
            {
                return false;
            }

            string payload = s.Substring(Prefix.Length);
            string[] parts = payload.Split('|');
            if (parts.Length < 20)
            {
                return false;
            }

            string bodyPayload = BuildBodyPayload(parts);
            BodyColors colors;
            if (!ColorSyncCodec.TryDecode(bodyPayload, out colors))
            {
                return false;
            }

            Color distal;
            if (ColorUtil.TryParseHex(parts[9], out distal)) colors.SpineDistal = distal;
            if (ColorUtil.TryParseHex(parts[10], out distal)) colors.LegLeftDistal = distal;
            if (ColorUtil.TryParseHex(parts[11], out distal)) colors.LegRightDistal = distal;
            if (ColorUtil.TryParseHex(parts[12], out distal)) colors.HandLeftDistal = distal;
            if (ColorUtil.TryParseHex(parts[13], out distal)) colors.HandRightDistal = distal;

            GlowSettings glow = new GlowSettings();
            glow.Enabled = parts[14] == "1";
            if (!ColorUtil.TryParseHex(parts[15], out glow.Color))
            {
                glow.Color = Color.cyan;
            }

            float parsedFloat;
            if (float.TryParse(parts[16], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out parsedFloat))
            {
                glow.Strength = Mathf.Clamp(parsedFloat, 0f, 2f);
            }

            if (float.TryParse(parts[17], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out parsedFloat))
            {
                glow.AuraWidth = Mathf.Clamp(parsedFloat, 1f, 3f);
            }

            if (float.TryParse(parts[18], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out parsedFloat))
            {
                glow.AuraAlpha = Mathf.Clamp(parsedFloat, 0.1f, 0.9f);
            }

            int zones;
            if (int.TryParse(parts[19], out zones))
            {
                glow.OnHead = (zones & 1) != 0;
                glow.OnTorso = (zones & 2) != 0;
                glow.OnArms = (zones & 4) != 0;
                glow.OnLegs = (zones & 8) != 0;
                glow.OnWings = (zones & 16) != 0;
            }

            if (parts.Length > 20)
            {
                glow.MaintainInLobby = parts[20] == "1";
            }

            bool uniform = parts.Length > 21 && parts[21] == "1";

            colors.HalfColorEnabled = HasDistinctHalfColors(colors);

            config = new ColorConfig
            {
                Colors = colors,
                Glow = glow,
                UseUniformSkin = uniform,
                ActivePreset = "Custom",
                BodyCustomizationActive = true
            };

            return true;
        }

        public static bool HasDistinctHalfColors(BodyColors colors)
        {
            if (colors == null)
            {
                return false;
            }

            return !ColorUtil.ColorsNear(colors.Spine, colors.SpineDistal)
                || !ColorUtil.ColorsNear(colors.LegLeft, colors.LegLeftDistal)
                || !ColorUtil.ColorsNear(colors.LegRight, colors.LegRightDistal)
                || !ColorUtil.ColorsNear(colors.HandLeft, colors.HandLeftDistal)
                || !ColorUtil.ColorsNear(colors.HandRight, colors.HandRightDistal);
        }

        private static string BuildBodyPayload(string[] parts)
        {
            if (parts.Length < 9)
            {
                return string.Empty;
            }

            return parts[0] + "|" + parts[1] + "|" + parts[2] + "|" + parts[3] + "|"
                + parts[4] + "|" + parts[5] + "|" + parts[6] + "|" + parts[7] + "|" + parts[8];
        }
    }
}
