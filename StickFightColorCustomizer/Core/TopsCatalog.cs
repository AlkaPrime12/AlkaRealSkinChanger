namespace StickFightColorCustomizer.Core
{
    public struct TopsCatalogEntry
    {
        public string Id;
        public string Label;
    }

    public static class TopsCatalog
    {
        public static readonly TopsCatalogEntry[] Entries =
        {
            new TopsCatalogEntry { Id = "none",         Label = "None" },
            new TopsCatalogEntry { Id = "tshirt",       Label = "T-Shirt" },
            new TopsCatalogEntry { Id = "hoodie",       Label = "Hoodie" },
            new TopsCatalogEntry { Id = "jacket",       Label = "Jacket" },
            new TopsCatalogEntry { Id = "tank",         Label = "Tank Top" },
            new TopsCatalogEntry { Id = "dress_shirt",  Label = "Dress Shirt" },
            new TopsCatalogEntry { Id = "jersey",       Label = "Jersey" },
            new TopsCatalogEntry { Id = "vest",         Label = "Vest" },
            // ── 5 nuevos tops "sarpados" ──
            new TopsCatalogEntry { Id = "armor_gold",   Label = "Gold Armor" },
            new TopsCatalogEntry { Id = "tuxedo",       Label = "Tuxedo" },
            new TopsCatalogEntry { Id = "clown",        Label = "Clown Suit" },
            new TopsCatalogEntry { Id = "neon",         Label = "Neon Cyber" },
            new TopsCatalogEntry { Id = "varsity",      Label = "Varsity Bomber" }
        };

        public static bool IsValid(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            for (int i = 0; i < Entries.Length; i++)
            {
                if (Entries[i].Id == id)
                {
                    return true;
                }
            }

            return false;
        }

        public static string Normalize(string id)
        {
            return IsValid(id) ? id : "none";
        }
    }
}
