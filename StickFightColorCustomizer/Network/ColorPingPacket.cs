using System.IO;
using System.Text;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;
using Steamworks;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// Piggyback en paquetes Ping/PingResponse (vanilla solo lee int32; bytes extra no rompen el juego).
    /// </summary>
    public static class ColorPingPacket
    {
        private static readonly System.Collections.Generic.Dictionary<ulong, float> LastReplyPerPeer =
            new System.Collections.Generic.Dictionary<ulong, float>();
        private const float ReplyCooldownSeconds = 3f;

        public static byte[] Build(string encodedColors)
        {
            if (string.IsNullOrEmpty(encodedColors))
            {
                return null;
            }

            byte[] text = Encoding.UTF8.GetBytes(encodedColors);
            byte[] buffer = new byte[8 + text.Length];
            using (MemoryStream output = new MemoryStream(buffer))
            {
                using (BinaryWriter writer = new BinaryWriter(output))
                {
                    writer.Write(Random.Range(int.MinValue, int.MaxValue));
                    writer.Write(ColorSyncCodec.PingMagic);
                    writer.Write(text);
                }
            }

            return buffer;
        }

        public static void TryConsume(ulong steamId, byte[] data)
        {
            if (data == null || data.Length < 8 || steamId == 0)
            {
                return;
            }

            int offset = 0;
            ReadInt32(data, ref offset);
            int magic = ReadInt32(data, ref offset);
            if (magic != ColorSyncCodec.PingMagic)
            {
                return;
            }

            int payloadLen = data.Length - offset;
            if (payloadLen <= 0)
            {
                return;
            }

            string encoded = Encoding.UTF8.GetString(data, offset, payloadLen);
            BodyColors colors;
            if (!ColorSyncCodec.TryDecode(encoded, out colors))
            {
                return;
            }

            ModPresenceRegistry.Mark(steamId);
            MatchEntryColorScheduler.ClearKnownNoMod(steamId);
            RemoteColorRegistry.SetBySteamEncoded(steamId, encoded, colors);

            byte? slot = NetworkPlayerIndexResolver.FindSlotForSteamId(new CSteamID(steamId));
            if (slot.HasValue)
            {
                RemoteColorRegistry.Set(slot.Value, colors);
                PlayerSlotCache.InvalidatePlayer(slot.Value);
                MatchEntryColorScheduler.EnqueueRemoteByPlayerId(slot.Value);
            }

            if (MatchmakingHandler.Instance != null && MatchmakingHandler.Instance.IsInsideLobby)
            {
                LobbySyncScheduler.MarkDirty(steamId);
            }

            TryReplyToPeer(steamId);
        }

        /// <summary>
        /// Si otro jugador tiene el mismo DLL, responde su Ping con nuestros colores (vanilla ignora bytes extra).
        /// </summary>
        private static void TryReplyToPeer(ulong steamId)
        {
            if (steamId == 0 || ColorCustomizerApp.Instance == null || ColorCustomizerApp.Instance.Config == null)
            {
                return;
            }

            if (!ModFeatureGate.IsBodyActive(ColorCustomizerApp.Instance.Config))
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            float last;
            if (LastReplyPerPeer.TryGetValue(steamId, out last) && now - last < ReplyCooldownSeconds)
            {
                return;
            }

            BodyColors localColors = ColorCustomizerApp.Instance.Config.Colors;
            if (localColors == null)
            {
                return;
            }

            byte[] packet = Build(ColorSyncCodec.Encode(localColors));
            if (packet == null)
            {
                return;
            }

            LastReplyPerPeer[steamId] = now;
            SendToPeer(new CSteamID(steamId), packet);
        }

        public static void SendToPeer(CSteamID peer, byte[] payload)
        {
            if (!peer.IsValid() || payload == null || payload.Length == 0)
            {
                return;
            }

            byte[] wrapped = WrapPingPacket(payload);
            SteamNetworking.SendP2PPacket(
                peer,
                wrapped,
                (uint)wrapped.Length,
                EP2PSend.k_EP2PSendReliable,
                ColorSyncCodec.PingP2PChannel);
        }

        /// <summary>
        /// Ping dirigido a un peer (entrada al lobby / detección tardía).
        /// </summary>
        public static void RequestPingToPeer(ulong steamId, BodyColors colors)
        {
            if (steamId == 0 || colors == null)
            {
                return;
            }

            CSteamID peer = new CSteamID(steamId);
            CSteamID local;
            if (!peer.IsValid() || !SteamReadyHelper.TryGetLocalSteamId(out local) || peer == local)
            {
                return;
            }

            if (MatchEntryColorScheduler.IsKnownNoMod(steamId) && !ModPresenceRegistry.IsPending(steamId))
            {
                return;
            }

            byte[] packet = Build(ColorSyncCodec.Encode(colors));
            if (packet != null)
            {
                SendToPeer(peer, packet);
            }
        }

        public static void BroadcastToLobby(BodyColors colors)
        {
            ModColorPingSync.BroadcastColorsThrottled(colors, false);
        }

        public static byte[] WrapPingPacket(byte[] payload)
        {
            uint timestamp = SteamUtils.GetServerRealTime();
            byte[] buffer = new byte[payload.Length + 5];
            using (MemoryStream output = new MemoryStream(buffer))
            {
                using (BinaryWriter writer = new BinaryWriter(output))
                {
                    writer.Write(timestamp);
                    writer.Write((byte)P2PPackageHandler.MsgType.Ping);
                    writer.Write(payload);
                }
            }

            return buffer;
        }

        private static int ReadInt32(byte[] data, ref int offset)
        {
            int value = data[offset]
                | (data[offset + 1] << 8)
                | (data[offset + 2] << 16)
                | (data[offset + 3] << 24);
            offset += 4;
            return value;
        }
    }
}
