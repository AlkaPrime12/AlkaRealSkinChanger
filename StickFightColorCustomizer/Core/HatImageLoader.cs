using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Loads PNG hat sprites from <ModsDir>/AlkaSkin_Images/ at runtime.
    /// Filenames map to hat ids via a small alias table — e.g. "Gorra de mario.png" → "img_mario".
    /// Sprites get normalised so bounds.size.x ≈ 1.0 world unit, matching the procedural hats.
    /// </summary>
    public static class HatImageLoader
    {
        // hat id → filename (without extension is fine, we strip it).
        // Keep id prefix "img_" so HatSpriteFactory can route through GetSprite.
        public sealed class ImageHatDef
        {
            public string Id;
            public string Label;
            public string FileName;        // filename to look for under AlkaSkin_Images
            public bool   FacesLeft;       // true if the artwork faces the viewer's left
            public float  WidthFactor;     // optional width scaler relative to procedural ratio
            public float  HeightFactor;
            public float  PivotY;          // 0 = bottom (sits on head), high = helmet hangs down
            public float  OffsetX;         // local X nudge in head-radius units (e.g. tactical helmet)

            public ImageHatDef(string id, string label, string fileName,
                                bool facesLeft = true,
                                float widthFactor = 1.0f, float heightFactor = 1.0f,
                                float pivotY = 0.05f,
                                float offsetX = 0f)
            {
                Id = id; Label = label; FileName = fileName;
                FacesLeft = facesLeft;
                WidthFactor = widthFactor;
                HeightFactor = heightFactor;
                PivotY = pivotY;
                OffsetX = offsetX;
            }

            public bool IsHelmetStyle { get { return PivotY > 0.4f; } }
        }

        // PivotY: 0.0 = pivot at bottom of sprite (sits ON TOP of head, like a cap).
        //         0.5 = pivot at vertical centre (helmet engulfs the head).
        //         caps/gorras  → use ~0.05–0.10 so the brim rests on the scalp.
        //         helmets/cascos → use ~0.45–0.55 so the helmet wraps around the head.
        // Helmets need pivotY high (≈0.30-0.35) so the sprite hangs DOWN around the head
        // when the attachment point is at the head's top vertex. WidthFactor/HeightFactor
        // > 1 make the helmet sprite a bit larger than the head so it fully encases it.
        public static readonly ImageHatDef[] Defs =
        {
            // Caps (gorras): the source PNGs all have ~15-25 % transparent padding below the
            // visible brim. With pivotY = 0 the SpriteRenderer anchors at that bottom edge,
            // so the empty padding sits on the head and the cap looks like it's floating
            // a few pixels above. Bumping pivotY to ~0.25 hides the padding inside the head
            // — the cap visually touches the scalp.
            new ImageHatDef("img_mario",       "Mario Cap (HD)",        "Gorra de mario",
                widthFactor: 1.20f, heightFactor: 1.10f, pivotY: 0.25f),
            // Luigi PNG has noticeably more transparent padding below the brim than the
            // Mario one — needs a higher pivotY to sit on the head. Bumped size up
            // slightly so it visually matches the other caps after the pivot shift.
            new ImageHatDef("img_luigi",       "Luigi Cap (HD)",        "Gorra de luigi",
                widthFactor: 1.40f, heightFactor: 1.28f, pivotY: 0.32f),
            new ImageHatDef("img_link",        "Link Cap (HD)",         "Gorra de Link",
                widthFactor: 1.18f, heightFactor: 1.18f, pivotY: 0.22f),
            new ImageHatDef("img_ash",         "Ash Cap (HD)",          "Gorra de Ash",
                widthFactor: 1.22f, heightFactor: 1.10f, pivotY: 0.25f),
            new ImageHatDef("img_bison",       "M. Bison Cap (HD)",     "M Bison Gorra",
                widthFactor: 1.22f, heightFactor: 1.15f, pivotY: 0.28f),
            // Cascos / hood: pivotY alto + bajada extra en HatAttachmentRenderer → tapa la cara.
            new ImageHatDef("img_wizard",      "Wizard Hood (HD)",      "Cabeza de mago",
                widthFactor: 2.05f, heightFactor: 2.05f, pivotY: 0.78f),
            new ImageHatDef("img_samus",       "Samus Helmet (HD)",     "Casco de samus",
                widthFactor: 2.15f, heightFactor: 2.10f, pivotY: 0.78f),
            new ImageHatDef("img_metroid",     "Metroid Helmet (HD)",   "Casco metroid",
                widthFactor: 2.15f, heightFactor: 2.10f, pivotY: 0.78f),
            new ImageHatDef("img_tacticalops", "Tactical Ops (HD)",     "Casco tactical ops",
                widthFactor: 2.10f, heightFactor: 2.08f, pivotY: 0.78f, offsetX: 0.055f),
            new ImageHatDef("img_masterchief", "Master Chief (HD)",     "Master chief casco",
                widthFactor: 2.18f, heightFactor: 2.14f, pivotY: 0.80f),
        };

        public static float GetWidthFactor(string hatId)
        {
            ImageHatDef d = GetDef(hatId);
            return d != null ? d.WidthFactor : 1f;
        }

        public static float GetHeightFactor(string hatId)
        {
            ImageHatDef d = GetDef(hatId);
            return d != null ? d.HeightFactor : 1f;
        }

        public static float GetOffsetX(string hatId)
        {
            ImageHatDef d = GetDef(hatId);
            return d != null ? d.OffsetX : 0f;
        }

        private static readonly Dictionary<string, Sprite> SpriteById = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, ImageHatDef> DefById = new Dictionary<string, ImageHatDef>();
        private static bool _initialised;
        private static bool _loggedInitSummary;

        public static bool IsImageHat(string hatId)
        {
            if (string.IsNullOrEmpty(hatId)) return false;
            return hatId.StartsWith("img_", StringComparison.OrdinalIgnoreCase);
        }

        public static ImageHatDef GetDef(string hatId)
        {
            EnsureInit();
            ImageHatDef def;
            DefById.TryGetValue(hatId, out def);
            return def;
        }

        public static Sprite TryGetSprite(string hatId)
        {
            EnsureInit();
            Sprite s;
            return SpriteById.TryGetValue(hatId, out s) ? s : null;
        }

        public static void EnsureInit()
        {
            if (_initialised) return;
            _initialised = true;

            try
            {
                foreach (ImageHatDef def in Defs)
                {
                    DefById[def.Id] = def;
                }

                string dir = ResolveImagesDirectory();
                if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                {
                    if (!_loggedInitSummary)
                    {
                        _loggedInitSummary = true;
                        ModLog.Info("[SFCC-HatImages] no AlkaSkin_Images folder — image hats disabled");
                    }

                    return;
                }

                int loaded = 0;
                int missing = 0;
                foreach (ImageHatDef def in Defs)
                {
                    string pngPath = Path.Combine(dir, def.FileName + ".png");
                    if (!File.Exists(pngPath))
                    {
                        missing++;
                        continue;
                    }

                    try
                    {
                        byte[] bytes = File.ReadAllBytes(pngPath);
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                        tex.filterMode = FilterMode.Bilinear;
                        tex.wrapMode = TextureWrapMode.Clamp;
                        if (!tex.LoadImage(bytes))
                        {
                            continue;
                        }

                        // Normalise PPU so bounds.size.x ≈ 1.0 world unit (matches the existing
                        // procedural hats which use w=24, PPU=24 → bounds 1.0).
                        float ppu = Mathf.Max(tex.width, 1);
                        Vector2 pivot = new Vector2(0.5f, def.PivotY);
                        Sprite sprite = Sprite.Create(
                            tex,
                            new Rect(0f, 0f, tex.width, tex.height),
                            pivot,
                            ppu);
                        SpriteById[def.Id] = sprite;
                        loaded++;
                    }
                    catch
                    {
                    }
                }

                if (!_loggedInitSummary)
                {
                    _loggedInitSummary = true;
                    ModLog.Info("[SFCC-HatImages] loaded " + loaded + "/" + Defs.Length
                        + (missing > 0 ? " (" + missing + " PNG missing)" : "") + " from " + dir);
                }
            }
            catch (Exception ex)
            {
                if (!_loggedInitSummary)
                {
                    _loggedInitSummary = true;
                    ModLog.Info("[SFCC-HatImages] init failed: " + ex.Message);
                }
            }
        }

        private static string ResolveImagesDirectory()
        {
            // 1. Next to the loaded mod DLL, in an "AlkaSkin_Images" subfolder.
            try
            {
                string asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!string.IsNullOrEmpty(asmDir))
                {
                    string cand = Path.Combine(asmDir, "AlkaSkin_Images");
                    if (Directory.Exists(cand)) return cand;
                }
            }
            catch { }

            // 2. <gameRoot>/Mods/AlkaSkin_Images (MelonLoader layout)
            try
            {
                string gameRoot = Path.GetDirectoryName(Application.dataPath);
                if (!string.IsNullOrEmpty(gameRoot))
                {
                    string cand = Path.Combine(Path.Combine(gameRoot, "Mods"), "AlkaSkin_Images");
                    if (Directory.Exists(cand)) return cand;

                    // 3. <gameRoot>/BepInEx/plugins/AlkaSkin/AlkaSkin_Images (BepInEx layout)
                    string bep = Path.Combine(Path.Combine(Path.Combine(Path.Combine(gameRoot, "BepInEx"), "plugins"), "AlkaSkin"), "AlkaSkin_Images");
                    if (Directory.Exists(bep)) return bep;
                }
            }
            catch { }

            return null;
        }

        public static void ClearCache()
        {
            SpriteById.Clear();
            _initialised = false;
        }
    }
}
