using HarmonyLib;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;
using StickFightColorCustomizer.Network;
using StickFightColorCustomizer.Patches;
using StickFightColorCustomizer.UI;
using UnityEngine;

namespace StickFightColorCustomizer.Hosting
{
    public sealed class ColorCustomizerApp
    {
        public static ColorCustomizerApp Instance { get; private set; }

        public ColorConfig Config { get; private set; }

        private ColorMenuGUI _gui;
        private HarmonyLib.Harmony _harmony;
        private bool _initialized;
        private float _rgbHue;
        private bool _wasInLobby;
        private bool _releaseGuiFocusOnNextGui;
        private int _focusReleaseFramesRemaining;
        private float _lastRgbLobbyLocalApplyTime;
        private float _lastWeaponColorTickTime;
        private float _lastShoeTickTime;
        private float _lastTopsTickTime;
        private float _lastWingTickTime;
        private float _lastObjectTickTime;
        private Controller _cachedLocalControllerThisFrame;
        private int _cachedLocalControllerFrame = -1;
        private const float WeaponColorTickInterval = 0.35f;
        private const float ShoeTickInterval = 0.1f;
        private const float ShoeTickIntervalLobbyBusy = 0.25f;
        private const float TopsTickInterval = 0.5f;
        private const float WingTickInterval = 2.0f;
        private const float ObjectTickInterval = 0.05f;
        private const float ObjectTickIntervalLobbyBusy = 0.12f;

        public void Initialize(string harmonyId)
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            Instance = this;
            Config = ColorConfigStore.Load();
            _gui = new ColorMenuGUI(this);

            _harmony = new HarmonyLib.Harmony(harmonyId);
            _harmony.PatchAll(typeof(Patch_ControllerHandler_CreatePlayer).Assembly);
            ModLog.Info(ModBranding.FullName + " v" + ModBranding.Version + " (" + ModBranding.ShortName + ")");
            ModLog.Info("by " + ModBranding.Author);
        }

