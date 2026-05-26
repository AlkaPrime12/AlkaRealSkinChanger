using System.Collections.Generic;

using StickFightColorCustomizer.Core;

using StickFightColorCustomizer.Models;



namespace StickFightColorCustomizer.Network

{

    public static class RemoteColorRegistry

    {

        private static readonly Dictionary<byte, BodyColors> ByPlayerId = new Dictionary<byte, BodyColors>();

        private static readonly Dictionary<ulong, BodyColors> BySteamId = new Dictionary<ulong, BodyColors>();

        private static readonly Dictionary<byte, int> HashByPlayerId = new Dictionary<byte, int>();

        private static readonly Dictionary<ulong, int> HashBySteamId = new Dictionary<ulong, int>();



        public static void Set(byte playerId, BodyColors colors)

        {

            if (colors == null)

            {

                return;

            }



            int hash = ComputeHash(colors);

            int prev;

            if (HashByPlayerId.TryGetValue(playerId, out prev) && prev == hash)

            {

                return;

            }



            HashByPlayerId[playerId] = hash;

            ByPlayerId[playerId] = colors.Clone();

        }



        public static void SetBySteam(ulong steamId, BodyColors colors)

        {

            if (colors == null || steamId == 0)

            {

                return;

            }



            int hash = ComputeHash(colors);

            int prev;

            if (HashBySteamId.TryGetValue(steamId, out prev) && prev == hash)

            {

                return;

            }



            HashBySteamId[steamId] = hash;

            BySteamId[steamId] = colors.Clone();

        }



        public static void SetBySteamEncoded(ulong steamId, string encoded, BodyColors colors)

        {

            if (colors == null || steamId == 0)

            {

                return;

            }



            int hash = encoded != null ? encoded.GetHashCode() : ComputeHash(colors);

            int prev;

            if (HashBySteamId.TryGetValue(steamId, out prev) && prev == hash)

            {

                return;

            }



            HashBySteamId[steamId] = hash;

            BySteamId[steamId] = colors.Clone();

        }



        public static bool TryGet(byte playerId, out BodyColors colors)

        {

            return ByPlayerId.TryGetValue(playerId, out colors);

        }



        public static bool TryGetBySteam(ulong steamId, out BodyColors colors)

        {

            return BySteamId.TryGetValue(steamId, out colors);

        }



        public static void RemoveSteam(ulong steamId)
        {
            if (steamId == 0)
            {
                return;
            }

            BySteamId.Remove(steamId);
            HashBySteamId.Remove(steamId);
            ModPresenceRegistry.Unmark(steamId);
        }

        public static void RemoveSlot(byte playerId)
        {
            ByPlayerId.Remove(playerId);
            HashByPlayerId.Remove(playerId);
        }

        public static void ClearPeerVanilla(ulong steamId)
        {
            if (steamId == 0)
            {
                return;
            }

            RemoveSteam(steamId);
            RemoteHatRegistry.RemoveSteam(steamId);
            RemoteShoeRegistry.RemoveSteam(steamId);
            RemoteObjectRegistry.RemoveSteam(steamId);
            byte slot;
            if (NetworkPlayerIndexResolver.TryGetSlot(steamId, out slot))
            {
                RemoveSlot(slot);
                RemoteHatRegistry.RemoveSlot(slot);
                RemoteShoeRegistry.RemoveSlot(slot);
                RemoteObjectRegistry.RemoveSlot(slot);
                PlayerSlotCache.InvalidatePlayer(slot);
            }
        }

        public static void Clear()

        {

            ByPlayerId.Clear();

            BySteamId.Clear();

            HashByPlayerId.Clear();

            HashBySteamId.Clear();

        }



        /// <summary>

        /// Restaura ModPresenceRegistry desde colores ya decodificados (p. ej. tras refresh merge en partida).

        /// </summary>

        public static void SyncModPresenceFromCache()

        {

            foreach (ulong steamId in BySteamId.Keys)

            {

                ModPresenceRegistry.Mark(steamId);

            }

        }



        private static int ComputeHash(BodyColors colors)

        {

            return ColorSyncCodec.Encode(colors).GetHashCode();

        }

    }

}


