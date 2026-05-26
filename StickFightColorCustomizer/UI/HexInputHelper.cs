using StickFightColorCustomizer.Core;
using UnityEngine;

namespace StickFightColorCustomizer.UI
{
    internal sealed class HexFieldState
    {
        private string _lastAppliedHex = "";
        private float _lastApplyTime;
        private const float DebounceSeconds = 0.15f;

        public void SyncFromColorIfNotFocused(string controlName, ref string hexField, Color color)
        {
            if (GUI.GetNameOfFocusedControl() == controlName)
            {
                return;
            }

            hexField = ColorUtil.ToHex(color);
            _lastAppliedHex = hexField;
        }

        public bool DrawAndTryApplyLive(
            ref string hexField,
            string controlName,
            System.Action<Color> onColorApplied,
            bool applyVisualWhenDisabled)
        {
            bool applied = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hex", ColorMenuTheme.Label, GUILayout.Width(30f));
            GUI.SetNextControlName(controlName);
            string next = GUILayout.TextField(hexField, ColorMenuTheme.TextField, GUILayout.Width(110f));

            if (next != hexField)
            {
                hexField = next;
            }

            if (Event.current.type == EventType.KeyDown
                && Event.current.keyCode == KeyCode.Return
                && GUI.GetNameOfFocusedControl() == controlName)
            {
                if (TryApplyNow(ref hexField, onColorApplied))
                {
                    applied = true;
                }

                GuiFocusHelper.ReleaseTextFieldFocus();
                Event.current.Use();
            }
            else if (IsCompleteHex(hexField) && hexField != _lastAppliedHex)
            {
                float now = Time.realtimeSinceStartup;
                if (now - _lastApplyTime >= DebounceSeconds)
                {
                    if (TryApplyNow(ref hexField, onColorApplied))
                    {
                        applied = applyVisualWhenDisabled;
                    }
                }
            }

            if (GUILayout.Button("OK", ColorMenuTheme.ButtonAccent, GUILayout.Width(36f)))
            {
                if (TryApplyNow(ref hexField, onColorApplied))
                {
                    applied = true;
                }

                GuiFocusHelper.ReleaseTextFieldFocus();
            }

            if (GUILayout.Button("Copiar", ColorMenuTheme.Button, GUILayout.Width(48f)))
            {
                CopyHex(hexField);
            }

            GUILayout.EndHorizontal();
            return applied;
        }

        public bool TryApplyNow(ref string hexField, System.Action<Color> onColorApplied)
        {
            Color parsed;
            if (!ColorUtil.TryParseHex(hexField, out parsed))
            {
                return false;
            }

            onColorApplied(parsed);
            _lastAppliedHex = ColorUtil.ToHex(parsed);
            hexField = _lastAppliedHex;
            _lastApplyTime = Time.realtimeSinceStartup;
            return true;
        }

        public void ForceSyncHex(string hex)
        {
            _lastAppliedHex = hex ?? "";
        }

        private static bool IsCompleteHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return false;
            }

            string s = hex.Trim();
            if (s.StartsWith("#"))
            {
                s = s.Substring(1);
            }

            return s.Length == 6 || s.Length == 8;
        }

        private static void CopyHex(string hex)
        {
            TextEditor te = new TextEditor();
            te.text = hex ?? "";
            te.SelectAll();
            te.Copy();
        }
    }
}
