# Cosméticos — Troubleshooting (SFCC)

Guía para otra IA o mantenimiento. Proyecto: `StickFightColorCustomizer` / `AlkaRealSkinChanger.dll`.

## Arquitectura actual (mega fix)

```
Controller
└── Renderers
    ├── SFCC_CosmeticPivot     ← pecho mundo, rotación identidad, escala 1
    │   └── SFCC_Objects       ← orbes (localPosition desde InverseTransformPoint)
    ├── spineRenderer          → tops, sort de objetos
    ├── legRenderer / legRenderer2
    │   └── LeftLeg / RightLeg
    │       └── SFCC_Shoe_Holder_L/R → SFCC_Shoe_L/R
    └── headRenderer           → hat (patrón de referencia)
```

**Pivot:** `Core/CosmeticFollowPivot.cs` — `EnsurePivot`, `TryGetChestWorld`, `ApplyPivotWorldPosition`, ejes de órbita.

---

## Síntoma → causa → fix

| Síntoma | Causa probable | Fix |
|---------|----------------|-----|
| Orbes en columna vertical | Offset solo en un eje o parent con escala/rotación del hueso | Parent en `SFCC_CosmeticPivot`; órbita `cos/sin` con `cam.right`/`cam.up`; posición con `InverseTransformPoint` |
| Orbes lejos del stickman | `position` mundo sin seguir `Renderers` | Pivot bajo `Renderers`; `pivot.position = chestWorld`; `root.localPosition = 0` |
| Orbes delante del cuerpo | `sortingOrder` demasiado alto | `sortingOrder = spine - 10`; `BehindBody = true` |
| Orbes jitter al caminar | Sync en cada pierna | Sync objetos **solo** en `spineRenderer` en patch; tick 0.05s en `TickObjects` |
| Zapatos invisibles | Parent directo en hueso con `lossyScale` extremo | Holder con escala inversa; si falla → anchor mundo |
| Zapatos estirados en V | Sprite hijo del hueso sin neutralizar escala | `SFCC_Shoe_Holder_*` con `localScale = 1/lossyScale` |
| Zapatos en torso / idle flotando | `TryGetShoeWorldPosition` o lerp al torso | Usar `GetFootWorldPosition(footLine)` + `GetFootLocalOnLine(0.97)` |
| Zapatos en el mapa lejos | Anchor sin sync o pie no resuelto | `ShoeTickInterval` 0.1s; sync en pierna/rodilla en patch |
| Tops finos / cuerpo visible | `tileX` bajo o ancho estrecho | `tileX` min 0.65; ancho spine × 1.18; underlay solid |
| Tops recoloreados por body | `MaintainSetLineColor` toca SFCC | Ignorar nombres `SFCC_*` en `PlayerColorApplier` |

---

## NO hacer (regresiones conocidas)

1. **No** parentear orbes a `spine.transform` del LineRenderer (escala del hueso distorsiona órbita).
2. **No** usar `InverseTransformVector` para posiciones de órbita (usar `InverseTransformPoint`).
3. **No** parentear zapatos directo al hueso del pie sin holder neutral.
4. **No** usar `TryGetShoeWorldPosition` como fuente principal de posición.
5. **No** poner `ObjectSortBoost` positivo grande si deben ir detrás del cuerpo.
6. **No** sync de objetos en cada `legRenderer` (solo `spineRenderer`).

---

## Historial de enfoques

### Zapatos

| Versión | Enfoque | Resultado |
|---------|---------|-----------|
| v1 | Primer LineRenderer en pierna | Rodilla / torso |
| v2 | Parent hueso + local | Visible, estirado |
| v3 | Controller + mundo + escala 0.10–0.28 | Funcionó (visible) |
| v4 | World + `TryGetShoeWorldPosition` | Flotaban en idle |
| v5–v6 | Parent `footLine` sin holder | Invisible |
| **Actual** | Holder en pie + fallback anchor 0.25–0.50 | Pie en render + visible |

### Objetos

| Versión | Enfoque | Resultado |
|---------|---------|-----------|
| Early | World position suelta | Despegados del avatar |
| Mid | Parent `spine.transform` | Columna / órbita rota |
| **Actual** | `SFCC_CosmeticPivot` + local orbit | Círculo en pantalla, sigue al torso |

---

## Logs de diagnóstico

Cada ~2s en lobby (muestra, no spam):

- `[SFCC-Objects]` — `chestWorld`, `offsetWorld`, `partLocal`, `partWorld`, `phase`
- `[SFCC-Shoe]` — `footLine`, `lossyScale`, `holderScale`, modo holder/anchor

Creación (una vez): `CosmeticAttachDiagnostics.LogSpriteOnce`.

Manual: `PlayerColorApplier.LogRendererPaths(controller)`.

---

## Checklist visual esperado (lobby)

| Prueba | Esperado |
|--------|----------|
| Idle | Orbes en anillo al pecho, detrás; zapatos en ambos pies |
| Caminar / saltar | Orbes y zapatos siguen sin despegarse |
| Tops | Camiseta cubre torso, sin líneas del stickman atravesando |
| Hat | Sin regresión (parent en cabeza) |
| MP | Remotos con sync existente en patch |

Build: `build-all.ps1` con el juego cerrado.

---

## Archivos tocados en mega fix

| Archivo | Rol |
|---------|-----|
| `Core/CosmeticFollowPivot.cs` | Pivot único bajo Renderers |
| `Core/ObjectsAttachmentRenderer.cs` | Órbita local desde pivot |
| `Core/ShoeAttachmentRenderer.cs` | Holder + anchor fallback |
| `Core/CosmeticLineAttachUtil.cs` | `TryResolveFootLine`, `GetFootWorldPosition` |
| `Core/CosmeticAttachDiagnostics.cs` | Logs periódicos pivot/orbe/pie |
| `Patches/SetLinePositionsPatch.cs` | Objetos solo spine |
| `Hosting/ColorCustomizerApp.cs` | `ShoeTickInterval` 0.1s |
| `Core/CosmeticBoneLineMirror.cs` | `tileX` min 0.65 |
| `Core/TopsSpriteFactory.cs` | `ArtGeneration` 14 |
