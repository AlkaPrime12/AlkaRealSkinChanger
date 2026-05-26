using System;
using System.Collections.Generic;
using StickFightColorCustomizer.Models;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Zapatos en el render del pie (LeftLeg/RightLeg): holder neutraliza escala del hueso + sprite visible.
    /// Fallback: anchor en Controller con posición mundo.
    /// </summary>
    public static class ShoeAttachmentRenderer
    {
        private sealed class ShoeFootMount
        {
            public LineRenderer FootLine;
            public GameObject Holder;
            public GameObject ShoeRoot;
        }

        private sealed class ShoeInstance
        {
            public string ShoeId;
            public GameObject Anchor;
            public ShoeFootMount Left;
            public ShoeFootMount Right;
        }

        private struct FootAttachPoint
        {
            public bool Valid;
            public LineRenderer FootLine;
            public Vector3 LocalPositionOnFoot;
            public Vector3 WorldPosition;
            public int SortingLayerId;
            public int SortingOrder;
            public float FootRadius;
        }

        private static readonly Dictionary<int, ShoeInstance> ByControllerId =
            new Dictionary<int, ShoeInstance>();

        private const int ShoeSortOrderBoost = 12;
        // Bumped a hair from the very-small 0.15..0.34 → 0.20..0.42 (still smaller
        // than the original 0.25..0.50 the user complained about).
        private const float MinWorldScale = 0.20f;
        private const float MaxWorldScale = 0.42f;

        public static void Apply(Controller controller, string shoeId)
        {
            shoeId = ShoeCatalog.Normalize(shoeId);
            if (controller == null || shoeId == "none")
            {
                Remove(controller);
                return;
            }

            int controllerId = controller.GetInstanceID();
            ShoeInstance existing;
            if (ByControllerId.TryGetValue(controllerId, out existing)
                && existing != null
                && existing.ShoeId == shoeId
                && HasAnyFoot(existing))
            {
                RefreshForController(controller);
                return;
            }

            Remove(controller);

            ShoeInstance inst = new ShoeInstance
            {
                ShoeId = shoeId,
                Anchor = EnsureShoeAnchor(controller)
            };

            TryCreateFoot(controller, false, shoeId, inst);
            TryCreateFoot(controller, true, shoeId, inst);
            ByControllerId[controllerId] = inst;
        }

        public static void ApplyRemoteShoeOnly(Controller controller, string shoeId)
        {
            Apply(controller, shoeId);
        }

        public static void Remove(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            ShoeInstance inst;
            if (ByControllerId.TryGetValue(controllerId, out inst) && inst != null)
            {
                DestroyFootMount(inst.Left);
                DestroyFootMount(inst.Right);
                DestroyObject(inst.Anchor);
            }

            ByControllerId.Remove(controllerId);
        }

        public static void ClearControllerCache(Controller controller)
        {
            Remove(controller);
        }

        public static void ClearAll()
        {
            foreach (KeyValuePair<int, ShoeInstance> kv in ByControllerId)
            {
                if (kv.Value != null)
                {
                    DestroyFootMount(kv.Value.Left);
                    DestroyFootMount(kv.Value.Right);
                    DestroyObject(kv.Value.Anchor);
                }
            }

            ByControllerId.Clear();
            CosmeticAttachDiagnostics.Clear();
        }

        public static bool HasShoesForController(int controllerInstanceId)
        {
            ShoeInstance inst;
            return ByControllerId.TryGetValue(controllerInstanceId, out inst) && HasAnyFoot(inst);
        }

        public static void RefreshForController(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            ShoeInstance inst;
            if (!ByControllerId.TryGetValue(controllerId, out inst) || inst == null)
            {
                return;
            }

            if (inst.Anchor == null)
            {
                inst.Anchor = EnsureShoeAnchor(controller);
            }

            if (inst.Left != null && inst.Left.ShoeRoot != null)
            {
                SyncShoeSprite(controller, inst, inst.Left, false, inst.ShoeId);
            }

            if (inst.Right != null && inst.Right.ShoeRoot != null)
            {
                SyncShoeSprite(controller, inst, inst.Right, true, inst.ShoeId);
            }
        }

        public static void SyncForSetLine(SetLinePositions setLine)
        {
            if (setLine == null)
            {
                return;
            }

            bool isRight;
            if (!TryResolveLegSide(setLine, out isRight))
            {
                return;
            }

            Controller controller = setLine.transform.root.GetComponent<Controller>();
            if (controller == null)
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            ShoeInstance inst;
            if (!ByControllerId.TryGetValue(controllerId, out inst) || inst == null)
            {
                if (ColorCustomizerApp.Instance == null
                    || !ModFeatureGate.IsShoeActive(ColorCustomizerApp.Instance.Config))
                {
                    return;
                }

                Apply(controller, ColorCustomizerApp.Instance.Config.Shoe.ShoeId);
                return;
            }

            if (inst.Anchor == null)
            {
                inst.Anchor = EnsureShoeAnchor(controller);
            }

            if (isRight)
            {
                if (inst.Right == null || inst.Right.ShoeRoot == null)
                {
                    TryCreateFoot(controller, true, inst.ShoeId, inst);
                }

                if (inst.Right != null && inst.Right.ShoeRoot != null)
                {
                    SyncShoeSprite(controller, inst, inst.Right, true, inst.ShoeId);
                }
            }
            else
            {
                if (inst.Left == null || inst.Left.ShoeRoot == null)
                {
                    TryCreateFoot(controller, false, inst.ShoeId, inst);
                }

                if (inst.Left != null && inst.Left.ShoeRoot != null)
                {
                    SyncShoeSprite(controller, inst, inst.Left, false, inst.ShoeId);
                }
            }
        }

        private static GameObject EnsureShoeAnchor(Controller controller)
        {
            if (controller == null)
            {
                return null;
            }

            Transform existing = controller.transform.Find("SFCC_Shoes_Anchor");
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject anchor = new GameObject("SFCC_Shoes_Anchor");
            anchor.transform.SetParent(controller.transform, false);
            anchor.transform.localPosition = Vector3.zero;
            anchor.transform.localRotation = Quaternion.identity;
            anchor.transform.localScale = Vector3.one;
            return anchor;
        }

        private static void TryCreateFoot(Controller controller, bool isRight, string shoeId, ShoeInstance inst)
        {
            if (controller == null || inst == null)
            {
                return;
            }

            Transform legNode = FindLegNode(controller, isRight);
            if (legNode != null)
            {
                DestroyLegacyLineOverlay(legNode, isRight ? "SFCC_Shoe_R_Line" : "SFCC_Shoe_L_Line");
            }

            if (inst.Anchor == null)
            {
                inst.Anchor = EnsureShoeAnchor(controller);
            }

            string name = isRight ? "SFCC_Shoe_R" : "SFCC_Shoe_L";
            GameObject root = new GameObject(name);

            try
            {
                root.tag = "DontChangeColor";
            }
            catch
            {
            }

            root.AddComponent<SpriteRenderer>().sharedMaterial = HatMeshFactory.GetHatMaterial();
            ShoeFootMount mount = new ShoeFootMount { ShoeRoot = root };
            SyncShoeSprite(controller, inst, mount, isRight, shoeId);

            if (isRight)
            {
                inst.Right = mount;
            }
            else
            {
                inst.Left = mount;
            }
        }

        private static void SyncShoeSprite(Controller controller, ShoeInstance inst, ShoeFootMount mount, bool isRight, string shoeId)
        {
            if (controller == null || inst == null || mount == null || mount.ShoeRoot == null)
            {
                return;
            }

            GameObject root = mount.ShoeRoot;
            Sprite sprite = ShoeSpriteFactory.GetSprite(shoeId);
            if (sprite == null)
            {
                return;
            }

            FootAttachPoint attach;
            if (!TryResolveFootAttach(controller, isRight, out attach) || !attach.Valid)
            {
                SpriteRenderer hidden = root.GetComponent<SpriteRenderer>();
                if (hidden != null)
                {
                    hidden.enabled = false;
                }

                return;
            }

            SpriteRenderer renderer = root.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = root.AddComponent<SpriteRenderer>();
            }

            renderer.sharedMaterial = HatMeshFactory.GetHatMaterial();
            renderer.sprite = sprite;
            renderer.flipX = isRight;
            renderer.color = Color.white;
            renderer.enabled = true;
            renderer.sortingLayerID = attach.SortingLayerId;
            renderer.sortingOrder = attach.SortingOrder;

            if (attach.FootLine != null && CanUseFootHolder(attach.FootLine.transform))
            {
                mount.FootLine = attach.FootLine;
                EnsureFootHolder(mount, isRight);
                if (mount.Holder != null)
                {
                    mount.Holder.transform.localPosition = attach.LocalPositionOnFoot;
                    ApplyNeutralHolderScale(mount.Holder.transform, attach.FootLine.transform);
                    if (root.transform.parent != mount.Holder.transform)
                    {
                        root.transform.SetParent(mount.Holder.transform, false);
                    }

                    root.transform.localPosition = Vector3.zero;
                    root.transform.localRotation = Quaternion.identity;
                    ApplyShoeScale(root.transform, sprite, attach.FootRadius, shoeId);

                    Camera cam = Camera.main;
                    if (cam != null)
                    {
                        root.transform.rotation = cam.transform.rotation;
                    }

                    return;
                }
            }

            if (inst.Anchor == null)
            {
                inst.Anchor = EnsureShoeAnchor(controller);
            }

            if (inst.Anchor != null)
            {
                if (root.transform.parent != inst.Anchor.transform)
                {
                    root.transform.SetParent(inst.Anchor.transform, false);
                }

                root.transform.position = attach.WorldPosition;
                root.transform.localRotation = Quaternion.identity;
                ApplyShoeScale(root.transform, sprite, attach.FootRadius, shoeId);

                Camera cam = Camera.main;
                if (cam != null)
                {
                    root.transform.rotation = cam.transform.rotation;
                }

            }
        }

        private static bool CanUseFootHolder(Transform footBone)
        {
            if (footBone == null)
            {
                return false;
            }

            Vector3 lossy = footBone.lossyScale;
            if (float.IsNaN(lossy.x) || float.IsInfinity(lossy.x)
                || float.IsNaN(lossy.y) || float.IsInfinity(lossy.y))
            {
                return false;
            }

            float ax = Mathf.Abs(lossy.x);
            float ay = Mathf.Abs(lossy.y);
            return ax > 0.001f && ay > 0.001f && ax < 50f && ay < 50f;
        }

        private static void EnsureFootHolder(ShoeFootMount mount, bool isRight)
        {
            if (mount == null || mount.FootLine == null)
            {
                return;
            }

            string holderName = isRight ? "SFCC_Shoe_Holder_R" : "SFCC_Shoe_Holder_L";
            Transform footBone = mount.FootLine.transform;
            Transform existing = footBone.Find(holderName);
            if (existing != null)
            {
                mount.Holder = existing.gameObject;
                return;
            }

            if (mount.Holder != null)
            {
                UnityEngine.Object.Destroy(mount.Holder);
            }

            GameObject holder = new GameObject(holderName);
            holder.transform.SetParent(footBone, false);
            holder.transform.localRotation = Quaternion.identity;
            mount.Holder = holder;
        }

        private static void ApplyNeutralHolderScale(Transform holder, Transform footBone)
        {
            if (holder == null || footBone == null)
            {
                return;
            }

            Vector3 lossy = footBone.lossyScale;
            holder.localScale = new Vector3(
                1f / Mathf.Max(Mathf.Abs(lossy.x), 0.001f),
                1f / Mathf.Max(Mathf.Abs(lossy.y), 0.001f),
                1f);
        }

        private static void ApplyShoeScale(Transform shoeTransform, Sprite sprite, float footRadius, string shoeId)
        {
            Bounds bounds = sprite.bounds;
            float spriteW = Mathf.Max(bounds.size.x, 0.01f);
            float spriteH = Mathf.Max(bounds.size.y, 0.01f);

            float targetW = Mathf.Clamp(
                footRadius * 2.8f * ShoeSpriteFactory.GetWidthScale(shoeId),
                MinWorldScale,
                MaxWorldScale);
            float targetH = Mathf.Clamp(
                footRadius * 1.2f * ShoeSpriteFactory.GetHeightScale(shoeId),
                MinWorldScale * 0.55f,
                MaxWorldScale);

            shoeTransform.localScale = new Vector3(
                targetW / spriteW,
                targetH / spriteH,
                1f);
        }

        private static bool TryResolveFootAttach(Controller controller, bool isRight, out FootAttachPoint attach)
        {
            attach = default(FootAttachPoint);

            Transform legNode = FindLegNode(controller, isRight);
            if (legNode == null)
            {
                return false;
            }

            LineRenderer footLine;
            if (!CosmeticLineAttachUtil.TryResolveFootLine(legNode, isRight, out footLine)
                || footLine == null
                || StickLineUtil.GetVertexCount(footLine) < 1)
            {
                Vector3 worldFoot;
                float radius;
                if (!CosmeticLineAttachUtil.TryGetDistalFootWorld(legNode, isRight, out worldFoot, out radius)
                    && !CosmeticLineAttachUtil.TryGetLegFootWorld(legNode, isRight, out worldFoot, out radius))
                {
                    return false;
                }

                attach.WorldPosition = worldFoot;
                attach.FootRadius = radius;
                attach.SortingLayerId = 0;
                attach.SortingOrder = ShoeSortOrderBoost;
                attach.Valid = true;
                return true;
            }

            Vector3 worldFootPos = CosmeticLineAttachUtil.GetFootWorldPosition(footLine);

            attach.FootLine = footLine;
            // Pass the controller so the endpoint (foot vs hip) is selected topologically,
            // not by world-Y. This stays correct in mid-jump/kick poses where the foot can
            // be above the hip (the bug that pulled the shoe to the torso in jumps).
            attach.LocalPositionOnFoot = CosmeticLineAttachUtil.GetFootLocalOnLine(footLine, 0.97f, controller);
            attach.WorldPosition = worldFootPos;
            attach.FootRadius = Mathf.Max(footLine.widthMultiplier * 2.6f, 0.22f);
            attach.SortingLayerId = footLine.sortingLayerID;
            attach.SortingOrder = footLine.sortingOrder + ShoeSortOrderBoost;
            attach.Valid = true;
            return true;
        }

        private static bool TryResolveLegSide(SetLinePositions setLine, out bool isRight)
        {
            isRight = false;
            if (setLine == null)
            {
                return false;
            }

            string name = setLine.gameObject.name;
            if (IsRightLegBoneName(name) || IsRightKneeBoneName(name))
            {
                isRight = true;
                return true;
            }

            if (IsLeftLegBoneName(name) || IsLeftKneeBoneName(name))
            {
                isRight = false;
                return true;
            }

            if (string.Equals(name, "legRenderer2", StringComparison.OrdinalIgnoreCase))
            {
                isRight = true;
                return true;
            }

            if (string.Equals(name, "legRenderer", StringComparison.OrdinalIgnoreCase))
            {
                isRight = false;
                return true;
            }

            Transform t = setLine.transform;
            while (t != null)
            {
                if (string.Equals(t.name, "legRenderer2", StringComparison.OrdinalIgnoreCase))
                {
                    isRight = true;
                    return true;
                }

                if (string.Equals(t.name, "legRenderer", StringComparison.OrdinalIgnoreCase))
                {
                    isRight = false;
                    return true;
                }

                t = t.parent;
            }

            return false;
        }

        private static bool IsLeftKneeBoneName(string name)
        {
            return string.Equals(name, "LeftKnee", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Knee_Left", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRightKneeBoneName(string name)
        {
            return string.Equals(name, "RightKnee", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Knee_Right", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsLeftLegBoneName(string name)
        {
            return string.Equals(name, "LeftLeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Leg_Left", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "L_Leg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Foot_Left", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "LeftFoot", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRightLegBoneName(string name)
        {
            return string.Equals(name, "RightLeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Leg_Right", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "R_Leg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Foot_Right", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "RightFoot", StringComparison.OrdinalIgnoreCase);
        }

        private static Transform FindLegNode(Controller controller, bool isRight)
        {
            Transform renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform);
            if (renderersRoot == null)
            {
                return null;
            }

            string nodeName = isRight ? "legRenderer2" : "legRenderer";
            return PlayerColorApplier.FindRendererTransform(renderersRoot, nodeName);
        }

        private static bool HasAnyFoot(ShoeInstance inst)
        {
            return inst != null
                && ((inst.Left != null && inst.Left.ShoeRoot != null)
                    || (inst.Right != null && inst.Right.ShoeRoot != null));
        }

        private static void DestroyFootMount(ShoeFootMount mount)
        {
            if (mount == null)
            {
                return;
            }

            DestroyObject(mount.ShoeRoot);
            DestroyObject(mount.Holder);
        }

        private static void DestroyLegacyLineOverlay(Transform bone, string lineObjectName)
        {
            if (bone == null)
            {
                return;
            }

            Transform legacy = bone.Find(lineObjectName);
            if (legacy != null)
            {
                UnityEngine.Object.Destroy(legacy.gameObject);
            }
        }

        private static void DestroyObject(GameObject go)
        {
            if (go != null)
            {
                UnityEngine.Object.Destroy(go);
            }
        }
    }
}
