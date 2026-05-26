using UnityEngine;

namespace StickFightColorCustomizer.Models
{
    public sealed class BodyColors
    {
        public Color Head = Color.white;
        public Color Spine = Color.white;
        public Color LegLeft = Color.white;
        public Color LegRight = Color.white;
        public Color HandLeft = Color.white;
        public Color HandRight = Color.white;
        public Color Crown = new Color(1f, 0.84f, 0f);
        public Color Wings = Color.white;

        public bool HalfColorEnabled;
        public Color SpineDistal = Color.white;
        public Color LegLeftDistal = Color.white;
        public Color LegRightDistal = Color.white;
        public Color HandLeftDistal = Color.white;
        public Color HandRightDistal = Color.white;

        public Color Get(StickColorPart part)
        {
            switch (part)
            {
                case StickColorPart.Head: return Head;
                case StickColorPart.Spine: return Spine;
                case StickColorPart.LegLeft: return LegLeft;
                case StickColorPart.LegRight: return LegRight;
                case StickColorPart.HandLeft: return HandLeft;
                case StickColorPart.HandRight: return HandRight;
                case StickColorPart.Crown: return Crown;
                case StickColorPart.Wings: return Wings;
                default: return Color.white;
            }
        }

        public void Set(StickColorPart part, Color color)
        {
            switch (part)
            {
                case StickColorPart.Head: Head = color; break;
                case StickColorPart.Spine: Spine = color; break;
                case StickColorPart.LegLeft: LegLeft = color; break;
                case StickColorPart.LegRight: LegRight = color; break;
                case StickColorPart.HandLeft: HandLeft = color; break;
                case StickColorPart.HandRight: HandRight = color; break;
                case StickColorPart.Crown: Crown = color; break;
                case StickColorPart.Wings: Wings = color; break;
            }
        }

        public bool SupportsHalfColor(StickColorPart part)
        {
            return part == StickColorPart.Spine
                || part == StickColorPart.LegLeft
                || part == StickColorPart.LegRight
                || part == StickColorPart.HandLeft
                || part == StickColorPart.HandRight;
        }

        public Color GetDistal(StickColorPart part)
        {
            switch (part)
            {
                case StickColorPart.Spine: return SpineDistal;
                case StickColorPart.LegLeft: return LegLeftDistal;
                case StickColorPart.LegRight: return LegRightDistal;
                case StickColorPart.HandLeft: return HandLeftDistal;
                case StickColorPart.HandRight: return HandRightDistal;
                default: return Get(part);
            }
        }

        public void SetDistal(StickColorPart part, Color color)
        {
            switch (part)
            {
                case StickColorPart.Spine: SpineDistal = color; break;
                case StickColorPart.LegLeft: LegLeftDistal = color; break;
                case StickColorPart.LegRight: LegRightDistal = color; break;
                case StickColorPart.HandLeft: HandLeftDistal = color; break;
                case StickColorPart.HandRight: HandRightDistal = color; break;
            }
        }

        public Color GetBodyUniformColor()
        {
            return Color.Lerp(
                Color.Lerp(Head, Spine, 0.5f),
                Color.Lerp(LegLeft, LegRight, 0.5f),
                0.5f);
        }

        public BodyColors Clone()
        {
            return new BodyColors
            {
                Head = Head,
                Spine = Spine,
                LegLeft = LegLeft,
                LegRight = LegRight,
                HandLeft = HandLeft,
                HandRight = HandRight,
                Crown = Crown,
                Wings = Wings,
                HalfColorEnabled = HalfColorEnabled,
                SpineDistal = SpineDistal,
                LegLeftDistal = LegLeftDistal,
                LegRightDistal = LegRightDistal,
                HandLeftDistal = HandLeftDistal,
                HandRightDistal = HandRightDistal
            };
        }
    }
}
