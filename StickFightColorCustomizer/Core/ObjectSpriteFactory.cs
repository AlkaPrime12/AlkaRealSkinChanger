using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Orbes y anillos simples para levitar alrededor del personaje.
    /// </summary>
    public static class ObjectSpriteFactory
    {
        private const int ArtGeneration = 5;
        private const float PixelsPerUnit = 32f;
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static void ClearCache()
        {
            Cache.Clear();
        }

        public static Sprite GetPartSprite(string objectId, int partIndex)
        {
            string id = ObjectsCatalog.Normalize(objectId);
            if (id == "none")
            {
                return null;
            }

            ObjectsCatalogEntry entry;
            if (!ObjectsCatalog.TryGet(id, out entry))
            {
                return null;
            }

            string partKey = entry.SpriteKey + "#" + partIndex + "#" + ArtGeneration;
            Sprite sprite;
            if (Cache.TryGetValue(partKey, out sprite) && sprite != null)
            {
                return sprite;
            }

            sprite = BuildSprite(entry.SpriteKey);
            Cache[partKey] = sprite;
            return sprite;
        }

        public static Sprite GetPreviewSprite(string objectId)
        {
            return GetPartSprite(objectId, 0);
        }

        private static Sprite BuildSprite(string spriteKey)
        {
            switch (spriteKey)
            {
                case "orb_white":
                    return BuildOrb(new Color(0.92f, 0.94f, 1f, 1f), new Color(1f, 1f, 1f, 1f));
                case "orb_cyan":
                    return BuildOrb(new Color(0.2f, 0.75f, 0.95f, 1f), new Color(0.55f, 0.95f, 1f, 1f));
                case "orb_gold":
                    return BuildOrb(new Color(0.95f, 0.78f, 0.15f, 1f), new Color(1f, 0.95f, 0.45f, 1f));
                case "orb_purple":
                    return BuildOrb(new Color(0.45f, 0.15f, 0.75f, 1f), new Color(0.75f, 0.4f, 1f, 1f));
                case "orb_green":
                    return BuildOrb(new Color(0.12f, 0.78f, 0.22f, 1f), new Color(0.45f, 1f, 0.35f, 1f));
                case "orb_red":
                    return BuildOrb(new Color(0.85f, 0.10f, 0.08f, 1f), new Color(1f, 0.40f, 0.25f, 1f));
                case "ring_red":
                    return BuildRing(new Color(0.9f, 0.15f, 0.12f, 1f), new Color(1f, 0.5f, 0.35f, 1f));
                case "ring_gold":
                    return BuildRing(new Color(0.95f, 0.78f, 0.2f, 1f), new Color(1f, 0.95f, 0.55f, 1f));
                case "ring_cyan":
                    return BuildRing(new Color(0.1f, 0.70f, 0.95f, 1f), new Color(0.4f, 1f, 1f, 1f));
                case "gem_blue":
                    return BuildGem(new Color(0.08f, 0.30f, 0.90f, 1f), new Color(0.35f, 0.65f, 1f, 1f), new Color(0.7f, 0.88f, 1f, 1f));
                case "gem_ruby":
                    return BuildGem(new Color(0.75f, 0.04f, 0.04f, 1f), new Color(1f, 0.22f, 0.18f, 1f), new Color(1f, 0.65f, 0.55f, 1f));
                case "gem_emerald":
                    return BuildGem(new Color(0.04f, 0.50f, 0.12f, 1f), new Color(0.10f, 0.88f, 0.28f, 1f), new Color(0.55f, 1f, 0.60f, 1f));
                case "gem_amethyst":
                    return BuildGem(new Color(0.42f, 0.08f, 0.70f, 1f), new Color(0.70f, 0.28f, 1f, 1f), new Color(0.88f, 0.70f, 1f, 1f));
                case "star_gold":
                    return BuildStar(new Color(0.95f, 0.72f, 0.05f, 1f), new Color(1f, 0.96f, 0.40f, 1f));
                case "star_white":
                    return BuildStar(new Color(0.80f, 0.85f, 1f, 1f), new Color(1f, 1f, 1f, 1f));
                case "plasma_blue":
                    return BuildPlasma(new Color(0.05f, 0.25f, 1f, 1f), new Color(0.40f, 0.80f, 1f, 1f), new Color(0.85f, 0.96f, 1f, 1f));
                case "plasma_purple":
                    return BuildPlasma(new Color(0.50f, 0.05f, 0.90f, 1f), new Color(0.80f, 0.35f, 1f, 1f), new Color(0.96f, 0.80f, 1f, 1f));
                case "knife_steel":
                    return BuildKnife(new Color(0.85f, 0.86f, 0.92f, 1f), new Color(0.40f, 0.42f, 0.48f, 1f), new Color(0.45f, 0.28f, 0.10f, 1f));
                case "knife_gold":
                    return BuildKnife(new Color(1f, 0.92f, 0.40f, 1f), new Color(0.65f, 0.45f, 0.05f, 1f), new Color(0.20f, 0.12f, 0.05f, 1f));
                case "shuriken_black":
                    return BuildShuriken(new Color(0.18f, 0.18f, 0.22f, 1f), new Color(0.55f, 0.55f, 0.62f, 1f));
                case "shuriken_red":
                    return BuildShuriken(new Color(0.85f, 0.10f, 0.10f, 1f), new Color(1f, 0.50f, 0.30f, 1f));
                case "sword_steel":
                    return BuildSword(new Color(0.85f, 0.85f, 0.95f, 1f), new Color(0.40f, 0.40f, 0.50f, 1f), new Color(0.65f, 0.45f, 0.20f, 1f));
                case "sword_fire":
                    return BuildSword(new Color(1f, 0.60f, 0.18f, 1f), new Color(0.85f, 0.10f, 0.05f, 1f), new Color(0.25f, 0.10f, 0.04f, 1f));
                case "kanji_red":
                    return BuildKanji(new Color(0.92f, 0.10f, 0.10f, 1f), new Color(0.45f, 0.04f, 0.04f, 1f));
                case "kanji_gold":
                    return BuildKanji(new Color(1f, 0.85f, 0.20f, 1f), new Color(0.45f, 0.30f, 0.05f, 1f));
                case "kanji_black":
                    return BuildKanji(new Color(0.10f, 0.10f, 0.12f, 1f), new Color(0.45f, 0.45f, 0.50f, 1f));
                case "skull_white":
                    return BuildSkull(new Color(0.95f, 0.95f, 0.92f, 1f), new Color(0.55f, 0.55f, 0.55f, 1f), new Color(0.10f, 0.10f, 0.10f, 1f));
                case "skull_black":
                    return BuildSkull(new Color(0.18f, 0.18f, 0.22f, 1f), new Color(0.55f, 0.05f, 0.05f, 1f), new Color(0.95f, 0.10f, 0.05f, 1f));
                case "heart_red":
                    return BuildHeart(new Color(1f, 0.20f, 0.30f, 1f), new Color(0.55f, 0.04f, 0.10f, 1f));
                case "heart_dark":
                    return BuildHeart(new Color(0.40f, 0.04f, 0.10f, 1f), new Color(0.85f, 0.10f, 0.20f, 1f));
                case "bolt_yellow":
                    return BuildLightning(new Color(1f, 0.95f, 0.30f, 1f), new Color(1f, 1f, 0.85f, 1f));
                case "bolt_cyan":
                    return BuildLightning(new Color(0.25f, 0.90f, 1f, 1f), new Color(0.85f, 1f, 1f, 1f));
                case "snowflake":
                    return BuildSnowflake(new Color(0.85f, 0.95f, 1f, 1f), new Color(0.40f, 0.65f, 0.95f, 1f));
                case "leaf_green":
                    return BuildLeaf(new Color(0.30f, 0.80f, 0.20f, 1f), new Color(0.08f, 0.40f, 0.10f, 1f));
                case "leaf_autumn":
                    return BuildLeaf(new Color(1f, 0.50f, 0.10f, 1f), new Color(0.55f, 0.18f, 0.04f, 1f));
                case "yinyang":
                    return BuildYinYang();
                case "cross_gold":
                    return BuildCross(new Color(1f, 0.85f, 0.20f, 1f), new Color(0.55f, 0.35f, 0.05f, 1f));
                case "moon_silver":
                    return BuildMoon(new Color(0.95f, 0.95f, 0.98f, 1f), new Color(0.55f, 0.60f, 0.70f, 1f));
                case "cross_inv_black":
                    return BuildCrossInverted(new Color(0.12f, 0.12f, 0.14f, 1f), new Color(0.55f, 0.55f, 0.60f, 1f));
                case "cross_inv_red":
                    return BuildCrossInverted(new Color(0.88f, 0.08f, 0.08f, 1f), new Color(0.45f, 0.02f, 0.02f, 1f));
                case "cross_inv_white":
                    return BuildCrossInverted(new Color(0.95f, 0.95f, 0.98f, 1f), new Color(0.70f, 0.72f, 0.78f, 1f));
                case "letter_x":
                    return BuildLetterGlyph('X', new Color(0.95f, 0.35f, 1f, 1f), new Color(0.35f, 0.05f, 0.55f, 1f));
                case "letter_o":
                    return BuildLetterGlyph('O', new Color(1f, 0.85f, 0.25f, 1f), new Color(0.55f, 0.35f, 0.05f, 1f));
                case "letter_z":
                    return BuildLetterGlyph('Z', new Color(0.35f, 0.95f, 1f, 1f), new Color(0.05f, 0.35f, 0.55f, 1f));
                case "han_red":
                    return BuildHanBrush(new Color(0.92f, 0.12f, 0.10f, 1f), new Color(0.40f, 0.04f, 0.04f, 1f), 0);
                case "han_gold":
                    return BuildHanBrush(new Color(1f, 0.88f, 0.22f, 1f), new Color(0.45f, 0.30f, 0.05f, 1f), 1);
                case "han_cyan":
                    return BuildHanBrush(new Color(0.25f, 0.92f, 1f, 1f), new Color(0.05f, 0.35f, 0.50f, 1f), 2);
                case "han_void":
                    return BuildHanBrush(new Color(0.08f, 0.08f, 0.10f, 1f), new Color(0.75f, 0.15f, 0.90f, 1f), 3);
                case "infinity":
                    return BuildInfinity(new Color(0.85f, 0.45f, 1f, 1f), new Color(0.35f, 0.15f, 0.65f, 1f));
                case "omega":
                    return BuildOmega(new Color(1f, 0.92f, 0.35f, 1f), new Color(0.55f, 0.40f, 0.08f, 1f));
                case "pentagram":
                    return BuildPentagram(new Color(0.90f, 0.12f, 0.12f, 1f), new Color(0.55f, 0.05f, 0.05f, 1f));
                case "wifi":
                    return BuildWifi(new Color(0.30f, 0.95f, 1f, 1f), new Color(0.10f, 0.45f, 0.75f, 1f));
                case "hashtag":
                    return BuildHashtag(new Color(1f, 0.55f, 0.20f, 1f), new Color(0.55f, 0.22f, 0.05f, 1f));
                case "smile_meme":
                    return BuildSmileMeme();
                case "meme_eyes":
                    return BuildMemeEyes();
                case "rune":
                    return BuildRune(new Color(0.92f, 0.88f, 0.75f, 1f), new Color(0.45f, 0.38f, 0.28f, 1f));
                case "flame_teardrop":
                    return BuildFlameTeardrop();
                case "mini_wing":
                    return BuildMiniWing(new Color(0.92f, 0.94f, 1f, 1f), new Color(0.55f, 0.65f, 0.95f, 1f));
                // ── 20 nuevos sprites ──
                case "obj_axe":      return BuildAxe();
                case "obj_arrow":    return BuildArrow();
                case "obj_dna":      return BuildDna();
                case "obj_atom":     return BuildAtom();
                case "obj_flame":    return BuildFlame();
                case "obj_iceshard": return BuildIceShard();
                case "obj_drop":     return BuildDrop();
                case "obj_clock":    return BuildClockSprite();
                case "obj_eye":      return BuildEyeSprite();
                case "obj_chain":    return BuildChainLink();
                case "obj_coin":     return BuildCoinSprite();
                case "obj_dice":     return BuildDiceSprite();
                case "obj_card":     return BuildCardSprite();
                case "obj_bomb":     return BuildBombSprite();
                case "obj_potion":   return BuildPotionSprite();
                case "obj_pumpkin":  return BuildPumpkinSprite();
                case "obj_anchor":   return BuildAnchorSprite();
                case "obj_horse":    return BuildHorseshoe();
                case "obj_paw":      return BuildPawSprite();
                case "obj_galaxy":   return BuildGalaxySprite();
                // ── 20 MORE new sprites (compact) ──
                case "obj_gear":     return BuildGear(new Color(0.55f, 0.40f, 0.18f, 1f), new Color(0.85f, 0.65f, 0.30f, 1f));
                case "obj_crystal":  return BuildCrystal(new Color(0.30f, 0.85f, 1f, 1f), new Color(0.10f, 0.40f, 0.78f, 1f));
                case "obj_note":     return BuildMusicNote(new Color(0.18f, 0.18f, 0.22f, 1f));
                case "obj_planet":   return BuildPlanet(new Color(0.40f, 0.65f, 0.95f, 1f), new Color(0.18f, 0.30f, 0.65f, 1f));
                case "obj_hex":      return BuildHexagon(new Color(1f, 0.78f, 0.18f, 1f), new Color(0.78f, 0.45f, 0.05f, 1f));
                case "obj_triangle": return BuildTriangleNeon(new Color(0.30f, 1f, 1f, 1f));
                case "obj_smoke":    return BuildSmokePuff(new Color(0.70f, 0.70f, 0.78f, 1f));
                case "obj_bubble":   return BuildBubble(new Color(0.55f, 0.85f, 1f, 1f));
                case "obj_rose":     return BuildRose(new Color(0.92f, 0.20f, 0.28f, 1f), new Color(0.55f, 0.06f, 0.10f, 1f));
                case "obj_sun":      return BuildSun(new Color(1f, 0.85f, 0.18f, 1f), new Color(1f, 0.60f, 0.10f, 1f));
                case "obj_key":      return BuildKey(new Color(1f, 0.85f, 0.18f, 1f), new Color(0.55f, 0.32f, 0.05f, 1f));
                case "obj_lock":     return BuildLock(new Color(0.55f, 0.55f, 0.60f, 1f), new Color(1f, 0.85f, 0.18f, 1f));
                case "obj_compass":  return BuildCompass(new Color(0.85f, 0.78f, 0.55f, 1f), new Color(0.30f, 0.20f, 0.10f, 1f));
                case "obj_fish":     return BuildFish(new Color(1f, 0.55f, 0.18f, 1f), new Color(0.55f, 0.10f, 0.05f, 1f));
                case "obj_bat":      return BuildBat(new Color(0.18f, 0.18f, 0.22f, 1f));
                case "obj_ghost":    return BuildGhost(new Color(0.95f, 0.95f, 1f, 1f));
                case "obj_meteor":   return BuildMeteor(new Color(0.55f, 0.32f, 0.10f, 1f), new Color(1f, 0.55f, 0.10f, 1f));
                case "obj_pac":      return BuildPacPellet(new Color(1f, 0.85f, 0.50f, 1f));
                case "obj_pad":      return BuildGamePad(new Color(0.20f, 0.20f, 0.25f, 1f), new Color(0.85f, 0.10f, 0.10f, 1f));
                case "obj_pizza":    return BuildPizzaSlice();
                default:
                    return BuildOrb(new Color(0.06f, 0.06f, 0.08f, 1f), new Color(0.35f, 0.35f, 0.42f, 1f));
            }
        }

        // ── 20 MORE compact sprite builders ──────────────────────────────────────
        private static Sprite BuildGear(Color body, Color teeth)
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 20, body);
            FillCircleArea(tex, 32, 32, 8, new Color(0,0,0,0));
            FillCircleArea(tex, 32, 32, 6, Color.black);
            // 8 teeth
            for (int i = 0; i < 8; i++)
            {
                float ang = i * Mathf.PI / 4f;
                int cx = 32 + (int)(Mathf.Cos(ang) * 23);
                int cy = 32 + (int)(Mathf.Sin(ang) * 23);
                FillCircleArea(tex, cx, cy, 4, teeth);
            }
            return Finish(tex);
        }

        private static Sprite BuildCrystal(Color hi, Color lo)
        {
            const int size = 64; var tex = NewTexture(size);
            // Hex-cut crystal pointing up
            int[] xs = { 32, 46, 46, 32, 18, 18 };
            int[] ys = { 8, 20, 44, 56, 44, 20 };
            for (int y = 8; y <= 56; y++)
            {
                float t = (y - 8f) / 48f;
                float halfWidth = Mathf.Lerp(0f, 14f, Mathf.Sin(t * Mathf.PI));
                for (int x = 32 - (int)halfWidth; x <= 32 + (int)halfWidth; x++)
                {
                    if (x < 0 || x >= size) continue;
                    float u = (x - 32f) / Mathf.Max(halfWidth, 1f);
                    Color c = u < -0.2f ? lo : (u > 0.4f ? Color.Lerp(lo, hi, 0.4f) : hi);
                    tex.SetPixel(x, y, c);
                }
            }
            // Sparkle
            tex.SetPixel(28, 26, Color.white);
            tex.SetPixel(29, 26, Color.white);
            return Finish(tex);
        }

        private static Sprite BuildMusicNote(Color c)
        {
            const int size = 64; var tex = NewTexture(size);
            // Stem
            FillSquare(tex, 36, 16, 39, 52, c);
            // Flag
            for (int y = 40; y < 52; y++) for (int x = 39; x < 50; x++)
            {
                if ((y - 40) > (x - 39) * 1.2f) continue;
                if ((50 - x) > (y - 40) * 0.5f) continue;
                tex.SetPixel(x, y, c);
            }
            // Note head
            FillCircleArea(tex, 30, 18, 8, c);
            FillCircleArea(tex, 33, 20, 4, new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildPlanet(Color body, Color shadow)
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 18, body);
            // Crater shadows
            FillCircleArea(tex, 26, 36, 4, shadow);
            FillCircleArea(tex, 38, 28, 3, shadow);
            FillCircleArea(tex, 30, 22, 2, shadow);
            // Ring
            DrawEllipseRing(tex, 32, 32, 28, 8, 0f, new Color(0.85f, 0.78f, 0.55f, 1f));
            DrawEllipseRing(tex, 32, 32, 26, 6, 0f, new Color(0.55f, 0.45f, 0.20f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildHexagon(Color hi, Color lo)
        {
            const int size = 64; var tex = NewTexture(size);
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                float dx = (x - 32f), dy = (y - 32f);
                float a = Mathf.Abs(dx) / 22f;
                float b = (Mathf.Abs(dy) + Mathf.Abs(dx) * 0.577f) / 26f;
                if (a < 1f && b < 1f)
                {
                    Color c = (dy < -2) ? hi : Color.Lerp(hi, lo, (dy + 22f) / 44f);
                    tex.SetPixel(x, y, c);
                }
            }
            return Finish(tex);
        }

        private static Sprite BuildTriangleNeon(Color c)
        {
            const int size = 64; var tex = NewTexture(size);
            // Hollow neon triangle (outline)
            for (int t = 0; t < 24; t++)
            {
                int x1 = 32, y1 = 56 - t;
                int x2 = 12 + t * 0, y2 = 12;
                int x3 = 52 - t * 0, y3 = 12;
                // 3 edges
                DrawLine(tex, 32, 56, 12 + t, 12, c);
                DrawLine(tex, 32, 56, 52 - t, 12, c);
                DrawLine(tex, 12, 12, 52, 12, c);
            }
            // Glow inner
            DrawLine(tex, 28, 52, 16, 16, new Color(1f, 1f, 1f, 0.5f));
            return Finish(tex);
        }

        private static Sprite BuildSmokePuff(Color c)
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 14, c);
            FillCircleArea(tex, 22, 28, 10, c);
            FillCircleArea(tex, 42, 28, 10, c);
            FillCircleArea(tex, 28, 42, 9, c);
            FillCircleArea(tex, 38, 40, 8, c);
            FillCircleArea(tex, 32, 22, 8, c);
            // Soft highlights
            FillCircleArea(tex, 26, 26, 3, Color.white);
            return Finish(tex);
        }

        private static Sprite BuildBubble(Color c)
        {
            const int size = 64; var tex = NewTexture(size);
            // Outline ring
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (d > 22) continue;
                if (d > 18) tex.SetPixel(x, y, c);
                else if (d > 16) tex.SetPixel(x, y, new Color(c.r, c.g, c.b, 0.4f));
            }
            // Highlight crescent
            FillCircleArea(tex, 26, 38, 3, Color.white);
            return Finish(tex);
        }

        private static Sprite BuildRose(Color hi, Color lo)
        {
            const int size = 64; var tex = NewTexture(size);
            // Layered petals
            FillCircleArea(tex, 32, 36, 16, lo);
            FillCircleArea(tex, 32, 36, 12, hi);
            FillCircleArea(tex, 32, 36, 8, lo);
            FillCircleArea(tex, 32, 36, 4, hi);
            tex.SetPixel(32, 36, lo);
            // Stem
            for (int y = 8; y < 20; y++) tex.SetPixel(32, y, new Color(0.08f, 0.40f, 0.10f, 1f));
            // Leaves
            FillCircleArea(tex, 28, 16, 3, new Color(0.30f, 0.80f, 0.20f, 1f));
            FillCircleArea(tex, 36, 14, 3, new Color(0.30f, 0.80f, 0.20f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildSun(Color hi, Color lo)
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 16, hi);
            FillCircleArea(tex, 32, 32, 12, new Color(1f, 1f, 0.65f, 1f));
            // Rays
            for (int i = 0; i < 8; i++)
            {
                float ang = i * Mathf.PI / 4f;
                int x1 = 32 + (int)(Mathf.Cos(ang) * 18);
                int y1 = 32 + (int)(Mathf.Sin(ang) * 18);
                int x2 = 32 + (int)(Mathf.Cos(ang) * 28);
                int y2 = 32 + (int)(Mathf.Sin(ang) * 28);
                DrawLine(tex, x1, y1, x2, y2, lo);
            }
            return Finish(tex);
        }

        private static Sprite BuildKey(Color body, Color shadow)
        {
            const int size = 64; var tex = NewTexture(size);
            // Bow (circle handle)
            FillCircleArea(tex, 20, 32, 10, body);
            FillCircleArea(tex, 20, 32, 5, new Color(0,0,0,0));
            // Shaft
            FillSquare(tex, 28, 30, 50, 34, body);
            // Teeth
            FillSquare(tex, 44, 26, 46, 30, body);
            FillSquare(tex, 48, 28, 50, 30, body);
            return Finish(tex);
        }

        private static Sprite BuildLock(Color body, Color keyhole)
        {
            const int size = 64; var tex = NewTexture(size);
            // Shackle
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                float dx = (x - 32f), dy = (y - 38f);
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d > 14 && d < 18 && y > 38) tex.SetPixel(x, y, body);
            }
            // Body box
            FillSquare(tex, 18, 12, 46, 36, body);
            FillSquare(tex, 20, 14, 44, 34, new Color(body.r * 1.2f, body.g * 1.2f, body.b * 1.2f, 1f));
            // Keyhole
            FillCircleArea(tex, 32, 24, 3, keyhole);
            FillSquare(tex, 31, 18, 33, 24, keyhole);
            return Finish(tex);
        }

        private static Sprite BuildCompass(Color face, Color needle)
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 24, new Color(0.55f, 0.45f, 0.20f, 1f));
            FillCircleArea(tex, 32, 32, 20, face);
            // Cardinal marks
            tex.SetPixel(32, 12, Color.black);
            tex.SetPixel(32, 52, Color.black);
            tex.SetPixel(12, 32, Color.black);
            tex.SetPixel(52, 32, Color.black);
            // Needle (red top, white bottom)
            for (int t = 0; t < 14; t++) tex.SetPixel(32, 32 + t, needle);
            for (int t = 0; t < 14; t++) tex.SetPixel(32, 32 - t, new Color(0.85f, 0.10f, 0.10f, 1f));
            FillCircleArea(tex, 32, 32, 3, Color.black);
            return Finish(tex);
        }

        private static Sprite BuildFish(Color body, Color fin)
        {
            const int size = 64; var tex = NewTexture(size);
            // Body ellipse
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                float u = (x - 28f) / 22f, v = (y - 32f) / 10f;
                if (u * u + v * v < 1f) tex.SetPixel(x, y, body);
            }
            // Tail
            for (int t = 0; t < 12; t++)
            {
                tex.SetPixel(50 + t, 32 + t, fin);
                tex.SetPixel(50 + t, 32 - t, fin);
            }
            // Eye
            FillCircleArea(tex, 20, 32, 2, Color.white);
            FillCircleArea(tex, 20, 32, 1, Color.black);
            return Finish(tex);
        }

        private static Sprite BuildBat(Color c)
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 6, c);
            // Wings — scalloped triangles
            for (int t = 0; t < 16; t++)
            {
                int x = 32 - 8 - t;
                int y = 32 + Mathf.Abs((t - 8)) - 4;
                if (x >= 0 && y >= 0 && y < size) FillSquare(tex, x, y, x + 1, y + 6, c);
                int x2 = 32 + 8 + t;
                int y2 = y;
                if (x2 < size && y2 >= 0 && y2 < size) FillSquare(tex, x2 - 1, y2, x2, y2 + 6, c);
            }
            // Ears
            FillSquare(tex, 28, 36, 30, 40, c);
            FillSquare(tex, 34, 36, 36, 40, c);
            // Eyes
            tex.SetPixel(29, 33, new Color(1f, 0.85f, 0.18f, 1f));
            tex.SetPixel(35, 33, new Color(1f, 0.85f, 0.18f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildGhost(Color c)
        {
            const int size = 64; var tex = NewTexture(size);
            // Round top
            FillCircleArea(tex, 32, 38, 16, c);
            FillSquare(tex, 16, 16, 48, 38, c);
            // Wavy bottom (scalloped)
            for (int x = 16; x < 48; x++)
            {
                int wave = (x % 8 < 4) ? 0 : 4;
                FillSquare(tex, x, 16 - wave, x, 16, c);
            }
            // Eyes
            FillCircleArea(tex, 26, 36, 3, Color.black);
            FillCircleArea(tex, 38, 36, 3, Color.black);
            // Mouth
            FillCircleArea(tex, 32, 28, 2, Color.black);
            return Finish(tex);
        }

        private static Sprite BuildMeteor(Color rock, Color trail)
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 22, 22, 10, rock);
            FillCircleArea(tex, 20, 24, 6, new Color(rock.r * 0.7f, rock.g * 0.7f, rock.b * 0.7f, 1f));
            // Flame trail
            for (int t = 0; t < 24; t++)
            {
                int x = 28 + t, y = 28 + t;
                if (x < size && y < size) FillCircleArea(tex, x, y, Mathf.Max(1, 5 - t / 5), trail);
            }
            return Finish(tex);
        }

        private static Sprite BuildPacPellet(Color c)
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 12, c);
            FillCircleArea(tex, 32, 32, 8, new Color(c.r * 1.2f, c.g * 1.2f, c.b * 1.2f, 1f));
            FillCircleArea(tex, 30, 34, 3, Color.white);
            return Finish(tex);
        }

        private static Sprite BuildGamePad(Color body, Color accent)
        {
            const int size = 64; var tex = NewTexture(size);
            // Rounded rect body
            FillSquare(tex, 12, 20, 52, 44, body);
            FillCircleArea(tex, 14, 32, 10, body);
            FillCircleArea(tex, 50, 32, 10, body);
            // D-pad
            FillSquare(tex, 16, 30, 22, 34, new Color(0.30f, 0.30f, 0.35f, 1f));
            FillSquare(tex, 18, 28, 20, 36, new Color(0.30f, 0.30f, 0.35f, 1f));
            // Buttons
            FillCircleArea(tex, 44, 36, 2, accent);
            FillCircleArea(tex, 48, 32, 2, new Color(1f, 0.85f, 0.18f, 1f));
            FillCircleArea(tex, 44, 28, 2, new Color(0.30f, 0.85f, 0.30f, 1f));
            FillCircleArea(tex, 40, 32, 2, new Color(0.30f, 0.55f, 0.95f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildPizzaSlice()
        {
            const int size = 64; var tex = NewTexture(size);
            Color crust = new Color(0.85f, 0.55f, 0.20f, 1f);
            Color cheese = new Color(1f, 0.92f, 0.42f, 1f);
            Color pep = new Color(0.85f, 0.10f, 0.10f, 1f);
            // Triangle
            for (int y = 8; y <= 56; y++)
            {
                float t = (y - 8f) / 48f;
                float half = Mathf.Lerp(0f, 22f, t);
                for (int x = 32 - (int)half; x <= 32 + (int)half; x++)
                {
                    if (x < 0 || x >= size) continue;
                    tex.SetPixel(x, y, cheese);
                }
            }
            // Crust edge (top arc)
            for (int x = 10; x <= 54; x++)
            {
                int y = 56 - Mathf.Abs(x - 32) / 2;
                if (y < size) for (int b = 0; b < 4; b++) if (y - b >= 0) tex.SetPixel(x, y - b, crust);
            }
            // Pepperoni
            FillCircleArea(tex, 30, 30, 3, pep);
            FillCircleArea(tex, 36, 26, 3, pep);
            FillCircleArea(tex, 30, 44, 3, pep);
            return Finish(tex);
        }

        private static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            int dx = Mathf.Abs(x1 - x0), dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            int safety = 256;
            while (safety-- > 0)
            {
                if (x0 >= 0 && y0 >= 0 && x0 < tex.width && y0 < tex.height) tex.SetPixel(x0, y0, c);
                if (x0 == x1 && y0 == y1) break;
                int e2 = err * 2;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 <  dx) { err += dx; y0 += sy; }
            }
        }

        // ── 20 NUEVOS SPRITES (todos 64×64 con pivot al centro) ────────────────────
        private static Sprite BuildAxe()
        {
            const int size = 64; var tex = NewTexture(size);
            // Handle diagonal
            for (int t = -16; t < 18; t++)
            {
                int x = 32 + t, y = 32 - t;
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    tex.SetPixel(x, y, new Color(0.55f, 0.32f, 0.10f, 1f));
                    if (x + 1 < size) tex.SetPixel(x + 1, y, new Color(0.30f, 0.16f, 0.05f, 1f));
                }
            }
            // Axe head
            FillCircleArea(tex, 18, 46, 9, new Color(0.78f, 0.80f, 0.86f, 1f));
            FillCircleArea(tex, 22, 50, 5, new Color(0.45f, 0.46f, 0.52f, 1f));
            // Sharp edge
            for (int t = 0; t < 14; t++) tex.SetPixel(10, 40 + t, new Color(1f, 1f, 1f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildArrow()
        {
            const int size = 64; var tex = NewTexture(size);
            Color shaft = new Color(0.55f, 0.32f, 0.10f, 1f);
            for (int t = 0; t < 36; t++)
            {
                int x = 14 + t, y = 32;
                tex.SetPixel(x, y, shaft);
                tex.SetPixel(x, y + 1, shaft);
            }
            // Tip
            for (int t = 0; t < 8; t++)
            {
                tex.SetPixel(50 + t, 32 - t, new Color(0.85f, 0.86f, 0.92f, 1f));
                tex.SetPixel(50 + t, 33 + t, new Color(0.85f, 0.86f, 0.92f, 1f));
            }
            // Feather fletching
            for (int t = 0; t < 6; t++)
            {
                tex.SetPixel(12 - t, 32 - t, new Color(0.85f, 0.10f, 0.10f, 1f));
                tex.SetPixel(12 - t, 34 + t, new Color(0.85f, 0.10f, 0.10f, 1f));
            }
            return Finish(tex);
        }

        private static Sprite BuildDna()
        {
            const int size = 64; var tex = NewTexture(size);
            Color a = new Color(0.30f, 0.85f, 1f, 1f);
            Color b = new Color(1f, 0.30f, 0.55f, 1f);
            for (int y = 4; y < 60; y++)
            {
                float ang = (y / 60f) * Mathf.PI * 4f;
                int x1 = 32 + Mathf.RoundToInt(Mathf.Cos(ang) * 12f);
                int x2 = 32 - Mathf.RoundToInt(Mathf.Cos(ang) * 12f);
                tex.SetPixel(x1, y, a); tex.SetPixel(x1 + 1, y, a);
                tex.SetPixel(x2, y, b); tex.SetPixel(x2 + 1, y, b);
                if ((y % 6) == 0)
                {
                    int lo = Mathf.Min(x1, x2), hi = Mathf.Max(x1, x2);
                    for (int x = lo + 2; x < hi; x++) tex.SetPixel(x, y, new Color(0.85f, 0.85f, 0.86f, 1f));
                }
            }
            return Finish(tex);
        }

        private static Sprite BuildAtom()
        {
            const int size = 64; var tex = NewTexture(size);
            // Nucleus
            FillCircleArea(tex, 32, 32, 6, new Color(1f, 0.45f, 0.15f, 1f));
            FillCircleArea(tex, 32, 32, 3, new Color(1f, 0.92f, 0.40f, 1f));
            // 3 orbit ellipses rotated
            DrawEllipseRing(tex, 32, 32, 26, 10, 0, new Color(0.30f, 0.85f, 1f, 1f));
            DrawEllipseRing(tex, 32, 32, 26, 10, 60, new Color(0.55f, 1f, 0.45f, 1f));
            DrawEllipseRing(tex, 32, 32, 26, 10, 120, new Color(1f, 0.50f, 0.85f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildFlame()
        {
            const int size = 64; var tex = NewTexture(size);
            Color outer = new Color(1f, 0.40f, 0.05f, 1f);
            Color mid = new Color(1f, 0.78f, 0.18f, 1f);
            Color inner = new Color(1f, 0.96f, 0.55f, 1f);
            Vector2 c = new Vector2(32, 28);
            for (int y = 4; y < 60; y++)
            {
                float t = (y - 4) / 56f;
                float width = Mathf.Lerp(20f, 2f, t);
                width += Mathf.Sin(t * 14f) * 1.6f;
                int xMin = (int)(c.x - width / 2);
                int xMax = (int)(c.x + width / 2);
                for (int x = xMin; x <= xMax; x++)
                {
                    if (x < 0 || x >= size) continue;
                    float u = Mathf.Abs(x - c.x) / (width * 0.5f);
                    Color col = u < 0.35f ? inner : (u < 0.7f ? mid : outer);
                    tex.SetPixel(x, y, col);
                }
            }
            return Finish(tex);
        }

        private static Sprite BuildIceShard()
        {
            const int size = 64; var tex = NewTexture(size);
            Color hi = new Color(0.85f, 0.95f, 1f, 1f);
            Color lo = new Color(0.35f, 0.65f, 0.95f, 1f);
            // Diamond + extra spike at the top
            Vector2 c = new Vector2(32, 36);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (x - c.x) / 16f, v = (y - c.y) / 24f;
                    if (Mathf.Abs(u) + Mathf.Abs(v) < 1f)
                        tex.SetPixel(x, y, Mathf.Abs(u) < 0.25f ? hi : lo);
                }
            }
            // Shine streak
            for (int t = 0; t < 14; t++) tex.SetPixel(28 + t / 3, 28 + t, Color.white);
            return Finish(tex);
        }

        private static Sprite BuildDrop()
        {
            const int size = 64; var tex = NewTexture(size);
            Color hi = new Color(0.55f, 0.85f, 1f, 1f);
            Color lo = new Color(0.10f, 0.40f, 0.78f, 1f);
            for (int y = 4; y < 60; y++)
            {
                float t = (y - 4) / 56f;
                float w = (t < 0.55f) ? Mathf.Lerp(2f, 18f, t / 0.55f)
                                       : Mathf.Lerp(18f, 14f, (t - 0.55f) / 0.45f);
                for (int x = 32 - (int)(w / 2); x <= 32 + (int)(w / 2); x++)
                {
                    if (x < 0 || x >= size) continue;
                    float u = Mathf.Abs(x - 32) / (w * 0.5f);
                    tex.SetPixel(x, y, u < 0.35f ? Color.white : Color.Lerp(hi, lo, u));
                }
            }
            return Finish(tex);
        }

        private static Sprite BuildClockSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 26, new Color(0.95f, 0.92f, 0.86f, 1f));
            FillCircleArea(tex, 32, 32, 22, new Color(1f, 1f, 1f, 1f));
            // Tick marks
            for (int i = 0; i < 12; i++)
            {
                float ang = i / 12f * Mathf.PI * 2f;
                int x = 32 + (int)(Mathf.Cos(ang) * 20);
                int y = 32 + (int)(Mathf.Sin(ang) * 20);
                tex.SetPixel(x, y, Color.black);
            }
            // Hands
            for (int t = 0; t < 14; t++) tex.SetPixel(32, 32 + t, Color.black);
            for (int t = 0; t < 10; t++) tex.SetPixel(32 + t, 32, new Color(0.85f, 0.10f, 0.10f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildEyeSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            // Almond shape
            for (int y = 26; y < 38; y++)
            {
                for (int x = 12; x < 52; x++)
                {
                    float u = (x - 32) / 20f, v = (y - 32) / 6f;
                    if (u * u + v * v < 1f) tex.SetPixel(x, y, Color.white);
                }
            }
            // Iris
            FillCircleArea(tex, 32, 32, 8, new Color(0.20f, 0.55f, 0.90f, 1f));
            FillCircleArea(tex, 32, 32, 4, Color.black);
            // Highlight
            tex.SetPixel(34, 34, Color.white);
            return Finish(tex);
        }

        private static Sprite BuildChainLink()
        {
            const int size = 64; var tex = NewTexture(size);
            // Outer ring, hollow centre
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (x - 32) / 18f, v = (y - 32) / 28f;
                    float d = u * u + v * v;
                    if (d < 1f && d > 0.42f)
                    {
                        Color c = d > 0.78f ? new Color(0.45f, 0.45f, 0.50f, 1f)
                                            : new Color(0.85f, 0.85f, 0.90f, 1f);
                        tex.SetPixel(x, y, c);
                    }
                }
            }
            return Finish(tex);
        }

        private static Sprite BuildCoinSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 32, 22, new Color(1f, 0.85f, 0.20f, 1f));
            FillCircleArea(tex, 32, 32, 19, new Color(1f, 0.96f, 0.45f, 1f));
            FillCircleArea(tex, 32, 32, 16, new Color(0.85f, 0.65f, 0.10f, 1f));
            // Star centre
            FillCircleArea(tex, 32, 32, 4, new Color(1f, 1f, 0.50f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildDiceSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            FillSquare(tex, 14, 14, 50, 50, new Color(0.95f, 0.95f, 0.95f, 1f));
            FillSquare(tex, 16, 16, 50, 50, new Color(0.78f, 0.78f, 0.78f, 1f));
            FillSquare(tex, 14, 14, 48, 48, new Color(1f, 1f, 1f, 1f));
            // Pip pattern (5)
            FillCircleArea(tex, 22, 22, 3, Color.black);
            FillCircleArea(tex, 42, 22, 3, Color.black);
            FillCircleArea(tex, 32, 32, 3, Color.black);
            FillCircleArea(tex, 22, 42, 3, Color.black);
            FillCircleArea(tex, 42, 42, 3, Color.black);
            return Finish(tex);
        }

        private static Sprite BuildCardSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            FillSquare(tex, 16, 6, 48, 58, new Color(0.95f, 0.95f, 0.95f, 1f));
            FillSquare(tex, 18, 8, 46, 56, Color.white);
            // Suit (heart) centred
            FillCircleArea(tex, 27, 36, 6, new Color(0.85f, 0.10f, 0.10f, 1f));
            FillCircleArea(tex, 37, 36, 6, new Color(0.85f, 0.10f, 0.10f, 1f));
            FillTriangleDown(tex, 22, 26, 42, 26, 32, 14, new Color(0.85f, 0.10f, 0.10f, 1f));
            // Corner A
            FillSquare(tex, 20, 50, 24, 54, new Color(0.85f, 0.10f, 0.10f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildBombSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 28, 18, new Color(0.10f, 0.10f, 0.12f, 1f));
            FillCircleArea(tex, 32, 28, 14, new Color(0.20f, 0.20f, 0.25f, 1f));
            // Shine
            FillCircleArea(tex, 26, 22, 3, Color.white);
            // Fuse
            for (int t = 0; t < 12; t++)
            {
                int x = 32 + t / 2, y = 46 + t;
                if (x < size && y < size) tex.SetPixel(x, y, new Color(0.55f, 0.32f, 0.10f, 1f));
            }
            // Spark
            FillCircleArea(tex, 38, 58, 3, new Color(1f, 0.78f, 0.18f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildPotionSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            // Bottle
            FillSquare(tex, 22, 6, 42, 36, new Color(0.30f, 0.85f, 1f, 1f));
            FillSquare(tex, 26, 36, 38, 50, new Color(0.55f, 0.85f, 1f, 1f));
            // Cork
            FillSquare(tex, 26, 50, 38, 56, new Color(0.55f, 0.32f, 0.10f, 1f));
            // Shine
            for (int y = 10; y < 30; y++) tex.SetPixel(26, y, Color.white);
            // Bubbles
            FillCircleArea(tex, 30, 18, 2, new Color(1f, 1f, 1f, 0.7f));
            FillCircleArea(tex, 36, 24, 2, new Color(1f, 1f, 1f, 0.7f));
            return Finish(tex);
        }

        private static Sprite BuildPumpkinSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            FillCircleArea(tex, 32, 30, 22, new Color(1f, 0.55f, 0.10f, 1f));
            // Vertical ridges
            for (int y = 12; y < 50; y++)
            {
                tex.SetPixel(22, y, new Color(0.78f, 0.32f, 0.04f, 1f));
                tex.SetPixel(32, y, new Color(0.78f, 0.32f, 0.04f, 1f));
                tex.SetPixel(42, y, new Color(0.78f, 0.32f, 0.04f, 1f));
            }
            // Face
            FillTriangleDown(tex, 22, 30, 28, 30, 25, 24, Color.black);
            FillTriangleDown(tex, 36, 30, 42, 30, 39, 24, Color.black);
            FillSquare(tex, 24, 18, 40, 22, Color.black);
            tex.SetPixel(26, 20, new Color(1f, 0.55f, 0.10f, 1f));
            tex.SetPixel(32, 20, new Color(1f, 0.55f, 0.10f, 1f));
            tex.SetPixel(38, 20, new Color(1f, 0.55f, 0.10f, 1f));
            // Stem
            FillSquare(tex, 30, 50, 34, 56, new Color(0.30f, 0.55f, 0.12f, 1f));
            return Finish(tex);
        }

        private static Sprite BuildAnchorSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            Color metal = new Color(0.45f, 0.50f, 0.58f, 1f);
            Color metalHi = new Color(0.78f, 0.82f, 0.86f, 1f);
            // Shaft
            FillSquare(tex, 30, 14, 34, 54, metal);
            FillSquare(tex, 31, 14, 33, 54, metalHi);
            // Top ring
            FillCircleArea(tex, 32, 54, 5, metal);
            FillCircleArea(tex, 32, 54, 3, new Color(0, 0, 0, 0));
            // Crossbar
            FillSquare(tex, 22, 44, 42, 46, metal);
            // Arms
            for (int t = 0; t < 14; t++)
            {
                int x1 = 18 + t / 2, y1 = 14 + t;
                int x2 = 46 - t / 2, y2 = 14 + t;
                if (x1 < size && y1 < size) tex.SetPixel(x1, y1, metal);
                if (x2 < size && y2 < size) tex.SetPixel(x2, y2, metal);
            }
            return Finish(tex);
        }

        private static Sprite BuildHorseshoe()
        {
            const int size = 64; var tex = NewTexture(size);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (x - 32) / 20f, v = (y - 28) / 22f;
                    float d = u * u + v * v;
                    if (d < 1f && d > 0.45f && y < 36)
                    {
                        Color c = d > 0.8f ? new Color(0.45f, 0.45f, 0.50f, 1f)
                                            : new Color(0.85f, 0.85f, 0.90f, 1f);
                        tex.SetPixel(x, y, c);
                    }
                }
            }
            // Nail holes
            for (int i = 0; i < 6; i++)
            {
                float ang = Mathf.Lerp(Mathf.PI, Mathf.PI * 2f, i / 5f);
                int hx = 32 + (int)(Mathf.Cos(ang) * 16);
                int hy = 28 + (int)(Mathf.Sin(ang) * 18);
                if (hx >= 0 && hy >= 0 && hx < size && hy < size) tex.SetPixel(hx, hy, Color.black);
            }
            return Finish(tex);
        }

        private static Sprite BuildPawSprite()
        {
            const int size = 64; var tex = NewTexture(size);
            Color pad = new Color(0.32f, 0.20f, 0.12f, 1f);
            // Main pad
            FillCircleArea(tex, 32, 24, 10, pad);
            // Four toes
            FillCircleArea(tex, 18, 36, 5, pad);
            FillCircleArea(tex, 26, 44, 5, pad);
            FillCircleArea(tex, 38, 44, 5, pad);
            FillCircleArea(tex, 46, 36, 5, pad);
            return Finish(tex);
        }

        private static Sprite BuildGalaxySprite()
        {
            const int size = 64; var tex = NewTexture(size);
            // Background dark
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (d > 28) continue;
                float bg = 0.08f - d * 0.002f;
                tex.SetPixel(x, y, new Color(bg, bg, bg + 0.05f, 1f));
            }
            // Spiral arms
            for (int t = 0; t < 60; t++)
            {
                float ang = t * 0.32f;
                float r = 4 + t * 0.4f;
                int x = 32 + (int)(Mathf.Cos(ang) * r);
                int y = 32 + (int)(Mathf.Sin(ang) * r);
                if (x >= 0 && y >= 0 && x < size && y < size)
                {
                    tex.SetPixel(x, y, new Color(0.55f, 0.65f, 1f, 1f));
                    if (x + 1 < size) tex.SetPixel(x + 1, y, new Color(0.85f, 0.85f, 1f, 1f));
                }
                int x2 = 32 - (int)(Mathf.Cos(ang) * r);
                int y2 = 32 - (int)(Mathf.Sin(ang) * r);
                if (x2 >= 0 && y2 >= 0 && x2 < size && y2 < size)
                {
                    tex.SetPixel(x2, y2, new Color(0.95f, 0.45f, 0.85f, 1f));
                }
            }
            // Bright core
            FillCircleArea(tex, 32, 32, 4, new Color(1f, 0.92f, 0.65f, 1f));
            return Finish(tex);
        }

        // ── shared helpers used by the new sprites ──
        private static void FillCircleArea(Texture2D tex, int cx, int cy, int r, Color c)
        {
            for (int y = cy - r; y <= cy + r; y++)
                for (int x = cx - r; x <= cx + r; x++)
                {
                    if (x < 0 || y < 0 || x >= tex.width || y >= tex.height) continue;
                    int dx = x - cx, dy = y - cy;
                    if (dx * dx + dy * dy <= r * r) tex.SetPixel(x, y, c);
                }
        }
        private static void DrawEllipseRing(Texture2D tex, int cx, int cy, int rx, int ry, float angleDeg, Color c)
        {
            float rad = angleDeg * Mathf.Deg2Rad;
            float cs = Mathf.Cos(rad), sn = Mathf.Sin(rad);
            for (float a = 0; a < Mathf.PI * 2; a += 0.05f)
            {
                float ex = rx * Mathf.Cos(a);
                float ey = ry * Mathf.Sin(a);
                int x = cx + Mathf.RoundToInt(ex * cs - ey * sn);
                int y = cy + Mathf.RoundToInt(ex * sn + ey * cs);
                if (x >= 0 && y >= 0 && x < tex.width && y < tex.height) tex.SetPixel(x, y, c);
            }
        }
        private static void FillTriangleDown(Texture2D tex, int x0, int y0, int x1, int y1, int x2, int y2, Color c)
        {
            int yMin = Mathf.Min(y0, Mathf.Min(y1, y2));
            int yMax = Mathf.Max(y0, Mathf.Max(y1, y2));
            for (int y = yMin; y <= yMax; y++)
            {
                int leftX = int.MaxValue, rightX = int.MinValue;
                CheckEdge(x0, y0, x1, y1, y, ref leftX, ref rightX);
                CheckEdge(x1, y1, x2, y2, y, ref leftX, ref rightX);
                CheckEdge(x2, y2, x0, y0, y, ref leftX, ref rightX);
                for (int x = leftX; x <= rightX; x++)
                    if (x >= 0 && y >= 0 && x < tex.width && y < tex.height) tex.SetPixel(x, y, c);
            }
        }
        private static void CheckEdge(int ax, int ay, int bx, int by, int y, ref int lo, ref int hi)
        {
            if ((ay <= y && by > y) || (by <= y && ay > y))
            {
                float t = (y - ay) / (float)(by - ay);
                int x = ax + Mathf.RoundToInt((bx - ax) * t);
                if (x < lo) lo = x;
                if (x > hi) hi = x;
            }
        }

        private static Sprite BuildOrb(Color core, Color highlight)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float r = size * 0.36f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), c) / r;
                    if (d > 1.05f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    Color col = Color.Lerp(core, highlight, Mathf.SmoothStep(1f, 0f, d) * 0.65f);
                    if (d < 0.28f)
                    {
                        col = Color.Lerp(col, highlight, 0.75f);
                    }

                    col.a = Mathf.SmoothStep(1f, 0.82f, d);
                    tex.SetPixel(x, y, col);
                }
            }

            return Finish(tex);
        }

        private static Sprite BuildRing(Color inner, Color outer)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float outerR = size * 0.4f;
            float innerR = size * 0.26f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), c);
                    if (d < innerR || d > outerR)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float t = (d - innerR) / (outerR - innerR);
                    tex.SetPixel(x, y, Color.Lerp(inner, outer, t));
                }
            }

            return Finish(tex);
        }

        /// <summary>
        /// Diamond-cut gem: octagonal silhouette with bright facet highlights.
        /// </summary>
        private static Sprite BuildGem(Color deep, Color mid, Color shine)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float r = size * 0.40f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - c.x) / r;
                    float dy = (y - c.y) / r;
                    // Diamond silhouette: |dx| + |dy| <= 1 clipped to circle
                    float diamondDist = Mathf.Abs(dx) + Mathf.Abs(dy);
                    float circleDist  = Mathf.Sqrt(dx * dx + dy * dy);
                    if (diamondDist > 1.05f || circleDist > 1.05f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    // Facet zones based on angle and radius
                    float angle = Mathf.Atan2(dy, dx); // -pi..pi
                    float facet = Mathf.Abs(Mathf.Sin(angle * 4f));  // 4 reflections

                    // Upper-left specular highlight
                    float specular = Mathf.Max(0f, -dx * 0.6f + -dy * 0.8f);
                    specular = Mathf.Pow(specular, 2.2f);

                    float t = Mathf.SmoothStep(0f, 1f, circleDist * 0.9f);
                    Color col = Color.Lerp(shine, mid, t);
                    col = Color.Lerp(col, deep, t * 0.6f);
                    col = Color.Lerp(col, shine, facet * 0.35f);
                    col = Color.Lerp(col, Color.white, specular * 0.65f);
                    col.a = Mathf.Clamp01(1f - Mathf.SmoothStep(0.80f, 1.05f, circleDist)
                                              - Mathf.SmoothStep(0.90f, 1.05f, diamondDist) * 0.5f);
                    tex.SetPixel(x, y, col);
                }
            }

            return Finish(tex);
        }

        /// <summary>
        /// 5-pointed star with glowing hot-white centre.
        /// </summary>
        private static Sprite BuildStar(Color outer, Color inner)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float outerR = size * 0.42f;
            float innerR = size * 0.18f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - c.x;
                    float dy = y - c.y;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    // Star boundary via sector interpolation
                    float angle = Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f; // 0 at top
                    float sector = angle / (Mathf.PI * 2f / 5f);
                    float frac = sector - Mathf.Floor(sector);
                    // Radial limit alternates outerR and innerR
                    float limitA = (frac < 0.5f)
                        ? Mathf.Lerp(outerR, innerR, frac * 2f)
                        : Mathf.Lerp(innerR, outerR, (frac - 0.5f) * 2f);

                    if (dist > limitA * 1.05f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float t = Mathf.Clamp01(dist / Mathf.Max(limitA, 0.01f));
                    Color col = Color.Lerp(Color.white, inner, t * 0.55f);
                    col = Color.Lerp(col, outer, t * 0.80f);
                    col.a = Mathf.SmoothStep(1f, 0.70f, t);
                    // Soft glow halo just outside
                    if (dist > limitA * 0.85f)
                    {
                        float glowT = (dist - limitA * 0.85f) / (limitA * 0.2f);
                        col.a *= Mathf.SmoothStep(1f, 0f, glowT);
                    }

                    tex.SetPixel(x, y, col);
                }
            }

            return Finish(tex);
        }

        /// <summary>
        /// Plasma ball: solid glowing core surrounded by a soft corona ring.
        /// </summary>
        private static Sprite BuildPlasma(Color core, Color mid, Color corona)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float coreR   = size * 0.22f;
            float coronaR = size * 0.44f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), c);
                    if (d > coronaR * 1.05f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    if (d < coreR)
                    {
                        // Inner hot core
                        float t = d / coreR;
                        Color col = Color.Lerp(Color.white, mid, t * 0.5f);
                        col.a = 1f;
                        tex.SetPixel(x, y, col);
                    }
                    else
                    {
                        // Corona halo
                        float t = (d - coreR) / (coronaR - coreR);
                        Color col = Color.Lerp(mid, corona, t * 0.7f);

                        // Electric arcs: modulate with angle-based noise
                        float angle = Mathf.Atan2(y - c.y, x - c.x);
                        float arc = Mathf.Abs(Mathf.Sin(angle * 7f + d * 0.18f));
                        col = Color.Lerp(col, Color.white, arc * (1f - t) * 0.45f);

                        col.a = Mathf.SmoothStep(1f, 0f, t * 0.85f) *
                                Mathf.SmoothStep(0f, 1f, 1f - t * 0.2f);
                        tex.SetPixel(x, y, col);
                    }
                }
            }

            return Finish(tex);
        }

        // ──────────────────────────────────────────────────────────────────────────────
        //                            NEW SPRITE BUILDERS
        //   All sprites are 64×64 with the pivot at the centre.
        //   Coordinates: (32,32) is the centre, x grows right, y grows up.
        // ──────────────────────────────────────────────────────────────────────────────

        /// <summary>Knife: straight diagonal blade with a small handle at one end.</summary>
        private static Sprite BuildKnife(Color blade, Color bladeDark, Color handle)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            // Blade — thin diagonal stripe with a tip
            for (int t = 0; t < 40; t++)
            {
                int x = 14 + t;
                int y = 14 + t;
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    tex.SetPixel(x, y, blade);
                    if (x - 1 >= 0) tex.SetPixel(x - 1, y, blade);
                    if (y - 1 >= 0) tex.SetPixel(x, y - 1, bladeDark);
                    if (x + 1 < size) tex.SetPixel(x + 1, y, bladeDark);
                }
            }
            // Tip highlight
            for (int t = 30; t < 40; t++)
            {
                int x = 14 + t, y = 14 + t;
                if (x < size - 1 && y < size - 1) tex.SetPixel(x + 1, y + 1, blade);
            }
            // Handle
            for (int t = -8; t < 0; t++)
            {
                int x = 14 + t, y = 14 + t;
                if (x >= 0 && y >= 0 && x < size && y < size)
                {
                    tex.SetPixel(x, y, handle);
                    if (x - 1 >= 0) tex.SetPixel(x - 1, y, handle);
                    if (y - 1 >= 0) tex.SetPixel(x, y - 1, handle);
                }
            }
            // Crossguard
            for (int k = -2; k <= 2; k++)
            {
                if (12 + k >= 0 && 16 + k < size) tex.SetPixel(12 + k, 16 - k, bladeDark);
            }
            return Finish(tex);
        }

        /// <summary>4-pointed shuriken/ninja star.</summary>
        private static Sprite BuildShuriken(Color body, Color edge)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float arm = size * 0.42f;
            float thickness = 4.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - c.x;
                    float dy = y - c.y;
                    // Star: union of two perpendicular thin diamonds.
                    bool inH = Mathf.Abs(dy) < thickness && Mathf.Abs(dx) < arm;
                    bool inV = Mathf.Abs(dx) < thickness && Mathf.Abs(dy) < arm;
                    // Taper: thickness shrinks toward the tip.
                    if (inH) inH = Mathf.Abs(dy) < thickness * (1f - Mathf.Abs(dx) / arm);
                    if (inV) inV = Mathf.Abs(dx) < thickness * (1f - Mathf.Abs(dy) / arm);
                    if (!inH && !inV) { tex.SetPixel(x, y, Color.clear); continue; }
                    // Centre hub
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    Color col = dist < 6 ? edge : body;
                    tex.SetPixel(x, y, col);
                }
            }
            return Finish(tex);
        }

        /// <summary>Sword: vertical long blade with cross-guard and pommel.</summary>
        private static Sprite BuildSword(Color blade, Color bladeDark, Color hilt)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            // Blade — vertical, tapered tip
            for (int y = 14; y <= 52; y++)
            {
                int w = (y > 48) ? Mathf.Max(0, 52 - y) : 3;
                for (int dx = -w; dx <= w; dx++)
                {
                    int x = 32 + dx;
                    if (x < 0 || x >= size) continue;
                    Color c = (dx == 0) ? blade : Color.Lerp(blade, bladeDark, Mathf.Abs(dx) / 3f);
                    tex.SetPixel(x, y, c);
                }
            }
            // Crossguard
            for (int x = 22; x <= 42; x++) { tex.SetPixel(x, 13, hilt); tex.SetPixel(x, 12, hilt); }
            // Grip
            for (int y = 5; y <= 11; y++) { tex.SetPixel(31, y, hilt); tex.SetPixel(32, y, hilt); tex.SetPixel(33, y, hilt); }
            // Pommel
            tex.SetPixel(31, 4, hilt); tex.SetPixel(32, 3, hilt); tex.SetPixel(33, 4, hilt); tex.SetPixel(32, 4, hilt);
            return Finish(tex);
        }

        /// <summary>Stylised Asian-ink character (cross-like brush strokes).</summary>
        private static Sprite BuildKanji(Color ink, Color shadow)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            // Horizontal stroke
            for (int x = 10; x <= 54; x++)
            {
                for (int yy = 33; yy <= 36; yy++) tex.SetPixel(x, yy, ink);
            }
            // Vertical stroke
            for (int y = 12; y <= 52; y++)
            {
                for (int xx = 30; xx <= 33; xx++) tex.SetPixel(xx, y, ink);
            }
            // Top short horizontal
            for (int x = 20; x <= 44; x++) tex.SetPixel(x, 48, ink);
            // Bottom diagonal strokes
            for (int t = 0; t < 12; t++)
            {
                tex.SetPixel(18 + t, 22 - t, ink);
                tex.SetPixel(46 - t, 22 - t, ink);
            }
            // Ink bleed shadow
            for (int x = 10; x <= 54; x++) tex.SetPixel(x, 32, shadow);
            for (int y = 12; y <= 52; y++) tex.SetPixel(29, y, shadow);
            return Finish(tex);
        }

        /// <summary>Pixel skull with eye sockets.</summary>
        private static Sprite BuildSkull(Color bone, Color shadow, Color sockets)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.55f);
            float r = size * 0.34f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - c.x) / r;
                    float dy = (y - c.y) / r;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    // Jaw extension
                    bool inJaw = (y < c.y - r * 0.4f) && Mathf.Abs(dx) < 0.55f && (c.y - y) < r * 1.1f;
                    if (d > 1.05f && !inJaw) { tex.SetPixel(x, y, Color.clear); continue; }
                    Color col = d < 0.8f ? bone : shadow;
                    if (inJaw) col = bone;
                    tex.SetPixel(x, y, col);
                }
            }
            // Eye sockets
            FillSquare(tex, 22, 36, 28, 42, sockets);
            FillSquare(tex, 36, 36, 42, 42, sockets);
            // Nose
            FillSquare(tex, 31, 28, 33, 32, sockets);
            // Teeth lines
            for (int x = 24; x <= 40; x += 3) FillSquare(tex, x, 18, x, 24, shadow);
            return Finish(tex);
        }

        /// <summary>Classic heart shape.</summary>
        private static Sprite BuildHeart(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.45f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - c.x) / (size * 0.32f);
                    float dy = (c.y - y) / (size * 0.32f);
                    // Heart implicit eq: (x^2 + y^2 - 1)^3 - x^2 * y^3 <= 0
                    float term = dx * dx + dy * dy - 1f;
                    float val = term * term * term - dx * dx * dy * dy * dy;
                    if (val > 0f) { tex.SetPixel(x, y, Color.clear); continue; }
                    // Specular highlight upper-left
                    float spec = Mathf.Max(0f, -dx * 0.7f + dy * 0.5f);
                    Color col = Color.Lerp(lo, hi, Mathf.Clamp01(spec + 0.55f));
                    tex.SetPixel(x, y, col);
                }
            }
            return Finish(tex);
        }

        /// <summary>Lightning bolt — zig-zag.</summary>
        private static Sprite BuildLightning(Color body, Color glow)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            // Zig-zag path with width
            int[] xs = { 40, 28, 36, 24 };
            int[] ys = { 56,  40, 30, 8 };
            for (int seg = 0; seg < xs.Length - 1; seg++)
            {
                int x0 = xs[seg], y0 = ys[seg], x1 = xs[seg + 1], y1 = ys[seg + 1];
                int steps = Mathf.Max(Mathf.Abs(x1 - x0), Mathf.Abs(y1 - y0));
                for (int s = 0; s <= steps; s++)
                {
                    float t = s / (float)steps;
                    int px = Mathf.RoundToInt(Mathf.Lerp(x0, x1, t));
                    int py = Mathf.RoundToInt(Mathf.Lerp(y0, y1, t));
                    for (int wx = -3; wx <= 3; wx++)
                    {
                        for (int wy = -3; wy <= 3; wy++)
                        {
                            int xx = px + wx, yy = py + wy;
                            if (xx < 0 || yy < 0 || xx >= size || yy >= size) continue;
                            float d = Mathf.Sqrt(wx * wx + wy * wy);
                            if (d < 1.6f) tex.SetPixel(xx, yy, body);
                            else if (d < 3.0f) tex.SetPixel(xx, yy, glow);
                        }
                    }
                }
            }
            return Finish(tex);
        }

        /// <summary>6-pointed snowflake.</summary>
        private static Sprite BuildSnowflake(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float arm = size * 0.42f;
            // 6 arms at 60° increments
            for (int a = 0; a < 6; a++)
            {
                float ang = a * Mathf.PI / 3f;
                float cx = Mathf.Cos(ang), sy = Mathf.Sin(ang);
                for (float t = 0; t < arm; t += 0.5f)
                {
                    int px = Mathf.RoundToInt(c.x + cx * t);
                    int py = Mathf.RoundToInt(c.y + sy * t);
                    if (px < 0 || py < 0 || px >= size || py >= size) continue;
                    tex.SetPixel(px, py, hi);
                    if (px + 1 < size) tex.SetPixel(px + 1, py, lo);
                    if (py + 1 < size) tex.SetPixel(px, py + 1, lo);
                }
                // Branch barbs at 3 distances along the arm
                for (int br = 1; br <= 3; br++)
                {
                    float t = arm * 0.3f * br;
                    int px = Mathf.RoundToInt(c.x + cx * t);
                    int py = Mathf.RoundToInt(c.y + sy * t);
                    float bAng1 = ang + Mathf.PI / 6f;
                    float bAng2 = ang - Mathf.PI / 6f;
                    for (float bt = 0; bt < arm * 0.18f; bt += 0.5f)
                    {
                        int bx1 = Mathf.RoundToInt(px + Mathf.Cos(bAng1) * bt);
                        int by1 = Mathf.RoundToInt(py + Mathf.Sin(bAng1) * bt);
                        int bx2 = Mathf.RoundToInt(px + Mathf.Cos(bAng2) * bt);
                        int by2 = Mathf.RoundToInt(py + Mathf.Sin(bAng2) * bt);
                        if (bx1 >= 0 && by1 >= 0 && bx1 < size && by1 < size) tex.SetPixel(bx1, by1, lo);
                        if (bx2 >= 0 && by2 >= 0 && bx2 < size && by2 < size) tex.SetPixel(bx2, by2, lo);
                    }
                }
            }
            // Centre dot
            FillSquare(tex, 30, 30, 34, 34, hi);
            return Finish(tex);
        }

        /// <summary>Stylised leaf / petal shape.</summary>
        private static Sprite BuildLeaf(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (x - size * 0.5f) / (size * 0.35f);
                    float v = (y - size * 0.5f) / (size * 0.45f);
                    // Pointed-oval (leaf) implicit boundary: u^2 + v^2 - |v| < 0.6
                    float val = u * u + v * v - Mathf.Abs(v) * 0.6f - 0.55f;
                    if (val > 0f) { tex.SetPixel(x, y, Color.clear); continue; }
                    Color col = Color.Lerp(lo, hi, Mathf.Clamp01(0.5f + u * 0.5f));
                    tex.SetPixel(x, y, col);
                }
            }
            // Veins
            for (int y = 12; y <= 52; y++) tex.SetPixel(32, y, lo);
            for (int t = 0; t < 8; t++)
            {
                tex.SetPixel(32 - t, 36 + t, lo);
                tex.SetPixel(32 + t, 36 + t, lo);
                tex.SetPixel(32 - t, 26 + t, lo);
                tex.SetPixel(32 + t, 26 + t, lo);
            }
            return Finish(tex);
        }

        /// <summary>Yin-yang circle.</summary>
        private static Sprite BuildYinYang()
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float r = size * 0.42f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - c.x;
                    float dy = y - c.y;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    if (d > r) { tex.SetPixel(x, y, Color.clear); continue; }
                    // Two smaller circles to make the S-curve.
                    bool whiteHalf = dx > 0f;
                    float dCircleTop = Mathf.Sqrt(dx * dx + (dy - r * 0.5f) * (dy - r * 0.5f));
                    float dCircleBot = Mathf.Sqrt(dx * dx + (dy + r * 0.5f) * (dy + r * 0.5f));
                    if (dCircleTop < r * 0.5f) whiteHalf = false;
                    if (dCircleBot < r * 0.5f) whiteHalf = true;
                    Color col = whiteHalf ? Color.white : new Color(0.06f, 0.06f, 0.08f, 1f);
                    // Dots
                    if (dCircleTop < r * 0.16f) col = new Color(0.06f, 0.06f, 0.08f, 1f);
                    if (dCircleBot < r * 0.16f) col = Color.white;
                    tex.SetPixel(x, y, col);
                }
            }
            return Finish(tex);
        }

        /// <summary>Cross/crucifix shape with bevel.</summary>
        private static Sprite BuildCross(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            // Vertical bar
            FillSquare(tex, 28, 8, 36, 56, hi);
            FillSquare(tex, 29, 8, 35, 55, lo);
            FillSquare(tex, 30, 8, 34, 54, hi);
            // Horizontal bar
            FillSquare(tex, 14, 36, 50, 44, hi);
            FillSquare(tex, 14, 37, 50, 43, lo);
            FillSquare(tex, 14, 38, 50, 42, hi);
            return Finish(tex);
        }

        /// <summary>Crescent moon (outline-circle minus offset circle).</summary>
        private static Sprite BuildMoon(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.45f, size * 0.5f);
            Vector2 c2 = new Vector2(size * 0.58f, size * 0.5f);
            float r = size * 0.40f;
            float r2 = size * 0.38f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d1 = Vector2.Distance(new Vector2(x, y), c);
                    float d2 = Vector2.Distance(new Vector2(x, y), c2);
                    if (d1 > r || d2 < r2) { tex.SetPixel(x, y, Color.clear); continue; }
                    Color col = Color.Lerp(hi, lo, d1 / r);
                    tex.SetPixel(x, y, col);
                }
            }
            return Finish(tex);
        }

        private static Sprite BuildCrossInverted(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            FillSquare(tex, 28, 4, 36, 48, hi);
            FillSquare(tex, 29, 4, 35, 47, lo);
            FillSquare(tex, 14, 32, 50, 40, hi);
            FillSquare(tex, 14, 33, 50, 39, lo);
            return Finish(tex);
        }

        private static Sprite BuildLetterGlyph(char letter, Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            if (letter == 'X')
            {
                for (int i = 0; i < 40; i++)
                {
                    int o = i / 2;
                    tex.SetPixel(14 + o, 12 + o, hi);
                    tex.SetPixel(50 - o, 12 + o, hi);
                    tex.SetPixel(15 + o, 13 + o, lo);
                    tex.SetPixel(49 - o, 13 + o, lo);
                }
            }
            else if (letter == 'O')
            {
                Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
                float rOut = size * 0.28f;
                float rIn = size * 0.16f;
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x, y), c);
                        if (d > rOut || d < rIn) { tex.SetPixel(x, y, Color.clear); continue; }
                        tex.SetPixel(x, y, Color.Lerp(lo, hi, d < (rIn + rOut) * 0.5f ? 1f : 0.4f));
                    }
                }
            }
            else
            {
                for (int y = 14; y <= 48; y++)
                {
                    int top = 48 - (y - 14);
                    int x0 = 16 + (48 - y) / 3;
                    int x1 = 48 - (48 - y) / 3;
                    for (int x = x0; x <= x1; x++)
                    {
                        tex.SetPixel(x, y, hi);
                    }
                }

                for (int x = 16; x <= 48; x++) { tex.SetPixel(x, 14, hi); }
            }

            return Finish(tex);
        }

        private static Sprite BuildHanBrush(Color ink, Color accent, int variant)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            if (variant == 0)
            {
                for (int x = 12; x <= 52; x++) { tex.SetPixel(x, 34, ink); tex.SetPixel(x, 35, ink); }
                for (int y = 14; y <= 50; y++) { tex.SetPixel(32, y, ink); tex.SetPixel(33, y, ink); }
            }
            else if (variant == 1)
            {
                for (int t = 0; t < 20; t++) { tex.SetPixel(20 + t, 40 - t / 2, ink); tex.SetPixel(44 - t, 40 - t / 2, ink); }
                for (int x = 18; x <= 46; x++) tex.SetPixel(x, 22, ink);
            }
            else if (variant == 2)
            {
                FillSquare(tex, 22, 18, 42, 46, ink);
                tex.SetPixel(32, 28, accent);
                for (int x = 26; x <= 38; x++) tex.SetPixel(x, 36, accent);
            }
            else
            {
                for (int y = 16; y <= 48; y++) { tex.SetPixel(20, y, ink); tex.SetPixel(44, y, ink); }
                for (int x = 20; x <= 44; x++) { tex.SetPixel(x, 24, ink); tex.SetPixel(x, 40, ink); }
                tex.SetPixel(32, 32, accent);
            }

            return Finish(tex);
        }

        private static Sprite BuildInfinity(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (x - c.x) / (size * 0.22f);
                    float v = (y - c.y) / (size * 0.18f);
                    float val = u * u * v * v - (u * u + v * v - 1f);
                    if (val > 0.12f) { tex.SetPixel(x, y, Color.clear); continue; }
                    tex.SetPixel(x, y, Color.Lerp(lo, hi, Mathf.Abs(val) * 8f));
                }
            }

            return Finish(tex);
        }

        private static Sprite BuildOmega(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.42f);
            float r = size * 0.30f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), c);
                    if (d < r * 0.55f || d > r) { tex.SetPixel(x, y, Color.clear); continue; }
                    tex.SetPixel(x, y, Color.Lerp(lo, hi, 1f - Mathf.Abs(d - r * 0.78f) / (r * 0.25f)));
                }
            }

            FillSquare(tex, 30, 8, 34, 18, hi);
            return Finish(tex);
        }

        private static Sprite BuildPentagram(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.52f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - c.x;
                    float dy = y - c.y;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float angle = Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;
                    float sector = angle / (Mathf.PI * 2f / 5f);
                    float frac = sector - Mathf.Floor(sector);
                    float limitA = frac < 0.5f ? Mathf.Lerp(size * 0.40f, size * 0.14f, frac * 2f)
                        : Mathf.Lerp(size * 0.14f, size * 0.40f, (frac - 0.5f) * 2f);
                    if (dist > limitA * 1.05f) { tex.SetPixel(x, y, Color.clear); continue; }
                    tex.SetPixel(x, y, Color.Lerp(lo, hi, Mathf.Clamp01(dist / limitA)));
                }
            }

            return Finish(tex);
        }

        private static Sprite BuildWifi(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, 16f);
            tex.SetPixel(32, 14, hi);
            tex.SetPixel(31, 15, hi);
            tex.SetPixel(33, 15, hi);
            for (int ring = 1; ring <= 3; ring++)
            {
                float r = ring * 10f;
                for (float a = 0.2f; a < Mathf.PI - 0.2f; a += 0.08f)
                {
                    int px = Mathf.RoundToInt(c.x + Mathf.Cos(a + Mathf.PI) * r);
                    int py = Mathf.RoundToInt(c.y + Mathf.Sin(a) * r);
                    if (px >= 0 && py >= 0 && px < size && py < size)
                    {
                        tex.SetPixel(px, py, ring == 3 ? hi : Color.Lerp(lo, hi, ring / 3f));
                    }
                }
            }

            return Finish(tex);
        }

        private static Sprite BuildHashtag(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            FillSquare(tex, 18, 22, 22, 42, hi);
            FillSquare(tex, 42, 22, 46, 42, hi);
            FillSquare(tex, 26, 14, 38, 18, hi);
            FillSquare(tex, 26, 46, 38, 50, hi);
            FillSquare(tex, 19, 23, 21, 41, lo);
            FillSquare(tex, 43, 23, 45, 41, lo);
            return Finish(tex);
        }

        private static Sprite BuildSmileMeme()
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float r = size * 0.36f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), c);
                    if (d > r) { tex.SetPixel(x, y, Color.clear); continue; }
                    tex.SetPixel(x, y, new Color(1f, 0.92f, 0.15f, 1f));
                }
            }

            FillSquare(tex, 20, 38, 26, 44, Color.black);
            FillSquare(tex, 38, 38, 44, 44, Color.black);
            for (int x = 18; x <= 46; x++)
            {
                int y = 18 + Mathf.Abs(x - 32) / 4;
                tex.SetPixel(x, y, Color.black);
            }

            return Finish(tex);
        }

        private static Sprite BuildMemeEyes()
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float r = size * 0.34f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), c);
                    if (d > r) { tex.SetPixel(x, y, Color.clear); continue; }
                    tex.SetPixel(x, y, Color.white);
                }
            }

            DrawSpiralEye(tex, 22, 36, 8);
            DrawSpiralEye(tex, 42, 36, 8);
            return Finish(tex);
        }

        private static void DrawSpiralEye(Texture2D tex, int cx, int cy, int radius)
        {
            for (float t = 0; t < 18f; t += 0.35f)
            {
                int px = cx + Mathf.RoundToInt(Mathf.Cos(t) * t * 0.35f);
                int py = cy + Mathf.RoundToInt(Mathf.Sin(t) * t * 0.35f);
                for (int dx = -2; dx <= 2; dx++)
                {
                    for (int dy = -2; dy <= 2; dy++)
                    {
                        int x = px + dx;
                        int y = py + dy;
                        if (x >= 0 && y >= 0 && x < tex.width && y < tex.height)
                        {
                            tex.SetPixel(x, y, Color.black);
                        }
                    }
                }
            }
        }

        private static Sprite BuildRune(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            for (int y = 10; y <= 50; y++) { tex.SetPixel(32, y, hi); tex.SetPixel(33, y, lo); }
            for (int x = 18; x <= 46; x += 2) { tex.SetPixel(x, 48, hi); }
            for (int t = 0; t < 14; t++) { tex.SetPixel(24 + t, 48 - t, hi); tex.SetPixel(40 - t, 48 - t, hi); }
            return Finish(tex);
        }

        private static Sprite BuildFlameTeardrop()
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.55f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - c.x) / (size * 0.22f);
                    float dy = (c.y - y) / (size * 0.45f);
                    float val = dx * dx + dy * dy * 0.6f - 1f;
                    if (val > 0.15f) { tex.SetPixel(x, y, Color.clear); continue; }
                    Color col = Color.Lerp(new Color(1f, 0.35f, 0.05f, 1f), new Color(1f, 0.95f, 0.35f, 1f), dy);
                    tex.SetPixel(x, y, col);
                }
            }

            return Finish(tex);
        }

        private static Sprite BuildMiniWing(Color hi, Color lo)
        {
            const int size = 64;
            Texture2D tex = NewTexture(size);
            for (int y = 20; y <= 44; y++)
            {
                int spread = (y - 20) / 2;
                for (int x = 32 - spread; x <= 32 + spread; x++)
                {
                    tex.SetPixel(x, y, Color.Lerp(lo, hi, (y - 20) / 24f));
                }
            }

            return Finish(tex);
        }

        private static void FillSquare(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            int xMin = Mathf.Min(x0, x1), xMax = Mathf.Max(x0, x1);
            int yMin = Mathf.Min(y0, y1), yMax = Mathf.Max(y0, y1);
            for (int y = yMin; y <= yMax; y++)
                for (int x = xMin; x <= xMax; x++)
                    if (x >= 0 && y >= 0 && x < tex.width && y < tex.height)
                        tex.SetPixel(x, y, c);
        }

        private static Texture2D NewTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            Color[] clear = new Color[size * size];
            for (int i = 0; i < clear.Length; i++)
            {
                clear[i] = Color.clear;
            }

            tex.SetPixels(clear);
            return tex;
        }

        private static Sprite Finish(Texture2D tex)
        {
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
        }
    }
}
