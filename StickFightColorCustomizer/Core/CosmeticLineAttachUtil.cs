using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Anclaje de sprites a LineRenderers: extremos por eje de cámara y posición en mundo.
    /// </summary>
    public static class CosmeticLineAttachUtil
    {
        public const float DepthOffset = 0.08f;

        private static readonly string[] LeftDistalLegBoneNames =
        {
            "LeftLeg", "Leg_Left", "L_Leg", "leftleg", "Foot_Left", "LeftFoot"
        };

        private static readonly string[] RightDistalLegBoneNames =
        {
            "RightLeg", "Leg_Right", "R_Leg", "rightleg", "Foot_Right", "RightFoot"
        };

        public static Vector3 GetDownAxisWorld()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                return -cam.transform.up;
            }

            return Vector3.down;
        }

        public static int FindLowestAlongDownAxis(LineRenderer line)
        {
            return FindExtremityAlongAxis(line, GetDownAxisWorld(), pickMinimumProjection: true);
        }

        public static int FindHighestAlongDownAxis(LineRenderer line)
        {
            return FindExtremityAlongAxis(line, GetDownAxisWorld(), pickMinimumProjection: false);
        }

        /// <summary>
        /// World position of the player's torso, used as the reference point to tell hip from foot.
        /// We can't use world-Y because mid-jump/kick the foot can be ABOVE the hip — but the hip
        /// is *always* the leg endpoint geometrically closest to the spine line (it's the joint
        /// shared with the torso), and the foot is *always* the other endpoint.
        /// </summary>
        public static Vector3 GetTorsoReferenceWorld(Controller controller)
        {
            if (controller == null) return Vector3.zero;

            Transform renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform);
            Transform spineNode = renderersRoot != null
                ? PlayerColorApplier.FindRendererTransform(renderersRoot, "spineRenderer")
                : null;

            if (spineNode != null)
            {
                LineRenderer[] lines = spineNode.GetComponentsInChildren<LineRenderer>(true);
                for (int i = 0; i < lines.Length; i++)
                {
                    LineRenderer l = lines[i];
                    if (l == null || l.gameObject.name.StartsWith("SFCC_")) continue;
                    int cnt = StickLineUtil.GetVertexCount(l);
                    if (cnt <= 0) continue;

                    Vector3 sum = Vector3.zero;
                    for (int v = 0; v < cnt; v++) sum += StickLineUtil.GetVertexWorld(l, v);
                    return sum / cnt;
                }
            }

            return controller.transform.position;
        }

        public static int FindFootVertexIndex(LineRenderer legLine)
        {
            return FindFootVertexIndex(legLine, null);
        }

        /// <summary>
        /// Returns the leg LineRenderer endpoint that represents the foot.
        /// Strategy: pick the endpoint (index 0 or count-1) that is FARTHEST from the torso
        /// reference point. Pose-invariant: works in idle, mid-jump, kicking, even when the
        /// character is upside-down.
        /// </summary>
        public static int FindFootVertexIndex(LineRenderer legLine, Controller controller)
        {
            if (legLine == null) return 0;
            int count = StickLineUtil.GetVertexCount(legLine);
            if (count <= 1) return 0;

            // Endpoint detection — foot is at one of the two ends, never the middle (knee).
            int endA = 0;
            int endB = count - 1;
            Vector3 worldA = StickLineUtil.GetVertexWorld(legLine, endA);
            Vector3 worldB = StickLineUtil.GetVertexWorld(legLine, endB);

            Vector3 torso = controller != null
                ? GetTorsoReferenceWorld(controller)
                : Vector3.zero;

            if (controller != null)
            {
                float dA = (worldA - torso).sqrMagnitude;
                float dB = (worldB - torso).sqrMagnitude;
                return dA > dB ? endA : endB;   // foot = endpoint FARTHEST from torso
            }

            // Fallback (no controller): the endpoint with smaller world Y is typically the foot.
            // Still better than picking the middle vertex.
            return worldA.y < worldB.y ? endA : endB;
        }

        public static int FindHipVertexIndex(LineRenderer legLine)
        {
            return FindHipVertexIndex(legLine, null);
        }

        /// <summary>Hip = the leg endpoint CLOSEST to the torso (opposite of the foot).</summary>
        public static int FindHipVertexIndex(LineRenderer legLine, Controller controller)
        {
            if (legLine == null) return 0;
            int count = StickLineUtil.GetVertexCount(legLine);
            if (count <= 1) return 0;

            int footIdx = FindFootVertexIndex(legLine, controller);
            return footIdx == 0 ? count - 1 : 0;
        }

        public static LineRenderer FindDistalLegBoneLine(Transform legNode, bool isRightLeg)
        {
            if (legNode == null)
            {
                return null;
            }

            string[] prefer = isRightLeg ? RightDistalLegBoneNames : LeftDistalLegBoneNames;
            LineRenderer[] lines = legNode.GetComponentsInChildren<LineRenderer>(true);

            for (int p = 0; p < prefer.Length; p++)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    LineRenderer line = lines[i];
                    if (line == null
                        || line.transform == legNode
                        || line.gameObject.name.StartsWith("SFCC_"))
                    {
                        continue;
                    }

                    if (string.Equals(line.gameObject.name, prefer[p], System.StringComparison.OrdinalIgnoreCase))
                    {
                        return line;
                    }
                }
            }

            Vector3 down = GetDownAxisWorld();
            LineRenderer best = null;
            float bestFootProj = float.MaxValue;

            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null
                    || line.transform == legNode
                    || line.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                string boneName = line.gameObject.name;
                if (boneName.IndexOf("Knee", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || boneName.IndexOf("Elbow", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }

                int count = StickLineUtil.GetVertexCount(line);
                if (count <= 0)
                {
                    continue;
                }

                int footIdx = FindLowestAlongDownAxis(line);
                float footProj = Vector3.Dot(StickLineUtil.GetVertexWorld(line, footIdx), down);
                if (footProj < bestFootProj)
                {
                    bestFootProj = footProj;
                    best = line;
                }
            }

            if (best != null)
            {
                return best;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line != null
                    && line.transform != legNode
                    && !line.gameObject.name.StartsWith("SFCC_")
                    && StickLineUtil.GetVertexCount(line) >= 2)
                {
                    return line;
                }
            }

            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line != null
                    && line.transform != legNode
                    && !line.gameObject.name.StartsWith("SFCC_"))
                {
                    return line;
                }
            }

            return null;
        }

        /// <summary>Pie en mundo solo en huesos distales (LeftLeg/RightLeg), no rodilla.</summary>
        public static bool TryGetDistalFootWorld(Transform legNode, bool isRightLeg, out Vector3 worldFoot, out float boneRadius)
        {
            worldFoot = Vector3.zero;
            boneRadius = 0.28f;
            if (legNode == null)
            {
                return false;
            }

            string[] prefer = isRightLeg ? RightDistalLegBoneNames : LeftDistalLegBoneNames;
            LineRenderer[] lines = legNode.GetComponentsInChildren<LineRenderer>(true);
            Vector3 down = GetDownAxisWorld();
            bool found = false;
            float bestProj = float.MaxValue;

            for (int p = 0; p < prefer.Length; p++)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    LineRenderer line = lines[i];
                    if (line == null
                        || line.gameObject.name.StartsWith("SFCC_")
                        || !string.Equals(line.gameObject.name, prefer[p], System.StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    float radius = Mathf.Max(line.widthMultiplier * 2.6f, 0.22f);
                    int count = StickLineUtil.GetVertexCount(line);
                    for (int v = 0; v < count; v++)
                    {
                        Vector3 world = StickLineUtil.GetVertexWorld(line, v);
                        float proj = Vector3.Dot(world, down);
                        if (proj < bestProj)
                        {
                            bestProj = proj;
                            worldFoot = world;
                            boneRadius = radius;
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                LineRenderer fallback = FindDistalLegBoneLine(legNode, isRightLeg);
                if (fallback == null)
                {
                    return false;
                }

                boneRadius = Mathf.Max(fallback.widthMultiplier * 2.6f, 0.22f);
                int footIdx = FindFootVertexIndex(fallback);
                worldFoot = StickLineUtil.GetVertexWorld(fallback, footIdx);
                found = true;
            }

            Camera cam = Camera.main;
            if (cam != null)
            {
                worldFoot += -cam.transform.forward * DepthOffset;
            }

            return found;
        }

        /// <summary>
        /// Posición del zapato en mundo. Usa el punto más bajo de la pierna y, si en idle queda alto (torso),
        /// baja hacia el pie real con el largo del hueso.
        /// </summary>
        public static bool TryGetShoeWorldPosition(Transform legNode, bool isRightLeg, out Vector3 worldFoot, out float boneRadius)
        {
            worldFoot = Vector3.zero;
            boneRadius = 0.28f;
            if (legNode == null)
            {
                return false;
            }

            Vector3 down = GetDownAxisWorld();
            LineRenderer[] lines = legNode.GetComponentsInChildren<LineRenderer>(true);
            bool found = false;
            float lowestProj = float.MaxValue;

            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null
                    || line.transform == legNode
                    || line.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                float radius = Mathf.Max(line.widthMultiplier * 2.6f, 0.22f);
                int count = StickLineUtil.GetVertexCount(line);
                for (int v = 0; v < count; v++)
                {
                    Vector3 world = StickLineUtil.GetVertexWorld(line, v);
                    float proj = Vector3.Dot(world, down);
                    if (proj < lowestProj)
                    {
                        lowestProj = proj;
                        worldFoot = world;
                        boneRadius = radius;
                        found = true;
                    }
                }
            }

            LineRenderer limb = PickBestLimbLine(legNode);
            if (limb != null)
            {
                int footIdx = FindFootVertexIndex(limb);
                int topIdx = FindHighestAlongDownAxis(limb);
                Vector3 footWorld = StickLineUtil.GetVertexWorld(limb, footIdx);
                Vector3 topWorld = StickLineUtil.GetVertexWorld(limb, topIdx);
                float footProj = Vector3.Dot(footWorld, down);
                float topProj = Vector3.Dot(topWorld, down);
                float spanAlongDown = Mathf.Abs(topProj - footProj);
                float spanDist = Vector3.Distance(topWorld, footWorld);

                Vector3 targetFoot;
                if (spanDist < 0.12f || spanAlongDown < 0.08f)
                {
                    targetFoot = footWorld + down * Mathf.Max(spanDist * 0.5f, boneRadius * 3.2f);
                }
                else
                {
                    targetFoot = Vector3.Lerp(topWorld, footWorld, 0.94f);
                }

                float targetProj = Vector3.Dot(targetFoot, down);
                if (!found || targetProj < lowestProj + 0.02f)
                {
                    worldFoot = targetFoot;
                    boneRadius = Mathf.Max(limb.widthMultiplier * 2.6f, 0.22f);
                    found = true;
                }
                else if (targetProj < lowestProj + spanAlongDown * 0.35f)
                {
                    worldFoot = targetFoot;
                }
            }

            if (!found)
            {
                return false;
            }

            Camera cam = Camera.main;
            if (cam != null)
            {
                worldFoot += -cam.transform.forward * DepthOffset;
            }

            return true;
        }

        /// <summary>Posición local en el pie del LineRenderer (lerp cadera→pie).</summary>
        public static Vector3 GetFootLocalOnLine(LineRenderer line, float footLerp)
        {
            return GetFootLocalOnLine(line, footLerp, null);
        }

        public static Vector3 GetFootLocalOnLine(LineRenderer line, float footLerp, Controller controller)
        {
            if (line == null) return Vector3.zero;
            int count = StickLineUtil.GetVertexCount(line);
            if (count < 1) return Vector3.zero;

            int footIdx = FindFootVertexIndex(line, controller);   // endpoint farthest from torso
            Vector3 localFoot = StickLineUtil.GetVertexLocal(line, footIdx);
            if (count < 2) return localFoot;

            int topIdx = FindHipVertexIndex(line, controller);     // opposite endpoint = hip
            Vector3 localTop = StickLineUtil.GetVertexLocal(line, topIdx);
            return Vector3.Lerp(localTop, localFoot, Mathf.Clamp01(footLerp));
        }

        /// <summary>Resuelve la línea distal del pie (LeftLeg/RightLeg) con fallbacks.</summary>
        public static bool TryResolveFootLine(Transform legNode, bool isRight, out LineRenderer footLine)
        {
            footLine = null;
            if (legNode == null)
            {
                return false;
            }

            footLine = FindDistalLegBoneLine(legNode, isRight);
            if (footLine != null && StickLineUtil.GetVertexCount(footLine) >= 1)
            {
                return true;
            }

            footLine = PickBestLimbLine(legNode);
            if (footLine != null && StickLineUtil.GetVertexCount(footLine) >= 1)
            {
                return true;
            }

            LineRenderer[] lines = legNode.GetComponentsInChildren<LineRenderer>(true);
            LineRenderer best = null;
            float bestProj = float.MaxValue;
            Vector3 down = GetDownAxisWorld();

            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null
                    || line.gameObject.name.StartsWith("SFCC_")
                    || StickLineUtil.GetVertexCount(line) < 1)
                {
                    continue;
                }

                int footIdx = FindFootVertexIndex(line);
                float proj = Vector3.Dot(StickLineUtil.GetVertexWorld(line, footIdx), down);
                if (proj < bestProj)
                {
                    bestProj = proj;
                    best = line;
                }
            }

            footLine = best;
            return footLine != null;
        }

        /// <summary>Vértice más bajo del pie en mundo (sin lerp al torso en idle).</summary>
        public static Vector3 GetFootWorldPosition(LineRenderer footLine)
        {
            if (footLine == null || StickLineUtil.GetVertexCount(footLine) < 1)
            {
                return Vector3.zero;
            }

            int footIdx = FindFootVertexIndex(footLine);
            Vector3 worldFoot = StickLineUtil.GetVertexWorld(footLine, footIdx);

            Camera cam = Camera.main;
            if (cam != null)
            {
                worldFoot += -cam.transform.forward * DepthOffset;
            }

            return worldFoot;
        }

        /// <summary>
        /// Pie en mundo para zapatos: hueso distal (LeftLeg/RightLeg) + lerp local, sin parent en hueso.
        /// </summary>
        public static bool ResolveFootWorld(
            Transform legNode,
            bool isRightLeg,
            float footLerp,
            out Vector3 worldFoot,
            out float boneRadius,
            out LineRenderer footLine)
        {
            worldFoot = Vector3.zero;
            boneRadius = 0.28f;
            footLine = null;

            if (legNode == null)
            {
                return false;
            }

            footLine = FindDistalLegBoneLine(legNode, isRightLeg);
            if (footLine == null || StickLineUtil.GetVertexCount(footLine) < 1)
            {
                if (!TryGetDistalFootWorld(legNode, isRightLeg, out worldFoot, out boneRadius))
                {
                    return TryGetLegFootWorld(legNode, isRightLeg, out worldFoot, out boneRadius);
                }

                footLine = FindDistalLegBoneLine(legNode, isRightLeg);
                if (footLine == null)
                {
                    footLine = PickBestLimbLine(legNode);
                }

                return true;
            }

            boneRadius = Mathf.Max(footLine.widthMultiplier * 2.6f, 0.22f);
            Vector3 localFoot = GetFootLocalOnLine(footLine, footLerp);
            worldFoot = footLine.transform.TransformPoint(localFoot);

            Camera cam = Camera.main;
            if (cam != null)
            {
                worldFoot += -cam.transform.forward * DepthOffset;
            }

            return true;
        }

        /// <summary>Pie en mundo usando la mejor línea de la pierna (fallback si no hay hueso distal).</summary>
        public static bool TryGetLegFootWorld(Transform legNode, bool isRightLeg, out Vector3 worldFoot, out float boneRadius)
        {
            if (TryGetDistalFootWorld(legNode, isRightLeg, out worldFoot, out boneRadius))
            {
                return true;
            }

            worldFoot = Vector3.zero;
            boneRadius = 0.28f;
            if (legNode == null)
            {
                return false;
            }

            LineRenderer line = FindDistalLegBoneLine(legNode, isRightLeg);
            if (line == null)
            {
                line = PickBestLimbLine(legNode);
            }

            if (line == null || StickLineUtil.GetVertexCount(line) < 1)
            {
                return false;
            }

            boneRadius = Mathf.Max(line.widthMultiplier * 2.6f, 0.22f);
            int footIdx = FindFootVertexIndex(line);
            worldFoot = StickLineUtil.GetVertexWorld(line, footIdx);

            Camera cam = Camera.main;
            if (cam != null)
            {
                worldFoot += -cam.transform.forward * DepthOffset;
            }

            return true;
        }

        public static int FindExtremityAlongAxis(LineRenderer line, Vector3 axis, bool pickMinimumProjection)
        {
            int count = StickLineUtil.GetVertexCount(line);
            if (count <= 0)
            {
                return 0;
            }

            if (count == 1)
            {
                return 0;
            }

            Vector3 dir = axis.normalized;
            int best = 0;
            float bestProj = pickMinimumProjection ? float.MaxValue : float.MinValue;

            for (int i = 0; i < count; i++)
            {
                float proj = Vector3.Dot(StickLineUtil.GetVertexWorld(line, i), dir);
                if (pickMinimumProjection ? proj < bestProj : proj > bestProj)
                {
                    bestProj = proj;
                    best = i;
                }
            }

            return best;
        }

        public static LineRenderer PickBestLimbLine(Transform limbNode)
        {
            if (limbNode == null)
            {
                return null;
            }

            LineRenderer[] lines = limbNode.GetComponentsInChildren<LineRenderer>(true);
            Vector3 down = GetDownAxisWorld();
            LineRenderer best = null;
            float bestSpan = -1f;

            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null
                    || line.transform == limbNode
                    || line.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                int low = FindLowestAlongDownAxis(line);
                int high = FindHighestAlongDownAxis(line);
                float span = Vector3.Dot(StickLineUtil.GetVertexWorld(line, high), down)
                    - Vector3.Dot(StickLineUtil.GetVertexWorld(line, low), down);
                span = Mathf.Abs(span);
                if (span > bestSpan)
                {
                    bestSpan = span;
                    best = line;
                }
            }

            if (best != null)
            {
                return best;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line != null
                    && line.transform != limbNode
                    && !line.gameObject.name.StartsWith("SFCC_"))
                {
                    return line;
                }
            }

            return null;
        }

        public static Vector3 LocalNudgeAlongBone(LineRenderer line, int fromIdx, int toIdx, float radiusFactor)
        {
            int count = StickLineUtil.GetVertexCount(line);
            if (count < 2 || fromIdx < 0 || toIdx < 0)
            {
                return Vector3.zero;
            }

            Vector3 localFrom = StickLineUtil.GetVertexLocal(line, fromIdx);
            Vector3 localTo = StickLineUtil.GetVertexLocal(line, toIdx);
            Vector3 along = localTo - localFrom;
            if (along.sqrMagnitude < 0.0001f)
            {
                return Vector3.zero;
            }

            float radius = Mathf.Max(line.widthMultiplier * 2.6f, 0.22f);
            return along.normalized * (radius * radiusFactor);
        }
    }
}
