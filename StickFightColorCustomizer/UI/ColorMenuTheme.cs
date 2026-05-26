using UnityEngine;

namespace StickFightColorCustomizer.UI
{
    public static class ColorMenuTheme
    {
        public static readonly Color Bg = new Color(0.04f, 0.04f, 0.05f, 0.97f);
        public static readonly Color Panel = new Color(0.08f, 0.08f, 0.1f, 1f);
        public static readonly Color AccentYellow = new Color(1f, 0.84f, 0.2f, 1f);
        public static readonly Color AccentViolet = new Color(0.58f, 0.28f, 0.92f, 1f);
        public static readonly Color AccentCyan = new Color(0.25f, 0.85f, 0.95f, 1f);
        public static readonly Color Text = new Color(0.92f, 0.92f, 0.95f, 1f);
        public static readonly Color TextMuted = new Color(0.55f, 0.55f, 0.62f, 1f);

        private static bool _ready;
        private static GUIStyle _window;
        private static GUIStyle _box;
        private static GUIStyle _label;
        private static GUIStyle _labelMuted;
        private static GUIStyle _labelCredit;
        private static GUIStyle _button;
        private static GUIStyle _buttonAccent;
        private static GUIStyle _tab;
        private static GUIStyle _tabActive;
        private static GUIStyle _toggle;
        private static GUIStyle _textField;
        private static GUIStyle _horizontalSlider;
        private static GUIStyle _horizontalSliderThumb;
        private static GUIStyle _sectionHeader;
        private static GUIStyle _warningLarge;
        private static Texture2D _texSeparator;
        private static Texture2D _texWindow;
        private static Texture2D _texPanel;
        private static Texture2D _texButton;
        private static Texture2D _texButtonHover;
        private static Texture2D _texAccent;
        private static Texture2D _texTabActive;

        public static void Ensure()
        {
            if (_ready)
            {
                return;
            }

            _texWindow = MakeTex(Bg);
            _texPanel = MakeTex(Panel);
            _texButton = MakeTex(new Color(0.14f, 0.14f, 0.17f, 1f));
            _texButtonHover = MakeTex(new Color(0.22f, 0.18f, 0.28f, 1f));
            _texAccent = MakeTex(AccentViolet);
            _texTabActive = MakeTex(new Color(0.2f, 0.35f, 0.45f, 1f));

            _window = new GUIStyle(GUI.skin.window);
            _window.normal.background = _texWindow;
            _window.onNormal.background = _texWindow;
            _window.fontStyle = FontStyle.Bold;
            _window.normal.textColor = AccentYellow;
            _window.padding = new RectOffset(14, 14, 24, 12);

            _box = new GUIStyle(GUI.skin.box);
            _box.normal.background = _texPanel;
            _box.normal.textColor = Text;
            _box.padding = new RectOffset(10, 10, 8, 8);

            _label = new GUIStyle(GUI.skin.label);
            _label.normal.textColor = Text;
            _label.fontStyle = FontStyle.Bold;

            _labelMuted = new GUIStyle(_label);
            _labelMuted.normal.textColor = TextMuted;
            _labelMuted.fontStyle = FontStyle.Normal;
            _labelMuted.wordWrap = true;

            _labelCredit = new GUIStyle(_labelMuted);
            _labelCredit.normal.textColor = new Color(1f, 0.88f, 0.35f, 0.75f);
            _labelCredit.fontSize = 9;
            _labelCredit.alignment = TextAnchor.MiddleRight;
            _labelCredit.padding = new RectOffset(0, 4, 0, 0);

            _button = new GUIStyle(GUI.skin.button);
            _button.normal.background = _texButton;
            _button.hover.background = _texButtonHover;
            _button.active.background = _texAccent;
            _button.normal.textColor = Text;
            _button.hover.textColor = AccentYellow;
            _button.padding = new RectOffset(8, 8, 6, 6);

            _buttonAccent = new GUIStyle(_button);
            _buttonAccent.normal.background = _texAccent;
            _buttonAccent.normal.textColor = AccentYellow;

            _tab = new GUIStyle(_button);
            _tab.fontStyle = FontStyle.Normal;
            _tab.padding = new RectOffset(4, 4, 4, 4);

            _tabActive = new GUIStyle(_tab);
            _tabActive.normal.background = _texTabActive;
            _tabActive.normal.textColor = AccentCyan;
            _tabActive.fontStyle = FontStyle.Bold;

            _toggle = new GUIStyle(GUI.skin.toggle);
            _toggle.normal.textColor = Text;

            _textField = new GUIStyle(GUI.skin.textField);
            _textField.normal.textColor = AccentYellow;
            _textField.normal.background = _texPanel;

            _horizontalSlider = new GUIStyle(GUI.skin.horizontalSlider);
            _horizontalSlider.normal.background = _texPanel;
            _horizontalSliderThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
            _horizontalSliderThumb.normal.background = _texAccent;

            _sectionHeader = new GUIStyle(_label);
            _sectionHeader.fontSize = 13;
            _sectionHeader.normal.textColor = AccentCyan;
            _sectionHeader.fontStyle = FontStyle.Bold;
            _sectionHeader.padding = new RectOffset(0, 0, 2, 2);

            _warningLarge = new GUIStyle(_label);
            _warningLarge.fontSize = 17;
            _warningLarge.fontStyle = FontStyle.Bold;
            _warningLarge.normal.textColor = AccentYellow;
            _warningLarge.alignment = TextAnchor.MiddleCenter;
            _warningLarge.wordWrap = true;
            _warningLarge.padding = new RectOffset(6, 6, 10, 10);

            _texSeparator = MakeTex(new Color(AccentViolet.r, AccentViolet.g, AccentViolet.b, 0.35f));

            _ready = true;
        }

        public static GUIStyle Window => _window;
        public static GUIStyle Box => _box;
        public static GUIStyle Label => _label;
        public static GUIStyle LabelMuted => _labelMuted;
        public static GUIStyle LabelCredit => _labelCredit;
        public static GUIStyle Button => _button;
        public static GUIStyle ButtonAccent => _buttonAccent;
        public static GUIStyle Tab => _tab;
        public static GUIStyle TabActive => _tabActive;
        public static GUIStyle Toggle => _toggle;
        public static GUIStyle TextField => _textField;
        public static GUIStyle HorizontalSlider => _horizontalSlider;
        public static GUIStyle HorizontalSliderThumb => _horizontalSliderThumb;
        public static GUIStyle SectionHeader => _sectionHeader;
        public static GUIStyle WarningLarge => _warningLarge;
        public static Texture2D SeparatorTex => _texSeparator;

        private static Texture2D MakeTex(Color c)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, c);
            tex.Apply();
            return tex;
        }

        public static void DrawColorSwatch(Rect rect, Color color)
        {
            Color prev = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = prev;
            GUI.Box(rect, GUIContent.none, Box);
        }

        public static void DrawGlowPreview(Rect rect, Color glowColor, float alpha)
        {
            Color c = glowColor;
            c.a = alpha;
            Color prev = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = prev;
            GUI.Box(rect, GUIContent.none, Box);
        }
    }
}
