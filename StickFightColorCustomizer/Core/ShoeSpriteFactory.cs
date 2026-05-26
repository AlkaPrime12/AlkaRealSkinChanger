using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Sprites de zapatos en vista lateral: siempre más anchos que altos (eje X = punta del pie).
    /// Pivot cerca de la punta para pegar al extremo del LineRenderer del pie.
    /// </summary>
    public static class ShoeSpriteFactory
    {
        private const int ArtGeneration = 29;
        private const float PixelsPerUnit = 18f;
        private static readonly Vector2 ToePivot = new Vector2(0.82f, 0.38f);

        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static void ClearCache()
        {
            Cache.Clear();
        }

        private static readonly Color Clear = new Color(0f, 0f, 0f, 0f);
        private static readonly Color Outline = new Color(0.05f, 0.04f, 0.06f, 1f);
        private static readonly Color Shadow = new Color(0f, 0f, 0f, 0.35f);

        private static readonly Color BlackHi = new Color(0.2f, 0.2f, 0.24f, 1f);
        private static readonly Color BlackLo = new Color(0.08f, 0.08f, 0.1f, 1f);
        private static readonly Color WhiteHi = new Color(0.96f, 0.96f, 0.96f, 1f);
        private static readonly Color WhiteLo = new Color(0.78f, 0.78f, 0.82f, 1f);
        private static readonly Color RedHi = new Color(0.92f, 0.22f, 0.18f, 1f);
        private static readonly Color RedLo = new Color(0.62f, 0.1f, 0.08f, 1f);
        private static readonly Color BlueHi = new Color(0.3f, 0.55f, 0.95f, 1f);
        private static readonly Color BlueLo = new Color(0.16f, 0.3f, 0.65f, 1f);
        private static readonly Color BrownHi = new Color(0.6f, 0.4f, 0.22f, 1f);
        private static readonly Color BrownLo = new Color(0.32f, 0.2f, 0.1f, 1f);
        private static readonly Color GrayHi = new Color(0.55f, 0.55f, 0.6f, 1f);
        private static readonly Color GrayLo = new Color(0.3f, 0.3f, 0.34f, 1f);
        private static readonly Color GreenHi = new Color(0.45f, 0.78f, 0.4f, 1f);
        private static readonly Color GreenLo = new Color(0.22f, 0.5f, 0.2f, 1f);
        private static readonly Color YellowHi = new Color(1f, 0.92f, 0.32f, 1f);
        private static readonly Color OrangeHi = new Color(1f, 0.6f, 0.18f, 1f);
        private static readonly Color Skin = new Color(0.95f, 0.82f, 0.68f, 1f);
        private static readonly Color Highlight = new Color(1f, 1f, 1f, 0.55f);

        public static Sprite GetSprite(string shoeId)
        {
            string id = ShoeCatalog.Normalize(shoeId);
            if (id == "none")
            {
                return null;
            }

            string cacheKey = id + "#" + ArtGeneration;
            Sprite sprite;
            if (Cache.TryGetValue(cacheKey, out sprite) && sprite != null)
            {
                return sprite;
            }

            sprite = BuildSprite(id);
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Texture2D GetTexture(string shoeId)
        {
            Sprite sprite = GetSprite(shoeId);
            return sprite != null ? sprite.texture : null;
        }

        /// <summary>Color para el LineRenderer de respaldo (sin sprite estirado).</summary>
        public static Color GetLineTint(string shoeId)
        {
            switch (ShoeCatalog.Normalize(shoeId))
            {
                case "boots": return BrownHi;
                case "combat": return BlackHi;
                case "dress": return BrownHi;
                case "sandals": return BrownHi;
                case "cleats": return BlueHi;
                case "slippers": return RedHi;
                case "steel_toe": return YellowHi;
                case "rollers": return BlueHi;
                case "spikes": return BlackHi;
                case "high_top": return BlackHi;
                case "running": return BlueHi;
                case "neon_kicks": return GreenHi;
                case "biker": return BlackLo;
                case "cowboy_b": return BrownHi;
                case "slim_black": return BlackHi;
                case "slim_brown": return BrownHi;
                case "slim_combat": return GreenLo;
                case "slim_ninja": return BlackLo;
                case "slim_formal": return BlackHi;
                case "slim_hiker": return BrownHi;
                case "slim_punk": return RedHi;
                case "slim_arctic": return WhiteHi;
                case "slim_rider": return BrownHi;
                case "slim_chelsea": return BlackHi;
                default: return RedHi;
            }
        }

        public static float GetWidthScale(string shoeId)
        {
            switch (ShoeCatalog.Normalize(shoeId))
            {
                case "boots": return 1.08f;
                case "combat": return 1.1f;
                case "rollers": return 1.12f;
                case "spikes": return 1.1f;
                case "cleats": return 1.05f;
                case "high_top": return 1.05f;
                case "running": return 1.10f;
                case "neon_kicks": return 1.05f;
                case "biker": return 1.06f;
                case "cowboy_b": return 1.04f;
                case "slim_black":
                case "slim_brown":
                case "slim_combat":
                case "slim_ninja":
                case "slim_formal":
                case "slim_hiker":
                case "slim_punk":
                case "slim_arctic":
                case "slim_rider":
                case "slim_chelsea":
                    return 0.98f;
                default: return 1f;
            }
        }

        public static float GetHeightScale(string shoeId)
        {
            switch (ShoeCatalog.Normalize(shoeId))
            {
                case "boots": return 1.05f;
                case "combat": return 1.08f;
                case "steel_toe": return 1.06f;
                case "spikes": return 1.05f;
                case "high_top": return 1.20f;
                case "running": return 1.00f;
                case "neon_kicks": return 1.00f;
                case "biker": return 1.18f;
                case "cowboy_b": return 1.14f;
                case "slim_black":
                case "slim_brown":
                case "slim_combat":
                case "slim_ninja":
                case "slim_formal":
                case "slim_hiker":
                case "slim_punk":
                case "slim_arctic":
                case "slim_rider":
                case "slim_chelsea":
                    return 1.02f;
                default: return 1f;
            }
        }

        private static Sprite BuildSprite(string id)
        {
            switch (id)
            {
                case "sneakers": return BuildSneakers();
                case "boots": return BuildBoots();
                case "combat": return BuildCombat();
                case "dress": return BuildDress();
                case "sandals": return BuildSandals();
                case "cleats": return BuildCleats();
                case "slippers": return BuildSlippers();
                case "steel_toe": return BuildSteelToe();
                case "rollers": return BuildRollers();
                case "spikes": return BuildSpikes();
                case "high_top":   return BuildHighTop();
                case "running":    return BuildRunning();
                case "neon_kicks": return BuildNeonKicks();
                case "biker":      return BuildBiker();
                case "cowboy_b":   return BuildCowboyBoot();
                case "slim_black":   return BuildSlimBlack();
                case "slim_brown":   return BuildSlimBrown();
                case "slim_combat":  return BuildSlimCombat();
                case "slim_ninja":   return BuildSlimNinja();
                case "slim_formal":  return BuildSlimFormal();
                case "slim_hiker":   return BuildSlimHiker();
                case "slim_punk":    return BuildSlimPunk();
                case "slim_arctic":  return BuildSlimArctic();
                case "slim_rider":   return BuildSlimRider();
                case "slim_chelsea": return BuildSlimChelsea();
                default: return BuildSneakers();
            }
        }

        // ── Slim line: mismo tamaño que sneakers (no botas altas/estrechas) ─────

        private static Sprite BuildCompactShoe(Color upperHi, Color upperLo, Color soleHi, bool stripe)
        {
            const int w = 28, h = 11;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            FillRect(tex, 1, 1, w - 2, 3, soleHi);
            FillRect(tex, 2, 3, w - 3, 8, upperHi);
            FillRect(tex, 2, 3, w - 3, 4, upperLo);
            FillEllipse(tex, w - 5, 5f, 3.5f, 2.5f, upperHi);
            if (stripe)
            {
                for (int x = 6; x < w - 7; x += 3)
                {
                    tex.SetPixel(x, 5, Highlight);
                }
            }

            DrawOutline(tex, 2, 1, w - 3, 8);
            return Finish(tex, w, h);
        }

        private static Sprite BuildSlimBlack() { return BuildCompactShoe(BlackHi, BlackLo, GrayLo, false); }
        private static Sprite BuildSlimBrown() { return BuildCompactShoe(BrownHi, BrownLo, BrownHi, true); }
        private static Sprite BuildSlimCombat()
        {
            Color oliveHi = new Color(0.42f, 0.48f, 0.28f, 1f);
            Color oliveLo = new Color(0.28f, 0.32f, 0.18f, 1f);
            return BuildCompactShoe(oliveHi, oliveLo, BlackHi, true);
        }
        private static Sprite BuildSlimNinja() { return BuildCompactShoe(BlackLo, BlackHi, GrayLo, false); }
        private static Sprite BuildSlimFormal() { return BuildCompactShoe(BlackHi, BlackLo, Highlight, false); }
        private static Sprite BuildSlimHiker() { return BuildCompactShoe(BrownHi, BrownLo, GrayLo, true); }
        private static Sprite BuildSlimPunk() { return BuildCompactShoe(RedHi, RedLo, BlackLo, true); }
        private static Sprite BuildSlimArctic()
        {
            Color iceHi = new Color(0.88f, 0.94f, 1f, 1f);
            Color iceLo = new Color(0.55f, 0.72f, 0.92f, 1f);
            return BuildCompactShoe(iceHi, iceLo, WhiteHi, false);
        }
        private static Sprite BuildSlimRider() { return BuildCompactShoe(BrownHi, BrownLo, BlackLo, true); }
        private static Sprite BuildSlimChelsea() { return BuildCompactShoe(BlackHi, BlackLo, GrayHi, false); }

        // ── 5 new shoe builders ────────────────────────────────────────────────────

        /// <summary>High-top sneaker: tall canvas upper + chunky white sole.</summary>
        private static Sprite BuildHighTop()
        {
            const int w = 32, h = 16;
            Texture2D tex = NewTex(w, h);
            // Shadow
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            // White sole (chunky)
            FillRect(tex, 1, 1, w - 2, 4, WhiteHi);
            FillRect(tex, 1, 1, w - 2, 2, WhiteLo);
            // Canvas upper — black with red stripe
            FillRect(tex, 3, 4, w - 4, 12, BlackHi);
            FillRect(tex, 3, 4, w - 4, 6, BlackLo);
            FillRect(tex, 3, 8, w - 4, 9, RedHi);
            // Ankle collar (taller back)
            FillRect(tex, 3, 12, 12, 14, BlackHi);
            FillRect(tex, 3, 14, 9, 15, WhiteHi);
            // Round toe cap
            FillEllipse(tex, w - 5, 6.5f, 4f, 3.5f, WhiteHi);
            // Laces
            for (int x = 10; x < w - 9; x += 3)
            {
                tex.SetPixel(x, 9, WhiteHi);
                tex.SetPixel(x, 11, WhiteHi);
            }
            DrawOutline(tex, 3, 1, w - 4, 14);
            return Finish(tex, w, h);
        }

        /// <summary>Running shoe: mesh body, swoosh stripe, sculpted sole.</summary>
        private static Sprite BuildRunning()
        {
            const int w = 34, h = 12;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            // Two-tone sole (darker base, white midsole)
            FillRect(tex, 1, 1, w - 2, 2, GrayLo);
            FillRect(tex, 1, 2, w - 2, 4, WhiteHi);
            FillRect(tex, 1, 4, w - 2, 4, WhiteLo);
            // Upper — bright cyan/blue mesh
            FillRect(tex, 3, 4, w - 4, 9, BlueHi);
            FillRect(tex, 3, 4, w - 4, 5, BlueLo);
            // Swoosh / accent stripe
            for (int t = 0; t < 18; t++)
            {
                int x = 8 + t, y = 7 - t / 6;
                if (x < w - 4 && y >= 4) tex.SetPixel(x, y, WhiteHi);
            }
            // Heel pull tab
            FillRect(tex, 3, 9, 5, 11, BlueLo);
            // Toe
            FillEllipse(tex, w - 5, 6f, 4.5f, 3f, BlueHi);
            // Laces
            for (int x = 12; x < w - 8; x += 3)
            {
                tex.SetPixel(x, 8, WhiteHi);
            }
            DrawOutline(tex, 2, 1, w - 3, 9);
            return Finish(tex, w, h);
        }

        /// <summary>Neon kicks: black body with bright neon highlights and glow stripe.</summary>
        private static Sprite BuildNeonKicks()
        {
            const int w = 32, h = 12;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            // Neon green outsole
            FillRect(tex, 1, 1, w - 2, 3, GreenLo);
            FillRect(tex, 1, 2, w - 2, 3, GreenHi);
            // Black upper
            FillRect(tex, 2, 3, w - 3, 9, BlackHi);
            FillRect(tex, 2, 3, w - 3, 4, BlackLo);
            // Bright magenta/cyan accent stripe along the side
            for (int x = 4; x < w - 4; x++)
            {
                if ((x % 4) < 2) tex.SetPixel(x, 6, new Color(0.30f, 1f, 1f, 1f));
                else             tex.SetPixel(x, 6, new Color(1f, 0.30f, 0.95f, 1f));
            }
            // Toe cap with bright outline
            FillEllipse(tex, w - 5, 5.5f, 4f, 3f, BlackHi);
            for (int t = 0; t < 8; t++)
            {
                int x = w - 4 + (int)(Mathf.Cos(t * 0.5f) * 2f);
                int y = 5 + (int)(Mathf.Sin(t * 0.5f) * 2f);
                if (x >= 0 && y >= 0 && x < w && y < h) tex.SetPixel(x, y, GreenHi);
            }
            DrawOutline(tex, 2, 1, w - 3, 9);
            return Finish(tex, w, h);
        }

        /// <summary>Biker boots: tall, heavy, leather with buckle.</summary>
        private static Sprite BuildBiker()
        {
            const int w = 28, h = 16;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            // Thick rubber sole
            FillRect(tex, 1, 1, w - 2, 3, BlackLo);
            FillRect(tex, 1, 1, w - 2, 2, BlackHi);
            // Leather upper (very dark)
            FillRect(tex, 2, 3, w - 3, 14, BlackHi);
            FillRect(tex, 2, 3, w - 3, 5, BlackLo);
            // Reflective stripes
            FillRect(tex, 4, 8, w - 5, 9, GrayHi);
            FillRect(tex, 4, 12, w - 5, 13, GrayHi);
            // Big buckle on top
            FillRect(tex, 7, 13, 16, 15, GrayLo);
            FillRect(tex, 9, 14, 14, 15, YellowHi);
            FillRect(tex, 11, 14, 12, 15, BlackLo);
            // Toe scuff
            FillEllipse(tex, w - 5, 5.5f, 3f, 2.5f, GrayLo);
            DrawOutline(tex, 2, 1, w - 3, 14);
            return Finish(tex, w, h);
        }

        /// <summary>Cowboy boots: pointed toe, stacked heel, decorative stitching.</summary>
        private static Sprite BuildCowboyBoot()
        {
            const int w = 30, h = 15;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            // Stacked heel (back)
            FillRect(tex, 2, 1, 6, 4, BrownLo);
            FillRect(tex, 2, 1, 6, 2, BlackLo);
            // Outsole running to the pointed toe
            for (int x = 6; x < w - 1; x++)
            {
                int sy = 1;
                tex.SetPixel(x, sy, BrownLo);
                tex.SetPixel(x, sy + 1, BrownHi);
            }
            // Upper shaft — tall, brown leather
            FillRect(tex, 3, 3, w - 4, 13, BrownHi);
            FillRect(tex, 3, 3, w - 4, 5, BrownLo);
            // Pointed toe
            for (int t = 0; t < 4; t++)
            {
                int x = w - 4 + t, y0 = 3 + t, y1 = 8 - t;
                if (x < w) for (int y = y0; y <= y1; y++) tex.SetPixel(x, y, BrownHi);
            }
            // Decorative stitching pattern
            for (int x = 5; x < w - 6; x += 3) tex.SetPixel(x, 9, YellowHi);
            for (int x = 6; x < w - 6; x += 3) tex.SetPixel(x, 11, YellowHi);
            // Spur (small star on heel back)
            tex.SetPixel(3, 5, YellowHi);
            tex.SetPixel(2, 6, YellowHi);
            tex.SetPixel(3, 7, YellowHi);
            DrawOutline(tex, 2, 1, w - 3, 13);
            return Finish(tex, w, h);
        }

        // Canvas estándar: ancho (X) = punta del pie hacia la derecha del sprite.

        private static Sprite BuildSneakers()
        {
            const int w = 32, h = 12;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            FillRect(tex, 1, 1, w - 2, 3, WhiteHi);
            FillRect(tex, 1, 1, w - 2, 2, WhiteLo);
            FillRect(tex, 2, 3, w - 3, 8, RedHi);
            FillRect(tex, 2, 3, w - 3, 4, RedLo);
            FillEllipse(tex, w - 5, 5.5f, 4f, 3f, RedHi);
            for (int x = 7; x < w - 8; x += 3)
            {
                tex.SetPixel(x, 6, WhiteHi);
            }
            DrawOutline(tex, 2, 1, w - 3, 8);
            return Finish(tex, w, h);
        }

        private static Sprite BuildBoots()
        {
            const int w = 30, h = 14;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            FillRect(tex, 1, 1, w - 2, 3, BlackHi);
            FillRect(tex, 3, 3, w - 4, 10, BrownHi);
            FillRect(tex, 3, 3, w - 4, 4, BrownLo);
            FillRect(tex, w - 6, 3, w - 4, 10, BrownLo);
            FillRect(tex, 4, 9, w - 5, 10, BrownLo);
            FillEllipse(tex, w - 5, 5f, 3.5f, 2.5f, BrownHi);
            FillRect(tex, 6, 7, 11, 8, YellowHi);
            DrawOutline(tex, 3, 1, w - 4, 10);
            return Finish(tex, w, h);
        }

        private static Sprite BuildCombat()
        {
            const int w = 32, h = 14;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            FillRect(tex, 1, 1, w - 2, 4, BlackLo);
            FillRect(tex, 3, 3, w - 4, 10, BlackHi);
            FillRect(tex, 3, 3, w - 4, 4, BlackLo);
            FillRect(tex, 4, 5, w - 5, 6, GreenLo);
            FillRect(tex, 4, 7, w - 5, 8, GreenHi);
            for (int x = 5; x < w - 6; x += 4)
            {
                FillRect(tex, x, 6, x + 1, 7, GrayHi);
            }
            FillEllipse(tex, w - 5, 5f, 3.5f, 2.5f, GrayHi);
            FillEllipse(tex, w - 5, 5.5f, 2f, 1.2f, WhiteLo);
            DrawOutline(tex, 3, 1, w - 4, 10);
            return Finish(tex, w, h);
        }

        private static Sprite BuildDress()
        {
            const int w = 30, h = 11;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 0, Shadow);
            FillRect(tex, 1, 1, w - 2, 1, BlackLo);
            FillRect(tex, 2, 2, w - 3, 7, BrownLo);
            FillRect(tex, 2, 3, w - 3, 7, BrownHi);
            for (int x = 6; x < w - 7; x++)
            {
                tex.SetPixel(x, 6, Highlight);
            }
            FillEllipse(tex, w - 4, 4f, 3f, 2f, BrownHi);
            DrawOutline(tex, 2, 1, w - 3, 7);
            return Finish(tex, w, h);
        }

        private static Sprite BuildSandals()
        {
            const int w = 28, h = 10;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 0, Shadow);
            FillRect(tex, 1, 1, w - 2, 3, BrownHi);
            FillRect(tex, 3, 3, w - 4, 6, Skin);
            FillRect(tex, 5, 4, w - 6, 5, BrownLo);
            FillRect(tex, 9, 3, 11, 6, BrownLo);
            DrawOutline(tex, 2, 1, w - 3, 6);
            return Finish(tex, w, h);
        }

        private static Sprite BuildCleats()
        {
            const int w = 32, h = 12;
            Texture2D tex = NewTex(w, h);
            for (int x = 3; x < w - 3; x += 3)
            {
                FillRect(tex, x, 0, x + 1, 1, BlackHi);
            }
            FillRect(tex, 1, 2, w - 2, 3, WhiteHi);
            FillRect(tex, 2, 3, w - 3, 8, BlueHi);
            FillRect(tex, 2, 3, w - 3, 4, BlueLo);
            FillEllipse(tex, w - 5, 5.5f, 4f, 3f, BlueHi);
            FillRect(tex, 5, 5, w - 6, 6, WhiteHi);
            DrawOutline(tex, 2, 2, w - 3, 8);
            return Finish(tex, w, h);
        }

        private static Sprite BuildSlippers()
        {
            const int w = 30, h = 11;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 0, Shadow);
            FillRect(tex, 2, 1, w - 3, 1, BrownLo);
            FillEllipse(tex, w / 2f, 5f, 10f, 4f, RedHi);
            FillRect(tex, 3, 3, w - 4, 7, RedHi);
            FillRect(tex, 3, 6, w - 4, 7, RedLo);
            for (int x = 5; x < w - 5; x += 2)
            {
                tex.SetPixel(x, 5, new Color(1f, 0.6f, 0.6f, 1f));
            }
            DrawOutline(tex, 2, 1, w - 3, 7);
            return Finish(tex, w, h);
        }

        private static Sprite BuildSteelToe()
        {
            const int w = 32, h = 13;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            FillRect(tex, 1, 1, w - 2, 3, BlackLo);
            FillRect(tex, 3, 3, w - 4, 9, YellowHi);
            FillRect(tex, 3, 3, w - 4, 4, OrangeHi);
            FillEllipse(tex, w - 5, 5f, 4f, 3f, GrayHi);
            FillEllipse(tex, w - 5, 5.5f, 2.5f, 1.5f, WhiteHi);
            for (int x = 6; x < w - 8; x += 3)
            {
                FillRect(tex, x, 6, x + 1, 7, BlackLo);
            }
            DrawOutline(tex, 3, 1, w - 4, 9);
            return Finish(tex, w, h);
        }

        private static Sprite BuildRollers()
        {
            const int w = 34, h = 13;
            Texture2D tex = NewTex(w, h);
            int[] wheelX = { 5, 11, 17, 23, 28 };
            for (int i = 0; i < wheelX.Length; i++)
            {
                FillEllipse(tex, wheelX[i], 2f, 2.2f, 2.2f, BlackHi);
                FillEllipse(tex, wheelX[i], 2f, 1f, 1f, GrayHi);
            }
            FillRect(tex, 2, 4, w - 3, 5, GrayLo);
            FillRect(tex, 3, 5, w - 4, 9, BlueHi);
            FillRect(tex, 3, 5, w - 4, 6, BlueLo);
            FillRect(tex, 6, 7, w - 7, 8, WhiteHi);
            DrawOutline(tex, 3, 5, w - 4, 9);
            return Finish(tex, w, h);
        }

        private static Sprite BuildSpikes()
        {
            const int w = 32, h = 13;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 2, 0, w - 3, 1, Shadow);
            FillRect(tex, 1, 1, w - 2, 3, BlackLo);
            FillRect(tex, 3, 3, w - 4, 8, BlackHi);
            FillRect(tex, 3, 3, w - 4, 4, BlackLo);
            int[] spikeX = { 7, 12, 17, 22 };
            for (int i = 0; i < spikeX.Length; i++)
            {
                FillTriangleUp(tex, spikeX[i], 8, 1, 3, GrayHi);
                tex.SetPixel(spikeX[i], 9, WhiteHi);
            }
            FillRect(tex, 6, 4, 10, 6, RedHi);
            FillRect(tex, 7, 5, 9, 5, RedLo);
            DrawOutline(tex, 3, 1, w - 4, 8);
            return Finish(tex, w, h);
        }

        private static Texture2D NewTex(int w, int h)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    tex.SetPixel(x, y, Clear);
                }
            }

            return tex;
        }

        private static Sprite Finish(Texture2D tex, int w, int h)
        {
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            return Sprite.Create(tex, new Rect(0f, 0f, w, h), ToePivot, PixelsPerUnit);
        }

        private static void FillRect(Texture2D tex, int x0, int y0, int x1, int y1, Color color)
        {
            int xMin = Mathf.Min(x0, x1);
            int xMax = Mathf.Max(x0, x1);
            int yMin = Mathf.Min(y0, y1);
            int yMax = Mathf.Max(y0, y1);
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    if (x >= 0 && y >= 0 && x < tex.width && y < tex.height)
                    {
                        tex.SetPixel(x, y, color);
                    }
                }
            }
        }

        private static void FillEllipse(Texture2D tex, float cx, float cy, float rx, float ry, Color color)
        {
            int xMin = Mathf.FloorToInt(cx - rx);
            int xMax = Mathf.CeilToInt(cx + rx);
            int yMin = Mathf.FloorToInt(cy - ry);
            int yMax = Mathf.CeilToInt(cy + ry);
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    if (x < 0 || y < 0 || x >= tex.width || y >= tex.height)
                    {
                        continue;
                    }

                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    if (dx * dx + dy * dy <= 1f)
                    {
                        tex.SetPixel(x, y, color);
                    }
                }
            }
        }

        private static void FillTriangleUp(Texture2D tex, int cx, int yBase, int halfW, int height, Color color)
        {
            for (int y = 0; y < height; y++)
            {
                float t = y / (float)height;
                int half = Mathf.Max(0, Mathf.RoundToInt(halfW * (1f - t)));
                FillRect(tex, cx - half, yBase + y, cx + half, yBase + y, color);
            }
        }

        private static void DrawOutline(Texture2D tex, int x0, int y0, int x1, int y1)
        {
            int xMin = Mathf.Min(x0, x1);
            int xMax = Mathf.Max(x0, x1);
            int yMin = Mathf.Min(y0, y1);
            int yMax = Mathf.Max(y0, y1);
            for (int x = xMin; x <= xMax; x++)
            {
                SetIfInside(tex, x, yMin - 1, Outline);
                SetIfInside(tex, x, yMax + 1, Outline);
            }

            for (int y = yMin; y <= yMax; y++)
            {
                SetIfInside(tex, xMin - 1, y, Outline);
                SetIfInside(tex, xMax + 1, y, Outline);
            }
        }

        private static void SetIfInside(Texture2D tex, int x, int y, Color color)
        {
            if (x >= 0 && y >= 0 && x < tex.width && y < tex.height)
            {
                Color current = tex.GetPixel(x, y);
                if (current.a < 0.05f)
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }
    }
}
