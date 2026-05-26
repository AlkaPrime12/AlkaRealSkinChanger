using UnityEngine;

namespace StickFightColorCustomizer.UI
{
    /// <summary>
    /// Liberar teclado para el chat del juego sin romper clics (hotControl) del menú IMGUI.
    /// </summary>
    public static class GuiFocusHelper
    {
        public const string HexBodyControl = "SFCC_HEX_BODY";
        public const string HexGlowControl = "SFCC_HEX_GLOW";
        public const string HexWeaponControl = "SFCC_HEX_WEAPON";
        public const string HexWingControl = "SFCC_HEX_WING";
        public const string ImportCodeControl = "SFCC_IMPORT_CODE";
        public const string ExportCodeControl = "SFCC_EXPORT_CODE";

        /// <summary>Solo campo de texto / teclado — no tocar ratón ni botones.</summary>
        public static void ReleaseTextFieldFocus()
        {
            GUI.FocusControl(string.Empty);
            GUIUtility.keyboardControl = 0;
        }

        /// <summary>Tras cerrar el menú: suelta teclado y arrastres IMGUI para que T/Y abran chat.</summary>
        public static void ReleaseAllForChat()
        {
            ReleaseTextFieldFocus();
            GUIUtility.hotControl = 0;
        }

        public static bool IsOurControlFocused()
        {
            string focused = GUI.GetNameOfFocusedControl();
            return !string.IsNullOrEmpty(focused) && focused.StartsWith("SFCC_");
        }

        /// <summary>Solo cuando el menú está cerrado: limpia foco SFCC colgado sin tocar hotControl ajeno.</summary>
        public static void ClearStuckSfccFocusWhenMenuClosed()
        {
            if (IsOurControlFocused())
            {
                ReleaseAllForChat();
            }
        }
    }
}
