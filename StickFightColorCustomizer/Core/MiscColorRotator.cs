using UnityEngine;
using StickFightColorCustomizer.Hosting;
using StickFightColorCustomizer.Models;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Runs the Misc → "Rotate colours continuously" effect. Ticks at most once per
    /// configured interval (no allocations, no log spam).
    /// </summary>
    public static class MiscColorRotator
    {
        private static float _nextTickAt;

        public static void Tick()
        {
            if (ColorCustomizerApp.Instance == null) return;
            ColorConfig cfg = ColorCustomizerApp.Instance.Config;
            if (cfg == null || cfg.Misc == null || !cfg.Misc.RotateColorsContinuously) return;

            float now = Time.realtimeSinceStartup;
            if (now < _nextTickAt) return;
            _nextTickAt = now + Mathf.Max(0.25f, cfg.Misc.ColorRotateInterval);

            BodyColors b = cfg.Colors;
            if (b == null) return;
            b.Head      = RandomVivid();
            b.Spine     = RandomVivid();
            b.LegLeft   = RandomVivid();
            b.LegRight  = RandomVivid();
            b.HandLeft  = RandomVivid();
            b.HandRight = RandomVivid();

            ColorCustomizerApp.Instance.ApplyBodyOnly();
        }

        public static void ResetSchedule()
        {
            _nextTickAt = 0f;
        }

        private static Color RandomVivid()
        {
            return Color.HSVToRGB(
                Random.value,
                0.75f + Random.value * 0.25f,
                0.80f + Random.value * 0.20f);
        }
    }
}
