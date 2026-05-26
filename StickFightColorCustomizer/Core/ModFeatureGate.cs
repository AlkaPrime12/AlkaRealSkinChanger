using StickFightColorCustomizer.Models;
using StickFightColorCustomizer.Network;

namespace StickFightColorCustomizer.Core
{
    public static class ModFeatureGate
    {
        public static bool IsBodyActive(ColorConfig config)
        {
            return config != null && config.BodyCustomizationActive;
        }

        public static bool IsGlowActive(ColorConfig config)
        {
            return config != null && config.Glow != null && config.Glow.Enabled;
        }

        public static bool IsHatActive(ColorConfig config)
        {
            if (config == null || config.Hat == null || !config.Hat.Enabled)
            {
                return false;
            }

            string id = config.Hat.HatId;
            return !string.IsNullOrEmpty(id) && id != "none";
        }

        public static bool IsShoeActive(ColorConfig config)
        {
            if (!ShoeFeatureFlags.Released)
            {
                return false;
            }

            if (config == null || config.Shoe == null || !config.Shoe.Enabled)
            {
                return false;
            }

            string id = config.Shoe.ShoeId;
            return !string.IsNullOrEmpty(id) && id != "none";
        }

        public static bool IsTopsActive(ColorConfig config)
        {
            if (!TopsFeatureFlags.Released)
            {
                return false;
            }

            if (config == null || config.Tops == null || !config.Tops.Enabled)
            {
                return false;
            }

            string id = config.Tops.TopsId;
            return !string.IsNullOrEmpty(id) && id != "none";
        }

        public static bool IsWingActive(ColorConfig config)
        {
            if (!WingFeatureFlags.Released)
            {
                return false;
            }

            return config != null && config.Wing != null && config.Wing.Enabled;
        }

        public static bool IsObjectsActive(ColorConfig config)
        {
            if (!ObjectFeatureFlags.Released)
            {
                return false;
            }

            if (config == null || config.Object == null || !config.Object.Enabled)
            {
                return false;
            }

            string id = config.Object.ObjectId;
            return !string.IsNullOrEmpty(id) && id != "none";
        }

        public static bool IsRgbActive(ColorConfig config)
        {
            return config != null && config.AnimatedRgb;
        }

        public static bool IsWeaponActive(ColorConfig config)
        {
            return config != null && config.Weapon != null && config.Weapon.Enabled;
        }

        public static bool NeedsSetLineMaintenance(ColorConfig config, bool lobbyMenu)
        {
            if (config == null)
            {
                return false;
            }

            if (IsBodyActive(config))
            {
                return true;
            }

            if (IsShoeActive(config))
            {
                return true;
            }

            if (IsObjectsActive(config))
            {
                return true;
            }

            if (!IsGlowActive(config))
            {
                return false;
            }

            return !lobbyMenu || (config.Glow != null && config.Glow.MaintainInLobby);
        }

        public static bool NeedsLobbyColorSync(ColorConfig config)
        {
            return IsBodyActive(config) || IsGlowActive(config);
        }

        public static bool NeedsColorPing(ColorConfig config)
        {
            return IsBodyActive(config);
        }

        public static string DescribeActive(ColorConfig config)
        {
            if (config == null)
            {
                return MenuLocalization.T("feat_none");
            }

            var parts = new System.Collections.Generic.List<string>();
            if (IsBodyActive(config))
            {
                parts.Add(MenuLocalization.T("feat_colors"));
            }

            if (IsGlowActive(config))
            {
                parts.Add(MenuLocalization.T("feat_glow"));
            }

            if (IsHatActive(config))
            {
                parts.Add(MenuLocalization.T("feat_hat"));
            }

            if (IsShoeActive(config))
            {
                parts.Add(MenuLocalization.T("feat_shoes"));
            }

            if (IsTopsActive(config))
            {
                parts.Add(MenuLocalization.T("feat_tops"));
            }

            if (IsObjectsActive(config))
            {
                parts.Add(MenuLocalization.T("feat_objects"));
            }

            if (IsWingActive(config))
            {
                parts.Add(MenuLocalization.T("feat_wings"));
            }

            if (IsWeaponActive(config))
            {
                parts.Add(MenuLocalization.T("feat_weapons"));
            }

            if (IsRgbActive(config))
            {
                parts.Add(MenuLocalization.T("feat_rgb"));
            }

            return parts.Count > 0 ? string.Join(", ", parts.ToArray()) : MenuLocalization.T("feat_none");
        }
    }
}
