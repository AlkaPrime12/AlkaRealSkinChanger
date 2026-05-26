using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Sprite en el hueso LineRenderer: posición/rotación/escala desde vértices cada frame (como el sombrero).
    /// </summary>
    public static class CosmeticBoneSpriteAttach
    {
        public struct BoneSegment
        {
            public Vector3 LocalPosition;
            public Vector3 LocalDirection;
            public float SegmentLength;
            public float BoneRadius;
        }

        private const float DepthWorldTowardCamera = 0.08f;
        private const float RadiusFactor = 2.6f;
        private const float MinRadius = 0.22f;

        public static bool TryGetSpineChestSegment(LineRenderer spine, float chestLerp, out BoneSegment segment)
        {
            segment = default(BoneSegment);
            if (spine == null)
            {
                return false;
            }

            int count = StickLineUtil.GetVertexCount(spine);
            if (count < 2)
            {
                return false;
            }

            int hipIdx = CosmeticLineAttachUtil.FindLowestAlongDownAxis(spine);
            int neckIdx = CosmeticLineAttachUtil.FindHighestAlongDownAxis(spine);
            if (hipIdx == neckIdx && count > 2)
            {
                hipIdx = 0;
                neckIdx = count - 1;
            }

            Vector3 localHip = StickLineUtil.GetVertexLocal(spine, hipIdx);
            Vector3 localNeck = StickLineUtil.GetVertexLocal(spine, neckIdx);
            Vector3 along = localNeck - localHip;
            if (along.sqrMagnitude < 0.0001f)
            {
                along = Vector3.up;
            }

            float length = along.magnitude;
            Vector3 dir = along / Mathf.Max(length, 0.0001f);
            Vector3 chest = Vector3.Lerp(localHip, localNeck, Mathf.Clamp01(chestLerp));
            chest = AddDepthTowardCameraLocal(spine.transform, chest, DepthWorldTowardCamera);

            segment.LocalPosition = chest;
            segment.LocalDirection = dir;
            segment.SegmentLength = length;
            segment.BoneRadius = Mathf.Max(spine.widthMultiplier * RadiusFactor, MinRadius);
            return true;
        }

        public static bool TryGetFootSegment(LineRenderer leg, out BoneSegment segment)
        {
            segment = default(BoneSegment);
            if (leg == null)
            {
                return false;
            }

            int count = StickLineUtil.GetVertexCount(leg);
            if (count < 1)
            {
                return false;
            }

            float radius = Mathf.Max(leg.widthMultiplier * RadiusFactor, MinRadius);
            Vector3 downLocal = GetDownLocal(leg);

            int footIdx = CosmeticLineAttachUtil.FindFootVertexIndex(leg);
            int topIdx = count >= 2
                ? CosmeticLineAttachUtil.FindHighestAlongDownAxis(leg)
                : footIdx;
            if (footIdx == topIdx && count >= 2)
            {
                topIdx = footIdx == 0 ? count - 1 : 0;
            }

            Vector3 localFoot = StickLineUtil.GetVertexLocal(leg, footIdx);
            Vector3 localTop = StickLineUtil.GetVertexLocal(leg, topIdx);
            Vector3 along = localFoot - localTop;
            if (along.sqrMagnitude < 0.0001f)
            {
                along = downLocal;
            }

            float length = along.magnitude;
            Vector3 dir = along / Mathf.Max(length, 0.0001f);

            Vector3 pos = localFoot;
            if (length >= 0.12f)
            {
                pos += dir * (radius * 0.1f);
            }
            else
            {
                float span = ComputeLineSpan(leg);
                pos = localTop + (downLocal * Mathf.Max(span * 0.92f, radius * 2.2f));
            }

            pos += downLocal * (radius * 0.08f);
            pos = AddDepthTowardCameraLocal(leg.transform, pos, DepthWorldTowardCamera);

            segment.LocalPosition = pos;
            segment.LocalDirection = dir;
            segment.SegmentLength = Mathf.Max(length, radius * 2f);
            segment.BoneRadius = radius;
            return true;
        }

        private static Vector3 GetDownLocal(LineRenderer leg)
        {
            Vector3 downLocal = Vector3.down;
            Camera cam = Camera.main;
            if (cam != null && leg != null)
            {
                downLocal = leg.transform.InverseTransformDirection(-cam.transform.up);
            }

            downLocal.x = 0f;
            if (downLocal.sqrMagnitude < 0.0001f)
            {
                downLocal = Vector3.down;
            }

            downLocal.Normalize();
            return downLocal;
        }

        private static float ComputeLineSpan(LineRenderer line)
        {
            int count = StickLineUtil.GetVertexCount(line);
            if (count < 2)
            {
                return Mathf.Max(line.widthMultiplier * RadiusFactor * 2f, MinRadius * 2f);
            }

            float maxDist = 0f;
            for (int i = 0; i < count; i++)
            {
                Vector3 a = StickLineUtil.GetVertexLocal(line, i);
                for (int j = i + 1; j < count; j++)
                {
                    float d = (StickLineUtil.GetVertexLocal(line, j) - a).magnitude;
                    if (d > maxDist)
                    {
                        maxDist = d;
                    }
                }
            }

            return Mathf.Max(maxDist, MinRadius);
        }

        private static Vector3 AddDepthTowardCameraLocal(Transform bone, Vector3 localPos, float worldAmount)
        {
            if (bone == null)
            {
                return localPos;
            }

            Camera cam = Camera.main;
            if (cam == null)
            {
                return localPos;
            }

            Vector3 worldOffset = -cam.transform.forward * worldAmount;
            Vector3 localOffset = bone.InverseTransformVector(worldOffset);
            return localPos + localOffset;
        }

        public static void SyncTorsoSprite(
            Transform boneParent,
            SpriteRenderer renderer,
            Sprite sprite,
            BoneSegment segment,
            float widthScale,
            float heightScale)
        {
            if (boneParent == null || renderer == null || sprite == null)
            {
                return;
            }

            if (renderer.transform.parent != boneParent)
            {
                renderer.transform.SetParent(boneParent, false);
            }

            renderer.sprite = sprite;
            renderer.flipX = false;
            renderer.color = Color.white;
            renderer.enabled = true;

            Bounds bounds = sprite.bounds;
            float spriteH = Mathf.Max(bounds.size.y, 0.01f);
            float spriteW = Mathf.Max(bounds.size.x, 0.01f);

            float targetH = segment.SegmentLength * heightScale * 0.72f;
            float targetW = segment.BoneRadius * 1.45f * widthScale;

            renderer.transform.localPosition = segment.LocalPosition;
            renderer.transform.localRotation = Quaternion.identity;
            renderer.transform.localScale = new Vector3(
                targetW / spriteW,
                targetH / spriteH,
                1f);
            CosmeticBillboard.FaceMainCamera(renderer.transform);
        }

        public static void SyncFootSprite(
            Transform boneParent,
            SpriteRenderer renderer,
            Sprite sprite,
            BoneSegment segment,
            float widthScale,
            float heightScale,
            bool flipX)
        {
            if (boneParent == null || renderer == null || sprite == null)
            {
                return;
            }

            if (renderer.transform.parent != boneParent)
            {
                renderer.transform.SetParent(boneParent, false);
            }

            renderer.sprite = sprite;
            renderer.flipX = flipX;
            renderer.color = Color.white;
            renderer.enabled = true;

            Bounds bounds = sprite.bounds;
            float spriteW = Mathf.Max(bounds.size.x, 0.01f);
            float spriteH = Mathf.Max(bounds.size.y, 0.01f);

            float targetW = segment.BoneRadius * 1.55f * widthScale;
            float targetH = segment.BoneRadius * 0.72f * heightScale;

            renderer.transform.localPosition = segment.LocalPosition;
            renderer.transform.localRotation = Quaternion.identity;
            renderer.transform.localScale = new Vector3(
                targetW / spriteW,
                targetH / spriteH,
                1f);
            CosmeticBillboard.FaceMainCamera(renderer.transform);
        }

        public static SpriteRenderer EnsureSpriteRenderer(GameObject root, LineRenderer bone, int sortingOrderBoost)
        {
            if (root == null || bone == null)
            {
                return null;
            }

            SpriteRenderer renderer = root.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = root.AddComponent<SpriteRenderer>();
            }

            renderer.sharedMaterial = HatMeshFactory.GetHatMaterial();
            renderer.sortingLayerID = bone.sortingLayerID;
            renderer.sortingOrder = bone.sortingOrder + sortingOrderBoost;
            return renderer;
        }

        public static GameObject CreateSpriteRoot(LineRenderer bone, string objectName)
        {
            GameObject go = new GameObject(objectName);
            go.transform.SetParent(bone.transform, false);
            go.transform.localRotation = Quaternion.identity;

            try
            {
                go.tag = "DontChangeColor";
            }
            catch
            {
            }

            return go;
        }
    }
}
