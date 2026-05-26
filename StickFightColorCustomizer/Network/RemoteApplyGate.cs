using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Única puerta para pintar peers: requiere Steam ID, mod confirmado y colores propios en registro.
    /// </summary>
    public static class RemoteApplyGate
    {
        public static bool TryGetPeerSteamId(Controller controller, out ulong peerSteam)
        {
            peerSteam = 0;
            if (controller == null || LocalPlayerResolver.IsLocalPlayerFast(controller))
            {
                return false;
            }

            if (!PlayerSlotCache.TryGetSteamId(controller.playerID, out peerSteam) || peerSteam == 0)
            {
                return false;
            }

            ulong localSteam;
            if (!SteamReadyHelper.TryGetLocalSteamId(out localSteam) || peerSteam == localSteam)
            {
                return false;
            }

            return true;
        }

        public static bool HasConfirmedModColors(ulong peerSteam)
        {
            if (peerSteam == 0 || !ModPresenceRegistry.HasMod(peerSteam))
            {
                return false;
            }

            BodyColors colors;
            return RemoteColorRegistry.TryGetBySteam(peerSteam, out colors) && colors != null;
        }

        private static bool IsRemoteCosmeticContext()
        {
            if (MatchmakingHandler.IsNetworkMatch)
            {
                return true;
            }

            return MatchmakingHandler.Instance != null && MatchmakingHandler.Instance.IsInsideLobby;
        }

        public static bool CanApplyRemoteBody(Controller controller, out BodyColors colors, out ulong peerSteam)
        {
            colors = null;
            peerSteam = 0;

            if (!IsRemoteCosmeticContext() || controller == null)
            {
                return false;
            }

            if (!TryGetPeerSteamId(controller, out peerSteam))
            {
                return false;
            }

            if (MatchEntryColorScheduler.IsKnownNoMod(peerSteam) && !ModPresenceRegistry.IsPending(peerSteam))
            {
                return false;
            }

            if (!HasConfirmedModColors(peerSteam))
            {
                return false;
            }

            return RemoteColorRegistry.TryGetBySteam(peerSteam, out colors) && colors != null;
        }

        public static bool CanApplyRemoteCosmetics(Controller controller)
        {
            ulong peerSteam;
            BodyColors colors;
            return CanApplyRemoteBody(controller, out colors, out peerSteam);
        }
    }
}
