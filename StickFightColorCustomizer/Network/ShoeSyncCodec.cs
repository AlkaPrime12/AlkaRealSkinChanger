namespace StickFightColorCustomizer.Network
{
    public static class ShoeSyncCodec
    {
        public const string LobbyMemberKey = "sfcc_shoes";

        public static string Encode(string shoeId)
        {
            if (string.IsNullOrEmpty(shoeId) || shoeId == "none")
            {
                return "0";
            }

            return shoeId;
        }

        public static string Decode(string raw)
        {
            if (string.IsNullOrEmpty(raw) || raw == "0")
            {
                return "none";
            }

            return Core.ShoeCatalog.Normalize(raw);
        }
    }
}
