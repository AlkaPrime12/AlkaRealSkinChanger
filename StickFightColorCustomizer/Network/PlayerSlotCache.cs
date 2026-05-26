using StickFightColorCustomizer.Core;
using UnityEngine;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Cache playerID -> SteamID y si tiene mod. Evita scans repetidos al spawnear jugadores.
    /// </summary>
    public static class PlayerSlotCache
    {
        private static readonly System.Collections.Generic.Dictionary<int, ulong> SteamByPlayerId =
            new System.Collections.Generic.Dictionary<int, ulong>();

        private static readonly System.Collections.Generic.Dictionary<int, bool> HasModByPlayerId =
            new System.Collections.Generic.Dictionary<int, bool>();

        private static MultiplayerManager _cachedManager;

        public static void Invalidate()
        {
            SteamByPlayerId.Clear();
            HasModByPlayerId.Clear();
            _cachedManager = null;
        }

        public static void InvalidateModPresence()
        {
            HasModByPlayerId.Clear();
        }

        public static bool TryGetSteamId(int playerId, out ulong steamId)
        {
            steamId = 0;
            if (playerId < 0)
            {
                return false;
            }

            if (SteamByPlayerId.TryGetValue(playerId, out steamId) && steamId != 0)
            {
                return true;
            }

            MultiplayerManager manager = GameNetworkCache.GetMultiplayerManager();
            if (manager == null)
            {
                return false;
            }

            if (_cachedManager != manager)
            {
                SteamByPlayerId.Clear();
                HasModByPlayerId.Clear();
                _cachedManager = manager;
            }

            ConnectedClientData[] clients = GameNetworkCache.GetConnectedClients(manager);
            if (clients == null || playerId >= clients.Length)
            {
                return false;
            }

            ConnectedClientData client = clients[playerId];
            if (client == null || !client.ClientID.IsValid())
            {
                SteamByPlayerId.Remove(playerId);
                return false;
            }

            steamId = client.ClientID.m_SteamID;
            SteamByPlayerId[playerId] = steamId;
            return steamId != 0;
        }

        public static bool TryGetHasMod(int playerId, out bool hasMod)
        {
            ulong steamId;
            if (!TryGetSteamId(playerId, out steamId) || steamId == 0)
            {
                hasMod = false;
                HasModByPlayerId[playerId] = false;
                return true;
            }

            bool confirmedMod = RemoteApplyGate.HasConfirmedModColors(steamId);

            if (HasModByPlayerId.TryGetValue(playerId, out hasMod))
            {
                if (!hasMod && confirmedMod)
                {
                    hasMod = true;
                    HasModByPlayerId[playerId] = true;
                }
                else if (hasMod && !confirmedMod)
                {
                    hasMod = false;
                    HasModByPlayerId[playerId] = false;
                }

                return true;
            }

            hasMod = confirmedMod;
            HasModByPlayerId[playerId] = hasMod;
            return true;
        }

        public static void InvalidatePlayer(int playerId)
        {
            SteamByPlayerId.Remove(playerId);
            HasModByPlayerId.Remove(playerId);
        }
    }
}
