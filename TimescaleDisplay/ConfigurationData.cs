using Microsoft.Xna.Framework;

namespace TimescaleDisplay
{
    [Serializable]
    public class ConfigurationData : IReadOnlyConfigurationData
    {
        public const float ScaleMin = 0.5f;

        public const float ScaleMax = 3f;

        private const float DefaultAlpha = 0.75f;

        public bool DisplayEnabled { get; set; }

        public int RenderOffsetX { get; set; }

        public int RenderOffsetY { get; set; }

        public float Scale { get; set; }

        public bool ShowBackground { get; set; }

        public Color Color { get; set; }

        public Color ColorBackground { get; set; }

        public bool DrawShadow { get; set; }

        public void OverwriteFrom(ConfigurationData other)
        {
            DisplayEnabled = other.DisplayEnabled;
            RenderOffsetX = other.RenderOffsetX;
            RenderOffsetY = other.RenderOffsetY;
            Scale = other.Scale;
            ShowBackground = other.ShowBackground;
            Color = other.Color;
            ColorBackground = other.ColorBackground;
            DrawShadow = other.DrawShadow;
        }

        public ConfigurationData Reset()
        {
            DisplayEnabled = true;
            RenderOffsetX = 224;
            RenderOffsetY = 90;
            Scale = 1.333f;
            ShowBackground = true;
            Color = new Color(Color.White, DefaultAlpha);
            ColorBackground = new Color(Color.Black, DefaultAlpha / 2f);
            DrawShadow = true;
            return this;
        }

        public bool ResetIfInvalid()
        {
            if (Scale is < ScaleMin or > ScaleMax)
            {
                Reset();
                return true;
            }

            return false;
        }
    }
}