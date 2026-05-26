using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Hosting;
using StickFightColorCustomizer.Models;
using StyleSlotManifest = StickFightColorCustomizer.Models.StyleSlotManifest;
using StyleSlotEntry = StickFightColorCustomizer.Models.StyleSlotEntry;
using StickFightColorCustomizer.Network;
using UnityEngine;

namespace StickFightColorCustomizer.UI
{
    public sealed class ColorMenuGUI
    {
        private readonly Hosting.ColorCustomizerApp _mod;
        private Rect _windowRect = new Rect(50f, 40f, 540f, 680f);
        private bool _resizingMenu;
        private const float MenuMinWidth = 380f;
        private const float MenuMinHeight = 500f;
        private const float MenuMaxWidth = 920f;
        private const float MenuMaxHeight = 950f;
        private Vector2 _scroll;
        private int _tab;
        private int _selectedPart;
        private string _hexBody = "#FFFFFF";
        private string _hexGlow = "#4DFFFF";
        private string _hexWeapon = "#FFFFFF";
        private string _statusMessage = "";
        private string _exportCode = "";
        private string _importCode = "";
        private const string ModCredit = ModBranding.CreditLine;

        private readonly HexFieldState _hexBodyField = new HexFieldState();
        private readonly HexFieldState _hexGlowField = new HexFieldState();
        private readonly HexFieldState _hexWeaponField = new HexFieldState();
        private readonly ColorPickerPanel _bodyPicker = new ColorPickerPanel();
        private readonly ColorPickerPanel _glowPicker = new ColorPickerPanel();
        private readonly ColorPickerPanel _weaponPicker = new ColorPickerPanel();
        private readonly ColorPickerPanel _halvesPicker = new ColorPickerPanel();
        private readonly ColorPickerPanel _wingPicker = new ColorPickerPanel();

        private readonly bool[] _tabInitialized = new bool[11];
        private int _halfEditPart;
        private Vector2 _slotsScroll;
        private readonly System.Collections.Generic.Dictionary<int, string> _slotNameDraft =
            new System.Collections.Generic.Dictionary<int, string>();

        private static readonly StickColorPart[] HalfEditParts =
        {
            StickColorPart.Spine, StickColorPart.HandLeft, StickColorPart.HandRight,
            StickColorPart.LegLeft, StickColorPart.LegRight
        };

        private static readonly string[] HalfEditPartKeys =
        {
            "part_spine", "part_arm_l", "part_arm_r", "part_leg_l", "part_leg_r"
        };

        private static readonly string[] RendererKeys =
        {
            "headRenderer", "spineRenderer", "legRenderer", "legRenderer2",
            "handRenderer", "handRenderer2", "", "Wings"
        };

        public bool Visible { get; set; }

        public ColorMenuGUI(ColorCustomizerApp mod)
        {
            _mod = mod;
        }

        public void Draw()
        {
            if (!Visible)
            {
                return;
            }

            ColorMenuTheme.Ensure();
            GUI.skin = null;
            ApplyWindowSizeFromConfig();
            _windowRect = GUI.Window(94721, _windowRect, DrawWindow, ModBranding.WindowTitle, ColorMenuTheme.Window);
            if (_resizingMenu && Event.current.type == EventType.MouseUp)
            {
                _resizingMenu = false;
                PersistWindowSize();
            }
        }

        private void ApplyWindowSizeFromConfig()
        {
            if (_mod == null || _mod.Config == null)
            {
                return;
            }

            _windowRect.width = Mathf.Clamp(_mod.Config.MenuWindowWidth, MenuMinWidth, MenuMaxWidth);
            _windowRect.height = Mathf.Clamp(_mod.Config.MenuWindowHeight, MenuMinHeight, MenuMaxHeight);
        }

        private void PersistWindowSize()
        {
            if (_mod == null || _mod.Config == null)
            {
                return;
            }

            _mod.Config.MenuWindowWidth = _windowRect.width;
            _mod.Config.MenuWindowHeight = _windowRect.height;
        }

        private void NudgeMenuSize(float deltaW, float deltaH)
        {
            ApplyWindowSizeFromConfig();
            _windowRect.width = Mathf.Clamp(_windowRect.width + deltaW, MenuMinWidth, MenuMaxWidth);
            _windowRect.height = Mathf.Clamp(_windowRect.height + deltaH, MenuMinHeight, MenuMaxHeight);
            PersistWindowSize();
        }

        private void DrawWindow(int id)
        {
            DrawTabs();
            _scroll = GUILayout.BeginScrollView(_scroll);

            EnsureTabInitialized(_tab);

            switch (_tab)
            {
                case 0: DrawTabBody(); break;
                case 1: DrawTabGlow(); break;
                case 2: DrawTabHats(); break;
                case 3: DrawTabWingCust(); break;
                case 4: DrawTabShoes(); break;
                case 5: DrawTabTops(); break;
                case 6: DrawTabObjects(); break;
                case 7: DrawTabWeapons(); break;
                case 8: DrawTabSlots(); break;
                case 9: DrawTabAdvanced(); break;
                case 10: DrawTabSettings(); break;
            }

            if (!string.IsNullOrEmpty(_statusMessage))
            {
                GUILayout.Label(_statusMessage, ColorMenuTheme.LabelMuted);
            }

            GUILayout.Space(4f);
            GUILayout.Label(MenuLocalization.T("menu_hint_keys"), ColorMenuTheme.LabelMuted);
            GUILayout.EndScrollView();

            GUILayout.Label(ModCredit, ColorMenuTheme.LabelCredit, GUILayout.Height(12f));

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape
                    || Event.current.keyCode == KeyCode.T
                    || Event.current.keyCode == KeyCode.Y)
                {
                    _mod.SetMenuVisible(false);
                    GuiFocusHelper.ReleaseAllForChat();
                    Event.current.Use();
                }
            }

