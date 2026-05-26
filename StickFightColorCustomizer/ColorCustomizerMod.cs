using MelonLoader;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer
{
    public sealed class ColorCustomizerMod : MelonMod
    {
        public static ColorCustomizerMod Instance { get; private set; }

        private ColorCustomizerApp _app;

        public Hosting.ColorCustomizerApp App
        {
            get { return _app; }
        }

        public Models.ColorConfig Config
        {
            get { return _app != null ? _app.Config : null; }
        }

        public override void OnApplicationStart()
        {
            EnsureInitialized();
        }

        public override void OnInitializeMelon()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (_app != null)
            {
                return;
            }

            ModStoragePaths.SetLoader(ModStoragePaths.LoaderKind.MelonLoader);
            ModLog.BindMelon();
            Instance = this;
            _app = new ColorCustomizerApp();
            _app.Initialize("StickFightColorCustomizer");
        }

        public override void OnUpdate()
        {
            if (_app != null)
            {
                _app.OnUpdate();
            }
        }

        public override void OnGUI()
        {
            if (_app != null)
            {
                _app.OnGUI();
            }
        }

        public override void OnApplicationQuit()
        {
            if (_app != null)
            {
                _app.OnApplicationQuit();
            }
        }

        public void SetMenuVisible(bool visible)
        {
            if (_app != null)
            {
                _app.SetMenuVisible(visible);
            }
        }

        public void ApplyToLocalPlayer()
        {
            if (_app != null)
            {
                _app.ApplyToLocalPlayer();
            }
        }

        public void ApplyBodyOnly()
        {
            if (_app != null)
            {
                _app.ApplyBodyOnly();
            }
        }

        public void ApplyWeaponOnly()
        {
            if (_app != null)
            {
                _app.ApplyWeaponOnly();
            }
        }

        public void ApplyHatOnly()
        {
            if (_app != null)
            {
                _app.ApplyHatOnly();
            }
        }

        public void ApplyWingOnly()
        {
            if (_app != null)
            {
                _app.ApplyWingOnly();
            }
        }

        public void ApplyShoeOnly()
        {
            if (_app != null)
            {
                _app.ApplyShoeOnly();
            }
        }

        public void ApplyGlowOnly()
        {
            if (_app != null)
            {
                _app.ApplyGlowOnly();
            }
        }

        public void BroadcastColors()
        {
            if (_app != null)
            {
                _app.BroadcastColors();
            }
        }

        public void MergeConfigFrom(Models.ColorConfig source)
        {
            if (_app != null)
            {
                _app.MergeConfigFrom(source);
            }
        }

        public void ApplyFullSkinFromConfig()
        {
            if (_app != null)
            {
                _app.ApplyFullSkinFromConfig();
            }
        }

        public Models.BodyColors ResolveColorsForController(Controller controller)
        {
            return _app != null ? _app.ResolveColorsForController(controller) : null;
        }

        public void TryApplyToController(Controller controller)
        {
            if (_app != null)
            {
                _app.TryApplyToController(controller);
            }
        }

        public void TryApplyRemoteHat(Controller controller)
        {
            if (_app != null)
            {
                _app.TryApplyRemoteHat(controller);
            }
        }

        public void TryApplyRemoteShoe(Controller controller)
        {
            if (_app != null)
            {
                _app.TryApplyRemoteShoe(controller);
            }
        }

        public void TryApplyRemoteObject(Controller controller)
        {
            if (_app != null)
            {
                _app.TryApplyRemoteObject(controller);
            }
        }

        public void ApplyObjectsOnly()
        {
            if (_app != null)
            {
                _app.ApplyObjectsOnly();
            }
        }

        public bool RemotePlayerHasMod(Controller controller)
        {
            return _app != null && _app.RemotePlayerHasMod(controller);
        }
    }
}
