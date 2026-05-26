namespace StickFightColorCustomizer.Models
{
    public sealed class ObjectSettings
    {
        public bool Enabled;
        public string ObjectId = "none";
        public float Scale = 1f;

        public ObjectSettings Clone()
        {
            return new ObjectSettings
            {
                Enabled = Enabled,
                ObjectId = ObjectId ?? "none",
                Scale = Scale
            };
        }
    }
}
