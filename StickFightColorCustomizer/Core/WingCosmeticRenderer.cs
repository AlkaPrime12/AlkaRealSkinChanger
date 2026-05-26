using System;
using System.Collections.Generic;
using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Alas del minijuego: solo clon del visual existente del personaje (sin geometría inventada).
    /// </summary>
    public static class WingCosmeticRenderer
    {
        private sealed class WingInstance
        {
            public GameObject Root;
            public Transform SpineAnchor;
            public Transform AttachParent;
            public Transform AttachBone;
            public LineRenderer SortLine;
            public Vector3 LocalAnchorOffset;
            public bool VanillaSuppressed;
            public string StyleId;
            public float Scale;
            public int ColorHash;
        }

        private static readonly Dictionary<int, WingInstance> ByControllerId = new Dictionary<int, WingInstance>();
        private static readonly HashSet<int> LoggedMissingTemplate = new HashSet<int>();
        private static readonly HashSet<int> LoggedAttachFailure = new HashSet<int>();
        private static readonly HashSet<int> LoggedCloneFailure = new HashSet<int>();
        private static int _lastAnchorSyncFrame = -1;
        private static int _lastAnchorSyncControllerId = -1;

        private const float DefaultScale = 0.55f;
        private const float MinScale = 0.35f;
        private const float MaxScale = 0.9f;
        private const float WingVisualScaleBoost = 2.35f;
        private const float WingVisualScaleMin = 1.35f;
        public static void Apply(Controller controller, WingSettings settings, BodyColors colors)
        {
            if (controller == null || settings == null || !settings.Enabled)
            {
                Remove(controller);
                return;
            }

            string styleId = WingCatalog.Normalize(settings.StyleId);
            float scale = Mathf.Clamp(settings.Scale > 0.01f ? settings.Scale : DefaultScale, MinScale, MaxScale);
            Color wingColor = colors != null ? colors.Wings : Color.white;
            int colorHash = ColorUtil.ToHex(wingColor).GetHashCode();

            int controllerId = controller.GetInstanceID();
            WingInstance existing;
            if (ByControllerId.TryGetValue(controllerId, out existing)
                && existing != null
                && existing.Root != null
                && existing.StyleId == styleId
                && Mathf.Abs(existing.Scale - scale) < 0.01f
                && existing.ColorHash == colorHash)
            {
                if (!existing.VanillaSuppressed)
                {
                    SuppressVanillaWings(controller);
                    existing.VanillaSuppressed = true;
                }

                SetActiveRecursive(existing.Root.transform, true);
                EnableAllRenderers(existing.Root.transform);
                ApplyColorsToHierarchy(existing.Root.transform, wingColor);
                return;
            }

            Remove(controller);

            Transform attachBone;
            LineRenderer sortLine;
            Transform renderersRoot;
            if (!TryResolveWingAttach(controller, out attachBone, out sortLine, out renderersRoot))
            {
                int failId = controller.GetInstanceID();
                if (!LoggedAttachFailure.Contains(failId))
                {
                    LoggedAttachFailure.Add(failId);
                    ModLog.Warning("[SFCC] Alas: no se encontró hueso Hip/Torso para adjuntar.");
                }

                return;
            }

            LoggedAttachFailure.Remove(controller.GetInstanceID());

            Transform template = FindExistingWingVisual(controller, renderersRoot);
            if (template == null)
            {
                int cid = controller.GetInstanceID();
                if (!LoggedMissingTemplate.Contains(cid))
                {
                    LoggedMissingTemplate.Add(cid);
                    ModLog.Warning("[SFCC] Alas: este personaje no tiene alas del minijuego para clonar.");
                }

                return;
            }

            LoggedMissingTemplate.Remove(controller.GetInstanceID());

            Transform spineAnchor = PlayerColorApplier.FindRendererTransform(renderersRoot, "spineRenderer");
            LineRenderer torsoLine = FindTorsoLine(spineAnchor);
            if (torsoLine != null)
            {
                sortLine = torsoLine;
                attachBone = torsoLine.transform;
            }

            Transform attachParent = sortLine != null ? sortLine.transform : (spineAnchor != null ? spineAnchor : attachBone);
            Vector3 anchorLocal = ResolveWingAnchorLocal(spineAnchor, sortLine, attachParent);
            GameObject root = new GameObject("SFCC_WingCosmetic");
            root.transform.SetParent(attachParent, false);
            root.transform.localPosition = anchorLocal;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            if (!TryCloneExistingWings(template, root.transform, wingColor, scale))
            {
                UnityEngine.Object.Destroy(root);
                int cid = controller.GetInstanceID();
                if (!LoggedCloneFailure.Contains(cid))
                {
                    LoggedCloneFailure.Add(cid);
                    ModLog.Warning("[SFCC] Alas: falló el clon del visual existente.");
                }

                return;
            }

            CopyLineStyleFromSpine(root.transform, sortLine);
            SuppressVanillaWings(controller);

            ByControllerId[controllerId] = new WingInstance
            {
                Root = root,
                SpineAnchor = spineAnchor,
                AttachParent = attachParent,
                AttachBone = attachBone,
                SortLine = sortLine,
                LocalAnchorOffset = anchorLocal,
                VanillaSuppressed = true,
                StyleId = styleId,
                Scale = scale,
                ColorHash = colorHash
            };
        }

        public static void Remove(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            WingInstance inst;
            if (ByControllerId.TryGetValue(controllerId, out inst) && inst != null && inst.Root != null)
            {
                UnityEngine.Object.Destroy(inst.Root);
            }

            ByControllerId.Remove(controllerId);
            LoggedMissingTemplate.Remove(controllerId);
            LoggedAttachFailure.Remove(controllerId);
            LoggedCloneFailure.Remove(controllerId);
        }

        public static void ClearControllerCache(Controller controller)
        {
            Remove(controller);
        }

        public static void ClearAll()
        {
            foreach (KeyValuePair<int, WingInstance> kv in ByControllerId)
            {
                if (kv.Value != null && kv.Value.Root != null)
                {
                    UnityEngine.Object.Destroy(kv.Value.Root);
                }
            }

            ByControllerId.Clear();
            LoggedMissingTemplate.Clear();
            LoggedAttachFailure.Clear();
            LoggedCloneFailure.Clear();
        }

        private static bool TryResolveWingAttach(
            Controller controller,
            out Transform attachBone,
            out LineRenderer sortLine,
            out Transform renderersRoot)
        {
            attachBone = null;
            sortLine = null;
            renderersRoot = null;

            if (controller == null)
            {
                return false;
            }

            renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform) ?? controller.transform;

            Transform spineNode = PlayerColorApplier.FindRendererTransform(renderersRoot, "spineRenderer");
            sortLine = FindTorsoLine(spineNode);
            if (sortLine != null)
            {
                attachBone = sortLine.transform;
                return true;
            }

            string[] boneNames = { "Torso", "Hip" };
            for (int i = 0; i < boneNames.Length; i++)
            {
                Transform bone = BoneIndexCache.GetBone(controller, boneNames[i]);
                if (bone != null)
                {
                    attachBone = bone;
                    sortLine = FindSortLineNearBone(controller, renderersRoot, boneNames[i]);
                    return true;
                }
            }

            if (spineNode != null)
            {
                attachBone = spineNode;
                return true;
            }

            return false;
        }

        private static LineRenderer FindSortLineNearBone(
            Controller controller, Transform renderersRoot, string boneName)
        {
            LineRenderer[] lines = controller.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line != null
                    && !line.gameObject.name.StartsWith("SFCC_")
                    && string.Equals(line.gameObject.name, boneName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return line;
                }
            }

            return FindSpineBoneLine(
                PlayerColorApplier.FindRendererTransform(renderersRoot, "spineRenderer"));
        }

        private static LineRenderer FindTorsoLine(Transform spineNode)
        {
            LineRenderer torso = FindNamedSpineLine(spineNode, "Torso");
            if (torso != null && StickLineUtil.GetVertexCount(torso) > 0)
            {
                return torso;
            }

            LineRenderer hip = FindNamedSpineLine(spineNode, "Hip");
            if (hip != null && StickLineUtil.GetVertexCount(hip) > 0)
            {
                return hip;
            }

            return FindSpineBoneLine(spineNode);
        }

        private static Vector3 ResolveWingAnchorLocal(Transform spineAnchor, LineRenderer sortLine, Transform attachParent)
        {
            if (attachParent == null)
            {
                return new Vector3(0f, 0f, -0.02f);
            }

            LineRenderer torsoLine = sortLine;
            if (spineAnchor != null)
            {
                LineRenderer namedTorso = FindNamedSpineLine(spineAnchor, "Torso");
                if (namedTorso != null && StickLineUtil.GetVertexCount(namedTorso) > 0)
                {
                    torsoLine = namedTorso;
                }
            }

            if (torsoLine != null && StickLineUtil.GetVertexCount(torsoLine) > 0)
            {
                if (attachParent == torsoLine.transform)
                {
                    Vector3 local = GetLineCentroidLocal(torsoLine);
                    local.z = -0.02f;
                    return local;
                }

                Vector3 world = torsoLine.transform.TransformPoint(GetLineCentroidLocal(torsoLine));
                Vector3 localOnParent = attachParent.InverseTransformPoint(world);
                localOnParent.z = -0.02f;
                return localOnParent;
            }

            if (sortLine != null && attachParent == sortLine.transform && StickLineUtil.GetVertexCount(sortLine) > 0)
            {
                Vector3 local = GetLineCentroidLocal(sortLine);
                local.z = -0.02f;
                return local;
            }

            return new Vector3(0f, 0f, -0.02f);
        }

        private static void AccumulateLineCentroidWorld(LineRenderer line, ref Vector3 worldSum, ref int count)
        {
            if (line == null || StickLineUtil.GetVertexCount(line) <= 0)
            {
                return;
            }

            worldSum += line.transform.TransformPoint(GetLineCentroidLocal(line));
            count++;
        }

        private static LineRenderer FindNamedSpineLine(Transform spineNode, string boneName)
        {
            if (spineNode == null || string.IsNullOrEmpty(boneName))
            {
                return null;
            }

            LineRenderer[] lines = spineNode.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line != null
                    && line.transform != spineNode
                    && !line.gameObject.name.StartsWith("SFCC_")
                    && string.Equals(line.gameObject.name, boneName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return line;
                }
            }

            return null;
        }

        private static Vector3 GetLineCentroidLocal(LineRenderer line)
        {
            int count = StickLineUtil.GetVertexCount(line);
            if (count <= 0)
            {
                return Vector3.zero;
            }

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                sum += StickLineUtil.GetVertexLocal(line, i);
            }

            return sum / count;
        }

        /// <summary>Hijos de spineRenderer (Torso/Hip), no el LineRenderer contenedor vacío.</summary>
        private static LineRenderer FindSpineBoneLine(Transform spineNode)
        {
            if (spineNode == null)
            {
                return null;
            }

            string[] prefer = { "Torso", "Hip" };
            LineRenderer[] lines = spineNode.GetComponentsInChildren<LineRenderer>(true);
            for (int p = 0; p < prefer.Length; p++)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    LineRenderer line = lines[i];
                    if (line != null
                        && line.transform != spineNode
                        && !line.gameObject.name.StartsWith("SFCC_")
                        && string.Equals(line.gameObject.name, prefer[p], System.StringComparison.OrdinalIgnoreCase))
                    {
                        return line;
                    }
                }
            }

            LineRenderer best = null;
            int bestVerts = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null
                    || line.transform == spineNode
                    || line.gameObject.name.StartsWith("SFCC_")
                    || string.Equals(line.gameObject.name, "spineRenderer", System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int vc = StickLineUtil.GetVertexCount(line);
                if (vc > bestVerts)
                {
                    bestVerts = vc;
                    best = line;
                }
            }

            return best;
        }

        public static bool HasWingsForController(int controllerInstanceId)
        {
            WingInstance inst;
            return ByControllerId.TryGetValue(controllerInstanceId, out inst) && inst != null && inst.Root != null;
        }

        public static bool WasSyncedThisFrame(int controllerInstanceId)
        {
            return Time.frameCount == _lastAnchorSyncFrame
                && _lastAnchorSyncControllerId == controllerInstanceId;
        }

        public static void SyncForSetLine(SetLinePositions setLine)
        {
            if (!ShouldSyncWingForSetLine(setLine))
            {
                return;
            }

            Controller controller = setLine.transform.root.GetComponent<Controller>();
            if (controller == null)
            {
                return;
            }

            int id = controller.GetInstanceID();
            WingInstance inst;
            if (!ByControllerId.TryGetValue(id, out inst) || inst == null || inst.Root == null)
            {
                return;
            }

            RefreshWingAnchor(inst);
            MarkAnchorSynced(id);
        }

        private static bool ShouldSyncWingForSetLine(SetLinePositions setLine)
        {
            if (setLine == null)
            {
                return false;
            }

            string name = setLine.gameObject.name;
            return string.Equals(name, "spineRenderer", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Torso", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Hip", StringComparison.OrdinalIgnoreCase);
        }

        private static void RefreshWingAnchor(WingInstance inst)
        {
            if (inst == null || inst.Root == null)
            {
                return;
            }

            LineRenderer torsoLine = FindTorsoLine(inst.SpineAnchor);
            if (torsoLine != null)
            {
                inst.SortLine = torsoLine;
                inst.AttachParent = torsoLine.transform;
                inst.AttachBone = torsoLine.transform;
            }

            Transform parent = inst.AttachParent != null ? inst.AttachParent : inst.AttachBone;
            if (parent == null)
            {
                return;
            }

            if (inst.Root.transform.parent != parent)
            {
                inst.Root.transform.SetParent(parent, false);
            }

            // The previous code set localPosition relative to the line transform — but the
            // line transform doesn't follow the physics bones, only the LINE VERTICES do.
            // Compute the chest world position from the spine vertices and set the wing root's
            // world position directly, so the wings always appear stuck to the actual body.
            Vector3 chestWorld;
            if (TryComputeChestWorld(inst.SpineAnchor, inst.SortLine, out chestWorld))
            {
                inst.Root.transform.position = chestWorld;
                inst.Root.transform.rotation = Quaternion.identity;
                inst.LocalAnchorOffset = inst.Root.transform.localPosition;
            }
            else
            {
                Vector3 anchor = ResolveWingAnchorLocal(inst.SpineAnchor, inst.SortLine, parent);
                inst.LocalAnchorOffset = anchor;
                inst.Root.transform.localPosition = anchor;
                inst.Root.transform.localRotation = Quaternion.identity;
            }

            CopyLineStyleFromSpine(inst.Root.transform, inst.SortLine);
        }

        /// <summary>Average world position of spine/torso/hip line vertices = chest centre.</summary>
        private static bool TryComputeChestWorld(Transform spineAnchor, LineRenderer sortLine, out Vector3 world)
        {
            world = Vector3.zero;
            Vector3 sum = Vector3.zero;
            int count = 0;

            if (sortLine != null)
            {
                int vc = StickLineUtil.GetVertexCount(sortLine);
                for (int i = 0; i < vc; i++) sum += StickLineUtil.GetVertexWorld(sortLine, i);
                count += vc;
            }

            if (spineAnchor != null)
            {
                LineRenderer[] lines = spineAnchor.GetComponentsInChildren<LineRenderer>(true);
                for (int i = 0; i < lines.Length; i++)
                {
                    LineRenderer l = lines[i];
                    if (l == null || l == sortLine) continue;
                    if (l.gameObject.name.StartsWith("SFCC_")) continue;
                    int vc = StickLineUtil.GetVertexCount(l);
                    for (int v = 0; v < vc; v++) sum += StickLineUtil.GetVertexWorld(l, v);
                    count += vc;
                    if (count >= 4) break;
                }
            }

            if (count == 0) return false;
            world = sum / count;
            return true;
        }

        private static void MarkAnchorSynced(int controllerInstanceId)
        {
            _lastAnchorSyncFrame = Time.frameCount;
            _lastAnchorSyncControllerId = controllerInstanceId;
        }

        public static void TickSway(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            int id = controller.GetInstanceID();
            if (WasSyncedThisFrame(id))
            {
                return;
            }

            WingInstance inst;
            if (!ByControllerId.TryGetValue(id, out inst) || inst == null || inst.Root == null)
            {
                return;
            }

            RefreshWingAnchor(inst);
        }

        public static void SuppressVanillaWings(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            Transform renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform);
            if (renderersRoot == null)
            {
                return;
            }

            Transform wingsFolder = PlayerColorApplier.FindRendererTransform(renderersRoot, "Wings");
            if (wingsFolder == null)
            {
                return;
            }

            for (int i = 0; i < wingsFolder.childCount; i++)
            {
                Transform child = wingsFolder.GetChild(i);
                if (child == null || child.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                if (IsMinigameWingBranch(child))
                {
                    SetActiveRecursive(child, false);
                }
            }
        }

        private static bool IsMinigameWingBranch(Transform node)
        {
            if (node == null)
            {
                return false;
            }

            if (node.name.Equals("Wings", System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            MonoBehaviour[] behaviours = node.GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] == null)
                {
                    continue;
                }

                string typeName = behaviours[i].GetType().Name;
                if (typeName == "Wobble" || typeName == "WobbleRotation" || typeName == "SetValueAnimationByValocity")
                {
                    return true;
                }
            }

            return false;
        }

        private static Transform FindExistingWingVisual(Controller controller, Transform renderersRoot)
        {
            Transform fromScripts = FindWingRootByGameplayScripts(controller.transform);
            if (fromScripts != null)
            {
                return fromScripts;
            }

            if (renderersRoot == null)
            {
                return null;
            }

            Transform wingsFolder = PlayerColorApplier.FindRendererTransform(renderersRoot, "Wings");
            if (wingsFolder == null)
            {
                return null;
            }

            Transform visual = FindWingVisualUnderFolder(wingsFolder);
            if (visual != null)
            {
                return visual;
            }

            for (int i = 0; i < wingsFolder.childCount; i++)
            {
                Transform child = wingsFolder.GetChild(i);
                if (child == null || child.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                if (HasVisualComponent(child))
                {
                    return child;
                }
            }

            return wingsFolder.childCount > 0 ? wingsFolder.GetChild(0) : null;
        }

        private static bool TryCloneExistingWings(Transform template, Transform parent, Color wingColor, float scale)
        {
            if (template == null)
            {
                return false;
            }

            try
            {
                bool wasActive = template.gameObject.activeInHierarchy;
                if (!wasActive)
                {
                    template.gameObject.SetActive(true);
                }

                GameObject clone = UnityEngine.Object.Instantiate(template.gameObject);
                if (!wasActive)
                {
                    template.gameObject.SetActive(false);
                }

                if (clone == null)
                {
                    return false;
                }

                clone.name = "SFCC_WingVisual";
                clone.transform.SetParent(parent, false);
                clone.transform.localPosition = Vector3.zero;
                clone.transform.localRotation = Quaternion.identity;
                float visualScale = Mathf.Max(scale * WingVisualScaleBoost, WingVisualScaleMin);
                clone.transform.localScale = Vector3.one * visualScale;

                StripCollidersOnly(clone);
                SetActiveRecursive(clone.transform, true);
                EnableAllRenderers(clone.transform);
                CopyLinePositionsFromTemplate(template, clone.transform);
                SyncLineMaterialsFromTemplate(template, clone.transform);
                ApplyColorsToHierarchy(clone.transform, wingColor);
                TagDontChangeColor(clone);
                return HasAnyVisibleRenderer(clone.transform);
            }
            catch
            {
                return false;
            }
        }

        private static void CopyLineStyleFromSpine(Transform wingRoot, LineRenderer spineLine)
        {
            if (spineLine == null || wingRoot == null)
            {
                return;
            }

            LineRenderer[] lines = wingRoot.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null)
                {
                    continue;
                }

                line.sortingLayerID = spineLine.sortingLayerID;
                line.sortingOrder = spineLine.sortingOrder + 2;
                line.enabled = true;
            }
        }

        private static void CopyLinePositionsFromTemplate(Transform template, Transform cloneRoot)
        {
            if (template == null || cloneRoot == null)
            {
                return;
            }

            LineRenderer[] srcLines = template.GetComponentsInChildren<LineRenderer>(true);
            LineRenderer[] dstLines = cloneRoot.GetComponentsInChildren<LineRenderer>(true);
            int count = Mathf.Min(srcLines.Length, dstLines.Length);
            for (int i = 0; i < count; i++)
            {
                LineRenderer src = srcLines[i];
                LineRenderer dst = dstLines[i];
                if (src == null || dst == null || StickLineUtil.GetVertexCount(src) <= 0)
                {
                    continue;
                }

                int verts = StickLineUtil.GetVertexCount(src);
                StickLineUtil.SetVertexCount(dst, verts);
                for (int v = 0; v < verts; v++)
                {
                    dst.SetPosition(v, src.GetPosition(v));
                }
            }
        }

        private static void SyncLineMaterialsFromTemplate(Transform template, Transform cloneRoot)
        {
            if (template == null || cloneRoot == null)
            {
                return;
            }

            LineRenderer[] templateLines = template.GetComponentsInChildren<LineRenderer>(true);
            LineRenderer[] cloneLines = cloneRoot.GetComponentsInChildren<LineRenderer>(true);
            int count = Mathf.Min(templateLines.Length, cloneLines.Length);
            for (int i = 0; i < count; i++)
            {
                LineRenderer src = templateLines[i];
                LineRenderer dst = cloneLines[i];
                if (src == null || dst == null)
                {
                    continue;
                }

                if (src.material != null)
                {
                    dst.material = src.material;
                }

                if (src.sharedMaterial != null)
                {
                    dst.sharedMaterial = src.sharedMaterial;
                }

                dst.widthMultiplier = src.widthMultiplier;
                dst.useWorldSpace = src.useWorldSpace;
            }
        }

        private static Transform FindWingRootByGameplayScripts(Transform root)
        {
            MonoBehaviour[] behaviours = root.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour mb = behaviours[i];
                if (mb == null)
                {
                    continue;
                }

                string typeName = mb.GetType().Name;
                if (typeName == "Wobble" || typeName == "WobbleRotation" || typeName == "SetValueAnimationByValocity")
                {
                    Transform t = mb.transform;
                    if (t.name.StartsWith("SFCC_"))
                    {
                        continue;
                    }

                    return GetVisualRootForWingScript(t);
                }
            }

            return null;
        }

        private static Transform GetVisualRootForWingScript(Transform scriptTransform)
        {
            Transform named = FindChildByName(scriptTransform, "2 (1)");
            if (named != null)
            {
                return named;
            }

            if (HasVisualComponent(scriptTransform))
            {
                return scriptTransform;
            }

            Transform current = scriptTransform;
            while (current.parent != null)
            {
                if (current.parent.name.Equals("Wings", System.StringComparison.OrdinalIgnoreCase)
                    || current.parent.parent != null && current.parent.parent.name.Equals("Wings", System.StringComparison.OrdinalIgnoreCase))
                {
                    return current;
                }

                current = current.parent;
            }

            return scriptTransform;
        }

        private static Transform FindWingVisualUnderFolder(Transform wingsFolder)
        {
            Transform nested = FindChildByName(wingsFolder, "Wings");
            Transform searchRoot = nested != null ? nested : wingsFolder;

            Transform named = FindChildByName(searchRoot, "2 (1)");
            if (named != null && HasVisualComponent(named))
            {
                return named;
            }

            Transform best = null;
            int bestVerts = 0;
            int bestDepth = int.MaxValue;
            LineRenderer[] lines = searchRoot.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null || line.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                int vc = StickLineUtil.GetVertexCount(line);
                if (vc <= 0)
                {
                    continue;
                }

                Transform t = line.transform;
                int depth = DepthFrom(searchRoot, t);
                if (vc > bestVerts || (vc == bestVerts && depth >= 0 && depth < bestDepth))
                {
                    bestVerts = vc;
                    bestDepth = depth;
                    best = GetVisualRoot(searchRoot, t);
                }
            }

            if (best != null)
            {
                return best;
            }

            MeshRenderer[] meshes = searchRoot.GetComponentsInChildren<MeshRenderer>(true);
            for (int i = 0; i < meshes.Length; i++)
            {
                MeshRenderer mesh = meshes[i];
                if (mesh == null || mesh.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                Transform t = mesh.transform;
                int depth = DepthFrom(searchRoot, t);
                if (depth >= 0 && depth < bestDepth)
                {
                    bestDepth = depth;
                    best = GetVisualRoot(searchRoot, t);
                }
            }

            return best;
        }

        private static bool HasAnyVisibleRenderer(Transform root)
        {
            return root.GetComponentsInChildren<LineRenderer>(true).Length > 0
                || root.GetComponentsInChildren<MeshRenderer>(true).Length > 0;
        }

        private static void EnableAllRenderers(Transform root)
        {
            LineRenderer[] lines = root.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != null)
                {
                    lines[i].enabled = true;
                }
            }

            MeshRenderer[] meshes = root.GetComponentsInChildren<MeshRenderer>(true);
            for (int i = 0; i < meshes.Length; i++)
            {
                if (meshes[i] != null)
                {
                    meshes[i].enabled = true;
                }
            }
        }

        private static Transform GetVisualRoot(Transform searchRoot, Transform node)
        {
            Transform current = node;
            Transform candidate = node;
            while (current != null && current != searchRoot)
            {
                if (current.parent == searchRoot)
                {
                    candidate = current;
                    break;
                }

                current = current.parent;
            }

            return candidate;
        }

        private static int DepthFrom(Transform root, Transform node)
        {
            int depth = 0;
            Transform current = node;
            while (current != null && current != root)
            {
                depth++;
                current = current.parent;
            }

            return current == root ? depth : -1;
        }

        private static bool HasVisualComponent(Transform t)
        {
            return t.GetComponentInChildren<LineRenderer>(true) != null
                || t.GetComponentInChildren<MeshRenderer>(true) != null
                || t.GetComponentInChildren<SpriteRenderer>(true) != null;
        }

        /// <summary>Solo quita colisiones; joints/Rigidbody/Wobble deben quedar para que las líneas se dibujen.</summary>
        private static void StripCollidersOnly(GameObject root)
        {
            MonoBehaviour[] behaviours = root.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour mb = behaviours[i];
                if (mb != null && mb.GetType().Name == "HealthHandler")
                {
                    UnityEngine.Object.Destroy(mb);
                }
            }

            Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                {
                    UnityEngine.Object.Destroy(colliders[i]);
                }
            }
        }

        private static void ApplyColorsToHierarchy(Transform root, Color color)
        {
            LineRenderer[] lines = root.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null)
                {
                    continue;
                }

                line.startColor = color;
                line.endColor = color;
                if (line.material != null)
                {
                    MaterialColorHelper.ApplyToMaterial(line.material, color);
                }
            }

            MeshRenderer[] meshes = root.GetComponentsInChildren<MeshRenderer>(true);
            for (int i = 0; i < meshes.Length; i++)
            {
                MeshRenderer mesh = meshes[i];
                if (mesh == null)
                {
                    continue;
                }

                if (mesh.material != null)
                {
                    MaterialColorHelper.ApplyToMaterial(mesh.material, color);
                }
            }

            SpriteRenderer[] sprites = root.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < sprites.Length; i++)
            {
                SpriteRenderer sprite = sprites[i];
                if (sprite != null)
                {
                    sprite.color = color;
                }
            }
        }

        private static void TagDontChangeColor(GameObject go)
        {
            try
            {
                go.tag = "DontChangeColor";
            }
            catch
            {
            }

            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
            {
                TagDontChangeColor(t.GetChild(i).gameObject);
            }
        }

        private static void SetActiveRecursive(Transform node, bool active)
        {
            if (node == null || node.gameObject.name.StartsWith("SFCC_"))
            {
                return;
            }

            node.gameObject.SetActive(active);
            for (int i = 0; i < node.childCount; i++)
            {
                SetActiveRecursive(node.GetChild(i), active);
            }
        }

        private static Transform FindChildByName(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name.Equals(childName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }

                Transform nested = FindChildByName(child, childName);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }
    }
}
