using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class StylePresets
    {
        public static readonly string[] All = { "Gold", "Neon", "Fire", "Ice", "Shadow", "Royal" };

        public static void ApplyFullStyle(ColorConfig config, string presetId)
        {
            if (config == null || string.IsNullOrEmpty(presetId))
            {
                return;
            }

            switch (presetId)
            {
                case "Gold":
                    ApplyGold(config);
                    break;
                case "Neon":
                    ApplyNeon(config);
                    break;
                default:
                    ColorPresets.ApplyBodyOnly(config, presetId);
                    config.Glow = ColorPresets.GetGlowPreset(presetId);
                    config.Glow.Enabled = presetId == "Neon" || presetId == "Fire" || presetId == "Ice" || presetId == "Royal";
                    if (config.Weapon == null)
                    {
                        config.Weapon = new WeaponColorSettings();
                    }
                    break;
            }

            config.ActivePreset = presetId;
            config.BodyCustomizationActive = true;
        }

        private static void ApplyGold(ColorConfig config)
        {
            if (config.Weapon == null)
            {
                config.Weapon = new WeaponColorSettings();
            }

            Color gold = new Color(1f, 0.84f, 0.2f);
            Color amber = new Color(0.95f, 0.65f, 0.1f);
            config.Colors = new BodyColors
            {
                Head = gold,
                Spine = amber,
                LegLeft = amber,
                LegRight = amber,
                HandLeft = gold,
                HandRight = gold,
                Crown = new Color(1f, 0.92f, 0.4f),
                Wings = amber
            };
            config.Glow = new GlowSettings();
            GlowPresetTuning.ApplySoftPreset(config.Glow, new Color(1f, 0.88f, 0.4f), 0.48f);
            config.Weapon.Enabled = true;
            config.Weapon.Color = gold;
            config.Weapon.TintMesh = true;
            config.Weapon.TintParticles = true;
            config.Weapon.NeonEnabled = false;
        }

        private static void ApplyNeon(ColorConfig config)
        {
            if (config.Weapon == null)
            {
                config.Weapon = new WeaponColorSettings();
            }

            ColorPresets.ApplyBodyOnly(config, "Neon");
            config.Glow = ColorPresets.GetGlowPreset("Neon");
            GlowPresetTuning.ApplySoftPreset(config.Glow, new Color(0.2f, 1f, 0.95f), 0.58f);
            config.Weapon.Enabled = true;
            config.Weapon.Color = new Color(1f, 0.1f, 0.9f);
            config.Weapon.TintMesh = true;
            config.Weapon.TintParticles = true;
            config.Weapon.NeonEnabled = true;
        }
    }
}