        public void Shutdown()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
                _harmony = null;
            }
        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                SetMenuVisible(!_gui.Visible);
            }

            if (_gui.Visible && Input.GetKeyDown(KeyCode.Escape))
            {
                SetMenuVisible(false);
            }

            bool colorsChanged = false;
            if (ModFeatureGate.IsRgbActive(Config))
            {
                _rgbHue += Time.deltaTime * Config.RgbSpeed * 0.15f;
                Config.Colors.Head = ColorUtil.HsvCycle(_rgbHue);
                Config.Colors.Spine = ColorUtil.HsvCycle(_rgbHue + 0.12f);
                Config.Colors.LegLeft = ColorUtil.HsvCycle(_rgbHue + 0.24f);
                Config.Colors.LegRight = ColorUtil.HsvCycle(_rgbHue + 0.36f);
                Config.Colors.HandLeft = ColorUtil.HsvCycle(_rgbHue + 0.48f);
                Config.Colors.HandRight = ColorUtil.HsvCycle(_rgbHue + 0.6f);
                Config.Colors.Crown = ColorUtil.HsvCycle(_rgbHue + 0.15f);
                colorsChanged = true;
            }

            bool inLobbyMenu = LobbyPerformance.IsLobbyMenuOnly();
            bool inLobby = IsInsideLobby();

            if (colorsChanged && ModFeatureGate.IsBodyActive(Config))
            {
                if (inLobbyMenu)
                {
                    float now = Time.realtimeSinceStartup;
                    if (now - _lastRgbLobbyLocalApplyTime >= 0.14f)
                    {
                        _lastRgbLobbyLocalApplyTime = now;
                        // RGB: no clearThrottle (preserva throttle por hash/tiempo).
                        ApplyBodyOnly(clearThrottle: false);
                        if (ModFeatureGate.IsGlowActive(Config) && Config.Glow.MaintainInLobby)
                        {
                            ApplyGlowOnly();
                        }
                    }
                }
                else
                {
                    // RGB en partida: throttle a ~12.5 Hz (antes era cada frame ~60 Hz).
                    // El MaintainSetLineColor del patch ya mantiene los colores aplicados continuamente.
                    float nowMatch = Time.realtimeSinceStartup;
                    if (nowMatch - _lastRgbLobbyLocalApplyTime >= 0.1f)
                    {
                        _lastRgbLobbyLocalApplyTime = nowMatch;
                        ApplyBodyOnly(clearThrottle: false);
                    }

                    if (MultiplayerCompat.AllowAutoColorPublish(Config))
                    {
                        ColorBroadcastService.PublishIfNeeded(Config.Colors);
                    }
                }
            }

            bool inLobbyNow = MatchmakingHandler.Instance != null && MatchmakingHandler.Instance.IsInsideLobby;
            if (inLobbyNow && !_wasInLobby)
            {
                LineMaintainThrottle.Clear();
                BoneIndexCache.Clear();
                MaterialColorHelper.ClearCache();
                LocalPlayerResolver.InvalidateCache();
                PlayerSlotCache.Invalidate();
                Patch_SetLinePositions_Update.ClearCache();
                MatchSessionState.ResetForLobbyMenu();
                LobbyRemotePaintPolicy.OnEnteredLobby();
                LobbySyncScheduler.OnEnteredLobby();
            }
            else if (inLobbyMenu && !_wasInLobby)
            {
                LobbySyncScheduler.OnEnteredLobby();
            }
            else if (!inLobbyMenu && MatchmakingHandler.IsNetworkMatch && !MatchSessionState.HasBootstrapped)
            {
                HalfColorCache.Clear();
                BoneIndexCache.Clear();
                GlowAuraRenderer.ResetSyncState();
                GameNetworkCache.Invalidate();
                LocalPlayerResolver.InvalidateCache();
                PlayerSlotCache.Invalidate();
                HatSpriteFactory.ClearCache();
                ShoeSpriteFactory.ClearCache();
                Patch_SetLinePositions_Update.ClearCache();
                LobbySyncScheduler.Clear();
                MatchBootstrap.BeginMatchEntry();
                MatchEntryColorScheduler.ScheduleLocalApplyOnce();
            }
            else if (_wasInLobby && !inLobbyNow && Config.Glow != null && Config.Glow.Enabled)
            {
                MatchEntryColorScheduler.ScheduleLocalApplyOnce();
            }

            if (inLobbyMenu)
            {
                LobbySyncScheduler.Tick();
            }

            ModColorPingSync.Tick();
            MatchEntryColorScheduler.Tick();
            TickWeaponColor();
            TickShoes();
            TickTops();
            TickObjects();
            TickWings();

            if (inLobbyMenu && Config.Glow != null && !Config.Glow.MaintainInLobby && Config.Glow.Enabled)
            {
                Controller local = GetLocalControllerCached();
                if (local != null)
                {
                    GlowAuraRenderer.RemoveAll(local);
                }
            }

            if (!_gui.Visible
                && (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Y)))
            {
                _focusReleaseFramesRemaining = 8;
            }

            _wasInLobby = inLobby;
        }

        public void SetMenuVisible(bool visible)
        {
            if (_gui.Visible == visible)
            {
                return;
            }

            _gui.Visible = visible;
            if (!visible)
            {
                _releaseGuiFocusOnNextGui = true;
                _focusReleaseFramesRemaining = 10;
            }
        }

        private static bool IsInsideLobby()
        {
            return MatchmakingHandler.Instance != null && MatchmakingHandler.Instance.IsInsideLobby;
        }

        public void OnGUI()
        {
            _gui.Draw();

            if (_gui.Visible)
            {
                return;
            }

            if (_releaseGuiFocusOnNextGui)
            {
                GuiFocusHelper.ReleaseAllForChat();
                _releaseGuiFocusOnNextGui = false;
            }

            if (_focusReleaseFramesRemaining > 0)
            {
                GuiFocusHelper.ReleaseAllForChat();
                _focusReleaseFramesRemaining--;
            }
            else
            {
                GuiFocusHelper.ClearStuckSfccFocusWhenMenuClosed();
            }
        }

        public void ApplyToLocalPlayer()
        {
            Controller local = LocalPlayerResolver.GetLocalController();
            TryApplyToController(local);
        }

        public void ApplyBodyOnly()
        {
            ApplyBodyOnly(clearThrottle: true);
        }

        /// <summary>Para RGB animado: clearThrottle=false evita anular el throttle por línea.</summary>
        private void ApplyBodyOnly(bool clearThrottle)
        {
            if (!ModFeatureGate.IsBodyActive(Config))
            {
                return;
            }

            Controller local = GetLocalControllerCached();
            if (local != null)
            {
                PlayerColorApplier.ApplyBodyColors(local, Config.Colors, clearThrottle);
            }
        }

        private void TickWeaponColor()
        {
            if (!ModFeatureGate.IsWeaponActive(Config))
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (now - _lastWeaponColorTickTime < WeaponColorTickInterval)
            {
                return;
            }

            _lastWeaponColorTickTime = now;
            Controller local = GetLocalControllerCached();
            if (local == null)
            {
                return;
            }

            Transform activeWeapon;
            WeaponColorResolver.HasActiveWeaponChanged(local, out activeWeapon);
            if (activeWeapon == null)
            {
                return;
            }

            WeaponColorApplier.Apply(activeWeapon, Config.Weapon, force: false);
        }

        public void ApplyWeaponOnly()
        {
            if (Config.Weapon == null || !ModFeatureGate.IsWeaponActive(Config))
            {
                return;
            }

            WeaponColorApplier.MarkDirty();
            Controller local = LocalPlayerResolver.GetLocalController();
            WeaponColorApplier.ApplyToLocalController(local, Config.Weapon, force: true);
        }

        public void ApplyHatOnly()
        {
            Controller local = LocalPlayerResolver.GetLocalController();
            if (local == null || Config.Hat == null)
            {
                return;
            }

            if (ModFeatureGate.IsHatActive(Config))
            {
                HatAttachmentRenderer.Apply(local, Config.Hat.HatId);
            }
            else
            {
                HatAttachmentRenderer.Remove(local);
            }

            SteamLobbyColorSync.PublishLocalHat();
        }

        public void ApplyWingOnly()
        {
            Controller local = LocalPlayerResolver.GetLocalController();
            if (local == null || Config.Wing == null)
            {
                return;
            }

            if (!WingFeatureFlags.Released)
            {
                WingCosmeticRenderer.Remove(local);
                return;
            }

            if (ModFeatureGate.IsWingActive(Config))
            {
                WingCosmeticRenderer.Apply(local, Config.Wing, Config.Colors);
            }
            else
            {
                WingCosmeticRenderer.Remove(local);
            }
        }

        public void ApplyShoeOnly()
        {
            if (!ShoeFeatureFlags.Released)
            {
                Controller strip = LocalPlayerResolver.GetLocalController();
                if (strip != null)
                {
                    ShoeAttachmentRenderer.Remove(strip);
                }

                return;
            }

            if (Config.Shoe == null)
            {
                return;
            }

            Controller local = LocalPlayerResolver.GetLocalController();
            if (local == null)
            {
                MatchEntryColorScheduler.ScheduleLocalApplyOnce();
                return;
            }

            if (ModFeatureGate.IsShoeActive(Config))
            {
                ShoeAttachmentRenderer.Apply(local, Config.Shoe.ShoeId);
            }
            else
            {
                ShoeAttachmentRenderer.Remove(local);
            }

            if (SteamReadyHelper.IsReady())
            {
                SteamLobbyColorSync.PublishLocalShoe();
            }
        }

        private void TickShoes()
        {
            if (!ShoeFeatureFlags.Released || !ModFeatureGate.IsShoeActive(Config))
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            float shoeInterval = ShoeTickInterval;
            if (LobbyPerformance.IsLobbyMenuOnly())
            {
                shoeInterval = ShoeTickIntervalLobbyBusy;
            }

            if (now - _lastShoeTickTime < shoeInterval)
            {
                return;
            }

            _lastShoeTickTime = now;
            Controller local = GetLocalControllerCached();
            if (local == null)
            {
                return;
            }

            int id = local.GetInstanceID();
            if (!ShoeAttachmentRenderer.HasShoesForController(id))
            {
                ApplyShoeOnly();
            }
            else if (shoeInterval <= ShoeTickInterval + 0.001f)
            {
                ShoeAttachmentRenderer.RefreshForController(local);
            }
        }

        public void ApplyTopsOnly()
        {
            if (!TopsFeatureFlags.Released)
            {
                Controller strip = LocalPlayerResolver.GetLocalController();
                if (strip != null)
                {
                    TopsAttachmentRenderer.Remove(strip);
                }

                return;
            }

            if (Config.Tops == null)
            {
                return;
            }

            Controller local = LocalPlayerResolver.GetLocalController();
            if (local == null)
            {
                MatchEntryColorScheduler.ScheduleLocalApplyOnce();
                return;
            }

            if (ModFeatureGate.IsTopsActive(Config))
            {
                TopsAttachmentRenderer.Apply(local, Config.Tops.TopsId);
            }
            else
            {
                TopsAttachmentRenderer.Remove(local);
            }
        }

        private void TickTops()
        {
            if (!TopsFeatureFlags.Released || !ModFeatureGate.IsTopsActive(Config))
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (now - _lastTopsTickTime < TopsTickInterval)
            {
                return;
            }

            _lastTopsTickTime = now;
            Controller local = GetLocalControllerCached();
            if (local == null)
            {
                return;
            }

            int id = local.GetInstanceID();
            if (!TopsAttachmentRenderer.HasTopsForController(id))
            {
                ApplyTopsOnly();
            }
            else
            {
                TopsAttachmentRenderer.RefreshForController(local);
            }
        }

        public void ApplyObjectsOnly()
        {
            if (!ObjectFeatureFlags.Released)
            {
                Controller strip = LocalPlayerResolver.GetLocalController();
                if (strip != null)
                {
                    ObjectsAttachmentRenderer.Remove(strip);
                }

                return;
            }

            if (Config.Object == null)
            {
                return;
            }

            Controller local = LocalPlayerResolver.GetLocalController();
            if (local == null)
            {
                MatchEntryColorScheduler.ScheduleLocalApplyOnce();
                return;
            }

            if (ModFeatureGate.IsObjectsActive(Config))
            {
                ObjectsAttachmentRenderer.Apply(local, Config.Object.ObjectId, Config.Object.Scale);
            }
            else
            {
                ObjectsAttachmentRenderer.Remove(local);
            }

            if (SteamReadyHelper.IsReady())
            {
                SteamLobbyColorSync.PublishLocalObject();
            }
        }

        private void TickObjects()
        {
            if (!ObjectFeatureFlags.Released || !ModFeatureGate.IsObjectsActive(Config))
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            float objectInterval = ObjectTickInterval;
            if (LobbyPerformance.IsLobbyMenuOnly())
            {
                objectInterval = ObjectTickIntervalLobbyBusy;
            }

            if (now - _lastObjectTickTime < objectInterval)
            {
                return;
            }

            _lastObjectTickTime = now;
            Controller local = GetLocalControllerCached();
            if (local == null)
            {
                return;
            }

            int id = local.GetInstanceID();
            if (!ObjectsAttachmentRenderer.HasObjectsForController(id))
            {
                ApplyObjectsOnly();
            }
            else if (objectInterval <= ObjectTickInterval + 0.001f)
            {
                ObjectsAttachmentRenderer.RefreshForController(local);
            }
        }

        private void TickWings()
        {
            if (!ModFeatureGate.IsWingActive(Config))
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (now - _lastWingTickTime < WingTickInterval)
            {
                return;
            }

            _lastWingTickTime = now;
            Controller local = GetLocalControllerCached();
            if (local == null)
            {
                return;
            }

            int id = local.GetInstanceID();
            if (!WingCosmeticRenderer.HasWingsForController(id))
            {
                ApplyWingOnly();
            }
            else
            {
                WingCosmeticRenderer.TickSway(local);
            }
        }

        /// <summary>Cache O(1) del controller local dentro de un mismo frame.</summary>
        private Controller GetLocalControllerCached()
        {
            int frame = Time.frameCount;
            if (frame == _cachedLocalControllerFrame && _cachedLocalControllerThisFrame != null)
            {
                return _cachedLocalControllerThisFrame;
            }

            _cachedLocalControllerThisFrame = LocalPlayerResolver.GetLocalController();
            _cachedLocalControllerFrame = frame;
            return _cachedLocalControllerThisFrame;
        }

        public void ApplyGlowOnly()
        {
            Controller local = LocalPlayerResolver.GetLocalController();
            if (local == null || Config.Glow == null)
            {
                return;
            }

            GlowAuraRenderer.ResetSyncState();
            if (Config.Glow.Enabled)
            {
                PlayerColorApplier.ApplyGlowAura(local, Config.Glow, Config.Colors);
            }
            else
            {
                PlayerColorApplier.RemoveGlow(local);
            }
        }

        public void BroadcastColors()
        {
            if (!ModFeatureGate.IsBodyActive(Config))
            {
                return;
            }

            ColorBroadcastService.PublishIfNeeded(Config.Colors, force: true);
        }

        public void MergeConfigFrom(ColorConfig source)
        {
            if (source == null)
            {
                return;
            }

            Config.Colors = source.Colors.Clone();
            Config.Glow = source.Glow.Clone();
            Config.Hat = source.Hat != null ? source.Hat.Clone() : new HatSettings();
            Config.Shoe = source.Shoe != null ? source.Shoe.Clone() : new ShoeSettings();
            Config.Tops = source.Tops != null ? source.Tops.Clone() : new TopsSettings();
            Config.Wing = source.Wing != null ? source.Wing.Clone() : new WingSettings();
            Config.Object = source.Object != null ? source.Object.Clone() : new ObjectSettings();
            Config.Weapon = source.Weapon != null ? source.Weapon.Clone() : new WeaponColorSettings();
            Config.BodyCustomizationActive = source.BodyCustomizationActive;
            Config.AnimatedRgb = source.AnimatedRgb;
            Config.RgbSpeed = source.RgbSpeed;
            Config.ActivePreset = source.ActivePreset;
            Config.UseUniformSkin = source.UseUniformSkin;
            Config.SafeVanillaMultiplayer = source.SafeVanillaMultiplayer;
            Config.MenuLanguage = source.MenuLanguage;
        }

        public void ApplyFullSkinFromConfig()
        {
            HalfColorCache.Clear();
            LineMaintainThrottle.MarkDirty();
            Controller local = LocalPlayerResolver.GetLocalController();
            if (local != null)
            {
                BoneIndexCache.ClearController(local);
            }

            ApplyBodyOnly();

            if (ModFeatureGate.IsGlowActive(Config))
            {
                ApplyGlowOnly();
            }
            else if (local != null)
            {
                PlayerColorApplier.RemoveGlow(local);
            }

            if (ModFeatureGate.IsHatActive(Config))
            {
                ApplyHatOnly();
            }
            else if (local != null)
            {
                HatAttachmentRenderer.Remove(local);
            }

            if (ModFeatureGate.IsWingActive(Config))
            {
                ApplyWingOnly();
            }
            else if (local != null)
            {
                WingCosmeticRenderer.Remove(local);
            }

            if (ModFeatureGate.IsShoeActive(Config))
            {
                ApplyShoeOnly();
            }
            else if (local != null)
            {
                ShoeAttachmentRenderer.Remove(local);
            }

            if (ModFeatureGate.IsTopsActive(Config))
            {
                ApplyTopsOnly();
            }
            else if (local != null)
            {
                TopsAttachmentRenderer.Remove(local);
            }

            if (ModFeatureGate.IsObjectsActive(Config))
            {
                ApplyObjectsOnly();
            }
            else if (local != null)
            {
                ObjectsAttachmentRenderer.Remove(local);
            }

            if (ModFeatureGate.IsWeaponActive(Config))
            {
                ApplyWeaponOnly();
            }

            if (ModFeatureGate.IsBodyActive(Config))
            {
                BroadcastColors();
            }
        }

        public BodyColors ResolveColorsForController(Controller controller)
        {
            if (controller == null || Config == null)
            {
                return null;
            }

            if (LocalPlayerResolver.IsLocalController(controller))
            {
                return Config.Colors;
            }

            return ResolveRemoteColorsOnly(controller);
        }

        public void TryApplyToController(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            if (LocalPlayerResolver.IsLocalController(controller))
            {
                LocalPlayerResolver.RefreshCache(controller);
                PlayerColorApplier.Apply(controller, Config);
                ApplyWeaponOnly();
                return;
            }

            BodyColors remote;
            ulong peerSteam;
            if (!RemoteApplyGate.CanApplyRemoteBody(controller, out remote, out peerSteam))
            {
                return;
            }

            PlayerColorApplier.ApplyRemoteCosmeticOnly(controller, remote);
        }

        public void TryApplyRemoteHat(Controller controller)
        {
            if (controller == null || !IsNetworkMatch() || LocalPlayerResolver.IsLocalController(controller))
            {
                return;
            }

            if (!RemoteApplyGate.CanApplyRemoteCosmetics(controller))
            {
                return;
            }

            ulong steamId;
            if (!RemoteApplyGate.TryGetPeerSteamId(controller, out steamId))
            {
                return;
            }

            string hatId;
            if (!RemoteHatRegistry.TryGetBySteam(steamId, out hatId))
            {
                hatId = "none";
            }

            PlayerColorApplier.ApplyRemoteHatOnly(controller, hatId);
        }

        public void TryApplyRemoteShoe(Controller controller)
        {
            if (controller == null || !IsNetworkMatch() || LocalPlayerResolver.IsLocalController(controller))
            {
                return;
            }

            if (!RemoteApplyGate.CanApplyRemoteCosmetics(controller))
            {
                return;
            }

            ulong steamId;
            if (!RemoteApplyGate.TryGetPeerSteamId(controller, out steamId))
            {
                return;
            }

            string shoeId;
            if (!RemoteShoeRegistry.TryGetBySteam(steamId, out shoeId))
            {
                shoeId = "none";
            }

            PlayerColorApplier.ApplyRemoteShoeOnly(controller, shoeId);
        }

        public void TryApplyRemoteObject(Controller controller)
        {
            if (controller == null || !IsNetworkMatch() || LocalPlayerResolver.IsLocalController(controller))
            {
                return;
            }

            if (!RemoteApplyGate.CanApplyRemoteCosmetics(controller))
            {
                return;
            }

            ulong steamId;
            if (!RemoteApplyGate.TryGetPeerSteamId(controller, out steamId))
            {
                return;
            }

            string objectId;
            if (!RemoteObjectRegistry.TryGetBySteam(steamId, out objectId))
            {
                objectId = "none";
            }

            PlayerColorApplier.ApplyRemoteObjectOnly(controller, objectId);
        }

        public bool RemotePlayerHasMod(Controller controller)
        {
            ulong peerSteam;
            return RemoteApplyGate.TryGetPeerSteamId(controller, out peerSteam)
                && RemoteApplyGate.HasConfirmedModColors(peerSteam);
        }

        private BodyColors ResolveRemoteColorsOnly(Controller controller)
        {
            BodyColors remote;
            ulong peerSteam;
            if (!RemoteApplyGate.CanApplyRemoteBody(controller, out remote, out peerSteam))
            {
                return null;
            }

            return remote;
        }

        private static bool IsNetworkMatch()
        {
            return MatchmakingHandler.IsNetworkMatch;
        }

        public void OnApplicationQuit()
        {
            if (Config != null)
            {
                ColorConfigStore.Save(Config);
                StyleSlotStore.SaveAll(Config);
            }
        }
    }
}
