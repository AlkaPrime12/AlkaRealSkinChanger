using StickFightColorCustomizer.Models;
using StickFightColorCustomizer.Network;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Solo cosmética local + metadatos de lobby Steam. Sin paquetes P2P extra ni tocar materiales de red.
    /// </summary>
    public static class MultiplayerCompat
    {
        public static bool IsSafeMode(ColorConfig config)
        {
            return config == null || config.SafeVanillaMultiplayer;
        }

        public static bool IsNetworkMatch()
        {
            return MatchmakingHandler.IsNetworkMatch;
        }

        public static bool IsInLobby()
        {
            return SteamLobbyColorSync.CanSyncLobbyColors();
        }

        /// <summary>
        /// Piggyback en Ping vanilla (bytes extra ignorados por clientes sin mod). Siempre activo.
        /// </summary>
        public static bool AllowPingColorPackets(ColorConfig config)
        {
            return true;
        }

        /// <summary>
        /// RGB animado + sync automático satura lobby/P2P; en MP seguro solo se pinta en local.
        /// </summary>
        public static bool AllowAutoColorPublish(ColorConfig config)
        {
            if (!IsSafeMode(config))
            {
                return true;
            }

            return !IsNetworkMatch() && !IsInLobby();
        }

        /// <summary>
        /// Publicar sfcc al lobby durante RGB en partida MP segura (throttled en ColorCustomizerMod).
        /// </summary>
        public static bool ShouldPublishRgbToLobby(ColorConfig config)
        {
            return config != null && config.AnimatedRgb && IsInLobby();
        }

        /// <summary>
        /// m_Colors del MultiplayerManager participa en spawn en red; no tocar en partida.
        /// </summary>
        public static bool AllowNetworkManagerSlotPatch(ColorConfig config)
        {
            if (IsSafeMode(config) && (IsNetworkMatch() || IsInLobby()))
            {
                return false;
            }

            return !IsNetworkMatch();
        }
    }
}
