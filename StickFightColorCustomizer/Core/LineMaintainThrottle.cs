using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class LineMaintainThrottle
    {
        private const float MinIntervalActiveLocal = 0.1f;
        private const float MinIntervalActiveNetwork = 0.18f;
        private const float MinIntervalHalfOnlyLocal = 0.28f;
        private const float MinIntervalHalfOnlyNetwork = 0.38f;
        private const float MinIntervalDefaultLocal = 0.08f;
        private const float MinIntervalDefaultNetwork = 0.14f;

        private static readonly System.Collections.Generic.Dictionary<int, int> LastHashByLine =
            new System.Collections.Generic.Dictionary<int, int>();

        private static readonly System.Collections.Generic.Dictionary<int, float> LastTimeByLine =
            new System.Collections.Generic.Dictionary<int, float>();

        private static bool _forceDirty;

        public static float GetMinInterval(ColorConfig config)
        {
            bool network = MatchmakingHandler.IsNetworkMatch;
            if (config == null)
            {
                return network ? MinIntervalDefaultNetwork : MinIntervalDefaultLocal;
            }

            bool activeCosmetic = ModFeatureGate.IsRgbActive(config)
                || ModFeatureGate.IsGlowActive(config)
                || ModFeatureGate.IsBodyActive(config);

            if (activeCosmetic)
            {
                return network ? MinIntervalActiveNetwork : MinIntervalActiveLocal;
            }

            if (config.Colors != null && config.Colors.HalfColorEnabled)
            {
                return network ? MinIntervalHalfOnlyNetwork : MinIntervalHalfOnlyLocal;
            }

            return network ? MinIntervalDefaultNetwork : MinIntervalDefaultLocal;
        }

        public static bool HasBeenApplied(int lineId)
        {
            return LastHashByLine.ContainsKey(lineId);
        }

        public static bool ShouldSkip(int lineId, int colorHash, ColorConfig config, bool colorsLookApplied)
        {
            if (_forceDirty || !colorsLookApplied)
            {
                return false;
            }

            int lastHash;
            float lastTime;
            float minInterval = GetMinInterval(config);
            if (LastHashByLine.TryGetValue(lineId, out lastHash) && lastHash == colorHash
                && LastTimeByLine.TryGetValue(lineId, out lastTime)
                && Time.realtimeSinceStartup - lastTime < minInterval)
            {
                return true;
            }

            return false;
        }

        public static bool TryConsumeHeavyApply(int lineId, bool colorsLookApplied)
        {
            if (_forceDirty || !colorsLookApplied || !HasBeenApplied(lineId))
            {
                return true;
            }

            return LineMaintainBudget.TryConsumeHeavyApply();
        }

        public static void MarkApplied(int lineId, int colorHash)
        {
            LastHashByLine[lineId] = colorHash;
            LastTimeByLine[lineId] = Time.realtimeSinceStartup;
        }

        public static void MarkDirty()
        {
            _forceDirty = true;
            LastTimeByLine.Clear();
            LineMaintainBudget.Reset();
        }

        public static void ClearForceDirtyIfNeeded()
        {
            if (_forceDirty)
            {
                _forceDirty = false;
            }
        }

        public static int ComputeBodyHash(Color body, Color distal, bool half, float splitT)
        {
            int h = body.GetHashCode();
            h = (h * 397) ^ distal.GetHashCode();
            h = (h * 397) ^ (half ? 1 : 0);
            h = (h * 397) ^ Mathf.RoundToInt(splitT * 1000f);
            return h;
        }

        public static void Clear()
        {
            LastHashByLine.Clear();
            LastTimeByLine.Clear();
            _forceDirty = false;
            LineMaintainBudget.Reset();
        }
    }
}
