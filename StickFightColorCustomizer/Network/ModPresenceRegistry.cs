using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Jugadores con payload sfcc válido (lobby), sfcc_ok pendiente, o ping magic (P2P).
    /// </summary>
    public static class ModPresenceRegistry
    {
        private static readonly HashSet<ulong> WithMod = new HashSet<ulong>();
        private static readonly Dictionary<ulong, float> PendingSince = new Dictionary<ulong, float>();

        public const float PendingGraceSeconds = 15f;

        public static int Count
        {
            get { return WithMod.Count; }
        }

        public static int PendingCount
        {
            get { return PendingSince.Count; }
        }

        public static void Mark(ulong steamId)
        {
            if (steamId == 0)
            {
                return;
            }

            WithMod.Add(steamId);
            PendingSince.Remove(steamId);
        }

        public static void MarkPending(ulong steamId)
        {
            if (steamId == 0 || WithMod.Contains(steamId))
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (!PendingSince.ContainsKey(steamId))
            {
                PendingSince[steamId] = now;
            }
        }

        public static bool IsPending(ulong steamId)
        {
            if (steamId == 0 || !PendingSince.ContainsKey(steamId))
            {
                return false;
            }

            float started;
            if (!PendingSince.TryGetValue(steamId, out started))
            {
                return false;
            }

            if (Time.realtimeSinceStartup - started > PendingGraceSeconds)
            {
                PendingSince.Remove(steamId);
                return false;
            }

            return true;
        }

        public static void Unmark(ulong steamId)
        {
            if (steamId != 0)
            {
                WithMod.Remove(steamId);
                PendingSince.Remove(steamId);
            }
        }

        public static bool HasMod(ulong steamId)
        {
            return steamId != 0 && WithMod.Contains(steamId);
        }

        public static void Clear()
        {
            WithMod.Clear();
            PendingSince.Clear();
        }

        public static void TickPendingGrace()
        {
            if (PendingSince.Count == 0)
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            var expired = new List<ulong>();
            foreach (KeyValuePair<ulong, float> kv in PendingSince)
            {
                if (now - kv.Value > PendingGraceSeconds)
                {
                    expired.Add(kv.Key);
                }
            }

            for (int i = 0; i < expired.Count; i++)
            {
                ulong steamId = expired[i];
                PendingSince.Remove(steamId);
                if (!WithMod.Contains(steamId))
                {
                    MatchEntryColorScheduler.MarkKnownNoMod(steamId);
                    PeerModLogger.LogVanillaOnce(steamId);
                }
            }
        }
    }
}
