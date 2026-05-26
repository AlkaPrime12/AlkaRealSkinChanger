using UnityEngine;
using StickFightColorCustomizer.Models;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Computes a time-modulated copy of GlowSettings according to its Style.
    /// The renderer itself stays untouched — we just hand it a tweaked GlowSettings.
    /// </summary>
    public static class GlowStyleModulator
    {
        // Cached scratch instance to avoid allocations per-frame.
        private static readonly GlowSettings _scratch = new GlowSettings();
        private static float _lastFlickerTime;
        private static float _flickerValue;

        public static GlowSettings GetEffective(GlowSettings src)
        {
            if (src == null) return null;
            if (src.Style == GlowStyleKind.Solid) return src;   // hot path, no work

            // Copy the source into the scratch buffer.
            _scratch.Enabled         = src.Enabled;
            _scratch.Strength        = src.Strength;
            _scratch.Color           = src.Color;
            _scratch.AuraWidth       = src.AuraWidth;
            _scratch.AuraAlpha       = src.AuraAlpha;
            _scratch.OnHead          = src.OnHead;
            _scratch.OnTorso         = src.OnTorso;
            _scratch.OnArms          = src.OnArms;
            _scratch.OnLegs          = src.OnLegs;
            _scratch.OnWings         = src.OnWings;
            _scratch.MaintainInLobby = src.MaintainInLobby;
            _scratch.BodyColorBlend  = src.BodyColorBlend;
            _scratch.Style           = src.Style;

            float t = Time.realtimeSinceStartup;
            switch (src.Style)
            {
                case GlowStyleKind.Pulse:
                {
                    // Smooth breathing: alpha oscillates 0.55× → 1.25× of source alpha.
                    float pulse = (Mathf.Sin(t * 3.4f) + 1f) * 0.5f;     // 0..1
                    float scale = Mathf.Lerp(0.55f, 1.25f, pulse);
                    _scratch.AuraAlpha = Mathf.Clamp01(src.AuraAlpha * scale);
                    _scratch.AuraWidth = src.AuraWidth * Mathf.Lerp(0.95f, 1.15f, pulse);
                    break;
                }
                case GlowStyleKind.Rainbow:
                {
                    // Hue cycles over 6 s. Keep saturation/value from the source colour so
                    // dark glows stay dark, bright stay bright.
                    Color src2 = src.Color;
                    float h, s, v;
                    Color.RGBToHSV(src2, out h, out s, out v);
                    h = (t / 6f) % 1f;
                    Color cyc = Color.HSVToRGB(h, Mathf.Max(s, 0.85f), Mathf.Max(v, 0.85f));
                    cyc.a = src2.a;
                    _scratch.Color = cyc;
                    break;
                }
                case GlowStyleKind.Flicker:
                {
                    // ~ 18 Hz random jitter on alpha (electric arc feel).
                    if (t - _lastFlickerTime > 0.055f)
                    {
                        _lastFlickerTime = t;
                        _flickerValue = Random.value;     // 0..1
                    }
                    float k = 0.55f + 0.95f * _flickerValue;   // 0.55..1.50
                    _scratch.AuraAlpha = Mathf.Clamp01(src.AuraAlpha * k);
                    // Slight width jitter too.
                    _scratch.AuraWidth = src.AuraWidth * (0.85f + 0.30f * _flickerValue);
                    break;
                }
                case GlowStyleKind.Trail:
                {
                    // Slow strong pulse for a comet/flame look: width swings further than alpha.
                    float ph = (Mathf.Sin(t * 1.2f) + 1f) * 0.5f;
                    _scratch.AuraWidth = src.AuraWidth * Mathf.Lerp(0.70f, 1.65f, ph);
                    _scratch.AuraAlpha = Mathf.Clamp01(src.AuraAlpha * Mathf.Lerp(0.80f, 1.20f, ph));
                    break;
                }
                case GlowStyleKind.Heartbeat:
                {
                    // Two quick beats + rest. Period 1.1 s.
                    float tt = (t * 0.9f) % 1f;
                    float beat = 0f;
                    if (tt < 0.10f)      beat = Mathf.Sin(tt / 0.10f * Mathf.PI);
                    else if (tt < 0.20f) beat = 0f;
                    else if (tt < 0.30f) beat = Mathf.Sin((tt - 0.20f) / 0.10f * Mathf.PI) * 0.75f;
                    float scale = 0.55f + 0.95f * beat;     // 0.55..1.50
                    _scratch.AuraAlpha = Mathf.Clamp01(src.AuraAlpha * scale);
                    _scratch.AuraWidth = src.AuraWidth * (0.95f + 0.20f * beat);
                    break;
                }
                case GlowStyleKind.Strobe:
                {
                    // 6 Hz pulse — never fully off (min 15%) to avoid looking like a rendering bug.
                    bool on = (Mathf.FloorToInt(t * 12f) & 1) == 0;
                    float scale = on ? 1.25f : 0.15f;
                    _scratch.AuraAlpha = Mathf.Clamp01(src.AuraAlpha * scale);
                    break;
                }
                case GlowStyleKind.Sunset:
                {
                    // Hue oscillates between 0.03 (orange) and 0.92 (pink-violet) at 0.18 Hz.
                    float ph = (Mathf.Sin(t * 1.1f) + 1f) * 0.5f;
                    float hue = Mathf.Lerp(0.03f, 0.92f, ph);
                    _scratch.Color = Color.HSVToRGB(hue, 0.90f, 1f);
                    _scratch.Color.a = src.Color.a;
                    break;
                }
                case GlowStyleKind.Toxic:
                {
                    // Toggle between toxic green and toxic amber + width glitch.
                    bool green = (Mathf.FloorToInt(t * 2f) & 1) == 0;
                    Color c = green ? new Color(0.40f, 1f, 0.10f, src.Color.a)
                                    : new Color(0.95f, 0.78f, 0.10f, src.Color.a);
                    _scratch.Color = c;
                    // Subtle "broken" jitter.
                    float jitter = (Mathf.PerlinNoise(t * 7f, 0f) - 0.5f) * 0.4f;
                    _scratch.AuraWidth = src.AuraWidth * (1f + jitter);
                    break;
                }
                case GlowStyleKind.Aurora:
                {
                    // Aurora: smooth wander between green, cyan, violet.
                    float s1 = Mathf.Sin(t * 0.6f) * 0.5f + 0.5f;
                    float s2 = Mathf.Sin(t * 0.6f + 2.1f) * 0.5f + 0.5f;
                    float r = 0.20f * s2;
                    float g = 0.55f + 0.45f * s1;
                    float b = 0.70f + 0.30f * (1f - s1);
                    _scratch.Color = new Color(r, g, b, src.Color.a);
                    _scratch.AuraAlpha = Mathf.Clamp01(src.AuraAlpha * (0.85f + 0.30f * s2));
                    break;
                }
            }

            return _scratch;
        }

        public static string GetLabel(GlowStyleKind k)
        {
            switch (k)
            {
                case GlowStyleKind.Pulse:     return "Pulse";
                case GlowStyleKind.Rainbow:   return "Rainbow";
                case GlowStyleKind.Flicker:   return "Flicker";
                case GlowStyleKind.Trail:     return "Trail";
                case GlowStyleKind.Heartbeat: return "Heartbeat";
                case GlowStyleKind.Strobe:    return "Strobe";
                case GlowStyleKind.Sunset:    return "Sunset";
                case GlowStyleKind.Toxic:     return "Toxic";
                case GlowStyleKind.Aurora:    return "Aurora";
                default: return "Solid";
            }
        }

        public static string GetTooltip(GlowStyleKind k)
        {
            switch (k)
            {
                case GlowStyleKind.Solid:     return "Steady aura (default)";
                case GlowStyleKind.Pulse:     return "Smooth breathing alpha";
                case GlowStyleKind.Rainbow:   return "Hue cycles through spectrum";
                case GlowStyleKind.Flicker:   return "Electric jitter";
                case GlowStyleKind.Trail:     return "Wide comet pulse";
                case GlowStyleKind.Heartbeat: return "Lub-dub double beat";
                case GlowStyleKind.Strobe:    return "Fast flash (soft floor)";
                case GlowStyleKind.Sunset:    return "Orange to pink-violet";
                case GlowStyleKind.Toxic:     return "Green / amber glitch";
                case GlowStyleKind.Aurora:    return "Green-cyan-violet waves";
                default: return "";
            }
        }

        public static readonly GlowStyleKind[] ClassicStyles =
        {
            GlowStyleKind.Solid,
            GlowStyleKind.Pulse,
            GlowStyleKind.Rainbow,
            GlowStyleKind.Flicker,
            GlowStyleKind.Trail
        };

        public static readonly GlowStyleKind[] EpicStyles =
        {
            GlowStyleKind.Heartbeat,
            GlowStyleKind.Strobe,
            GlowStyleKind.Sunset,
            GlowStyleKind.Toxic,
            GlowStyleKind.Aurora
        };

        public static GlowStyleKind[] AllStyles
        {
            get
            {
                return new[]
                {
                    GlowStyleKind.Solid,
                    GlowStyleKind.Pulse,
                    GlowStyleKind.Rainbow,
                    GlowStyleKind.Flicker,
                    GlowStyleKind.Trail,
                    GlowStyleKind.Heartbeat,
                    GlowStyleKind.Strobe,
                    GlowStyleKind.Sunset,
                    GlowStyleKind.Toxic,
                    GlowStyleKind.Aurora
                };
            }
        }
    }
}
