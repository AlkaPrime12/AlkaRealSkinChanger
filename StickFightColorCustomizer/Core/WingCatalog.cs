namespace StickFightColorCustomizer.Core
{
    public static class WingCatalog
    {
        public const string Minigame = "minigame";

        public static string Normalize(string styleId)
        {
            return string.IsNullOrEmpty(styleId) || styleId == "none" ? Minigame : styleId;
        }
    }
}
