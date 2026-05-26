# AlkaSkin — Alka Real Skin Changer

Mod MelonLoader para **Stick Fight: The Game**. Menú **AlkaSkin** (F6): colores por partes, glow, sombreros, armas, slots de estilo y sync entre jugadores con el mismo mod.

*by alka · v2.3.0*

**Steam Workshop:** ver `WORKSHOP.md` para título, descripción e instalación.

**v2.0.0** — Presets Gold/Neon/Fire/Ice, neon en armas, rueda cromática (HSV) + RGB/hex, 3 slots de estilo, menú en inglés por defecto + pestaña Settings (EN/ES), fix lobby MP (solo tu skin local; remotos con su `sfcc`).

**v1.9.1** — Fix armas: el modelo está en hijos `collider` con MeshRenderer (ya no se saltan). Mejor detección del arma activa (componente Weapon).

**v1.9.0** — Pestaña Armas: color del arma equipada (mesh, líneas, partículas/muzzle). Solo local. Carga diferida.

**v1.8.0** — Sombreros (Top Hat procedural en cabeza). Pestaña Sombreros. Sync `sfcc_hat` entre mods. Carga diferida: colores/glow/sombrero/red solo si los activas en menú.

**v1.7.1** — Menos lag al cargar rondas (puntos): spawns agrupados, bootstrap MP una vez por sesión, glow limitado a 2 líneas/frame.

**v1.7.0** — Hex en vivo (cuerpo y glow sin pulsar OK). Código de skin: Guardar código / Importar / Copiar (cuerpo + mitades + glow). Glow: color del aura antes de activar.

**v1.6.1** — Menos lag en lobby: RGB solo pintura local throttled (sin publicar sfcc cada frame), `PublishLocal` si el payload no cambió, spawn en lobby no encola remotos, `SetLinePositions` desactivado en menú lobby, debounce/heartbeat más largos con 3+ jugadores.

**v1.6.0** — Comunicación mod-a-mod: ping P2P en lobby y partida (piggyback Ping, vanilla ignorado). Lobby sin lag: debounce `OnLobbyDataUpdate`, preview 1/frame, heartbeat 5 s. Modo seguro sigue compatible con vanilla.

**v1.5.9** — Fix crash `StackOverflow` (IsLocalPlayerFast ↔ IsLocalController). Reintentos de detección no borran mods ya vistos si Steam no devuelve `sfcc` en partida (refresh merge).

**v1.5.8** — Detección MP entre mods: auto-publicación `sfcc` al entrar al lobby y al empezar partida, bootstrap ordenado (refresh + publish → enqueue en frame +2), **3 intentos de detección cada 3 s** (solo encola remotos que faltan), cache Steam→slot en un pass. Menos lag al spawn: apply local debounced, sin pintar todos en cada spawn.

**v1.5.7** — Colores de otros jugadores con mod: sync `sfcc` también en partida (no solo en lobby UI), cola al entrar/spawn/ping. Remotos sin half-color (menos lag). Cache mod se actualiza si llega `sfcc` tarde.

**v1.5.6** — Mitades: índice de huesos por jugador (Hip, Knee_*, etc.) una sola vez; corte en articulación por proyección directa, sin buscar en el árbol cada frame. Intervalo de repintado más largo si el corte no se movió.

**v1.5.5** — Glow arreglado: gradiente visible (alpha en la línea, no start/end a 0), sync por hueso, mezcla con mitades del cuerpo; RemoveAll no borra auras de otros.

**v1.5.4** — Sin lag al entrar otros jugadores: ignora huesos remotos en SetLinePositions, sin re-pintar a todos, cache de slots Steam/mod.

**v1.5.3** — Menú F6 vuelve a ser clicable (botones/sliders); liberación de foco solo al cerrar menú para el chat.

**v1.5.2** — Corrige customizer roto tras v1.5.1 (jugador local en solo, colores/gradientes vuelven a aplicarse).

**v1.5.1** — Half-color (gradientes) sin lag sostenido en partida; mantiene v1.5 MP/sync/chat.

**v1.5.0** — MP sin lag, detección de jugadores con mod, chat libre.

## Requisitos

