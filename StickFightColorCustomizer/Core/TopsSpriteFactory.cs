using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Tops textures (used as a 1-D material on the spine LineRenderer). To compensate for
    /// the flat tube look, every fabric paint now uses a horizontal cylinder gradient:
    /// dark at the edges, bright specular highlight in the centre column. That fakes a
    /// rounded torso when the texture is wrapped around a line.
    /// </summary>
    public static class TopsSpriteFactory
    {
        private const int ArtGeneration = 18;
        private const float PixelsPerUnit = 40f;
        private static readonly Vector2 ChestPivot = new Vector2(0.5f, 0.42f);
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        // ── Palette ──────────────────────────────────────────────────────────────
        private static readonly Color Clear     = new Color(0f, 0f, 0f, 0f);

        private static readonly Color RedHi     = new Color(1.00f, 0.32f, 0.22f, 1f);
        private static readonly Color RedMid    = new Color(0.86f, 0.18f, 0.14f, 1f);
        private static readonly Color RedLo     = new Color(0.46f, 0.06f, 0.06f, 1f);

        private static readonly Color BlueHi    = new Color(0.42f, 0.66f, 1.00f, 1f);
        private static readonly Color BlueMid   = new Color(0.22f, 0.42f, 0.86f, 1f);
        private static readonly Color BlueLo    = new Color(0.06f, 0.18f, 0.46f, 1f);

        private static readonly Color GrayHi    = new Color(0.78f, 0.78f, 0.82f, 1f);
        private static readonly Color GrayMid   = new Color(0.50f, 0.50f, 0.54f, 1f);
        private static readonly Color GrayLo    = new Color(0.20f, 0.20f, 0.24f, 1f);

        private static readonly Color WhiteHi   = new Color(1.00f, 1.00f, 1.00f, 1f);
        private static readonly Color WhiteLo   = new Color(0.70f, 0.70f, 0.74f, 1f);

        private static readonly Color BlackHi   = new Color(0.28f, 0.28f, 0.32f, 1f);
        private static readonly Color BlackLo   = new Color(0.06f, 0.06f, 0.08f, 1f);

        private static readonly Color BrownHi   = new Color(0.74f, 0.50f, 0.28f, 1f);
        private static readonly Color BrownMid  = new Color(0.50f, 0.32f, 0.16f, 1f);
        private static readonly Color BrownLo   = new Color(0.20f, 0.12f, 0.04f, 1f);

        private static readonly Color DenimHi   = new Color(0.46f, 0.62f, 0.84f, 1f);
        private static readonly Color DenimMid  = new Color(0.28f, 0.42f, 0.62f, 1f);
        private static readonly Color DenimLo   = new Color(0.10f, 0.18f, 0.34f, 1f);

        private static readonly Color GoldHi    = new Color(1.00f, 0.94f, 0.50f, 1f);
        private static readonly Color GoldMid   = new Color(0.92f, 0.72f, 0.18f, 1f);
        private static readonly Color GoldLo    = new Color(0.46f, 0.30f, 0.05f, 1f);

        private static readonly Color YellowHi  = new Color(1.00f, 0.96f, 0.34f, 1f);
        private static readonly Color OrangeHi  = new Color(1.00f, 0.62f, 0.18f, 1f);
        private static readonly Color GreenHi   = new Color(0.30f, 0.92f, 0.36f, 1f);
        private static readonly Color GreenLo   = new Color(0.08f, 0.40f, 0.14f, 1f);
        private static readonly Color MagentaHi = new Color(1.00f, 0.34f, 0.86f, 1f);
        private static readonly Color CyanHi    = new Color(0.30f, 0.95f, 1.00f, 1f);
        private static readonly Color CyanLo    = new Color(0.08f, 0.42f, 0.62f, 1f);
        private static readonly Color PurpleHi  = new Color(0.80f, 0.45f, 1.00f, 1f);

        public static Sprite GetSprite(string topsId)
        {
            string id = TopsCatalog.Normalize(topsId);
            if (id == "none") return null;

            string key = id + "#" + ArtGeneration;
            Sprite sprite;
            if (Cache.TryGetValue(key, out sprite) && sprite != null) return sprite;
            sprite = BuildSprite(id);
            Cache[key] = sprite;
            return sprite;
        }

        public static Texture2D GetTexture(string topsId)
        {
            Sprite s = GetSprite(topsId);
            return s != null ? s.texture : null;
        }

        public static float GetWidthScale(string topsId)  { return 1f; }
        public static float GetHeightScale(string topsId) { return 1f; }

        public static Color GetLineTint(string topsId)
        {
            switch (TopsCatalog.Normalize(topsId))
            {
                case "hoodie":      return BlueLo;
                case "jacket":      return DenimMid;
                case "tank":        return GrayMid;
                case "dress_shirt": return WhiteHi;
                case "jersey":      return BlueMid;
                case "vest":        return BrownMid;
                case "armor_gold":  return GoldMid;
                case "tuxedo":      return BlackHi;
                case "clown":       return RedHi;
                case "neon":        return CyanHi;
                case "varsity":     return RedMid;
                default:            return RedMid;
            }
        }

        public static float GetLineWidthMultiplier(string topsId) { return 1.28f; }

        private static Sprite BuildSprite(string id)
        {
            switch (id)
            {
                case "tshirt":      return BuildShaded(RedHi,    RedMid,    RedLo,    Accent.None);
                case "hoodie":      return BuildShaded(BlueHi,   BlueMid,   BlueLo,   Accent.Hoodie);
                case "jacket":      return BuildShaded(DenimHi,  DenimMid,  DenimLo,  Accent.Zipper);
                case "tank":        return BuildShaded(GrayHi,   GrayMid,   GrayLo,   Accent.TankStraps);
                case "dress_shirt": return BuildShaded(WhiteHi,  WhiteLo,   GrayLo,   Accent.Tie);
                case "jersey":      return BuildShaded(BlueHi,   BlueMid,   BlueLo,   Accent.JerseyStripes);
                case "vest":        return BuildShaded(BrownHi,  BrownMid,  BrownLo,  Accent.Buttons);
                case "armor_gold":  return BuildShaded(GoldHi,   GoldMid,   GoldLo,   Accent.ArmorPlates);
                case "tuxedo":      return BuildShaded(BlackHi,  BlackLo,   BlackLo,  Accent.TuxLapels);
                case "clown":       return BuildShaded(RedHi,    RedMid,    RedLo,    Accent.ClownDots);
                case "neon":        return BuildShaded(CyanHi,   CyanLo,    BlackLo,  Accent.NeonGrid);
                case "varsity":     return BuildShaded(RedHi,    RedMid,    RedLo,    Accent.VarsityCollar);
                default:            return BuildShaded(RedHi,    RedMid,    RedLo,    Accent.None);
            }
        }

        private enum Accent
        {
            None, Hoodie, Zipper, TankStraps, Tie, JerseyStripes, Buttons,
            ArmorPlates, TuxLapels, ClownDots, NeonGrid, VarsityCollar
        }

        // ── Core builder: cylindrical shading + decorative accent layer ──────────
        private const int W = 16;
        private const int H = 72;

        private static Sprite BuildShaded(Color hi, Color mid, Color lo, Accent accent)
        {
            Texture2D tex = NewTex(W, H);

            // Body silhouette with cylindrical shading.
            // Per column, blend between lo (edges) → mid → hi (centre) → mid → lo.
            for (int y = 6; y <= 60; y++)
            {
                for (int x = 2; x <= 13; x++)
                {
                    // 0 at the centre column, 1 at the outer edges.
                    float u = Mathf.Abs((x - 7.5f) / 5.5f);
                    Color col;
                    if (u < 0.35f)        col = Color.Lerp(hi, mid, u / 0.35f);
                    else if (u < 0.85f)   col = Color.Lerp(mid, lo, (u - 0.35f) / 0.5f);
                    else                  col = lo;

                    // Vertical tonal variation: slight bottom darken, slight top highlight.
                    float v = (y - 6f) / 54f;
                    if (v < 0.18f)        col = Color.Lerp(col, lo, (0.18f - v) * 1.4f);  // hem
                    else if (v > 0.82f)   col = Color.Lerp(col, hi, (v - 0.82f) * 1.4f);  // shoulder
                    tex.SetPixel(x, y, col);
                }
            }

            // Armhole cutouts.
            FillRect(tex, 2, 32, 3, 39, Clear);
            FillRect(tex, 12, 32, 13, 39, Clear);

            // Shoulder caps / collar band.
            FillRect(tex, 4, 60, 11, 65, hi);
            // Subtle horizontal seam at chest.
            FillRect(tex, 4, 52, 11, 52, hi);
            // Waist crease.
            FillRect(tex, 4, 22, 11, 22, lo);

            DrawAccent(tex, accent, hi, mid, lo);

            return Finish(tex);
        }

        private static void DrawAccent(Texture2D tex, Accent kind, Color hi, Color mid, Color lo)
        {
            switch (kind)
            {
                case Accent.Hoodie:
                    // Drawstring pocket and hood lining.
                    FillRect(tex, 4, 62, 11, 71, mid);
                    FillRect(tex, 5, 64,  6, 70, lo);
                    FillRect(tex, 9, 64, 10, 70, lo);
                    FillRect(tex, 6, 17,  9, 27, lo);
                    FillRect(tex, 6, 17,  9, 18, hi);
                    // Strings
                    for (int y = 56; y <= 62; y++) { tex.SetPixel(6, y, lo); tex.SetPixel(9, y, lo); }
                    break;

                case Accent.Zipper:
                    // Vertical zipper
                    for (int y = 12; y <= 58; y++) tex.SetPixel(7, y, lo);
                    for (int y = 12; y <= 58; y += 4) tex.SetPixel(8, y, hi);
                    // Lapels
                    FillRect(tex, 4, 50, 5, 60, lo);
                    FillRect(tex, 10, 50, 11, 60, lo);
                    break;

                case Accent.TankStraps:
                    // Open V-neck and slim straps
                    FillRect(tex, 4, 56, 5, 65, hi);
                    FillRect(tex, 10, 56, 11, 65, hi);
                    FillRect(tex, 6, 56, 9, 60, Clear);
                    break;

                case Accent.Tie:
                    // Red tie down the middle
                    FillRect(tex,  7, 12,  8, 56, RedMid);
                    FillRect(tex,  6, 54,  9, 58, RedMid);
                    FillRect(tex,  7, 14,  8, 14, RedHi);
                    // Buttons
                    for (int y = 18; y <= 50; y += 6)
                    {
                        tex.SetPixel(7, y, WhiteHi);
                    }
                    break;

                case Accent.JerseyStripes:
                    FillRect(tex, 4, 40, 11, 42, WhiteHi);
                    FillRect(tex, 4, 16, 11, 18, WhiteHi);
                    // Big number block
                    FillRect(tex, 7, 24,  8, 36, WhiteHi);
                    // Collar
                    FillRect(tex, 4, 60, 11, 64, WhiteLo);
                    break;

                case Accent.Buttons:
                    for (int y = 16; y <= 56; y += 8)
                    {
                        tex.SetPixel(7, y, YellowHi);
                        tex.SetPixel(8, y, YellowHi);
                    }
                    // Open vest seam
                    for (int y = 12; y <= 58; y++) tex.SetPixel(7, y, lo);
                    break;

                case Accent.ArmorPlates:
                    // Two big chest plates with bevel
                    FillRect(tex, 4, 36, 7, 56, GoldMid);
                    FillRect(tex, 8, 36, 11, 56, GoldMid);
                    FillRect(tex, 5, 38, 6, 54, GoldHi);
                    FillRect(tex, 9, 38, 10, 54, GoldHi);
                    FillRect(tex, 4, 35, 11, 36, GoldLo);   // bottom rim
                    FillRect(tex, 4, 56, 11, 57, GoldLo);   // top rim
                    // Belt
                    FillRect(tex, 4, 14, 11, 17, GoldLo);
                    FillRect(tex, 7, 14,  8, 17, GoldHi);
                    // Rivets
                    tex.SetPixel(4, 56, WhiteHi); tex.SetPixel(11, 56, WhiteHi);
                    tex.SetPixel(4, 36, WhiteHi); tex.SetPixel(11, 36, WhiteHi);
                    break;

                case Accent.TuxLapels:
                    // White shirt strip
                    FillRect(tex, 6, 12, 9, 56, WhiteHi);
                    FillRect(tex, 7, 12, 8, 56, WhiteLo);
                    // Lapels (satin black, slightly shinier)
                    FillRect(tex, 4, 46, 6, 60, BlackHi);
                    FillRect(tex, 9, 46, 11, 60, BlackHi);
                    // Bow tie
                    FillRect(tex, 6, 56, 9, 60, RedMid);
                    FillRect(tex, 7, 56, 8, 60, RedHi);
                    // Buttons
                    tex.SetPixel(7, 24, BlackLo); tex.SetPixel(7, 32, BlackLo);
                    tex.SetPixel(7, 40, BlackLo);
                    break;

                case Accent.ClownDots:
                    // Big polka dots — yellow, blue, green
                    FillRect(tex, 5, 18,  6, 19, YellowHi);
                    FillRect(tex, 9, 26, 10, 27, BlueHi);
                    FillRect(tex, 6, 32,  7, 33, GreenHi);
                    FillRect(tex, 9, 40, 10, 41, YellowHi);
                    FillRect(tex, 5, 48,  6, 49, MagentaHi);
                    FillRect(tex, 9, 14, 10, 15, MagentaHi);
                    // Ruff collar
                    FillRect(tex, 3, 60, 12, 64, WhiteHi);
                    for (int x = 3; x <= 12; x++)
                        tex.SetPixel(x, 65, (x % 2 == 0) ? RedMid : WhiteHi);
                    break;

                case Accent.NeonGrid:
                    // Dark base, cyan circuit lines
                    FillRect(tex, 4, 8, 11, 60, BlackLo);
                    for (int y = 12; y <= 58; y += 6) FillRect(tex, 4, y, 11, y, CyanHi);
                    for (int x = 4; x <= 11; x += 3) for (int y = 12; y <= 58; y++) tex.SetPixel(x, y, CyanLo);
                    // Glow centre stripe
                    FillRect(tex, 7, 12, 8, 58, CyanHi);
                    // Magenta accents at the joints
                    tex.SetPixel(4, 24, MagentaHi); tex.SetPixel(11, 24, MagentaHi);
                    tex.SetPixel(4, 48, MagentaHi); tex.SetPixel(11, 48, MagentaHi);
                    break;

                case Accent.VarsityCollar:
                    // Cream sleeves stripes near armholes
                    FillRect(tex, 2, 28, 3, 32, WhiteHi);
                    FillRect(tex, 12, 28, 13, 32, WhiteHi);
                    // Big chest letter "A" centred
                    FillRect(tex, 7, 30, 8, 50, WhiteHi);
                    FillRect(tex, 6, 32, 9, 33, WhiteHi);   // crossbar
                    FillRect(tex, 6, 50, 9, 50, WhiteHi);   // peak
                    // Collar
                    FillRect(tex, 4, 60, 11, 64, WhiteHi);
                    FillRect(tex, 4, 62, 11, 62, BlueLo);
                    // Hem stripe
                    FillRect(tex, 4, 8, 11, 9, WhiteHi);
                    FillRect(tex, 4, 10, 11, 10, BlueLo);
                    break;
            }
        }

        // ── helpers ───────────────────────────────────────────────────────────────
        private static Texture2D NewTex(int w, int h)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    tex.SetPixel(x, y, Clear);
            return tex;
        }

        private static Sprite Finish(Texture2D tex)
        {
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            return Sprite.Create(tex, new Rect(0f, 0f, W, H), ChestPivot, PixelsPerUnit);
        }

        private static void FillRect(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            int xMin = Mathf.Min(x0, x1), xMax = Mathf.Max(x0, x1);
            int yMin = Mathf.Min(y0, y1), yMax = Mathf.Max(y0, y1);
            for (int y = yMin; y <= yMax; y++)
                for (int x = xMin; x <= xMax; x++)
                    if (x >= 0 && y >= 0 && x < tex.width && y < tex.height)
                        tex.SetPixel(x, y, c);
        }
    }
}
