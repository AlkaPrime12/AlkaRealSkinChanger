using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Ruta única para config y slots (MelonLoader UserData + ColorCustomizer).
    /// </summary>
    public static class ModStoragePaths
    {
        public enum LoaderKind
        {
            MelonLoader,
            BepInEx
        }

        private static LoaderKind _loader = LoaderKind.MelonLoader;
        private static string _bepInExConfigRoot;
        private static string _colorCustomizerRoot;

        public static void SetLoader(LoaderKind loader)
        {
            _loader = loader;
            _colorCustomizerRoot = null;
        }

        public static void SetBepInExConfigRoot(string bepInExConfigPath)
        {
            _loader = LoaderKind.BepInEx;
            _bepInExConfigRoot = bepInExConfigPath;
            _colorCustomizerRoot = null;
        }

        public static string ColorCustomizerRoot
        {
            get
            {
                if (string.IsNullOrEmpty(_colorCustomizerRoot))
                {
                    _colorCustomizerRoot = ResolveColorCustomizerRoot();
                }

                return _colorCustomizerRoot;
            }
        }

        public static string ConfigFilePath
        {
            get { return Path.Combine(ColorCustomizerRoot, "config.json"); }
        }

        public static string SlotsRoot
        {
            get { return Path.Combine(ColorCustomizerRoot, "slots"); }
        }

        public static string GetSlotFilePath(int index)
        {
            return Path.Combine(SlotsRoot, "slot" + index + ".json");
        }

        public static void EnsureColorCustomizerDirectory()
        {
            if (!Directory.Exists(ColorCustomizerRoot))
            {
                Directory.CreateDirectory(ColorCustomizerRoot);
            }
        }

        public static void EnsureSlotsDirectory()
        {
            EnsureColorCustomizerDirectory();
            if (!Directory.Exists(SlotsRoot))
            {
                Directory.CreateDirectory(SlotsRoot);
            }
        }

        private static string ResolveColorCustomizerRoot()
        {
            if (_loader == LoaderKind.BepInEx && !string.IsNullOrEmpty(_bepInExConfigRoot))
            {
                return Path.Combine(_bepInExConfigRoot, "AlkaSkin");
            }

            string userData = TryGetMelonUserDataDirectory();

            if (string.IsNullOrEmpty(userData))
            {
                string gameRoot = Path.GetDirectoryName(Application.dataPath);
                userData = Path.Combine(gameRoot, "UserData");
            }

            return Path.Combine(userData, "ColorCustomizer");
        }

        private static string TryGetMelonUserDataDirectory()
        {
            try
            {
                Type envType = Type.GetType("MelonLoader.MelonEnvironment, MelonLoader");
                if (envType != null)
                {
                    PropertyInfo prop = envType.GetProperty(
                        "UserDataDirectory",
                        BindingFlags.Public | BindingFlags.Static);
                    if (prop != null)
                    {
                        string dir = prop.GetValue(null, null) as string;
                        if (!string.IsNullOrEmpty(dir))
                        {
                            return dir;
                        }
                    }
                }

                Type utilsType = Type.GetType("MelonLoader.MelonUtils, MelonLoader");
                if (utilsType != null)
                {
                    PropertyInfo prop = utilsType.GetProperty(
                        "UserDataDirectory",
                        BindingFlags.Public | BindingFlags.Static);
                    if (prop != null)
                    {
                        return prop.GetValue(null, null) as string;
                    }
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
