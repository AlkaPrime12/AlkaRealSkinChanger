using HarmonyLib;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class WeaponColorResolver
    {
        private static int _lastActiveWeaponInstanceId = -1;
        private static bool _loggedMissingWeaponsRoot;
        private static System.Type _weaponComponentType;

        private static System.Type WeaponComponentType
        {
            get
            {
                if (_weaponComponentType == null)
                {
                    _weaponComponentType = AccessTools.TypeByName("Weapon");
                }

                return _weaponComponentType;
            }
        }

        public static Transform FindWeaponsRoot(Controller controller)
        {
            if (controller == null)
            {
                return null;
            }

            Transform direct = controller.transform.Find("Weapons");
            if (direct != null)
            {
                return direct;
            }

            Transform[] children = controller.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; i++)
            {
                Transform t = children[i];
                if (t != null && t.name == "Weapons")
                {
                    return t;
                }
            }

            return null;
        }

        public static Transform GetActiveWeapon(Controller controller)
        {
            Transform weaponsRoot = FindWeaponsRoot(controller);
            if (weaponsRoot == null)
            {
                LogMissingOnce();
                return null;
            }

            Transform best = null;
            int bestScore = -1;
            int childCount = weaponsRoot.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = weaponsRoot.GetChild(i);
                if (child == null || !child.gameObject.activeInHierarchy)
                {
                    continue;
                }

                int score = ScoreWeaponCandidate(child);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = child;
                }
            }

            return best;
        }

        private static int ScoreWeaponCandidate(Transform weaponRoot)
        {
            int score = 0;
            if (weaponRoot.gameObject.activeSelf)
            {
                score += 100;
            }

            if (WeaponComponentType != null && weaponRoot.GetComponent(WeaponComponentType) != null)
            {
                score += 200;
            }

            score += CountTintableRenderers(weaponRoot) * 15;
            return score;
        }

        public static int CountTintableRenderers(Transform weaponRoot)
        {
            if (weaponRoot == null)
            {
                return 0;
            }

            int count = 0;
            Renderer[] renderers = weaponRoot.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (WeaponColorApplier.IsTintableRenderer(renderers[i]))
                {
                    count++;
                }
            }

            return count;
        }

        public static bool HasActiveWeaponChanged(Controller controller, out Transform activeWeapon)
        {
            activeWeapon = GetActiveWeapon(controller);
            int id = activeWeapon != null ? activeWeapon.gameObject.GetInstanceID() : -1;
            if (id == _lastActiveWeaponInstanceId)
            {
                return false;
            }

            _lastActiveWeaponInstanceId = id;
            return true;
        }

        public static void InvalidateActiveWeaponCache()
        {
            _lastActiveWeaponInstanceId = -1;
        }

        public static bool BelongsToLocalPlayer(Transform weaponTransform)
        {
            if (weaponTransform == null)
            {
                return false;
            }

            Controller controller = weaponTransform.root.GetComponent<Controller>();
            return controller != null && LocalPlayerResolver.IsLocalPlayerFast(controller);
        }

        private static void LogMissingOnce()
        {
            if (_loggedMissingWeaponsRoot)
            {
                return;
            }

            _loggedMissingWeaponsRoot = true;
            ModLog.Warning("SFCC: no se encontró Character/Weapons para color de arma.");
        }
    }
}
