namespace StickFightColorCustomizer.Models
{
    public sealed class WingSettings
    {
        public bool Enabled;
        public string StyleId = "minigame";
        public float Scale = 0.55f;

        public WingSettings Clone()
        {
            return new WingSettings
            {
                Enabled = Enabled,
                StyleId = StyleId ?? "minigame",
                Scale = Scale
            };
        }
    }
}
