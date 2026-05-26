using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class GlowTextureFactory
    {
        private static Texture2D _lineSoft;

        /// <summary>
        /// Perfil de alpha en el grosor de la línea: fuerte en el centro, cae suave hacia los bordes (simula halo).
        /// </summary>
        public static Texture2D GetLineSoft()
        {
            if (_lineSoft != null)
            {
                return _lineSoft;
            }

            const int w = 16;
            const int h = 4;
            _lineSoft = new Texture2D(w, h, TextureFormat.ARGB32, false);
            float mid = (w - 1) * 0.5f;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dist = Mathf.Abs(x - mid) / (mid + 0.001f);
                    float a = Mathf.Exp(-dist * dist * 3.2f);
                    _lineSoft.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }

            _lineSoft.Apply();
            _lineSoft.wrapMode = TextureWrapMode.Clamp;
            _lineSoft.filterMode = FilterMode.Bilinear;
            return _lineSoft;
        }

        public static Sprite GetRadialSprite()
        {
            return null;
        }
    }
}
