namespace StickFightColorCustomizer.Models
{
    public sealed class TopsSettings
    {
        public bool Enabled;
        public string TopsId = "none";

        public TopsSettings Clone()
        {
            return new TopsSettings { Enabled = Enabled, TopsId = TopsId };
        }
    }
}
