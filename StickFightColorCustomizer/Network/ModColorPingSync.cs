using System.Collections.Generic;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;
using Steamworks;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Sync mod-a-mod vía piggyback en Ping/PingResponse (inofensivo para clientes vanilla).
    /// </summary>
    public static class ModColorPingSync
    {
        private const float GlobalSendCooldown = 2f;
        private const float PerPeerCooldown = 4f;

        private static readonly Queue<ulong> PendingPeerSteamIds = new Queue<ulong>();
        private static readonly HashSet<ulong> PendingSet = new HashSet<ulong>();
        private static readonly Dictionary<ulong, float> LastSendPerPeer = new Dictionary<ulong, float>();

        private static float _lastGlobalSend;
        private static string _cachedEncodedPayload;
        private static int _cachedPayloadHash;

        public static void OnVanillaPingReceived(ulong steamId, byte[] data)
        {
            if (steamId == 0)
            {
                return;
            }

            ColorPingPacket.TryConsume(steamId, data);
        }

        public static void BroadcastColorsThrottled(BodyColors colors, bool force = false)
        {
            if (colors == null || !SteamReadyHelper.IsReady())
            {
                return;
            }

            if (ColorCustomizerApp.Instance != null)
            {
                if (!ModFeatureGate.NeedsColorPing(ColorCustomizerApp.Instance.Config))
                {
                    return;
                }

                if (!MultiplayerCompat.AllowPingColorPackets(ColorCustomizerApp.Instance.Config))
                {
                    return;
                }
            }

            string encoded = ColorSyncCodec.Encode(colors);
            int hash = encoded != null ? encoded.GetHashCode() : 0;
            if (!force && hash == _cachedPayloadHash)
            {
                return;
            }

            _cachedEncodedPayload = encoded;
            _cachedPayloadHash = hash;

            if (force)
            {
                _lastGlobalSend = 0f;
            }

            PendingPeerSteamIds.Clear();
            PendingSet.Clear();
            EnqueueAllTargets();
        }

        public static void Tick()
        {
            if (!SteamReadyHelper.IsReady())
            {
                return;
            }

            if (ColorCustomizerApp.Instance != null
                && !ModFeatureGate.NeedsColorPing(ColorCustomizerApp.Instance.Config))
            {
                return;
            }

            if (string.IsNullOrEmpty(_cachedEncodedPayload) || PendingPeerSteamIds.Count == 0)
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (now - _lastGlobalSend < GlobalSendCooldown)
            {
                return;
            }

            byte[] packet = ColorPingPacket.Build(_cachedEncodedPayload);
            if (packet == null)
            {
                return;
            }

            int attempts = PendingPeerSteamIds.Count;
            for (int i = 0; i < attempts; i++)
            {
                ulong steamId = PendingPeerSteamIds.Dequeue();
                PendingSet.Remove(steamId);

                float lastPeer;
                if (LastSendPerPeer.TryGetValue(steamId, out lastPeer) && now - lastPeer < PerPeerCooldown)
                {
                    PendingPeerSteamIds.Enqueue(steamId);
                    PendingSet.Add(steamId);
                    continue;
                }

                if (MatchEntryColorScheduler.IsKnownNoMod(steamId) && !ModPresenceRegistry.IsPending(steamId))
                {
                    continue;
                }

                CSteamID peer = new CSteamID(steamId);
                CSteamID local;
                if (!peer.IsValid() || !SteamReadyHelper.TryGetLocalSteamId(out local) || peer == local)
                {
                    continue;
                }

                ColorPingPacket.SendToPeer(peer, packet);
                LastSendPerPeer[steamId] = now;
                _lastGlobalSend = now;
                return;
            }
        }

        public static void RequestPingToPeer(ulong steamId)
        {
            if (ColorCustomizerApp.Instance == null || ColorCustomizerApp.Instance.Config == null)
            {
                return;
            }

            BodyColors colors = ColorCustomizerApp.Instance.Config.Colors;
            if (colors == null || !ModFeatureGate.IsBodyActive(ColorCustomizerApp.Instance.Config))
            {
                return;
            }

            ColorPingPacket.RequestPingToPeer(steamId, colors);

            CSteamID local;
            if (SteamReadyHelper.TryGetLocalSteamId(out local))
            {
                EnqueuePeer(new CSteamID(steamId), local);
            }
        }

        public static void Clear()
        {
            PendingPeerSteamIds.Clear();
            PendingSet.Clear();
            LastSendPerPeer.Clear();
            _cachedEncodedPayload = null;
            _cachedPayloadHash = 0;
            _lastGlobalSend = 0f;
        }

        private static void EnqueueAllTargets()
        {
            CSteamID local;
            if (!SteamReadyHelper.TryGetLocalSteamId(out local))
            {
                return;
            }

            if (MatchmakingHandler.Instance != null)
            {
                CSteamID lobby = MatchmakingHandler.Instance.CurrentLobby;
                if (lobby.IsValid())
                {
                    int members = SteamApiGuard.GetNumLobbyMembers(lobby);
                    for (int i = 0; i < members; i++)
                    {
                        CSteamID member = SteamApiGuard.GetLobbyMemberByIndex(lobby, i);
                        EnqueuePeer(member, local);
                    }
                }
            }

            if (MatchmakingHandler.IsNetworkMatch)
            {
                MultiplayerManager manager = GameNetworkCache.GetMultiplayerManager();
                ConnectedClientData[] clients = GameNetworkCache.GetConnectedClients(manager);
                if (clients != null)
                {
                    for (int i = 0; i < clients.Length; i++)
                    {
                        ConnectedClientData client = clients[i];
                        if (client == null || !client.ClientID.IsValid())
                        {
                            continue;
                        }

                        EnqueuePeer(client.ClientID, local);
                    }
                }
            }
        }

        private static void EnqueuePeer(CSteamID peer, CSteamID local)
        {
            if (!peer.IsValid() || peer == local)
            {
                return;
            }

            ulong steamId = peer.m_SteamID;
            if (PendingSet.Contains(steamId))
            {
                return;
            }

            if (MatchEntryColorScheduler.IsKnownNoMod(steamId) && !ModPresenceRegistry.IsPending(steamId))
            {
                return;
            }

            PendingSet.Add(steamId);
            PendingPeerSteamIds.Enqueue(steamId);
        }
    }
}
