using System.Collections.Generic;

namespace StickFightColorCustomizer.Network
{
    public static class RemoteObjectRegistry
    {
        private static readonly Dictionary<ulong, string> BySteam = new Dictionary<ulong, string>();
        private static readonly Dictionary<int, string> BySlot = new Dictionary<int, string>();

        public static void SetBySteam(ulong steamId, string objectId)
        {
            if (steamId == 0)
            {
                return;
            }

            objectId = ObjectSyncCodec.Decode(objectId);
            BySteam[steamId] = objectId;
        }

        public static void Set(int slot, string objectId)
        {
            objectId = ObjectSyncCodec.Decode(objectId);
            BySlot[slot] = objectId;
        }

        public static bool TryGetBySteam(ulong steamId, out string objectId)
        {
            return BySteam.TryGetValue(steamId, out objectId);
        }

        public static bool TryGetBySlot(int slot, out string objectId)
        {
            return BySlot.TryGetValue(slot, out objectId);
        }

        public static void RemoveSteam(ulong steamId)
        {
            BySteam.Remove(steamId);
        }

        public static void RemoveSlot(int slot)
        {
            BySlot.Remove(slot);
        }

        public static void Clear()
        {
            BySteam.Clear();
            BySlot.Clear();
        }
    }
}
