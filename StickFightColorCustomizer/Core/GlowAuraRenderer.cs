using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class GlowAuraRenderer
    {
        private const float TorsoAuraWidthBoost = 1.08f;

        private static readonly string[] LineRendererKeys =
        {
            "headRenderer", "spineRenderer", "legRenderer", "legRenderer2",
            "handRenderer", "handRenderer2", "Wings"
        };

        private static readonly FieldInfo SetLineLineField = AccessTools.Field(typeof(SetLinePositions), "line");

        private static readonly Dictionary<int, LineRenderer> LineAuraBySource = new Dictionary<int, LineRenderer>();
        private static readonly Dictionary<int, SpriteRenderer> HeadAuraBySource = new Dictionary<int, SpriteRenderer>();
        private static readonly Dictionary<int, Gradient> GradientByAuraLine = new Dictionary<int, Gradient>();
        private static readonly Dictionary<int, int> GradientHashByAuraLine = new Dictionary<int, int>();
        private static Material _lineAuraMaterialTemplate;
        private static Material _headAuraMaterialTemplate;
        private static readonly Dictionary<int, Material> LineMaterialCache = new Dictionary<int, Material>();
        private static readonly Dictionary<int, Material> HeadMaterialCache = new Dictionary<int, Material>();
        private static readonly Dictionary<int, float> LastStyleSyncBySource = new Dictionary<int, float>();
        private const float StyleSyncInterval = 0.1f;
        private const float MaxPeakAlpha = 0.3f;

        public static void ResetSyncState()
        {
            LastStyleSyncBySource.Clear();
            GlowAuraThrottle.Reset();
            GlowSyncBudget.Reset();
        }

        public static void Apply(Controller controller, GlowSettings glow, BodyColors bodyColors = null)
        {
            if (controller == null || glow == null || !glow.Enabled)
            {
                RemoveAll(controller);
                return;
            }

            DestroyLegacyHeadAuras(controller);
            ApplyLineAuras(controller.transform, glow, bodyColors);
            SyncAll(controller, glow, bodyColors);
        }

        private static void DestroyLegacyHeadAuras(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            Transform[] all = controller.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                Transform t = all[i];
                if (t != null && t.gameObject.name == "SFCC_Aura_Head")
                {
                    Object.Destroy(t.gameObject);
                }
            }
        }

        public static void SyncForSetLine(SetLinePositions setLine, GlowSettings glow, BodyColors bodyColors)
        {
            if (setLine == null || glow == null || !glow.Enabled || SetLineLineField == null)
            {
                return;
            }

            LineRenderer source = SetLineLineField.GetValue(setLine) as LineRenderer;
            if (source == null || source.gameObject.name.StartsWith("SFCC_Aura"))
            {
                return;
            }

            int sourceId = source.GetInstanceID();
            string key = ResolveRendererKey(source);
            if (!ShouldAuraLine(key, glow))
            {
                return;
            }

            LineRenderer aura;
            if (!LineAuraBySource.TryGetValue(sourceId, out aura) || aura == null)
            {
                StickColorPart part;
                PlayerColorApplier.TryGetPartForRenderer(key, out part);
                aura = GetOrCreateLineAura(source);
                StyleLineAura(aura, source, glow, bodyColors, part);
                return;
            }

            StickColorPart syncPart;
            PlayerColorApplier.TryGetPartForRenderer(key, out syncPart);
            SyncLineTransform(source, aura, glow, syncPart);
            if (ShouldSyncStyle(sourceId))
            {
                ApplyLineColorGradient(aura, glow, bodyColors, syncPart);
            }
        }

        public static void SyncAll(Controller controller, GlowSettings glow, BodyColors bodyColors = null)
        {
            if (controller == null || glow == null || !glow.Enabled)
            {
                return;
            }

            ForEachGlowSourceLine(controller.transform, glow, (source, part) =>
            {
                LineRenderer aura;
                if (!LineAuraBySource.TryGetValue(source.GetInstanceID(), out aura) || aura == null)
                {
                    return;
                }

                SyncLineTransform(source, aura, glow, part);
                if (ShouldSyncStyle(source.GetInstanceID()))
                {
                    ApplyLineColorGradient(aura, glow, bodyColors, part);
                }
            });
        }

        public static void RemoveAll(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            Transform root = PlayerColorApplier.FindRenderersRoot(controller.transform) ?? controller.transform;
            HashSet<int> sourceIds = new HashSet<int>();
            LineRenderer[] lines = root.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line != null && !line.gameObject.name.StartsWith("SFCC_Aura"))
                {
                    sourceIds.Add(line.GetInstanceID());
                }
            }

            List<int> keysToRemove = new List<int>();
            foreach (KeyValuePair<int, LineRenderer> entry in LineAuraBySource)
            {
                if (sourceIds.Contains(entry.Key))
                {
                    keysToRemove.Add(entry.Key);
                }
            }

            for (int i = 0; i < keysToRemove.Count; i++)
            {
                int key = keysToRemove[i];
                LineRenderer aura;
                if (LineAuraBySource.TryGetValue(key, out aura) && aura != null)
                {
                    Object.Destroy(aura.gameObject);
                }

                LineAuraBySource.Remove(key);
                GradientByAuraLine.Remove(key);
                GradientHashByAuraLine.Remove(key);
            }

            HeadRenderer head = controller.GetComponentInChildren<HeadRenderer>(true);
            if (head != null)
            {
                SpriteRenderer headSource = head.GetComponent<SpriteRenderer>();
                if (headSource != null)
                {
                    int headId = headSource.GetInstanceID();
                    SpriteRenderer headAura;
                    if (HeadAuraBySource.TryGetValue(headId, out headAura) && headAura != null)
                    {
                        Object.Destroy(headAura.gameObject);
                    }

                    HeadAuraBySource.Remove(headId);
                }
            }

            Transform[] all = controller.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                Transform t = all[i];
                if (t != null && t.gameObject.name.StartsWith("SFCC_Aura"))
                {
                    Object.Destroy(t.gameObject);
                }
            }
        }

        private static void ApplyLineAuras(Transform searchRoot, GlowSettings glow, BodyColors bodyColors)
        {
            ForEachGlowSourceLine(searchRoot, glow, (source, part) =>
            {
                LineRenderer aura = GetOrCreateLineAura(source);
                StyleLineAura(aura, source, glow, bodyColors, part);
            });
        }

        private static void ForEachGlowSourceLine(
            Transform searchRoot,
            GlowSettings glow,
            System.Action<LineRenderer, StickColorPart> action)
        {
            if (searchRoot == null || glow == null || action == null)
            {
                return;
            }

            LineRenderer[] lines = searchRoot.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer source = lines[i];
                if (source == null || source.gameObject.name.StartsWith("SFCC_Aura"))
                {
                    continue;
                }

                string key = ResolveRendererKey(source);
                if (!ShouldAuraLine(key, glow))
                {
                    continue;
                }

                StickColorPart part;
                PlayerColorApplier.TryGetPartForRenderer(key, out part);
                action(source, part);
            }
        }

        private static void ApplyHeadAura(Controller controller, GlowSettings glow)
        {
            if (!glow.OnHead)
            {
                return;
            }

            HeadRenderer head = controller.GetComponentInChildren<HeadRenderer>(true);
            if (head == null)
            {
                return;
            }

            SpriteRenderer source = head.GetComponent<SpriteRenderer>();
            if (source == null)
            {
                return;
            }

            SpriteRenderer aura = GetOrCreateHeadAura(source);
            StyleHeadAura(aura, source, glow);
        }

        private static void SyncOneLine(
            LineRenderer source,
            LineRenderer aura,
            GlowSettings glow,
            BodyColors bodyColors,
            StickColorPart part)
        {
            SyncLineTransform(source, aura, glow, part);
            ApplyLineColorGradient(aura, glow, bodyColors, part);
        }

        private static void SyncLineTransform(
            LineRenderer source,
            LineRenderer aura,
            GlowSettings glow,
            StickColorPart part = default(StickColorPart))
        {
            CopyLinePositions(source, aura);
            aura.useWorldSpace = source.useWorldSpace;
            aura.alignment = source.alignment;
            float widthMul = Mathf.Clamp(glow.AuraWidth * (0.7f + glow.Strength * 0.12f), 1.08f, 1.75f);
            if (part == StickColorPart.Spine || ResolveRendererKey(source) == "spineRenderer")
            {
                widthMul *= TorsoAuraWidthBoost;
            }

            aura.widthMultiplier = source.widthMultiplier * widthMul;
            aura.sortingLayerID = source.sortingLayerID;
            aura.sortingOrder = source.sortingOrder - 2;
            aura.enabled = source.enabled;
        }

        private static bool ShouldSyncStyle(int sourceLineId)
        {
            float now = Time.realtimeSinceStartup;
            float last;
            if (LastStyleSyncBySource.TryGetValue(sourceLineId, out last) && now - last < StyleSyncInterval)
            {
                return false;
            }

            LastStyleSyncBySource[sourceLineId] = now;
            return true;
        }

        private static void SyncHead(Controller controller, GlowSettings glow)
        {
            if (!glow.OnHead)
            {
                return;
            }

            HeadRenderer head = controller.GetComponentInChildren<HeadRenderer>(true);
            if (head == null)
            {
                return;
            }

            SpriteRenderer headSource = head.GetComponent<SpriteRenderer>();
            SpriteRenderer headAura;
            if (headSource != null && HeadAuraBySource.TryGetValue(headSource.GetInstanceID(), out headAura) && headAura != null)
            {
                StyleHeadAura(headAura, headSource, glow);
            }
        }

        private static void CopyLinePositions(LineRenderer source, LineRenderer aura)
        {
            int count = source.positionCount;
            aura.positionCount = count;
            aura.useWorldSpace = source.useWorldSpace;
            for (int i = 0; i < count; i++)
            {
                aura.SetPosition(i, source.GetPosition(i));
            }
        }

        private static void ApplyLineColorGradient(
            LineRenderer aura,
            GlowSettings glow,
            BodyColors bodyColors,
            StickColorPart part)
        {
            float peakAlpha = Mathf.Min(
                MaxPeakAlpha,
                glow.AuraAlpha * (0.55f + glow.Strength * 0.25f));
            Color proximal = glow.Color;
            Color distal = glow.Color;
            proximal.a = 1f;
            distal.a = 1f;

            float blend = Mathf.Clamp01(glow.BodyColorBlend);
            bool useBodyGradient = bodyColors != null && bodyColors.HalfColorEnabled
                && part != default(StickColorPart) && bodyColors.SupportsHalfColor(part);

            if (useBodyGradient)
            {
                Color bodyProx = bodyColors.Get(part);
                Color bodyDist = bodyColors.GetDistal(part);
                proximal = Color.Lerp(glow.Color, bodyProx, blend);
                distal = Color.Lerp(glow.Color, bodyDist, blend);
                proximal.a = 1f;
                distal.a = 1f;
            }
            else if (blend > 0.01f && bodyColors != null && part != default(StickColorPart))
            {
                Color bodyProx = bodyColors.Get(part);
                proximal = Color.Lerp(glow.Color, bodyProx, blend * 0.65f);
                distal = proximal;
                proximal.a = 1f;
                distal.a = 1f;
            }

            int auraId = aura.GetInstanceID();
            int hash = ComputeGlowGradientHash(proximal, distal, peakAlpha, useBodyGradient);
            Gradient gradient;
            if (!GradientByAuraLine.TryGetValue(auraId, out gradient) || gradient == null
                || !GradientHashByAuraLine.TryGetValue(auraId, out int prevHash) || prevHash != hash)
            {
                gradient = BuildSoftLineGradient(proximal, distal, peakAlpha, useBodyGradient);
                GradientByAuraLine[auraId] = gradient;
                GradientHashByAuraLine[auraId] = hash;
            }

            aura.colorGradient = gradient;
            aura.startColor = Color.white;
            aura.endColor = Color.white;
        }

        private static Gradient BuildSoftLineGradient(Color proximal, Color distal, float peakAlpha, bool dualColor)
        {
            Gradient gradient = new Gradient();
            GradientAlphaKey[] alphaKeys = new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(peakAlpha * 0.12f, 0.12f),
                new GradientAlphaKey(peakAlpha * 0.45f, 0.32f),
                new GradientAlphaKey(peakAlpha, 0.5f),
                new GradientAlphaKey(peakAlpha * 0.45f, 0.68f),
                new GradientAlphaKey(peakAlpha * 0.12f, 0.88f),
                new GradientAlphaKey(0f, 1f)
            };

            if (dualColor)
            {
                gradient.SetKeys(
                    new[]
                    {
                        new GradientColorKey(proximal, 0f),
                        new GradientColorKey(proximal, 0.35f),
                        new GradientColorKey(distal, 0.65f),
                        new GradientColorKey(distal, 1f)
                    },
                    alphaKeys);
            }
            else
            {
                gradient.SetKeys(
                    new[]
                    {
                        new GradientColorKey(proximal, 0f),
                        new GradientColorKey(proximal, 0.5f),
                        new GradientColorKey(proximal, 1f)
                    },
                    alphaKeys);
            }

            return gradient;
        }

        private static bool ShouldAuraLine(string rendererKey, GlowSettings glow)
        {
            switch (rendererKey)
            {
                case "spineRenderer": return glow.OnTorso;
                case "handRenderer":
                case "handRenderer2": return glow.OnArms;
                case "legRenderer":
                case "legRenderer2": return glow.OnLegs;
                case "Wings": return glow.OnWings;
                case "headRenderer":
                case "Head":
                case "head": return glow.OnHead;
                default: return false;
            }
        }

        private static LineRenderer GetOrCreateLineAura(LineRenderer source)
        {
            int id = source.GetInstanceID();
            LineRenderer aura;
            if (LineAuraBySource.TryGetValue(id, out aura) && aura != null)
            {
                return aura;
            }

            GameObject go = new GameObject("SFCC_Aura_" + ResolveRendererKey(source));
            go.transform.SetParent(source.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            aura = go.AddComponent<LineRenderer>();
            aura.useWorldSpace = source.useWorldSpace;
            aura.alignment = source.alignment;
            aura.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            aura.receiveShadows = false;
            LineAuraBySource[id] = aura;
            return aura;
        }

        private static string ResolveRendererKey(LineRenderer source)
        {
            if (source.GetComponentInParent<HeadRenderer>() != null)
            {
                return "headRenderer";
            }

            Transform t = source.transform;
            while (t != null)
            {
                string n = t.gameObject.name;
                for (int i = 0; i < LineRendererKeys.Length; i++)
                {
                    if (n == LineRendererKeys[i])
                    {
                        return n;
                    }
                }

                if (n.IndexOf("spine", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "spineRenderer";
                }

                if (n.IndexOf("head", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "headRenderer";
                }

                t = t.parent;
            }

            return source.gameObject.name;
        }

        private static SpriteRenderer GetOrCreateHeadAura(SpriteRenderer source)
        {
            int id = source.GetInstanceID();
            SpriteRenderer aura;
            if (HeadAuraBySource.TryGetValue(id, out aura) && aura != null)
            {
                return aura;
            }

            GameObject go = new GameObject("SFCC_Aura_Head");
            go.transform.SetParent(source.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            aura = go.AddComponent<SpriteRenderer>();
            HeadAuraBySource[id] = aura;
            return aura;
        }

        private static void StyleLineAura(
            LineRenderer aura,
            LineRenderer source,
            GlowSettings glow,
            BodyColors bodyColors,
            StickColorPart part)
        {
            aura.numCornerVertices = 8;
            aura.numCapVertices = 8;
            aura.sharedMaterial = GetLineAuraMaterial(glow);
            aura.material = aura.sharedMaterial;
            SyncOneLine(source, aura, glow, bodyColors, part);
        }

        private static void StyleHeadAura(SpriteRenderer aura, SpriteRenderer source, GlowSettings glow)
        {
            aura.sprite = GlowTextureFactory.GetRadialSprite();
            aura.sortingLayerID = source.sortingLayerID;
            aura.sortingOrder = source.sortingOrder - 2;

            float scale = (1.35f + glow.Strength * 0.45f) * Mathf.Max(0.01f, Mathf.Max(source.transform.lossyScale.x, source.transform.localScale.x));
            aura.transform.localScale = Vector3.one * scale;

            Color c = glow.Color;
            c.a = glow.AuraAlpha * (0.4f + glow.Strength * 0.5f);
            aura.color = c;
            aura.sharedMaterial = GetHeadAuraMaterial(glow);
            aura.material = aura.sharedMaterial;
            aura.enabled = source.enabled;
        }

        private static int ComputeGlowMaterialHash(GlowSettings glow, bool head)
        {
            int h = glow.Color.GetHashCode();
            h = (h * 397) ^ Mathf.RoundToInt(glow.Strength * 100f);
            h = (h * 397) ^ Mathf.RoundToInt(glow.AuraAlpha * 100f);
            h = (h * 397) ^ (head ? 1 : 0);
            return h;
        }

        private static int ComputeGlowGradientHash(Color proximal, Color distal, float peakAlpha, bool dualColor)
        {
            int h = proximal.GetHashCode();
            h = (h * 397) ^ distal.GetHashCode();
            h = (h * 397) ^ Mathf.RoundToInt(peakAlpha * 1000f);
            h = (h * 397) ^ (dualColor ? 1 : 0);
            return h;
        }

        private static Material GetLineAuraMaterial(GlowSettings glow)
        {
            if (_lineAuraMaterialTemplate == null)
            {
                Shader shader = Shader.Find("Particles/Additive");
                if (shader == null)
                {
                    shader = Shader.Find("Legacy Shaders/Particles/Additive");
                }

                if (shader == null)
                {
                    shader = Shader.Find("Sprites/Default");
                }

                _lineAuraMaterialTemplate = new Material(shader);
            }

            Texture2D soft = GlowTextureFactory.GetLineSoft();
            if (_lineAuraMaterialTemplate.mainTexture != soft)
            {
                _lineAuraMaterialTemplate.mainTexture = soft;
                LineMaterialCache.Clear();
            }

            int hash = ComputeGlowMaterialHash(glow, false);
            Material cached;
            if (LineMaterialCache.TryGetValue(hash, out cached) && cached != null)
            {
                return cached;
            }

            Material inst = new Material(_lineAuraMaterialTemplate);
            Color tint = glow.Color * (0.92f + glow.Strength * 0.12f);
            tint.a = 1f;
            inst.color = tint;
            if (inst.HasProperty("_TintColor"))
            {
                inst.SetColor("_TintColor", tint);
            }

            LineMaterialCache[hash] = inst;
            return inst;
        }

        private static Material GetHeadAuraMaterial(GlowSettings glow)
        {
            if (_headAuraMaterialTemplate == null)
            {
                Shader shader = Shader.Find("Particles/Additive");
                if (shader == null)
                {
                    shader = Shader.Find("Legacy Shaders/Particles/Additive");
                }

                if (shader == null)
                {
                    shader = Shader.Find("Sprites/Default");
                }

                _headAuraMaterialTemplate = new Material(shader);
            }

            int hash = ComputeGlowMaterialHash(glow, true);
            Material cached;
            if (HeadMaterialCache.TryGetValue(hash, out cached) && cached != null)
            {
                return cached;
            }

            Material inst = new Material(_headAuraMaterialTemplate);
            Color tint = glow.Color * (1.15f + glow.Strength * 0.5f);
            tint.a = 1f;
            inst.color = tint;
            if (inst.HasProperty("_TintColor"))
            {
                inst.SetColor("_TintColor", tint);
            }

            HeadMaterialCache[hash] = inst;
            return inst;
        }
    }
}
