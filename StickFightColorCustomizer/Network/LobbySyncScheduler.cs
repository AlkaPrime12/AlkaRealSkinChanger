using System.Collections.Generic;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Lobby: debounce refresh Steam, preview remoto 1/frame, sin enqueue masivo por OnLobbyDataUpdate.
    /// </summary>
    public static class LobbySyncScheduler
    {
        private const float DebounceSeconds = 1.25f;
        private const float HeartbeatSeconds = 6f;

        private static bool _refreshScheduled;
        private static float _refreshDueTime;
        private static float _lastHeartbeatTime;
        private static readonly Queue<ulong> DirtyPreviewSteamIds = new Queue<ulong>();
        private static readonly HashSet<ulong> DirtySet = new HashSet<ulong>();
        private static int _previewProcessedThisFrame;

        public static void OnEnteredLobby()
        {
            LobbyRemotePaintPolicy.OnEnteredLobby();
            _refreshScheduled = true;
            _refreshDueTime = Time.realtimeSinceStartup;
            _lastHeartbeatTime = 0f;
            DirtyPreviewSteamIds.Clear();
            DirtySet.Clear();
            SteamLobbyColorSync.ResetPublishCache();
            SteamLobbyColorSync.RefreshFromLobby(true, mergeOnly: false);
            TryLobbyHeartbeat(publishOnly: true);
            if (ColorCustomizerApp.Instance != null
                && ModFeatureGate.IsHatActive(ColorCustomizerApp.Instance.Config))
            {
                SteamLobbyColorSync.PublishLocalHat();
            }

            if (ColorCustomizerApp.Instance != null
                && ModFeatureGate.IsShoeActive(ColorCustomizerApp.Instance.Config))
            {
                SteamLobbyColorSync.PublishLocalShoe();
            }
        }

        public static void OnLobbyDataChanged()
        {
            _refreshScheduled = true;
            _refreshDueTime = Time.realtimeSinceStartup + DebounceSeconds;
        }

        public static void Tick()
        {
            _previewProcessedThisFrame = 0;
            float now = Time.realtimeSinceStartup;

            ColorConfig config = ColorCustomizerApp.Instance != null
                ? ColorCustomizerApp.Instance.Config
                : null;
            if (!ModFeatureGate.NeedsLobbyColorSync(config))
            {
                return;
            }

            if (_refreshScheduled && now >= _refreshDueTime)
            {
                _refreshScheduled = false;
                SteamLobbyColorSync.RefreshFromLobby(false, mergeOnly: true);
                MarkDirtyFromActiveRemotes();
            }

            TryLobbyHeartbeat(publishOnly: false);

            while (DirtyPreviewSteamIds.Count > 0 && _previewProcessedThisFrame < MatchEntryColorScheduler.MaxAppliesPerFrame)
            {
                ulong steamId = DirtyPreviewSteamIds.Dequeue();
                DirtySet.Remove(steamId);

                if (!ModPresenceRegistry.HasMod(steamId) || MatchEntryColorScheduler.IsKnownNoMod(steamId))
                {
                    continue;
                }

                EnqueuePreviewForSteamId(steamId);
                _previewProcessedThisFrame++;
            }
        }

        public static void Clear()
        {
            _refreshScheduled = false;
            _refreshDueTime = 0f;
            _lastHeartbeatTime = 0f;
            DirtyPreviewSteamIds.Clear();
            DirtySet.Clear();
            _previewProcessedThisFrame = 0;
        }

        private static void TryLobbyHeartbeat(bool publishOnly)
        {
            if (ColorCustomizerApp.Instance == null || ColorCustomizerApp.Instance.Config == null)
            {
                return;
            }

            if (MatchmakingHandler.Instance == null || !MatchmakingHandler.Instance.IsInsideLobby)
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (now - _lastHeartbeatTime < HeartbeatSeconds)
            {
                return;
            }

            _lastHeartbeatTime = now;
            BodyColors colors = ColorCustomizerApp.Instance.Config.Colors;
            if (colors == null)
            {
                return;
            }

            if (ModFeatureGate.IsBodyActive(ColorCustomizerApp.Instance.Config))
            {
                SteamLobbyColorSync.PublishLocal(colors);
                if (!publishOnly)
                {
                    ModColorPingSync.BroadcastColorsThrottled(colors, false);
                }
            }

            if (ModFeatureGate.IsHatActive(ColorCustomizerApp.Instance.Config))
            {
                SteamLobbyColorSync.PublishLocalHat();
            }

            if (ModFeatureGate.IsShoeActive(ColorCustomizerApp.Instance.Config))
            {
                SteamLobbyColorSync.PublishLocalShoe();
            }
        }

        private static void MarkDirtyFromActiveRemotes()
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

                if (!LobbyRemotePaintPolicy.ShouldEnqueueRemotePreview(steamId))
                {
                    continue;
                }

                MarkDirty(steamId);
            }
        }

        public static void MarkDirty(ulong steamId)
        {
            if (steamId == 0 || DirtySet.Contains(steamId))
            {
                return;
            }

            DirtySet.Add(steamId);
            DirtyPreviewSteamIds.Enqueue(steamId);
        }

        private static void EnqueuePreviewForSteamId(ulong steamId)
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
                    MatchEntryColorScheduler.EnqueueRemote(controller);
                    return;
                }
            }
        }
    }
}
