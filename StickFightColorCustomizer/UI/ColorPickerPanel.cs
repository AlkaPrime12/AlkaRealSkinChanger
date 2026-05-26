using StickFightColorCustomizer.Core;
using UnityEngine;

namespace StickFightColorCustomizer.UI
{
    public sealed class ColorPickerPanel
    {
        private readonly HexFieldState _hexField = new HexFieldState();
        private bool _wheelOpen;
        private float _brightness = 1f;
        private string _hex = "#FFFFFF";

        public void SyncHex(Color color)
        {
            _hex = ColorUtil.ToHex(color);
            _hexField.ForceSyncHex(_hex);
            Color.RGBToHSV(color, out float h, out float s, out float v);
            _brightness = v;
        }

        public Color Draw(
            Color color,
            string hexControlName,
            System.Action<Color> onApply,
            System.Func<string, int, Color, int> drawChannel)
        {
            ColorMenuTheme.Ensure();
            ColorMenuTheme.DrawColorSwatch(GUILayoutUtility.GetRect(120f, 28f), color);

            ColorUtil.RgbToBytes(color, out int r, out int g, out int b);
            r = drawChannel("R", r, ColorMenuTheme.AccentYellow);
            g = drawChannel("G", g, ColorMenuTheme.AccentViolet);
            b = drawChannel("B", b, new Color(0.4f, 0.65f, 1f));
            Color next = ColorUtil.FromRgbBytes(r, g, b);
            if (next != color)
            {
                color = next;
                _hexField.SyncFromColorIfNotFocused(hexControlName, ref _hex, color);
                onApply(color);
            }

            _hexField.DrawAndTryApplyLive(
                ref _hex,
                hexControlName,
                c =>
                {
                    color = c;
                    SyncHex(color);
                    onApply(color);
                },
                applyVisualWhenDisabled: false);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(MenuLocalization.T("color_wheel"), ColorMenuTheme.Button))
            {
                _wheelOpen = !_wheelOpen;
            }

            GUILayout.EndHorizontal();

            if (_wheelOpen)
            {
                color = DrawWheel(color, onApply);
            }

            return color;
        }

        private Color DrawWheel(Color color, System.Action<Color> onApply)
        {
            Texture2D wheel = ColorWheelTexture.GetWheel();
            Rect rect = GUILayoutUtility.GetRect(140f, 140f);
            GUI.DrawTexture(rect, wheel, ScaleMode.ScaleToFit);

            float newBright = GUILayout.HorizontalSlider(
                _brightness,
                0f,
                1f,
                ColorMenuTheme.HorizontalSlider,
                ColorMenuTheme.HorizontalSliderThumb);
            if (!Mathf.Approximately(newBright, _brightness))
            {
                _brightness = newBright;
                Color.RGBToHSV(color, out float h, out float s, out float v);
                color = Color.HSVToRGB(h, s, _brightness);
                color.a = 1f;
                SyncHex(color);
                onApply(color);
            }

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Color picked;
                if (ColorWheelTexture.TryPickColor(e.mousePosition, rect, out picked))
                {
                    picked.a = 1f;
                    Color.RGBToHSV(picked, out float h, out float s, out float v);
                    color = Color.HSVToRGB(h, s, _brightness);
                    color.a = 1f;
                    SyncHex(color);
                    onApply(color);
                    e.Use();
                }
            }

            return color;
        }
    }
}
