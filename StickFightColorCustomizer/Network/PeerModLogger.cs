using System.Collections.Generic;
using StickFightColorCustomizer.Core;

namespace StickFightColorCustomizer.Network
{
    internal static class PeerModLogger
    {
        private static readonly HashSet<ulong> LoggedVanillaPeers = new HashSet<ulong>();
        private static readonly HashSet<ulong> LoggedModPeers = new HashSet<ulong>();

        public static void LogVanillaOnce(ulong steamId)
        {
            if (steamId == 0 || LoggedVanillaPeers.Contains(steamId))
            {
                return;
            }

            LoggedVanillaPeers.Add(steamId);
            ModLog.Info("SFCC: peer " + steamId + " sin SFCC (vanilla u otro mod) — ignorado, color por defecto");
        }

        public static void LogModOnce(ulong steamId)
        {
            if (steamId == 0 || LoggedModPeers.Contains(steamId))
            {
                return;
            }

            LoggedModPeers.Add(steamId);
            ModLog.Info("SFCC: peer " + steamId + " usa SFCC — sync activado");
        }

        public static void Clear()
        {
            LoggedVanillaPeers.Clear();
            LoggedModPeers.Clear();
        }
    }
}
