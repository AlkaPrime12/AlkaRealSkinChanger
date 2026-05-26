using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Máximo de syncs de glow por frame (todas las líneas).
    /// </summary>
    public static class GlowSyncBudget
    {
        public const int MaxSyncsPerFrame = 8;

        private static int _lastFrame = -1;
        private static int _usedThisFrame;

        public static bool TryConsume()
        {
            int frame = Time.frameCount;
            if (frame != _lastFrame)
            {
                _lastFrame = frame;
                _usedThisFrame = 0;
            }

            if (_usedThisFrame >= MaxSyncsPerFrame)
            {
                return false;
            }

            _usedThisFrame++;
            return true;
        }

        public static void Reset()
        {
            _lastFrame = -1;
            _usedThisFrame = 0;
        }
    }
}
