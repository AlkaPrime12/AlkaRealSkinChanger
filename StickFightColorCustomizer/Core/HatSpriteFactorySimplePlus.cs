using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>10 categorías extra × 10 variantes (sprites pulidos).</summary>
    public static class HatSpriteFactorySimplePlus
    {
        private static readonly Color Clear = new Color(0f, 0f, 0f, 0f);
        private static readonly Color Outline = new Color(0.06f, 0.05f, 0.08f, 1f);
        private static readonly Color Hi = new Color(1f, 1f, 1f, 0.75f);

        public static Sprite Build(string hatId)
        {
            if (hatId.StartsWith("mask_")) { return BuildMask(hatId); }
            if (hatId.StartsWith("hair_")) { return BuildHair(hatId); }
            if (hatId.StartsWith("brow_")) { return BuildBrow(hatId); }
            if (hatId.StartsWith("blush_")) { return BuildBlush(hatId); }
            if (hatId.StartsWith("mohawk_")) { return BuildMohawk(hatId); }
            if (hatId.StartsWith("antenna_")) { return BuildAntenna(hatId); }
            if (hatId.StartsWith("aura_")) { return BuildAura(hatId); }
            if (hatId.StartsWith("feather_")) { return BuildFeather(hatId); }
            if (hatId.StartsWith("patch_")) { return BuildPatch(hatId); }
            if (hatId.StartsWith("spike_")) { return BuildSpike(hatId); }
            return null;
        }

        private static Sprite BuildMask(string id)
        {
            const int w = 30, h = 12;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color hi = new Color(0.2f, 0.2f, 0.24f, 1f);
            Color lo = new Color(0.1f, 0.1f, 0.12f, 1f);
            if (id.Contains("skull")) { hi = new Color(0.92f, 0.92f, 0.95f, 1f); lo = new Color(0.55f, 0.55f, 0.6f, 1f); }
            else if (id.Contains("gas")) { hi = new Color(0.75f, 0.82f, 0.35f, 0.9f); lo = new Color(0.45f, 0.55f, 0.2f, 0.85f); }
            else if (id.Contains("clown")) { hi = new Color(0.95f, 0.25f, 0.2f, 1f); lo = new Color(0.65f, 0.1f, 0.1f, 1f); }
            else if (id.Contains("hero")) { hi = new Color(0.2f, 0.35f, 0.75f, 1f); lo = new Color(0.1f, 0.18f, 0.45f, 1f); }

            FillEllipse(tex, mid, 6, 12f, 5f, lo);
            FillEllipse(tex, mid, 6, 11f, 4f, hi);
            if (id.Contains("skull"))
            {
                FillEllipse(tex, mid - 5, 6, 2f, 2.5f, lo);
                FillEllipse(tex, mid + 5, 6, 2f, 2.5f, lo);
                FillRect(tex, Mathf.RoundToInt(mid) - 2, 7, Mathf.RoundToInt(mid) + 2, 8, lo);
            }
            else
            {
                FillEllipse(tex, mid - 6, 6, 3.5f, 3f, new Color(0.05f, 0.05f, 0.08f, 0.95f));
                FillEllipse(tex, mid + 6, 6, 3.5f, 3f, new Color(0.05f, 0.05f, 0.08f, 0.95f));
            }

            DrawOutline(tex, 4, 3, w - 5, 8);
            return Finish(tex, w, h, new Vector2(0.5f, 0.5f));
        }

        private static Sprite BuildHair(string id)
        {
            const int w = 32, h = 16;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color hi = new Color(0.35f, 0.22f, 0.12f, 1f);
            Color lo = new Color(0.22f, 0.12f, 0.06f, 1f);
            if (id.Contains("green")) { hi = new Color(0.3f, 0.75f, 0.28f, 1f); lo = new Color(0.15f, 0.45f, 0.14f, 1f); }
            else if (id.Contains("pink")) { hi = new Color(0.95f, 0.45f, 0.65f, 1f); lo = new Color(0.65f, 0.2f, 0.4f, 1f); }
            else if (id.Contains("blue")) { hi = new Color(0.35f, 0.55f, 0.95f, 1f); lo = new Color(0.15f, 0.28f, 0.65f, 1f); }
            else if (id.Contains("white")) { hi = new Color(0.95f, 0.95f, 0.98f, 1f); lo = new Color(0.7f, 0.7f, 0.75f, 1f); }

            if (id.Contains("afro"))
            {
                FillEllipse(tex, mid, 9, 13f, 7f, lo);
                FillEllipse(tex, mid, 9, 12f, 6f, hi);
            }
            else if (id.Contains("mop") || id.Contains("bang"))
            {
                for (int x = 6; x < w - 6; x++)
                {
                    int len = 4 + (x % 5);
                    FillRect(tex, x, 2, x, 2 + len, hi);
                    tex.SetPixel(x, 2 + len, lo);
                }
            }
            else if (id.Contains("pompadour"))
            {
                FillEllipse(tex, mid + 4, 8, 10f, 6f, hi);
                FillEllipse(tex, mid + 4, 8, 9f, 5f, lo);
            }
            else
            {
                for (int y = 4; y < 14; y++)
                {
                    float t = (y - 4) / 10f;
                    int half = Mathf.Max(1, Mathf.RoundToInt((1f - t) * 10f + 2f));
                    FillRect(tex, Mathf.RoundToInt(mid) - half, y, Mathf.RoundToInt(mid) + half, y, y < 8 ? hi : lo);
                }
            }

            FillRect(tex, 7, 0, w - 8, 2, lo);
            DrawOutline(tex, 5, 2, w - 6, 13);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
        }

        private static Sprite BuildBrow(string id)
        {
            const int w = 32, h = 6;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color c = new Color(0.2f, 0.15f, 0.12f, 1f);
            if (id.Contains("red")) { c = new Color(0.55f, 0.12f, 0.1f, 1f); }
            else if (id.Contains("blond") || id.Contains("bush")) { c = new Color(0.55f, 0.38f, 0.15f, 1f); }

            if (id.Contains("angry"))
            {
                for (int x = 5; x < 12; x++) { tex.SetPixel(x, 4 - (x - 5) / 2, c); }
                for (int x = 20; x < 27; x++) { tex.SetPixel(x, 4 - (25 - x) / 2, c); }
            }
            else if (id.Contains("sad"))
            {
                for (int x = 5; x < 12; x++) { tex.SetPixel(x, 2 + (x - 5) / 2, c); }
                for (int x = 20; x < 27; x++) { tex.SetPixel(x, 2 + (25 - x) / 2, c); }
            }
            else if (id.Contains("uni"))
            {
                FillRect(tex, 6, 3, w - 7, 4, c);
            }
            else
            {
                FillRect(tex, 6, 3, 12, 4, c);
                FillRect(tex, 20, 3, 26, 4, c);
            }

            DrawOutline(tex, 4, 2, w - 5, 4);
            return Finish(tex, w, h, new Vector2(0.5f, 0.5f));
        }

        private static Sprite BuildBlush(string id)
        {
            const int w = 34, h = 8;
            Texture2D tex = NewTex(w, h);
            Color pink = new Color(0.95f, 0.45f, 0.55f, 0.85f);
            if (id.Contains("heart"))
            {
                FillEllipse(tex, 8, 4, 3f, 2.5f, pink);
                FillEllipse(tex, w - 8, 4, 3f, 2.5f, pink);
            }
            else if (id.Contains("freckle"))
            {
                for (int i = 0; i < 6; i++)
                {
                    tex.SetPixel(6 + i * 2, 3 + (i % 2), new Color(0.75f, 0.45f, 0.3f, 1f));
                    tex.SetPixel(w - 6 - i * 2, 3 + (i % 2), new Color(0.75f, 0.45f, 0.3f, 1f));
                }
            }
            else
            {
                FillEllipse(tex, 8, 4, 4f, 3f, pink);
                FillEllipse(tex, w - 8, 4, 4f, 3f, pink);
            }

            return Finish(tex, w, h, new Vector2(0.5f, 0.5f));
        }

        private static Sprite BuildMohawk(string id)
        {
            const int w = 20, h = 22;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color hi = new Color(0.95f, 0.2f, 0.15f, 1f);
            Color lo = new Color(0.55f, 0.08f, 0.08f, 1f);
            if (id.Contains("blue")) { hi = new Color(0.35f, 0.55f, 0.95f, 1f); lo = new Color(0.15f, 0.28f, 0.65f, 1f); }
            else if (id.Contains("green")) { hi = new Color(0.4f, 0.85f, 0.35f, 1f); lo = new Color(0.18f, 0.5f, 0.15f, 1f); }
            else if (id.Contains("flame")) { hi = new Color(1f, 0.65f, 0.15f, 1f); lo = new Color(0.9f, 0.25f, 0.05f, 1f); }
            else if (id.Contains("ice")) { hi = new Color(0.75f, 0.92f, 1f, 1f); lo = new Color(0.35f, 0.6f, 0.85f, 1f); }
            else if (id.Contains("gold")) { hi = new Color(1f, 0.88f, 0.35f, 1f); lo = new Color(0.75f, 0.52f, 0.1f, 1f); }

            int height = id.Contains("short") ? 10 : (id.Contains("tall") ? 18 : 14);
            for (int y = 2; y < height; y++)
            {
                float t = y / (float)height;
                int half = Mathf.Max(0, Mathf.RoundToInt((1f - t) * 4f));
                FillRect(tex, Mathf.RoundToInt(mid) - half, y, Mathf.RoundToInt(mid) + half, y, hi);
                tex.SetPixel(Mathf.RoundToInt(mid) - half, y, lo);
                tex.SetPixel(Mathf.RoundToInt(mid) + half, y, lo);
            }

            tex.SetPixel(Mathf.RoundToInt(mid), height, Hi);
            DrawOutline(tex, 6, 1, w - 7, height);
            return Finish(tex, w, h, new Vector2(0.5f, 0.05f));
        }

        private static Sprite BuildAntenna(string id)
        {
            const int w = 28, h = 20;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color stem = new Color(0.45f, 0.45f, 0.5f, 1f);
            Color ball = new Color(0.95f, 0.3f, 0.25f, 1f);
            if (id.Contains("glow")) { ball = new Color(0.4f, 0.95f, 1f, 1f); }
            else if (id.Contains("bee")) { ball = new Color(1f, 0.85f, 0.2f, 1f); }

            if (id.Contains("double") || id.Contains("fork"))
            {
                FillRect(tex, Mathf.RoundToInt(mid) - 6, 2, Mathf.RoundToInt(mid) - 6, 10, stem);
                FillRect(tex, Mathf.RoundToInt(mid) + 6, 2, Mathf.RoundToInt(mid) + 6, 10, stem);
                FillEllipse(tex, mid - 6, 12, 3f, 3f, ball);
                FillEllipse(tex, mid + 6, 12, 3f, 3f, ball);
            }
            else
            {
                FillRect(tex, Mathf.RoundToInt(mid), 2, Mathf.RoundToInt(mid), 12, stem);
                float tipY = id.Contains("long") ? 17f : 13f;
                FillEllipse(tex, mid, tipY, id.Contains("ball") ? 4f : 3f, id.Contains("ball") ? 4f : 3f, ball);
                if (id.Contains("spiral"))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int x = Mathf.RoundToInt(mid + Mathf.Cos(i * 0.9f) * 3f);
                        int y = 4 + i;
                        tex.SetPixel(x, y, stem);
                    }
                }
            }

            FillRect(tex, 6, 0, w - 7, 2, stem);
            return Finish(tex, w, h, new Vector2(0.5f, 0.05f));
        }

        private static Sprite BuildAura(string id)
        {
            const int w = 38, h = 14;
            Texture2D tex = NewTex(w, h);
            float mid = w / 2f;
            Color hi = new Color(1f, 1f, 1f, 0.55f);
            Color lo = new Color(0.85f, 0.9f, 1f, 0.35f);
            if (id.Contains("fire")) { hi = new Color(1f, 0.6f, 0.15f, 0.6f); lo = new Color(0.95f, 0.2f, 0.05f, 0.4f); }
            else if (id.Contains("poison")) { hi = new Color(0.55f, 0.95f, 0.35f, 0.55f); lo = new Color(0.2f, 0.55f, 0.15f, 0.35f); }
            else if (id.Contains("dark")) { hi = new Color(0.35f, 0.2f, 0.45f, 0.5f); lo = new Color(0.1f, 0.05f, 0.15f, 0.35f); }

            for (int a = 0; a < 36; a++)
            {
                float ang = a / 36f * Mathf.PI * 2f;
                int x = Mathf.RoundToInt(mid + Mathf.Cos(ang) * 15f);
                int y = Mathf.RoundToInt(6 + Mathf.Sin(ang) * 4f);
                tex.SetPixel(x, y, lo);
                SetIfEmpty(tex, x, y, hi);
            }

            FillEllipse(tex, mid, 6, 14f, 4f, lo);
            return Finish(tex, w, h, new Vector2(0.5f, 0.08f));
        }

        private static Sprite BuildFeather(string id)
        {
            const int w = 22, h = 18;
            Texture2D tex = NewTex(w, h);
            Color hi = new Color(0.95f, 0.25f, 0.2f, 1f);
            Color lo = new Color(0.65f, 0.1f, 0.08f, 1f);
            if (id.Contains("blue")) { hi = new Color(0.35f, 0.55f, 0.95f, 1f); lo = new Color(0.15f, 0.28f, 0.65f, 1f); }
            else if (id.Contains("gold")) { hi = new Color(1f, 0.88f, 0.35f, 1f); lo = new Color(0.75f, 0.52f, 0.1f, 1f); }
            else if (id.Contains("peacock")) { hi = new Color(0.25f, 0.75f, 0.85f, 1f); lo = new Color(0.1f, 0.35f, 0.45f, 1f); }

            for (int y = 2; y < 16; y++)
            {
                float t = y / 15f;
                int half = Mathf.Max(1, Mathf.RoundToInt((1f - t) * 4f + 1f));
                FillRect(tex, 10 - half, y, 10 + half, y, hi);
                tex.SetPixel(10 - half, y, lo);
            }

            FillRect(tex, 9, 0, 11, 3, lo);
            DrawOutline(tex, 4, 2, 16, 15);
            return Finish(tex, w, h, new Vector2(0.5f, 0.12f));
        }

        private static Sprite BuildPatch(string id)
        {
            const int w = 18, h = 10;
            Texture2D tex = NewTex(w, h);
            Color patch = new Color(0.12f, 0.12f, 0.15f, 1f);
            Color band = new Color(0.55f, 0.12f, 0.1f, 1f);
            if (id.Contains("gold")) { band = new Color(0.95f, 0.75f, 0.2f, 1f); }
            FillRect(tex, 3, 3, 14, 7, patch);
            FillRect(tex, 2, 2, 15, 3, band);
            if (id.Contains("skull"))
            {
                tex.SetPixel(8, 5, new Color(0.85f, 0.85f, 0.9f, 1f));
                tex.SetPixel(9, 5, patch);
            }
            DrawOutline(tex, 2, 2, 15, 7);
            return Finish(tex, w, h, new Vector2(0.35f, 0.5f));
        }

        private static Sprite BuildSpike(string id)
        {
            const int w = 34, h = 12;
            Texture2D tex = NewTex(w, h);
            Color hi = new Color(0.75f, 0.75f, 0.82f, 1f);
            Color lo = new Color(0.4f, 0.4f, 0.48f, 1f);
            if (id.Contains("fire")) { hi = new Color(1f, 0.55f, 0.15f, 1f); lo = new Color(0.75f, 0.2f, 0.05f, 1f); }
            else if (id.Contains("ice")) { hi = new Color(0.75f, 0.92f, 1f, 1f); lo = new Color(0.35f, 0.6f, 0.85f, 1f); }
            else if (id.Contains("neon")) { hi = new Color(0.45f, 0.95f, 1f, 1f); lo = new Color(0.15f, 0.45f, 0.65f, 1f); }

            int[] xs = { 5, 10, 17, 24, 29 };
            for (int i = 0; i < xs.Length; i++)
            {
                int hSpike = id.Contains("long") ? 8 : 5;
                FillTriangleUp(tex, xs[i], 2, 2, hSpike, hi);
                tex.SetPixel(xs[i], 2 + hSpike, lo);
            }

            FillRect(tex, 3, 0, w - 4, 2, lo);
            DrawOutline(tex, 3, 0, w - 4, 10);
            return Finish(tex, w, h, new Vector2(0.5f, 0.06f));
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

        private static void SetIfEmpty(Texture2D tex, int x, int y, Color color)
        {
            if (x >= 0 && y >= 0 && x < tex.width && y < tex.height && tex.GetPixel(x, y).a < 0.05f)
            {
                tex.SetPixel(x, y, color);
            }
        }
    }
}
