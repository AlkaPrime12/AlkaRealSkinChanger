using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>Sprites simples: casi todo ARRIBA de la cabeza; ojos/visor en el centro (máx. 2px bajo).</summary>
    public static class HatSpriteFactorySimple
    {
        private static readonly Color Clear = new Color(0f, 0f, 0f, 0f);
        private static readonly Color Outline = new Color(0.07f, 0.06f, 0.08f, 1f);
        private static readonly Color Hi = new Color(1f, 1f, 1f, 0.7f);

        public static Sprite Build(string hatId)
        {
            if (hatId.StartsWith("halo_"))
            {
                return BuildHalo(hatId);
            }

            if (hatId.StartsWith("horn_"))
            {
                return BuildHorn(hatId);
            }

            if (hatId.StartsWith("eye_"))
            {
                return BuildEye(hatId);
            }

            if (hatId.StartsWith("crown_"))
            {
                return BuildCrown(hatId);
            }

            if (hatId.StartsWith("visor_"))
            {
                return BuildVisor(hatId);
            }

            if (hatId.StartsWith("ear_"))
            {
                return BuildEar(hatId);
            }

            if (hatId.StartsWith("mark_"))
            {
                return BuildMark(hatId);
            }

            Sprite plus = HatSpriteFactorySimplePlus.Build(hatId);
            if (plus != null)
            {
                return plus;
            }

            return BuildHalo("halo_white");
        }

        // --- Halo: anillo fino, pivot abajo (solo encima de la cabeza) ---
        private static Sprite BuildHalo(string id)
        {
            Color hi;
            Color lo;
            ResolveHaloColors(id, out hi, out lo);
            const int w = 36, h = 10;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            for (int a = 0; a < 40; a++)
            {
                float ang = a / 40f * Mathf.PI * 2f;
                int x = Mathf.RoundToInt(mid + Mathf.Cos(ang) * 14f);
                int y = Mathf.RoundToInt(4 + Mathf.Sin(ang) * 2.5f);
                tex.SetPixel(x, y, lo);
                SetIfEmpty(tex, x, y - 1, hi);
            }

            FillEllipse(tex, mid, 4, 14f, 2.5f, lo);
            FillEllipse(tex, mid, 4, 13f, 1.8f, hi);
            DrawEllipseOutline(tex, mid, 4, 14f, 2.5f);
            return Finish(tex, w, h, new Vector2(0.5f, 0.05f));
        }

        // --- Horns: crecen hacia ARRIBA desde la base ---
        private static Sprite BuildHorn(string id)
        {
            const int w = 28, h = 22;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color hi = new Color(0.95f, 0.35f, 0.28f, 1f);
            Color lo = new Color(0.55f, 0.12f, 0.1f, 1f);
            if (id.Contains("crystal")) { hi = new Color(0.6f, 0.9f, 1f, 1f); lo = new Color(0.25f, 0.45f, 0.7f, 1f); }
            else if (id.Contains("demon") || id.Contains("devil") || id.Contains("imp"))
            {
                hi = new Color(0.85f, 0.15f, 0.12f, 1f); lo = new Color(0.45f, 0.05f, 0.05f, 1f);
            }
            else if (id.Contains("unicorn"))
            {
                hi = new Color(0.95f, 0.92f, 1f, 1f); lo = new Color(0.75f, 0.7f, 0.85f, 1f);
            }
            else if (id.Contains("stag") || id.Contains("antler"))
            {
                hi = new Color(0.65f, 0.42f, 0.22f, 1f); lo = new Color(0.38f, 0.22f, 0.1f, 1f);
            }

            int style = id.GetHashCode() % 5;
            if (style == 0 || id.Contains("devil"))
            {
                DrawHornPair(tex, mid, 2, -6, hi, lo);
            }
            else if (style == 1 || id.Contains("bull"))
            {
                DrawHornPair(tex, mid, 2, -4, hi, lo);
                FillEllipse(tex, mid, 3, 6f, 2f, lo);
            }
            else if (id.Contains("unicorn"))
            {
                DrawSingleHorn(tex, mid, 2, hi, lo, 14);
            }
            else if (id.Contains("ram") || id.Contains("antler"))
            {
                DrawCurvedHorn(tex, mid - 5, 2, -1, hi, lo, 10);
                DrawCurvedHorn(tex, mid + 5, 2, 1, hi, lo, 10);
            }
            else
            {
                DrawHornPair(tex, mid, 2, -5, hi, lo);
            }

            FillRect(tex, 6, 0, w - 7, 2, lo);
            DrawOutline(tex, 4, 0, w - 5, 4);
            return Finish(tex, w, h, new Vector2(0.5f, 0.04f));
        }

        // --- Eyes: banda horizontal en el centro (cara) ---
        private static Sprite BuildEye(string id)
        {
            const int w = 32, h = 8;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color white = new Color(0.98f, 0.98f, 0.98f, 1f);
            Color pupil = new Color(0.08f, 0.08f, 0.1f, 1f);
            Color lid = new Color(0.2f, 0.18f, 0.22f, 1f);

            if (id.Contains("angry"))
            {
                DrawAngryEyes(tex, mid);
            }
            else if (id.Contains("sleepy"))
            {
                for (int x = 8; x < 14; x++) { FillRect(tex, x, 4, x, 4, lid); }
                for (int x = 18; x < 24; x++) { FillRect(tex, x, 4, x, 4, lid); }
            }
            else if (id.Contains("wide") || id.Contains("dot"))
            {
                FillEllipse(tex, mid - 7, 4, id.Contains("wide") ? 4f : 2.5f, id.Contains("wide") ? 3.5f : 2.5f, white);
                FillEllipse(tex, mid + 7, 4, id.Contains("wide") ? 4f : 2.5f, id.Contains("wide") ? 3.5f : 2.5f, white);
                FillEllipse(tex, mid - 7, 4, 1.5f, 1.5f, pupil);
                FillEllipse(tex, mid + 7, 4, 1.5f, 1.5f, pupil);
            }
            else if (id.Contains("visor") || id.Contains("shade") || id.Contains("glow"))
            {
                FillRect(tex, 4, 3, w - 5, 5, new Color(0.1f, 0.1f, 0.12f, 1f));
                FillRect(tex, 5, 4, w - 6, 4, id.Contains("glow") ? new Color(0.3f, 0.9f, 1f, 1f) : new Color(0.05f, 0.05f, 0.08f, 1f));
                tex.SetPixel(Mathf.RoundToInt(mid) - 6, 4, Hi);
                tex.SetPixel(Mathf.RoundToInt(mid) + 6, 4, Hi);
            }
            else if (id.Contains("x"))
            {
                DrawXEye(tex, mid - 7, 4);
                DrawXEye(tex, mid + 7, 4);
            }
            else if (id.Contains("heart"))
            {
                FillEllipse(tex, mid - 7, 4, 2.5f, 2f, new Color(0.95f, 0.25f, 0.35f, 1f));
                FillEllipse(tex, mid + 7, 4, 2.5f, 2f, new Color(0.95f, 0.25f, 0.35f, 1f));
            }
            else
            {
                FillEllipse(tex, mid - 7, 4, 3f, 3f, white);
                FillEllipse(tex, mid + 7, 4, 3f, 3f, white);
                FillEllipse(tex, mid - 7, 4, 1.2f, 1.2f, pupil);
                FillEllipse(tex, mid + 7, 4, 1.2f, 1.2f, pupil);
                tex.SetPixel(Mathf.RoundToInt(mid) - 8, 5, Hi);
                tex.SetPixel(Mathf.RoundToInt(mid) + 8, 5, Hi);
            }

            DrawOutline(tex, 3, 2, w - 4, 5);
            return Finish(tex, w, h, new Vector2(0.5f, 0.5f));
        }

        // --- Crown: base en pivot inferior ---
        private static Sprite BuildCrown(string id)
        {
            const int w = 30, h = 14;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color hi = new Color(1f, 0.88f, 0.35f, 1f);
            Color lo = new Color(0.75f, 0.52f, 0.1f, 1f);
            if (id.Contains("silver")) { hi = new Color(0.9f, 0.92f, 0.96f, 1f); lo = new Color(0.55f, 0.58f, 0.65f, 1f); }
            else if (id.Contains("ruby") || id.Contains("neon")) { hi = new Color(0.95f, 0.25f, 0.3f, 1f); lo = new Color(0.55f, 0.08f, 0.12f, 1f); }
            else if (id.Contains("ice")) { hi = new Color(0.75f, 0.92f, 1f, 1f); lo = new Color(0.35f, 0.55f, 0.75f, 1f); }
            else if (id.Contains("dark")) { hi = new Color(0.35f, 0.32f, 0.4f, 1f); lo = new Color(0.15f, 0.12f, 0.18f, 1f); }
            else if (id.Contains("leaf")) { hi = new Color(0.45f, 0.82f, 0.38f, 1f); lo = new Color(0.22f, 0.5f, 0.2f, 1f); }

            FillRect(tex, 5, 1, w - 6, 4, lo);
            FillRect(tex, 6, 2, w - 7, 3, hi);
            int spikes = id.Contains("mini") ? 3 : 5;
            for (int i = 0; i < spikes; i++)
            {
                int cx = 6 + (i * (w - 12)) / Mathf.Max(1, spikes - 1);
                FillTriangleUp(tex, cx, 4, 2, 6, hi);
                tex.SetPixel(cx, 10, lo);
            }

            if (id.Contains("star"))
            {
                tex.SetPixel(Mathf.RoundToInt(mid), 8, new Color(1f, 0.95f, 0.5f, 1f));
            }

            DrawOutline(tex, 5, 1, w - 6, 10);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
        }

        // --- Visor: gafas en centro ---
        private static Sprite BuildVisor(string id)
        {
            const int w = 42, h = 12;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color frame = new Color(0.1f, 0.1f, 0.13f, 1f);
            Color lens = new Color(0.22f, 0.26f, 0.34f, 0.92f);
            Color lensHi = new Color(0.45f, 0.52f, 0.62f, 0.75f);
            if (id.Contains("red")) { lens = new Color(0.6f, 0.14f, 0.14f, 0.88f); lensHi = new Color(0.85f, 0.35f, 0.32f, 0.7f); }
            else if (id.Contains("blue")) { lens = new Color(0.14f, 0.38f, 0.78f, 0.88f); lensHi = new Color(0.4f, 0.65f, 0.95f, 0.7f); }
            else if (id.Contains("neon") || id.Contains("future") || id.Contains("tech"))
            {
                lens = new Color(0.15f, 0.82f, 0.95f, 0.9f);
                lensHi = new Color(0.55f, 0.98f, 1f, 0.8f);
            }

            FillRect(tex, 2, 4, w - 3, 8, frame);
            FillEllipse(tex, mid - 9, 6, 7.5f, 4.5f, lens);
            FillEllipse(tex, mid + 9, 6, 7.5f, 4.5f, lens);
            FillEllipse(tex, mid - 9, 6, 6f, 3.5f, lensHi);
            FillEllipse(tex, mid + 9, 6, 6f, 3.5f, lensHi);
            FillRect(tex, Mathf.RoundToInt(mid) - 2, 5, Mathf.RoundToInt(mid) + 2, 7, frame);

            if (id.Contains("shades"))
            {
                FillEllipse(tex, mid - 9, 6, 7f, 4f, new Color(0.04f, 0.04f, 0.06f, 1f));
                FillEllipse(tex, mid + 9, 6, 7f, 4f, new Color(0.04f, 0.04f, 0.06f, 1f));
            }

            FillRect(tex, 3, 5, 8, 6, frame);
            FillRect(tex, w - 9, 5, w - 4, 6, frame);
            tex.SetPixel(Mathf.RoundToInt(mid) - 10, 7, Hi);
            tex.SetPixel(Mathf.RoundToInt(mid) + 10, 7, Hi);
            DrawOutline(tex, 2, 4, w - 3, 8);
            return Finish(tex, w, h, new Vector2(0.5f, 0.5f));
        }

        // --- Ear: par de orejas arriba de la cabeza (pivot abajo) ---
        private static Sprite BuildEar(string id)
        {
            const int w = 34, h = 16;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color hi = new Color(0.95f, 0.82f, 0.68f, 1f);
            Color lo = new Color(0.78f, 0.58f, 0.42f, 1f);
            if (id.Contains("cat") || id.Contains("fox"))
            {
                FillTriangleUp(tex, 8, 2, 4, 11, hi);
                FillTriangleUp(tex, Mathf.RoundToInt(w) - 8, 2, 4, 11, hi);
                FillTriangleUp(tex, 8, 3, 3, 9, lo);
                FillTriangleUp(tex, Mathf.RoundToInt(w) - 8, 3, 3, 9, lo);
            }
            else if (id.Contains("bunny") || id.Contains("long"))
            {
                FillEllipse(tex, 9, 10, 4f, 8f, hi);
                FillEllipse(tex, w - 9, 10, 4f, 8f, hi);
                FillEllipse(tex, 9, 10, 3f, 7f, lo);
                FillEllipse(tex, w - 9, 10, 3f, 7f, lo);
            }
            else if (id.Contains("bat"))
            {
                FillTriangleUp(tex, 8, 4, 5, 5, lo);
                FillTriangleUp(tex, w - 8, 4, 5, 5, lo);
                FillRect(tex, 6, 6, w - 7, 7, hi);
            }
            else if (id.Contains("wolf") || id.Contains("bear"))
            {
                FillEllipse(tex, 8, 8, 5f, 6f, lo);
                FillEllipse(tex, w - 8, 8, 5f, 6f, lo);
                FillEllipse(tex, 8, 8, 4f, 5f, hi);
                FillEllipse(tex, w - 8, 8, 4f, 5f, hi);
            }
            else
            {
                FillEllipse(tex, 9, 7, 4f, 5f, hi);
                FillEllipse(tex, w - 9, 7, 4f, 5f, hi);
            }

            FillRect(tex, 8, 0, w - 9, 2, lo);
            DrawOutline(tex, 4, 0, w - 5, 13);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
        }

        // --- Mark: símbolo en frente (arriba del centro) ---
        private static Sprite BuildMark(string id)
        {
            const int w = 16, h = 12;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color hi = new Color(1f, 0.9f, 0.35f, 1f);
            Color lo = new Color(0.75f, 0.5f, 0.1f, 1f);
            if (id.Contains("scar")) { hi = new Color(0.85f, 0.25f, 0.2f, 1f); lo = new Color(0.45f, 0.08f, 0.08f, 1f); }
            else if (id.Contains("bolt")) { hi = new Color(0.95f, 0.9f, 0.3f, 1f); lo = new Color(0.55f, 0.45f, 0.05f, 1f); }
            else if (id.Contains("skull")) { hi = new Color(0.92f, 0.92f, 0.95f, 1f); lo = new Color(0.55f, 0.55f, 0.6f, 1f); }

            if (id.Contains("star"))
            {
                FillRect(tex, Mathf.RoundToInt(mid), 4, Mathf.RoundToInt(mid), 9, hi);
                FillRect(tex, 4, 6, w - 5, 6, hi);
                FillRect(tex, 6, 4, 6, 9, hi);
                FillRect(tex, w - 7, 4, w - 7, 9, hi);
            }
            else if (id.Contains("gem"))
            {
                FillTriangleUp(tex, Mathf.RoundToInt(mid), 3, 4, 5, hi);
                FillTriangleDown(tex, Mathf.RoundToInt(mid), 9, 4, 4, lo);
            }
            else if (id.Contains("heart"))
            {
                FillEllipse(tex, mid - 2, 6, 2.5f, 2f, hi);
                FillEllipse(tex, mid + 2, 6, 2.5f, 2f, hi);
                FillTriangleDown(tex, Mathf.RoundToInt(mid), 10, 4, 3, hi);
            }
            else if (id.Contains("flame"))
            {
                for (int y = 3; y < 10; y++)
                {
                    int half = Mathf.Max(1, 3 - (y - 3) / 2);
                    FillRect(tex, Mathf.RoundToInt(mid) - half, y, Mathf.RoundToInt(mid) + half, y,
                        y < 6 ? new Color(1f, 0.9f, 0.3f, 1f) : new Color(0.95f, 0.35f, 0.1f, 1f));
                }
            }
            else if (id.Contains("cross"))
            {
                FillRect(tex, Mathf.RoundToInt(mid), 3, Mathf.RoundToInt(mid), 9, hi);
                FillRect(tex, 5, 6, w - 6, 6, hi);
            }
            else if (id.Contains("moon"))
            {
                FillEllipse(tex, mid, 6, 4f, 4f, hi);
                FillEllipse(tex, mid + 2, 6, 3f, 3.5f, Clear);
            }
            else
            {
                FillEllipse(tex, mid, 6, 3f, 3f, hi);
                FillEllipse(tex, mid, 6, 2f, 2f, lo);
            }

            DrawOutline(tex, 3, 3, w - 4, 9);
            return Finish(tex, w, h, new Vector2(0.5f, 0.15f));
        }

        private static void ResolveHaloColors(string id, out Color hi, out Color lo)
        {
            hi = new Color(1f, 1f, 1f, 1f);
            lo = new Color(0.85f, 0.85f, 0.9f, 1f);
            if (id.Contains("gold")) { hi = new Color(1f, 0.92f, 0.4f, 1f); lo = new Color(0.85f, 0.65f, 0.15f, 1f); }
            else if (id.Contains("red")) { hi = new Color(1f, 0.45f, 0.35f, 1f); lo = new Color(0.7f, 0.15f, 0.1f, 1f); }
            else if (id.Contains("blue")) { hi = new Color(0.55f, 0.8f, 1f, 1f); lo = new Color(0.2f, 0.4f, 0.75f, 1f); }
            else if (id.Contains("green")) { hi = new Color(0.55f, 0.95f, 0.5f, 1f); lo = new Color(0.2f, 0.55f, 0.2f, 1f); }
            else if (id.Contains("purple")) { hi = new Color(0.8f, 0.55f, 1f, 1f); lo = new Color(0.4f, 0.2f, 0.6f, 1f); }
            else if (id.Contains("cyan")) { hi = new Color(0.5f, 0.95f, 1f, 1f); lo = new Color(0.15f, 0.5f, 0.65f, 1f); }
            else if (id.Contains("pink")) { hi = new Color(1f, 0.65f, 0.85f, 1f); lo = new Color(0.75f, 0.25f, 0.5f, 1f); }
            else if (id.Contains("silver")) { hi = new Color(0.92f, 0.94f, 0.98f, 1f); lo = new Color(0.6f, 0.62f, 0.7f, 1f); }
            else if (id.Contains("rainbow"))
            {
                hi = new Color(1f, 0.5f, 0.5f, 1f);
                lo = new Color(0.5f, 0.5f, 1f, 1f);
            }
        }

        private static void DrawHornPair(Texture2D tex, float mid, int baseY, int spread, Color hi, Color lo)
        {
            DrawSingleHorn(tex, mid - spread, baseY, hi, lo, 12);
            DrawSingleHorn(tex, mid + spread, baseY, hi, lo, 12);
        }

        private static void DrawSingleHorn(Texture2D tex, float rootX, int baseY, Color hi, Color lo, int len)
        {
            for (int i = 0; i < len; i++)
            {
                float t = i / (float)len;
                int half = Mathf.Max(1, Mathf.RoundToInt((1f - t) * 2f));
                int x = Mathf.RoundToInt(rootX);
                int y = baseY + i;
                FillRect(tex, x - half, y, x + half, y, lo);
                tex.SetPixel(x, y, hi);
            }
        }

        private static void DrawCurvedHorn(Texture2D tex, float rootX, int baseY, int dir, Color hi, Color lo, int len)
        {
            for (int i = 0; i < len; i++)
            {
                float t = i / (float)len;
                int x = Mathf.RoundToInt(rootX + dir * t * 4f);
                int y = baseY + i;
                FillRect(tex, x - 1, y, x + 1, y, lo);
                tex.SetPixel(x, y, hi);
            }
        }

        private static void DrawAngryEyes(Texture2D tex, float mid)
        {
            for (int x = 6; x < 12; x++)
            {
                tex.SetPixel(x, 5 - (x - 6) / 2, Outline);
            }

            for (int x = 20; x < 26; x++)
            {
                tex.SetPixel(x, 5 - (25 - x) / 2, Outline);
            }

            FillEllipse(tex, mid - 7, 4, 2.5f, 2f, new Color(0.95f, 0.2f, 0.15f, 1f));
            FillEllipse(tex, mid + 7, 4, 2.5f, 2f, new Color(0.95f, 0.2f, 0.15f, 1f));
        }

        private static void DrawXEye(Texture2D tex, float cx, int cy)
        {
            for (int d = -2; d <= 2; d++)
            {
                tex.SetPixel(Mathf.RoundToInt(cx) + d, cy + d, Outline);
                tex.SetPixel(Mathf.RoundToInt(cx) + d, cy - d, Outline);
            }
        }

        private static void FillTriangleDown(Texture2D tex, int cx, int yBase, int halfW, int height, Color color)
        {
            for (int y = 0; y < height; y++)
            {
                float t = y / (float)height;
                int half = Mathf.Max(0, Mathf.RoundToInt(halfW * (1f - t)));
                FillRect(tex, cx - half, yBase - y, cx + half, yBase - y, color);
            }
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
                SetIfEmpty(tex, x, yMin - 1, Outline);
                SetIfEmpty(tex, x, yMax + 1, Outline);
            }

            for (int y = yMin; y <= yMax; y++)
            {
                SetIfEmpty(tex, xMin - 1, y, Outline);
                SetIfEmpty(tex, xMax + 1, y, Outline);
            }
        }

        private static void DrawEllipseOutline(Texture2D tex, float cx, float cy, float rx, float ry)
        {
            for (int i = 0; i < 48; i++)
            {
                float a = i / 48f * Mathf.PI * 2f;
                int x = Mathf.RoundToInt(cx + Mathf.Cos(a) * (rx + 0.5f));
                int y = Mathf.RoundToInt(cy + Mathf.Sin(a) * (ry + 0.5f));
                SetIfEmpty(tex, x, y, Outline);
            }
        }

        private static void SetIfEmpty(Texture2D tex, int x, int y, Color color)
        {
            if (x >= 0 && y >= 0 && x < tex.width && y < tex.height && tex.GetPixel(x, y).a < 0.05f)
            {
                tex.SetPixel(x, y, color);
            }
        }
    }
}
