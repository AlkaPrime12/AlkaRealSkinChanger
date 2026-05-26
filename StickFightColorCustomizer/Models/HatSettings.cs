namespace StickFightColorCustomizer.Models
{
    public sealed class HatSettings
    {
        public bool Enabled;
        public string HatId = "none";

        public HatSettings Clone()
        {
            return new HatSettings
            {
                Enabled = Enabled,
                HatId = HatId ?? "none"
            };
        }
    }
}
