using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class MaterialColorHelper
    {
        private static readonly Dictionary<int, Material> MaterialCache = new Dictionary<int, Material>();

        public static void ApplyColor(Renderer renderer, Color bodyColor)
        {
            if (renderer == null)
            {
                return;
            }

            Material mat = GetOrCreate(renderer, bodyColor);
            renderer.material = mat;

            if (renderer is SpriteRenderer sprite)
            {
                sprite.color = bodyColor;
            }
        }

        public static void ApplyLineMaterial(LineRenderer line, Color bodyColor)
        {
            if (line == null)
            {
                return;
            }

            line.material = GetOrCreate(line, bodyColor);
        }

        public static void ApplyLineMaterialIfUnchanged(LineRenderer line, Color bodyColor)
        {
            if (line == null)
            {
                return;
            }

            int key = line.GetInstanceID() ^ (bodyColor.GetHashCode() * 397);
            Material cached;
            if (MaterialCache.TryGetValue(key, out cached) && cached != null)
            {
                Material current = line.material;
                if (current == cached || line.sharedMaterial == cached)
                {
                    if (ColorsApproximatelyMatch(current, bodyColor))
                    {
                        return;
                    }
                }
            }

            ApplyLineMaterial(line, bodyColor);
        }

        private static bool ColorsApproximatelyMatch(Material mat, Color target)
        {
            if (mat == null)
            {
                return false;
            }

            Color c = mat.color;
            return Mathf.Abs(c.r - target.r) < 0.02f
                && Mathf.Abs(c.g - target.g) < 0.02f
                && Mathf.Abs(c.b - target.b) < 0.02f
                && Mathf.Abs(c.a - target.a) < 0.02f;
        }

        public static Material GetOrCreate(Renderer renderer, Color bodyColor)
        {
            int key = renderer.GetInstanceID() ^ (bodyColor.GetHashCode() * 397);
            Material cached;
            if (MaterialCache.TryGetValue(key, out cached) && cached != null)
            {
                ApplyToMaterial(cached, bodyColor);
                return cached;
            }

            Material source = renderer.material ?? renderer.sharedMaterial;
            Material mat = source != null ? new Material(source) : new Material(Shader.Find("Sprites/Default"));
            ApplyToMaterial(mat, bodyColor);
            MaterialCache[key] = mat;
            return mat;
        }

        public static void ApplyToMaterial(Material mat, Color bodyColor)
        {
            if (mat == null)
            {
                return;
            }

            mat.color = bodyColor;
            if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", bodyColor);
            }

            if (mat.HasProperty("_TintColor"))
            {
                mat.SetColor("_TintColor", bodyColor);
            }

            if (mat.HasProperty("_MainColor"))
            {
                mat.SetColor("_MainColor", bodyColor);
            }

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", bodyColor * 0.35f);
            }
        }

        public static void ApplyWeaponNeon(Material mat, Color color, float strength)
        {
            if (mat == null)
            {
                return;
            }

            ApplyToMaterial(mat, color);
            Color emission = color * (1.2f + strength * 1.5f);
            emission.a = 1f;
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", emission);
            }

            mat.color = Color.Lerp(color, Color.white, 0.15f * strength);
        }

        public static void ClearCache()
        {
            MaterialCache.Clear();
        }
    }
}
