using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class HatAttachmentRenderer
    {
        private sealed class HatInstance
        {
            public GameObject Root;
            public string HatId;
        }

        private struct HeadAttachPoint
        {
            public Transform Parent;
            public int SortingLayerId;
            public int SortingOrder;
            public float HeadRadius;
            public Vector3 LocalPosition;
            public Vector3 LocalFaceCenter;
        }

        private static readonly Dictionary<int, HatInstance> ByControllerId = new Dictionary<int, HatInstance>();
        private static readonly Dictionary<int, int> LastRemoteHashByController = new Dictionary<int, int>();

        public static void Apply(Controller controller, string hatId)
        {
            hatId = HatCatalog.Normalize(hatId);
            if (controller == null || hatId == "none")
            {
                Remove(controller);
                return;
            }

            int controllerId = controller.GetInstanceID();
            HatInstance existing;
            if (ByControllerId.TryGetValue(controllerId, out existing)
                && existing != null
                && existing.Root != null
                && existing.HatId == hatId)
            {
                RefreshHatTransform(existing.Root, controller, existing.HatId);
                return;
            }

            Remove(controller);
            HeadAttachPoint attach;
            if (!TryResolveHeadAttach(controller, out attach))
            {
                return;
            }

            GameObject root = new GameObject("SFCC_Hat");
            root.transform.SetParent(attach.Parent, false);
            root.transform.localRotation = Quaternion.identity;

            SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
            renderer.sprite = GetSpriteForHat(hatId);
            renderer.sharedMaterial = HatMeshFactory.GetHatMaterial();
            renderer.color = Color.white;
            renderer.sortingLayerID = attach.SortingLayerId;
            renderer.sortingOrder = attach.SortingOrder;
            try
            {
                renderer.gameObject.tag = "DontChangeColor";
            }
            catch
            {
                // Tag puede no existir en algunas builds; IsModCosmeticObject cubre el caso.
            }

            ApplyTransform(root.transform, attach, hatId);
            PreserveHatTint(renderer);

            ByControllerId[controllerId] = new HatInstance
            {
                Root = root,
                HatId = hatId
            };
        }

        public static void ApplyRemoteHatOnly(Controller controller, string hatId)
        {
            if (controller == null)
            {
                return;
            }

            hatId = HatCatalog.Normalize(hatId);
            int controllerId = controller.GetInstanceID();
            int hash = hatId != null ? hatId.GetHashCode() : 0;
            int prevHash;
            if (LastRemoteHashByController.TryGetValue(controllerId, out prevHash) && prevHash == hash)
            {
                return;
            }

            LastRemoteHashByController[controllerId] = hash;

            if (hatId == "none")
            {
                Remove(controller);
                return;
            }

            Apply(controller, hatId);
        }

        public static void Remove(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            HatInstance inst;
            if (ByControllerId.TryGetValue(controllerId, out inst))
            {
                if (inst != null && inst.Root != null)
                {
                    Object.Destroy(inst.Root);
                }

                ByControllerId.Remove(controllerId);
            }

            LastRemoteHashByController.Remove(controllerId);
        }

        public static void ClearControllerCache(Controller controller)
        {
            Remove(controller);
        }

        /// <summary>
        /// Updates the hat's horizontal flip based on the controller's facing direction.
        /// Only meaningful for image hats whose source PNG faces a specific side.
        /// Called per-frame from SetLinePositionsPatch (cheap dictionary lookup).
        /// </summary>
        public static void RefreshFacing(Controller controller)
        {
            if (controller == null) return;
            int id = controller.GetInstanceID();
            HatInstance inst;
            if (!ByControllerId.TryGetValue(id, out inst) || inst == null || inst.Root == null)
                return;

            SpriteRenderer sr = inst.Root.GetComponent<SpriteRenderer>();
            if (sr == null) return;

            if (HatImageLoader.IsImageHat(inst.HatId))
            {
                HatImageLoader.ImageHatDef def = HatImageLoader.GetDef(inst.HatId);
                bool facingRight = PlayerFacingTracker.IsFacingRight(controller);
                // PNG faces left → flip when player faces right.
                // PNG faces right → flip when player faces left.
                bool flip = (def != null && def.FacesLeft) ? facingRight : !facingRight;
                if (sr.flipX != flip) sr.flipX = flip;
            }
        }

        public static void ClearAll()
        {
            foreach (KeyValuePair<int, HatInstance> kv in ByControllerId)
            {
                if (kv.Value != null && kv.Value.Root != null)
                {
                    Object.Destroy(kv.Value.Root);
                }
            }

            ByControllerId.Clear();
            LastRemoteHashByController.Clear();
        }

        private static void RefreshHatTransform(GameObject root, Controller controller, string hatId)
        {
            HeadAttachPoint attach;
            if (!TryResolveHeadAttach(controller, out attach))
            {
                return;
            }

            SpriteRenderer renderer = root.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerID = attach.SortingLayerId;
                renderer.sortingOrder = attach.SortingOrder;
                PreserveHatTint(renderer);
            }

            ApplyTransform(root.transform, attach, hatId);
        }

        private static void PreserveHatTint(SpriteRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            renderer.color = Color.white;
            if (renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.color = Color.white;
            }
        }

        // Punto medio entre escala grande (5.5) y chica (3.35)
        private const float HatScaleMultiplier = 4.45f;
        private const float HatScaleMin = 0.9f;
        private const float HatScaleMax = 5.8f;
        private const int HatSortOrderBoost = 28;
        // Bumped from 0.01 → 0.25 (per-head-radius) so all procedural hats (cap, beanie,
        // tophat-procedural-look-alikes, cowboy, mario_cap, link_cap, ash_cap, etc.) sit
        // visibly ON the head instead of hovering a few pixels above it. Helmets and image
        // hats keep their own per-id nudge logic further below.
        private const float HatNudgeDownRadiusFactor = 0.25f;

        private static void ApplyTransform(Transform hatTransform, HeadAttachPoint attach, string hatId)
        {
            float scale = Mathf.Clamp(attach.HeadRadius * HatScaleMultiplier, HatScaleMin, HatScaleMax);
            float widthScale = HatSpriteFactory.GetWidthScale(hatId);
            float heightScale = HatSpriteFactory.GetHeightScale(hatId);
            HatAttachKind kind = HatSpriteFactory.GetAttachKind(hatId);

            if (kind == HatAttachKind.Face)
            {
                scale *= hatId.StartsWith("visor_") ? 1.02f : 0.94f;
                heightScale *= hatId.StartsWith("visor_") ? 0.95f : 0.78f;
            }
            else if (kind == HatAttachKind.EarLeft || kind == HatAttachKind.EarRight)
            {
                scale *= 0.82f;
            }
            else if (kind == HatAttachKind.Forehead)
            {
                scale *= 0.78f;
                heightScale *= 0.8f;
            }

            Vector3 pos;
            switch (kind)
            {
                case HatAttachKind.Face:
                    pos = attach.LocalFaceCenter;
                    pos.y += attach.HeadRadius * 0.05f;
                    pos.x += attach.HeadRadius * HatSpriteFactory.GetAttachOffsetX(hatId);
                    break;
                case HatAttachKind.Forehead:
                    pos = attach.LocalFaceCenter;
                    pos.y += attach.HeadRadius * HatSpriteFactory.GetAttachYOffset(hatId);
                    break;
                case HatAttachKind.EarLeft:
                    pos = attach.LocalFaceCenter;
                    pos.x -= attach.HeadRadius * 0.82f;
                    pos.y += attach.HeadRadius * 0.08f;
                    break;
                case HatAttachKind.EarRight:
                    pos = attach.LocalFaceCenter;
                    pos.x += attach.HeadRadius * 0.82f;
                    pos.y += attach.HeadRadius * 0.08f;
                    hatTransform.localScale = new Vector3(-scale * widthScale, scale * heightScale, 1f);
                    pos.z = -0.08f;
                    hatTransform.localPosition = pos;
                    hatTransform.localRotation = Quaternion.identity;
                    return;
                default:
                    pos = attach.LocalPosition;
                    pos.y += attach.HeadRadius * HatSpriteFactory.GetAttachYOffset(hatId);
                    pos.x += attach.HeadRadius * HatSpriteFactory.GetAttachOffsetX(hatId);
                    break;
            }

            if (ShouldNudgeHatDownSlightly(hatId))
            {
                pos.y -= attach.HeadRadius * HatNudgeDownRadiusFactor;
            }

            if (hatId == "toad_cap")
            {
                pos.y -= attach.HeadRadius * 0.14f;
            }

            if (HatImageLoader.IsImageHat(hatId))
            {
                HatImageLoader.ImageHatDef imgDef = HatImageLoader.GetDef(hatId);
                if (imgDef != null)
                {
                    if (imgDef.IsHelmetStyle)
                    {
                        // Todos los cascos a la misma altura: cubren la cara del stickman.
                        pos.y -= attach.HeadRadius * 0.17f;
                    }
                    else
                    {
                        // Gorras HD: un poquito más abajo.
                        pos.y -= attach.HeadRadius * 0.05f;
                    }
                }
            }

            pos.z = -0.08f;
            hatTransform.localPosition = pos;
            hatTransform.localRotation = Quaternion.identity;
            hatTransform.localScale = new Vector3(scale * widthScale, scale * heightScale, 1f);
        }

        private static bool ShouldNudgeHatDownSlightly(string hatId)
        {
            if (string.IsNullOrEmpty(hatId))
            {
                return false;
            }

            return !hatId.StartsWith("halo_") && hatId != "tophat";
        }

        private static bool TryResolveHeadAttach(Controller controller, out HeadAttachPoint attach)
        {
            attach = default(HeadAttachPoint);

            HeadRenderer head = controller.GetComponentInChildren<HeadRenderer>(true);
            if (TryResolveHeadLineAttach(controller, out attach))
            {
                return true;
            }

            SpriteRenderer headSprite = head != null ? head.GetComponent<SpriteRenderer>() : null;
            if (headSprite != null && headSprite.sprite != null && headSprite.enabled)
            {
                Bounds b = headSprite.sprite.bounds;
                float spriteHalfH = b.extents.y;
                float spriteHalfW = b.extents.x;
                float radius = Mathf.Max(spriteHalfH, spriteHalfW);
                if (radius < 0.01f)
                {
                    radius = 0.5f;
                }

                attach.Parent = headSprite.transform;
                attach.SortingLayerId = headSprite.sortingLayerID;
                attach.SortingOrder = headSprite.sortingOrder + HatSortOrderBoost;
                attach.HeadRadius = radius;
                attach.LocalPosition = new Vector3(0f, spriteHalfH * 1.05f, -0.08f);
                attach.LocalFaceCenter = new Vector3(0f, spriteHalfH * 0.15f, -0.08f);
                return true;
            }

            if (head != null)
            {
                attach.Parent = head.transform;
                attach.SortingLayerId = 0;
                attach.SortingOrder = HatSortOrderBoost;
                attach.HeadRadius = 0.45f;
                attach.LocalPosition = new Vector3(0f, 0.55f, -0.08f);
                attach.LocalFaceCenter = new Vector3(0f, 0.08f, -0.08f);
                return true;
            }

            return false;
        }

        private static bool TryResolveHeadLineAttach(Controller controller, out HeadAttachPoint attach)
        {
            attach = default(HeadAttachPoint);

            Transform renderers = PlayerColorApplier.FindRenderersRoot(controller.transform);
            Transform headNode = renderers != null
                ? PlayerColorApplier.FindRendererTransform(renderers, "headRenderer")
                : null;

            LineRenderer headLine = null;
            if (headNode != null)
            {
                LineRenderer[] lines = headNode.GetComponentsInChildren<LineRenderer>(true);
                for (int i = 0; i < lines.Length; i++)
                {
                    LineRenderer line = lines[i];
                    if (line != null && !line.gameObject.name.StartsWith("SFCC_"))
                    {
                        headLine = line;
                        break;
                    }
                }
            }

            if (headLine == null)
            {
                return false;
            }

            float radius = Mathf.Max(headLine.widthMultiplier * 2.6f, 0.28f);
            Vector3 localTop = Vector3.up * (radius * 1.1f);
            Vector3 localFace = Vector3.zero;
            if (headLine.positionCount > 0)
            {
                int topIndex = headLine.positionCount - 1;
                int midIndex = headLine.positionCount / 2;
                Vector3 pTop = headLine.GetPosition(topIndex);
                Vector3 pMid = headLine.GetPosition(midIndex);
                if (headLine.useWorldSpace)
                {
                    pTop = headLine.transform.InverseTransformPoint(pTop);
                    pMid = headLine.transform.InverseTransformPoint(pMid);
                }

                localTop = pTop + Vector3.up * (radius * 0.42f);
                localFace = pMid;
            }

            attach.Parent = headLine.transform;
            attach.SortingLayerId = headLine.sortingLayerID;
            attach.SortingOrder = headLine.sortingOrder + HatSortOrderBoost;
            attach.HeadRadius = radius;
            attach.LocalPosition = localTop;
            attach.LocalFaceCenter = localFace;
            return true;
        }

        private static Sprite GetSpriteForHat(string hatId)
        {
            return HatSpriteFactory.GetSprite(hatId);
        }
    }
}
