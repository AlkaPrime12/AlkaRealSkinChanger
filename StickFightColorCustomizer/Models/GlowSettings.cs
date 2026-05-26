using UnityEngine;

namespace StickFightColorCustomizer.Models
{
    /// <summary>
    /// Built-in glow visual styles. The base renderer logic is untouched — a small
    /// modulator computes a per-frame "effective" GlowSettings before handing it off.
    /// </summary>
    public enum GlowStyleKind
    {
        Solid    = 0,    // steady, original behaviour
        Pulse    = 1,    // alpha breathes with a sine wave
        Rainbow  = 2,    // hue cycles through the spectrum
        Flicker  = 3,    // fast random brightness jitter (electric)
        Trail    = 4,    // periodic width pulse (comet/flame trail)
        // ── 5 nuevos estilos épicos (no rompen el rendering) ──
        Heartbeat = 5,   // dos pulsos rápidos seguidos de pausa (lub-dub)
        Strobe    = 6,   // on/off duro a ~6 Hz
        Sunset    = 7,   // hue oscila entre naranja y rosa-violeta
        Toxic     = 8,   // verde/ámbar alternados + leve glitch de ancho
        Aurora    = 9    // tres hues mezclados (verde-cyan-violeta) que ondean
    }

    public sealed class GlowSettings
    {
        public bool Enabled;
        public float Strength = 0.5f;
        public Color Color = new Color(0.3f, 1f, 1f, 1f);
        public float AuraWidth = 1.4f;
        public float AuraAlpha = 0.26f;
        public bool OnHead = true;
        public bool OnTorso = true;
        public bool OnArms = true;
        public bool OnLegs = true;
        public bool OnWings;
        public bool MaintainInLobby;

        /// <summary>0 = solo color glow; 1 = mezcla fuerte con colores del cuerpo en el aura.</summary>
        public float BodyColorBlend = 0.25f;

        /// <summary>Visual style (drives the per-frame modulator).</summary>
        public GlowStyleKind Style = GlowStyleKind.Solid;

        public GlowSettings Clone()
        {
            return new GlowSettings
            {
                Enabled = Enabled,
                Strength = Strength,
                Color = Color,
                AuraWidth = AuraWidth,
                AuraAlpha = AuraAlpha,
                OnHead = OnHead,
                OnTorso = OnTorso,
                OnArms = OnArms,
                OnLegs = OnLegs,
                OnWings = OnWings,
                MaintainInLobby = MaintainInLobby,
                BodyColorBlend = BodyColorBlend,
                Style = Style
            };
        }
    }
}
