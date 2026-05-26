using System;
using System.Collections.Generic;
using StickFightColorCustomizer.Models;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Top: línea espejo con textura del sprite (sigue la curva de la espina).
    /// </summary>
    public static class TopsAttachmentRenderer
    {
        private sealed class TopsInstance
        {
            public string TopsId;
            public LineRenderer Spine;
            public LineRenderer Mirror;
            public LineRenderer SolidUnderlay;
        }

        private static readonly Dictionary<int, TopsInstance> ByControllerId =
            new Dictionary<int, TopsInstance>();

        private const int TopsSortOrderBoost = 120;
        private const int TopsSolidUnderlayBoost = 118;
        private const float MinWidthMult = 1.15f;
        private const float MaxWidthMult = 1.75f;
        private const float SolidUnderlayWidthFactor = 1.02f;

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
                && existing.Mirror != null
                && existing.TopsId == topsId)
            {
                RefreshForController(controller);
                return;
            }

            Remove(controller);

            LineRenderer spineLine = FindSpineLine(controller);
            if (spineLine == null)
            {
                return;
            }

            DestroyLegacySpriteRoot(spineLine.transform);
            LineRenderer solid = CosmeticBoneLineMirror.GetOrCreate(spineLine, "SFCC_Tops_Solid");
            LineRenderer mirror = CosmeticBoneLineMirror.GetOrCreate(spineLine, "SFCC_Tops_Line");
            TopsInstance inst = new TopsInstance
            {
                TopsId = topsId,
                Spine = spineLine,
                Mirror = mirror,
                SolidUnderlay = solid
            };

            SyncTopLine(spineLine, solid, mirror, topsId);
            ByControllerId[id] = inst;
        }

        public static void ApplyRemoteTopsOnly(Controller controller, string topsId)
        {
            Apply(controller, topsId);
        }

        public static void Remove(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int id = controller.GetInstanceID();
            TopsInstance inst;
            if (ByControllerId.TryGetValue(id, out inst) && inst != null)
            {
                if (inst.Spine != null)
                {
                    CosmeticBoneLineMirror.DestroyMirror(inst.Spine);
                }
            }

            ByControllerId.Remove(id);
        }

        public static void ClearControllerCache(Controller controller)
        {
            Remove(controller);
        }

        public static void ClearAll()
        {
            foreach (KeyValuePair<int, TopsInstance> kv in ByControllerId)
            {
                if (kv.Value != null && kv.Value.Spine != null)
                {
                    CosmeticBoneLineMirror.DestroyMirror(kv.Value.Spine);
                }
            }

            ByControllerId.Clear();
            CosmeticAttachDiagnostics.Clear();
        }

        public static bool HasTopsForController(int controllerInstanceId)
        {
            TopsInstance inst;
            return ByControllerId.TryGetValue(controllerInstanceId, out inst)
                && inst != null && inst.Mirror != null;
        }

        public static void RefreshForController(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int id = controller.GetInstanceID();
            TopsInstance inst;
            if (!ByControllerId.TryGetValue(id, out inst) || inst == null)
            {
                return;
            }

            LineRenderer spineLine = inst.Spine != null ? inst.Spine : FindSpineLine(controller);
            if (spineLine == null || inst.Mirror == null)
            {
                return;
            }

            inst.Spine = spineLine;
            if (inst.SolidUnderlay == null)
            {
                inst.SolidUnderlay = CosmeticBoneLineMirror.GetOrCreate(spineLine, "SFCC_Tops_Solid");
            }

            SyncTopLine(spineLine, inst.SolidUnderlay, inst.Mirror, inst.TopsId);
        }

        public static void SyncForSetLine(SetLinePositions setLine)
        {
            if (setLine == null)
            {
                return;
            }

            if (!string.Equals(setLine.gameObject.name, "spineRenderer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Controller controller = setLine.transform.root.GetComponent<Controller>();
            if (controller == null)
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            TopsInstance inst;
            if (!ByControllerId.TryGetValue(controllerId, out inst) || inst == null)
            {
                if (ColorCustomizerApp.Instance == null
                    || !ModFeatureGate.IsTopsActive(ColorCustomizerApp.Instance.Config))
                {
                    return;
                }

                Apply(controller, ColorCustomizerApp.Instance.Config.Tops.TopsId);
                return;
            }

            LineRenderer spineLine = FindSpineLine(controller);
            if (spineLine == null || inst.Mirror == null)
            {
                return;
            }

            inst.Spine = spineLine;
            if (inst.SolidUnderlay == null)
            {
                inst.SolidUnderlay = CosmeticBoneLineMirror.GetOrCreate(spineLine, "SFCC_Tops_Solid");
            }

            SyncTopLine(spineLine, inst.SolidUnderlay, inst.Mirror, inst.TopsId);
        }

        private static void SyncTopLine(LineRenderer spine, LineRenderer solid, LineRenderer mirror, string topsId)
        {
            if (spine == null || mirror == null)
            {
                return;
            }

            Sprite sprite = TopsSpriteFactory.GetSprite(topsId);
            if (sprite == null)
            {
                return;
            }

            float widthMult = ComputeTopsWidthMultiplier(spine, sprite, topsId);
            Color tint = TopsSpriteFactory.GetLineTint(topsId);

            if (solid != null)
            {
                CosmeticBoneLineMirror.SyncSolidOverlay(
                    spine,
                    solid,
                    tint,
                    widthMult * SolidUnderlayWidthFactor,
                    TopsSolidUnderlayBoost);
            }

            CosmeticBoneLineMirror.SyncSpriteTexturedOverlay(
                spine,
                mirror,
                sprite,
                widthMult,
                TopsSortOrderBoost);
        }

        private static float ComputeTopsWidthMultiplier(LineRenderer spine, Sprite sprite, string topsId)
        {
            float spriteThin = Mathf.Max(sprite.bounds.size.x, 0.01f);
            float spineW = spine != null ? Mathf.Max(spine.widthMultiplier, 0.05f) : 0.2f;
            float ratio = (spineW * 1.18f * TopsSpriteFactory.GetWidthScale(topsId)) / spriteThin;
            return Mathf.Clamp(ratio, MinWidthMult, MaxWidthMult);
        }

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
            if (spineNode == null)
            {
                return null;
            }

            SetLinePositions setLine = spineNode.GetComponent<SetLinePositions>();
            if (setLine != null)
            {
                LineRenderer fromSetLine = CosmeticBoneLineMirror.TryGetSourceLine(setLine);
                if (fromSetLine != null)
                {
                    return fromSetLine;
                }
            }

            LineRenderer best = CosmeticLineAttachUtil.PickBestLimbLine(spineNode);
            if (best != null)
            {
                return best;
            }

            LineRenderer[] lines = spineNode.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer l = lines[i];
                if (l != null && !l.gameObject.name.StartsWith("SFCC_"))
                {
                    return l;
                }
            }

            return null;
        }

        private static void DestroyLegacySpriteRoot(Transform spineBone)
        {
            if (spineBone == null)
            {
                return;
            }

            Transform legacy = spineBone.parent != null
                ? spineBone.parent.Find("SFCC_Tops")
                : null;
            if (legacy == null)
            {
                legacy = spineBone.Find("SFCC_Tops");
            }

            Transform spineNode = FindSpineNodeFromBone(spineBone);
            if (legacy == null && spineNode != null)
            {
                legacy = spineNode.Find("SFCC_Tops");
            }

            if (legacy != null)
            {
                UnityEngine.Object.Destroy(legacy.gameObject);
            }
        }

        private static Transform FindSpineNodeFromBone(Transform spineBone)
        {
            Transform t = spineBone;
            while (t != null)
            {
                if (string.Equals(t.name, "spineRenderer", StringComparison.OrdinalIgnoreCase))
                {
                    return t;
                }

                t = t.parent;
            }

            return null;
        }
    }
}
