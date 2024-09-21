using Microsoft.Xna.Framework;

namespace TimescaleDisplay
{
    public interface IReadOnlyConfigurationData
    {
        bool DisplayEnabled { get; }

        int RenderOffsetX { get; }

        int RenderOffsetY { get; }

        float Scale { get; }

        bool ShowBackground { get; }

        public Color Color { get; }

        public Color ColorBackground { get; }

        bool DrawShadow { get; }
    }
}