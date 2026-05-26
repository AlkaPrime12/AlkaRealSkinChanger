using System.Collections.Generic;

namespace StickFightColorCustomizer.Network
{
    public static class RemoteHatRegistry
    {
        private static readonly Dictionary<ulong, string> BySteamId = new Dictionary<ulong, string>();
        private static readonly Dictionary<byte, string> ByPlayerId = new Dictionary<byte, string>();
        private static readonly Dictionary<ulong, int> HashBySteamId = new Dictionary<ulong, int>();

        public static void SetBySteam(ulong steamId, string hatId)
        {
            if (steamId == 0)
            {
                return;
            }

            hatId = HatSyncCodec.Decode(hatId);
            int hash = hatId != null ? hatId.GetHashCode() : 0;
            int prev;
            if (HashBySteamId.TryGetValue(steamId, out prev) && prev == hash)
            {
                return;
            }

            HashBySteamId[steamId] = hash;
            BySteamId[steamId] = hatId;
        }

        public static void Set(byte playerId, string hatId)
        {
            hatId = HatSyncCodec.Decode(hatId);
            ByPlayerId[playerId] = hatId;
        }

        public static bool TryGetBySteam(ulong steamId, out string hatId)
        {
            return BySteamId.TryGetValue(steamId, out hatId);
        }

        public static bool TryGet(byte playerId, out string hatId)
        {
            return ByPlayerId.TryGetValue(playerId, out hatId);
        }

        public static void RemoveSteam(ulong steamId)
        {
            if (steamId == 0)
            {
                return;
            }

            BySteamId.Remove(steamId);
            HashBySteamId.Remove(steamId);
        }

        public static void RemoveSlot(byte playerId)
        {
            ByPlayerId.Remove(playerId);
        }

        public static void Clear()
        {
            BySteamId.Clear();
            ByPlayerId.Clear();
            HashBySteamId.Clear();
        }
    }
}
