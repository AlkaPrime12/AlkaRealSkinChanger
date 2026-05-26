using UnityEngine;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Agrupa spawns de nueva ronda (4 jugadores) para no encolar 12+ applies de golpe.
    /// </summary>
    public static class SpawnBurstCoordinator
    {
        private const float BurstWindowSeconds = 2.5f;
        private const float PostBurstEnqueueDelay = 0.35f;

        private static float _burstEndTime;
        private static float _postBurstEnqueueTime;
        private static bool _postBurstEnqueuePending;
        private static bool _localApplyUsedInBurst;

        public static bool IsInBurst
        {
            get { return Time.realtimeSinceStartup < _burstEndTime; }
        }

        public static void NotifySpawn()
        {
            _burstEndTime = Time.realtimeSinceStartup + BurstWindowSeconds;
            _postBurstEnqueueTime = _burstEndTime + PostBurstEnqueueDelay;
            _postBurstEnqueuePending = true;
        }

        public static void NotifyPlayerSpawnedNetwork()
        {
            NotifySpawn();
        }

        public static bool TryScheduleLocalApplyOnce()
        {
            if (IsInBurst)
            {
                if (_localApplyUsedInBurst)
                {
                    return false;
                }

                _localApplyUsedInBurst = true;
            }

            MatchEntryColorScheduler.ScheduleLocalApplyOnce();
            return true;
        }

        public static void Tick()
        {
            if (!_postBurstEnqueuePending)
            {
                return;
            }

            if (Time.realtimeSinceStartup < _postBurstEnqueueTime)
            {
                return;
            }

            _postBurstEnqueuePending = false;
            _localApplyUsedInBurst = false;
            SteamLobbyColorSync.RefreshFromLobby(false, mergeOnly: true);
            MatchEntryColorScheduler.EnqueueMissingRemoteWithMod();
        }

        public static void Clear()
        {
            _burstEndTime = 0f;
            _postBurstEnqueueTime = 0f;
            _postBurstEnqueuePending = false;
            _localApplyUsedInBurst = false;
        }
    }
}
