using System.Collections.Generic;
using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class WeaponColorApplier
    {
        private static readonly Dictionary<int, int> LastHashByWeaponId = new Dictionary<int, int>();

        public static void Apply(Transform weaponRoot, WeaponColorSettings settings, bool force = false)
        {
            if (weaponRoot == null || settings == null || !settings.Enabled)
            {
                return;
            }

            int weaponId = weaponRoot.gameObject.GetInstanceID();
            int colorHash = ComputeHash(settings);
            int prev;
            if (!force
                && LastHashByWeaponId.TryGetValue(weaponId, out prev)
                && prev == colorHash)
            {
                return;
            }

            LastHashByWeaponId[weaponId] = colorHash;
            Color color = settings.Color;

            if (settings.TintMesh)
            {
                ApplyMeshAndLines(weaponRoot, color, settings.NeonEnabled);
            }

            if (settings.TintParticles)
            {
                ApplyParticles(weaponRoot, color);
            }
        }

        public static void ApplyToLocalController(Controller controller, WeaponColorSettings settings, bool force = false)
        {
            if (controller == null || settings == null || !settings.Enabled)
            {
                return;
            }

            Transform active = WeaponColorResolver.GetActiveWeapon(controller);
            if (active != null)
            {
                Apply(active, settings, force);
            }
        }

        public static void ClearCache()
        {
            LastHashByWeaponId.Clear();
            WeaponColorResolver.InvalidateActiveWeaponCache();
        }

        public static void MarkDirty()
        {
            LastHashByWeaponId.Clear();
            WeaponColorResolver.InvalidateActiveWeaponCache();
        }

        public static bool IsTintableRenderer(Renderer renderer)
        {
            if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
            {
                return false;
            }

            if (renderer is ParticleSystemRenderer)
            {
                return false;
            }

            LineRenderer line = renderer as LineRenderer;
            if (line != null)
            {
                return line.positionCount > 0 || line.sharedMaterial != null || line.material != null;
            }

            MeshRenderer mesh = renderer as MeshRenderer;
            if (mesh != null)
            {
                MeshFilter filter = mesh.GetComponent<MeshFilter>();
                return (filter != null && filter.sharedMesh != null)
                    || mesh.sharedMaterial != null
                    || mesh.material != null;
            }

            SpriteRenderer sprite = renderer as SpriteRenderer;
            if (sprite != null)
            {
                return sprite.sprite != null;
            }

            return renderer.sharedMaterial != null || renderer.material != null;
        }

        private static int ComputeHash(WeaponColorSettings settings)
        {
            int h = settings.Color.GetHashCode();
            h = (h * 397) ^ (settings.TintMesh ? 1 : 0);
            h = (h * 397) ^ (settings.TintParticles ? 1 : 0);
            h = (h * 397) ^ (settings.NeonEnabled ? 1 : 0);
            return h;
        }

        private static void ApplyMeshAndLines(Transform root, Color color, bool neon)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (!IsTintableRenderer(renderer))
                {
                    continue;
                }

                LineRenderer line = renderer as LineRenderer;
                if (line != null)
                {
                    MaterialColorHelper.ApplyLineMaterial(line, color);
                    line.startColor = color;
                    line.endColor = color;
                    continue;
                }

                ApplyRendererColor(renderer, color, neon);
            }
        }

        private static void ApplyRendererColor(Renderer renderer, Color color, bool neon)
        {
            if (!neon)
            {
                MaterialColorHelper.ApplyColor(renderer, color);
                return;
            }

            Material mat = MaterialColorHelper.GetOrCreate(renderer, color);
            MaterialColorHelper.ApplyWeaponNeon(mat, color, 1f);
            renderer.material = mat;
            SpriteRenderer sprite = renderer as SpriteRenderer;
            if (sprite != null)
            {
                sprite.color = color;
            }
        }

        private static void ApplyParticles(Transform root, Color color)
        {
            ParticleSystem[] systems = root.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < systems.Length; i++)
            {
                ParticleSystem ps = systems[i];
                if (ps == null)
                {
                    continue;
                }

                ParticleSystem.MainModule main = ps.main;
                main.startColor = color;

                ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
                if (psr != null && psr.enabled)
                {
                    MaterialColorHelper.ApplyColor(psr, color);
                }
            }
        }
    }
}
