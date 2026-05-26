using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Hosting;
using StickFightColorCustomizer.Patches;
using UnityEngine;

namespace StickFightColorCustomizer.BepInEx
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("com.bepis.bepinex.configurationmanager", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class AlkaSkinPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "alka.alkaskin";
        public const string PluginName = "AlkaSkin";
        public const string PluginVersion = "2.3.0";

        internal static ManualLogSource Log;
        private static ColorCustomizerApp _app;
        private Harmony _harmony;
        private GameObject _runtimeObject;

        private void Awake()
        {
            Log = Logger;
            ModLog.BindBepInEx(
                msg => Log.LogInfo(msg),
                msg => Log.LogWarning(msg),
                msg => Log.LogError(msg));

            ModStoragePaths.SetBepInExConfigRoot(Paths.ConfigPath);
            ColorConfigStore.TryMigrateFromMelonUserData();

            _app = new ColorCustomizerApp();
            _app.Initialize(PluginGuid);

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll(typeof(Patch_ControllerHandler_CreatePlayer).Assembly);

            _runtimeObject = new GameObject("AlkaSkin_Runtime");
            Object.DontDestroyOnLoad(_runtimeObject);
            _runtimeObject.AddComponent<AlkaSkinBehaviour>();

            Log.LogInfo("AlkaSkin BepInEx " + PluginVersion + " loaded.");
        }

        private void OnDestroy()
        {
            if (_app != null)
            {
                _app.OnApplicationQuit();
                _app.Shutdown();
                _app = null;
            }

            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
                _harmony = null;
            }

            if (_runtimeObject != null)
            {
                Object.Destroy(_runtimeObject);
                _runtimeObject = null;
            }
        }
    }

    internal sealed class AlkaSkinBehaviour : MonoBehaviour
    {
        private void Update()
        {
            if (ColorCustomizerApp.Instance != null)
            {
                ColorCustomizerApp.Instance.OnUpdate();
            }
        }

        private void OnGUI()
        {
            if (ColorCustomizerApp.Instance != null)
            {
                ColorCustomizerApp.Instance.OnGUI();
            }
        }
    }
}
