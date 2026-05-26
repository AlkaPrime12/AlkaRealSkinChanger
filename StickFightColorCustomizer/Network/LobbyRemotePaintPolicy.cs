using Steamworks;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;

namespace StickFightColorCustomizer.Network
{
    public static class LobbyRemotePaintPolicy
    {
        public static void OnEnteredLobby()
        {
            NetworkPlayerIndexResolver.RebuildSlotMap();
            PlayerSlotCache.Invalidate();
            MatchEntryColorScheduler.ResetLobbyPaintState();
        }

        public static bool ShouldEnqueueRemotePreview(ulong steamId)
        {
            if (steamId == 0 || !SteamReadyHelper.IsReady())
            {
                return false;
            }

            ulong localSteam;
            if (!SteamReadyHelper.TryGetLocalSteamId(out localSteam) || steamId == localSteam)
            {
                return false;
            }

            return RemoteApplyGate.HasConfirmedModColors(steamId);
        }

        public static bool CanApplyRemoteController(Controller controller, out BodyColors remoteColors)
        {
            ulong peerSteam;
            return RemoteApplyGate.CanApplyRemoteBody(controller, out remoteColors, out peerSteam);
        }
    }
}
