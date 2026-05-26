using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Huesos visuales: LineRenderer hijo que copia vértices del hueso del juego cada frame (como GlowAura).
    /// </summary>
    public static class CosmeticBoneLineMirror
    {
        private static readonly FieldInfo SetLineLineField = AccessTools.Field(typeof(SetLinePositions), "line");
        private static readonly Dictionary<string, LineRenderer> MirrorByKey = new Dictionary<string, LineRenderer>();
        private static readonly Dictionary<int, Material> MaterialByTextureId = new Dictionary<int, Material>();

        public static LineRenderer TryGetSourceLine(SetLinePositions setLine)
        {
            if (setLine == null || SetLineLineField == null)
            {
                return null;
            }

            LineRenderer line = SetLineLineField.GetValue(setLine) as LineRenderer;
            if (line == null || line.gameObject.name.StartsWith("SFCC_"))
            {
                return null;
            }

            return line;
        }

        public static LineRenderer GetOrCreate(LineRenderer source, string objectName)
        {
            if (source == null || string.IsNullOrEmpty(objectName))
            {
                return null;
            }

            string cacheKey = BuildMirrorKey(source.GetInstanceID(), objectName);
            LineRenderer mirror;
            if (MirrorByKey.TryGetValue(cacheKey, out mirror) && mirror != null)
            {
                return mirror;
            }

            Transform existing = source.transform.Find(objectName);
            if (existing != null)
            {
                mirror = existing.GetComponent<LineRenderer>();
                if (mirror != null)
                {
                    MirrorByKey[cacheKey] = mirror;
                    return mirror;
                }
            }

            GameObject go = new GameObject(objectName);
            go.transform.SetParent(source.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            try
            {
                go.tag = "DontChangeColor";
            }
            catch
            {
            }

            mirror = go.AddComponent<LineRenderer>();
            mirror.useWorldSpace = source.useWorldSpace;
            mirror.alignment = source.alignment;
            mirror.numCornerVertices = 6;
            mirror.numCapVertices = 6;
            mirror.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mirror.receiveShadows = false;

            if (source.sharedMaterial != null)
            {
                mirror.sharedMaterial = source.sharedMaterial;
            }

            MirrorByKey[cacheKey] = mirror;
            return mirror;
        }

        private static string BuildMirrorKey(int sourceId, string objectName)
        {
            return sourceId + ":" + objectName;
        }

        public static void SyncSolidOverlay(
            LineRenderer source,
            LineRenderer mirror,
            Color tint,
            float widthMultiplier,
            int sortingOrderBoost)
        {
            if (source == null || mirror == null)
            {
                return;
            }

            CopyLinePositions(source, mirror);
            mirror.useWorldSpace = source.useWorldSpace;
            mirror.alignment = source.alignment;
            mirror.widthMultiplier = source.widthMultiplier * widthMultiplier;
            mirror.sortingLayerID = source.sortingLayerID;
            mirror.sortingOrder = source.sortingOrder + sortingOrderBoost;
            mirror.enabled = source.enabled;

            MaterialColorHelper.ApplyLineMaterial(mirror, tint);
            StickLineUtil.SetSolidColor(mirror, tint);
        }

        /// <summary>Línea espejo con textura del sprite (se curva con el hueso, detalle del modelo).</summary>
        public static void SyncSpriteTexturedOverlay(
            LineRenderer source,
            LineRenderer mirror,
            Sprite sprite,
            float widthMultiplier,
            int sortingOrderBoost)
        {
            if (source == null || mirror == null || sprite == null)
            {
                return;
            }

            CopyLinePositions(source, mirror);
            mirror.useWorldSpace = source.useWorldSpace;
            mirror.alignment = source.alignment;
            mirror.widthMultiplier = source.widthMultiplier * widthMultiplier;
            mirror.sortingLayerID = source.sortingLayerID;
            mirror.sortingOrder = source.sortingOrder + sortingOrderBoost;
            mirror.enabled = source.enabled;

            float lineLen = EstimateLineLength(source);
            float spriteAlong = Mathf.Max(sprite.bounds.size.y, sprite.bounds.size.x * 0.25f, 0.01f);
            float tileX = Mathf.Max(lineLen / spriteAlong, 0.65f);
            Material mat = GetOrCreateSpriteLineMaterial(sprite, tileX);
            mirror.sharedMaterial = mat;
            mirror.material = mat;
            mirror.startColor = Color.white;
            mirror.endColor = Color.white;
            mirror.colorGradient = BuildSolidGradient();
        }

        private static Gradient BuildSolidGradient()
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                });
            return gradient;
        }

        private static Material GetOrCreateSpriteLineMaterial(Sprite sprite, float tileX)
        {
            Texture2D tex = sprite.texture;
            int texId = tex != null ? tex.GetInstanceID() : sprite.GetInstanceID();
            int key = texId ^ (Mathf.RoundToInt(tileX * 100f) << 16);
            Material mat;
            if (MaterialByTextureId.TryGetValue(key, out mat) && mat != null)
            {
                return mat;
            }

            mat = new Material(HatMeshFactory.GetHatMaterial());
            if (tex != null)
            {
                mat.mainTexture = tex;
            }

            mat.mainTextureScale = new Vector2(tileX, 1f);
            mat.color = new Color(1f, 1f, 1f, 1f);
            mat.renderQueue = 3100;
            MaterialByTextureId[key] = mat;
            return mat;
        }

        public static float EstimateLineLength(LineRenderer line)
        {
            int count = line.positionCount;
            if (count < 2)
            {
                return 1f;
            }

            float sum = 0f;
            for (int i = 1; i < count; i++)
            {
                Vector3 a = line.GetPosition(i - 1);
                Vector3 b = line.GetPosition(i);
                if (line.useWorldSpace)
                {
                    sum += Vector3.Distance(a, b);
                }
                else
                {
                    sum += Vector3.Distance(line.transform.TransformPoint(a), line.transform.TransformPoint(b));
                }
            }

            return Mathf.Max(sum, 0.2f);
        }

        /// <summary>Solo el tramo distal (pie) visible; el muslo queda transparente.</summary>
        public static void SyncDistalFootOverlay(
            LineRenderer source,
            LineRenderer mirror,
            Color tint,
            float widthMultiplier,
            int sortingOrderBoost,
            float footStartNormalized)
        {
            SyncSolidOverlay(source, mirror, tint, widthMultiplier, sortingOrderBoost);
            ApplyDistalAlphaGradient(mirror, footStartNormalized);
        }

        public static void DestroyMirror(LineRenderer source)
        {
            if (source == null)
            {
                return;
            }

            DestroyAllMirrorsUnder(source.transform);
        }

        public static void DestroyAllMirrorsUnder(Transform root)
        {
            if (root == null)
            {
                return;
            }

            LineRenderer[] lines = root.GetComponentsInChildren<LineRenderer>(true);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer line = lines[i];
                if (line == null || line.gameObject == null)
                {
                    continue;
                }

                string n = line.gameObject.name;
                if (!n.StartsWith("SFCC_"))
                {
                    continue;
                }

                if (n.StartsWith("SFCC_Shoe_") || n.StartsWith("SFCC_Tops_"))
                {
                    Object.Destroy(line.gameObject);
                }
            }

            List<string> stale = new List<string>();
            foreach (KeyValuePair<string, LineRenderer> kv in MirrorByKey)
            {
                if (kv.Value == null)
                {
                    stale.Add(kv.Key);
                }
            }

            for (int i = 0; i < stale.Count; i++)
            {
                MirrorByKey.Remove(stale[i]);
            }
        }

        public static void ClearRegistry()
        {
            MirrorByKey.Clear();
        }

        private static void CopyLinePositions(LineRenderer source, LineRenderer mirror)
        {
            int count = source.positionCount;
            mirror.positionCount = count;
            mirror.useWorldSpace = source.useWorldSpace;
            for (int i = 0; i < count; i++)
            {
                mirror.SetPosition(i, source.GetPosition(i));
            }
        }

        private static void ApplyDistalAlphaGradient(LineRenderer line, float footStartNormalized)
        {
            float start = Mathf.Clamp01(footStartNormalized);
            float rampEnd = Mathf.Min(start + 0.1f, 1f);

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0f, start),
                    new GradientAlphaKey(1f, rampEnd),
                    new GradientAlphaKey(1f, 1f)
                });

            line.colorGradient = gradient;
            line.startColor = Color.white;
            line.endColor = Color.white;
        }
    }
}
