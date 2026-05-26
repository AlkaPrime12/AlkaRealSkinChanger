using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class BoneColorSplitter
    {
        private static readonly Dictionary<string, string[]> SplitBonesByRenderer = new Dictionary<string, string[]>
        {
            { "spineRenderer", new[] { "Hip", "Torso" } },
            { "handRenderer", new[] { "LeftElbow", "LeftArm", "Elbow_Left", "Arm_Left" } },
            { "handRenderer2", new[] { "RightElbow", "RightArm", "Elbow_Right", "Arm_Right" } },
            { "legRenderer", new[] { "LeftKnee", "LeftLeg", "Knee_Left", "Leg_Left" } },
            { "legRenderer2", new[] { "RightKnee", "RightLeg", "Knee_Right", "Leg_Right" } }
        };

        private const float SplitTChangeThreshold = 0.025f;

        public static bool SupportsHalfSplit(string rendererObjectName)
        {
            return SplitBonesByRenderer.ContainsKey(rendererObjectName);
        }

        public static string GetBoneHint(string rendererObjectName)
        {
            string[] bones;
            if (!SplitBonesByRenderer.TryGetValue(rendererObjectName, out bones) || bones.Length == 0)
            {
                return "";
            }

            return bones[0];
        }

        public static void ApplyHalfGradient(
            LineRenderer line,
            Color proximal,
            Color distal,
            float splitT,
            Gradient cachedGradient)
        {
            if (line == null)
            {
                return;
            }

            splitT = Mathf.Clamp(splitT, 0.08f, 0.92f);
            Gradient gradient = cachedGradient ?? BuildGradient(proximal, distal, splitT);
            line.colorGradient = gradient;
            line.startColor = proximal;
            line.endColor = distal;
            MaterialColorHelper.ApplyLineMaterialIfUnchanged(line, Color.Lerp(proximal, distal, 0.5f));
        }

        public static Gradient BuildGradient(Color proximal, Color distal, float splitT)
        {
            splitT = Mathf.Clamp(splitT, 0.08f, 0.92f);
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(proximal, 0f),
                    new GradientColorKey(proximal, splitT),
                    new GradientColorKey(distal, splitT),
                    new GradientColorKey(distal, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                });
            return gradient;
        }

        /// <summary>
        /// Hueso cacheado + proyección sobre la línea (barato). Sin búsqueda en jerarquía por frame.
        /// </summary>
        public static float ResolveSplitT(Controller controller, LineRenderer line, string rendererObjectName)
        {
            if (controller == null || line == null || line.positionCount < 2)
            {
                return 0.5f;
            }

            Transform bone = GetSplitBoneTransform(controller, rendererObjectName);
            if (bone == null)
            {
                return 0.5f;
            }

            return ProjectOntoLine(line, bone.position);
        }

        /// <summary>
        /// Actualiza splitT solo si el hueso se movió lo bastante o no hay cache.
        /// </summary>
        public static float ResolveSplitTTracked(
            Controller controller,
            LineRenderer line,
            string rendererObjectName,
            int lineId,
            out bool splitChanged)
        {
            splitChanged = false;
            float splitT = ResolveSplitT(controller, line, rendererObjectName);

            CachedHalfColor entry;
            if (HalfColorCache.TryGet(lineId, out entry) && entry != null)
            {
                splitChanged = Mathf.Abs(entry.SplitT - splitT) > SplitTChangeThreshold;
            }
            else
            {
                splitChanged = true;
            }

            return splitT;
        }

        public static Transform GetSplitBoneTransform(Controller controller, string rendererObjectName)
        {
            string[] boneNames;
            if (!SplitBonesByRenderer.TryGetValue(rendererObjectName, out boneNames))
            {
                return null;
            }

            for (int i = 0; i < boneNames.Length; i++)
            {
                Transform bone = BoneIndexCache.GetBone(controller, boneNames[i]);
                if (bone != null)
                {
                    return bone;
                }
            }

            return null;
        }

        public static float FindSplitT(Controller controller, LineRenderer line, string rendererObjectName)
        {
            return ResolveSplitT(controller, line, rendererObjectName);
        }

        private static float ProjectOntoLine(LineRenderer line, Vector3 worldPoint)
        {
            int count = line.positionCount;
            if (count < 2)
            {
                return 0.5f;
            }

            float bestDist = float.MaxValue;
            float bestAlong = 0f;
            float walked = 0f;
            Vector3 prev = line.useWorldSpace
                ? line.GetPosition(0)
                : line.transform.TransformPoint(line.GetPosition(0));

            for (int i = 1; i < count; i++)
            {
                Vector3 cur = line.useWorldSpace
                    ? line.GetPosition(i)
                    : line.transform.TransformPoint(line.GetPosition(i));
                float segLen = Vector3.Distance(prev, cur);
                Vector3 seg = cur - prev;
                if (segLen > 0.0001f)
                {
                    float t = Mathf.Clamp01(Vector3.Dot(worldPoint - prev, seg) / (segLen * segLen));
                    Vector3 closest = prev + seg * t;
                    float dist = Vector3.SqrMagnitude(worldPoint - closest);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestAlong = walked + segLen * t;
                    }

                    walked += segLen;
                }

                prev = cur;
            }

            if (walked <= 0.0001f)
            {
                return 0.5f;
            }

            return Mathf.Clamp(bestAlong / walked, 0.08f, 0.92f);
        }
    }
}
