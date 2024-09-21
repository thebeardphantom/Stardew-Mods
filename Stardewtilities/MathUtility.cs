namespace BeardPhantom.Stardewtilities
{
    public static class MathUtility
    {
        public static float Lerp(float x, float y, float t)
        {
            return x * (1 - t) + y * t;
        }
    }
}