using Microsoft.Xna.Framework;

namespace BeardPhantom.Stardewtilities
{
    public static class RectangleUtility
    {
        public static Rectangle GetAnchoredRectangle(this Rectangle rectangle, in RectAnchor anchor)
        {
            return anchor.GetAnchoredRectangle(rectangle);
        }

        public static Point GetAnchorLocation(this Rectangle rectangle, in RectAnchor anchor)
        {
            return anchor.GetAnchorLocation(rectangle);
        }

        public static Point GetOffsetLocationForAnchor(this Rectangle rectangle, in RectAnchor anchor)
        {
            return anchor.GetOffsetLocationForAnchor(rectangle);
        }
    }
}