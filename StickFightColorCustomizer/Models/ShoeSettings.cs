namespace StickFightColorCustomizer.Models
{
    public sealed class ShoeSettings
    {
        public bool Enabled;
        public string ShoeId = "none";

        public ShoeSettings Clone()
        {
            return new ShoeSettings
            {
                Enabled = Enabled,
                ShoeId = ShoeId ?? "none"
            };
        }
    }
}
