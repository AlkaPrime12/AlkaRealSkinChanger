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
            new TopsCatalogEntry { Id = "tx_diver",      Label = "Wetsuit" },
            // ── 20 nuevos tops épicos (segunda tanda) ──
            new TopsCatalogEntry { Id = "tx2_mech",       Label = "Mech Suit" },
            new TopsCatalogEntry { Id = "tx2_crystal",    Label = "Crystal Armor" },
            new TopsCatalogEntry { Id = "tx2_lightning",  Label = "Lightning Robe" },
            new TopsCatalogEntry { Id = "tx2_tribal",     Label = "Tribal Paint" },
            new TopsCatalogEntry { Id = "tx2_patchwork",  Label = "Patchwork Jacket" },
            new TopsCatalogEntry { Id = "tx2_sweater",    Label = "Knit Sweater" },
            new TopsCatalogEntry { Id = "tx2_dragon",     Label = "Dragon Scales" },
            new TopsCatalogEntry { Id = "tx2_cyberpunk",  Label = "Cyberpunk Vest" },
            new TopsCatalogEntry { Id = "tx2_robe",       Label = "Mystic Robe" },
            new TopsCatalogEntry { Id = "tx2_football",   Label = "Football Jersey" },
            new TopsCatalogEntry { Id = "tx2_basket",     Label = "Basketball Tank" },
            new TopsCatalogEntry { Id = "tx2_hawaiian",   Label = "Hawaiian Shirt" },
            new TopsCatalogEntry { Id = "tx2_plaid",      Label = "Plaid Flannel" },
            new TopsCatalogEntry { Id = "tx2_pinstripe",  Label = "Pinstripe Suit" },
            new TopsCatalogEntry { Id = "tx2_captain",    Label = "Pirate Captain" },
            new TopsCatalogEntry { Id = "tx2_skulls",     Label = "Skulls Print" },
            new TopsCatalogEntry { Id = "tx2_carbon",     Label = "Carbon Fiber" },
            new TopsCatalogEntry { Id = "tx2_lab2",       Label = "Hazmat Suit" },
            new TopsCatalogEntry { Id = "tx2_jet",        Label = "Jet Pilot" },
            new TopsCatalogEntry { Id = "tx2_circuit",    Label = "Circuit Board" }
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
