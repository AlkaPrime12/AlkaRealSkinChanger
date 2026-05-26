namespace StickFightColorCustomizer.Network
{
    public static class HatSyncCodec
    {
        public const string LobbyMemberKey = "sfcc_hat";

        public static string Encode(string hatId)
        {
            if (string.IsNullOrEmpty(hatId) || hatId == "none")
            {
                return "0";
            }

            return hatId;
        }

        public static string Decode(string raw)
        {
            if (string.IsNullOrEmpty(raw) || raw == "0")
            {
                return "none";
            }

            return Core.HatCatalog.Normalize(raw);
        }
    }
}
