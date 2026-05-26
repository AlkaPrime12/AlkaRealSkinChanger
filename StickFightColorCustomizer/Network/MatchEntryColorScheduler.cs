using System.Collections.Generic;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Colores remotos: cola 1/frame, bootstrap retardado, reintentos de detección.
    /// </summary>
    public static class MatchEntryColorScheduler
    {
        private static readonly Queue<Controller> Pending = new Queue<Controller>();
        private static readonly HashSet<int> QueuedControllerIds = new HashSet<int>();
        private static readonly HashSet<ulong> KnownNoModSteamIds = new HashSet<ulong>();
        private static readonly HashSet<ulong> PaintedSteamIds = new HashSet<ulong>();

        private static bool _localApplyScheduled;
        private static int _processedThisFrame;

        public const int MaxAppliesPerFrame = 1;

        public static void ResetForMatchEntry()
        {
            Pending.Clear();
            QueuedControllerIds.Clear();
            PaintedSteamIds.Clear();
            _localApplyScheduled = false;
            _processedThisFrame = 0;
            PlayerColorApplier.ClearRemoteApplyCache();
        }

        public static void ResetLobbyPaintState()
        {
            PaintedSteamIds.Clear();
            Pending.Clear();
            QueuedControllerIds.Clear();
            PlayerColorApplier.ClearRemoteApplyCache();
        }

        public static void OnPlayerSpawnedNetwork()
        {
            SpawnBurstCoordinator.NotifyPlayerSpawnedNetwork();
            if (MatchBootstrap.IsActive)
            {
                return;
            }
        }

        public static void ScheduleLocalApplyOnce()
        {
            _localApplyScheduled = true;
        }

        public static void MarkKnownNoMod(ulong steamId)
        {
            if (steamId == 0)
            {
                return;
            }

            KnownNoModSteamIds.Add(steamId);
            RemoteColorRegistry.ClearPeerVanilla(steamId);
            StripRemoteCosmeticsForSteam(steamId);
        }

        private static void StripRemoteCosmeticsForSteam(ulong steamId)
        {
            if (ControllerHandler.Instance == null)
            {
                return;
            }

            List<Controller> active = ControllerHandler.Instance.ActivePlayers;
            if (active == null)
            {
                return;
            }

            for (int i = 0; i < active.Count; i++)
            {
                Controller controller = active[i];
                if (controller == null || LocalPlayerResolver.IsLocalPlayerFast(controller))
                {
                    continue;
                }

                ulong id;
                if (PlayerSlotCache.TryGetSteamId(controller.playerID, out id) && id == steamId)
                {
                    PlayerColorApplier.StripRemoteCosmetics(controller);
                }
            }
        }

        public static void ClearKnownNoMod(ulong steamId)
        {
            if (steamId != 0)
            {
                KnownNoModSteamIds.Remove(steamId);
            }
        }

        public static bool IsKnownNoMod(ulong steamId)
        {
            return steamId != 0 && KnownNoModSteamIds.Contains(steamId);
        }

        public static void EnqueueRemote(Controller controller)
        {
            if (controller == null || ColorCustomizerApp.Instance == null)
            {
                return;
            }

            if (LocalPlayerResolver.IsLocalPlayerFast(controller))
            {
                ScheduleLocalApplyOnce();
                return;
            }

            if (!MatchmakingHandler.IsNetworkMatch
                && (MatchmakingHandler.Instance == null || !MatchmakingHandler.Instance.IsInsideLobby))
            {
                return;
            }

            int controllerId = controller.GetInstanceID();
            if (QueuedControllerIds.Contains(controllerId))
            {
                return;
            }

            BodyColors remoteColors;
            if (!LobbyRemotePaintPolicy.CanApplyRemoteController(controller, out remoteColors))
            {
                return;
            }

            QueuedControllerIds.Add(controllerId);
            Pending.Enqueue(controller);
        }

        public static void EnqueueRemoteByPlayerId(int playerId)
        {
            if (ControllerHandler.Instance == null)
            {
                return;
            }

            List<Controller> active = ControllerHandler.Instance.ActivePlayers;
            if (active == null)
            {
                return;
            }

            for (int i = 0; i < active.Count; i++)
            {
                Controller controller = active[i];
                if (controller != null && controller.playerID == playerId)
                {
                    EnqueueRemote(controller);
                    return;
                }
            }
        }

        public static void EnqueueAllRemoteWithMod()
        {
            if (ControllerHandler.Instance == null || ColorCustomizerApp.Instance == null)
            {
                return;
            }

            List<Controller> active = ControllerHandler.Instance.ActivePlayers;
            if (active == null)
            {
                return;
            }

            for (int i = 0; i < active.Count; i++)
            {
                Controller controller = active[i];
                if (controller == null || LocalPlayerResolver.IsLocalPlayerFast(controller))
                {
                    continue;
                }

                EnqueueRemote(controller);
            }
        }

        public static void EnqueueMissingRemoteWithMod()
        {
            if (ControllerHandler.Instance == null)
            {
                return;
            }

            List<Controller> active = ControllerHandler.Instance.ActivePlayers;
            if (active == null)
            {
                return;
            }

            for (int i = 0; i < active.Count; i++)
            {
                Controller controller = active[i];
                if (controller == null || LocalPlayerResolver.IsLocalPlayerFast(controller))
                {
                    continue;
                }

                ulong steamId;
                if (!PlayerSlotCache.TryGetSteamId(controller.playerID, out steamId) || steamId == 0)
                {
                    continue;
                }

                if (PaintedSteamIds.Contains(steamId) || IsKnownNoMod(steamId))
                {
                    continue;
                }

                if (!RemoteApplyGate.HasConfirmedModColors(steamId))
                {
                    continue;
                }

                EnqueueRemote(controller);
            }
        }

        public static void Tick()
        {
            _processedThisFrame = 0;

            if (_localApplyScheduled)
            {
                bool applied = SteamLobbyColorSync.ApplyLocalPlayerOnly();
                if (applied)
                {
                    _localApplyScheduled = false;
                }
            }

            bool lobbyMenu = LobbyPerformance.IsLobbyMenuOnly();

            ModPresenceRegistry.TickPendingGrace();

            if (!lobbyMenu)
            {
                SpawnBurstCoordinator.Tick();
                MatchBootstrap.Tick();
            }

            while (Pending.Count > 0 && _processedThisFrame < MaxAppliesPerFrame)
            {
                Controller controller = Pending.Dequeue();
                if (controller == null)
                {
                    continue;
                }

                QueuedControllerIds.Remove(controller.GetInstanceID());

                if (ColorCustomizerApp.Instance != null)
                {
                    if (LocalPlayerResolver.IsLocalPlayerFast(controller))
                    {
                        ColorCustomizerApp.Instance.TryApplyToController(controller);
                    }
                    else
                    {
                        BodyColors recheck;
                        if (LobbyRemotePaintPolicy.CanApplyRemoteController(controller, out recheck))
                        {
                            ColorCustomizerApp.Instance.TryApplyToController(controller);
                            ColorCustomizerApp.Instance.TryApplyRemoteHat(controller);
                            ColorCustomizerApp.Instance.TryApplyRemoteShoe(controller);
                            ColorCustomizerApp.Instance.TryApplyRemoteObject(controller);
                            ulong steamId;
                            if (PlayerSlotCache.TryGetSteamId(controller.playerID, out steamId) && steamId != 0)
                            {
                                PaintedSteamIds.Add(steamId);
                            }
                        }
                    }
                }

                _processedThisFrame++;
            }
        }

        public static void Clear()
        {
            Pending.Clear();
            QueuedControllerIds.Clear();
            KnownNoModSteamIds.Clear();
            PaintedSteamIds.Clear();
            _localApplyScheduled = false;
            _processedThisFrame = 0;
            MatchBootstrap.Clear();
            LobbySyncScheduler.Clear();
            ModColorPingSync.Clear();
            PeerModLogger.Clear();
            PlayerColorApplier.ClearRemoteApplyCache();
            RemoteHatRegistry.Clear();
            RemoteShoeRegistry.Clear();
            RemoteObjectRegistry.Clear();
            HatAttachmentRenderer.ClearAll();
            WingCosmeticRenderer.ClearAll();
            ShoeAttachmentRenderer.ClearAll();
            TopsAttachmentRenderer.ClearAll();
            ObjectsAttachmentRenderer.ClearAll();
            WeaponColorApplier.ClearCache();
        }

        private static bool RemotePlayerHasMod(Controller controller)
        {
            ulong steamId;
            if (!PlayerSlotCache.TryGetSteamId(controller.playerID, out steamId) || steamId == 0)
            {
                return false;
            }

            if (KnownNoModSteamIds.Contains(steamId) && !ModPresenceRegistry.IsPending(steamId))
            {
                return false;
            }

            bool hasMod;
            if (PlayerSlotCache.TryGetHasMod(controller.playerID, out hasMod) && hasMod)
            {
                return true;
            }

            if (!ModPresenceRegistry.HasMod(steamId))
            {
                KnownNoModSteamIds.Add(steamId);
                PeerModLogger.LogVanillaOnce(steamId);
                return false;
            }

            PlayerSlotCache.InvalidatePlayer(controller.playerID);
            return PlayerSlotCache.TryGetHasMod(controller.playerID, out hasMod) && hasMod;
        }
    }
}
