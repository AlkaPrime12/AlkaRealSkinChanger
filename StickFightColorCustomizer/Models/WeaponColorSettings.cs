using UnityEngine;

namespace StickFightColorCustomizer.Models
{
    public sealed class WeaponColorSettings
    {
        public bool Enabled;
        public Color Color = Color.white;
        public bool TintMesh = true;
        public bool TintParticles = true;
        public bool NeonEnabled;

        public WeaponColorSettings Clone()
        {
            return new WeaponColorSettings
            {
                Enabled = Enabled,
                Color = Color,
                TintMesh = TintMesh,
                TintParticles = TintParticles,
                NeonEnabled = NeonEnabled
            };
        }
    }
}
