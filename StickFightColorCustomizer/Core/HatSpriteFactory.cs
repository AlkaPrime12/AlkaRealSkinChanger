using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Sprites pixel art para los sombreros (generados en runtime, cacheados por id).
    /// FilterMode.Point para look crujiente. Pivot en (0.5, 0) para apoyarse sobre la cabeza.
    /// </summary>
    public static class HatSpriteFactory
    {
        private const int ArtGeneration = 12;
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static void ClearCache()
        {
            Cache.Clear();
        }

        // Paleta con sombreado para look pixel art
        private static readonly Color Clear   = new Color(0f, 0f, 0f, 0f);
        private static readonly Color Outline = new Color(0.06f, 0.05f, 0.07f, 1f);

        private static readonly Color BlackHi = new Color(0.18f, 0.18f, 0.22f, 1f);
        private static readonly Color BlackLo = new Color(0.08f, 0.08f, 0.1f, 1f);

        private static readonly Color RedHi   = new Color(0.95f, 0.25f, 0.2f, 1f);
        private static readonly Color RedLo   = new Color(0.65f, 0.1f, 0.1f, 1f);

        private static readonly Color BlueHi  = new Color(0.32f, 0.55f, 0.95f, 1f);
        private static readonly Color BlueLo  = new Color(0.16f, 0.32f, 0.7f, 1f);

        private static readonly Color BrownHi = new Color(0.65f, 0.42f, 0.22f, 1f);
        private static readonly Color BrownLo = new Color(0.4f, 0.25f, 0.12f, 1f);

        private static readonly Color GoldHi  = new Color(1f, 0.88f, 0.35f, 1f);
        private static readonly Color GoldLo  = new Color(0.85f, 0.62f, 0.1f, 1f);

        private static readonly Color YellowHi = new Color(1f, 0.95f, 0.35f, 1f);

        private static readonly Color GreenHi = new Color(0.4f, 0.8f, 0.35f, 1f);

        private static readonly Color PurpleHi = new Color(0.55f, 0.35f, 0.85f, 1f);
        private static readonly Color PurpleLo = new Color(0.32f, 0.18f, 0.55f, 1f);

        private static readonly Color OrangeHi = new Color(1f, 0.78f, 0.18f, 1f);
        private static readonly Color OrangeLo = new Color(0.85f, 0.5f, 0.05f, 1f);

        private static readonly Color GrayHi  = new Color(0.55f, 0.55f, 0.6f, 1f);
        private static readonly Color GrayLo  = new Color(0.32f, 0.32f, 0.36f, 1f);

        private static readonly Color White   = new Color(0.96f, 0.96f, 0.96f, 1f);

        public static Sprite GetSprite(string hatId)
        {
            string id = HatCatalog.Normalize(hatId);
            if (id == "none")
            {
                return null;
            }

            // Image-based hats (PNG files on disk): route through the loader.
            if (HatImageLoader.IsImageHat(id))
            {
                Sprite loaded = HatImageLoader.TryGetSprite(id);
                if (loaded != null)
                {
                    return loaded;
                }
                // Fall through to procedural if the PNG isn't present.
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

        public static Sprite GetTopHatSprite()
        {
            return GetSprite("tophat");
        }

        public static HatAttachKind GetAttachKind(string hatId)
        {
            string id = HatCatalog.Normalize(hatId);
            if (id.StartsWith("eye_") || id.StartsWith("visor_") || id.StartsWith("mask_")
                || id.StartsWith("blush_"))
            {
                return HatAttachKind.Face;
            }

            if (id.StartsWith("patch_"))
            {
                return HatAttachKind.Face;
            }

            if (id.StartsWith("brow_"))
            {
                return HatAttachKind.Forehead;
            }

            if (id.StartsWith("mark_"))
            {
                return HatAttachKind.Forehead;
            }

            if (id.StartsWith("ear_") || id.StartsWith("halo_") || id.StartsWith("horn_")
                || id.StartsWith("crown_") || id.StartsWith("hair_") || id.StartsWith("mohawk_")
                || id.StartsWith("antenna_") || id.StartsWith("aura_") || id.StartsWith("feather_")
                || id.StartsWith("spike_"))
            {
                return HatAttachKind.Top;
            }

            if (id == "ac_hood")
            {
                return HatAttachKind.Face;
            }

            return HatAttachKind.Top;
        }

        public static float GetAttachOffsetX(string hatId)
        {
            string id = HatCatalog.Normalize(hatId);
            if (id.StartsWith("patch_"))
            {
                return -0.32f;
            }

            if (HatImageLoader.IsImageHat(id))
            {
                return HatImageLoader.GetOffsetX(id);
            }

            return 0f;
        }

        public static bool CoversFace(string hatId)
        {
            return GetAttachKind(hatId) == HatAttachKind.Face;
        }

        public static float GetAttachYOffset(string hatId)
        {
            HatAttachKind kind = GetAttachKind(hatId);
            if (kind == HatAttachKind.Face)
            {
                return 0.02f;
            }

            if (kind == HatAttachKind.Forehead)
            {
                return 0.22f;
            }

            if (kind == HatAttachKind.Top && hatId.StartsWith("halo_"))
            {
                return 0.38f;
            }

            if (hatId == "chef")
            {
                return 0.34f;
            }

            if (hatId == "dunce")
            {
                return 0.3f;
            }

            if (hatId == "toad_cap")
            {
                return 0.04f;
            }

            if (kind == HatAttachKind.Top)
            {
                return 0.28f;
            }

            return 0f;
        }

        public static float GetWidthScale(string hatId)
        {
            string id = HatCatalog.Normalize(hatId);
            if (HatImageLoader.IsImageHat(id)) { return HatImageLoader.GetWidthFactor(id); }
            if (id.StartsWith("halo_")) { return 1.15f; }
            if (id.StartsWith("horn_")) { return 1.05f; }
            if (id.StartsWith("eye_")) { return 1.14f; }
            if (id.StartsWith("visor_")) { return 1.38f; }
            if (id.StartsWith("mask_")) { return 1.15f; }
            if (id.StartsWith("blush_")) { return 1.2f; }
            if (id.StartsWith("patch_")) { return 1.05f; }
            if (id.StartsWith("brow_")) { return 1.1f; }
            if (id.StartsWith("hair_")) { return 1.08f; }
            if (id.StartsWith("mohawk_")) { return 1f; }
            if (id.StartsWith("antenna_")) { return 1.05f; }
            if (id.StartsWith("aura_")) { return 1.2f; }
            if (id.StartsWith("feather_")) { return 1f; }
            if (id.StartsWith("spike_")) { return 1.12f; }
            if (id.StartsWith("crown_")) { return 1.08f; }
            if (id.StartsWith("ear_")) { return 0.95f; }
            if (id.StartsWith("mark_")) { return 0.88f; }

            switch (id)
            {
                case "cowboy":   return 1.22f;
                case "wizard":   return 1f;
                case "crown":    return 1.05f;
                case "horns":    return 1.08f;
                case "bandana":  return 1.1f;
                case "ac_hood":  return 1.12f;
                case "toad_cap": return 1.18f;
                case "chef":     return 1.06f;
                case "dunce":    return 1.02f;
                default:         return 1f;
            }
        }

        public static float GetHeightScale(string hatId)
        {
            string id = HatCatalog.Normalize(hatId);
            if (HatImageLoader.IsImageHat(id)) { return HatImageLoader.GetHeightFactor(id); }
            if (id.StartsWith("halo_")) { return 0.95f; }
            if (id.StartsWith("horn_")) { return 1.05f; }
            if (id.StartsWith("eye_")) { return 0.82f; }
            if (id.StartsWith("visor_")) { return 0.98f; }
            if (id.StartsWith("mask_")) { return 0.88f; }
            if (id.StartsWith("blush_")) { return 0.78f; }
            if (id.StartsWith("patch_")) { return 0.85f; }
            if (id.StartsWith("brow_")) { return 0.7f; }
            if (id.StartsWith("hair_")) { return 1f; }
            if (id.StartsWith("mohawk_")) { return 1.08f; }
            if (id.StartsWith("antenna_")) { return 1.05f; }
            if (id.StartsWith("aura_")) { return 0.9f; }
            if (id.StartsWith("feather_")) { return 1.02f; }
            if (id.StartsWith("spike_")) { return 0.85f; }
            if (id.StartsWith("crown_")) { return 0.92f; }
            if (id.StartsWith("ear_")) { return 1f; }
            if (id.StartsWith("mark_")) { return 0.82f; }

            switch (id)
            {
                case "wizard":   return 1.12f;
                case "cone":     return 1.1f;
                case "tophat":   return 1.1f;
                case "link_cap": return 1.18f;
                case "ac_hood":  return 1.05f;
                case "chef":     return 1.22f;
                case "dunce":    return 1.18f;
                default:         return 1f;
            }
        }

        private static Sprite BuildSprite(string id)
        {
            switch (id)
            {
                case "tophat":    return BuildTopHat();
                case "cap":       return BuildCap();
                case "beanie":    return BuildBeanie();
                case "cowboy":    return BuildCowboy();
                case "cone":      return BuildPartyCone();
                case "crown":     return BuildCrown();
                case "wizard":    return BuildWizard();
                case "hardhat":   return BuildHardHat();
                case "bandana":   return BuildBandana();
                case "propeller": return BuildPropeller();
                case "horns":     return BuildHorns();
                case "mario_cap": return BuildMarioCap();
                case "link_cap":  return BuildLinkCap();
                case "ash_cap":   return BuildAshCap();
                case "ac_hood":   return BuildAssassinHood();
                case "toad_cap":  return BuildToadCap();
                case "chef":      return BuildChefToque();
                case "dunce":     return BuildDunceCap();
                default:
                    if (HatCategoryCatalog.IsVariant(id))
                    {
                        return HatSpriteFactorySimple.Build(id);
                    }

                    return BuildTopHat();
            }
        }

        // ---------------- Builders ----------------

        private static Sprite BuildTopHat()
        {
            const int w = 40, h = 34;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            FillEllipse(tex, mid, 5, 17f, 5f, BlackLo);
            FillEllipse(tex, mid, 6, 17f, 4f, BlackHi);
            FillRect(tex, 12, 7, w - 13, 28, BlackLo);
            FillRect(tex, 13, 8, w - 14, 27, BlackHi);
            FillRect(tex, 13, 26, w - 14, 27, BlackLo);
            FillRect(tex, 11, 10, w - 12, 12, RedLo);
            FillRect(tex, 11, 11, w - 12, 11, RedHi);
            FillRect(tex, 15, 14, 16, 22, new Color(0.35f, 0.08f, 0.08f, 1f));
            tex.SetPixel(17, 18, GoldHi);
            tex.SetPixel(22, 18, GoldHi);
            DrawEllipseOutline(tex, mid, 5, 17f, 5f);
            DrawOutline(tex, 12, 7, w - 13, 28);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
        }

        private static Sprite BuildCap()
        {
            const int w = 36, h = 22;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            FillEllipse(tex, mid, 6, 15f, 5f, BlueLo);
            FillEllipse(tex, mid, 7, 15f, 4f, BlueHi);
            FillRect(tex, 9, 7, w - 10, 16, BlueLo);
            FillRect(tex, 10, 8, w - 11, 15, BlueHi);
            FillRect(tex, 10, 8, 14, 14, new Color(0.12f, 0.22f, 0.45f, 1f));
            FillRect(tex, w - 15, 8, w - 11, 14, new Color(0.2f, 0.38f, 0.72f, 1f));
            FillRect(tex, 14, 11, 22, 13, White);
            FillRect(tex, 15, 12, 21, 12, new Color(0.85f, 0.12f, 0.15f, 1f));
            FillEllipse(tex, mid, 15, 14f, 4f, BlueLo);
            FillEllipse(tex, mid, 16, 14f, 3f, BlueHi);
            DrawEllipseOutline(tex, mid, 6, 15f, 5f);
            DrawOutline(tex, 9, 7, w - 10, 16);
            return Finish(tex, w, h, new Vector2(0.5f, 0.12f));
        }

        private static Sprite BuildBeanie()
        {
            const int w = 30, h = 26;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            FillRect(tex, 4, 3, w - 5, 7, new Color(0.45f, 0.05f, 0.08f, 1f));
            FillRect(tex, 4, 6, w - 5, 7, RedLo);
            FillRect(tex, 5, 7, w - 6, 7, RedHi);
            for (int x = 6; x < w - 6; x += 3)
            {
                tex.SetPixel(x, 6, new Color(0.55f, 0.08f, 0.12f, 1f));
            }
            FillEllipse(tex, mid, 15, 11f, 10f, RedLo);
            FillEllipse(tex, mid, 14, 10f, 9f, RedHi);
            FillEllipse(tex, mid, 23, 3.5f, 3.5f, White);
            FillEllipse(tex, mid, 22, 2.5f, 2.5f, new Color(0.92f, 0.92f, 0.95f, 1f));
            DrawOutline(tex, 4, 3, w - 5, 7);
            DrawEllipseOutline(tex, mid, 15, 11f, 10f);
            return Finish(tex, w, h, new Vector2(0.5f, 0.1f));
        }

        private static Sprite BuildCowboy()
        {
            const int w = 36, h = 18;
            Texture2D tex = NewTex(w, h);
            FillEllipse(tex, w / 2f, 5, 16f, 4f, BrownLo);
            FillEllipse(tex, w / 2f, 6, 16f, 3f, BrownHi);
            FillRect(tex, 11, 7, w - 12, 15, BrownLo);
            FillRect(tex, 11, 14, w - 12, 15, BrownHi);
            FillRect(tex, 11, 9, w - 12, 11, new Color(0.28f, 0.16f, 0.06f, 1f));
            DrawEllipseOutline(tex, w / 2f, 5, 16f, 4f);
            DrawOutline(tex, 11, 7, w - 12, 15);
            return Finish(tex, w, h, new Vector2(0.5f, 0.18f));
        }

        private static Sprite BuildPartyCone()
        {
            const int w = 28, h = 34;
            Texture2D tex = NewTex(w, h);
            float midX = w / 2f;
            for (int y = 2; y < h - 2; y++)
            {
                float t = (y - 2) / (float)(h - 4);
                int half = Mathf.Max(1, Mathf.RoundToInt((1f - t) * (w / 2f - 2f)));
                int band = (h - 2 - y) / 3;
                Color c = band % 2 == 0 ? YellowHi : RedHi;
                Color shade = band % 2 == 0 ? GoldLo : RedLo;
                int xMid = Mathf.RoundToInt(midX);
                FillRect(tex, xMid - half, y, xMid + half, y, c);
                if (half > 0)
                {
                    tex.SetPixel(xMid - half, y, shade);
                    tex.SetPixel(xMid + half, y, Outline);
                }
            }
            FillEllipse(tex, midX, h - 2, 3.5f, 3.5f, White);
            FillEllipse(tex, midX, h - 3, 2.5f, 2.5f, new Color(0.9f, 0.9f, 0.92f, 1f));
            tex.SetPixel(6, 4, BlueHi);
            tex.SetPixel(w - 7, 4, BlueHi);
            DrawTriangleOutline(tex, midX, h - 2, 2, Mathf.RoundToInt(midX) - 2, h - 4);
            return Finish(tex, w, h, new Vector2(0.5f, 0.05f));
        }

        private static Sprite BuildCrown()
        {
            const int w = 34, h = 18;
            Texture2D tex = NewTex(w, h);
            // Banda dorada fina (sin bloque violeta gigante)
            FillRect(tex, 4, 2, w - 5, 4, GoldLo);
            FillRect(tex, 4, 4, w - 5, 5, GoldHi);
            FillRect(tex, 5, 3, w - 6, 3, new Color(0.42f, 0.1f, 0.48f, 1f));
            int[] tipsX = { 6, 12, 17, 22, 28 };
            for (int i = 0; i < tipsX.Length; i++)
            {
                int cx = tipsX[i];
                int half = i == 2 ? 2 : 2;
                int spikeH = i == 2 ? 9 : 8;
                FillTriangleUp(tex, cx, 5, half, spikeH, GoldHi);
                tex.SetPixel(cx, 5 + spikeH, YellowHi);
                tex.SetPixel(cx - 1, 5 + spikeH - 1, GoldLo);
                tex.SetPixel(cx + 1, 5 + spikeH - 1, GoldLo);
            }
            tex.SetPixel(9, 4, RedHi);
            tex.SetPixel(17, 4, BlueHi);
            tex.SetPixel(24, 4, GreenHi);
            tex.SetPixel(14, 4, YellowHi);
            DrawOutline(tex, 4, 2, w - 5, 5);
            return Finish(tex, w, h, new Vector2(0.5f, 0.08f));
        }

        private static Sprite BuildWizard()
        {
            const int w = 34, h = 44;
            Texture2D tex = NewTex(w, h);
            float midX = w / 2f;
            FillEllipse(tex, midX, 6, 14f, 4f, PurpleLo);
            FillEllipse(tex, midX, 7, 14f, 3f, PurpleHi);
            for (int y = 8; y < h - 2; y++)
            {
                float t = (y - 8) / (float)(h - 10);
                int half = Mathf.Max(1, Mathf.RoundToInt((1f - t) * (w / 2f - 4f)));
                int bend = Mathf.RoundToInt(Mathf.Sin(t * 3.2f) * 2f);
                int xMid = Mathf.RoundToInt(midX) + bend;
                FillRect(tex, xMid - half, y, xMid + half, y, PurpleHi);
                if (half > 1)
                {
                    tex.SetPixel(xMid - half, y, PurpleLo);
                    tex.SetPixel(xMid + half, y, new Color(0.22f, 0.1f, 0.38f, 1f));
                }
            }
            tex.SetPixel(12, 20, YellowHi);
            tex.SetPixel(20, 28, YellowHi);
            tex.SetPixel(16, 36, YellowHi);
            tex.SetPixel(18, 14, YellowHi);
            DrawEllipseOutline(tex, midX, 6, 14f, 4f);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
        }

        private static Sprite BuildHardHat()
        {
            const int w = 36, h = 22;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            FillEllipse(tex, mid, 7, 15f, 5f, OrangeLo);
            FillEllipse(tex, mid, 8, 15f, 4f, OrangeHi);
            FillRect(tex, 8, 8, w - 9, 15, OrangeLo);
            FillRect(tex, 9, 9, w - 10, 14, OrangeHi);
            FillRect(tex, 10, 10, w - 11, 11, new Color(0.95f, 0.75f, 0.1f, 1f));
            FillRect(tex, 14, 12, 22, 12, GrayHi);
            FillRect(tex, 15, 13, 21, 13, White);
            FillRect(tex, 16, 14, 17, 14, BlackLo);
            DrawEllipseOutline(tex, mid, 7, 15f, 5f);
            DrawOutline(tex, 8, 8, w - 9, 15);
            return Finish(tex, w, h, new Vector2(0.5f, 0.14f));
        }

        private static Sprite BuildBandana()
        {
            const int w = 38, h = 18;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 4, 5, w - 5, 14, RedLo);
            FillRect(tex, 4, 10, w - 5, 14, RedHi);
            for (int x = 6; x < w - 6; x += 5)
            {
                tex.SetPixel(x, 8, White);
                tex.SetPixel(x + 1, 11, new Color(0.75f, 0.75f, 0.8f, 1f));
                tex.SetPixel(x + 2, 9, BlueHi);
            }
            FillTriangleDown(tex, w - 4, 10, 4, 7, RedHi);
            FillTriangleDown(tex, 4, 10, 4, 7, RedLo);
            FillRect(tex, w - 6, 6, w - 4, 9, RedHi);
            DrawOutline(tex, 4, 5, w - 5, 14);
            return Finish(tex, w, h, new Vector2(0.5f, 0.28f));
        }

        private static Sprite BuildPropeller()
        {
            const int w = 32, h = 32;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            FillEllipse(tex, mid, 10, 11f, 9f, BlueLo);
            FillEllipse(tex, mid, 10, 11f, 8f, BlueHi);
            FillRect(tex, 11, 15, 21, 18, YellowHi);
            FillRect(tex, 2, 19, w - 3, 20, RedHi);
            FillRect(tex, 2, 18, w - 3, 19, RedLo);
            FillRect(tex, 2, 20, w - 3, 21, YellowHi);
            int midI = w / 2;
            FillRect(tex, midI - 1, 21, midI + 1, 28, BlackHi);
            FillEllipse(tex, mid, 28, 2f, 2f, RedHi);
            DrawEllipseOutline(tex, mid, 10, 11f, 9f);
            return Finish(tex, w, h, new Vector2(0.5f, 0.1f));
        }

        private static Sprite BuildHorns()
        {
            const int w = 34, h = 22;
            Texture2D tex = NewTex(w, h);
            FillRect(tex, 8, 2, w - 9, 4, GrayLo);
            FillRect(tex, 8, 4, w - 9, 5, GrayHi);
            DrawCurvedHorn(tex, 13, 5, -1, RedLo, RedHi);
            DrawCurvedHorn(tex, w - 14, 5, +1, RedLo, RedHi);
            DrawOutline(tex, 8, 2, w - 9, 5);
            return Finish(tex, w, h, new Vector2(0.5f, 0.1f));
        }

        private static Sprite BuildMarioCap()
        {
            const int w = 34, h = 22;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            // Visera (overflow inferior)
            FillEllipse(tex, mid, 6, 14f, 4f, RedLo);
            FillEllipse(tex, mid, 7, 14f, 3f, RedHi);
            // Cuerpo de la gorra (cúpula redondeada)
            FillEllipse(tex, mid, 13, 10f, 8f, RedLo);
            FillEllipse(tex, mid, 13, 9f, 7f, RedHi);
            FillRect(tex, 8, 8, w - 9, 13, RedHi);
            FillRect(tex, 8, 8, w - 9, 9, RedLo);
            // Círculo blanco frontal con "M" sugerida
            FillEllipse(tex, mid, 14, 4.5f, 4.5f, White);
            FillEllipse(tex, mid, 14, 3.5f, 3.5f, new Color(0.98f, 0.98f, 1f, 1f));
            tex.SetPixel(Mathf.RoundToInt(mid) - 2, 13, RedHi);
            tex.SetPixel(Mathf.RoundToInt(mid) - 1, 14, RedHi);
            tex.SetPixel(Mathf.RoundToInt(mid),     13, RedHi);
            tex.SetPixel(Mathf.RoundToInt(mid) + 1, 14, RedHi);
            tex.SetPixel(Mathf.RoundToInt(mid) + 2, 13, RedHi);
            tex.SetPixel(Mathf.RoundToInt(mid) - 2, 15, RedHi);
            tex.SetPixel(Mathf.RoundToInt(mid) + 2, 15, RedHi);
            DrawEllipseOutline(tex, mid, 6, 14f, 4f);
            DrawEllipseOutline(tex, mid, 13, 10f, 8f);
            return Finish(tex, w, h, new Vector2(0.5f, 0.1f));
        }

        private static Sprite BuildLinkCap()
        {
            const int w = 26, h = 32;
            Texture2D tex = NewTex(w, h);
            float midX = w / 2f;
            Color greenHi = new Color(0.32f, 0.7f, 0.3f, 1f);
            Color greenLo = new Color(0.16f, 0.45f, 0.15f, 1f);
            Color greenDark = new Color(0.1f, 0.3f, 0.1f, 1f);
            // Base ancha sobre la cabeza
            FillRect(tex, 3, 2, w - 4, 5, greenLo);
            FillRect(tex, 3, 5, w - 4, 6, greenHi);
            // Cono que cae hacia un lado (estilo Link)
            for (int y = 7; y < h - 1; y++)
            {
                float t = (y - 7) / (float)(h - 8);
                int half = Mathf.Max(1, Mathf.RoundToInt((1f - t) * (w / 2f - 2f)));
                // Inclinación: que la punta caiga hacia la izquierda
                int bend = Mathf.RoundToInt(-t * 5f);
                int xMid = Mathf.RoundToInt(midX) + bend;
                FillRect(tex, xMid - half, y, xMid + half, y, greenHi);
                if (half > 0)
                {
                    tex.SetPixel(xMid - half, y, greenLo);
                    tex.SetPixel(xMid + half, y, greenDark);
                }
            }
            DrawOutline(tex, 3, 2, w - 4, 6);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
        }

        private static Sprite BuildAshCap()
        {
            const int w = 34, h = 20;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            // Visera blanca recta hacia adelante
            FillEllipse(tex, mid + 3, 5, 13f, 3f, new Color(0.9f, 0.9f, 0.93f, 1f));
            FillEllipse(tex, mid + 3, 6, 13f, 2f, White);
            // Parte trasera/lateral roja
            FillEllipse(tex, mid, 12, 11f, 7f, RedLo);
            FillEllipse(tex, mid, 12, 10f, 6f, RedHi);
            FillRect(tex, 7, 7, w - 8, 12, RedHi);
            FillRect(tex, 7, 7, w - 8, 8, RedLo);
            // Pokeball: semicírculo verde mar / blanco con punto
            int cx = Mathf.RoundToInt(mid);
            FillEllipse(tex, cx, 13, 3.5f, 3.5f, White);
            FillRect(tex, cx - 3, 13, cx + 3, 13, new Color(0.12f, 0.12f, 0.15f, 1f));
            FillEllipse(tex, cx, 14, 3.5f, 2f, new Color(0.2f, 0.6f, 0.85f, 1f));
            tex.SetPixel(cx, 13, BlackLo);
            DrawEllipseOutline(tex, mid + 3, 5, 13f, 3f);
            DrawEllipseOutline(tex, mid, 12, 11f, 7f);
            return Finish(tex, w, h, new Vector2(0.5f, 0.12f));
        }

        private static Sprite BuildAssassinHood()
        {
            const int w = 32, h = 30;
            Texture2D tex = NewTex(w, h);
            float midX = w / 2f;
            Color hoodHi = new Color(0.92f, 0.9f, 0.85f, 1f);
            Color hoodLo = new Color(0.65f, 0.62f, 0.55f, 1f);
            Color hoodDark = new Color(0.4f, 0.38f, 0.32f, 1f);
            // Cuerpo de capucha (cubre cabeza)
            FillEllipse(tex, midX, 10, 14f, 9f, hoodLo);
            FillEllipse(tex, midX, 11, 13f, 8f, hoodHi);
            // Punta caída hacia adelante (lado izquierdo)
            for (int i = 0; i < 12; i++)
            {
                float t = i / 11f;
                int x = Mathf.RoundToInt(midX - 8 - t * 6f);
                int y = Mathf.RoundToInt(16 + t * 8f);
                int half = Mathf.Max(1, Mathf.RoundToInt((1f - t) * 3f));
                FillRect(tex, x - half, y - 1, x + half, y + 1, hoodHi);
                tex.SetPixel(x - half, y, hoodLo);
                tex.SetPixel(x + half, y, hoodDark);
            }
            // Sombra interior (cara oscura)
            FillEllipse(tex, midX, 8, 5f, 4f, new Color(0.15f, 0.13f, 0.1f, 1f));
            // Hebilla roja
            tex.SetPixel(Mathf.RoundToInt(midX) - 6, 4, RedHi);
            tex.SetPixel(Mathf.RoundToInt(midX) - 5, 4, RedLo);
            DrawEllipseOutline(tex, midX, 10, 14f, 9f);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
        }

        private static Sprite BuildToadCap()
        {
            const int w = 38, h = 26;
            Texture2D tex = NewTex(w, h);
            float midX = w / 2f;
            // Borde inferior (banda que apoya en la cabeza — pivot en y≈0)
            FillRect(tex, 5, 0, w - 6, 2, new Color(0.72f, 0.7f, 0.72f, 1f));
            FillRect(tex, 6, 2, w - 7, 5, new Color(0.8f, 0.78f, 0.78f, 1f));
            FillRect(tex, 7, 4, w - 8, 5, White);
            // Cúpula gigante tipo hongo
            FillEllipse(tex, midX, 14, 17f, 11f, new Color(0.92f, 0.92f, 0.95f, 1f));
            FillEllipse(tex, midX, 14, 16f, 10f, White);
            // Lunares rojos: 1 central grande + 2 laterales
            FillEllipse(tex, midX, 17, 5.5f, 5.5f, RedLo);
            FillEllipse(tex, midX, 17, 4.5f, 4.5f, RedHi);
            FillEllipse(tex, midX - 10, 13, 3.5f, 3.5f, RedLo);
            FillEllipse(tex, midX - 10, 13, 2.5f, 2.5f, RedHi);
            FillEllipse(tex, midX + 10, 13, 3.5f, 3.5f, RedLo);
            FillEllipse(tex, midX + 10, 13, 2.5f, 2.5f, RedHi);
            DrawEllipseOutline(tex, midX, 14, 17f, 11f);
            DrawOutline(tex, 5, 0, w - 6, 5);
            return Finish(tex, w, h, new Vector2(0.5f, 0.03f));
        }

        /// <summary>Gorro de chef clásico: pliegues, volumen y brillo.</summary>
        private static Sprite BuildChefToque()
        {
            const int w = 30, h = 36;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color whiteHi = new Color(0.98f, 0.98f, 1f, 1f);
            Color whiteLo = new Color(0.82f, 0.84f, 0.88f, 1f);
            Color fold = new Color(0.68f, 0.7f, 0.76f, 1f);

            FillEllipse(tex, mid, 6, 12f, 4f, whiteLo);
            FillEllipse(tex, mid, 7, 11f, 3f, whiteHi);
            for (int y = 8; y < h - 3; y++)
            {
                float t = (y - 8) / (float)(h - 11);
                int half = Mathf.Max(2, Mathf.RoundToInt((1f - t * 0.35f) * 11f));
                FillRect(tex, Mathf.RoundToInt(mid) - half, y, Mathf.RoundToInt(mid) + half, y, y % 3 == 0 ? fold : whiteLo);
                FillRect(tex, Mathf.RoundToInt(mid) - half + 1, y, Mathf.RoundToInt(mid) + half - 1, y, whiteHi);
            }

            for (int x = 9; x < w - 9; x += 3)
            {
                FillRect(tex, x, 10, x, h - 5, fold);
                tex.SetPixel(x + 1, 12, whiteHi);
            }

            FillEllipse(tex, mid, h - 4, 13f, 3f, whiteLo);
            FillEllipse(tex, mid, h - 5, 12f, 2f, whiteHi);
            tex.SetPixel(Mathf.RoundToInt(mid) - 4, 14, new Color(1f, 1f, 1f, 0.8f));
            tex.SetPixel(Mathf.RoundToInt(mid) + 3, 18, new Color(1f, 1f, 1f, 0.55f));
            DrawEllipseOutline(tex, mid, 8, 12f, 12f);
            DrawOutline(tex, 7, 6, w - 8, h - 4);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
        }

        /// <summary>Cono de burro: clásico, salado, franjas y letra D.</summary>
        private static Sprite BuildDunceCap()
        {
            const int w = 26, h = 32;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color yellowHi = new Color(1f, 0.92f, 0.35f, 1f);
            Color yellowLo = new Color(0.85f, 0.68f, 0.12f, 1f);
            Color band = new Color(0.12f, 0.1f, 0.14f, 1f);

            for (int y = 3; y < h - 2; y++)
            {
                float t = (y - 3) / (float)(h - 5);
                int half = Mathf.Max(1, Mathf.RoundToInt((1f - t) * (w / 2f - 1f)));
                bool isBand = y == 8 || y == 14 || y == 20;
                Color c = isBand ? band : (y % 2 == 0 ? yellowHi : yellowLo);
                FillRect(tex, Mathf.RoundToInt(mid) - half, y, Mathf.RoundToInt(mid) + half, y, c);
                if (!isBand && half > 1)
                {
                    tex.SetPixel(Mathf.RoundToInt(mid) - half + 1, y, yellowHi);
                    tex.SetPixel(Mathf.RoundToInt(mid) + half - 1, y, yellowLo);
                }
            }

            FillEllipse(tex, mid, h - 2, 4f, 2.5f, band);
            FillRect(tex, Mathf.RoundToInt(mid) - 2, 11, Mathf.RoundToInt(mid) + 2, 16, band);
            tex.SetPixel(Mathf.RoundToInt(mid) - 1, 12, yellowHi);
            tex.SetPixel(Mathf.RoundToInt(mid) + 1, 12, yellowHi);
            tex.SetPixel(Mathf.RoundToInt(mid), 13, yellowHi);
            tex.SetPixel(Mathf.RoundToInt(mid) - 1, 14, yellowHi);
            tex.SetPixel(Mathf.RoundToInt(mid) + 1, 14, yellowHi);
            tex.SetPixel(Mathf.RoundToInt(mid), 15, yellowHi);
            DrawTriangleOutline(tex, mid, h - 2, 2, Mathf.RoundToInt(mid) - 1, 4);
            return Finish(tex, w, h, new Vector2(0.5f, 0.05f));
        }

        // ---------------- Helpers ----------------

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

        private static Sprite Finish(Texture2D tex, int w, int h, Vector2 pivot)
        {
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            return Sprite.Create(tex, new Rect(0f, 0f, w, h), pivot, 24f);
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
                    if (x < 0 || y < 0 || x >= tex.width || y >= tex.height) continue;
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

        private static void FillTriangleDown(Texture2D tex, int cx, int yTop, int halfW, int height, Color color)
        {
            for (int y = 0; y < height; y++)
            {
                float t = y / (float)height;
                int half = Mathf.Max(0, Mathf.RoundToInt(halfW * (1f - t)));
                FillRect(tex, cx - half, yTop - y, cx + half, yTop - y, color);
            }
        }

        private static void DrawCurvedHorn(Texture2D tex, int rootX, int rootY, int dir, Color shade, Color hi)
        {
            const int steps = 22;
            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)(steps - 1);
                int halfThick = Mathf.Max(1, Mathf.RoundToInt((1f - t) * 2.5f));
                int x = rootX + Mathf.RoundToInt(dir * (2f + t * 10f + Mathf.Sin(t * 2.8f) * 1.2f));
                int y = rootY + Mathf.RoundToInt(t * 14f + t * t * 1.5f);
                FillRect(tex, x - halfThick, y - 1, x + halfThick, y + 1, shade);
                if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                {
                    tex.SetPixel(x, y, hi);
                }
            }

            int tipX = rootX + Mathf.RoundToInt(dir * 12f);
            int tipY = rootY + 15;
            if (tipX >= 0 && tipX < tex.width && tipY >= 0 && tipY < tex.height)
            {
                tex.SetPixel(tipX, tipY, hi);
                SetIfInside(tex, tipX + dir, tipY, Outline);
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

        private static void DrawEllipseOutline(Texture2D tex, float cx, float cy, float rx, float ry)
        {
            int steps = 64;
            for (int i = 0; i < steps; i++)
            {
                float a = (i / (float)steps) * Mathf.PI * 2f;
                int x = Mathf.RoundToInt(cx + Mathf.Cos(a) * (rx + 0.6f));
                int y = Mathf.RoundToInt(cy + Mathf.Sin(a) * (ry + 0.6f));
                SetIfInside(tex, x, y, Outline);
            }
        }

        private static void DrawTriangleOutline(Texture2D tex, float topX, float topY, int yBaseFromTop, float halfW, float baseY)
        {
            for (int i = 0; i <= 24; i++)
            {
                float t = i / 24f;
                int xL = Mathf.RoundToInt(topX - halfW * t);
                int xR = Mathf.RoundToInt(topX + halfW * t);
                int y = Mathf.RoundToInt(topY - (topY - baseY) * t);
                SetIfInside(tex, xL - 1, y, Outline);
                SetIfInside(tex, xR + 1, y, Outline);
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
