using StickFightColorCustomizer.Core;

using StickFightColorCustomizer.Models;

using UnityEngine;



namespace StickFightColorCustomizer.Network

{

    public static class ColorSyncCodec

    {

        public const string LobbyMemberKey = "sfcc";

        public const string LobbyPresenceKey = "sfcc_ok";

        public const string LobbyVersionKey = "sfcc_ver";

        public const string ProtocolVersion = "230";

        public const int PingMagic = 0x53464343;

        public const int PingP2PChannel = 0;



        public static string Encode(BodyColors colors)

        {

            if (colors == null)

            {

                return string.Empty;

            }



            return "3|" + ColorUtil.ToHex(colors.Head) + "|"

                + ColorUtil.ToHex(colors.Spine) + "|"

                + ColorUtil.ToHex(colors.LegLeft) + "|"

                + ColorUtil.ToHex(colors.LegRight) + "|"

                + ColorUtil.ToHex(colors.HandLeft) + "|"

                + ColorUtil.ToHex(colors.HandRight) + "|"

                + ColorUtil.ToHex(colors.Crown) + "|"

                + ColorUtil.ToHex(colors.Wings) + "|"

                + (colors.HalfColorEnabled ? "1" : "0");

        }



        public static bool TryDecode(string raw, out BodyColors colors)

        {

            colors = new BodyColors();

            if (string.IsNullOrEmpty(raw))

            {

                return false;

            }



            string[] parts = raw.Split('|');

            if (parts.Length >= 2 && parts[0] == "3")

            {

                return TryDecodeV3(parts, out colors);

            }



            if (parts.Length >= 2 && parts[0] == "2")

            {

                return TryDecodeV2(parts, out colors);

            }



            if (parts.Length == 4)

            {

                return TryDecodeLegacy(parts, out colors);

            }



            return false;

        }



        private static bool TryDecodeV3(string[] parts, out BodyColors colors)

        {

            colors = new BodyColors();

            if (parts.Length < 10)

            {

                return false;

            }



            Color parsed;

            if (!TryPart(parts[1], out parsed)) return false;

            colors.Head = parsed;

            if (!TryPart(parts[2], out parsed)) return false;

            colors.Spine = parsed;

            if (!TryPart(parts[3], out parsed)) return false;

            colors.LegLeft = parsed;

            if (!TryPart(parts[4], out parsed)) return false;

            colors.LegRight = parsed;

            if (!TryPart(parts[5], out parsed)) return false;

            colors.HandLeft = parsed;

            if (!TryPart(parts[6], out parsed)) return false;

            colors.HandRight = parsed;

            if (!TryPart(parts[7], out parsed)) return false;

            colors.Crown = parsed;

            if (!TryPart(parts[8], out parsed)) return false;

            colors.Wings = parsed;

            colors.HalfColorEnabled = parts[9] == "1";

            return true;

        }



        private static bool TryDecodeV2(string[] parts, out BodyColors colors)

        {

            colors = new BodyColors();

            if (parts.Length < 9)

            {

                return false;

            }



            Color parsed;

            if (!TryPart(parts[1], out parsed)) return false;

            colors.Head = parsed;

            if (!TryPart(parts[2], out parsed)) return false;

            colors.Spine = parsed;

            if (!TryPart(parts[3], out parsed)) return false;

            colors.LegLeft = parsed;

            if (!TryPart(parts[4], out parsed)) return false;

            colors.LegRight = parsed;

            if (!TryPart(parts[5], out parsed)) return false;

            colors.HandLeft = parsed;

            if (!TryPart(parts[6], out parsed)) return false;

            colors.HandRight = parsed;

            if (!TryPart(parts[7], out parsed)) return false;

            colors.Crown = parsed;

            if (!TryPart(parts[8], out parsed)) return false;

            colors.Wings = parsed;

            return true;

        }



        private static bool TryDecodeLegacy(string[] parts, out BodyColors colors)

        {

            colors = new BodyColors();

            Color parsed;

            if (!TryPart(parts[0], out parsed)) return false;

            colors.Head = parsed;

            if (!TryPart(parts[1], out parsed)) return false;

            colors.Spine = parsed;

            if (!TryPart(parts[2], out parsed)) return false;

            colors.LegLeft = parsed;

            colors.LegRight = parsed;

            if (!TryPart(parts[3], out parsed)) return false;

            colors.HandLeft = parsed;

            colors.HandRight = parsed;

            return true;

        }



        private static bool TryPart(string hex, out Color color)

        {

            return ColorUtil.TryParseHex(hex, out color);

        }

    }

}


