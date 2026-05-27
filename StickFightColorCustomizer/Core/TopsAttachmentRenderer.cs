using System;
using System.Collections.Generic;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Tops are rendered as TWO half-sprites pinned to the spine's middle bone:
    ///   • Upper sprite — covers from mid → neck (top half of the shirt texture).
    ///   • Lower sprite — covers from hip → mid (bottom half of the shirt texture).
    /// Each half rotates with ITS OWN spine segment, so when the stickman bends the
    /// shirt visually creases at the waist instead of staying a flat billboard.
    /// </summary>
    public static class TopsAttachmentRenderer
    {
        private sealed class TopsInstance
        {
            public string TopsId;
            public LineRenderer Spine;
            public GameObject Root;
            public SpriteRenderer Upper;
            public SpriteRenderer Lower;
        }

        private static readonly Dictionary<int, TopsInstance> ByControllerId =
            new Dictionary<int, TopsInstance>();

        private const int TopsSortOrderBoost = 120;
        // Each half slightly overlaps the other so no seam shows at the waist.
        // 1.04 → 1.24 so each half extends a bit past the hip / neck. Gives the shirt
        // the "ligeramente más alto hacia arriba y abajo" feel the user asked for.
        private const float TopsHalfOverlap = 1.24f;
        // Narrow shirt that hugs the torso. The 32×144 source sprite still looks sharp.
        private const float TopsWidthFactor = 1.7f;
        private const float TopsWidthMin = 0.22f;
        private const float TopsWidthMax = 0.50f;

        public static void Apply(Controller controller, string topsId)
        {
            topsId = TopsCatalog.Normalize(topsId);
            if (controller == null || topsId == "none")
            {
                Remove(controller);
                return;
            }

            int id = controller.GetInstanceID();
            TopsInstance existing;
            if (ByControllerId.TryGetValue(id, out existing)
                && existing != null
                && existing.Root != null
                && existing.Upper != null && existing.Upper.gameObject != null
                && existing.Lower != null && existing.Lower.gameObject != null
                && existing.TopsId == topsId)
            {
                SyncSprite(controller, existing);
                return;
            }

            Remove(controller);

            LineRenderer spineLine = FindSpineLine(controller);
            if (spineLine == null) return;

            DestroyLegacyLineNodes(spineLine.transform);
            DestroyLegacySpriteRoot(spineLine.transform);

            GameObject root = new GameObject("SFCC_Tops");
            root.transform.SetParent(spineLine.transform, false);

            try { root.tag = "DontChangeColor"; } catch { }

            SpriteRenderer upper = CreateHalf(root.transform, "Upper",
                TopsSpriteFactory.GetUpperHalf(topsId), spineLine);
            SpriteRenderer lower = CreateHalf(root.transform, "Lower",
                TopsSpriteFactory.GetLowerHalf(topsId), spineLine);

            TopsInstance inst = new TopsInstance
            {
                TopsId = topsId,
                Spine = spineLine,
                Root = root,
                Upper = upper,
                Lower = lower
            };

            SyncSprite(controller, inst);
            ByControllerId[id] = inst;
        }

        private static SpriteRenderer CreateHalf(Transform parent, string name, Sprite sprite, LineRenderer spineLine)
        {
            GameObject go = new GameObject("SFCC_Tops_" + name);
            go.transform.SetParent(parent, false);
            try { go.tag = "DontChangeColor"; } catch { }
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite          = sprite;
            sr.sharedMaterial  = HatMeshFactory.GetHatMaterial();
            sr.color           = Color.white;
            sr.sortingLayerID  = spineLine.sortingLayerID;
            sr.sortingOrder    = spineLine.sortingOrder + TopsSortOrderBoost;
            sr.enabled         = true;
            return sr;
        }

        public static void ApplyRemoteTopsOnly(Controller controller, string topsId)
        {
            Apply(controller, topsId);
        }

        public static void Remove(Controller controller)
        {
            if (controller == null) return;
            int id = controller.GetInstanceID();
            TopsInstance inst;
            if (ByControllerId.TryGetValue(id, out inst) && inst != null)
            {
                if (inst.Root != null) UnityEngine.Object.Destroy(inst.Root);
                if (inst.Spine != null) DestroyLegacyLineNodes(inst.Spine.transform);
            }
            ByControllerId.Remove(id);
        }

        public static void ClearControllerCache(Controller controller) { Remove(controller); }

        public static void ClearAll()
        {
            foreach (KeyValuePair<int, TopsInstance> kv in ByControllerId)
            {
                if (kv.Value == null) continue;
                if (kv.Value.Root != null) UnityEngine.Object.Destroy(kv.Value.Root);
                if (kv.Value.Spine != null) DestroyLegacyLineNodes(kv.Value.Spine.transform);
            }
            ByControllerId.Clear();
            CosmeticAttachDiagnostics.Clear();
        }

        public static bool HasTopsForController(int controllerInstanceId)
        {
            TopsInstance inst;
            return ByControllerId.TryGetValue(controllerInstanceId, out inst)
                && inst != null
                && inst.Upper != null && inst.Upper.gameObject != null
                && inst.Lower != null && inst.Lower.gameObject != null;
        }

        public static void RefreshForController(Controller controller)
        {
            if (controller == null) return;
            int id = controller.GetInstanceID();
            TopsInstance inst;
            if (!ByControllerId.TryGetValue(id, out inst) || inst == null) return;
            LineRenderer spineLine = inst.Spine != null ? inst.Spine : FindSpineLine(controller);
            if (spineLine == null) return;
            inst.Spine = spineLine;
            SyncSprite(controller, inst);
        }

        public static void SyncForSetLine(SetLinePositions setLine)
        {
            if (setLine == null) return;
            if (!string.Equals(setLine.gameObject.name, "spineRenderer", StringComparison.OrdinalIgnoreCase))
                return;

            Controller controller = setLine.transform.root.GetComponent<Controller>();
            if (controller == null) return;

            int controllerId = controller.GetInstanceID();
            TopsInstance inst;
            if (!ByControllerId.TryGetValue(controllerId, out inst) || inst == null)
            {
                if (ColorCustomizerApp.Instance == null
                    || !ModFeatureGate.IsTopsActive(ColorCustomizerApp.Instance.Config))
                    return;
                Apply(controller, ColorCustomizerApp.Instance.Config.Tops.TopsId);
                return;
            }

            LineRenderer spineLine = FindSpineLine(controller);
            if (spineLine == null) return;
            inst.Spine = spineLine;
            SyncSprite(controller, inst);
        }

        // ── Per-frame sync: position / rotation / scale for both halves ───────────
        private static void SyncSprite(Controller controller, TopsInstance inst)
        {
            if (inst == null || inst.Upper == null || inst.Lower == null) return;
            LineRenderer spine = inst.Spine;
            if (spine == null) return;

            int count = StickLineUtil.GetVertexCount(spine);
            if (count < 2) return;

            // Hip = lowest-Y endpoint, Neck = highest-Y endpoint.
            int hipIdx = 0, neckIdx = 0;
            float minY = float.MaxValue, maxY = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                float y = StickLineUtil.GetVertexWorld(spine, i).y;
                if (y < minY) { minY = y; hipIdx = i; }
                if (y > maxY) { maxY = y; neckIdx = i; }
            }
            Vector3 worldHip  = StickLineUtil.GetVertexWorld(spine, hipIdx);
            Vector3 worldNeck = StickLineUtil.GetVertexWorld(spine, neckIdx);

            // Middle anchor — the interior vertex closest to the half-Y of the spine.
            // For a 2-vertex spine, falls back to the midpoint (no bend possible).
            Vector3 worldMid;
            if (count >= 3)
            {
                float halfY = (minY + maxY) * 0.5f;
                int midIdx = -1;
                float bestDist = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    if (i == hipIdx || i == neckIdx) continue;
                    Vector3 v = StickLineUtil.GetVertexWorld(spine, i);
                    float d = Mathf.Abs(v.y - halfY);
                    if (d < bestDist) { bestDist = d; midIdx = i; }
                }
                worldMid = midIdx >= 0
                    ? StickLineUtil.GetVertexWorld(spine, midIdx)
                    : (worldHip + worldNeck) * 0.5f;
            }
            else
            {
                worldMid = (worldHip + worldNeck) * 0.5f;
            }

            Camera cam = Camera.main;
            Vector3 spriteForward = cam != null ? -cam.transform.forward : Vector3.back;

            // Width target — same for both halves so the shirt has uniform width.
            float targetW = Mathf.Clamp(spine.widthMultiplier * TopsWidthFactor,
                TopsWidthMin, TopsWidthMax);

            // ── Upper half: mid → neck ──
            Vector3 upperVec = worldNeck - worldMid;
            float upperLen = upperVec.magnitude;
            if (upperLen > 0.01f)
            {
                Vector3 upperDir = upperVec / upperLen;
                inst.Upper.transform.position = worldMid;
                inst.Upper.transform.rotation = Quaternion.LookRotation(spriteForward, upperDir);

                Sprite upSprite = inst.Upper.sprite != null
                    ? inst.Upper.sprite
                    : TopsSpriteFactory.GetUpperHalf(inst.TopsId);
                if (upSprite != null)
                {
                    float sh = Mathf.Max(upSprite.bounds.size.y, 0.01f);
                    float sw = Mathf.Max(upSprite.bounds.size.x, 0.01f);
                    inst.Upper.transform.localScale = new Vector3(
                        targetW / sw,
                        upperLen * TopsHalfOverlap / sh,
                        1f);
                    inst.Upper.sprite = upSprite;
                }
                inst.Upper.sortingLayerID = spine.sortingLayerID;
                inst.Upper.sortingOrder   = spine.sortingOrder + TopsSortOrderBoost;
                inst.Upper.color          = Color.white;
                inst.Upper.enabled        = true;
            }

            // ── Lower half: hip → mid ──
            Vector3 lowerVec = worldMid - worldHip;
            float lowerLen = lowerVec.magnitude;
            if (lowerLen > 0.01f)
            {
                Vector3 lowerDir = lowerVec / lowerLen;
                inst.Lower.transform.position = worldMid;
                // Same up-vector (mid→hip is the opposite, but we want the lower sprite's
                // pivot=(0.5,1) so it grows downward from mid toward hip). With
                // transform.up = (mid→neck reversed) = (hip→mid)... actually we want
                // localScale.y = lowerLen so the sprite hangs that far. Pivot at top means
                // local -Y direction matches lowerDir reversed. Use Quaternion that aligns
                // transform.up = lowerDir (pointing from hip TOWARDS mid, i.e. roughly up).
                inst.Lower.transform.rotation = Quaternion.LookRotation(spriteForward, lowerDir);

                Sprite loSprite = inst.Lower.sprite != null
                    ? inst.Lower.sprite
                    : TopsSpriteFactory.GetLowerHalf(inst.TopsId);
                if (loSprite != null)
                {
                    float sh = Mathf.Max(loSprite.bounds.size.y, 0.01f);
                    float sw = Mathf.Max(loSprite.bounds.size.x, 0.01f);
                    inst.Lower.transform.localScale = new Vector3(
                        targetW / sw,
                        lowerLen * TopsHalfOverlap / sh,
                        1f);
                    inst.Lower.sprite = loSprite;
                }
                inst.Lower.sortingLayerID = spine.sortingLayerID;
                inst.Lower.sortingOrder   = spine.sortingOrder + TopsSortOrderBoost;
                inst.Lower.color          = Color.white;
                inst.Lower.enabled        = true;
            }
        }

        // ── Spine lookup (unchanged) ─────────────────────────────────────────────
        private static Transform FindSpineNode(Controller controller)
        {
            Transform renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform);
            return renderersRoot != null
                ? PlayerColorApplier.FindRendererTransform(renderersRoot, "spineRenderer")
                : null;
        }

        private static LineRenderer FindSpineLine(Controller controller)
        {
            Transform spineNode = FindSpineNode(controller);
            if (spineNode == null) return null;

            SetLinePositions setLine = spineNode.GetComponent<SetLinePositions>();
            if (setLine != null)
            {
                LineRenderer fromSetLine = CosmeticBoneLineMirror.TryGetSourceLine(setLine);
                if (fromSetLine != null) return fromSetLine;
            }

            LineRenderer best = CosmeticLineAttachUtil.PickBestLimbLine(spineNode);
            if (best != null) return best;

            LineRenderer[] lines = spineNode.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer l = lines[i];
                if (l != null && !l.gameObject.name.StartsWith("SFCC_"))
                    return l;
            }
            return null;
        }

        // ── Cleanup of legacy nodes ──────────────────────────────────────────────
        private static void DestroyLegacyLineNodes(Transform spineBone)
        {
            if (spineBone == null) return;
            Transform t = spineBone.Find("SFCC_Tops_Line");
            if (t != null) UnityEngine.Object.Destroy(t.gameObject);
            t = spineBone.Find("SFCC_Tops_Solid");
            if (t != null) UnityEngine.Object.Destroy(t.gameObject);
            t = spineBone.Find("SFCC_Tops_Sprite");
            if (t != null) UnityEngine.Object.Destroy(t.gameObject);
        }

        private static void DestroyLegacySpriteRoot(Transform spineBone)
        {
            if (spineBone == null) return;
            Transform legacy = spineBone.parent != null
                ? spineBone.parent.Find("SFCC_Tops")
                : null;
            if (legacy == null) legacy = spineBone.Find("SFCC_Tops");
            if (legacy != null) UnityEngine.Object.Destroy(legacy.gameObject);
        }
    }
}
