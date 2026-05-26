# Guía de loaders — Stick Fight

## Problema: MelonLoader bloquea BepInEx

Si instalaste **MelonLoader**, mods **BepInEx** (p. ej. [QOL-Mod / Monkey](https://github.com/Mn0ky/QOL-Mod)) no cargan. Solo un loader puede inyectar el juego a la vez.

## Opción A — Solo BepInEx (recomendado con QOL)

1. Desinstala MelonLoader.
2. Instala BepInEx 5.4.x (x86).
3. Compila AlkaSkin BepInEx: `StickFightColorCustomizer.BepInEx\build-bepinex.ps1`
4. Coloca QOL-Mod y otros plugins en `BepInEx\plugins\`.

## Opción B — Solo MelonLoader

1. Sin BepInEx.
2. MelonLoader **0.5.7** (x86) — no uses 0.6+ en Unity 5.6.
3. `build-and-deploy.ps1` → `Mods\AlkaRealSkinChanger.dll`
4. Puedes añadir **Lobby Viewer** u otros mods ML.

## Sync de colores entre jugadores

- Ambos necesitan **AlkaSkin** (ML o BepInEx, mismo protocolo `sfcc`).
- Vanilla u otros mods de colores no se detectan automáticamente.
- Host publica `sfcc` al entrar; ping P2P refuerza detección.

## Pruebas MP sugeridas

1. Dos jugadores con AlkaSkin → colores visibles en lobby/partida.
2. Uno entra tarde → detección en pocos segundos (chat enter + ping).
3. ML + BepInEx en dos PCs → deben verse mutuamente si ambos tienen AlkaSkin.
