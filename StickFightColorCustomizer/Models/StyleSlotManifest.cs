using System.Collections.Generic;

namespace StickFightColorCustomizer.Models
{
    public sealed class StyleSlotEntry
    {
        public int Index;
        public string Name = "";
        public bool HasData;
    }

    public sealed class StyleSlotManifest
    {
        public List<StyleSlotEntry> Slots = new List<StyleSlotEntry>();

        public static StyleSlotManifest CreateDefault(int count)
        {
            var manifest = new StyleSlotManifest();
            for (int i = 1; i <= count; i++)
            {
                manifest.Slots.Add(new StyleSlotEntry
                {
                    Index = i,
                    Name = "Slot " + i,
                    HasData = false
                });
            }

            return manifest;
        }
    }
}
