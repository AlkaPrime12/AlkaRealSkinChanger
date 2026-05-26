using System.Collections.Generic;
using Steamworks;
using StickFightColorCustomizer.Network;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class LocalPlayerResolver
    {
        private static int? _cachedLocalPlayerId;
        private static bool _cacheValid;

        public static void InvalidateCache()
        {
            _cacheValid = false;
            _cachedLocalPlayerId = null;
        }

        public static void RefreshCache(Controller controller)
        {
            if (controller == null || controller.isAI)
            {
                return;
            }

            _cachedLocalPlayerId = controller.playerID;
            _cacheValid = true;
        }

        private static void EnsureNetworkCache()
        {
            if (_cacheValid || !MatchmakingHandler.IsNetworkMatch || !SteamReadyHelper.IsReady())
            {
                return;
            }

            CSteamID localSteam;
            if (SteamReadyHelper.TryGetLocalSteamId(out localSteam))
            {
                byte? localSlot = NetworkPlayerIndexResolver.FindSlotForSteamId(localSteam);
                if (localSlot.HasValue)
                {
                    _cachedLocalPlayerId = localSlot.Value;
                    _cacheValid = true;
                }
            }
        }

        /// <summary>
        /// Comprobación O(1) para spawn/patches (sin recursión con IsLocalController).
        /// </summary>
        public static bool IsLocalPlayerFast(Controller controller)
        {
            if (controller == null || controller.isAI)
            {
                return false;
            }

            if (MatchmakingHandler.IsNetworkMatch)
            {
                return IsLocalPlayerInNetworkMatch(controller);
            }

            if (controller.HasControl)
            {
                RefreshCache(controller);
                return true;
            }

            EnsureNetworkCache();
            if (_cacheValid && _cachedLocalPlayerId.HasValue
                && controller.playerID == _cachedLocalPlayerId.Value)
            {
                return true;
            }

            if (IsOnlyHumanPlayer(controller))
            {
                RefreshCache(controller);
                return true;
            }

            return false;
        }

        /// <summary>
        /// En MP solo Steam ID / slot de red; nunca HasControl ni "único humano" (evita pintar al host como local).
        /// </summary>
        private static bool IsLocalPlayerInNetworkMatch(Controller controller)
        {
            if (!SteamReadyHelper.IsReady())
            {
                return false;
            }

            ulong localSteam;
            if (!SteamReadyHelper.TryGetLocalSteamId(out localSteam))
            {
                return false;
            }

            ulong steamId;
            if (PlayerSlotCache.TryGetSteamId(controller.playerID, out steamId) && steamId != 0)
            {
                if (steamId == localSteam)
                {
                    RefreshCache(controller);
                    return true;
                }

                return false;
            }

            // Sin Steam ID resuelto: no asumir local por slot (evita pintar randoms con tu skin).
            return false;
        }

        private static bool IsOnlyHumanPlayer(Controller controller)
        {
            if (ControllerHandler.Instance == null)
            {
                return false;
            }

            List<Controller> active = ControllerHandler.Instance.ActivePlayers;
            if (active == null)
            {
                return false;
            }

            Controller onlyHuman = null;
            int humanCount = 0;
            for (int i = 0; i < active.Count; i++)
            {
                Controller c = active[i];
                if (c == null || c.isAI)
                {
                    continue;
                }

                humanCount++;
                onlyHuman = c;
            }

            return humanCount == 1 && onlyHuman == controller;
        }

        public static Controller GetLocalController()
        {
            if (ControllerHandler.Instance == null)
            {
                return null;
            }

            List<Controller> active = ControllerHandler.Instance.ActivePlayers;
            if (active == null || active.Count == 0)
            {
                return null;
            }

            if (MatchmakingHandler.IsNetworkMatch)
            {
                if (!SteamReadyHelper.IsReady())
                {
                    return null;
                }

                ulong localSteam;
                if (!SteamReadyHelper.TryGetLocalSteamId(out localSteam))
                {
                    return null;
                }

                for (int i = 0; i < active.Count; i++)
                {
                    Controller controller = active[i];
                    if (controller == null)
                    {
                        continue;
                    }

                    ulong steamId;
                    if (PlayerSlotCache.TryGetSteamId(controller.playerID, out steamId)
                        && steamId == localSteam)
                    {
                        RefreshCache(controller);
                        return controller;
                    }
                }

                CSteamID localCSteam;
                if (SteamReadyHelper.TryGetLocalSteamId(out localCSteam))
                {
                    byte? localSlot = NetworkPlayerIndexResolver.FindSlotForSteamId(localCSteam);
                    if (localSlot.HasValue)
                    {
                        for (int i = 0; i < active.Count; i++)
                        {
                            Controller controller = active[i];
                            if (controller != null && controller.playerID == localSlot.Value)
                            {
                                RefreshCache(controller);
                                return controller;
                            }
                        }
                    }
                }

                return null;
            }

            EnsureNetworkCache();
            if (_cacheValid && _cachedLocalPlayerId.HasValue)
            {
                for (int i = 0; i < active.Count; i++)
                {
                    Controller controller = active[i];
                    if (controller != null && controller.playerID == _cachedLocalPlayerId.Value)
                    {
                        return controller;
                    }
                }
            }

            for (int i = 0; i < active.Count; i++)
            {
                Controller controller = active[i];
                if (controller != null && IsLocalPlayerFast(controller))
                {
                    return controller;
                }
            }

            return null;
        }

        public static bool IsLocalController(Controller controller)
        {
            return IsLocalPlayerFast(controller);
        }
    }
}