- Stick Fight con **MelonLoader 0.5.7 (x86)** o **BepInEx 5.4.x (x86)** — no ambos a la vez.
- Visual Studio / MSBuild con **.NET Framework 3.5**.
- DLLs de referencia en `Deps/` (copiadas del juego).

Ver **[INSTALL-LOADERS.md](INSTALL-LOADERS.md)** (MelonLoader vs BepInEx, QOL-Mod) y **[README-BEPINEX.md](README-BEPINEX.md)**.

## Compilar e instalar

**MelonLoader:**

```powershell
cd "StickFightColorCustomizer"
.\build-and-deploy.ps1
```

→ `Mods\AlkaRealSkinChanger.dll`

**BepInEx:**

```powershell
cd "StickFightColorCustomizer.BepInEx"
.\build-bepinex.ps1
```

→ `BepInEx\plugins\AlkaSkin\AlkaSkin.dll`

## Uso en juego

| Tecla | Acción |
|-------|--------|
| **F6** | Abrir / cerrar menú |
| **Escape** | Cerrar menú |
| **T / Y** | Cierran menú y liberan teclado para chat del juego |
| Sliders RGB / HSV | Editar parte seleccionada |
| Presets | Fire, Ice, RGB, Shadow, Neon |
| Guardar | `UserData/ColorCustomizer/config.json` |

## Multijugador (v1.5)

### Detección automática de mod

Solo se aplica cosmética remota si el jugador tiene payload válido en lobby (`sfcc`) o ping con magic SFCC. **Jugadores sin mod = cero trabajo pesado** en tu PC.

### Sync entre jugadores con el mod

1. **Steam Lobby Member Data** (`sfcc`, `sfcc_ok`, `sfcc_ver`)
2. **Ping piggyback** (magic SFCC; canal P2P 0)
3. **Entrada al lobby**: patch `OnLobbyChatUpdate` + ping dirigido al nuevo miembro
4. **Partida**: bootstrap 5 intentos; `OnPlayerJoined` / spawn encolan remotos con mod
5. **Grace 15 s**: si solo llega `sfcc_ok`, no se marca vanilla hasta timeout
6. F6 → Settings: contador **SFCC mods** y último evento de sync

Jugadores con **AlkaSkin MelonLoader** y **AlkaSkin BepInEx** usan el mismo protocolo y se ven mutuamente.

### Gradientes (half-color) v1.5.1

- Cache O(1) del jugador local en el postfix de `SetLinePositions` (sin scan Steam por línea/frame).
- Fast path: si el gradiente cacheado sigue en la línea, no reaplica.
- Máximo 3 líneas con apply pesado por frame; `MarkDirty` al cambiar colores en F6.

### Modo seguro MP (recomendado)

Por defecto activo: sin paquetes P2P custom, sin tocar `m_Colors` en red. Tus colores/RGB/glow/half-color se ven en **tu** cliente; otros con mod ven tus colores vía `sfcc` en lobby.

### Chat y UnityExplorer

Si el chat no responde tras usar el menú F6, cierra el menú con Escape (o T/Y) y espera un instante; el mod libera el foco IMGUI solo con el menú cerrado. Si **UnityExplorer** u otro mod de UI está abierto, puede quedarse el foco en ese mod — ciérralo antes de chatear.

## Pruebas recomendadas en MP

1. Partida 4 jugadores (2 con mod, 2 vanilla): sin tirones al entrar.
2. Half-color + RGB + glow en local: fluido en partida.
3. F6 → Escape → T: escribir en chat y salir con Escape.
4. Jugador con mod entra tarde: colores tras spawn sin lag sostenido.

## Arquitectura

```
MelonLoader
  → ColorCustomizerMod (F6, RGB, scheduler MP)
  → ModPresenceRegistry / MatchEntryColorScheduler
  → Harmony: CreatePlayer, RevivePlayer, OnPlayerSpawned, SetLinePositions
  → PlayerColorApplier (local completo, remoto liviano)
  → SteamLobbyColorSync + ColorPingPacket (opcional)
```

## Referencias de código del juego

- `ControllerHandler.CreatePlayer` — spawn
- `MultiplayerManager.OnPlayerSpawned` — red
- `SetLinePositions.Update` — mantenimiento de líneas (solo local)
