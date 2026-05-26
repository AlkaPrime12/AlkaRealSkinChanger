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
            new TopsCatalogEntry { Id = "varsity",      Label = "Varsity Bomber" },
            // ── 20 new tops (hyper-detailed) ──
            new TopsCatalogEntry { Id = "tx_lava",       Label = "Lava Tee" },
            new TopsCatalogEntry { Id = "tx_galaxy",     Label = "Galaxy Print" },
            new TopsCatalogEntry { Id = "tx_camo",       Label = "Camo Field" },
            new TopsCatalogEntry { Id = "tx_pirate",     Label = "Pirate Coat" },
            new TopsCatalogEntry { Id = "tx_knight",     Label = "Knight Plate" },
            new TopsCatalogEntry { Id = "tx_ninja",      Label = "Ninja Gi" },
            new TopsCatalogEntry { Id = "tx_pharaoh",    Label = "Pharaoh Robe" },
            new TopsCatalogEntry { Id = "tx_robot",      Label = "Robot Chassis" },
            new TopsCatalogEntry { Id = "tx_skeleton",   Label = "Skeleton Print" },
            new TopsCatalogEntry { Id = "tx_lab",        Label = "Lab Coat" },
            new TopsCatalogEntry { Id = "tx_track",      Label = "Track Suit" },
            new TopsCatalogEntry { Id = "tx_kimono",     Label = "Kimono" },
            new TopsCatalogEntry { Id = "tx_punk",       Label = "Punk Vest" },
            new TopsCatalogEntry { Id = "tx_gradient",   Label = "Gradient Pop" },
            new TopsCatalogEntry { Id = "tx_streetwear", Label = "Streetwear" },
            new TopsCatalogEntry { Id = "tx_holiday",    Label = "Holiday Sweater" },
            new TopsCatalogEntry { Id = "tx_racer",      Label = "Race Suit" },
            new TopsCatalogEntry { Id = "tx_priest",     Label = "Cleric Robe" },
            new TopsCatalogEntry { Id = "tx_chef",       Label = "Chef Apron" },
            new TopsCatalogEntry { Id = "tx_diver",      Label = "Wetsuit" }
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
