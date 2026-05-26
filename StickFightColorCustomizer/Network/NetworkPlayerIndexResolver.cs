using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace StickFightColorCustomizer.Network
{
    public static class NetworkPlayerIndexResolver
    {
        private static readonly Dictionary<ulong, byte> SlotBySteamId = new Dictionary<ulong, byte>();

        public static void RebuildSlotMap()
        {
            SlotBySteamId.Clear();
            MultiplayerManager manager = GameNetworkCache.GetMultiplayerManager();
            if (manager == null)
            {
                return;
            }

            ConnectedClientData[] clients = GameNetworkCache.GetConnectedClients(manager);
            if (clients == null)
            {
                return;
            }

            for (byte i = 0; i < clients.Length; i++)
            {
                ConnectedClientData client = clients[i];
                if (client == null || !client.ClientID.IsValid())
                {
                    continue;
                }

                ulong steamId = client.ClientID.m_SteamID;
                if (steamId != 0)
                {
                    SlotBySteamId[steamId] = i;
                }
            }
        }

        public static void ClearSlotMap()
        {
            SlotBySteamId.Clear();
        }

        public static bool TryGetSlot(ulong steamId, out byte slot)
        {
            return SlotBySteamId.TryGetValue(steamId, out slot);
        }

        public static byte? FindSlotForSteamId(CSteamID steamId)
        {
            if (!steamId.IsValid())
            {
                return null;
            }

            byte slot;
            if (TryGetSlot(steamId.m_SteamID, out slot))
            {
                return slot;
            }

            RebuildSlotMap();
            if (TryGetSlot(steamId.m_SteamID, out slot))
            {
                return slot;
            }

            return null;
        }
    }
}
