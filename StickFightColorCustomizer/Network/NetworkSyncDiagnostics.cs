namespace StickFightColorCustomizer.Network
{
    public static class NetworkSyncDiagnostics
    {
        public static string LastEvent = "—";
        public static string LastError = string.Empty;

        public static int DetectedModCount
        {
            get { return ModPresenceRegistry.Count; }
        }

        public static int PendingModCount
        {
            get { return ModPresenceRegistry.PendingCount; }
        }
    }
}
