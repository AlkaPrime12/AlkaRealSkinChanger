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
        private const int ArtGeneration = 22;
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
                // 20 new
                case "tx_lava":       return BuildShaded(new Color(1f, 0.55f, 0.10f, 1f), new Color(0.85f, 0.20f, 0.05f, 1f), new Color(0.30f, 0.04f, 0.02f, 1f), Accent.Lava);
                case "tx_galaxy":     return BuildShaded(new Color(0.45f, 0.20f, 0.85f, 1f), new Color(0.18f, 0.08f, 0.42f, 1f), new Color(0.04f, 0.02f, 0.16f, 1f), Accent.Galaxy);
                case "tx_camo":       return BuildShaded(new Color(0.42f, 0.55f, 0.25f, 1f), new Color(0.25f, 0.35f, 0.14f, 1f), new Color(0.10f, 0.16f, 0.06f, 1f), Accent.Camo);
                case "tx_pirate":     return BuildShaded(new Color(0.55f, 0.10f, 0.10f, 1f), new Color(0.32f, 0.04f, 0.04f, 1f), new Color(0.12f, 0.02f, 0.02f, 1f), Accent.Pirate);
                case "tx_knight":     return BuildShaded(new Color(0.86f, 0.88f, 0.92f, 1f), new Color(0.50f, 0.52f, 0.58f, 1f), new Color(0.18f, 0.20f, 0.24f, 1f), Accent.Knight);
                case "tx_ninja":      return BuildShaded(new Color(0.18f, 0.18f, 0.22f, 1f), new Color(0.10f, 0.10f, 0.12f, 1f), new Color(0.03f, 0.03f, 0.05f, 1f), Accent.Ninja);
                case "tx_pharaoh":    return BuildShaded(new Color(1f, 0.92f, 0.42f, 1f), new Color(0.78f, 0.55f, 0.10f, 1f), new Color(0.30f, 0.18f, 0.02f, 1f), Accent.Pharaoh);
                case "tx_robot":      return BuildShaded(new Color(0.78f, 0.82f, 0.88f, 1f), new Color(0.40f, 0.45f, 0.55f, 1f), new Color(0.10f, 0.12f, 0.16f, 1f), Accent.Robot);
                case "tx_skeleton":   return BuildShaded(BlackHi,  BlackLo,   BlackLo,  Accent.Skeleton);
                case "tx_lab":        return BuildShaded(WhiteHi,  WhiteLo,   GrayLo,   Accent.LabCoat);
                case "tx_track":      return BuildShaded(new Color(0.10f, 0.10f, 0.12f, 1f), new Color(0.05f, 0.05f, 0.06f, 1f), new Color(0.02f, 0.02f, 0.03f, 1f), Accent.Track);
                case "tx_kimono":     return BuildShaded(new Color(0.92f, 0.20f, 0.30f, 1f), new Color(0.55f, 0.06f, 0.14f, 1f), new Color(0.20f, 0.02f, 0.06f, 1f), Accent.Kimono);
                case "tx_punk":       return BuildShaded(new Color(0.14f, 0.14f, 0.18f, 1f), new Color(0.04f, 0.04f, 0.06f, 1f), new Color(0.02f, 0.02f, 0.04f, 1f), Accent.Punk);
                case "tx_gradient":   return BuildShaded(new Color(1f, 0.40f, 0.85f, 1f), new Color(0.55f, 0.25f, 0.95f, 1f), new Color(0.20f, 0.05f, 0.50f, 1f), Accent.Gradient);
                case "tx_streetwear": return BuildShaded(new Color(0.35f, 0.42f, 0.52f, 1f), new Color(0.18f, 0.22f, 0.30f, 1f), new Color(0.06f, 0.08f, 0.12f, 1f), Accent.Streetwear);
                case "tx_holiday":    return BuildShaded(new Color(0.85f, 0.10f, 0.12f, 1f), new Color(0.45f, 0.04f, 0.06f, 1f), new Color(0.15f, 0.02f, 0.04f, 1f), Accent.Holiday);
                case "tx_racer":      return BuildShaded(new Color(0.95f, 0.20f, 0.10f, 1f), new Color(0.55f, 0.06f, 0.04f, 1f), new Color(0.15f, 0.02f, 0.02f, 1f), Accent.Racer);
                case "tx_priest":     return BuildShaded(new Color(0.95f, 0.92f, 0.86f, 1f), new Color(0.70f, 0.65f, 0.55f, 1f), new Color(0.32f, 0.28f, 0.20f, 1f), Accent.Priest);
                case "tx_chef":       return BuildShaded(WhiteHi,  WhiteLo,   GrayLo,   Accent.ChefApron);
                case "tx_diver":      return BuildShaded(new Color(0.10f, 0.18f, 0.42f, 1f), new Color(0.04f, 0.08f, 0.22f, 1f), new Color(0.02f, 0.04f, 0.10f, 1f), Accent.Diver);
                default:            return BuildShaded(RedHi,    RedMid,    RedLo,    Accent.None);
            }
        }

        private enum Accent
        {
            None, Hoodie, Zipper, TankStraps, Tie, JerseyStripes, Buttons,
            ArmorPlates, TuxLapels, ClownDots, NeonGrid, VarsityCollar,
            // 20 new
            Lava, Galaxy, Camo, Pirate, Knight, Ninja, Pharaoh, Robot, Skeleton, LabCoat,
            Track, Kimono, Punk, Gradient, Streetwear, Holiday, Racer, Priest, ChefApron, Diver
        }

        // ── Core builder: cylindrical shading + decorative accent layer ──────────
        private const int W = 16;
        private const int H = 72;

        private static Sprite BuildShaded(Color hi, Color mid, Color lo, Accent accent)
        {
            Texture2D tex = NewTex(W, H);

            // Improved cylindrical body shading: a soft cosine falloff from a bright centre
            // column to dark edges + vertical micro variation. The narrow centre highlight
            // and dark side ribbons sell the "fabric wrapped around a cylinder" illusion.
            Color spec = Color.Lerp(hi, WhiteHi, 0.35f);   // bright specular reflection
            for (int y = 6; y <= 60; y++)
            {
                for (int x = 2; x <= 13; x++)
                {
                    // 0 at the centre column, 1 at the outer edges (signed for hi/shadow side).
                    float t = (x - 7.5f) / 5.5f;
                    float u = Mathf.Abs(t);

                    // Lambert-ish: bright centre, falls off to dark at edges, with a tight
                    // specular sheen one step off-centre to mimic 3D fabric.
                    float lamb = Mathf.Cos(u * Mathf.PI * 0.45f);    // 1 centre → 0.7 edge
                    Color col = Color.Lerp(lo, hi, lamb);
                    if (u < 0.18f)
                        col = Color.Lerp(col, spec, (1f - u / 0.18f) * 0.55f);     // hot highlight
                    if (u > 0.78f)
                        col = Color.Lerp(col, lo, (u - 0.78f) / 0.22f * 0.85f);    // hard shadow at edge

                    // Vertical tonal variation: hem darker, shoulder lighter, mid-chest bump.
                    float v = (y - 6f) / 54f;
                    if (v < 0.18f)        col = Color.Lerp(col, lo, (0.18f - v) * 1.4f);
                    else if (v > 0.82f)   col = Color.Lerp(col, hi, (v - 0.82f) * 1.4f);
                    // Tiny pectoral highlight band
                    if (v > 0.55f && v < 0.62f) col = Color.Lerp(col, spec, 0.18f);

                    tex.SetPixel(x, y, col);
                }
            }

            // Armhole cutouts.
            FillRect(tex, 2, 32, 3, 39, Clear);
            FillRect(tex, 12, 32, 13, 39, Clear);

            // Shoulder caps / collar band (rounded, with bevel below).
            FillRect(tex, 4, 60, 11, 65, hi);
            FillRect(tex, 4, 65, 11, 65, spec);                 // top bevel highlight
            FillRect(tex, 4, 59, 11, 59, lo);                   // collar shadow
            // Subtle horizontal seam at chest.
            FillRect(tex, 4, 52, 11, 52, hi);
            // Waist crease — darker pinch.
            FillRect(tex, 4, 22, 11, 22, lo);
            // Hem stitch.
            for (int x = 4; x <= 11; x++)
                if ((x & 1) == 0) tex.SetPixel(x, 7, lo);

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

                // ── 20 NEW ACCENTS ──
                case Accent.Lava:
                    // Crackled lava pattern: dark veins on bright orange
                    for (int y = 10; y <= 58; y += 3)
                        for (int x = 4; x <= 11; x++)
                            if (((x + y) % 5) == 0) tex.SetPixel(x, y, BlackLo);
                    // Hot glow at the heart
                    FillRect(tex, 6, 36, 9, 42, YellowHi);
                    FillRect(tex, 7, 37, 8, 41, WhiteHi);
                    break;

                case Accent.Galaxy:
                    // Random stars
                    int[] sxs = { 4, 7, 10, 5, 9, 6, 11, 8 };
                    int[] sys = { 12, 18, 24, 30, 36, 42, 50, 56 };
                    for (int i = 0; i < sxs.Length; i++)
                    {
                        tex.SetPixel(sxs[i], sys[i], WhiteHi);
                        tex.SetPixel(sxs[i] + 1, sys[i], WhiteLo);
                        tex.SetPixel(sxs[i], sys[i] + 1, WhiteLo);
                    }
                    // Nebula swirl
                    FillRect(tex, 5, 30, 10, 32, MagentaHi);
                    FillRect(tex, 6, 30, 9, 31, CyanHi);
                    break;

                case Accent.Camo:
                    // Splotchy camo: random olive/brown patches
                    int[,] blobs = { {4,16,6,20}, {8,24,11,30}, {5,40,7,46}, {9,46,11,52}, {7,12,10,15} };
                    for (int i = 0; i < blobs.GetLength(0); i++)
                        FillRect(tex, blobs[i,0], blobs[i,1], blobs[i,2], blobs[i,3], BrownLo);
                    int[,] blobs2 = { {5,28,7,32}, {9,16,11,20}, {6,52,9,56} };
                    for (int i = 0; i < blobs2.GetLength(0); i++)
                        FillRect(tex, blobs2[i,0], blobs2[i,1], blobs2[i,2], blobs2[i,3], BlackHi);
                    break;

                case Accent.Pirate:
                    // Wide buckled belt + open collar with rim
                    FillRect(tex, 4, 14, 11, 18, BrownMid);
                    FillRect(tex, 6, 15, 9, 17, GoldHi);
                    FillRect(tex, 7, 15, 8, 17, BrownLo);
                    // Lapels
                    FillRect(tex, 4, 48, 6, 60, lo);
                    FillRect(tex, 9, 48, 11, 60, lo);
                    // Gold buttons
                    tex.SetPixel(7, 24, GoldHi); tex.SetPixel(7, 32, GoldHi);
                    tex.SetPixel(7, 40, GoldHi); tex.SetPixel(7, 46, GoldHi);
                    // White shirt strip
                    FillRect(tex, 7, 50, 8, 60, WhiteHi);
                    break;

                case Accent.Knight:
                    // Plate armor: rivets at the corners + cross emblem
                    for (int yy = 12; yy <= 58; yy += 6)
                    {
                        tex.SetPixel(4, yy, GrayLo);
                        tex.SetPixel(11, yy, GrayLo);
                    }
                    // Cross (red on chest)
                    FillRect(tex, 7, 26, 8, 50, RedMid);
                    FillRect(tex, 5, 36, 10, 38, RedMid);
                    // Plate edges
                    FillRect(tex, 4, 38, 11, 39, GrayLo);
                    FillRect(tex, 4, 22, 11, 23, GrayLo);
                    break;

                case Accent.Ninja:
                    // Belt sash diagonally
                    for (int t = 0; t < 14; t++)
                    {
                        int xx = 4 + t / 2;
                        int yy = 30 - t;
                        if (xx >= 0 && yy >= 6 && xx < 16 && yy < 72)
                            tex.SetPixel(xx, yy, RedHi);
                    }
                    FillRect(tex, 4, 16, 11, 18, RedMid);
                    // Stitching
                    for (int yy = 24; yy <= 56; yy += 4) tex.SetPixel(7, yy, RedLo);
                    break;

                case Accent.Pharaoh:
                    // Vertical gold stripes (pleated robe)
                    for (int yy = 10; yy <= 58; yy++)
                    {
                        tex.SetPixel(5, yy, GoldLo);
                        tex.SetPixel(8, yy, GoldLo);
                        tex.SetPixel(11, yy, GoldLo);
                    }
                    // Sacred eye
                    FillRect(tex, 6, 40, 9, 44, CyanHi);
                    FillRect(tex, 7, 41, 8, 43, BlackLo);
                    // Wide jewelled collar
                    FillRect(tex, 4, 58, 11, 60, CyanHi);
                    FillRect(tex, 4, 60, 11, 61, GoldHi);
                    break;

                case Accent.Robot:
                    // Vertical panel lines + chest plate + indicator lights
                    FillRect(tex, 7, 10, 8, 60, BlackLo);
                    FillRect(tex, 4, 36, 11, 44, GrayMid);
                    FillRect(tex, 5, 37, 10, 43, GrayLo);
                    // LEDs
                    tex.SetPixel(5, 40, GreenHi);
                    tex.SetPixel(8, 40, CyanHi);
                    tex.SetPixel(11, 40, RedHi);
                    // Bolts at corners
                    tex.SetPixel(4, 18, GrayLo); tex.SetPixel(11, 18, GrayLo);
                    tex.SetPixel(4, 54, GrayLo); tex.SetPixel(11, 54, GrayLo);
                    break;

                case Accent.Skeleton:
                    // Rib cage outline
                    for (int yy = 22; yy <= 50; yy += 4)
                        FillRect(tex, 5, yy, 10, yy, WhiteHi);
                    FillRect(tex, 7, 22, 8, 52, WhiteHi);          // sternum
                    // Collarbones
                    FillRect(tex, 4, 56, 11, 57, WhiteHi);
                    break;

                case Accent.LabCoat:
                    // Open collar / lapels in lo + breast pocket
                    FillRect(tex, 4, 48, 6, 60, lo);
                    FillRect(tex, 9, 48, 11, 60, lo);
                    // Pocket
                    FillRect(tex, 5, 30, 7, 38, WhiteLo);
                    FillRect(tex, 5, 38, 7, 38, lo);
                    // Buttons
                    tex.SetPixel(7, 24, BlackLo); tex.SetPixel(7, 32, BlackLo);
                    tex.SetPixel(7, 40, BlackLo); tex.SetPixel(7, 46, BlackLo);
                    break;

                case Accent.Track:
                    // Two parallel side stripes (white) along the whole length
                    for (int yy = 10; yy <= 60; yy++)
                    {
                        tex.SetPixel(4, yy, WhiteHi);
                        tex.SetPixel(11, yy, WhiteHi);
                    }
                    // Zipper
                    for (int yy = 12; yy <= 58; yy++) tex.SetPixel(7, yy, GrayHi);
                    for (int yy = 12; yy <= 58; yy += 3) tex.SetPixel(8, yy, WhiteHi);
                    break;

                case Accent.Kimono:
                    // Diagonal cross-over flap (right over left)
                    for (int t = 0; t < 18; t++)
                    {
                        int xx = 4 + t / 3;
                        int yy = 60 - t;
                        if (xx < 16 && yy >= 12) tex.SetPixel(xx, yy, hi);
                    }
                    // White obi belt
                    FillRect(tex, 4, 14, 11, 20, WhiteHi);
                    FillRect(tex, 4, 14, 11, 15, GrayLo);
                    FillRect(tex, 4, 20, 11, 20, GrayLo);
                    // Floral dot accent
                    tex.SetPixel(9, 36, WhiteHi);
                    tex.SetPixel(10, 35, WhiteHi);
                    tex.SetPixel(10, 37, WhiteHi);
                    tex.SetPixel(11, 36, WhiteHi);
                    break;

                case Accent.Punk:
                    // Studs at shoulders + safety pins
                    for (int xx = 5; xx <= 10; xx += 2)
                    {
                        tex.SetPixel(xx, 58, GrayHi);
                        tex.SetPixel(xx, 12, GrayHi);
                    }
                    // Diagonal patch (red)
                    FillRect(tex, 5, 32, 8, 40, RedMid);
                    FillRect(tex, 6, 33, 7, 39, RedHi);
                    // Tear/rip
                    for (int t = 0; t < 6; t++)
                        tex.SetPixel(10, 28 + t, BlackLo);
                    break;

                case Accent.Gradient:
                    // Replace body with a multi-stop vertical gradient (pink → purple → blue)
                    for (int y = 7; y <= 60; y++)
                    {
                        float v = (y - 7f) / 53f;
                        Color stop;
                        if (v < 0.5f) stop = Color.Lerp(new Color(1f, 0.40f, 0.85f, 1f), new Color(0.55f, 0.25f, 0.95f, 1f), v / 0.5f);
                        else          stop = Color.Lerp(new Color(0.55f, 0.25f, 0.95f, 1f), new Color(0.20f, 0.45f, 1f, 1f), (v - 0.5f) / 0.5f);
                        for (int x = 4; x <= 11; x++)
                        {
                            float u = Mathf.Abs((x - 7.5f) / 5.5f);
                            Color px = stop * (1f - u * 0.45f);
                            px.a = 1f;
                            tex.SetPixel(x, y, px);
                        }
                    }
                    break;

                case Accent.Streetwear:
                    // Big chest logo block
                    FillRect(tex, 5, 36, 10, 44, WhiteHi);
                    FillRect(tex, 6, 37, 9, 43, BlackLo);
                    // Drawstring at the bottom
                    for (int xx = 5; xx <= 10; xx++) tex.SetPixel(xx, 12, lo);
                    tex.SetPixel(6, 11, lo);
                    tex.SetPixel(9, 11, lo);
                    break;

                case Accent.Holiday:
                    // Snowflake-ish dots + zig-zag pattern
                    for (int yy = 16; yy <= 56; yy += 8)
                    {
                        tex.SetPixel(5, yy, WhiteHi); tex.SetPixel(8, yy, WhiteHi); tex.SetPixel(11, yy, WhiteHi);
                        tex.SetPixel(5, yy - 1, GreenHi);
                    }
                    // Snowfall band
                    FillRect(tex, 4, 28, 11, 30, WhiteLo);
                    FillRect(tex, 4, 44, 11, 46, WhiteLo);
                    break;

                case Accent.Racer:
                    // Diagonal sponsor stripes white + blue
                    for (int t = 0; t < 30; t++)
                    {
                        int xx = 4 + t / 4;
                        int yy = 8 + t;
                        if (xx < 12 && yy < 62)
                        {
                            tex.SetPixel(xx, yy, WhiteHi);
                            if (xx + 1 < 12) tex.SetPixel(xx + 1, yy, BlueHi);
                        }
                    }
                    // Number on the chest
                    FillRect(tex, 7, 36, 8, 44, WhiteHi);  // "1"
                    tex.SetPixel(6, 43, WhiteHi);
                    break;

                case Accent.Priest:
                    // Vertical gold cross + lapel rope sash
                    FillRect(tex, 7, 14, 8, 56, GoldHi);
                    FillRect(tex, 5, 32, 10, 34, GoldHi);
                    // Rope sash
                    for (int yy = 14; yy <= 20; yy++) tex.SetPixel(11, yy, GoldLo);
                    tex.SetPixel(11, 13, GoldLo);
                    break;

                case Accent.ChefApron:
                    // Apron strap loop top + tied bow at the waist
                    FillRect(tex, 4, 58, 11, 60, WhiteLo);
                    FillRect(tex, 6, 60, 9, 65, WhiteHi);   // bib
                    // Waist bow
                    FillRect(tex, 4, 14, 6, 18, WhiteLo);
                    FillRect(tex, 9, 14, 11, 18, WhiteLo);
                    FillRect(tex, 6, 15, 9, 17, WhiteHi);
                    // Subtle red collar
                    FillRect(tex, 4, 60, 11, 60, RedHi);
                    break;

                case Accent.Diver:
                    // Wetsuit panels: bright stripe under arm + reflective trim
                    FillRect(tex, 7, 10, 8, 60, CyanHi);
                    FillRect(tex, 4, 30, 11, 32, CyanLo);
                    FillRect(tex, 4, 50, 11, 51, CyanHi);
                    // Logo on chest
                    FillRect(tex, 5, 40, 6, 43, WhiteHi);
                    FillRect(tex, 9, 40, 10, 43, WhiteHi);
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
