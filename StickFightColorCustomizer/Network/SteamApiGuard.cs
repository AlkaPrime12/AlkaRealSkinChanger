using Steamworks;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Envuelve Steamworks para no lanzar "Steam is not initialized" en menú / arranque.
    /// </summary>
    public static class SteamApiGuard
    {
        public static int GetNumLobbyMembers(CSteamID lobby)
        {
            if (!lobby.IsValid() || !SteamReadyHelper.IsReady())
            {
                return 0;
            }

            try
            {
                return SteamMatchmaking.GetNumLobbyMembers(lobby);
            }
            catch
            {
                return 0;
            }
        }

        public static CSteamID GetLobbyMemberByIndex(CSteamID lobby, int index)
        {
            if (!lobby.IsValid() || !SteamReadyHelper.IsReady())
            {
                return CSteamID.Nil;
            }

            try
            {
                return SteamMatchmaking.GetLobbyMemberByIndex(lobby, index);
            }
            catch
            {
                return CSteamID.Nil;
            }
        }

        public static string GetLobbyMemberData(CSteamID lobby, CSteamID member, string key)
        {
            if (!lobby.IsValid() || !member.IsValid() || !SteamReadyHelper.IsReady()
                || string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            try
            {
                return SteamMatchmaking.GetLobbyMemberData(lobby, member, key) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void SetLobbyMemberData(CSteamID lobby, string key, string value)
        {
            if (!lobby.IsValid() || !SteamReadyHelper.IsReady() || string.IsNullOrEmpty(key))
            {
                return;
            }

            try
            {
                SteamMatchmaking.SetLobbyMemberData(lobby, key, value ?? string.Empty);
            }
            catch
            {
                // Steam aún no listo
            }
        }
    }
}
