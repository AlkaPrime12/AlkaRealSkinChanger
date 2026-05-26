using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Network
{
    public static class ColorBroadcastService
    {
        private static float _lastPublishTime;
        private const float PublishCooldown = 0.35f;
        public const float RgbLobbyPublishCooldown = 0.65f;

        public static void PublishIfNeeded(BodyColors colors, bool force = false)
        {
            if (colors == null)
            {
                return;
            }

            if (ColorCustomizerApp.Instance != null
                && !ModFeatureGate.IsBodyActive(ColorCustomizerApp.Instance.Config))
            {
                return;
            }

            if (!force && Time.realtimeSinceStartup - _lastPublishTime < PublishCooldown)
            {
                return;
            }

            _lastPublishTime = Time.realtimeSinceStartup;
            SteamLobbyColorSync.PublishLocal(colors);
            ModColorPingSync.BroadcastColorsThrottled(colors, force);
        }

        /// <summary>
        /// RGB animado en MP seguro: solo lobby member data, sin P2P ni refresh masivo.
        /// </summary>
        public static void PublishLobbyOnly(BodyColors colors)
        {
            if (colors == null)
            {
                return;
            }

            SteamLobbyColorSync.PublishLocal(colors);
        }
    }
}
