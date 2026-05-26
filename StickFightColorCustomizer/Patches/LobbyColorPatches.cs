using HarmonyLib;
using StickFightColorCustomizer.Network;
using Steamworks;

namespace StickFightColorCustomizer.Patches
{
    [HarmonyPatch(typeof(MatchmakingHandler), "OnLobbyDataUpdate")]
    public static class Patch_MatchmakingHandler_OnLobbyDataUpdate
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            LobbySyncScheduler.OnLobbyDataChanged();
        }
    }

    [HarmonyPatch(typeof(MatchmakingHandler), "OnLobbyChatUpdate")]
    public static class Patch_MatchmakingHandler_OnLobbyChatUpdate
    {
        [HarmonyPostfix]
        public static void Postfix(LobbyChatUpdate_t param)
        {
            if (param.m_rgfChatMemberStateChange != (int)EChatMemberStateChange.k_EChatMemberStateChangeEntered)
            {
                return;
            }

            ulong steamId = param.m_ulSteamIDUserChanged;
            if (steamId == 0)
            {
                return;
            }

            NetworkSyncDiagnostics.LastEvent = "Lobby enter " + steamId;
            SteamLobbyColorSync.RefreshFromLobby(true, mergeOnly: true);
            ModColorPingSync.RequestPingToPeer(steamId);
            LobbySyncScheduler.MarkDirty(steamId);
        }
    }

    [HarmonyPatch(typeof(MultiplayerManager), "OnPlayerJoined")]
    public static class Patch_MultiplayerManager_OnPlayerJoined
    {
        [HarmonyPostfix]
        public static void Postfix(CSteamID SteamID)
        {
            if (!SteamID.IsValid())
            {
                return;
            }

            ulong steamId = SteamID.m_SteamID;
            NetworkSyncDiagnostics.LastEvent = "Player joined " + steamId;
            SteamLobbyColorSync.RefreshFromLobby(true, mergeOnly: true);
            ModColorPingSync.RequestPingToPeer(steamId);

            if (!ModPresenceRegistry.HasMod(steamId) && !ModPresenceRegistry.IsPending(steamId))
            {
                return;
            }

            if (ControllerHandler.Instance == null)
            {
                return;
            }

            var active = ControllerHandler.Instance.ActivePlayers;
            if (active == null)
            {
                return;
            }

            for (int i = 0; i < active.Count; i++)
            {
                Controller controller = active[i];
                if (controller == null)
                {
                    continue;
                }

                ulong id;
                if (PlayerSlotCache.TryGetSteamId(controller.playerID, out id) && id == steamId)
                {
                    MatchEntryColorScheduler.EnqueueRemote(controller);
                    return;
                }
            }
        }
    }

    [HarmonyPatch(typeof(P2PPackageHandler), "CheckMessageType")]
    public static class Patch_P2PPackageHandler_CheckMessageType
    {
        [HarmonyPrefix]
        public static void Prefix(byte[] data, P2PPackageHandler.MsgType type, CSteamID steamIdRemote)
        {
            if (type == P2PPackageHandler.MsgType.Ping || type == P2PPackageHandler.MsgType.PingResponse)
            {
                ModColorPingSync.OnVanillaPingReceived(steamIdRemote.m_SteamID, data);
            }
        }
    }
}
