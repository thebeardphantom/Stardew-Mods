using Microsoft.Xna.Framework;

namespace BeardPhantom.Stardewtilities
{
    public static class ColorUtility
    {
        public static Color GetPremultiplied(this Color color)
        {
            const float MaxValue = byte.MaxValue;
            float alphaFloat = color.A / MaxValue;
            float rFloat = color.R / MaxValue;
            float gFloat = color.G / MaxValue;
            float bFloat = color.B / MaxValue;
            return new Color(rFloat * alphaFloat, gFloat * alphaFloat, bFloat * alphaFloat, alphaFloat);
        }
    }
}