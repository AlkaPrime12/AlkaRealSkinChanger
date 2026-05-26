using System;

using System.IO;

using System.Text;

using StickFightColorCustomizer.Models;

using UnityEngine;



namespace StickFightColorCustomizer.Core

{

    public static class ColorConfigStore

    {

        private const string ConfigDirName = "ColorCustomizer";

        private const string ConfigFileName = "config.json";



        private static string UserDataRoot

        {

            get { return ModStoragePaths.ColorCustomizerRoot; }

        }



        public static ColorConfig Load()

        {

            string path = ModStoragePaths.ConfigFilePath;

            if (!File.Exists(path))

            {

                return new ColorConfig();

            }



            try

            {

                string json = File.ReadAllText(path, Encoding.UTF8);

                return Parse(json);

            }

            catch (Exception ex)

            {

                ModLog.Warning("No se pudo leer config: " + ex.Message);

                return new ColorConfig();

            }

        }



        public static string SerializeToJson(ColorConfig config)
        {
            return Serialize(config);
        }

        public static ColorConfig ParseFromJson(string json)
        {
            return Parse(json);
        }

        public static void Save(ColorConfig config)

        {

            ModStoragePaths.EnsureColorCustomizerDirectory();

            string path = ModStoragePaths.ConfigFilePath;

            File.WriteAllText(path, Serialize(config), Encoding.UTF8);

            ModLog.Info("Config guardada en " + path);

        }



        private static string Serialize(ColorConfig config)

