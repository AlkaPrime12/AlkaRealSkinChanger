using UnityEngine;

namespace StickFightColorCustomizer.UI
{
    public static class ColorWheelTexture
    {
        private static Texture2D _wheel;
        private const int Size = 128;

        public static Texture2D GetWheel()
        {
            if (_wheel != null)
            {
                return _wheel;
            }

            _wheel = new Texture2D(Size, Size, TextureFormat.ARGB32, false);
            float center = (Size - 1) * 0.5f;
            float radius = center - 2f;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float dx = (x - center) / radius;
                    float dy = (y - center) / radius;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > 1f)
                    {
                        _wheel.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                        continue;
                    }

                    float hue = (Mathf.Atan2(dy, dx) / (Mathf.PI * 2f) + 1f) % 1f;
                    float sat = dist;
                    Color rgb = Color.HSVToRGB(hue, sat, 1f);
                    rgb.a = 1f;
                    _wheel.SetPixel(x, y, rgb);
                }
            }

            _wheel.Apply();
            _wheel.wrapMode = TextureWrapMode.Clamp;
            _wheel.filterMode = FilterMode.Bilinear;
            return _wheel;
        }

        public static bool TryPickColor(Vector2 guiPos, Rect wheelRect, out Color color)
        {
            color = Color.white;
            if (!wheelRect.Contains(guiPos))
            {
                return false;
            }

            float u = (guiPos.x - wheelRect.x) / wheelRect.width;
            float v = (guiPos.y - wheelRect.y) / wheelRect.height;
            float dx = u * 2f - 1f;
            float dy = (1f - v) * 2f - 1f;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            if (dist > 1f)
            {
                dist = 1f;
            }

            float hue = (Mathf.Atan2(dy, dx) / (Mathf.PI * 2f) + 1f) % 1f;
            color = Color.HSVToRGB(hue, dist, 1f);
            color.a = 1f;
            return true;
        }
    }
}
