# AlkaSkin — build BepInEx

Misma funcionalidad y protocolo **SFCC** (`sfcc`, `sfcc_ok`, ping P2P) que la versión MelonLoader. Jugadores ML y BepInEx se ven los colores mutuamente.

## Requisitos

- **Stick Fight: The Game** (Steam, x86)
- **[BepInEx 5.4.x](https://docs.bepinex.dev/)** instalado en la carpeta del juego (no MelonLoader al mismo tiempo)
- Visual Studio / MSBuild (.NET 3.5)

## Instalación BepInEx en Stick Fight

### Descarga directa (DLL, sin ZIP)

- **BepInEx:** [`../release/BepInEx/AlkaSkin.dll`](../release/BepInEx/AlkaSkin.dll)
- **MelonLoader (alternativa):** [`../release/MelonLoader/AlkaRealSkinChanger.dll`](../release/MelonLoader/AlkaRealSkinChanger.dll)

Descarga el `.dll` y pégalo en la ruta correcta. No uses ZIP (rompe la instalación si queda en carpeta anidada).

1. Descarga **BepInEx 5.4.x** para Unity Mono / x86.
2. Extrae en `Stick Fight The Game\` (debe existir `BepInEx\core\BepInEx.dll`).
3. Ejecuta el juego una vez para generar carpetas.

## Desinstalar MelonLoader (importante)

MelonLoader y BepInEx **no deben** convivir (ambos usan `version.dll` / inyección).

1. Cierra el juego.
2. Haz backup de `Mods\` y `UserData\`.
3. Usa el desinstalador de MelonLoader o elimina `version.dll`, carpeta `MelonLoader\` y `Mods\` según tu instalación.
4. Instala BepInEx como arriba.

La primera vez que arranques AlkaSkin BepInEx, si existe `UserData\ColorCustomizer\config.json`, se copia a `BepInEx\config\AlkaSkin\config.json`.

## Compilar e instalar AlkaSkin (BepInEx)

```powershell
cd "StickFightColorCustomizer.BepInEx"
.\build-bepinex.ps1
```

Salida: `Stick Fight The Game\BepInEx\plugins\AlkaSkin\AlkaSkin.dll`

## Uso

| Tecla | Acción |
|-------|--------|
| **F6** | Menú AlkaSkin |
| **Escape** | Cerrar menú |

## Coexistencia con QOL-Mod (Monkey)

QOL-Mod es plugin **BepInEx**. Con esta build puedes usar:

- `BepInEx\plugins\AlkaSkin\AlkaSkin.dll`
- QOL-Mod en `BepInEx\plugins\`

Si hay conflictos Harmony, revisa `BepInEx\LogOutput.log`.

## MelonLoader vs BepInEx

| Stack | AlkaSkin | Otros mods |
|-------|----------|------------|
| MelonLoader 0.5.7 | `Mods\AlkaRealSkinChanger.dll` | Lobby Viewer (ML) |
| BepInEx 5.x | `BepInEx\plugins\AlkaSkin\` | QOL-Mod, etc. |

No mezcles loaders en la misma instalación.

## Multijugador

Mismo protocolo SFCC que MelonLoader. En F6 → Settings verás contador de mods detectados y último evento de sync.
