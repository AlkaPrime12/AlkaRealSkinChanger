# Build dependencies (not included in Git)

Copy these files from your **Stick Fight: The Game** install and **MelonLoader** setup into this folder before building:

| File | Typical source |
|------|----------------|
| `Assembly-CSharp.dll` | `StickFight_Data/Managed/` |
| `Assembly-CSharp-firstpass.dll` | `StickFight_Data/Managed/` |
| `UnityEngine.dll` | `StickFight_Data/Managed/` |
| `UnityEngine.UI.dll` | `StickFight_Data/Managed/` (if referenced) |
| `MelonLoader.dll` | `MelonLoader/MelonLoader.dll` (use **0.5.7** match) |
| `0Harmony.dll` | `MelonLoader/Dependencies/0Harmony.dll` |

For BepInEx build, also see `StickFightColorCustomizer.BepInEx/fetch-bepinex-deps.ps1`.

**Do not commit these DLLs to Git** — they are copyrighted game / loader binaries.
