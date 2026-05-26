namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Bootstrap de detección solo una vez por sesión MP (no cada ronda/puntos).
    /// </summary>
    public static class MatchSessionState
    {
        private static bool _bootstrapDone;

        public static bool HasBootstrapped
        {
            get { return _bootstrapDone; }
        }

        public static void MarkBootstrapped()
        {
            _bootstrapDone = true;
        }

        public static void ResetForLobbyMenu()
        {
            _bootstrapDone = false;
            MatchBootstrap.Clear();
            SpawnBurstCoordinator.Clear();
        }
    }
}
