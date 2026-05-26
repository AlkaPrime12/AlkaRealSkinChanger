# Compilar AlkaSkin (MelonLoader y BepInEx)

## Estructura del repo

| Carpeta | Salida | Loader |
|---------|--------|--------|
| `StickFightColorCustomizer/` | `AlkaRealSkinChanger.dll` | MelonLoader 0.5.7 x86 |
| `StickFightColorCustomizer.BepInEx/` | `AlkaSkin.dll` | BepInEx 5.4.x x86 |
| `StickFightColorCustomizer.Core/` | `StickFightColorCustomizer.Core.dll` (opcional) | Ninguno (biblioteca) |

Código compartido: `Hosting/ColorCustomizerApp.cs` + `Core/`, `Network/`, `Patches/`, `UI/`.

## Requisitos

- Visual Studio / MSBuild
- .NET Framework 3.5
- `Deps/` con DLLs del juego (`Assembly-CSharp`, `UnityEngine`, `MelonLoader`, `0Harmony`, …)
- Para BepInEx: `Deps/BepInEx.dll` (ejecuta `StickFightColorCustomizer.BepInEx\fetch-bepinex-deps.ps1` si falta)

## Compilar todo

Desde la raíz `Centauri sucks2`:

```powershell
.\build-all.ps1
```

O por separado:

```powershell
cd StickFightColorCustomizer
.\build-and-deploy.ps1
```

```powershell
cd StickFightColorCustomizer.BepInEx
.\build-bepinex.ps1
```

## Destino en el juego

| Build | Ruta en Stick Fight |
|-------|---------------------|
| MelonLoader | `Mods\AlkaRealSkinChanger.dll` |
| BepInEx | `BepInEx\plugins\AlkaSkin\AlkaSkin.dll` |

**No instales ambos loaders a la vez.** Ver `INSTALL-LOADERS.md`.

## Visual Studio

Abre `StickFightColorCustomizer.sln` y compila **Release | Any CPU**.
