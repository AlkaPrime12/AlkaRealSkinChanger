using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Throttle por línea (no global): evita picos sin dejar el glow congelado en un solo hueso.
    /// </summary>
    public static class GlowAuraThrottle
    {
        private const float MinInterval = 0.06f;
        private static readonly Dictionary<int, float> LastSyncBySourceLine = new Dictionary<int, float>();

        public static bool ShouldSync(int sourceLineId)
        {
            float last;
            float now = Time.realtimeSinceStartup;
            if (LastSyncBySourceLine.TryGetValue(sourceLineId, out last) && now - last < MinInterval)
            {
                return false;
            }

            LastSyncBySourceLine[sourceLineId] = now;
            return true;
        }

        public static void ForceNext(int sourceLineId)
        {
            LastSyncBySourceLine.Remove(sourceLineId);
        }

        public static void Reset()
        {
            LastSyncBySourceLine.Clear();
        }
    }
}
