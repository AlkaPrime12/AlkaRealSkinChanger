using UnityEngine;

namespace StickFightColorCustomizer.Models
{
    /// <summary>
    /// Catch-all bag of "miscellaneous" customisations.  Each field is independent of the
    /// other tabs (Body, Glow, Hat, etc.) — they layer on top.  Adding a new toggle/slider
    /// here is the recommended way to ship one-off cosmetic ideas without growing a brand
    /// new module.
    /// </summary>
    public sealed class MiscSettings
    {
        // ── Visibility ────────────────────────────────────────────────────────────
        /// <summary>Master kill-switch for ALL of our cosmetics (body recolour, glow,
        /// hat, shoes, tops, objects, wings, weapon tint). When false the local player
        /// renders 100% vanilla. Quick way to disable everything at once for a clean MP
        /// session without losing your saved config.</summary>
        public bool MasterCosmeticsEnabled = true;

        /// <summary>0..1 alpha multiplier applied to the stickman's body lines via the
        /// existing maintain-set-line path. 1 = fully solid (default), 0 = ghost mode.
        /// </summary>
        public float BodyOpacity = 1.0f;

        // ── Line tweaks ───────────────────────────────────────────────────────────
        /// <summary>Multiplier on each body line's widthMultiplier. 0.5..2.0. Cosmetic
        /// only — the physics rigidbody isn't resized.</summary>
        public float BodyLineWidth = 1.0f;

        // ── Auto-actions ──────────────────────────────────────────────────────────
        /// <summary>If true, every time the local player respawns the body colours are
        /// re-randomised (Crayon-style). Hats/shoes/tops are unaffected.</summary>
        public bool RandomColorsOnRespawn;

        /// <summary>If true, body colours randomise every N seconds while alive
        /// (party mode). N = ColorRotateInterval.</summary>
        public bool RotateColorsContinuously;
        public float ColorRotateInterval = 4f;

        // ── Vanity ────────────────────────────────────────────────────────────────
        /// <summary>Optional emoji or short text to show floating above the head as a
        /// "name tag". Empty = disabled.</summary>
        public string NameTag = "";
        public Color NameTagColor = new Color(1f, 1f, 1f, 1f);

        /// <summary>Spin the cosmetics on each respawn for a couple of seconds (extra
        /// flair, no gameplay impact).</summary>
        public bool SpawnSpinBurst = true;

        // ── Footprints / trail ────────────────────────────────────────────────────
        /// <summary>If true, draw a fading colour trail behind the local player.</summary>
        public bool TrailEnabled;
        public Color TrailColor = new Color(0.30f, 0.85f, 1f, 1f);

        public MiscSettings Clone()
        {
            return new MiscSettings
            {
                MasterCosmeticsEnabled   = MasterCosmeticsEnabled,
                BodyOpacity              = BodyOpacity,
                BodyLineWidth            = BodyLineWidth,
                RandomColorsOnRespawn    = RandomColorsOnRespawn,
                RotateColorsContinuously = RotateColorsContinuously,
                ColorRotateInterval      = ColorRotateInterval,
                NameTag                  = NameTag,
                NameTagColor             = NameTagColor,
                SpawnSpinBurst           = SpawnSpinBurst,
                TrailEnabled             = TrailEnabled,
                TrailColor               = TrailColor
            };
        }
    }
}
