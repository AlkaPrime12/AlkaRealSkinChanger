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
        private const int ArtGeneration = 25;
        // 32 × 128 native resolution (4× area vs the old 16 × 72), PPU=80 so the world size
        // is the same — the SpriteRenderer in TopsAttachmentRenderer rescales anyway.
        private const float PixelsPerUnit = 80f;
        private static readonly Vector2 ChestPivot = new Vector2(0.5f, 0.5f);
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

        // ── Half-sprite helpers (top renderer splits the shirt into upper / lower halves
        //    so each half rotates with its own spine segment, letting the garment bend
        //    when the stickman flexes). Both share the same underlying texture so we
        //    pay the build cost once.
        private static readonly Dictionary<string, Sprite> UpperCache = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, Sprite> LowerCache = new Dictionary<string, Sprite>();

        public static Sprite GetUpperHalf(string topsId)
        {
            string id = TopsCatalog.Normalize(topsId);
            if (id == "none") return null;
            string key = id + "#" + ArtGeneration + "#U";
            Sprite s;
            if (UpperCache.TryGetValue(key, out s) && s != null) return s;
            Sprite full = GetSprite(id);
            if (full == null) return null;
            Texture2D tex = full.texture;
            // Upper-half rect with pivot at bottom-centre (0.5, 0). The sprite extends in
            // the local +Y direction → so when we set transform.up = (mid→neck), it grows
            // from the middle bone toward the neck.
            s = Sprite.Create(tex,
                new Rect(0, tex.height / 2, tex.width, tex.height / 2),
                new Vector2(0.5f, 0f), PixelsPerUnit);
            UpperCache[key] = s;
            return s;
        }

        public static Sprite GetLowerHalf(string topsId)
        {
            string id = TopsCatalog.Normalize(topsId);
            if (id == "none") return null;
            string key = id + "#" + ArtGeneration + "#L";
            Sprite s;
            if (LowerCache.TryGetValue(key, out s) && s != null) return s;
            Sprite full = GetSprite(id);
            if (full == null) return null;
            Texture2D tex = full.texture;
            // Lower-half rect with pivot at top-centre (0.5, 1). The sprite extends in the
            // local -Y direction → when transform.up = (hip→mid), it hangs from the middle
            // bone down to the hip.
            s = Sprite.Create(tex,
                new Rect(0, 0, tex.width, tex.height / 2),
                new Vector2(0.5f, 1f), PixelsPerUnit);
            LowerCache[key] = s;
            return s;
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
                // ── 20 nuevos (segunda tanda) ──
                case "tx2_mech":       return BuildShaded(new Color(0.55f, 0.60f, 0.70f, 1f), new Color(0.32f, 0.36f, 0.45f, 1f), new Color(0.10f, 0.12f, 0.16f, 1f), Accent.Mech);
                case "tx2_crystal":    return BuildShaded(new Color(0.65f, 0.90f, 1f, 1f), new Color(0.30f, 0.60f, 0.95f, 1f), new Color(0.05f, 0.20f, 0.55f, 1f), Accent.Crystal);
                case "tx2_lightning":  return BuildShaded(new Color(0.18f, 0.20f, 0.42f, 1f), new Color(0.06f, 0.06f, 0.22f, 1f), new Color(0.02f, 0.02f, 0.08f, 1f), Accent.LightningRobe);
                case "tx2_tribal":     return BuildShaded(new Color(0.85f, 0.62f, 0.32f, 1f), new Color(0.50f, 0.30f, 0.10f, 1f), new Color(0.20f, 0.10f, 0.04f, 1f), Accent.Tribal);
                case "tx2_patchwork":  return BuildShaded(new Color(0.60f, 0.55f, 0.45f, 1f), new Color(0.35f, 0.32f, 0.25f, 1f), new Color(0.15f, 0.12f, 0.08f, 1f), Accent.Patchwork);
                case "tx2_sweater":    return BuildShaded(new Color(0.85f, 0.55f, 0.45f, 1f), new Color(0.55f, 0.28f, 0.20f, 1f), new Color(0.25f, 0.12f, 0.08f, 1f), Accent.Sweater);
                case "tx2_dragon":     return BuildShaded(new Color(0.50f, 0.10f, 0.10f, 1f), new Color(0.25f, 0.04f, 0.04f, 1f), new Color(0.08f, 0.02f, 0.02f, 1f), Accent.Dragon);
                case "tx2_cyberpunk":  return BuildShaded(new Color(0.10f, 0.10f, 0.12f, 1f), new Color(0.05f, 0.05f, 0.06f, 1f), new Color(0.02f, 0.02f, 0.03f, 1f), Accent.Cyberpunk);
                case "tx2_robe":       return BuildShaded(new Color(0.42f, 0.25f, 0.72f, 1f), new Color(0.20f, 0.10f, 0.42f, 1f), new Color(0.06f, 0.04f, 0.18f, 1f), Accent.MysticRobe);
                case "tx2_football":   return BuildShaded(new Color(0.10f, 0.45f, 0.20f, 1f), new Color(0.04f, 0.25f, 0.10f, 1f), new Color(0.02f, 0.10f, 0.04f, 1f), Accent.Football);
                case "tx2_basket":     return BuildShaded(new Color(0.92f, 0.55f, 0.10f, 1f), new Color(0.65f, 0.28f, 0.04f, 1f), new Color(0.25f, 0.10f, 0.02f, 1f), Accent.Basketball);
                case "tx2_hawaiian":   return BuildShaded(new Color(0.20f, 0.78f, 0.55f, 1f), new Color(0.10f, 0.45f, 0.32f, 1f), new Color(0.04f, 0.18f, 0.14f, 1f), Accent.Hawaiian);
                case "tx2_plaid":      return BuildShaded(new Color(0.78f, 0.20f, 0.18f, 1f), new Color(0.45f, 0.08f, 0.06f, 1f), new Color(0.18f, 0.04f, 0.04f, 1f), Accent.Plaid);
                case "tx2_pinstripe":  return BuildShaded(new Color(0.12f, 0.12f, 0.16f, 1f), new Color(0.06f, 0.06f, 0.09f, 1f), new Color(0.02f, 0.02f, 0.04f, 1f), Accent.Pinstripe);
                case "tx2_captain":    return BuildShaded(new Color(0.18f, 0.20f, 0.45f, 1f), new Color(0.08f, 0.10f, 0.25f, 1f), new Color(0.02f, 0.04f, 0.10f, 1f), Accent.Captain);
                case "tx2_skulls":     return BuildShaded(new Color(0.12f, 0.12f, 0.14f, 1f), new Color(0.06f, 0.06f, 0.08f, 1f), new Color(0.02f, 0.02f, 0.04f, 1f), Accent.SkullsPrint);
                case "tx2_carbon":     return BuildShaded(new Color(0.20f, 0.20f, 0.22f, 1f), new Color(0.08f, 0.08f, 0.10f, 1f), new Color(0.02f, 0.02f, 0.03f, 1f), Accent.Carbon);
                case "tx2_lab2":       return BuildShaded(new Color(0.95f, 0.85f, 0.20f, 1f), new Color(0.70f, 0.55f, 0.05f, 1f), new Color(0.32f, 0.22f, 0.02f, 1f), Accent.Hazmat);
                case "tx2_jet":        return BuildShaded(new Color(0.30f, 0.42f, 0.55f, 1f), new Color(0.14f, 0.22f, 0.32f, 1f), new Color(0.05f, 0.08f, 0.14f, 1f), Accent.JetPilot);
                case "tx2_circuit":    return BuildShaded(new Color(0.10f, 0.35f, 0.15f, 1f), new Color(0.04f, 0.18f, 0.06f, 1f), new Color(0.02f, 0.08f, 0.03f, 1f), Accent.Circuit);
                default:            return BuildShaded(RedHi,    RedMid,    RedLo,    Accent.None);
            }
        }

        private enum Accent
        {
            None, Hoodie, Zipper, TankStraps, Tie, JerseyStripes, Buttons,
            ArmorPlates, TuxLapels, ClownDots, NeonGrid, VarsityCollar,
            // 20 first batch
            Lava, Galaxy, Camo, Pirate, Knight, Ninja, Pharaoh, Robot, Skeleton, LabCoat,
            Track, Kimono, Punk, Gradient, Streetwear, Holiday, Racer, Priest, ChefApron, Diver,
            // 20 new — second batch
            Mech, Crystal, LightningRobe, Tribal, Patchwork, Sweater, Dragon, Cyberpunk,
            MysticRobe, Football, Basketball, Hawaiian, Plaid, Pinstripe, Captain,
            SkullsPrint, Carbon, Hazmat, JetPilot, Circuit
        }

        // ── Core builder: cylindrical shading + decorative accent layer ──────────
        // Canvas doubled vs the old 16×72 so the cylindrical gradient and accent details
        // have twice the pixel density. PPU bumped to 80 so the world size stays the same.
        private const int W = 32;
        private const int H = 144;

        private static Sprite BuildShaded(Color hi, Color mid, Color lo, Accent accent)
        {
            Texture2D tex = NewTex(W, H);

            // Improved cylindrical body shading at 32×144. Same algorithm as before — a
            // cosine-falloff centre highlight + edge shadow + vertical hem/shoulder bands —
            // but at 4× the pixel count for visibly smoother shading.
            Color spec = Color.Lerp(hi, WhiteHi, 0.35f);
            for (int y = 12; y <= 120; y++)
            {
                for (int x = 4; x <= 27; x++)
                {
                    // 0 at the centre column, 1 at the outer edges.
                    float t = (x - 15.5f) / 11.5f;
                    float u = Mathf.Abs(t);

                    float lamb = Mathf.Cos(u * Mathf.PI * 0.45f);
                    Color col = Color.Lerp(lo, hi, lamb);
                    if (u < 0.18f)
                        col = Color.Lerp(col, spec, (1f - u / 0.18f) * 0.55f);
                    if (u > 0.78f)
                        col = Color.Lerp(col, lo, (u - 0.78f) / 0.22f * 0.85f);

                    float v = (y - 12f) / 108f;
                    if (v < 0.18f)        col = Color.Lerp(col, lo, (0.18f - v) * 1.4f);
                    else if (v > 0.82f)   col = Color.Lerp(col, hi, (v - 0.82f) * 1.4f);
                    if (v > 0.55f && v < 0.62f) col = Color.Lerp(col, spec, 0.18f);

                    RawPixel(tex, x, y, col);
                }
            }

            // Armhole cutouts (both sides, waist level). RR = raw (no logical-coord scaling).
            RR(tex, 4, 64, 6, 78, Clear);
            RR(tex, 25, 64, 27, 78, Clear);

            // Shoulder caps / collar band (rounded, with bevel below).
            RR(tex, 8, 120, 23, 130, hi);
            RR(tex, 8, 130, 23, 130, spec);
            RR(tex, 8, 118, 23, 118, lo);
            // Subtle horizontal chest seam.
            RR(tex, 8, 104, 23, 104, hi);
            // Waist crease — darker pinch.
            RR(tex, 8, 44, 23, 44, lo);
            // Hem stitch (every other pixel) — raw coords.
            for (int x = 8; x <= 23; x++)
                if ((x & 1) == 0) RawPixel(tex, x, 14, lo);

            DrawAccent(tex, accent, hi, mid, lo);

            return Finish(tex);
        }

        // Translate "logical" 16×72 coords into the new 32×144 canvas (×2 each axis).
        // Every accent FillRect/SetPixel below uses logical coords so the old layouts keep
        // working unchanged — just at twice the pixel density.
        private static void LR(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            int xa = Mathf.Min(x0, x1) * 2;
            int ya = Mathf.Min(y0, y1) * 2;
            int xb = Mathf.Max(x0, x1) * 2 + 1;
            int yb = Mathf.Max(y0, y1) * 2 + 1;
            FillRect(tex, xa, ya, xb, yb, c);
        }

        private static void LP(Texture2D tex, int x, int y, Color c)
        {
            int x0 = x * 2, y0 = y * 2;
            for (int dy = 0; dy < 2; dy++)
                for (int dx = 0; dx < 2; dx++)
                    if (x0 + dx >= 0 && y0 + dy >= 0
                        && x0 + dx < tex.width && y0 + dy < tex.height)
                        tex.SetPixel(x0 + dx, y0 + dy, c);
        }

        // Raw (no scaling) — used only by BuildShaded which already writes 32×144 coords.
        private static void RR(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            FillRect(tex, x0, y0, x1, y1, c);
        }

        private static void RawPixel(Texture2D tex, int x, int y, Color c)
        {
            if (x >= 0 && y >= 0 && x < tex.width && y < tex.height) tex.SetPixel(x, y, c);
        }

        private static void DrawAccent(Texture2D tex, Accent kind, Color hi, Color mid, Color lo)
        {
            switch (kind)
            {
                case Accent.Hoodie:
                    // Drawstring pocket and hood lining.
                    LR(tex, 4, 62, 11, 71, mid);
                    LR(tex, 5, 64,  6, 70, lo);
                    LR(tex, 9, 64, 10, 70, lo);
                    LR(tex, 6, 17,  9, 27, lo);
                    LR(tex, 6, 17,  9, 18, hi);
                    // Strings
                    for (int y = 56; y <= 62; y++) { LP(tex,6, y, lo); LP(tex,9, y, lo); }
                    break;

                case Accent.Zipper:
                    // Vertical zipper
                    for (int y = 12; y <= 58; y++) LP(tex,7, y, lo);
                    for (int y = 12; y <= 58; y += 4) LP(tex,8, y, hi);
                    // Lapels
                    LR(tex, 4, 50, 5, 60, lo);
                    LR(tex, 10, 50, 11, 60, lo);
                    break;

                case Accent.TankStraps:
                    // Open V-neck and slim straps
                    LR(tex, 4, 56, 5, 65, hi);
                    LR(tex, 10, 56, 11, 65, hi);
                    LR(tex, 6, 56, 9, 60, Clear);
                    break;

                case Accent.Tie:
                    // Red tie down the middle
                    LR(tex,  7, 12,  8, 56, RedMid);
                    LR(tex,  6, 54,  9, 58, RedMid);
                    LR(tex,  7, 14,  8, 14, RedHi);
                    // Buttons
                    for (int y = 18; y <= 50; y += 6)
                    {
                        LP(tex,7, y, WhiteHi);
                    }
                    break;

                case Accent.JerseyStripes:
                    LR(tex, 4, 40, 11, 42, WhiteHi);
                    LR(tex, 4, 16, 11, 18, WhiteHi);
                    // Big number block
                    LR(tex, 7, 24,  8, 36, WhiteHi);
                    // Collar
                    LR(tex, 4, 60, 11, 64, WhiteLo);
                    break;

                case Accent.Buttons:
                    for (int y = 16; y <= 56; y += 8)
                    {
                        LP(tex,7, y, YellowHi);
                        LP(tex,8, y, YellowHi);
                    }
                    // Open vest seam
                    for (int y = 12; y <= 58; y++) LP(tex,7, y, lo);
                    break;

                case Accent.ArmorPlates:
                    // Two big chest plates with bevel
                    LR(tex, 4, 36, 7, 56, GoldMid);
                    LR(tex, 8, 36, 11, 56, GoldMid);
                    LR(tex, 5, 38, 6, 54, GoldHi);
                    LR(tex, 9, 38, 10, 54, GoldHi);
                    LR(tex, 4, 35, 11, 36, GoldLo);   // bottom rim
                    LR(tex, 4, 56, 11, 57, GoldLo);   // top rim
                    // Belt
                    LR(tex, 4, 14, 11, 17, GoldLo);
                    LR(tex, 7, 14,  8, 17, GoldHi);
                    // Rivets
                    LP(tex,4, 56, WhiteHi); LP(tex,11, 56, WhiteHi);
                    LP(tex,4, 36, WhiteHi); LP(tex,11, 36, WhiteHi);
                    break;

                case Accent.TuxLapels:
                    // White shirt strip
                    LR(tex, 6, 12, 9, 56, WhiteHi);
                    LR(tex, 7, 12, 8, 56, WhiteLo);
                    // Lapels (satin black, slightly shinier)
                    LR(tex, 4, 46, 6, 60, BlackHi);
                    LR(tex, 9, 46, 11, 60, BlackHi);
                    // Bow tie
                    LR(tex, 6, 56, 9, 60, RedMid);
                    LR(tex, 7, 56, 8, 60, RedHi);
                    // Buttons
                    LP(tex,7, 24, BlackLo); LP(tex,7, 32, BlackLo);
                    LP(tex,7, 40, BlackLo);
                    break;

                case Accent.ClownDots:
                    // Big polka dots — yellow, blue, green
                    LR(tex, 5, 18,  6, 19, YellowHi);
                    LR(tex, 9, 26, 10, 27, BlueHi);
                    LR(tex, 6, 32,  7, 33, GreenHi);
                    LR(tex, 9, 40, 10, 41, YellowHi);
                    LR(tex, 5, 48,  6, 49, MagentaHi);
                    LR(tex, 9, 14, 10, 15, MagentaHi);
                    // Ruff collar
                    LR(tex, 3, 60, 12, 64, WhiteHi);
                    for (int x = 3; x <= 12; x++)
                        LP(tex,x, 65, (x % 2 == 0) ? RedMid : WhiteHi);
                    break;

                case Accent.NeonGrid:
                    // Dark base, cyan circuit lines
                    LR(tex, 4, 8, 11, 60, BlackLo);
                    for (int y = 12; y <= 58; y += 6) LR(tex, 4, y, 11, y, CyanHi);
                    for (int x = 4; x <= 11; x += 3) for (int y = 12; y <= 58; y++) LP(tex,x, y, CyanLo);
                    // Glow centre stripe
                    LR(tex, 7, 12, 8, 58, CyanHi);
                    // Magenta accents at the joints
                    LP(tex,4, 24, MagentaHi); LP(tex,11, 24, MagentaHi);
                    LP(tex,4, 48, MagentaHi); LP(tex,11, 48, MagentaHi);
                    break;

                case Accent.VarsityCollar:
                    // Cream sleeves stripes near armholes
                    LR(tex, 2, 28, 3, 32, WhiteHi);
                    LR(tex, 12, 28, 13, 32, WhiteHi);
                    // Big chest letter "A" centred
                    LR(tex, 7, 30, 8, 50, WhiteHi);
                    LR(tex, 6, 32, 9, 33, WhiteHi);   // crossbar
                    LR(tex, 6, 50, 9, 50, WhiteHi);   // peak
                    // Collar
                    LR(tex, 4, 60, 11, 64, WhiteHi);
                    LR(tex, 4, 62, 11, 62, BlueLo);
                    // Hem stripe
                    LR(tex, 4, 8, 11, 9, WhiteHi);
                    LR(tex, 4, 10, 11, 10, BlueLo);
                    break;

                // ── 20 NEW ACCENTS ──
                case Accent.Lava:
                    // Crackled lava pattern: dark veins on bright orange
                    for (int y = 10; y <= 58; y += 3)
                        for (int x = 4; x <= 11; x++)
                            if (((x + y) % 5) == 0) LP(tex,x, y, BlackLo);
                    // Hot glow at the heart
                    LR(tex, 6, 36, 9, 42, YellowHi);
                    LR(tex, 7, 37, 8, 41, WhiteHi);
                    break;

                case Accent.Galaxy:
                    // Random stars
                    int[] sxs = { 4, 7, 10, 5, 9, 6, 11, 8 };
                    int[] sys = { 12, 18, 24, 30, 36, 42, 50, 56 };
                    for (int i = 0; i < sxs.Length; i++)
                    {
                        LP(tex,sxs[i], sys[i], WhiteHi);
                        LP(tex,sxs[i] + 1, sys[i], WhiteLo);
                        LP(tex,sxs[i], sys[i] + 1, WhiteLo);
                    }
                    // Nebula swirl
                    LR(tex, 5, 30, 10, 32, MagentaHi);
                    LR(tex, 6, 30, 9, 31, CyanHi);
                    break;

                case Accent.Camo:
                    // Splotchy camo: random olive/brown patches
                    int[,] blobs = { {4,16,6,20}, {8,24,11,30}, {5,40,7,46}, {9,46,11,52}, {7,12,10,15} };
                    for (int i = 0; i < blobs.GetLength(0); i++)
                        LR(tex, blobs[i,0], blobs[i,1], blobs[i,2], blobs[i,3], BrownLo);
                    int[,] blobs2 = { {5,28,7,32}, {9,16,11,20}, {6,52,9,56} };
                    for (int i = 0; i < blobs2.GetLength(0); i++)
                        LR(tex, blobs2[i,0], blobs2[i,1], blobs2[i,2], blobs2[i,3], BlackHi);
                    break;

                case Accent.Pirate:
                    // Wide buckled belt + open collar with rim
                    LR(tex, 4, 14, 11, 18, BrownMid);
                    LR(tex, 6, 15, 9, 17, GoldHi);
                    LR(tex, 7, 15, 8, 17, BrownLo);
                    // Lapels
                    LR(tex, 4, 48, 6, 60, lo);
                    LR(tex, 9, 48, 11, 60, lo);
                    // Gold buttons
                    LP(tex,7, 24, GoldHi); LP(tex,7, 32, GoldHi);
                    LP(tex,7, 40, GoldHi); LP(tex,7, 46, GoldHi);
                    // White shirt strip
                    LR(tex, 7, 50, 8, 60, WhiteHi);
                    break;

                case Accent.Knight:
                    // Plate armor: rivets at the corners + cross emblem
                    for (int yy = 12; yy <= 58; yy += 6)
                    {
                        LP(tex,4, yy, GrayLo);
                        LP(tex,11, yy, GrayLo);
                    }
                    // Cross (red on chest)
                    LR(tex, 7, 26, 8, 50, RedMid);
                    LR(tex, 5, 36, 10, 38, RedMid);
                    // Plate edges
                    LR(tex, 4, 38, 11, 39, GrayLo);
                    LR(tex, 4, 22, 11, 23, GrayLo);
                    break;

                case Accent.Ninja:
                    // Belt sash diagonally
                    for (int t = 0; t < 14; t++)
                    {
                        int xx = 4 + t / 2;
                        int yy = 30 - t;
                        if (xx >= 0 && yy >= 6 && xx < 16 && yy < 72)
                            LP(tex,xx, yy, RedHi);
                    }
                    LR(tex, 4, 16, 11, 18, RedMid);
                    // Stitching
                    for (int yy = 24; yy <= 56; yy += 4) LP(tex,7, yy, RedLo);
                    break;

                case Accent.Pharaoh:
                    // Vertical gold stripes (pleated robe)
                    for (int yy = 10; yy <= 58; yy++)
                    {
                        LP(tex,5, yy, GoldLo);
                        LP(tex,8, yy, GoldLo);
                        LP(tex,11, yy, GoldLo);
                    }
                    // Sacred eye
                    LR(tex, 6, 40, 9, 44, CyanHi);
                    LR(tex, 7, 41, 8, 43, BlackLo);
                    // Wide jewelled collar
                    LR(tex, 4, 58, 11, 60, CyanHi);
                    LR(tex, 4, 60, 11, 61, GoldHi);
                    break;

                case Accent.Robot:
                    // Vertical panel lines + chest plate + indicator lights
                    LR(tex, 7, 10, 8, 60, BlackLo);
                    LR(tex, 4, 36, 11, 44, GrayMid);
                    LR(tex, 5, 37, 10, 43, GrayLo);
                    // LEDs
                    LP(tex,5, 40, GreenHi);
                    LP(tex,8, 40, CyanHi);
                    LP(tex,11, 40, RedHi);
                    // Bolts at corners
                    LP(tex,4, 18, GrayLo); LP(tex,11, 18, GrayLo);
                    LP(tex,4, 54, GrayLo); LP(tex,11, 54, GrayLo);
                    break;

                case Accent.Skeleton:
                    // Rib cage outline
                    for (int yy = 22; yy <= 50; yy += 4)
                        LR(tex, 5, yy, 10, yy, WhiteHi);
                    LR(tex, 7, 22, 8, 52, WhiteHi);          // sternum
                    // Collarbones
                    LR(tex, 4, 56, 11, 57, WhiteHi);
                    break;

                case Accent.LabCoat:
                    // Open collar / lapels in lo + breast pocket
                    LR(tex, 4, 48, 6, 60, lo);
                    LR(tex, 9, 48, 11, 60, lo);
                    // Pocket
                    LR(tex, 5, 30, 7, 38, WhiteLo);
                    LR(tex, 5, 38, 7, 38, lo);
                    // Buttons
                    LP(tex,7, 24, BlackLo); LP(tex,7, 32, BlackLo);
                    LP(tex,7, 40, BlackLo); LP(tex,7, 46, BlackLo);
                    break;

                case Accent.Track:
                    // Two parallel side stripes (white) along the whole length
                    for (int yy = 10; yy <= 60; yy++)
                    {
                        LP(tex,4, yy, WhiteHi);
                        LP(tex,11, yy, WhiteHi);
                    }
                    // Zipper
                    for (int yy = 12; yy <= 58; yy++) LP(tex,7, yy, GrayHi);
                    for (int yy = 12; yy <= 58; yy += 3) LP(tex,8, yy, WhiteHi);
                    break;

                case Accent.Kimono:
                    // Diagonal cross-over flap (right over left)
                    for (int t = 0; t < 18; t++)
                    {
                        int xx = 4 + t / 3;
                        int yy = 60 - t;
                        if (xx < 16 && yy >= 12) LP(tex,xx, yy, hi);
                    }
                    // White obi belt
                    LR(tex, 4, 14, 11, 20, WhiteHi);
                    LR(tex, 4, 14, 11, 15, GrayLo);
                    LR(tex, 4, 20, 11, 20, GrayLo);
                    // Floral dot accent
                    LP(tex,9, 36, WhiteHi);
                    LP(tex,10, 35, WhiteHi);
                    LP(tex,10, 37, WhiteHi);
                    LP(tex,11, 36, WhiteHi);
                    break;

                case Accent.Punk:
                    // Studs at shoulders + safety pins
                    for (int xx = 5; xx <= 10; xx += 2)
                    {
                        LP(tex,xx, 58, GrayHi);
                        LP(tex,xx, 12, GrayHi);
                    }
                    // Diagonal patch (red)
                    LR(tex, 5, 32, 8, 40, RedMid);
                    LR(tex, 6, 33, 7, 39, RedHi);
                    // Tear/rip
                    for (int t = 0; t < 6; t++)
                        LP(tex,10, 28 + t, BlackLo);
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
                            LP(tex,x, y, px);
                        }
                    }
                    break;

                case Accent.Streetwear:
                    // Big chest logo block
                    LR(tex, 5, 36, 10, 44, WhiteHi);
                    LR(tex, 6, 37, 9, 43, BlackLo);
                    // Drawstring at the bottom
                    for (int xx = 5; xx <= 10; xx++) LP(tex,xx, 12, lo);
                    LP(tex,6, 11, lo);
                    LP(tex,9, 11, lo);
                    break;

                case Accent.Holiday:
                    // Snowflake-ish dots + zig-zag pattern
                    for (int yy = 16; yy <= 56; yy += 8)
                    {
                        LP(tex,5, yy, WhiteHi); LP(tex,8, yy, WhiteHi); LP(tex,11, yy, WhiteHi);
                        LP(tex,5, yy - 1, GreenHi);
                    }
                    // Snowfall band
                    LR(tex, 4, 28, 11, 30, WhiteLo);
                    LR(tex, 4, 44, 11, 46, WhiteLo);
                    break;

                case Accent.Racer:
                    // Diagonal sponsor stripes white + blue
                    for (int t = 0; t < 30; t++)
                    {
                        int xx = 4 + t / 4;
                        int yy = 8 + t;
                        if (xx < 12 && yy < 62)
                        {
                            LP(tex,xx, yy, WhiteHi);
                            if (xx + 1 < 12) LP(tex,xx + 1, yy, BlueHi);
                        }
                    }
                    // Number on the chest
                    LR(tex, 7, 36, 8, 44, WhiteHi);  // "1"
                    LP(tex,6, 43, WhiteHi);
                    break;

                case Accent.Priest:
                    // Vertical gold cross + lapel rope sash
                    LR(tex, 7, 14, 8, 56, GoldHi);
                    LR(tex, 5, 32, 10, 34, GoldHi);
                    // Rope sash
                    for (int yy = 14; yy <= 20; yy++) LP(tex,11, yy, GoldLo);
                    LP(tex,11, 13, GoldLo);
                    break;

                case Accent.ChefApron:
                    // Apron strap loop top + tied bow at the waist
                    LR(tex, 4, 58, 11, 60, WhiteLo);
                    LR(tex, 6, 60, 9, 65, WhiteHi);   // bib
                    // Waist bow
                    LR(tex, 4, 14, 6, 18, WhiteLo);
                    LR(tex, 9, 14, 11, 18, WhiteLo);
                    LR(tex, 6, 15, 9, 17, WhiteHi);
                    // Subtle red collar
                    LR(tex, 4, 60, 11, 60, RedHi);
                    break;

                case Accent.Diver:
                    // Wetsuit panels: bright stripe under arm + reflective trim
                    LR(tex, 7, 10, 8, 60, CyanHi);
                    LR(tex, 4, 30, 11, 32, CyanLo);
                    LR(tex, 4, 50, 11, 51, CyanHi);
                    // Logo on chest
                    LR(tex, 5, 40, 6, 43, WhiteHi);
                    LR(tex, 9, 40, 10, 43, WhiteHi);
                    break;

                // ── SECOND BATCH (20 new) ──
                case Accent.Mech:
                    LR(tex, 4, 30, 11, 50, GrayLo);
                    LR(tex, 5, 32, 10, 48, GrayHi);
                    LR(tex, 6, 36, 9, 44, CyanHi);
                    LR(tex, 7, 38, 8, 42, WhiteHi);
                    for (int yy = 16; yy <= 56; yy += 6) { LP(tex, 4, yy, BlackLo); LP(tex, 11, yy, BlackLo); }
                    LR(tex, 4, 58, 6, 62, GrayLo);
                    LR(tex, 9, 58, 11, 62, GrayLo);
                    break;

                case Accent.Crystal:
                    for (int yy = 14; yy <= 56; yy += 8)
                        for (int xx = 5; xx <= 10; xx += 2)
                        {
                            LP(tex, xx, yy, WhiteHi);
                            LP(tex, xx + 1, yy + 1, hi);
                        }
                    LR(tex, 6, 32, 9, 48, hi);
                    LR(tex, 7, 36, 8, 44, WhiteHi);
                    break;

                case Accent.LightningRobe:
                    for (int t = 0; t < 16; t++)
                    {
                        int xx = 4 + t / 3;
                        int yy = 50 - t;
                        if (xx < 12 && yy >= 12) LP(tex, xx, yy, YellowHi);
                    }
                    for (int t = 0; t < 16; t++)
                    {
                        int xx = 11 - t / 3;
                        int yy = 50 - t;
                        if (xx >= 4 && yy >= 12) LP(tex, xx, yy, YellowHi);
                    }
                    LR(tex, 7, 10, 8, 60, CyanHi);
                    break;

                case Accent.Tribal:
                    for (int yy = 16; yy <= 56; yy += 8)
                    {
                        LR(tex, 4, yy, 11, yy, BlackLo);
                        LR(tex, 5, yy + 1, 6, yy + 1, lo);
                        LR(tex, 9, yy + 1, 10, yy + 1, lo);
                    }
                    for (int t = 0; t < 4; t++)
                    {
                        LP(tex, 6 - t, 32 + t, BlackLo);
                        LP(tex, 9 + t, 32 + t, BlackLo);
                    }
                    break;

                case Accent.Patchwork:
                    LR(tex, 4, 12, 7, 24, new Color(0.55f, 0.10f, 0.10f, 1f));
                    LR(tex, 8, 12, 11, 24, new Color(0.18f, 0.40f, 0.60f, 1f));
                    LR(tex, 4, 26, 11, 38, new Color(0.30f, 0.55f, 0.20f, 1f));
                    LR(tex, 4, 40, 7, 56, new Color(0.85f, 0.55f, 0.10f, 1f));
                    LR(tex, 8, 40, 11, 56, new Color(0.55f, 0.18f, 0.55f, 1f));
                    for (int xx = 4; xx <= 11; xx++) { LP(tex, xx, 25, WhiteHi); LP(tex, xx, 39, WhiteHi); }
                    for (int yy = 12; yy <= 56; yy++) { if ((yy & 1) == 0) LP(tex, 7, yy, WhiteHi); }
                    break;

                case Accent.Sweater:
                    for (int yy = 14; yy <= 58; yy += 4)
                        for (int xx = 4; xx <= 11; xx++)
                            LP(tex, xx, yy, ((xx + yy / 4) & 1) == 0 ? WhiteLo : lo);
                    for (int yy = 14; yy <= 58; yy += 6)
                    {
                        LP(tex, 7, yy, WhiteHi);
                        LP(tex, 8, yy + 1, WhiteHi);
                    }
                    break;

                case Accent.Dragon:
                    for (int yy = 12; yy <= 56; yy += 5)
                        for (int xx = 4; xx <= 11; xx++)
                        {
                            int dx = (xx - 4 + (yy / 5) % 2 * 2) % 4;
                            if (dx < 2) LP(tex, xx, yy, hi);
                            else LP(tex, xx, yy, mid);
                            LP(tex, xx, yy + 1, GoldHi);
                        }
                    LR(tex, 7, 12, 8, 56, GoldHi);
                    break;

                case Accent.Cyberpunk:
                    LR(tex, 4, 12, 11, 60, BlackLo);
                    for (int yy = 16; yy <= 56; yy += 4)
                        LR(tex, 4, yy, 11, yy, CyanHi);
                    for (int xx = 4; xx <= 11; xx += 3)
                        for (int yy = 14; yy <= 58; yy++)
                            LP(tex, xx, yy, MagentaHi);
                    LR(tex, 5, 26, 6, 50, MagentaHi);
                    break;

                case Accent.MysticRobe:
                    int[] stxs = { 5, 9, 6, 10, 7 };
                    int[] stys = { 16, 22, 34, 40, 52 };
                    for (int i = 0; i < stxs.Length; i++)
                    {
                        LP(tex, stxs[i], stys[i], YellowHi);
                        LP(tex, stxs[i] + 1, stys[i], WhiteHi);
                        LP(tex, stxs[i], stys[i] + 1, WhiteHi);
                    }
                    LR(tex, 6, 30, 9, 34, GoldHi);
                    LR(tex, 7, 31, 8, 33, BlackLo);
                    break;

                case Accent.Football:
                    LR(tex, 4, 30, 11, 48, WhiteHi);
                    LR(tex, 7, 32, 8, 46, BlackLo);
                    LR(tex, 4, 56, 11, 60, hi);
                    LR(tex, 6, 58, 9, 60, WhiteHi);
                    break;

                case Accent.Basketball:
                    LR(tex, 4, 56, 6, 65, hi);
                    LR(tex, 9, 56, 11, 65, hi);
                    LR(tex, 6, 56, 9, 60, Clear);
                    for (int t = 0; t < 14; t++)
                    {
                        int xx = 4 + t / 2;
                        int yy = 40 - t;
                        if (xx < 12 && yy >= 14) LP(tex, xx, yy, WhiteHi);
                    }
                    LR(tex, 6, 22, 9, 26, BlackHi);
                    LP(tex, 7, 24, WhiteHi);
                    break;

                case Accent.Hawaiian:
                    LR(tex, 5, 18, 6, 20, new Color(1f, 0.50f, 0.10f, 1f));
                    LR(tex, 9, 30, 10, 32, new Color(1f, 0.95f, 0.40f, 1f));
                    LR(tex, 5, 42, 7, 44, new Color(0.85f, 0.10f, 0.50f, 1f));
                    LR(tex, 8, 50, 10, 52, new Color(0.95f, 0.75f, 0.20f, 1f));
                    for (int yy = 14; yy <= 56; yy += 6)
                        LP(tex, 4, yy, GreenLo);
                    LR(tex, 6, 26, 7, 28, GreenHi);
                    LR(tex, 9, 38, 10, 40, GreenHi);
                    break;

                case Accent.Plaid:
                    for (int yy = 12; yy <= 60; yy += 4) LR(tex, 4, yy, 11, yy, BlackLo);
                    for (int xx = 4; xx <= 11; xx += 3)
                        for (int yy = 12; yy <= 60; yy++)
                            LP(tex, xx, yy, BlackLo);
                    LR(tex, 7, 12, 7, 60, YellowHi);
                    LR(tex, 4, 36, 11, 36, YellowHi);
                    break;

                case Accent.Pinstripe:
                    for (int xx = 5; xx <= 10; xx += 2)
                        for (int yy = 10; yy <= 60; yy++)
                            LP(tex, xx, yy, WhiteHi);
                    LR(tex, 4, 48, 5, 60, lo);
                    LR(tex, 10, 48, 11, 60, lo);
                    LR(tex, 7, 14, 8, 56, RedMid);
                    break;

                case Accent.Captain:
                    LR(tex, 4, 58, 11, 62, GoldHi);
                    LR(tex, 4, 60, 5, 65, GoldMid);
                    LR(tex, 10, 60, 11, 65, GoldMid);
                    for (int yy = 16; yy <= 50; yy += 6)
                    {
                        LP(tex, 6, yy, GoldHi);
                        LP(tex, 9, yy, GoldHi);
                    }
                    LR(tex, 4, 50, 5, 58, lo);
                    LR(tex, 10, 50, 11, 58, lo);
                    break;

                case Accent.SkullsPrint:
                    int[] skxs = { 6, 9, 5, 10 };
                    int[] skys = { 18, 30, 44, 54 };
                    for (int i = 0; i < skxs.Length; i++)
                    {
                        int sx = skxs[i], sy = skys[i];
                        LR(tex, sx - 1, sy, sx + 1, sy + 2, WhiteHi);
                        LP(tex, sx - 1, sy + 1, BlackLo);
                        LP(tex, sx + 1, sy + 1, BlackLo);
                        LP(tex, sx, sy, BlackLo);
                    }
                    break;

                case Accent.Carbon:
                    for (int yy = 12; yy <= 60; yy++)
                        for (int xx = 4; xx <= 11; xx++)
                            LP(tex, xx, yy, ((xx + yy) & 1) == 0 ? BlackLo : GrayLo);
                    for (int yy = 14; yy <= 58; yy++) LP(tex, 8, yy, GrayHi);
                    break;

                case Accent.Hazmat:
                    LR(tex, 4, 12, 11, 60, hi);
                    LR(tex, 6, 30, 9, 32, BlackLo);
                    LR(tex, 6, 36, 9, 38, BlackLo);
                    LR(tex, 7, 33, 8, 35, BlackLo);
                    LR(tex, 2, 26, 3, 32, BlackLo);
                    LR(tex, 12, 26, 13, 32, BlackLo);
                    LR(tex, 4, 58, 11, 60, BlackLo);
                    break;

                case Accent.JetPilot:
                    for (int yy = 14; yy <= 58; yy++) LP(tex, 7, yy, BlackLo);
                    for (int yy = 14; yy <= 58; yy += 3) LP(tex, 8, yy, GrayHi);
                    LR(tex, 4, 40, 6, 48, RedMid);
                    LR(tex, 4, 40, 6, 41, WhiteHi);
                    LR(tex, 4, 58, 6, 60, YellowHi);
                    LR(tex, 9, 58, 11, 60, YellowHi);
                    break;

                case Accent.Circuit:
                    for (int yy = 14; yy <= 58; yy += 6)
                        LR(tex, 4, yy, 11, yy, GoldHi);
                    for (int xx = 4; xx <= 11; xx += 4)
                        for (int yy = 14; yy <= 58; yy++)
                            LP(tex, xx, yy, GoldLo);
                    LP(tex, 5, 20, WhiteHi);
                    LP(tex, 9, 32, WhiteHi);
                    LP(tex, 6, 44, WhiteHi);
                    LP(tex, 10, 50, WhiteHi);
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
