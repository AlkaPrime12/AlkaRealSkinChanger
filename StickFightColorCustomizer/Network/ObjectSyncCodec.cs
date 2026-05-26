namespace StickFightColorCustomizer.Network
{
    public static class ObjectSyncCodec
    {
        public const string LobbyMemberKey = "sfcc_objects";

        public static string Encode(string objectId)
        {
            if (string.IsNullOrEmpty(objectId) || objectId == "none")
            {
                return "0";
            }

            return objectId;
        }

        public static string Decode(string raw)
        {
            if (string.IsNullOrEmpty(raw) || raw == "0")
            {
                return "none";
            }

            return Core.ObjectsCatalog.Normalize(raw);
        }
    }
}
