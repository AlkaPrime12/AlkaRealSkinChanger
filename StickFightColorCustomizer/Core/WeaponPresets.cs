using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Presets que tocan SOLO el arma (no cuerpo ni glow). Para tab Weapons.
    /// </summary>
    public static class WeaponPresets
    {
        public static readonly string[] All =
        {
            "Fire", "Ice", "Neon", "Gold", "Shadow", "Royal"
        };

        public static void Apply(WeaponColorSettings weapon, string presetId)
        {
            if (weapon == null || string.IsNullOrEmpty(presetId))
            {
                return;
            }

            weapon.Enabled = true;
            weapon.TintMesh = true;
            weapon.TintParticles = true;
            weapon.NeonEnabled = false;

            switch (presetId)
            {
                case "Fire":
                    weapon.Color = new Color(1f, 0.45f, 0.1f);
                    break;
                case "Ice":
                    weapon.Color = new Color(0.5f, 0.9f, 1f);
                    break;
                case "Neon":
                    weapon.Color = new Color(1f, 0.1f, 0.9f);
                    weapon.NeonEnabled = true;
                    break;
                case "Gold":
                    weapon.Color = new Color(1f, 0.84f, 0.2f);
                    break;
                case "Shadow":
                    weapon.Color = new Color(0.25f, 0.2f, 0.32f);
                    break;
                case "Royal":
                    weapon.Color = new Color(0.55f, 0.2f, 0.85f);
                    break;
                default:
                    weapon.Color = Color.white;
                    break;
            }
        }
    }
}
