namespace StickFightColorCustomizer.Core
{
    public struct ShoeCatalogEntry
    {
        public string Id;
        public string Label;
    }

    public static class ShoeCatalog
    {
        public static readonly ShoeCatalogEntry[] Entries =
        {
            new ShoeCatalogEntry { Id = "none",      Label = "None" },
            new ShoeCatalogEntry { Id = "sneakers",  Label = "Sneakers" },
            new ShoeCatalogEntry { Id = "boots",     Label = "Boots" },
            new ShoeCatalogEntry { Id = "combat",    Label = "Combat" },
            new ShoeCatalogEntry { Id = "dress",     Label = "Dress" },
            new ShoeCatalogEntry { Id = "sandals",   Label = "Sandals" },
            new ShoeCatalogEntry { Id = "cleats",    Label = "Cleats" },
            new ShoeCatalogEntry { Id = "slippers",  Label = "Slippers" },
            new ShoeCatalogEntry { Id = "steel_toe", Label = "Steel Toe" },
            new ShoeCatalogEntry { Id = "rollers",   Label = "Rollers" },
            new ShoeCatalogEntry { Id = "spikes",    Label = "Spike Boots" },
            // ── 5 new shoes ──
            new ShoeCatalogEntry { Id = "high_top",   Label = "High-Top Sneakers" },
            new ShoeCatalogEntry { Id = "running",    Label = "Running Shoes" },
            new ShoeCatalogEntry { Id = "neon_kicks", Label = "Neon Kicks" },
            new ShoeCatalogEntry { Id = "biker",      Label = "Biker Boots" },
            new ShoeCatalogEntry { Id = "cowboy_b",   Label = "Cowboy Boots" },
            // ── 10 botas altas y estrechas (poco ancho, más alto) ──
            new ShoeCatalogEntry { Id = "slim_black",   Label = "Slim Black Boot" },
            new ShoeCatalogEntry { Id = "slim_brown",   Label = "Slim Leather Boot" },
            new ShoeCatalogEntry { Id = "slim_combat",  Label = "Slim Combat Boot" },
            new ShoeCatalogEntry { Id = "slim_ninja",   Label = "Slim Ninja Boot" },
            new ShoeCatalogEntry { Id = "slim_formal",  Label = "Slim Dress Boot" },
            new ShoeCatalogEntry { Id = "slim_hiker",   Label = "Slim Hiker Boot" },
            new ShoeCatalogEntry { Id = "slim_punk",    Label = "Slim Punk Boot" },
            new ShoeCatalogEntry { Id = "slim_arctic",  Label = "Slim Arctic Boot" },
            new ShoeCatalogEntry { Id = "slim_rider",   Label = "Slim Rider Boot" },
            new ShoeCatalogEntry { Id = "slim_chelsea", Label = "Slim Chelsea Boot" }
        };

        public static bool IsValid(string shoeId)
        {
            if (string.IsNullOrEmpty(shoeId))
            {
                return false;
            }

            for (int i = 0; i < Entries.Length; i++)
            {
                if (Entries[i].Id == shoeId)
                {
                    return true;
                }
            }

            return false;
        }

        public static string Normalize(string shoeId)
        {
            return IsValid(shoeId) ? shoeId : "none";
        }
    }
}
