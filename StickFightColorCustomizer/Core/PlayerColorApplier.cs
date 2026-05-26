using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using StickFightColorCustomizer.Models;
using StickFightColorCustomizer.Network;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class PlayerColorApplier
    {
        private static readonly Dictionary<string, StickColorPart> RendererMap = new Dictionary<string, StickColorPart>
        {
            { "headRenderer", StickColorPart.Head },
            { "spineRenderer", StickColorPart.Spine },
            { "legRenderer", StickColorPart.LegLeft },
            { "legRenderer2", StickColorPart.LegRight },
            { "handRenderer", StickColorPart.HandLeft },
            { "handRenderer2", StickColorPart.HandRight },
            { "Wings", StickColorPart.Wings }
        };

        private static readonly FieldInfo SetLineStartMatField = AccessTools.Field(typeof(SetLinePositions), "startMat");
        private static readonly FieldInfo SetLineLineField = AccessTools.Field(typeof(SetLinePositions), "line");
        private static int _lastApplyControllerId = -1;
        private static bool _legacyGlowCleaned;
        private static readonly Dictionary<int, int> LastRemoteApplyHashByController =
            new Dictionary<int, int>();
        private static Crown _cachedCrown;
        private static float _lastCrownLookupTime;

        public static void Apply(Controller controller, ColorConfig config)
        {
            if (controller == null || config == null)
            {
                return;
            }

            if (!LocalPlayerResolver.IsLocalController(controller))
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            if (controllerId != _lastApplyControllerId)
            {
                HalfColorCache.Clear();
                LineMaintainThrottle.Clear();
                BoneIndexCache.ClearController(controller);
                HatAttachmentRenderer.ClearControllerCache(controller);
                WingCosmeticRenderer.ClearControllerCache(controller);
                ShoeAttachmentRenderer.ClearControllerCache(controller);
                TopsAttachmentRenderer.ClearControllerCache(controller);
                ObjectsAttachmentRenderer.ClearControllerCache(controller);
                _lastApplyControllerId = controllerId;
            }

            if (config.UseUniformSkin && ModFeatureGate.IsBodyActive(config))
            {
                VanillaSkinBridge.ApplyPlayerSlotMaterial(controller, config.Colors, useUniformSkin: true);
                if (ModFeatureGate.IsHatActive(config))
                {
                    ApplyHatForConfig(controller, config);
                }

                ApplyWingForConfig(controller, config);
                ApplyShoeForConfig(controller, config);
                ApplyTopsForConfig(controller, config);
                ApplyObjectForConfig(controller, config);
                return;
            }

            if (ModFeatureGate.IsBodyActive(config))
            {
                ApplyBodyColors(controller, config.Colors);
            }

            if (ModFeatureGate.IsGlowActive(config))
            {
                ApplyGlowAura(controller, config.Glow, config.Colors);
            }
            else
            {
                GlowAuraRenderer.RemoveAll(controller);
            }

            if (ModFeatureGate.IsHatActive(config))
            {
                ApplyHatForConfig(controller, config);
            }
            else
            {
                HatAttachmentRenderer.Remove(controller);
            }

            ApplyWingForConfig(controller, config);
            ApplyShoeForConfig(controller, config);
            ApplyTopsForConfig(controller, config);
            ApplyObjectForConfig(controller, config);
        }

        private static void ApplyWingForConfig(Controller controller, ColorConfig config)
        {
            if (!WingFeatureFlags.Released)
            {
                WingCosmeticRenderer.Remove(controller);
                return;
            }

            if (ModFeatureGate.IsWingActive(config))
            {
                WingCosmeticRenderer.Apply(controller, config.Wing, config.Colors);
            }
            else
            {
                WingCosmeticRenderer.Remove(controller);
            }
        }

        private static void ApplyHatForConfig(Controller controller, ColorConfig config)
        {
            if (ModFeatureGate.IsHatActive(config))
            {
                HatAttachmentRenderer.Apply(controller, config.Hat.HatId);
            }
            else
            {
                HatAttachmentRenderer.Remove(controller);
            }
        }

        private static void ApplyShoeForConfig(Controller controller, ColorConfig config)
        {
            if (!ShoeFeatureFlags.Released)
            {
                ShoeAttachmentRenderer.Remove(controller);
                return;
            }

            if (ModFeatureGate.IsShoeActive(config))
            {
                ShoeAttachmentRenderer.Apply(controller, config.Shoe.ShoeId);
            }
            else
            {
                ShoeAttachmentRenderer.Remove(controller);
            }
        }

        private static void ApplyTopsForConfig(Controller controller, ColorConfig config)
        {
            if (!TopsFeatureFlags.Released)
            {
                TopsAttachmentRenderer.Remove(controller);
                return;
            }

            if (ModFeatureGate.IsTopsActive(config))
            {
                TopsAttachmentRenderer.Apply(controller, config.Tops.TopsId);
            }
            else
            {
                TopsAttachmentRenderer.Remove(controller);
            }
        }

        /// <summary>
        /// Remoto con mod: solo colores de cuerpo, sin glow ni corona (liviano en MP).
        /// </summary>
        public static void ClearRemoteApplyCache()
        {
            LastRemoteApplyHashByController.Clear();
        }

        /// <summary>
        /// Quita cosmética SFCC de un peer vanilla (restaura slot del juego).
        /// </summary>
        public static void StripRemoteCosmetics(Controller controller)
        {
            if (controller == null || LocalPlayerResolver.IsLocalController(controller))
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            LastRemoteApplyHashByController.Remove(controllerId);
            HatAttachmentRenderer.Remove(controller);
            ShoeAttachmentRenderer.Remove(controller);
            TopsAttachmentRenderer.Remove(controller);
            ObjectsAttachmentRenderer.Remove(controller);
            GlowAuraRenderer.RemoveAll(controller);
            WingCosmeticRenderer.Remove(controller);
            ResetLinesToVanillaSlot(controller);
        }

        private static void ResetLinesToVanillaSlot(Controller controller)
        {
            if (controller == null || ControllerHandler.Instance == null
                || ControllerHandler.Instance.colors == null)
            {
                return;
            }

            int slot = controller.playerID;
            if (slot < 0 || slot >= ControllerHandler.Instance.colors.Length)
            {
                return;
            }

            Material slotMat = ControllerHandler.Instance.colors[slot];
            Color slotColor = slotMat != null ? slotMat.color : Color.white;

            LineRenderer[] lines = controller.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null || line.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                if (slotMat != null)
                {
                    line.sharedMaterial = slotMat;
                }

                line.startColor = slotColor;
                line.endColor = slotColor;
            }

            SpriteRenderer[] sprites = controller.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < sprites.Length; i++)
            {
                SpriteRenderer sprite = sprites[i];
                if (sprite == null || sprite.transform.tag == "DontChangeColor")
                {
                    continue;
                }

                sprite.color = slotColor;
            }
        }

        public static void ApplyRemoteCosmeticOnly(Controller controller, BodyColors colors)
        {
            if (controller == null || colors == null)
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            int colorHash = ColorSyncCodec.Encode(colors).GetHashCode();
            int previousHash;
            if (LastRemoteApplyHashByController.TryGetValue(controllerId, out previousHash)
                && previousHash == colorHash)
            {
                return;
            }

            ApplyBodyColorsRemoteLight(controller, colors);
            LastRemoteApplyHashByController[controllerId] = colorHash;
        }

        public static void ApplyRemoteHatOnly(Controller controller, string hatId)
        {
            HatAttachmentRenderer.ApplyRemoteHatOnly(controller, hatId);
        }

        public static void ApplyRemoteShoeOnly(Controller controller, string shoeId)
        {
            ShoeAttachmentRenderer.ApplyRemoteShoeOnly(controller, shoeId);
        }

        private static void ApplyObjectForConfig(Controller controller, ColorConfig config)
        {
            if (!ObjectFeatureFlags.Released)
            {
                ObjectsAttachmentRenderer.Remove(controller);
                return;
            }

            if (ModFeatureGate.IsObjectsActive(config))
            {
                ObjectsAttachmentRenderer.Apply(
                    controller,
                    config.Object.ObjectId,
                    config.Object.Scale);
            }
            else
            {
                ObjectsAttachmentRenderer.Remove(controller);
            }
        }

        public static void ApplyRemoteObjectOnly(Controller controller, string objectId)
        {
            ObjectsAttachmentRenderer.ApplyRemoteObjectOnly(controller, objectId, 1f);
        }

        /// <summary>
        /// Remoto: sin half-color ni corona (más liviano en MP).
        /// </summary>
        private static void ApplyBodyColorsRemoteLight(Controller controller, BodyColors colors)
        {
            bool savedHalf = colors.HalfColorEnabled;
            colors.HalfColorEnabled = false;
            ApplyBodyColorsInternal(controller, colors, applyCrown: false);
            colors.HalfColorEnabled = savedHalf;
        }

        public static void ApplyBodyColors(Controller controller, BodyColors colors)
        {
            ApplyBodyColors(controller, colors, clearThrottle: true);
        }

        /// <summary>
        /// Aplica colores. Si clearThrottle=false, no resetea LineMaintainThrottle:
        /// usado por RGB animado donde el hash de color cambia naturalmente y no se
        /// necesita repintar todo cada frame.
        /// </summary>
        public static void ApplyBodyColors(Controller controller, BodyColors colors, bool clearThrottle)
        {
            if (controller == null || colors == null)
            {
                return;
            }

            if (!LocalPlayerResolver.IsLocalController(controller))
            {
                return;
            }

            if (clearThrottle)
            {
                LineMaintainThrottle.Clear();
            }

            ApplyBodyColorsInternal(controller, colors, applyCrown: true);
        }

        private static void ApplyBodyColorsInternal(Controller controller, BodyColors colors, bool applyCrown)
        {
            Transform renderersRoot = FindRenderersRoot(controller.transform);
            if (renderersRoot != null)
            {
                ApplyAllUnder(renderersRoot, colors, controller);
            }
            else
            {
                ApplyAllUnder(controller.transform, colors, controller);
            }

            ApplyHead(controller, colors);
            if (!_legacyGlowCleaned)
            {
                CleanupLegacyGlowObjects(controller.transform, destroyAuras: false);
                _legacyGlowCleaned = true;
            }

            if (applyCrown && LocalPlayerResolver.IsLocalController(controller))
            {
                ApplyCrownForPlayer(controller, colors);
            }
        }

        public static bool IsModCosmeticObject(GameObject go)
        {
            if (go == null)
            {
                return false;
            }

            string n = go.name;
            if (n != null && n.StartsWith("SFCC_"))
            {
                return true;
            }

            Transform parent = go.transform.parent;
            return parent != null && parent.name != null && parent.name.StartsWith("SFCC_");
        }

        public static void ApplyGlowAura(Controller controller, GlowSettings glow, BodyColors bodyColors = null)
        {
            if (controller == null || glow == null)
            {
                return;
            }

            GlowAuraRenderer.Apply(controller, glow, bodyColors);
        }

        public static void RemoveGlow(Controller controller)
        {
            GlowAuraRenderer.RemoveAll(controller);
        }

        public static void MaintainSetLineColor(SetLinePositions setLine, BodyColors colors, ColorConfig config)
        {
            if (setLine == null || colors == null || SetLineLineField == null)
            {
                return;
            }

            LineRenderer line = SetLineLineField.GetValue(setLine) as LineRenderer;
            if (line == null
                || line.gameObject.name.StartsWith("SFCC_"))
            {
                return;
            }

            string rendererName = ResolveRendererName(setLine.gameObject.name, line.gameObject.name);
            StickColorPart part;
            if (!RendererMap.TryGetValue(rendererName, out part))
            {
                return;
            }

            Color bodyColor = colors.Get(part);
            bool useHalf = colors.HalfColorEnabled && colors.SupportsHalfColor(part)
                && BoneColorSplitter.SupportsHalfSplit(rendererName);

            int lineId = line.GetInstanceID();
            float splitT = 0.5f;
            Color distal = colors.GetDistal(part);
            bool splitChanged = false;
            if (useHalf)
            {
                Controller controller = setLine.transform.root.GetComponent<Controller>();
                splitT = BoneColorSplitter.ResolveSplitTTracked(controller, line, rendererName, lineId, out splitChanged);
                HalfColorCache.Store(lineId, splitT, bodyColor, distal);
            }
            else if (ColorsMatch(line.startColor, bodyColor) && ColorsMatch(line.endColor, bodyColor))
            {
                LineMaintainThrottle.ClearForceDirtyIfNeeded();
                return;
            }

            int colorHash = LineMaintainThrottle.ComputeBodyHash(bodyColor, distal, useHalf, splitT);
            bool colorsLookApplied = ColorsMatch(line.startColor, bodyColor)
                && ColorsMatch(line.endColor, useHalf ? distal : bodyColor);

            if (useHalf)
            {
                CachedHalfColor entry;
                HalfColorCache.TryGet(lineId, out entry);
                if (!splitChanged && colorsLookApplied && entry != null && entry.ColorHash == colorHash)
                {
                    LineMaintainThrottle.ClearForceDirtyIfNeeded();
                    return;
                }

                if (!splitChanged
                    && LineMaintainThrottle.ShouldSkip(lineId, colorHash, config, colorsLookApplied))
                {
                    return;
                }

                if (!splitChanged
                    && !LineMaintainThrottle.TryConsumeHeavyApply(lineId, colorsLookApplied))
                {
                    return;
                }

                HalfColorCache.Store(lineId, splitT, bodyColor, distal);
                HalfColorCache.TryGet(lineId, out entry);
                if (entry == null)
                {
                    entry = new CachedHalfColor();
                }

                if (entry.CachedGradient == null || entry.ColorHash != colorHash)
                {
                    entry.CachedGradient = BoneColorSplitter.BuildGradient(bodyColor, distal, splitT);
                    HalfColorCache.StoreGradient(lineId, entry.CachedGradient, colorHash);
                    HalfColorCache.TryGet(lineId, out entry);
                }

                BoneColorSplitter.ApplyHalfGradient(line, bodyColor, distal, splitT, entry.CachedGradient);
            }
            else
            {
                if (LineMaintainThrottle.ShouldSkip(lineId, colorHash, config, colorsLookApplied))
                {
                    return;
                }

                if (!LineMaintainThrottle.TryConsumeHeavyApply(lineId, colorsLookApplied))
                {
                    return;
                }

                line.startColor = bodyColor;
                line.endColor = bodyColor;
                MaterialColorHelper.ApplyLineMaterial(line, bodyColor);
            }

            Material mat = line.sharedMaterial != null ? line.sharedMaterial : line.material;
            if (SetLineStartMatField != null && mat != null)
            {
                SetLineStartMatField.SetValue(setLine, mat);
            }

            LineMaintainThrottle.MarkApplied(lineId, colorHash);
            LineMaintainThrottle.ClearForceDirtyIfNeeded();
        }

        public static Transform FindRendererTransform(Transform root, string rendererKey)
        {
            return FindChildByName(root, rendererKey);
        }

        private static bool ColorsMatch(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.02f
                && Mathf.Abs(a.g - b.g) < 0.02f
                && Mathf.Abs(a.b - b.b) < 0.02f
                && Mathf.Abs(a.a - b.a) < 0.02f;
        }

        public static void ApplyAllUnder(Transform root, BodyColors colors, Controller controller)
        {
            foreach (KeyValuePair<string, StickColorPart> entry in RendererMap)
            {
                Transform node = FindChildByName(root, entry.Key);
                if (node == null)
                {
                    continue;
                }

                ApplyOnTransform(controller, node, entry.Key, colors.Get(entry.Value), colors);
            }
        }

        public static void ApplyHead(Controller controller, BodyColors colors)
        {
            if (controller == null || colors == null)
            {
                return;
            }

            Color headColor = colors.Head;
            HeadRenderer headRenderer = controller.GetComponentInChildren<HeadRenderer>(true);
            if (headRenderer != null)
            {
                ApplyOnTransform(controller, headRenderer.transform, "headRenderer", headColor, colors, includeTaggedDontChange: true);
            }

            Transform renderers = FindRenderersRoot(controller.transform);
            if (renderers != null)
            {
                Transform headNode = FindChildByName(renderers, "headRenderer");
                if (headNode != null)
                {
                    ApplyOnTransform(controller, headNode, "headRenderer", headColor, colors, includeTaggedDontChange: true);
                }
            }
        }

        public static void ApplyOnTransform(
            Controller controller,
            Transform node,
            string rendererName,
            Color bodyColor,
            BodyColors colors,
            bool includeTaggedDontChange = false)
        {
            StickColorPart part;
            RendererMap.TryGetValue(rendererName, out part);

            bool useHalf = colors.HalfColorEnabled && part != default(StickColorPart)
                && colors.SupportsHalfColor(part) && BoneColorSplitter.SupportsHalfSplit(rendererName);

            LineRenderer[] lines = node.GetComponentsInChildren<LineRenderer>(true);
            foreach (LineRenderer line in lines)
            {
                if (line == null || line.gameObject.name.StartsWith("SFCC_"))
                {
                    continue;
                }

                string lineRendererName = ResolveRendererName(rendererName, line.gameObject.name);

                if (useHalf && controller != null)
                {
                    int lineId = line.GetInstanceID();
                    Color distal = colors.GetDistal(part);
                    float splitT = BoneColorSplitter.ResolveSplitT(controller, line, lineRendererName);
                    int hash = LineMaintainThrottle.ComputeBodyHash(bodyColor, distal, true, splitT);
                    HalfColorCache.Store(lineId, splitT, bodyColor, distal);
                    CachedHalfColor entry;
                    HalfColorCache.TryGet(lineId, out entry);
                    if (entry == null || entry.CachedGradient == null || entry.ColorHash != hash)
                    {
                        Gradient gradient = BoneColorSplitter.BuildGradient(bodyColor, distal, splitT);
                        HalfColorCache.StoreGradient(lineId, gradient, hash);
                        HalfColorCache.TryGet(lineId, out entry);
                    }

                    BoneColorSplitter.ApplyHalfGradient(line, bodyColor, distal, splitT, entry.CachedGradient);
                }
                else
                {
                    MaterialColorHelper.ApplyLineMaterial(line, bodyColor);
                    line.startColor = bodyColor;
                    line.endColor = bodyColor;
                }

                PinSetLinePositionsMaterial(line.transform, bodyColor);
            }

            SpriteRenderer[] sprites = node.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sprite in sprites)
            {
                if (sprite == null || IsModCosmeticObject(sprite.gameObject))
                {
                    continue;
                }

                if (!includeTaggedDontChange && sprite.transform.tag == "DontChangeColor")
                {
                    continue;
                }

                MaterialColorHelper.ApplyColor(sprite, bodyColor);
            }

            Renderer[] renderers = node.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer is LineRenderer || renderer is SpriteRenderer)
                {
                    continue;
                }

                MaterialColorHelper.ApplyColor(renderer, bodyColor);
            }
        }

        public static void PinSetLinePositionsMaterial(Transform node, Color bodyColor)
        {
            SetLinePositions setLine = node.GetComponent<SetLinePositions>();
            if (setLine == null || SetLineLineField == null || SetLineStartMatField == null)
            {
                return;
            }

            LineRenderer line = SetLineLineField.GetValue(setLine) as LineRenderer;
            if (line == null)
            {
                return;
            }

            Material mat = line.material ?? MaterialColorHelper.GetOrCreate(line, bodyColor);
            SetLineStartMatField.SetValue(setLine, mat);
            line.material = mat;
        }

        public static bool TryGetPartForRenderer(string rendererName, out StickColorPart part)
        {
            if (RendererMap.TryGetValue(rendererName, out part))
            {
                return true;
            }

            if (string.IsNullOrEmpty(rendererName))
            {
                part = default(StickColorPart);
                return false;
            }

            string lower = rendererName.ToLowerInvariant();
            if (lower.Contains("head"))
            {
                part = StickColorPart.Head;
                return true;
            }

            if (lower.Contains("spine"))
            {
                part = StickColorPart.Spine;
                return true;
            }

            part = default(StickColorPart);
            return false;
        }

        public static bool TryGetColorForSetLinePositions(SetLinePositions setLine, BodyColors colors, out Color color)
        {
            color = Color.white;
            if (setLine == null || colors == null)
            {
                return false;
            }

            StickColorPart part;
            string name = setLine.gameObject.name;
            if (SetLineLineField != null)
            {
                LineRenderer line = SetLineLineField.GetValue(setLine) as LineRenderer;
                if (line != null)
                {
                    name = ResolveRendererName(name, line.gameObject.name);
                }
            }

            if (!RendererMap.TryGetValue(name, out part))
            {
                return false;
            }

            color = colors.Get(part);
            return true;
        }

        private static Crown GetCachedCrown()
        {
            if (_cachedCrown != null && Time.realtimeSinceStartup - _lastCrownLookupTime < 5f)
            {
                return _cachedCrown;
            }

            _lastCrownLookupTime = Time.realtimeSinceStartup;
            _cachedCrown = Object.FindObjectOfType<Crown>();
            return _cachedCrown;
        }

        public static void ApplyCrownForPlayer(Controller controller, BodyColors colors)
        {
            Crown crown = GetCachedCrown();
            if (crown == null || crown.crownLevels == null)
            {
                return;
            }

            if (crown.crownBarrer != null && crown.crownBarrer != controller)
            {
                return;
            }

            Color crownColor = colors.Crown;
            foreach (GameObject level in crown.crownLevels)
            {
                if (level == null || !level.activeInHierarchy)
                {
                    continue;
                }

                Renderer[] renderers = level.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    MaterialColorHelper.ApplyColor(renderer, crownColor);
                }
            }
        }

        private static void CleanupLegacyGlowObjects(Transform root, bool destroyAuras)
        {
            if (root == null)
            {
                return;
            }

            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                Transform t = all[i];
                if (t == null)
                {
                    continue;
                }

                string n = t.gameObject.name;
                if (n.StartsWith("SFCC_Glow") || (destroyAuras && n.StartsWith("SFCC_Aura")))
                {
                    Object.Destroy(t.gameObject);
                }
            }
        }

        public static Transform FindRenderersRoot(Transform root)
        {
            Transform direct = root.Find("Renderers");
            if (direct != null)
            {
                return direct;
            }

            return FindChildByName(root, "Renderers");
        }

        private static Transform FindChildByName(Transform parent, string childName)
        {
            if (parent.name.Equals(childName, System.StringComparison.OrdinalIgnoreCase))
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform found = FindChildByName(parent.GetChild(i), childName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        public static void LogRendererMap(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            Transform root = FindRenderersRoot(controller.transform) ?? controller.transform;
            foreach (KeyValuePair<string, StickColorPart> entry in RendererMap)
            {
                Transform t = FindChildByName(root, entry.Key);
                string path = t != null ? GetPath(t) : "(no encontrado)";
                ModLog.Info(entry.Value + " [" + entry.Key + "] -> " + path);
            }
        }

        private static string ResolveRendererName(string setLineObjectName, string lineObjectName)
        {
            if (RendererMap.ContainsKey(setLineObjectName))
            {
                return setLineObjectName;
            }

            if (RendererMap.ContainsKey(lineObjectName))
            {
                return lineObjectName;
            }

            return setLineObjectName;
        }

        private static string GetPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }

            return path;
        }
    }
}
