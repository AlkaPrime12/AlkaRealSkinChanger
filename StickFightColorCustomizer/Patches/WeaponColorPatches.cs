using System.Reflection;
using HarmonyLib;
using StickFightColorCustomizer.Core;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Patches
{
    public static class WeaponColorPatchHandlers
    {
        public static void OnWeaponEnabled(MonoBehaviour weaponBehaviour)
        {
            if (weaponBehaviour == null || ColorCustomizerApp.Instance == null)
            {
                return;
            }

            Models.ColorConfig config = ColorCustomizerApp.Instance.Config;
            if (config == null || config.Weapon == null || !ModFeatureGate.IsWeaponActive(config))
            {
                return;
            }

            if (!WeaponColorResolver.BelongsToLocalPlayer(weaponBehaviour.transform))
            {
                return;
            }

            WeaponColorApplier.MarkDirty();
            WeaponColorApplier.Apply(weaponBehaviour.transform, config.Weapon, force: true);
        }
    }

    [HarmonyPatch]
    public static class Patch_Weapon_OnEnable
    {
        private static MethodBase _targetMethod;

        public static bool Prepare()
        {
            return ResolveTarget() != null;
        }

        static MethodBase TargetMethod()
        {
            return ResolveTarget();
        }

        private static MethodBase ResolveTarget()
        {
            if (_targetMethod != null)
            {
                return _targetMethod;
            }

            System.Type weaponType = AccessTools.TypeByName("Weapon");
            if (weaponType == null)
            {
                return null;
            }

            _targetMethod = AccessTools.Method(weaponType, "OnEnable")
                ?? AccessTools.Method(weaponType, "Awake");
            return _targetMethod;
        }

        [HarmonyPostfix]
        public static void Postfix(MonoBehaviour __instance)
        {
            WeaponColorPatchHandlers.OnWeaponEnabled(__instance);
        }
    }
}
