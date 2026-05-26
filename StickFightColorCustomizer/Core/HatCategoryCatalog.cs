namespace StickFightColorCustomizer.Core
{
    public struct HatCategoryDef
    {
        public string CategoryId;
        public string Label;
        public string RepresentativeId;
        public string[] VariantIds;
    }

    /// <summary>17 categorías × 10 variantes. UI: icono + clic abre selector.</summary>
    public static class HatCategoryCatalog
    {
        public static readonly HatCategoryDef[] Categories =
        {
            Make("halo", "Halo", new[]
            {
                "halo_white", "halo_gold", "halo_red", "halo_blue", "halo_green",
                "halo_purple", "halo_cyan", "halo_pink", "halo_silver", "halo_rainbow"
            }),
            Make("horn", "Horns", new[]
            {
                "horn_devil", "horn_bull", "horn_ram", "horn_imp", "horn_stag",
                "horn_unicorn", "horn_demon", "horn_antler", "horn_spike", "horn_crystal"
            }),
            Make("eye", "Eyes", new[]
            {
                "eye_normal", "eye_angry", "eye_sleepy", "eye_wide", "eye_dot",
                "eye_visor", "eye_shade", "eye_glow", "eye_x", "eye_heart"
            }),
            Make("visor", "Visors", new[]
            {
                "visor_shades", "visor_round", "visor_mono", "visor_red", "visor_blue",
                "visor_neon", "visor_tech", "visor_pilot", "visor_3d", "visor_future"
            }),
            Make("crown", "Crowns", new[]
            {
                "crown_gold", "crown_silver", "crown_ruby", "crown_ice", "crown_dark",
                "crown_mini", "crown_spike", "crown_leaf", "crown_star", "crown_neon"
            }),
            Make("ear", "Ears", new[]
            {
                "ear_cat", "ear_bunny", "ear_bear", "ear_fox", "ear_elf",
                "ear_mouse", "ear_wolf", "ear_bat", "ear_pig", "ear_long"
            }),
            Make("mark", "Marks", new[]
            {
                "mark_star", "mark_gem", "mark_scar", "mark_dot", "mark_cross",
                "mark_moon", "mark_flame", "mark_bolt", "mark_heart", "mark_skull"
            }),
            Make("mask", "Masks", new[]
            {
                "mask_ninja", "mask_skull", "mask_gas", "mask_clown", "mask_bandit",
                "mask_hero", "mask_phantom", "mask_steel", "mask_lace", "mask_void"
            }),
            Make("hair", "Hair", new[]
            {
                "hair_spike", "hair_mop", "hair_afro", "hair_pompadour", "hair_bang",
                "hair_shine", "hair_green", "hair_pink", "hair_blue", "hair_white"
            }),
            Make("brow", "Brows", new[]
            {
                "brow_thick", "brow_thin", "brow_angry", "brow_sad", "brow_uni",
                "brow_red", "brow_bush", "brow_arch", "brow_bolt", "brow_star"
            }),
            Make("blush", "Blush", new[]
            {
                "blush_pink", "blush_red", "blush_heart", "blush_circle", "blush_stripe",
                "blush_freckle", "blush_scar_l", "blush_scar_r", "blush_burn", "blush_cute"
            }),
            Make("mohawk", "Mohawk", new[]
            {
                "mohawk_red", "mohawk_blue", "mohawk_green", "mohawk_punk", "mohawk_flame",
                "mohawk_ice", "mohawk_gold", "mohawk_purple", "mohawk_short", "mohawk_tall"
            }),
            Make("antenna", "Antenna", new[]
            {
                "antenna_classic", "antenna_ball", "antenna_spiral", "antenna_alien", "antenna_robot",
                "antenna_bee", "antenna_double", "antenna_long", "antenna_glow", "antenna_fork"
            }),
            Make("aura", "Aura", new[]
            {
                "aura_white", "aura_gold", "aura_fire", "aura_ice", "aura_poison",
                "aura_holy", "aura_dark", "aura_pulse", "aura_rainbow", "aura_soft"
            }),
            Make("feather", "Feather", new[]
            {
                "feather_red", "feather_blue", "feather_gold", "feather_white", "feather_peacock",
                "feather_crow", "feather_flame", "feather_leaf", "feather_royal", "feather_twin"
            }),
            Make("patch", "Eye Patch", new[]
            {
                "patch_black", "patch_red", "patch_gold", "patch_skull", "patch_star",
                "patch_heart", "patch_cross", "patch_tech", "patch_pirate", "patch_royal"
            }),
            Make("spike", "Spikes", new[]
            {
                "spike_collar", "spike_bone", "spike_ice", "spike_fire", "spike_metal",
                "spike_short", "spike_long", "spike_crown", "spike_dark", "spike_neon"
            })
        };

        private static HatCategoryDef Make(string catId, string label, string[] variants)
        {
            return new HatCategoryDef
            {
                CategoryId = catId,
                Label = label,
                RepresentativeId = variants[0],
                VariantIds = variants
            };
        }

        public static bool IsVariant(string hatId)
        {
            if (string.IsNullOrEmpty(hatId))
            {
                return false;
            }

            for (int c = 0; c < Categories.Length; c++)
            {
                string[] ids = Categories[c].VariantIds;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] == hatId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryGetCategoryForVariant(string hatId, out HatCategoryDef category)
        {
            category = default(HatCategoryDef);
            for (int c = 0; c < Categories.Length; c++)
            {
                string[] ids = Categories[c].VariantIds;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] == hatId)
                    {
                        category = Categories[c];
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GetVariantLabel(string hatId)
        {
            HatCategoryDef cat;
            if (!TryGetCategoryForVariant(hatId, out cat))
            {
                return hatId;
            }

            for (int i = 0; i < cat.VariantIds.Length; i++)
            {
                if (cat.VariantIds[i] == hatId)
                {
                    string name = MenuLocalization.HatCategoryLabel(cat.CategoryId, cat.Label);
                    return name + " " + MenuLocalization.Tf("hat_variant_num", i + 1);
                }
            }

            return hatId;
        }
    }
}