        {

            var sb = new StringBuilder();

            sb.AppendLine("{");

            sb.AppendLine("  \"head\": \"" + ColorUtil.ToHex(config.Colors.Head) + "\",");

            sb.AppendLine("  \"spine\": \"" + ColorUtil.ToHex(config.Colors.Spine) + "\",");

            sb.AppendLine("  \"legLeft\": \"" + ColorUtil.ToHex(config.Colors.LegLeft) + "\",");

            sb.AppendLine("  \"legRight\": \"" + ColorUtil.ToHex(config.Colors.LegRight) + "\",");

            sb.AppendLine("  \"handLeft\": \"" + ColorUtil.ToHex(config.Colors.HandLeft) + "\",");

            sb.AppendLine("  \"handRight\": \"" + ColorUtil.ToHex(config.Colors.HandRight) + "\",");

            sb.AppendLine("  \"crown\": \"" + ColorUtil.ToHex(config.Colors.Crown) + "\",");

            sb.AppendLine("  \"wings\": \"" + ColorUtil.ToHex(config.Colors.Wings) + "\",");

            sb.AppendLine("  \"halfColorEnabled\": " + (config.Colors.HalfColorEnabled ? "true" : "false") + ",");

            sb.AppendLine("  \"spineDistal\": \"" + ColorUtil.ToHex(config.Colors.SpineDistal) + "\",");

            sb.AppendLine("  \"legLeftDistal\": \"" + ColorUtil.ToHex(config.Colors.LegLeftDistal) + "\",");

            sb.AppendLine("  \"legRightDistal\": \"" + ColorUtil.ToHex(config.Colors.LegRightDistal) + "\",");

            sb.AppendLine("  \"handLeftDistal\": \"" + ColorUtil.ToHex(config.Colors.HandLeftDistal) + "\",");

            sb.AppendLine("  \"handRightDistal\": \"" + ColorUtil.ToHex(config.Colors.HandRightDistal) + "\",");

            SerializeGlow(sb, config.Glow);

            HatSettings hat = config.Hat ?? new HatSettings();
            sb.AppendLine("  \"bodyCustomizationActive\": " + (config.BodyCustomizationActive ? "true" : "false") + ",");
            sb.AppendLine("  \"hatEnabled\": " + (hat.Enabled ? "true" : "false") + ",");
            sb.AppendLine("  \"hatId\": \"" + Escape(hat.HatId ?? "none") + "\",");

            ShoeSettings shoe = config.Shoe ?? new ShoeSettings();
            sb.AppendLine("  \"shoeEnabled\": " + (shoe.Enabled ? "true" : "false") + ",");
            sb.AppendLine("  \"shoeId\": \"" + Escape(shoe.ShoeId ?? "none") + "\",");

            WingSettings wing = config.Wing ?? new WingSettings();
            sb.AppendLine("  \"wingEnabled\": " + (wing.Enabled ? "true" : "false") + ",");
            sb.AppendLine("  \"wingStyleId\": \"" + Escape(wing.StyleId ?? "minigame") + "\",");
            sb.AppendLine("  \"wingScale\": " + wing.Scale.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");

            ObjectSettings obj = config.Object ?? new ObjectSettings();
            sb.AppendLine("  \"objectEnabled\": " + (obj.Enabled ? "true" : "false") + ",");
            sb.AppendLine("  \"objectId\": \"" + Escape(obj.ObjectId ?? "none") + "\",");
            sb.AppendLine("  \"objectScale\": " + obj.Scale.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");

            WeaponColorSettings weapon = config.Weapon ?? new WeaponColorSettings();
            sb.AppendLine("  \"weaponEnabled\": " + (weapon.Enabled ? "true" : "false") + ",");
            sb.AppendLine("  \"weaponColor\": \"" + ColorUtil.ToHex(weapon.Color) + "\",");
            sb.AppendLine("  \"weaponTintMesh\": " + (weapon.TintMesh ? "true" : "false") + ",");
            sb.AppendLine("  \"weaponTintParticles\": " + (weapon.TintParticles ? "true" : "false") + ",");
            sb.AppendLine("  \"weaponNeonEnabled\": " + (weapon.NeonEnabled ? "true" : "false") + ",");

            sb.AppendLine("  \"menuLanguage\": \"" + (config.MenuLanguage == MenuLanguage.Spanish ? "es" : "en") + "\",");
            sb.AppendLine("  \"menuWindowWidth\": " + config.MenuWindowWidth.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"menuWindowHeight\": " + config.MenuWindowHeight.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"safeVanillaMultiplayer\": " + (config.SafeVanillaMultiplayer ? "true" : "false") + ",");
            sb.AppendLine("  \"uniformSkin\": " + (config.UseUniformSkin ? "true" : "false") + ",");

            sb.AppendLine("  \"animatedRgb\": " + (config.AnimatedRgb ? "true" : "false") + ",");

            sb.AppendLine("  \"rgbSpeed\": " + config.RgbSpeed.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");

            sb.AppendLine("  \"activePreset\": \"" + Escape(config.ActivePreset) + "\"");

            sb.AppendLine("}");

            return sb.ToString();

        }



        private static void SerializeGlow(StringBuilder sb, GlowSettings glow)

        {

            if (glow == null)

            {

                glow = new GlowSettings();

            }



            sb.AppendLine("  \"glowEnabled\": " + (glow.Enabled ? "true" : "false") + ",");

            sb.AppendLine("  \"glowStrength\": " + glow.Strength.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");

            sb.AppendLine("  \"glowColor\": \"" + ColorUtil.ToHex(glow.Color) + "\",");

            sb.AppendLine("  \"glowAuraWidth\": " + glow.AuraWidth.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");

            sb.AppendLine("  \"glowAuraAlpha\": " + glow.AuraAlpha.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");

            sb.AppendLine("  \"glowOnHead\": " + (glow.OnHead ? "true" : "false") + ",");

            sb.AppendLine("  \"glowOnTorso\": " + (glow.OnTorso ? "true" : "false") + ",");

            sb.AppendLine("  \"glowOnArms\": " + (glow.OnArms ? "true" : "false") + ",");

            sb.AppendLine("  \"glowOnLegs\": " + (glow.OnLegs ? "true" : "false") + ",");

            sb.AppendLine("  \"glowOnWings\": " + (glow.OnWings ? "true" : "false") + ",");

            sb.AppendLine("  \"glowMaintainInLobby\": " + (glow.MaintainInLobby ? "true" : "false") + ",");
            sb.AppendLine("  \"glowBodyColorBlend\": " + glow.BodyColorBlend.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"glowStyle\": \"" + Escape(glow.Style.ToString()) + "\",");

        }



        private static ColorConfig Parse(string json)

        {

            var config = new ColorConfig();

            config.Colors.Head = ReadHex(json, "head", config.Colors.Head);

            config.Colors.Spine = ReadHex(json, "spine", config.Colors.Spine);

            config.Colors.LegLeft = ReadHex(json, "legLeft", ReadHex(json, "legs", config.Colors.LegLeft));

            config.Colors.LegRight = ReadHex(json, "legRight", config.Colors.LegLeft);

            config.Colors.HandLeft = ReadHex(json, "handLeft", ReadHex(json, "hands", config.Colors.HandLeft));

            config.Colors.HandRight = ReadHex(json, "handRight", config.Colors.HandLeft);

            config.Colors.Crown = ReadHex(json, "crown", config.Colors.Crown);

            config.Colors.Wings = ReadHex(json, "wings", config.Colors.Wings);

            config.Colors.HalfColorEnabled = ReadBool(json, "halfColorEnabled", false);

            config.Colors.SpineDistal = ReadHex(json, "spineDistal", config.Colors.Spine);

            config.Colors.LegLeftDistal = ReadHex(json, "legLeftDistal", config.Colors.LegLeft);

            config.Colors.LegRightDistal = ReadHex(json, "legRightDistal", config.Colors.LegRight);

            config.Colors.HandLeftDistal = ReadHex(json, "handLeftDistal", config.Colors.HandLeft);

            config.Colors.HandRightDistal = ReadHex(json, "handRightDistal", config.Colors.HandRight);

            ParseGlow(json, config);
            ParseHat(json, config);
            ParseShoe(json, config);
            ParseWing(json, config);
            ParseObject(json, config);
            ParseWeapon(json, config);

            bool hasBodyFlag = json.IndexOf("\"bodyCustomizationActive\"", StringComparison.OrdinalIgnoreCase) >= 0;
            if (hasBodyFlag)
            {
                config.BodyCustomizationActive = ReadBool(json, "bodyCustomizationActive", false);
            }
            else
            {
                config.BodyCustomizationActive = InferBodyCustomizationFromLegacy(config, json);
            }

            config.SafeVanillaMultiplayer = ReadBool(json, "safeVanillaMultiplayer", true);
            config.UseUniformSkin = ReadBool(json, "uniformSkin", false);

            config.AnimatedRgb = ReadBool(json, "animatedRgb", false);

            config.RgbSpeed = ReadFloat(json, "rgbSpeed", 1f);

            config.ActivePreset = ReadString(json, "activePreset", "Custom");

            string lang = ReadString(json, "menuLanguage", "en");
            config.MenuLanguage = string.Equals(lang, "es", StringComparison.OrdinalIgnoreCase)
                ? MenuLanguage.Spanish
                : MenuLanguage.English;

            config.MenuWindowWidth = Mathf.Clamp(ReadFloat(json, "menuWindowWidth", 540f), 380f, 920f);
            config.MenuWindowHeight = Mathf.Clamp(ReadFloat(json, "menuWindowHeight", 680f), 500f, 950f);

            return config;

        }



        private static void ParseHat(string json, ColorConfig config)
        {
            HatSettings h = config.Hat ?? new HatSettings();
            h.Enabled = ReadBool(json, "hatEnabled", false);
            h.HatId = HatCatalog.Normalize(ReadString(json, "hatId", "none"));
            config.Hat = h;
        }

        private static void ParseShoe(string json, ColorConfig config)
        {
            ShoeSettings s = config.Shoe ?? new ShoeSettings();
            s.Enabled = ReadBool(json, "shoeEnabled", false);
            s.ShoeId = ShoeCatalog.Normalize(ReadString(json, "shoeId", "none"));
            config.Shoe = s;
        }

        private static void ParseWing(string json, ColorConfig config)
        {
            WingSettings w = config.Wing ?? new WingSettings();
            w.Enabled = ReadBool(json, "wingEnabled", false);
            w.StyleId = WingCatalog.Normalize(ReadString(json, "wingStyleId", "minigame"));
            w.Scale = ReadFloat(json, "wingScale", w.Scale > 0.01f ? w.Scale : 0.38f);
            config.Wing = w;
        }

        private static void ParseObject(string json, ColorConfig config)
        {
            ObjectSettings o = config.Object ?? new ObjectSettings();
            o.Enabled = ReadBool(json, "objectEnabled", false);
            o.ObjectId = ObjectsCatalog.Normalize(ReadString(json, "objectId", "none"));
            o.Scale = ReadFloat(json, "objectScale", 1f);
            config.Object = o;
        }

        private static void ParseWeapon(string json, ColorConfig config)
        {
            WeaponColorSettings w = config.Weapon ?? new WeaponColorSettings();
            w.Enabled = ReadBool(json, "weaponEnabled", false);
            w.Color = ReadHex(json, "weaponColor", w.Color);
            w.TintMesh = ReadBool(json, "weaponTintMesh", true);
            w.TintParticles = ReadBool(json, "weaponTintParticles", true);
            w.NeonEnabled = ReadBool(json, "weaponNeonEnabled", false);
            config.Weapon = w;
        }

        private static bool InferBodyCustomizationFromLegacy(ColorConfig config, string json)
        {
            if (config.AnimatedRgb)
            {
                return true;
            }

            if (config.Glow != null && config.Glow.Enabled)
            {
                return true;
            }

            if (config.UseUniformSkin)
            {
                return true;
            }

            if (config.Colors != null && config.Colors.HalfColorEnabled)
            {
                return true;
            }

            if (!string.Equals(config.ActivePreset, "Custom", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(config.ActivePreset))
            {
                return true;
            }

            if (json.IndexOf("\"head\"", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Color white = Color.white;
                if (!ColorsNear(config.Colors.Head, white)
                    || !ColorsNear(config.Colors.Spine, white))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ColorsNear(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.02f
                && Mathf.Abs(a.g - b.g) < 0.02f
                && Mathf.Abs(a.b - b.b) < 0.02f;
        }

        private static void ParseGlow(string json, ColorConfig config)

        {

            GlowSettings g = config.Glow ?? new GlowSettings();

            bool hasEnabled = json.IndexOf("\"glowEnabled\"", StringComparison.OrdinalIgnoreCase) >= 0;
            if (hasEnabled)
            {
                g.Enabled = ReadBool(json, "glowEnabled", false);
            }
            else
            {
                float legacy = ReadFloat(json, "glow", 0f);
                g.Enabled = legacy > 0.01f;
                if (legacy > 0.01f)
                {
                    g.Strength = legacy;
                }
            }

            g.Strength = ReadFloat(json, "glowStrength", g.Strength);

            g.Color = ReadHex(json, "glowColor", g.Color);

            g.AuraWidth = ReadFloat(json, "glowAuraWidth", g.AuraWidth);

            g.AuraAlpha = ReadFloat(json, "glowAuraAlpha", g.AuraAlpha);

            g.OnHead = ReadBool(json, "glowOnHead", true);

            g.OnTorso = ReadBool(json, "glowOnTorso", true);

            g.OnArms = ReadBool(json, "glowOnArms", true);

            g.OnLegs = ReadBool(json, "glowOnLegs", true);

            g.OnWings = ReadBool(json, "glowOnWings", false);

            g.MaintainInLobby = ReadBool(json, "glowMaintainInLobby", false);
            g.BodyColorBlend = ReadFloat(json, "glowBodyColorBlend", g.BodyColorBlend);
            g.Style = ParseGlowStyle(ReadString(json, "glowStyle", "Solid"));

            config.Glow = g;

        }

        private static GlowStyleKind ParseGlowStyle(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return GlowStyleKind.Solid;
            }

            try
            {
                return (GlowStyleKind)Enum.Parse(typeof(GlowStyleKind), raw, true);
            }
            catch
            {
                return GlowStyleKind.Solid;
            }
        }



        private static Color ReadHex(string json, string key, Color fallback)

        {

            string value = ReadString(json, key, null);

            Color parsed;

            if (value != null && ColorUtil.TryParseHex(value, out parsed))

            {

                return parsed;

            }



            return fallback;

        }



        private static string ReadString(string json, string key, string fallback)

        {

            string token = "\"" + key + "\"";

            int i = json.IndexOf(token, StringComparison.OrdinalIgnoreCase);

            if (i < 0)

            {

                return fallback;

            }



            int colon = json.IndexOf(':', i);

            if (colon < 0)

            {

                return fallback;

            }



            int quoteStart = json.IndexOf('"', colon + 1);

            if (quoteStart < 0)

            {

                return fallback;

            }



            int quoteEnd = json.IndexOf('"', quoteStart + 1);

            if (quoteEnd < 0)

            {

                return fallback;

            }



            return json.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);

        }



        private static bool ReadBool(string json, string key, bool fallback)

        {

            string token = "\"" + key + "\"";

            int i = json.IndexOf(token, StringComparison.OrdinalIgnoreCase);

            if (i < 0)

            {

                return fallback;

            }



            int colon = json.IndexOf(':', i);

            if (colon < 0)

            {

                return fallback;

            }



            string tail = json.Substring(colon + 1, Math.Min(12, json.Length - colon - 1)).Trim().ToLowerInvariant();

            if (tail.StartsWith("true"))

            {

                return true;

            }



            if (tail.StartsWith("false"))

            {

                return false;

            }



            return fallback;

        }



        private static float ReadFloat(string json, string key, float fallback)

        {

            string token = "\"" + key + "\"";

            int i = json.IndexOf(token, StringComparison.OrdinalIgnoreCase);

            if (i < 0)

            {

                return fallback;

            }



            int colon = json.IndexOf(':', i);

            if (colon < 0)

            {

                return fallback;

            }



            int end = colon + 1;

            while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '.' || json[end] == '-'))

            {

                end++;

            }



            string num = json.Substring(colon + 1, end - colon - 1).Trim();

            float parsed;

            if (float.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out parsed))

            {

                return parsed;

            }



            return fallback;

        }



        private static string Escape(string value)

        {

            return (value ?? "Custom").Replace("\\", "\\\\").Replace("\"", "\\\"");

        }

        /// <summary>
        /// Si existe config de MelonLoader y no hay config BepInEx, copia una vez.
        /// </summary>
        public static void TryMigrateFromMelonUserData()
        {
            string target = ModStoragePaths.ConfigFilePath;
            if (File.Exists(target))
            {
                return;
            }

            string gameRoot = Path.GetDirectoryName(Application.dataPath);
            string melonConfig = Path.Combine(
                Path.Combine(gameRoot, "UserData"),
                Path.Combine("ColorCustomizer", "config.json"));
            if (!File.Exists(melonConfig))
            {
                return;
            }

            try
            {
                ModStoragePaths.EnsureColorCustomizerDirectory();
                File.Copy(melonConfig, target, false);
                ModLog.Info("Config migrada desde MelonLoader UserData.");
            }
            catch (Exception ex)
            {
                ModLog.Warning("Migracion config ML: " + ex.Message);
            }
        }

    }

}