            GUI.DragWindow(new Rect(0f, 0f, _windowRect.width - 24f, 24f));
            DrawMenuResizeGrip();
        }

        private void DrawMenuResizeGrip()
        {
            const float grip = 18f;
            Rect gripRect = new Rect(_windowRect.width - grip, _windowRect.height - grip, grip, grip);
            GUI.Label(gripRect, "◢", ColorMenuTheme.LabelMuted);

            Event e = Event.current;
            Vector2 mouseInWindow = e.mousePosition - _windowRect.position;
            if (e.type == EventType.MouseDown && gripRect.Contains(mouseInWindow))
            {
                _resizingMenu = true;
                e.Use();
            }

            if (_resizingMenu && e.type == EventType.MouseDrag)
            {
                _windowRect.width = Mathf.Clamp(mouseInWindow.x, MenuMinWidth, MenuMaxWidth);
                _windowRect.height = Mathf.Clamp(mouseInWindow.y, MenuMinHeight, MenuMaxHeight);
                e.Use();
            }
        }

        private void DrawTabs()
        {
            string[] tabLabels = MenuLocalization.GetTabLabels();
            GUILayout.BeginHorizontal();
            for (int i = 0; i < tabLabels.Length; i++)
            {
                GUIStyle style = _tab == i ? ColorMenuTheme.TabActive : ColorMenuTheme.Tab;
                if (GUILayout.Button(tabLabels[i], style, GUILayout.Height(28f)))
                {
                    _tab = i;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);
        }

        private void EnsureTabInitialized(int tab)
        {
            if (tab < 0 || tab >= _tabInitialized.Length || _tabInitialized[tab])
            {
                return;
            }

            _tabInitialized[tab] = true;
            if (tab == 0 || tab == 1 || tab == 4 || tab == 5 || tab == 6)
            {
                RefreshUiFromConfig();
            }
        }

        private void DrawTabBody()
        {
            GUILayout.BeginVertical(ColorMenuTheme.Box);
            bool bodyActive = GUILayout.Toggle(
                _mod.Config.BodyCustomizationActive,
                MenuLocalization.T("body_enable"),
                ColorMenuTheme.Toggle);
            if (bodyActive != _mod.Config.BodyCustomizationActive)
            {
                _mod.Config.BodyCustomizationActive = bodyActive;
                if (bodyActive)
                {
                    LineMaintainThrottle.MarkDirty();
                    ApplyBodyAndSync();
                    _statusMessage = MenuLocalization.T("body_enabled");
                }
                else
                {
                    _statusMessage = MenuLocalization.T("body_disabled");
                }
            }

            if (!_mod.Config.BodyCustomizationActive)
            {
                GUILayout.Label(MenuLocalization.T("body_enable_hint"), ColorMenuTheme.LabelMuted);
                GUILayout.EndVertical();
                return;
            }

            GUILayout.EndVertical();
            GUILayout.Space(4f);

            StickColorPart part = (StickColorPart)_selectedPart;
            Color current = _mod.Config.Colors.Get(part);
            _hexBodyField.SyncFromColorIfNotFocused(GuiFocusHelper.HexBodyControl, ref _hexBody, current);

            GUILayout.BeginVertical(ColorMenuTheme.Box);
            GUILayout.Label(MenuLocalization.T("body_part"), ColorMenuTheme.Label);
            _selectedPart = GUILayout.SelectionGrid(_selectedPart, MenuLocalization.GetPartLabels(), 2, ColorMenuTheme.Button);
            if (_selectedPart < 0)
            {
                _selectedPart = 0;
            }

            _bodyPicker.SyncHex(current);
            current = _bodyPicker.Draw(
                current,
                GuiFocusHelper.HexBodyControl,
                c =>
                {
                    _mod.Config.Colors.Set(part, c);
                    ApplyBodyAndSync();
                },
                DrawChannel);
            _mod.Config.Colors.Set(part, current);

            DrawStylePresetsRow();

            GUILayout.Space(8f);
            DrawBodyHalfSection();

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(MenuLocalization.T("repaint_body"), ColorMenuTheme.ButtonAccent))
            {
                _mod.ApplyBodyOnly();
                _statusMessage = MenuLocalization.T("body_applied");
            }
            if (GUILayout.Button(MenuLocalization.T("reset_part"), ColorMenuTheme.Button))
            {
                _mod.Config.Colors.Set(part, Color.white);
                _hexBody = "#FFFFFF";
                _hexBodyField.ForceSyncHex(_hexBody);
                ApplyBodyAndSync();
                _statusMessage = MenuLocalization.T("part_reset");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawTabHats()
        {
            HatSettings hat = _mod.Config.Hat ?? new HatSettings();
            ColorMenuLayout.BeginSection(MenuLocalization.T("tab_hats"));
            ColorMenuLayout.HintLabel(MenuLocalization.T("hat_hint"));

            bool enabled = GUILayout.Toggle(hat.Enabled, MenuLocalization.T("hat_enable"), ColorMenuTheme.Toggle);
            if (enabled != hat.Enabled)
            {
                hat.Enabled = enabled;
                if (enabled && (string.IsNullOrEmpty(hat.HatId) || hat.HatId == "none"))
                {
                    hat.HatId = "tophat";
                }

                _mod.Config.Hat = hat;
                _mod.ApplyHatOnly();
                _statusMessage = enabled ? MenuLocalization.T("hat_on") : MenuLocalization.T("hat_off");
            }

            if (!hat.Enabled)
            {
                ColorMenuLayout.HintLabel(MenuLocalization.T("hat_pick"));
                ColorMenuLayout.EndSection();
                return;
            }

            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("hat_classic"), ColorMenuTheme.Label);
            DrawHatClassicGrid(hat);

            GUILayout.Space(8f);
            GUILayout.Label(MenuLocalization.T("hat_categories"), ColorMenuTheme.Label);
            ColorMenuLayout.HintLabel(MenuLocalization.T("hat_category_click_hint"));
            DrawHatCategoryRows(hat);

            ColorMenuLayout.EndSection();
        }

        private int _hatOpenCategoryIndex = -1;

        private void DrawHatClassicGrid(HatSettings hat)
        {
            const int columns = 4;
            const float cellSize = 72f;
            const float previewSize = 52f;
            int col = 0;
            GUILayout.BeginHorizontal();
            for (int i = 0; i < HatCatalog.ClassicEntries.Length; i++)
            {
                HatCatalogEntry entry = HatCatalog.ClassicEntries[i];
                if (entry.Id == "none")
                {
                    continue;
                }

                DrawHatPickerCell(hat, entry.Id, entry.Label, cellSize, previewSize, ref col, columns);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawHatCategoryRows(HatSettings hat)
        {
            const float rowH = 72f;
            const float icon = 48f;

            for (int c = 0; c < HatCategoryCatalog.Categories.Length; c++)
            {
                HatCategoryDef cat = HatCategoryCatalog.Categories[c];
                string showId = cat.RepresentativeId;
                bool rowSelected = false;
                HatCategoryDef wornCat;
                if (HatCategoryCatalog.TryGetCategoryForVariant(hat.HatId, out wornCat))
                {
                    rowSelected = wornCat.CategoryId == cat.CategoryId;
                    if (rowSelected)
                    {
                        showId = hat.HatId;
                    }
                }

                bool panelOpen = _hatOpenCategoryIndex == c;
                GUILayout.BeginHorizontal(GUILayout.Height(rowH));
                Rect rowRect = GUILayoutUtility.GetRect(420f, rowH, ColorMenuTheme.Button);

                GUI.Box(rowRect, GUIContent.none, rowSelected || panelOpen ? ColorMenuTheme.TabActive : ColorMenuTheme.Button);
                DrawHatSpriteInRect(showId, rowRect, icon, 8f);

                GUI.Label(new Rect(rowRect.x + icon + 10f, rowRect.y + 10f, 150f, 20f), cat.Label, ColorMenuTheme.Label);
                GUI.Label(
                    new Rect(rowRect.x + icon + 10f, rowRect.y + 30f, 180f, 14f),
                    HatCategoryCatalog.GetVariantLabel(showId),
                    ColorMenuTheme.LabelMuted);

                Rect openBtn = new Rect(rowRect.xMax - 128f, rowRect.y + 18f, 118f, 36f);
                string btnLabel = panelOpen
                    ? MenuLocalization.T("hat_variants_close")
                    : MenuLocalization.T("hat_variants_open");
                if (GUI.Button(openBtn, btnLabel, ColorMenuTheme.ButtonAccent))
                {
                    _hatOpenCategoryIndex = panelOpen ? -1 : c;
                }
                else if (Event.current.type == EventType.MouseUp
                    && rowRect.Contains(Event.current.mousePosition)
                    && !openBtn.Contains(Event.current.mousePosition))
                {
                    _hatOpenCategoryIndex = panelOpen ? -1 : c;
                }

                GUILayout.EndHorizontal();

                if (panelOpen)
                {
                    DrawHatCategoryPanel(hat, cat);
                    GUILayout.Space(4f);
                }

                GUILayout.Space(3f);
            }
        }

        private void DrawHatCategoryPanel(HatSettings hat, HatCategoryDef cat)
        {
            ColorMenuLayout.BeginSection(cat.Label + " — " + MenuLocalization.T("hat_pick_variant"));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(MenuLocalization.T("hat_close_panel"), ColorMenuTheme.Button, GUILayout.Width(100f)))
            {
                _hatOpenCategoryIndex = -1;
            }

            GUILayout.EndHorizontal();

            const int columns = 5;
            const float cellSize = 76f;
            const float previewSize = 58f;
            int col = 0;
            GUILayout.BeginHorizontal();
            for (int i = 0; i < cat.VariantIds.Length; i++)
            {
                string vid = cat.VariantIds[i];
                string label = "#" + (i + 1);
                DrawHatPickerCell(hat, vid, label, cellSize, previewSize, ref col, columns);
            }

            GUILayout.EndHorizontal();
            ColorMenuLayout.EndSection();
        }

        private void DrawHatPickerCell(
            HatSettings hat, string id, string label, float cellSize, float previewSize,
            ref int col, int columns)
        {
            bool selected = hat.HatId == id;
            GUIStyle style = selected ? ColorMenuTheme.TabActive : ColorMenuTheme.Button;
            GUILayout.BeginVertical(GUILayout.Width(cellSize));
            Rect cellRect = GUILayoutUtility.GetRect(cellSize, cellSize, style);
            if (GUI.Button(cellRect, GUIContent.none, style))
            {
                SelectHatVariant(hat, id, label);
            }

            DrawHatSpriteInRect(id, cellRect, previewSize, 6f);
            GUI.Label(
                new Rect(cellRect.x, cellRect.y + cellRect.height - 16f, cellRect.width, 14f),
                label,
                ColorMenuTheme.LabelMuted);
            GUILayout.EndVertical();
            col++;
            if (col >= columns)
            {
                col = 0;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }

        private static void DrawHatSpriteInRect(string hatId, Rect cellRect, float previewSize, float padY)
        {
            if (hatId == "none")
            {
                return;
            }

            Sprite preview = HatSpriteFactory.GetSprite(hatId);
            if (preview == null || preview.texture == null)
            {
                return;
            }

            Texture2D tex = preview.texture;
            float aspect = (float)tex.width / Mathf.Max(1, tex.height);
            float w = previewSize;
            float h = previewSize;
            if (aspect > 1f) { h = previewSize / aspect; }
            else { w = previewSize * aspect; }
            Rect imgRect = new Rect(
                cellRect.x + (cellRect.width - w) * 0.5f,
                cellRect.y + padY,
                w,
                h);
            GUI.DrawTexture(imgRect, tex, ScaleMode.ScaleToFit, true);
        }

        private void SelectHatVariant(HatSettings hat, string hatId, string label)
        {
            if (hat.HatId == hatId)
            {
                return;
            }

            hat.HatId = hatId;
            _mod.Config.Hat = hat;
            _mod.ApplyHatOnly();
            _statusMessage = MenuLocalization.T("tab_hats") + ": " + label;
        }

        private void DrawTabWingCust()
        {
            ColorMenuLayout.BeginSection(MenuLocalization.T("tab_wingcust"));

            if (!WingFeatureFlags.Released)
            {
                ColorMenuTheme.Ensure();
                GUILayout.Space(6f);
                GUILayout.Label(MenuLocalization.T("wing_not_ready"), ColorMenuTheme.WarningLarge);
                ColorMenuLayout.HintLabel(MenuLocalization.T("wing_not_ready_hint"));
                WingSettings wingOff = _mod.Config.Wing ?? new WingSettings();
                if (wingOff.Enabled)
                {
                    wingOff.Enabled = false;
                    _mod.Config.Wing = wingOff;
                    _mod.ApplyWingOnly();
                }

                ColorMenuLayout.EndSection();
                return;
            }

            WingSettings wing = _mod.Config.Wing ?? new WingSettings();
            ColorMenuLayout.HintLabel(MenuLocalization.T("wing_hint"));

            bool enabled = GUILayout.Toggle(wing.Enabled, MenuLocalization.T("wing_enable"), ColorMenuTheme.Toggle);
            if (enabled != wing.Enabled)
            {
                wing.Enabled = enabled;
                _mod.Config.Wing = wing;
                _mod.ApplyWingOnly();
                _statusMessage = enabled ? MenuLocalization.T("wing_on") : MenuLocalization.T("wing_off");
            }

            if (!wing.Enabled)
            {
                ColorMenuLayout.HintLabel(MenuLocalization.T("wing_pick"));
                ColorMenuLayout.EndSection();
                return;
            }

            GUILayout.Space(6f);
            GUILayout.Label("Wing Size", ColorMenuTheme.LabelMuted);

            // Wider range so user can make them very small (0.15) or fairly large (1.10).
            float newScale = GUILayout.HorizontalSlider(
                wing.Scale,
                0.15f,
                1.10f,
                ColorMenuTheme.HorizontalSlider,
                ColorMenuTheme.HorizontalSliderThumb);
            GUILayout.Label(MenuLocalization.T("wing_scale") + newScale.ToString("0.00"), ColorMenuTheme.Label);
            if (Mathf.Abs(newScale - wing.Scale) > 0.005f)
            {
                wing.Scale = newScale;
                _mod.Config.Wing = wing;
                _mod.ApplyWingOnly();
            }

            // Wing color picker (uses Config.Colors.Wings — already wired through to the renderer).
            GUILayout.Space(8f);
            GUILayout.Label("Wing Color", ColorMenuTheme.Label);
            BodyColors colors = _mod.Config.Colors;
            if (colors != null)
            {
                Color wColor = colors.Wings;
                _wingPicker.SyncHex(wColor);
                wColor = _wingPicker.Draw(
                    wColor,
                    GuiFocusHelper.HexWingControl,
                    c =>
                    {
                        colors.Wings = c;
                        _mod.ApplyWingOnly();
                        _statusMessage = "Wing color updated";
                    },
                    DrawChannel);
                colors.Wings = wColor;
            }

            GUILayout.Space(4f);
            if (GUILayout.Button(MenuLocalization.T("apply_now"), ColorMenuTheme.ButtonAccent))
            {
                _mod.ApplyWingOnly();
                _statusMessage = MenuLocalization.T("wing_on");
            }

            ColorMenuLayout.EndSection();
        }

        private void DrawTabShoes()
        {
            ColorMenuLayout.BeginSection(MenuLocalization.T("tab_shoes"));

            if (!ShoeFeatureFlags.Released)
            {
                GUILayout.Space(8f);
                GUILayout.Label(MenuLocalization.T("shoe_coming_soon"), ColorMenuTheme.SectionHeader);
                ColorMenuLayout.HintLabel(MenuLocalization.T("shoe_coming_soon_hint"));
                ShoeSettings shoeOff = _mod.Config.Shoe ?? new ShoeSettings();
                if (shoeOff.Enabled)
                {
                    shoeOff.Enabled = false;
                    _mod.Config.Shoe = shoeOff;
                    _mod.ApplyShoeOnly();
                }

                ColorMenuLayout.EndSection();
                return;
            }

            ShoeSettings shoe = _mod.Config.Shoe ?? new ShoeSettings();
            ColorMenuLayout.HintLabel(MenuLocalization.T("shoe_hint"));

            bool enabled = GUILayout.Toggle(shoe.Enabled, MenuLocalization.T("shoe_enable"), ColorMenuTheme.Toggle);
            if (enabled != shoe.Enabled)
            {
                shoe.Enabled = enabled;
                if (enabled && (string.IsNullOrEmpty(shoe.ShoeId) || shoe.ShoeId == "none"))
                {
                    shoe.ShoeId = "sneakers";
                }

                _mod.Config.Shoe = shoe;
                _mod.ApplyShoeOnly();
                _statusMessage = enabled ? MenuLocalization.T("shoe_on") : MenuLocalization.T("shoe_off");
            }

            if (!shoe.Enabled)
            {
                ColorMenuLayout.HintLabel(MenuLocalization.T("shoe_pick"));
                ColorMenuLayout.EndSection();
                return;
            }

            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("shoe_model"), ColorMenuTheme.Label);
            DrawShoePickerGrid(shoe);

            ColorMenuLayout.EndSection();
        }

        private void DrawTabTops()
        {
            ColorMenuLayout.BeginSection(MenuLocalization.T("tab_tops"));

            TopsSettings tops = _mod.Config.Tops ?? new TopsSettings();
            ColorMenuLayout.HintLabel(MenuLocalization.T("tops_hint"));

            bool enabled = GUILayout.Toggle(tops.Enabled, MenuLocalization.T("tops_enable"), ColorMenuTheme.Toggle);
            if (enabled != tops.Enabled)
            {
                tops.Enabled = enabled;
                if (enabled && (string.IsNullOrEmpty(tops.TopsId) || tops.TopsId == "none"))
                {
                    tops.TopsId = "tshirt";
                }

                _mod.Config.Tops = tops;
                _mod.ApplyTopsOnly();
                _statusMessage = enabled ? MenuLocalization.T("tops_on") : MenuLocalization.T("tops_off");
            }

            if (!tops.Enabled)
            {
                ColorMenuLayout.HintLabel(MenuLocalization.T("tops_pick"));
                ColorMenuLayout.EndSection();
                return;
            }

            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("tops_model"), ColorMenuTheme.Label);
            DrawTopsPickerGrid(tops);

            ColorMenuLayout.EndSection();
        }

        private void DrawTabObjects()
        {
            ColorMenuLayout.BeginSection(MenuLocalization.T("tab_objects"));

            if (!ObjectFeatureFlags.Released)
            {
                GUILayout.Label(MenuLocalization.T("shoe_coming_soon"), ColorMenuTheme.SectionHeader);
                ColorMenuLayout.EndSection();
                return;
            }

            ObjectSettings obj = _mod.Config.Object ?? new ObjectSettings();
            ColorMenuLayout.HintLabel(MenuLocalization.T("objects_hint"));

            bool enabled = GUILayout.Toggle(obj.Enabled, MenuLocalization.T("objects_enable"), ColorMenuTheme.Toggle);
            if (enabled != obj.Enabled)
            {
                obj.Enabled = enabled;
                if (enabled && (string.IsNullOrEmpty(obj.ObjectId) || obj.ObjectId == "none"))
                {
                    obj.ObjectId = "rings_battle_6";
                }

                _mod.Config.Object = obj;
                _mod.ApplyObjectsOnly();
                _statusMessage = enabled ? MenuLocalization.T("objects_on") : MenuLocalization.T("objects_off");
            }

            if (!obj.Enabled)
            {
                ColorMenuLayout.HintLabel(MenuLocalization.T("objects_pick"));
                ColorMenuLayout.EndSection();
                return;
            }

            float newScale = GUILayout.HorizontalSlider(
                obj.Scale,
                0.5f,
                1.5f,
                ColorMenuTheme.HorizontalSlider,
                ColorMenuTheme.HorizontalSliderThumb);
            GUILayout.Label(MenuLocalization.T("objects_scale") + " " + newScale.ToString("0.00"), ColorMenuTheme.Label);
            if (Mathf.Abs(newScale - obj.Scale) > 0.005f)
            {
                obj.Scale = newScale;
                _mod.Config.Object = obj;
                _mod.ApplyObjectsOnly();
            }

            GUILayout.Space(6f);
            DrawObjectsPickerGrid(obj);

            ColorMenuLayout.EndSection();
        }

        private void DrawObjectsPickerGrid(ObjectSettings obj)
        {
            const int columns = 3;
            const float cellSize = 96f;
            const float previewSize = 56f;

            int col = 0;
            GUILayout.BeginHorizontal();
            for (int i = 0; i < ObjectsCatalog.Entries.Length; i++)
            {
                ObjectsCatalogEntry entry = ObjectsCatalog.Entries[i];
                if (entry.Id == "none")
                {
                    continue;
                }

                bool selected = obj.ObjectId == entry.Id;
                GUIStyle style = selected ? ColorMenuTheme.TabActive : ColorMenuTheme.Button;
                GUILayout.BeginVertical(GUILayout.Width(cellSize));
                Rect cellRect = GUILayoutUtility.GetRect(cellSize, cellSize, style);
                if (GUI.Button(cellRect, GUIContent.none, style))
                {
                    obj.ObjectId = entry.Id;
                    _mod.Config.Object = obj;
                    _mod.ApplyObjectsOnly();
                    _statusMessage = MenuLocalization.T("tab_objects") + ": " + entry.Label;
                }

                Sprite preview = ObjectSpriteFactory.GetPreviewSprite(entry.Id);
                if (preview != null && preview.texture != null)
                {
                    Texture2D tex = preview.texture;
                    Rect imgRect = new Rect(
                        cellRect.x + (cellRect.width - previewSize) * 0.5f,
                        cellRect.y + 10f,
                        previewSize,
                        previewSize);
                    GUI.DrawTexture(imgRect, tex, ScaleMode.ScaleToFit, true);
                }

                GUI.Label(
                    new Rect(cellRect.x, cellRect.y + cellRect.height - 18f, cellRect.width, 16f),
                    entry.Label,
                    ColorMenuTheme.LabelMuted);
                GUILayout.EndVertical();

                col++;
                if (col >= columns)
                {
                    col = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawTopsPickerGrid(TopsSettings tops)
        {
            const int columns = 4;
            const float cellSize = 88f;
            const float previewSize = 60f;

            int col = 0;
            GUILayout.BeginHorizontal();
            for (int i = 0; i < TopsCatalog.Entries.Length; i++)
            {
                TopsCatalogEntry entry = TopsCatalog.Entries[i];
                if (entry.Id == "none")
                {
                    continue;
                }

                bool selected = tops.TopsId == entry.Id;
                GUIStyle style = selected ? ColorMenuTheme.TabActive : ColorMenuTheme.Button;
                GUILayout.BeginVertical(GUILayout.Width(cellSize));
                Rect cellRect = GUILayoutUtility.GetRect(cellSize, cellSize, style);
                if (GUI.Button(cellRect, GUIContent.none, style))
                {
                    tops.TopsId = entry.Id;
                    _mod.Config.Tops = tops;
                    _mod.ApplyTopsOnly();
                    _statusMessage = MenuLocalization.T("tab_tops") + ": " + entry.Label;
                }

                Sprite preview = TopsSpriteFactory.GetSprite(entry.Id);
                if (preview != null && preview.texture != null)
                {
                    Texture2D tex = preview.texture;
                    float aspect = (float)tex.width / Mathf.Max(1, tex.height);
                    float w = previewSize;
                    float h = previewSize;
                    if (aspect > 1f) { h = previewSize / aspect; }
                    else { w = previewSize * aspect; }
                    Rect imgRect = new Rect(
                        cellRect.x + (cellRect.width - w) * 0.5f,
                        cellRect.y + 8f,
                        w,
                        h);
                    GUI.DrawTexture(imgRect, tex, ScaleMode.ScaleToFit, true);
                }

                GUI.Label(
                    new Rect(cellRect.x, cellRect.y + cellRect.height - 18f, cellRect.width, 16f),
                    entry.Label,
                    ColorMenuTheme.LabelMuted);
                GUILayout.EndVertical();

                col++;
                if (col >= columns)
                {
                    col = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawShoePickerGrid(ShoeSettings shoe)
        {
            const int columns = 4;
            const float cellSize = 88f;
            const float previewSize = 60f;

            int col = 0;
            GUILayout.BeginHorizontal();
            for (int i = 0; i < ShoeCatalog.Entries.Length; i++)
            {
                ShoeCatalogEntry entry = ShoeCatalog.Entries[i];
                if (entry.Id == "none")
                {
                    continue;
                }

                bool selected = shoe.ShoeId == entry.Id;
                GUIStyle style = selected ? ColorMenuTheme.TabActive : ColorMenuTheme.Button;
                GUILayout.BeginVertical(GUILayout.Width(cellSize));
                Rect cellRect = GUILayoutUtility.GetRect(cellSize, cellSize, style);
                if (GUI.Button(cellRect, GUIContent.none, style))
                {
                    shoe.ShoeId = entry.Id;
                    _mod.Config.Shoe = shoe;
                    _mod.ApplyShoeOnly();
                    _statusMessage = MenuLocalization.T("tab_shoes") + ": " + entry.Label;
                }

                Sprite preview = ShoeSpriteFactory.GetSprite(entry.Id);
                if (preview != null && preview.texture != null)
                {
                    Texture2D tex = preview.texture;
                    float aspect = (float)tex.width / Mathf.Max(1, tex.height);
                    float w = previewSize;
                    float h = previewSize;
                    if (aspect > 1f) { h = previewSize / aspect; }
                    else { w = previewSize * aspect; }
                    Rect imgRect = new Rect(
                        cellRect.x + (cellRect.width - w) * 0.5f,
                        cellRect.y + 8f,
                        w,
                        h);
                    GUI.DrawTexture(imgRect, tex, ScaleMode.ScaleToFit, true);
                }

                GUI.Label(
                    new Rect(cellRect.x, cellRect.y + cellRect.height - 18f, cellRect.width, 16f),
                    entry.Label,
                    ColorMenuTheme.LabelMuted);
                GUILayout.EndVertical();

                col++;
                if (col >= columns)
                {
                    col = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawTabWeapons()
        {
            WeaponColorSettings weapon = _mod.Config.Weapon ?? new WeaponColorSettings();
            _hexWeaponField.SyncFromColorIfNotFocused(GuiFocusHelper.HexWeaponControl, ref _hexWeapon, weapon.Color);

            ColorMenuLayout.BeginSection(MenuLocalization.T("tab_weapons"));
            ColorMenuLayout.HintLabel(MenuLocalization.T("weapon_hint"));

            bool enabled = GUILayout.Toggle(weapon.Enabled, MenuLocalization.T("weapon_enable"), ColorMenuTheme.Toggle);
            if (enabled != weapon.Enabled)
            {
                weapon.Enabled = enabled;
                _mod.Config.Weapon = weapon;
                WeaponColorApplier.MarkDirty();
                _mod.ApplyWeaponOnly();
                _statusMessage = enabled ? MenuLocalization.T("weapon_on") : MenuLocalization.T("weapon_off");
            }

            if (!weapon.Enabled)
            {
                ColorMenuLayout.HintLabel(MenuLocalization.T("weapon_pick"));
                ColorMenuLayout.EndSection();
                return;
            }

            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("weapon_color"), ColorMenuTheme.Label);
            Color weaponColor = weapon.Color;
            _weaponPicker.SyncHex(weaponColor);
            weaponColor = _weaponPicker.Draw(
                weaponColor,
                GuiFocusHelper.HexWeaponControl,
                c =>
                {
                    weapon.Color = c;
                    _mod.Config.Weapon = weapon;
                    _mod.ApplyWeaponOnly();
                    _statusMessage = MenuLocalization.T("weapon_updated");
                },
                DrawChannel);
            weapon.Color = weaponColor;
            _mod.Config.Weapon = weapon;

            bool neon = GUILayout.Toggle(weapon.NeonEnabled, MenuLocalization.T("weapon_neon"), ColorMenuTheme.Toggle);
            if (neon != weapon.NeonEnabled)
            {
                weapon.NeonEnabled = neon;
                _mod.Config.Weapon = weapon;
                WeaponColorApplier.MarkDirty();
                _mod.ApplyWeaponOnly();
            }

            DrawWeaponPresetsRow(weapon);

            GUILayout.Space(6f);
            bool tintMesh = GUILayout.Toggle(weapon.TintMesh, MenuLocalization.T("weapon_tint_mesh"), ColorMenuTheme.Toggle);
            bool tintParticles = GUILayout.Toggle(weapon.TintParticles, MenuLocalization.T("weapon_tint_particles"), ColorMenuTheme.Toggle);
            if (tintMesh != weapon.TintMesh || tintParticles != weapon.TintParticles)
            {
                weapon.TintMesh = tintMesh;
                weapon.TintParticles = tintParticles;
                _mod.Config.Weapon = weapon;
                WeaponColorApplier.MarkDirty();
                _mod.ApplyWeaponOnly();
            }

            GUILayout.Space(8f);
            if (GUILayout.Button(MenuLocalization.T("apply_now"), ColorMenuTheme.ButtonAccent))
            {
                WeaponColorApplier.MarkDirty();
                _mod.ApplyWeaponOnly();
                _statusMessage = MenuLocalization.T("weapon_repainted");
            }

            ColorMenuLayout.EndSection();
        }

        private void DrawBodyHalfSection()
        {
            GUILayout.BeginVertical(ColorMenuTheme.Box);
            bool half = GUILayout.Toggle(_mod.Config.Colors.HalfColorEnabled, MenuLocalization.T("body_gradients"), ColorMenuTheme.Toggle);
            if (half != _mod.Config.Colors.HalfColorEnabled)
            {
                _mod.Config.Colors.HalfColorEnabled = half;
                ApplyBodyAndSync();
            }

            if (!_mod.Config.Colors.HalfColorEnabled)
            {
                GUILayout.EndVertical();
                return;
            }

            string[] labels = new string[HalfEditParts.Length];
            for (int i = 0; i < HalfEditParts.Length; i++)
            {
                labels[i] = MenuLocalization.T(HalfEditPartKeys[i]);
            }

            _halfEditPart = GUILayout.SelectionGrid(_halfEditPart, labels, 3, ColorMenuTheme.Button);
            if (_halfEditPart < 0 || _halfEditPart >= HalfEditParts.Length)
            {
                _halfEditPart = 0;
            }

            StickColorPart part = HalfEditParts[_halfEditPart];
            GUILayout.Label(MenuLocalization.T("half_color") + " — " + labels[_halfEditPart], ColorMenuTheme.LabelMuted);
            Color distal = _mod.Config.Colors.GetDistal(part);
            _halvesPicker.SyncHex(distal);
            Color nextDistal = _halvesPicker.Draw(
                distal,
                "SFCC_HEX_HALF_" + (int)part,
                c =>
                {
                    _mod.Config.Colors.SetDistal(part, c);
                    LineMaintainThrottle.MarkDirty();
                    ApplyBodyAndSync();
                },
                DrawChannel);
            if (nextDistal != distal)
            {
                _mod.Config.Colors.SetDistal(part, nextDistal);
                LineMaintainThrottle.MarkDirty();
            }

            GUILayout.EndVertical();
        }

        private void DrawTabGlow()
        {
            GlowSettings glow = _mod.Config.Glow;

            GUILayout.BeginVertical(ColorMenuTheme.Box);
            GUILayout.Label(MenuLocalization.T("glow_color_label"), ColorMenuTheme.Label);
            Color glowColor = glow.Color;
            _glowPicker.SyncHex(glowColor);
            glowColor = _glowPicker.Draw(
                glowColor,
                GuiFocusHelper.HexGlowControl,
                c =>
                {
                    glow.Color = c;
                    if (glow.Enabled)
                    {
                        _mod.ApplyGlowOnly();
                    }

                    _statusMessage = MenuLocalization.T("glow_updated");
                },
                DrawChannel);
            glow.Color = glowColor;

            GUILayout.Space(4f);
            float blend = GUILayout.HorizontalSlider(glow.BodyColorBlend, 0f, 1f, ColorMenuTheme.HorizontalSlider, ColorMenuTheme.HorizontalSliderThumb);
            if (!Mathf.Approximately(blend, glow.BodyColorBlend))
            {
                glow.BodyColorBlend = blend;
                ApplyGlowIfEnabled();
            }
            GUILayout.Label(MenuLocalization.T("glow_body_blend") + ": " + glow.BodyColorBlend.ToString("0.00"), ColorMenuTheme.LabelMuted);

            GUILayout.Space(6f);
            bool enabled = GUILayout.Toggle(glow.Enabled, MenuLocalization.T("glow_enable"), ColorMenuTheme.Toggle);
            if (enabled != glow.Enabled)
            {
                glow.Enabled = enabled;
                if (glow.Enabled)
                {
                    _mod.ApplyGlowOnly();
                    _statusMessage = MenuLocalization.T("glow_on");
                }
                else
                {
                    _mod.ApplyGlowOnly();
                    _statusMessage = MenuLocalization.T("glow_off");
                }
            }

            float strength = GUILayout.HorizontalSlider(glow.Strength, 0f, 0.85f, ColorMenuTheme.HorizontalSlider, ColorMenuTheme.HorizontalSliderThumb);
            if (!Mathf.Approximately(strength, glow.Strength))
            {
                glow.Strength = strength;
                ApplyGlowIfEnabled();
            }
            GUILayout.Label(MenuLocalization.T("glow_strength") + glow.Strength.ToString("0.0"), ColorMenuTheme.LabelMuted);

            // ── Style picker: clásicos + épicos ──
            GUILayout.Space(6f);
            GUILayout.BeginVertical(ColorMenuTheme.Box);
            GUILayout.Label("✦  Glow Style  ✦", ColorMenuTheme.SectionHeader);
            GUILayout.Label("Cambia la forma del aura sin tocar el resto.", ColorMenuTheme.LabelMuted);
            GUILayout.Space(4f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Activo:", ColorMenuTheme.LabelMuted, GUILayout.Width(48f));
            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = glow.Color;
            GUILayout.Box(" ", GUILayout.Width(14f), GUILayout.Height(14f));
            GUI.backgroundColor = prevBg;
            GUILayout.Label(GlowStyleModulator.GetLabel(glow.Style), ColorMenuTheme.SectionHeader);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);
            GUILayout.Label("Clásicos", ColorMenuTheme.LabelMuted);
            DrawGlowStyleRow(glow, GlowStyleModulator.ClassicStyles);

            GUILayout.Space(4f);
            GUILayout.Label("Épicos", ColorMenuTheme.LabelMuted);
            DrawGlowStyleRow(glow, GlowStyleModulator.EpicStyles);

            GUILayout.Space(2f);
            GUILayout.Label(GlowStyleModulator.GetTooltip(glow.Style), ColorMenuTheme.LabelMuted);
            GUILayout.EndVertical();

            GUILayout.Space(4f);
            GUILayout.Label(MenuLocalization.T("glow_zones"), ColorMenuTheme.Label);
            bool zoneChanged = false;
            bool onHead = GUILayout.Toggle(glow.OnHead, MenuLocalization.T("zone_head"), ColorMenuTheme.Toggle);
            bool onTorso = GUILayout.Toggle(glow.OnTorso, MenuLocalization.T("zone_torso"), ColorMenuTheme.Toggle);
            bool onArms = GUILayout.Toggle(glow.OnArms, MenuLocalization.T("zone_arms"), ColorMenuTheme.Toggle);
            bool onLegs = GUILayout.Toggle(glow.OnLegs, MenuLocalization.T("zone_legs"), ColorMenuTheme.Toggle);
            bool onWings = GUILayout.Toggle(glow.OnWings, MenuLocalization.T("zone_wings"), ColorMenuTheme.Toggle);
            if (onHead != glow.OnHead || onTorso != glow.OnTorso || onArms != glow.OnArms
                || onLegs != glow.OnLegs || onWings != glow.OnWings)
            {
                glow.OnHead = onHead;
                glow.OnTorso = onTorso;
                glow.OnArms = onArms;
                glow.OnLegs = onLegs;
                glow.OnWings = onWings;
                zoneChanged = true;
            }
            if (zoneChanged)
            {
                ApplyGlowIfEnabled();
            }

            float width = GUILayout.HorizontalSlider(glow.AuraWidth, 1.1f, 1.85f, ColorMenuTheme.HorizontalSlider, ColorMenuTheme.HorizontalSliderThumb);
            if (!Mathf.Approximately(width, glow.AuraWidth))
            {
                glow.AuraWidth = width;
                ApplyGlowIfEnabled();
            }
            GUILayout.Label(MenuLocalization.T("glow_width") + glow.AuraWidth.ToString("0.0"), ColorMenuTheme.LabelMuted);

            float alpha = GUILayout.HorizontalSlider(glow.AuraAlpha, 0.08f, 0.38f, ColorMenuTheme.HorizontalSlider, ColorMenuTheme.HorizontalSliderThumb);
            if (!Mathf.Approximately(alpha, glow.AuraAlpha))
            {
                glow.AuraAlpha = alpha;
                ApplyGlowIfEnabled();
            }
            GUILayout.Label(MenuLocalization.T("glow_alpha") + glow.AuraAlpha.ToString("0.00"), ColorMenuTheme.LabelMuted);

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(MenuLocalization.T("glow_apply"), ColorMenuTheme.ButtonAccent))
            {
                glow.Enabled = true;
                _mod.ApplyGlowOnly();
                _statusMessage = MenuLocalization.T("glow_applied");
            }
            if (GUILayout.Button(MenuLocalization.T("glow_remove"), ColorMenuTheme.Button))
            {
                glow.Enabled = false;
                _mod.ApplyGlowOnly();
                _statusMessage = MenuLocalization.T("glow_removed");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawGlowStyleRow(GlowSettings glow, GlowStyleKind[] styles)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < styles.Length; i++)
            {
                DrawGlowStyleButton(glow, styles[i], GlowStyleModulator.GetLabel(styles[i]));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawGlowStyleButton(GlowSettings glow, GlowStyleKind kind, string label)
        {
            bool isActive = glow.Style == kind;
            GUIStyle style = isActive ? ColorMenuTheme.ButtonAccent : ColorMenuTheme.Button;
            string tip = GlowStyleModulator.GetTooltip(kind);
            if (GUILayout.Button(new GUIContent(label, tip), style, GUILayout.MinWidth(54f), GUILayout.Height(22f)))
            {
                if (glow.Style != kind)
                {
                    glow.Style = kind;
                    ApplyGlowIfEnabled();
                    _statusMessage = "Glow style: " + label;
                }
            }
        }

        private void DrawTabSlots()
        {
            GUILayout.BeginVertical(ColorMenuTheme.Box);
            GUILayout.Label(MenuLocalization.T("slots_hint"), ColorMenuTheme.LabelMuted);
            GUILayout.Space(4f);
            DrawStylePresetsRow();
            GUILayout.Space(6f);

            if (GUILayout.Button(MenuLocalization.T("slot_add"), ColorMenuTheme.ButtonAccent))
            {
                if (StyleSlotStore.AddSlot())
                {
                    _statusMessage = MenuLocalization.T("slot_add");
                }
            }

            StyleSlotManifest manifest = StyleSlotStore.GetManifest();
            _slotsScroll = GUILayout.BeginScrollView(_slotsScroll, GUILayout.MinHeight(220f));
            for (int i = 0; i < manifest.Slots.Count; i++)
            {
                StyleSlotEntry entry = manifest.Slots[i];
                int slotIndex = i + 1;
                entry.Index = slotIndex;
                bool has = StyleSlotStore.HasSlot(slotIndex);
                entry.HasData = has;

                GUILayout.Space(6f);
                GUILayout.BeginVertical(ColorMenuTheme.Box);

                if (!_slotNameDraft.ContainsKey(slotIndex))
                {
                    _slotNameDraft[slotIndex] = StyleSlotStore.GetSlotName(slotIndex);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(MenuLocalization.T("slot_rename"), ColorMenuTheme.Label, GUILayout.Width(52f));
                string nameControl = "SFCC_SLOT_NAME_" + slotIndex;
                GUI.SetNextControlName(nameControl);
                string draft = GUILayout.TextField(_slotNameDraft[slotIndex], ColorMenuTheme.TextField);
                if (draft != _slotNameDraft[slotIndex])
                {
                    _slotNameDraft[slotIndex] = draft;
                }

                if (GUI.GetNameOfFocusedControl() != nameControl
                    && draft != StyleSlotStore.GetSlotName(slotIndex))
                {
                    StyleSlotStore.SetSlotName(slotIndex, draft);
                }

                GUILayout.Label(has ? MenuLocalization.T("saved") : MenuLocalization.T("empty"), ColorMenuTheme.LabelMuted, GUILayout.Width(56f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(MenuLocalization.T("save"), ColorMenuTheme.Button))
                {
                    string name = _slotNameDraft.ContainsKey(slotIndex) ? _slotNameDraft[slotIndex] : StyleSlotStore.GetSlotName(slotIndex);
                    StyleSlotStore.SetSlotName(slotIndex, name);
                    if (StyleSlotStore.SaveSlot(slotIndex, _mod.Config, name))
                    {
                        entry.HasData = true;
                        _statusMessage = name + " " + MenuLocalization.T("saved");
                    }
                    else
                    {
                        _statusMessage = MenuLocalization.T("slot_save_failed");
                    }
                }
                if (GUILayout.Button(MenuLocalization.T("load"), ColorMenuTheme.ButtonAccent))
                {
                    ColorConfig loaded;
                    if (StyleSlotStore.LoadSlot(slotIndex, out loaded) && loaded != null)
                    {
                        _mod.MergeConfigFrom(loaded);
                        _mod.Config.BodyCustomizationActive = true;
                        RefreshUiFromConfig();
                        _mod.ApplyFullSkinFromConfig();
                        _statusMessage = StyleSlotStore.GetSlotName(slotIndex) + MenuLocalization.T("slot_loaded");
                    }
                    else
                    {
                        _statusMessage = MenuLocalization.T("empty");
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawTabSettings()
        {
            GUILayout.BeginVertical(ColorMenuTheme.Box);
            GUILayout.Label(MenuLocalization.T("language"), ColorMenuTheme.Label);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(MenuLocalization.T("english"), _mod.Config.MenuLanguage == MenuLanguage.English ? ColorMenuTheme.TabActive : ColorMenuTheme.Button))
            {
                MenuLocalization.SetLanguage(MenuLanguage.English);
            }
            if (GUILayout.Button(MenuLocalization.T("spanish"), _mod.Config.MenuLanguage == MenuLanguage.Spanish ? ColorMenuTheme.TabActive : ColorMenuTheme.Button))
            {
                MenuLocalization.SetLanguage(MenuLanguage.Spanish);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.Label(MenuLocalization.T("menu_size"), ColorMenuTheme.Label);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(MenuLocalization.T("menu_smaller"), ColorMenuTheme.Button))
            {
                NudgeMenuSize(-40f, -50f);
            }
            if (GUILayout.Button(MenuLocalization.T("menu_larger"), ColorMenuTheme.Button))
            {
                NudgeMenuSize(40f, 50f);
            }
            if (GUILayout.Button(MenuLocalization.T("menu_reset_size"), ColorMenuTheme.Button))
            {
                _mod.Config.MenuWindowWidth = 540f;
                _mod.Config.MenuWindowHeight = 680f;
                ApplyWindowSizeFromConfig();
            }
            GUILayout.EndHorizontal();
            GUILayout.Label(
                MenuLocalization.T("menu_size_hint")
                    + " (" + (int)_windowRect.width + "×" + (int)_windowRect.height + ")",
                ColorMenuTheme.LabelMuted);

            GUILayout.Space(10f);
            GUILayout.Label("Activo: " + ModFeatureGate.DescribeActive(_mod.Config), ColorMenuTheme.LabelMuted);
            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("options"), ColorMenuTheme.Label);
            bool safeMp = GUILayout.Toggle(
                _mod.Config.SafeVanillaMultiplayer,
                MenuLocalization.T("mp_safe"),
                ColorMenuTheme.Toggle);
            if (safeMp != _mod.Config.SafeVanillaMultiplayer)
            {
                _mod.Config.SafeVanillaMultiplayer = safeMp;
                _statusMessage = safeMp
                    ? MenuLocalization.T("mp_safe_on")
                    : MenuLocalization.T("mp_safe_off");
            }

            GUILayout.Space(4f);
            ColorMenuLayout.HintLabel(MenuLocalization.T("mod_sync_hint"));

            GUILayout.Space(6f);
            GUILayout.Label("SFCC mods: " + Network.NetworkSyncDiagnostics.DetectedModCount
                + " | pending: " + Network.NetworkSyncDiagnostics.PendingModCount, ColorMenuTheme.LabelMuted);
            GUILayout.Label("Last: " + Network.NetworkSyncDiagnostics.LastEvent, ColorMenuTheme.LabelMuted);
            if (!ModFeatureGate.IsBodyActive(_mod.Config))
            {
                GUILayout.Label("Body sync OFF — enable body colors to publish sfcc.", ColorMenuTheme.LabelMuted);
            }

            _mod.Config.UseUniformSkin = GUILayout.Toggle(_mod.Config.UseUniformSkin, MenuLocalization.T("uniform_skin"), ColorMenuTheme.Toggle);
            _mod.Config.AnimatedRgb = GUILayout.Toggle(_mod.Config.AnimatedRgb, MenuLocalization.T("rgb_anim"), ColorMenuTheme.Toggle);
            if (_mod.Config.AnimatedRgb)
            {
                _mod.Config.RgbSpeed = GUILayout.HorizontalSlider(_mod.Config.RgbSpeed, 0.2f, 4f, ColorMenuTheme.HorizontalSlider, ColorMenuTheme.HorizontalSliderThumb);
            }

            if (_mod.Config.Glow != null)
            {
                _mod.Config.Glow.MaintainInLobby = GUILayout.Toggle(
                    _mod.Config.Glow.MaintainInLobby,
                    MenuLocalization.T("glow_lobby"),
                    ColorMenuTheme.Toggle);
            }

            GUILayout.Space(8f);
            if (GUILayout.Button(MenuLocalization.T("save_config"), ColorMenuTheme.ButtonAccent))
            {
                ColorConfigStore.Save(_mod.Config);
                ApplyBodyAndSync();
                _statusMessage = MenuLocalization.T("config_saved");
            }
            if (GUILayout.Button(MenuLocalization.T("sync_lobby"), ColorMenuTheme.Button))
            {
                ApplyBodyAndSync();
                _mod.ApplyHatOnly();
                _statusMessage = MenuLocalization.T("sync_sent");
            }

            GUILayout.Space(12f);
            GUILayout.Label(MenuLocalization.T("made_by_alka"), ColorMenuTheme.LabelMuted);
            GUILayout.EndVertical();
        }

        private void DrawTabAdvanced()
        {
            GUILayout.BeginVertical(ColorMenuTheme.Box);
            DrawSkinCodeSection();

            GUILayout.Space(10f);
            GUILayout.Label(MenuLocalization.T("active") + ": " + ModFeatureGate.DescribeActive(_mod.Config), ColorMenuTheme.LabelMuted);
            GUILayout.Space(8f);
            if (GUILayout.Button(MenuLocalization.T("log_renderers"), ColorMenuTheme.Button))
            {
                Controller local = LocalPlayerResolver.GetLocalController();
                if (local != null)
                {
                    PlayerColorApplier.LogRendererMap(local);
                }
            }

            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("mp_sync_hint"), ColorMenuTheme.LabelMuted);
            GUILayout.Label(MenuLocalization.T("chat_hint"), ColorMenuTheme.LabelMuted);
            GUILayout.EndVertical();
        }

        private void DrawSkinCodeSection()
        {
            GUILayout.Label(MenuLocalization.T("skin_code"), ColorMenuTheme.Label);
            if (GUILayout.Button(MenuLocalization.T("gen_code"), ColorMenuTheme.ButtonAccent))
            {
                _exportCode = SkinShareCodec.Encode(_mod.Config);
                _statusMessage = string.IsNullOrEmpty(_exportCode)
                    ? MenuLocalization.T("code_fail")
                    : MenuLocalization.T("code_ok");
            }

            GUI.SetNextControlName(GuiFocusHelper.ExportCodeControl);
            _exportCode = GUILayout.TextArea(
                _exportCode ?? "",
                ColorMenuTheme.TextField,
                GUILayout.MinHeight(52f));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(MenuLocalization.T("copy_code"), ColorMenuTheme.Button))
            {
                CopyToClipboard(_exportCode);
                _statusMessage = MenuLocalization.T("code_copied");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("paste_import"), ColorMenuTheme.LabelMuted);
            GUI.SetNextControlName(GuiFocusHelper.ImportCodeControl);
            _importCode = GUILayout.TextField(_importCode ?? "", ColorMenuTheme.TextField);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(MenuLocalization.T("import"), ColorMenuTheme.ButtonAccent))
            {
                ImportSkinCode();
            }
            if (GUILayout.Button(MenuLocalization.T("paste"), ColorMenuTheme.Button))
            {
                _importCode = PasteFromClipboard();
                _statusMessage = MenuLocalization.T("code_pasted");
            }
            GUILayout.EndHorizontal();
        }

        private void ImportSkinCode()
        {
            ColorConfig imported;
            if (!SkinShareCodec.TryDecode(_importCode, out imported))
            {
                _statusMessage = MenuLocalization.T("code_invalid");
                return;
            }

            _mod.MergeConfigFrom(imported);
            _mod.Config.BodyCustomizationActive = true;
            _mod.Config.Colors.HalfColorEnabled = imported.Colors.HalfColorEnabled
                || SkinShareCodec.HasDistinctHalfColors(imported.Colors);

            RefreshUiFromConfig();
            ColorConfigStore.Save(_mod.Config);
            _mod.ApplyFullSkinFromConfig();
            _mod.BroadcastColors();
            _statusMessage = MenuLocalization.T("skin_imported");
        }

        private void RefreshUiFromConfig()
        {
            StickColorPart part = (StickColorPart)_selectedPart;
            _hexBody = ColorUtil.ToHex(_mod.Config.Colors.Get(part));
            _hexBodyField.ForceSyncHex(_hexBody);
            _hexGlow = ColorUtil.ToHex(_mod.Config.Glow.Color);
            _hexGlowField.ForceSyncHex(_hexGlow);
            if (_mod.Config.Weapon != null)
            {
                _hexWeapon = ColorUtil.ToHex(_mod.Config.Weapon.Color);
                _hexWeaponField.ForceSyncHex(_hexWeapon);
            }
        }

        internal static int DrawChannel(string label, int value, Color accent)
        {
            GUILayout.BeginHorizontal();
            Color prev = GUI.color;
            GUI.color = accent;
            GUILayout.Label(label, ColorMenuTheme.Label, GUILayout.Width(18f));
            GUI.color = prev;
            int v = Mathf.RoundToInt(GUILayout.HorizontalSlider(value, 0f, 255f, ColorMenuTheme.HorizontalSlider, ColorMenuTheme.HorizontalSliderThumb));
            GUILayout.Label(v.ToString(), ColorMenuTheme.LabelMuted, GUILayout.Width(32f));
            GUILayout.EndHorizontal();
            return v;
        }

        private void DrawStylePresetsRow(bool applyWeapons = false)
        {
            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("style_presets"), ColorMenuTheme.Label);
            int col = 0;
            GUILayout.BeginHorizontal();
            foreach (string preset in StylePresets.All)
            {
                if (GUILayout.Button(preset, ColorMenuTheme.Button, GUILayout.Width(68f)))
                {
                    StylePresets.ApplyFullStyle(_mod.Config, preset);
                    RefreshUiFromConfig();
                    if (applyWeapons)
                    {
                        WeaponColorApplier.MarkDirty();
                        _mod.ApplyWeaponOnly();
                    }
                    _mod.ApplyFullSkinFromConfig();
                    _statusMessage = "Preset " + preset;
                }
                col++;
                if (col >= 3)
                {
                    col = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawWeaponPresetsRow(WeaponColorSettings weapon)
        {
            GUILayout.Space(6f);
            GUILayout.Label(MenuLocalization.T("weapon_presets"), ColorMenuTheme.Label);
            int col = 0;
            GUILayout.BeginHorizontal();
            foreach (string preset in WeaponPresets.All)
            {
                if (GUILayout.Button(preset, ColorMenuTheme.Button, GUILayout.Width(68f)))
                {
                    WeaponPresets.Apply(weapon, preset);
                    _mod.Config.Weapon = weapon;
                    _hexWeapon = ColorUtil.ToHex(weapon.Color);
                    _hexWeaponField.ForceSyncHex(_hexWeapon);
                    WeaponColorApplier.MarkDirty();
                    _mod.ApplyWeaponOnly();
                    _statusMessage = "Weapon: " + preset;
                }
                col++;
                if (col >= 3)
                {
                    col = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ApplyBodyAndSync()
        {
            if (!_mod.Config.BodyCustomizationActive)
            {
                return;
            }

            LineMaintainThrottle.MarkDirty();
            _mod.ApplyBodyOnly();
            if (ModFeatureGate.IsWingActive(_mod.Config))
            {
                _mod.ApplyWingOnly();
            }

            _mod.BroadcastColors();
        }

        private void ApplyGlowIfEnabled()
        {
            _mod.ApplyGlowOnly();
        }

        private static void CopyToClipboard(string text)
        {
            TextEditor te = new TextEditor();
            te.text = text ?? "";
            te.SelectAll();
            te.Copy();
        }

        private static string PasteFromClipboard()
        {
            TextEditor te = new TextEditor();
            te.Paste();
            return te.text ?? "";
        }
    }
}
