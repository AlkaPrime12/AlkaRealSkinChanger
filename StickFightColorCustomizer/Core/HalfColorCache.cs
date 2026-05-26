using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public sealed class CachedHalfColor
    {
        public float SplitT;
        public Color Proximal;
        public Color Distal;
        public Gradient CachedGradient;
        public int ColorHash;
    }

    public static class HalfColorCache
    {
        private static readonly System.Collections.Generic.Dictionary<int, CachedHalfColor> ByLineId =
            new System.Collections.Generic.Dictionary<int, CachedHalfColor>();

        public static void Store(int lineId, float splitT, Color proximal, Color distal)
        {
            CachedHalfColor entry;
            if (!ByLineId.TryGetValue(lineId, out entry))
            {
                entry = new CachedHalfColor();
                ByLineId[lineId] = entry;
            }

            entry.SplitT = splitT;
            entry.Proximal = proximal;
            entry.Distal = distal;
        }

        public static void StoreGradient(int lineId, Gradient gradient, int colorHash)
        {
            CachedHalfColor entry;
            if (!ByLineId.TryGetValue(lineId, out entry))
            {
                entry = new CachedHalfColor();
                ByLineId[lineId] = entry;
            }

            entry.CachedGradient = gradient;
            entry.ColorHash = colorHash;
        }

        public static bool TryGet(int lineId, out CachedHalfColor entry)
        {
            return ByLineId.TryGetValue(lineId, out entry);
        }

        public static void Clear()
        {
            ByLineId.Clear();
        }
    }
}
