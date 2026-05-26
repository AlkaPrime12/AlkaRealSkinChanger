using System;
using System.Collections.Generic;
using StickFightColorCustomizer.Models;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Objetos pegados al jugador: centro en pecho (mundo) y órbita circular en el plano de la cámara.
    /// </summary>
    public static class ObjectsAttachmentRenderer
    {
        private sealed class ObjectInstance
        {
            public string ObjectId;
            public float UserScale;
            public GameObject Root;
            public SpriteRenderer[] Parts;
            public LineRenderer SortLine;
            public Controller Controller;
        }

        private static readonly Dictionary<int, ObjectInstance> ByControllerId =
            new Dictionary<int, ObjectInstance>();

        private const int ObjectSortBehindOffset = -10;
        private const int ObjectSortFrontOffset = 5;

        public static void Apply(Controller controller, string objectId, float userScale)
        {
            objectId = ObjectsCatalog.Normalize(objectId);
            if (controller == null || objectId == "none")
            {
                Remove(controller);
                return;
            }

            ObjectsCatalogEntry def;
            if (!ObjectsCatalog.TryGet(objectId, out def) || def.PartCount < 1)
            {
                Remove(controller);
                return;
            }

            int id = controller.GetInstanceID();
            ObjectInstance existing;
            if (ByControllerId.TryGetValue(id, out existing)
                && existing != null
                && existing.Root != null
                && existing.ObjectId == objectId
                && Mathf.Abs(existing.UserScale - userScale) < 0.01f)
            {
                RefreshForController(controller);
                return;
            }

            Remove(controller);

            LineRenderer sortLine = FindSortLine(controller);
            Transform pivot = CosmeticFollowPivot.EnsurePivot(controller);
            GameObject root = new GameObject("SFCC_Objects");
            root.transform.SetParent(pivot != null ? pivot : controller.transform, false);

            try
            {
                root.tag = "DontChangeColor";
            }
            catch
            {
            }

            SpriteRenderer[] parts = new SpriteRenderer[def.PartCount];
            for (int i = 0; i < def.PartCount; i++)
            {
                GameObject partGo = new GameObject("SFCC_Obj_" + i);
                partGo.transform.SetParent(root.transform, false);
                SpriteRenderer sr = partGo.AddComponent<SpriteRenderer>();
                sr.sharedMaterial = HatMeshFactory.GetHatMaterial();
                sr.sprite = ObjectSpriteFactory.GetPartSprite(objectId, i);
                sr.color = Color.white;
                sr.enabled = true;
                parts[i] = sr;
            }

            ObjectInstance inst = new ObjectInstance
            {
                ObjectId = objectId,
                UserScale = userScale,
                Root = root,
                Parts = parts,
                SortLine = sortLine,
                Controller = controller
            };

            ByControllerId[id] = inst;
            SyncParts(inst);
        }

        public static void ApplyRemoteObjectOnly(Controller controller, string objectId, float userScale)
        {
            Apply(controller, objectId, userScale);
        }

        public static void Remove(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int id = controller.GetInstanceID();
            ObjectInstance inst;
            if (ByControllerId.TryGetValue(id, out inst) && inst != null)
            {
                DestroyObject(inst.Root);
            }

            ByControllerId.Remove(id);
        }

        public static void ClearControllerCache(Controller controller)
        {
            Remove(controller);
        }

        public static void ClearAll()
        {
            foreach (KeyValuePair<int, ObjectInstance> kv in ByControllerId)
            {
                if (kv.Value != null)
                {
                    DestroyObject(kv.Value.Root);
                }
            }

            ByControllerId.Clear();
        }

        public static bool HasObjectsForController(int controllerInstanceId)
        {
            ObjectInstance inst;
            return ByControllerId.TryGetValue(controllerInstanceId, out inst)
                && inst != null && inst.Root != null;
        }

        public static void RefreshForController(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int id = controller.GetInstanceID();
            ObjectInstance inst;
            if (!ByControllerId.TryGetValue(id, out inst) || inst == null || inst.Root == null)
            {
                return;
            }

            inst.Controller = controller;
            LineRenderer sortLine = FindSortLine(controller);
            if (sortLine != null)
            {
                inst.SortLine = sortLine;
            }

            SyncParts(inst);
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
            ObjectInstance inst;
            if (!ByControllerId.TryGetValue(controllerId, out inst) || inst == null)
            {
                return;
            }

            RefreshForController(controller);
        }

        private static void SyncParts(ObjectInstance inst)
        {
            if (inst == null || inst.Parts == null || inst.Root == null)
            {
                return;
            }

            Controller controller = inst.Controller;
            if (controller == null)
            {
                return;
            }

            ObjectsCatalogEntry def;
            if (!ObjectsCatalog.TryGet(inst.ObjectId, out def))
            {
                return;
            }

            LineRenderer spine = inst.SortLine != null ? inst.SortLine : FindSortLine(controller);
            inst.SortLine = spine;

            Vector3 chestWorld;
            float torsoHeight;
            if (!CosmeticFollowPivot.TryGetChestWorld(spine, controller, out chestWorld, out torsoHeight))
            {
                return;
            }

            // Do NOT override chestWorld.z from spine.transform.position.z — that transform
            // belongs to the Renderers/spineRenderer node which stays at the Controller spawn
            // (X=0, Z=0) while the actual character bones (and therefore the line vertices)
            // move horizontally with the physics rigidbody. The vertex-derived chestWorld
            // already carries the correct player Z; we only nudge it slightly for layering.
            chestWorld.z += def.BehindBody ? 0.03f : -0.02f;

            Transform pivot = CosmeticFollowPivot.EnsurePivot(controller);
            CosmeticFollowPivot.ApplyPivotWorldPosition(pivot, chestWorld);

            if (pivot != null && inst.Root.transform.parent != pivot)
            {
                inst.Root.transform.SetParent(pivot, false);
            }

            inst.Root.transform.localPosition = Vector3.zero;
            inst.Root.transform.localRotation = Quaternion.identity;
            inst.Root.transform.localScale = Vector3.one;

            float userScale = Mathf.Clamp(inst.UserScale, 0.5f, 1.5f);
            float radius = def.OrbitRadius * userScale * Mathf.Max(torsoHeight * 0.5f, 0.42f);
            float partSize = def.BaseScale * userScale * Mathf.Max(torsoHeight * 0.42f, 0.35f);
            float spin = -Time.time * def.OrbitSpeed;
            int sortLayer = spine != null ? spine.sortingLayerID : 0;
            int spineOrder = spine != null ? spine.sortingOrder : 0;
            // sortOrder is now computed per-part inside the loop based on orbit phase.

            Camera cam = Camera.main;
            Vector3 orbitRight;
            Vector3 orbitUp;
            CosmeticFollowPivot.GetCameraOrbitAxes(cam, out orbitRight, out orbitUp);

            Vector3 centerWorld = chestWorld;
            float orbitRadiusX = radius;
            float orbitRadiusY = radius * 1.08f;

            if (def.Layout == ObjectLayoutKind.OrbitHalo)
            {
                orbitRadiusX = def.OrbitRadius * userScale * Mathf.Max(torsoHeight * 0.32f, 0.28f);
                orbitRadiusY = orbitRadiusX;
                centerWorld += orbitUp * (torsoHeight * 0.2f);
            }

            for (int i = 0; i < inst.Parts.Length; i++)
            {
                SpriteRenderer sr = inst.Parts[i];
                if (sr == null)
                {
                    continue;
                }

                if (sr.transform.parent != inst.Root.transform)
                {
                    sr.transform.SetParent(inst.Root.transform, false);
                }

                sr.sprite = ObjectSpriteFactory.GetPartSprite(inst.ObjectId, i);
                sr.sortingLayerID = sortLayer;
                sr.color = Color.white;
                sr.enabled = true;

                float phase = spin + (i / (float)Mathf.Max(def.PartCount, 1)) * Mathf.PI * 2f;

                // Objects flagged BehindBody stay BEHIND the stickman at all times (do not flip
                // to a higher sort order when the orbit crosses cos>0 — that caused them to pop
                // forward through glow/body).  Use a strong negative offset to make sure they
                // render under glow, body lines, hat, etc.
                sr.sortingOrder = def.BehindBody
                    ? spineOrder + ObjectSortBehindOffset
                    : spineOrder + ObjectSortFrontOffset;

                Vector3 offsetWorld = CosmeticFollowPivot.GetOrbitOffsetWorld(
                    orbitRight,
                    orbitUp,
                    phase,
                    orbitRadiusX,
                    orbitRadiusY);

                // Do NOT override partWorld.z — the orbit offset already provides the correct Z
                // (horizontal) component from the Y-Z plane orbit.
                Vector3 partWorld = centerWorld + offsetWorld;

                Vector3 partLocal = inst.Root.transform.InverseTransformPoint(partWorld);
                sr.transform.localPosition = partLocal;
                sr.transform.localRotation = Quaternion.identity;
                if (cam != null)
                {
                    sr.transform.rotation = cam.transform.rotation;
                }

                Bounds b = sr.sprite != null ? sr.sprite.bounds : new Bounds(Vector3.zero, Vector3.one);
                float sw = Mathf.Max(b.size.x, 0.01f);
                float sh = Mathf.Max(b.size.y, 0.01f);
                float target = partSize / Mathf.Max(sw, sh);
                sr.transform.localScale = new Vector3(target, target, 1f);
            }
        }

        private static LineRenderer FindSortLine(Controller controller)
        {
            LineRenderer spine = FindSpineLine(controller);
            if (spine != null)
            {
                return spine;
            }

            Transform renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform);
            if (renderersRoot == null)
            {
                return null;
            }

            LineRenderer[] lines = renderersRoot.GetComponentsInChildren<LineRenderer>(true);
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

        private static LineRenderer FindSpineLine(Controller controller)
        {
            Transform renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform);
            Transform spineNode = renderersRoot != null
                ? PlayerColorApplier.FindRendererTransform(renderersRoot, "spineRenderer")
                : null;
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

        private static void DestroyObject(GameObject go)
        {
            if (go != null)
            {
                UnityEngine.Object.Destroy(go);
            }
        }
    }
}
