using UnityEngine;

namespace StickFightColorCustomizer.UI
{
    /// <summary>
    /// Helpers visuales reutilizables para hacer las pestañas más uniformes.
    /// </summary>
    public static class ColorMenuLayout
    {
        public static void BeginSection(string title)
        {
            GUILayout.BeginVertical(ColorMenuTheme.Box);
            if (!string.IsNullOrEmpty(title))
            {
                GUILayout.Label(title, ColorMenuTheme.SectionHeader);
                Separator();
            }
        }

        public static void EndSection()
        {
            GUILayout.EndVertical();
            GUILayout.Space(4f);
        }

        public static void Separator()
        {
            Rect r = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
            GUI.DrawTexture(r, ColorMenuTheme.SeparatorTex);
            GUILayout.Space(2f);
        }

        public static void HintLabel(string text)
        {
            GUILayout.Label(text, ColorMenuTheme.LabelMuted);
        }
    }
}
