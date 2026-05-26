using Steamworks;
using StickFightColorCustomizer.Hosting;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Network
{
    public static class SteamLobbyColorSync
    {
        private static float _lastLobbyRefresh;
        private static int _lastPublishedPayloadHash;
        private static int _lastPublishedHatHash;
        private static int _lastPublishedShoeHash;
        private static int _lastPublishedObjectHash;
        private const float LobbyRefreshCooldown = 1.5f;
        private const float LobbyRefreshCooldownBusy = 3f;

        public static bool CanSyncLobbyColors()
        {
            if (!SteamReadyHelper.IsReady() || MatchmakingHandler.Instance == null)
            {
                return false;
            }

            if (MatchmakingHandler.Instance.IsInsideLobby)
            {
                return true;
            }

            return MatchmakingHandler.IsNetworkMatch && GetCurrentLobby().IsValid();
        }

        public static CSteamID GetCurrentLobby()
        {
            if (MatchmakingHandler.Instance == null)
            {
                return CSteamID.Nil;
            }

            return MatchmakingHandler.Instance.CurrentLobby;
        }

        public static void PublishLocal(BodyColors colors)
        {
            if (colors == null || !SteamReadyHelper.IsReady() || !CanSyncLobbyColors())
            {
                return;
            }

            if (ColorCustomizerApp.Instance != null
                && !ModFeatureGate.IsBodyActive(ColorCustomizerApp.Instance.Config))
            {
                return;
            }

            CSteamID lobby = GetCurrentLobby();
            if (!lobby.IsValid())
            {
                return;
            }

            string encoded = ColorSyncCodec.Encode(colors);
            int hash = encoded != null ? encoded.GetHashCode() : 0;
            if (hash != 0 && hash == _lastPublishedPayloadHash)
            {
                return;
            }

            _lastPublishedPayloadHash = hash;
            SteamApiGuard.SetLobbyMemberData(lobby, ColorSyncCodec.LobbyMemberKey, encoded);
            SteamApiGuard.SetLobbyMemberData(lobby, ColorSyncCodec.LobbyPresenceKey, "1");
            SteamApiGuard.SetLobbyMemberData(lobby, ColorSyncCodec.LobbyVersionKey, ColorSyncCodec.ProtocolVersion);
            ulong localSteam;
            CSteamID localCSteam;
            if (!SteamReadyHelper.TryGetLocalSteamId(out localSteam) || !SteamReadyHelper.TryGetLocalSteamId(out localCSteam))
            {
                return;
            }

            ModPresenceRegistry.Mark(localSteam);
            RemoteColorRegistry.SetBySteamEncoded(localSteam, encoded, colors);
            PublishLocalHatFromConfig();
            PublishLocalShoeFromConfig();
            PublishLocalObjectFromConfig();

            byte? slot = NetworkPlayerIndexResolver.FindSlotForSteamId(localCSteam);
            if (slot.HasValue)
            {
                RemoteColorRegistry.Set(slot.Value, colors);
            }
        }

        public static void RefreshFromLobby(bool force = false, bool mergeOnly = false)
        {
            if (!CanSyncLobbyColors())
            {
                return;
            }

            float cooldown = LobbyRefreshCooldown;
            if (LobbyPerformance.IsLobbyMenuOnly())
            {
                CSteamID lobbyCheck = GetCurrentLobby();
                if (lobbyCheck.IsValid() && SteamApiGuard.GetNumLobbyMembers(lobbyCheck) >= 3)
                {
                    cooldown = LobbyRefreshCooldownBusy;
                }
            }

            if (!force && Time.realtimeSinceStartup - _lastLobbyRefresh < cooldown)
            {
                return;
            }

            _lastLobbyRefresh = Time.realtimeSinceStartup;

            if (force)
            {
                NetworkPlayerIndexResolver.RebuildSlotMap();
                if (!mergeOnly)
                {
                    PlayerSlotCache.InvalidateModPresence();
                }
                else
                {
                    RemoteColorRegistry.SyncModPresenceFromCache();
                }
            }

            CSteamID lobby = GetCurrentLobby();
            if (!lobby.IsValid())
            {
                if (mergeOnly)
                {
                    RemoteColorRegistry.SyncModPresenceFromCache();
                }

                return;
            }

            int members = SteamApiGuard.GetNumLobbyMembers(lobby);
            for (int i = 0; i < members; i++)
            {
                CSteamID member = SteamApiGuard.GetLobbyMemberByIndex(lobby, i);
                if (!member.IsValid())
                {
                    continue;
                }

                ulong steamId = member.m_SteamID;
                string raw = SteamApiGuard.GetLobbyMemberData(lobby, member, ColorSyncCodec.LobbyMemberKey);
                string presenceOk = SteamApiGuard.GetLobbyMemberData(lobby, member, ColorSyncCodec.LobbyPresenceKey);
                BodyColors colors;
                if (!ColorSyncCodec.TryDecode(raw, out colors))
                {
                    if (presenceOk == "1")
                    {
                        ModPresenceRegistry.MarkPending(steamId);
                        MatchEntryColorScheduler.ClearKnownNoMod(steamId);
                        continue;
                    }

                    if (!mergeOnly && !ModPresenceRegistry.IsPending(steamId))
                    {
                        RemoteColorRegistry.ClearPeerVanilla(steamId);
                        MatchEntryColorScheduler.MarkKnownNoMod(steamId);
                        PeerModLogger.LogVanillaOnce(steamId);
                    }

                    continue;
                }

            ModPresenceRegistry.Mark(steamId);
            MatchEntryColorScheduler.ClearKnownNoMod(steamId);
            RemoteColorRegistry.SetBySteamEncoded(steamId, raw, colors);
            PeerModLogger.LogModOnce(steamId);
            ReadMemberHat(lobby, member, steamId);
            ReadMemberShoe(lobby, member, steamId);
            ReadMemberObject(lobby, member, steamId);

                byte slot;
                if (NetworkPlayerIndexResolver.TryGetSlot(steamId, out slot))
                {
                    RemoteColorRegistry.Set(slot, colors);
                    PlayerSlotCache.InvalidatePlayer(slot);
                }
            }

            if (mergeOnly)
            {
                RemoteColorRegistry.SyncModPresenceFromCache();
            }
        }

        /// <summary>
        /// Aplica al local. Devuelve false si todavía no hay controller local (para reintento por scheduler).
        /// </summary>
        public static bool ApplyLocalPlayerOnly()
        {
            if (ColorCustomizerApp.Instance == null)
            {
                return true;
            }

            Controller local = LocalPlayerResolver.GetLocalController();
            if (local == null)
            {
                return false;
            }

            ColorCustomizerApp.Instance.TryApplyToController(local);
            ColorCustomizerApp.Instance.ApplyGlowOnly();
            ColorCustomizerApp.Instance.ApplyShoeOnly();
            return true;
        }

        public static void ApplyControllerIfRemote(Controller controller)
        {
            if (ColorCustomizerApp.Instance == null || controller == null)
            {
                return;
            }

            if (LocalPlayerResolver.IsLocalController(controller))
            {
                MatchEntryColorScheduler.ScheduleLocalApplyOnce();
                return;
            }

            MatchEntryColorScheduler.EnqueueRemote(controller);
        }

        public static void PublishLocalHat()
        {
            if (ColorCustomizerApp.Instance == null)
            {
                return;
            }

            PublishLocalHatFromConfig();
        }

        public static void PublishLocalHatFromConfig()
        {
            if (!SteamReadyHelper.IsReady() || !CanSyncLobbyColors())
            {
                return;
            }

            ColorConfig config = ColorCustomizerApp.Instance.Config;
            if (config == null || config.Hat == null || !ModFeatureGate.IsHatActive(config))
            {
                PublishLocalHatValue("none");
                return;
            }

            PublishLocalHatValue(config.Hat.HatId);
        }

        private static void PublishLocalHatValue(string hatId)
        {
            CSteamID lobby = GetCurrentLobby();
            if (!lobby.IsValid())
            {
                return;
            }

            string encoded = HatSyncCodec.Encode(hatId);
            int hash = encoded != null ? encoded.GetHashCode() : 0;
            if (hash != 0 && hash == _lastPublishedHatHash)
            {
                return;
            }

            _lastPublishedHatHash = hash;
            SteamApiGuard.SetLobbyMemberData(lobby, HatSyncCodec.LobbyMemberKey, encoded);
            ulong localSteam;
            CSteamID localCSteam;
            if (!SteamReadyHelper.TryGetLocalSteamId(out localSteam) || !SteamReadyHelper.TryGetLocalSteamId(out localCSteam))
            {
                return;
            }

            RemoteHatRegistry.SetBySteam(localSteam, hatId);

            byte? slot = NetworkPlayerIndexResolver.FindSlotForSteamId(localCSteam);
            if (slot.HasValue)
            {
                RemoteHatRegistry.Set(slot.Value, hatId);
            }
        }

        private static void ReadMemberHat(CSteamID lobby, CSteamID member, ulong steamId)
        {
            string raw = SteamApiGuard.GetLobbyMemberData(lobby, member, HatSyncCodec.LobbyMemberKey);
            if (string.IsNullOrEmpty(raw))
            {
                RemoteHatRegistry.RemoveSteam(steamId);
                return;
            }

            RemoteHatRegistry.SetBySteam(steamId, HatSyncCodec.Decode(raw));
            byte slot;
            if (NetworkPlayerIndexResolver.TryGetSlot(steamId, out slot))
            {
                RemoteHatRegistry.Set(slot, HatSyncCodec.Decode(raw));
            }
        }

        public static void PublishLocalShoe()
        {
            if (ColorCustomizerApp.Instance == null)
            {
                return;
            }

            PublishLocalShoeFromConfig();
        }

        public static void PublishLocalShoeFromConfig()
        {
            if (!SteamReadyHelper.IsReady() || !CanSyncLobbyColors())
            {
                return;
            }

            ColorConfig config = ColorCustomizerApp.Instance.Config;
            if (config == null || config.Shoe == null || !ModFeatureGate.IsShoeActive(config))
            {
                PublishLocalShoeValue("none");
                return;
            }

            PublishLocalShoeValue(config.Shoe.ShoeId);
        }

        private static void PublishLocalShoeValue(string shoeId)
        {
            CSteamID lobby = GetCurrentLobby();
            if (!lobby.IsValid())
            {
                return;
            }

            string encoded = ShoeSyncCodec.Encode(shoeId);
            int hash = encoded != null ? encoded.GetHashCode() : 0;
            if (hash != 0 && hash == _lastPublishedShoeHash)
            {
                return;
            }

            _lastPublishedShoeHash = hash;
            SteamApiGuard.SetLobbyMemberData(lobby, ShoeSyncCodec.LobbyMemberKey, encoded);
            ulong localSteam;
            CSteamID localCSteam;
            if (!SteamReadyHelper.TryGetLocalSteamId(out localSteam) || !SteamReadyHelper.TryGetLocalSteamId(out localCSteam))
            {
                return;
            }

            RemoteShoeRegistry.SetBySteam(localSteam, shoeId);

            byte? slot = NetworkPlayerIndexResolver.FindSlotForSteamId(localCSteam);
            if (slot.HasValue)
            {
                RemoteShoeRegistry.Set(slot.Value, shoeId);
            }
        }

        private static void ReadMemberShoe(CSteamID lobby, CSteamID member, ulong steamId)
        {
            string raw = SteamApiGuard.GetLobbyMemberData(lobby, member, ShoeSyncCodec.LobbyMemberKey);
            if (string.IsNullOrEmpty(raw))
            {
                RemoteShoeRegistry.RemoveSteam(steamId);
                return;
            }

            RemoteShoeRegistry.SetBySteam(steamId, ShoeSyncCodec.Decode(raw));
            byte slot;
            if (NetworkPlayerIndexResolver.TryGetSlot(steamId, out slot))
            {
                RemoteShoeRegistry.Set(slot, ShoeSyncCodec.Decode(raw));
            }
        }

        public static void PublishLocalObject()
        {
            if (ColorCustomizerApp.Instance == null)
            {
                return;
            }

            PublishLocalObjectFromConfig();
        }

        public static void PublishLocalObjectFromConfig()
        {
            if (!SteamReadyHelper.IsReady() || !CanSyncLobbyColors())
            {
                return;
            }

            ColorConfig config = ColorCustomizerApp.Instance.Config;
            if (config == null || config.Object == null || !ModFeatureGate.IsObjectsActive(config))
            {
                PublishLocalObjectValue("none");
                return;
            }

            PublishLocalObjectValue(config.Object.ObjectId);
        }

        private static void PublishLocalObjectValue(string objectId)
        {
            CSteamID lobby = GetCurrentLobby();
            if (!lobby.IsValid())
            {
                return;
            }

            string encoded = ObjectSyncCodec.Encode(objectId);
            int hash = encoded != null ? encoded.GetHashCode() : 0;
            if (hash != 0 && hash == _lastPublishedObjectHash)
            {
                return;
            }

            _lastPublishedObjectHash = hash;
            SteamApiGuard.SetLobbyMemberData(lobby, ObjectSyncCodec.LobbyMemberKey, encoded);
            ulong localSteam;
            CSteamID localCSteam;
            if (!SteamReadyHelper.TryGetLocalSteamId(out localSteam) || !SteamReadyHelper.TryGetLocalSteamId(out localCSteam))
            {
                return;
            }

            RemoteObjectRegistry.SetBySteam(localSteam, objectId);

            byte? slot = NetworkPlayerIndexResolver.FindSlotForSteamId(localCSteam);
            if (slot.HasValue)
            {
                RemoteObjectRegistry.Set(slot.Value, objectId);
            }
        }

        private static void ReadMemberObject(CSteamID lobby, CSteamID member, ulong steamId)
        {
            string raw = SteamApiGuard.GetLobbyMemberData(lobby, member, ObjectSyncCodec.LobbyMemberKey);
            if (string.IsNullOrEmpty(raw))
            {
                RemoteObjectRegistry.RemoveSteam(steamId);
                return;
            }

            RemoteObjectRegistry.SetBySteam(steamId, ObjectSyncCodec.Decode(raw));
            byte slot;
            if (NetworkPlayerIndexResolver.TryGetSlot(steamId, out slot))
            {
                RemoteObjectRegistry.Set(slot, ObjectSyncCodec.Decode(raw));
            }
        }

        public static void ResetPublishCache()
        {
            _lastPublishedPayloadHash = 0;
            _lastPublishedHatHash = 0;
            _lastPublishedShoeHash = 0;
            _lastPublishedObjectHash = 0;
        }
    }
}
