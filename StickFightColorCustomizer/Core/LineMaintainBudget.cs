using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Limita applies pesados de gradiente por frame para evitar picos.
    /// </summary>
    public static class LineMaintainBudget
    {
        public const int MaxHeavyAppliesPerFrame = 3;

        private static int _lastFrame = -1;
        private static int _appliedThisFrame;

        public static bool TryConsumeHeavyApply()
        {
            int frame = Time.frameCount;
            if (frame != _lastFrame)
            {
                _lastFrame = frame;
                _appliedThisFrame = 0;
            }

            if (_appliedThisFrame >= MaxHeavyAppliesPerFrame)
            {
                return false;
            }

            _appliedThisFrame++;
            return true;
        }

        public static void Reset()
        {
            _lastFrame = -1;
            _appliedThisFrame = 0;
        }
    }
}
