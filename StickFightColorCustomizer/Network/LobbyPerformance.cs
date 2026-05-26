namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Menú de lobby (no confundir con partida MP: ahí IsInsideLobby puede seguir true).
    /// </summary>
    public static class LobbyPerformance
    {
        public static bool IsLobbyMenuOnly()
        {
            return MatchmakingHandler.Instance != null
                && MatchmakingHandler.Instance.IsInsideLobby
                && !MatchmakingHandler.IsNetworkMatch;
        }
    }
}
