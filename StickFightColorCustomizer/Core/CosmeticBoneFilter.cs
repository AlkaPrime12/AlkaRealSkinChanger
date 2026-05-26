using System;

namespace StickFightColorCustomizer.Core
{
    /// <summary>Evita sync cosmético en huesos irrelevantes (menos trabajo por SetLinePositions en MP).</summary>
    public static class CosmeticBoneFilter
    {
        public static bool IsSpineOrTorsoBone(string boneName)
        {
            if (string.IsNullOrEmpty(boneName))
            {
                return false;
            }

            return string.Equals(boneName, "spineRenderer", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "Torso", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "Hip", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsLegBoneForShoes(string boneName)
        {
            if (string.IsNullOrEmpty(boneName))
            {
                return false;
            }

            return string.Equals(boneName, "legRenderer", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "legRenderer2", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "LeftLeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "RightLeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "Leg_Left", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "Leg_Right", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "LeftKnee", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "RightKnee", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "Knee_Left", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "Knee_Right", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsHeadBoneForGlow(string boneName)
        {
            if (string.IsNullOrEmpty(boneName))
            {
                return false;
            }

            return string.Equals(boneName, "headRenderer", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "Head", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsArmBoneForGlow(string boneName)
        {
            if (string.IsNullOrEmpty(boneName))
            {
                return false;
            }

            return boneName.IndexOf("hand", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(boneName, "handRenderer", StringComparison.OrdinalIgnoreCase)
                || string.Equals(boneName, "handRenderer2", StringComparison.OrdinalIgnoreCase);
        }
    }
}
