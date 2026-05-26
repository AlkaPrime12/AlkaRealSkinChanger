namespace StickFightColorCustomizer.Core
{
    public enum ObjectLayoutKind
    {
        OrbitBackArc,
        OrbitFull,
        OrbitHalo
    }

    public struct ObjectsCatalogEntry
    {
        public string Id;
        public string Label;
        public ObjectLayoutKind Layout;
        public int PartCount;
        public float OrbitRadius;
        public float OrbitSpeed;
        public float BaseScale;
        public bool BehindBody;
        public string SpriteKey;
    }

    public static class ObjectsCatalog
    {
        public static readonly ObjectsCatalogEntry[] Entries =
        {
            new ObjectsCatalogEntry { Id = "none", Label = "None", Layout = ObjectLayoutKind.OrbitFull, PartCount = 0 },
            new ObjectsCatalogEntry
            {
                Id = "orbs_truth_9",
                Label = "Truth Orbs (9)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 9,
                OrbitRadius = 1.35f,
                OrbitSpeed = 0.9f,
                BaseScale = 0.95f,
                BehindBody = true,
                SpriteKey = "orb_black"
            },
            new ObjectsCatalogEntry
            {
                Id = "rings_battle_6",
                Label = "Battle Rings (6)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 6,
                OrbitRadius = 1.25f,
                OrbitSpeed = 1.2f,
                BaseScale = 0.85f,
                BehindBody = true,
                SpriteKey = "ring_red"
            },
            new ObjectsCatalogEntry
            {
                Id = "orbit_white_6",
                Label = "White Orbs (6)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 6,
                OrbitRadius = 1.35f,
                OrbitSpeed = 1f,
                BaseScale = 0.9f,
                BehindBody = true,
                SpriteKey = "orb_white"
            },
            new ObjectsCatalogEntry
            {
                Id = "orbit_cyan_5",
                Label = "Cyan Gems (5)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 5,
                OrbitRadius = 1.2f,
                OrbitSpeed = 1.15f,
                BaseScale = 0.88f,
                BehindBody = true,
                SpriteKey = "orb_cyan"
            },
            new ObjectsCatalogEntry
            {
                Id = "orbit_gold_4",
                Label = "Gold Orbs (4)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 4,
                OrbitRadius = 1.4f,
                OrbitSpeed = 0.9f,
                BaseScale = 0.92f,
                BehindBody = true,
                SpriteKey = "orb_gold"
            },
            new ObjectsCatalogEntry
            {
                Id = "orbit_purple_8",
                Label = "Purple Orbs (8)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 8,
                OrbitRadius = 1.5f,
                OrbitSpeed = 1.35f,
                BaseScale = 0.88f,
                BehindBody = true,
                SpriteKey = "orb_purple"
            },
            new ObjectsCatalogEntry
            {
                Id = "halo_orbit",
                Label = "Floating Halo",
                Layout = ObjectLayoutKind.OrbitHalo,
                PartCount = 1,
                OrbitRadius = 0.55f,
                OrbitSpeed = 0.6f,
                BaseScale = 0.5f,
                BehindBody = true,
                SpriteKey = "ring_gold"
            },
            // ── Epic new orbs ──────────────────────────────────────────────────────
            new ObjectsCatalogEntry
            {
                Id = "gems_blue_6",
                Label = "Sapphire Gems (6)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 6,
                OrbitRadius = 1.30f,
                OrbitSpeed = 0.85f,
                BaseScale = 0.90f,
                BehindBody = true,
                SpriteKey = "gem_blue"
            },
            new ObjectsCatalogEntry
            {
                Id = "gems_ruby_5",
                Label = "Ruby Gems (5)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 5,
                OrbitRadius = 1.25f,
                OrbitSpeed = 1.00f,
                BaseScale = 0.92f,
                BehindBody = true,
                SpriteKey = "gem_ruby"
            },
            new ObjectsCatalogEntry
            {
                Id = "gems_emerald_7",
                Label = "Emerald Gems (7)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 7,
                OrbitRadius = 1.45f,
                OrbitSpeed = 0.75f,
                BaseScale = 0.85f,
                BehindBody = true,
                SpriteKey = "gem_emerald"
            },
            new ObjectsCatalogEntry
            {
                Id = "gems_amethyst_8",
                Label = "Amethyst Gems (8)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 8,
                OrbitRadius = 1.55f,
                OrbitSpeed = 1.20f,
                BaseScale = 0.82f,
                BehindBody = true,
                SpriteKey = "gem_amethyst"
            },
            new ObjectsCatalogEntry
            {
                Id = "stars_gold_7",
                Label = "Golden Stars (7)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 7,
                OrbitRadius = 1.40f,
                OrbitSpeed = 0.70f,
                BaseScale = 0.95f,
                BehindBody = true,
                SpriteKey = "star_gold"
            },
            new ObjectsCatalogEntry
            {
                Id = "stars_white_5",
                Label = "White Stars (5)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 5,
                OrbitRadius = 1.30f,
                OrbitSpeed = 0.90f,
                BaseScale = 0.90f,
                BehindBody = true,
                SpriteKey = "star_white"
            },
            new ObjectsCatalogEntry
            {
                Id = "plasma_blue_6",
                Label = "Blue Plasma (6)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 6,
                OrbitRadius = 1.35f,
                OrbitSpeed = 1.40f,
                BaseScale = 0.88f,
                BehindBody = true,
                SpriteKey = "plasma_blue"
            },
            new ObjectsCatalogEntry
            {
                Id = "plasma_purple_8",
                Label = "Purple Plasma (8)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 8,
                OrbitRadius = 1.60f,
                OrbitSpeed = 1.60f,
                BaseScale = 0.84f,
                BehindBody = true,
                SpriteKey = "plasma_purple"
            },
            new ObjectsCatalogEntry
            {
                Id = "rings_cyan_5",
                Label = "Cyan Rings (5)",
                Layout = ObjectLayoutKind.OrbitFull,
                PartCount = 5,
                OrbitRadius = 1.20f,
                OrbitSpeed = 1.10f,
                BaseScale = 0.80f,
                BehindBody = true,
                SpriteKey = "ring_cyan"
            },
            // ── 20 new themed objects ────────────────────────────────────────────────
            new ObjectsCatalogEntry { Id = "knives_steel_6",  Label = "Steel Knives (6)",   Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.30f, OrbitSpeed = 1.30f, BaseScale = 0.95f, BehindBody = true,  SpriteKey = "knife_steel" },
            new ObjectsCatalogEntry { Id = "knives_gold_8",   Label = "Gold Knives (8)",    Layout = ObjectLayoutKind.OrbitFull, PartCount = 8, OrbitRadius = 1.45f, OrbitSpeed = 1.05f, BaseScale = 0.90f, BehindBody = true,  SpriteKey = "knife_gold" },
            new ObjectsCatalogEntry { Id = "shurikens_black_5", Label = "Shurikens (5)",     Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.25f, OrbitSpeed = 1.80f, BaseScale = 0.88f, BehindBody = true,  SpriteKey = "shuriken_black" },
            new ObjectsCatalogEntry { Id = "shurikens_red_6",   Label = "Red Shurikens (6)", Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.35f, OrbitSpeed = 2.10f, BaseScale = 0.84f, BehindBody = true,  SpriteKey = "shuriken_red" },
            new ObjectsCatalogEntry { Id = "swords_steel_4",  Label = "Steel Swords (4)",   Layout = ObjectLayoutKind.OrbitFull, PartCount = 4, OrbitRadius = 1.55f, OrbitSpeed = 0.70f, BaseScale = 1.10f, BehindBody = true,  SpriteKey = "sword_steel" },
            new ObjectsCatalogEntry { Id = "swords_fire_5",   Label = "Fire Swords (5)",    Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.60f, OrbitSpeed = 0.85f, BaseScale = 1.05f, BehindBody = true,  SpriteKey = "sword_fire" },
            new ObjectsCatalogEntry { Id = "kanji_red_6",     Label = "Red Kanji (6)",      Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.35f, OrbitSpeed = 0.60f, BaseScale = 0.95f, BehindBody = true,  SpriteKey = "kanji_red" },
            new ObjectsCatalogEntry { Id = "kanji_gold_5",    Label = "Gold Kanji (5)",     Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.30f, OrbitSpeed = 0.55f, BaseScale = 1.00f, BehindBody = true,  SpriteKey = "kanji_gold" },
            new ObjectsCatalogEntry { Id = "kanji_black_7",   Label = "Black Kanji (7)",    Layout = ObjectLayoutKind.OrbitFull, PartCount = 7, OrbitRadius = 1.40f, OrbitSpeed = 0.65f, BaseScale = 0.90f, BehindBody = true,  SpriteKey = "kanji_black" },
            new ObjectsCatalogEntry { Id = "skulls_white_5",  Label = "Skulls (5)",         Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.35f, OrbitSpeed = 0.95f, BaseScale = 0.95f, BehindBody = true,  SpriteKey = "skull_white" },
            new ObjectsCatalogEntry { Id = "skulls_black_6",  Label = "Cursed Skulls (6)",  Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.45f, OrbitSpeed = 1.10f, BaseScale = 0.90f, BehindBody = true,  SpriteKey = "skull_black" },
            new ObjectsCatalogEntry { Id = "hearts_red_8",    Label = "Hearts (8)",         Layout = ObjectLayoutKind.OrbitFull, PartCount = 8, OrbitRadius = 1.40f, OrbitSpeed = 0.80f, BaseScale = 0.85f, BehindBody = true,  SpriteKey = "heart_red" },
            new ObjectsCatalogEntry { Id = "hearts_dark_6",   Label = "Dark Hearts (6)",    Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.30f, OrbitSpeed = 1.00f, BaseScale = 0.92f, BehindBody = true,  SpriteKey = "heart_dark" },
            new ObjectsCatalogEntry { Id = "bolts_yellow_7",  Label = "Lightning (7)",      Layout = ObjectLayoutKind.OrbitFull, PartCount = 7, OrbitRadius = 1.45f, OrbitSpeed = 1.90f, BaseScale = 0.95f, BehindBody = true,  SpriteKey = "bolt_yellow" },
            new ObjectsCatalogEntry { Id = "bolts_cyan_6",    Label = "Cyan Bolts (6)",     Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.40f, OrbitSpeed = 1.70f, BaseScale = 0.95f, BehindBody = true,  SpriteKey = "bolt_cyan" },
            new ObjectsCatalogEntry { Id = "snowflakes_6",    Label = "Snowflakes (6)",     Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.35f, OrbitSpeed = 0.70f, BaseScale = 0.95f, BehindBody = true,  SpriteKey = "snowflake" },
            new ObjectsCatalogEntry { Id = "leaves_green_7",  Label = "Green Leaves (7)",   Layout = ObjectLayoutKind.OrbitFull, PartCount = 7, OrbitRadius = 1.40f, OrbitSpeed = 0.60f, BaseScale = 0.95f, BehindBody = true,  SpriteKey = "leaf_green" },
            new ObjectsCatalogEntry { Id = "leaves_autumn_8", Label = "Autumn Leaves (8)",  Layout = ObjectLayoutKind.OrbitFull, PartCount = 8, OrbitRadius = 1.50f, OrbitSpeed = 0.55f, BaseScale = 0.92f, BehindBody = true,  SpriteKey = "leaf_autumn" },
            new ObjectsCatalogEntry { Id = "yinyang_4",       Label = "Yin-Yang (4)",       Layout = ObjectLayoutKind.OrbitFull, PartCount = 4, OrbitRadius = 1.30f, OrbitSpeed = 0.85f, BaseScale = 1.00f, BehindBody = true,  SpriteKey = "yinyang" },
            new ObjectsCatalogEntry { Id = "crosses_gold_6",  Label = "Gold Crosses (6)",   Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.35f, OrbitSpeed = 0.75f, BaseScale = 0.92f, BehindBody = true,  SpriteKey = "cross_gold" },
            new ObjectsCatalogEntry { Id = "moons_silver_5",  Label = "Silver Moons (5)",   Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.30f, OrbitSpeed = 0.50f, BaseScale = 1.00f, BehindBody = true,  SpriteKey = "moon_silver" },
            // ── 20 floating internet / symbol orbs ───────────────────────────────────
            new ObjectsCatalogEntry { Id = "float_cross_inv_b_6", Label = "Inverted Crosses (6)", Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.32f, OrbitSpeed = 0.72f, BaseScale = 0.94f, BehindBody = true, SpriteKey = "cross_inv_black" },
            new ObjectsCatalogEntry { Id = "float_cross_inv_r_5", Label = "Red Inverted Cross (5)", Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.28f, OrbitSpeed = 0.80f, BaseScale = 0.96f, BehindBody = true, SpriteKey = "cross_inv_red" },
            new ObjectsCatalogEntry { Id = "float_cross_inv_w_4", Label = "White Inverted Cross (4)", Layout = ObjectLayoutKind.OrbitFull, PartCount = 4, OrbitRadius = 1.25f, OrbitSpeed = 0.65f, BaseScale = 1.00f, BehindBody = true, SpriteKey = "cross_inv_white" },
            new ObjectsCatalogEntry { Id = "float_letter_x_6",    Label = "Glow Letter X (6)",    Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.35f, OrbitSpeed = 1.05f, BaseScale = 0.98f, BehindBody = true, SpriteKey = "letter_x" },
            new ObjectsCatalogEntry { Id = "float_letter_o_5",    Label = "Glow Letter O (5)",    Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.30f, OrbitSpeed = 0.90f, BaseScale = 0.95f, BehindBody = true, SpriteKey = "letter_o" },
            new ObjectsCatalogEntry { Id = "float_letter_z_5",    Label = "Glow Letter Z (5)",    Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.32f, OrbitSpeed = 1.15f, BaseScale = 0.97f, BehindBody = true, SpriteKey = "letter_z" },
            new ObjectsCatalogEntry { Id = "float_han_red_6",     Label = "Red Han (6)",          Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.34f, OrbitSpeed = 0.58f, BaseScale = 1.02f, BehindBody = true, SpriteKey = "han_red" },
            new ObjectsCatalogEntry { Id = "float_han_gold_5",    Label = "Gold Han (5)",         Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.30f, OrbitSpeed = 0.52f, BaseScale = 1.04f, BehindBody = true, SpriteKey = "han_gold" },
            new ObjectsCatalogEntry { Id = "float_han_cyan_7",    Label = "Cyan Han (7)",         Layout = ObjectLayoutKind.OrbitFull, PartCount = 7, OrbitRadius = 1.38f, OrbitSpeed = 0.62f, BaseScale = 0.96f, BehindBody = true, SpriteKey = "han_cyan" },
            new ObjectsCatalogEntry { Id = "float_han_void_6",    Label = "Void Han (6)",         Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.36f, OrbitSpeed = 0.48f, BaseScale = 1.00f, BehindBody = true, SpriteKey = "han_void" },
            new ObjectsCatalogEntry { Id = "float_infinity_6",    Label = "Infinity (6)",         Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.33f, OrbitSpeed = 0.88f, BaseScale = 0.92f, BehindBody = true, SpriteKey = "infinity" },
            new ObjectsCatalogEntry { Id = "float_omega_5",       Label = "Omega (5)",            Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.28f, OrbitSpeed = 0.70f, BaseScale = 1.05f, BehindBody = true, SpriteKey = "omega" },
            new ObjectsCatalogEntry { Id = "float_pentagram_6",   Label = "Pentagram (6)",        Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.40f, OrbitSpeed = 0.95f, BaseScale = 0.90f, BehindBody = true, SpriteKey = "pentagram" },
            new ObjectsCatalogEntry { Id = "float_wifi_6",        Label = "Wi-Fi Arc (6)",        Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.30f, OrbitSpeed = 1.20f, BaseScale = 0.88f, BehindBody = true, SpriteKey = "wifi" },
            new ObjectsCatalogEntry { Id = "float_hashtag_5",     Label = "Hashtag (5)",          Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.26f, OrbitSpeed = 0.75f, BaseScale = 1.00f, BehindBody = true, SpriteKey = "hashtag" },
            new ObjectsCatalogEntry { Id = "float_smile_6",       Label = "Meme Smile (6)",       Layout = ObjectLayoutKind.OrbitFull, PartCount = 6, OrbitRadius = 1.32f, OrbitSpeed = 0.68f, BaseScale = 0.98f, BehindBody = true, SpriteKey = "smile_meme" },
            new ObjectsCatalogEntry { Id = "float_eyes_5",        Label = "Spiral Eyes (5)",      Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.34f, OrbitSpeed = 1.10f, BaseScale = 0.94f, BehindBody = true, SpriteKey = "meme_eyes" },
            new ObjectsCatalogEntry { Id = "float_rune_5",        Label = "Norse Rune (5)",       Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.30f, OrbitSpeed = 0.55f, BaseScale = 1.02f, BehindBody = true, SpriteKey = "rune" },
            new ObjectsCatalogEntry { Id = "float_flame_7",       Label = "Soul Flame (7)",       Layout = ObjectLayoutKind.OrbitFull, PartCount = 7, OrbitRadius = 1.38f, OrbitSpeed = 1.25f, BaseScale = 0.90f, BehindBody = true, SpriteKey = "flame_teardrop" },
            new ObjectsCatalogEntry { Id = "float_wing_5",        Label = "Mini Wings (5)",       Layout = ObjectLayoutKind.OrbitFull, PartCount = 5, OrbitRadius = 1.42f, OrbitSpeed = 0.82f, BaseScale = 0.86f, BehindBody = true, SpriteKey = "mini_wing" }
        };

        public static bool IsValid(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                return false;
            }

            for (int i = 0; i < Entries.Length; i++)
            {
                if (Entries[i].Id == objectId)
                {
                    return true;
                }
            }

            return false;
        }

        public static string Normalize(string objectId)
        {
            return IsValid(objectId) ? objectId : "none";
        }

        public static bool TryGet(string objectId, out ObjectsCatalogEntry entry)
        {
            objectId = Normalize(objectId);
            for (int i = 0; i < Entries.Length; i++)
            {
                if (Entries[i].Id == objectId)
                {
                    entry = Entries[i];
                    return true;
                }
            }

            entry = default(ObjectsCatalogEntry);
            return false;
        }
    }
}
