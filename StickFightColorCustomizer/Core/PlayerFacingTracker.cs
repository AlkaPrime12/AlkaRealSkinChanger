using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Tracks which way each Controller is facing so cosmetics that have a "front" side
    /// (image hats, eventually shoes/tops) can mirror themselves.
    ///
    /// Stick Fight plays in the Y-Z plane (camera looks along +X), so screen horizontal
    /// equals world Z. We classify facing by the Z-velocity of the controller's rigidbody,
    /// with a small hysteresis so the hat doesn't flicker at zero velocity.
    /// </summary>
    public static class PlayerFacingTracker
    {
        private struct State
        {
            public bool FacingRight;
            public float LastNonZeroAt;
        }

        private static readonly Dictionary<int, State> ByControllerId = new Dictionary<int, State>();
        private const float VelocityThreshold = 0.6f;

        public static void Clear()
        {
            ByControllerId.Clear();
        }

        public static void ClearController(Controller controller)
        {
            if (controller == null) return;
            ByControllerId.Remove(controller.GetInstanceID());
        }

        /// <summary>
        /// Returns true if the controller is currently facing the +Z direction
        /// (visually screen-right). Defaults to facing-right when unknown.
        /// </summary>
        public static bool IsFacingRight(Controller controller)
        {
            if (controller == null) return true;

            int id = controller.GetInstanceID();
            State st;
            if (!ByControllerId.TryGetValue(id, out st))
            {
                st = new State { FacingRight = true, LastNonZeroAt = 0f };
            }

            float zVel = ReadHorizontalVelocity(controller);
            if (Mathf.Abs(zVel) > VelocityThreshold)
            {
                // Sign inverted: in Stick Fight, "A" (visually moving the stickman to the
                // left of the screen) increases world Z and "D" decreases it. Treat
                // velocity.z > 0 as facing-LEFT so the flip lines up with what the user
                // sees on screen.
                st.FacingRight = zVel < 0f;
                st.LastNonZeroAt = Time.realtimeSinceStartup;
                ByControllerId[id] = st;
            }
            else
            {
                // Hold the last facing — no change while idle/standing still.
                ByControllerId[id] = st;
            }

            return st.FacingRight;
        }

        private static float ReadHorizontalVelocity(Controller controller)
        {
            // Try common ragdoll rigidbody locations.
            Rigidbody rb = controller.GetComponent<Rigidbody>();
            if (rb != null) return rb.velocity.z;

            Rigidbody[] all = controller.GetComponentsInChildren<Rigidbody>(true);
            float bestMag = 0f;
            float bestZ = 0f;
            for (int i = 0; i < all.Length; i++)
            {
                Rigidbody r = all[i];
                if (r == null) continue;
                float m = Mathf.Abs(r.velocity.z);
                if (m > bestMag)
                {
                    bestMag = m;
                    bestZ = r.velocity.z;
                }
            }
            return bestZ;
        }
    }
}
