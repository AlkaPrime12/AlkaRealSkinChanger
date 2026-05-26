using StickFightColorCustomizer.Models;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class ColorPresets
    {
        public static readonly string[] Names = { "Fire", "Ice", "RGB", "Shadow", "Neon", "Royal" };

        public static void ApplyBodyOnly(ColorConfig config, string name)
        {
            BodyColors body = GetBody(name);
            config.Colors = body;
            config.ActivePreset = name;
            config.AnimatedRgb = name == "RGB";
        }

        public static BodyColors GetBody(string name)
        {
            switch (name)
            {
                case "Fire":
                    return MakeBody(
                        new Color(1f, 0.45f, 0.1f),
                        new Color(1f, 0.2f, 0.05f),
                        new Color(0.9f, 0.1f, 0.05f),
                        new Color(0.9f, 0.1f, 0.05f),
                        new Color(1f, 0.7f, 0.2f),
                        new Color(1f, 0.7f, 0.2f),
                        new Color(1f, 0.85f, 0.2f));
                case "Ice":
                    return MakeBody(
                        new Color(0.75f, 0.95f, 1f),
                        new Color(0.5f, 0.85f, 1f),
                        new Color(0.35f, 0.7f, 0.95f),
                        new Color(0.35f, 0.7f, 0.95f),
                        new Color(0.9f, 1f, 1f),
                        new Color(0.9f, 1f, 1f),
                        new Color(0.7f, 0.9f, 1f));
                case "RGB":
                    return MakeBody(Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.white);
                case "Shadow":
                    return MakeBody(
                        new Color(0.15f, 0.15f, 0.2f),
                        new Color(0.08f, 0.08f, 0.12f),
                        new Color(0.05f, 0.05f, 0.08f),
                        new Color(0.05f, 0.05f, 0.08f),
                        new Color(0.25f, 0.25f, 0.3f),
                        new Color(0.25f, 0.25f, 0.3f),
                        new Color(0.35f, 0.3f, 0.1f));
                case "Neon":
                    return MakeBody(
                        new Color(0f, 1f, 0.9f),
                        new Color(1f, 0f, 0.8f),
                        new Color(0.2f, 1f, 0.2f),
                        new Color(0.2f, 1f, 0.2f),
                        new Color(1f, 1f, 0.2f),
                        new Color(1f, 1f, 0.2f),
                        new Color(1f, 0.9f, 0.2f));
                case "Royal":
                    return MakeBody(
                        new Color(0.95f, 0.78f, 0.2f),
                        new Color(0.55f, 0.2f, 0.85f),
                        new Color(0.45f, 0.15f, 0.75f),
                        new Color(0.45f, 0.15f, 0.75f),
                        new Color(0.95f, 0.78f, 0.2f),
                        new Color(0.95f, 0.78f, 0.2f),
                        new Color(1f, 0.84f, 0f));
                default:
                    return new BodyColors();
            }
        }

        public static GlowSettings GetGlowPreset(string name)
        {
            GlowSettings g = new GlowSettings();
            Color color = Color.white;
            float strength = 0.5f;
            switch (name)
            {
                case "Fire":
                    color = new Color(1f, 0.5f, 0.15f);
                    strength = 0.52f;
                    break;
                case "Ice":
                    color = new Color(0.55f, 0.9f, 1f);
                    strength = 0.45f;
                    break;
                case "Neon":
                    color = new Color(0.25f, 1f, 0.95f);
                    strength = 0.55f;
                    break;
                case "Royal":
                    color = new Color(1f, 0.9f, 0.45f);
                    strength = 0.5f;
                    break;
                case "Shadow":
                    color = new Color(0.45f, 0.35f, 0.65f);
                    strength = 0.42f;
                    break;
                default:
                    strength = 0.48f;
                    break;
            }

            GlowPresetTuning.ApplySoftPreset(g, color, strength);
            return g;
        }

        private static BodyColors MakeBody(
            Color head,
            Color spine,
            Color legL,
            Color legR,
            Color handL,
            Color handR,
            Color crown)
        {
            return new BodyColors
            {
                Head = head,
                Spine = spine,
                LegLeft = legL,
                LegRight = legR,
                HandLeft = handL,
                HandRight = handR,
                Crown = crown,
                Wings = spine
            };
        }
    }
}
