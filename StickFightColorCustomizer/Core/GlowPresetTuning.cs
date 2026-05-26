using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class GlowPresetTuning
    {
        public static void ApplySoftPreset(GlowSettings glow, Color color, float strength = 0.5f)
        {
            if (glow == null)
            {
                return;
            }

            glow.Enabled = true;
            glow.Color = color;
            glow.Strength = Mathf.Clamp(strength, 0.35f, 0.65f);
            glow.AuraAlpha = Mathf.Clamp(0.2f + glow.Strength * 0.12f, 0.18f, 0.32f);
            glow.AuraWidth = Mathf.Clamp(1.25f + glow.Strength * 0.2f, 1.2f, 1.65f);
            glow.OnHead = true;
            glow.OnTorso = true;
            glow.OnArms = true;
            glow.OnLegs = true;
        }
    }
}
