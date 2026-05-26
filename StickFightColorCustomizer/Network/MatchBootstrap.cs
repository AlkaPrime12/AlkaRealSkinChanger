using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Hosting;
using StickFightColorCustomizer.Models;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Arranque de partida: refresh + publish síncronos, enqueue retardado, reintentos de detección.
    /// </summary>
    public static class MatchBootstrap
    {
        private const int EnqueueDelayFrames = 2;
        /// <summary>Intentos de detección al empezar partida (inicial + reintentos).</summary>
        private const int MaxDetectionAttempts = 5;
        private const float RetryIntervalFastSeconds = 2f;
        private const float RetryIntervalSlowSeconds = 4f;
        private const float LobbyPublishCooldown = 2f;

        private static bool _active;
        private static int _enqueueDelayRemaining;
        private static int _detectionAttemptsDone;
        private static float _lastRetryTime;
        private static float _lastLobbyPublishTime;

        public static bool IsActive
        {
            get { return _active; }
        }

        public static void BeginMatchEntry()
        {
            if (_active || ColorCustomizerApp.Instance == null || ColorCustomizerApp.Instance.Config == null)
            {
                return;
            }

            _active = true;
            _enqueueDelayRemaining = EnqueueDelayFrames;
            _detectionAttemptsDone = 1;
            _lastRetryTime = UnityEngine.Time.realtimeSinceStartup;

            MatchEntryColorScheduler.ResetForMatchEntry();
            MatchSessionState.MarkBootstrapped();

            SteamLobbyColorSync.RefreshFromLobby(true, mergeOnly: false);
            TryPublishLocalPresence(force: true);
            BodyColors colors = ColorCustomizerApp.Instance.Config.Colors;
            if (colors != null)
            {
                ModColorPingSync.BroadcastColorsThrottled(colors, true);
            }
            RemoteColorRegistry.SyncModPresenceFromCache();

            MatchEntryColorScheduler.ScheduleLocalApplyOnce();

            int detected = ModPresenceRegistry.Count;
            ModLog.Info("SFCC: deteccion 1/" + MaxDetectionAttempts + " — mods con sfcc: " + detected);
        }

        public static void OnEnteredLobby()
        {
            TryPublishLocalPresence(force: false);
        }

        public static void Tick()
        {
            if (_enqueueDelayRemaining > 0)
            {
                _enqueueDelayRemaining--;
                if (_enqueueDelayRemaining == 0 && !SpawnBurstCoordinator.IsInBurst)
                {
                    MatchEntryColorScheduler.EnqueueMissingRemoteWithMod();
                }
            }

            if (!_active || _detectionAttemptsDone >= MaxDetectionAttempts)
            {
                if (_detectionAttemptsDone >= MaxDetectionAttempts)
                {
                    _active = false;
                }

                return;
            }

            float now = UnityEngine.Time.realtimeSinceStartup;
            float interval = _detectionAttemptsDone < 3 ? RetryIntervalFastSeconds : RetryIntervalSlowSeconds;
            if (now - _lastRetryTime < interval)
            {
                return;
            }

            _lastRetryTime = now;
            _detectionAttemptsDone++;

            TryPublishLocalPresence(force: false);
            if (ColorCustomizerApp.Instance != null && ColorCustomizerApp.Instance.Config != null)
            {
                ModColorPingSync.BroadcastColorsThrottled(ColorCustomizerApp.Instance.Config.Colors, false);
            }

            SteamLobbyColorSync.RefreshFromLobby(true, mergeOnly: true);
            MatchEntryColorScheduler.EnqueueMissingRemoteWithMod();

            ModLog.Info("SFCC: deteccion " + _detectionAttemptsDone + "/" + MaxDetectionAttempts
                + " — mods con sfcc: " + ModPresenceRegistry.Count);

            if (_detectionAttemptsDone >= MaxDetectionAttempts)
            {
                _active = false;
            }
        }

        public static void Clear()
        {
            _active = false;
            _enqueueDelayRemaining = 0;
            _detectionAttemptsDone = 0;
            _lastRetryTime = 0f;
        }

        private static void TryPublishLocalPresence(bool force)
        {
            if (ColorCustomizerApp.Instance == null || ColorCustomizerApp.Instance.Config == null)
            {
                return;
            }

            float now = UnityEngine.Time.realtimeSinceStartup;
            if (!force && now - _lastLobbyPublishTime < LobbyPublishCooldown)
            {
                return;
            }

            _lastLobbyPublishTime = now;
            BodyColors colors = ColorCustomizerApp.Instance.Config.Colors;
            if (colors != null)
            {
                SteamLobbyColorSync.PublishLocal(colors);
            }
        }
    }
}
