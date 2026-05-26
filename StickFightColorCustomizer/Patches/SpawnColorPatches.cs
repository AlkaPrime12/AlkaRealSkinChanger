using HarmonyLib;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Network;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Patches
{
    [HarmonyPatch(typeof(ControllerHandler), "CreatePlayer")]
    public static class Patch_ControllerHandler_CreatePlayer
    {
        [HarmonyPostfix]
        public static void Postfix(Controller __result)
        {
            if (__result != null && LocalPlayerResolver.IsLocalPlayerFast(__result))
            {
                LocalPlayerResolver.RefreshCache(__result);
                BoneIndexCache.ClearController(__result);
                Patch_SetLinePositions_Update.ClearCache();
            }

            SpawnColorHandlers.HandleSpawnColor(__result);
        }
    }

    [HarmonyPatch(typeof(GameManager), "RevivePlayer")]
    public static class Patch_GameManager_RevivePlayer
    {
        [HarmonyPostfix]
        public static void Postfix(Controller playerToRevive)
        {
            if (playerToRevive != null && LocalPlayerResolver.IsLocalPlayerFast(playerToRevive))
            {
                LocalPlayerResolver.RefreshCache(playerToRevive);
                BoneIndexCache.ClearController(playerToRevive);
                Patch_SetLinePositions_Update.ClearCache();
            }

            SpawnColorHandlers.HandleSpawnColor(playerToRevive);
        }
    }

    [HarmonyPatch(typeof(MultiplayerManager), "OnPlayerSpawned")]
    public static class Patch_MultiplayerManager_OnPlayerSpawned
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (LobbyPerformance.IsLobbyMenuOnly())
            {
                return;
            }

            MatchEntryColorScheduler.OnPlayerSpawnedNetwork();
            SteamLobbyColorSync.RefreshFromLobby(true, mergeOnly: true);
            MatchEntryColorScheduler.EnqueueMissingRemoteWithMod();
        }
    }

    [HarmonyPatch(typeof(MultiplayerManager), "SpawnPlayerDummy")]
    public static class Patch_MultiplayerManager_SpawnPlayerDummy
    {
        [HarmonyPostfix]
        public static void Postfix(GameObject __result)
        {
            if (__result == null)
            {
                return;
            }

            Controller controller = __result.GetComponent<Controller>();
            SpawnColorHandlers.HandleSpawnColor(controller);
        }
    }

    internal static class SpawnColorHandlers
    {
        internal static void HandleSpawnColor(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            if (LobbyPerformance.IsLobbyMenuOnly())
            {
                if (LocalPlayerResolver.IsLocalPlayerFast(controller))
                {
                    MatchEntryColorScheduler.ScheduleLocalApplyOnce();
                    if (ColorCustomizerApp.Instance != null)
                    {
                        if (ModFeatureGate.IsWingActive(ColorCustomizerApp.Instance.Config))
                        {
                            ColorCustomizerApp.Instance.ApplyWingOnly();
                        }

                        if (ModFeatureGate.IsShoeActive(ColorCustomizerApp.Instance.Config))
                        {
                            ColorCustomizerApp.Instance.ApplyShoeOnly();
                        }

                        if (ModFeatureGate.IsTopsActive(ColorCustomizerApp.Instance.Config))
                        {
                            ColorCustomizerApp.Instance.ApplyTopsOnly();
                        }

                        if (ModFeatureGate.IsObjectsActive(ColorCustomizerApp.Instance.Config))
                        {
                            ColorCustomizerApp.Instance.ApplyObjectsOnly();
                        }
                    }
                }

                return;
            }

            SpawnBurstCoordinator.NotifySpawn();

            if (LocalPlayerResolver.IsLocalPlayerFast(controller))
            {
                SpawnBurstCoordinator.TryScheduleLocalApplyOnce();
                if (ColorCustomizerApp.Instance != null)
                {
                    ColorCustomizerApp.Instance.ApplyWeaponOnly();
                    ColorCustomizerApp.Instance.ApplyWingOnly();
                    ColorCustomizerApp.Instance.ApplyShoeOnly();
                    ColorCustomizerApp.Instance.ApplyTopsOnly();
                    ColorCustomizerApp.Instance.ApplyObjectsOnly();
                }
            }
            else if (!SpawnBurstCoordinator.IsInBurst)
            {
                MatchEntryColorScheduler.EnqueueRemote(controller);
            }
        }
    }
}
