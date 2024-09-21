using Microsoft.Xna.Framework;

namespace BeardPhantom.Stardewtilities
{
    public readonly struct RectAnchor
    {
        public static readonly RectAnchor TopLeft = new(0f, 0f);

        public static readonly RectAnchor Top = new(0.5f, 0f);

        public static readonly RectAnchor TopRight = new(1f, 0f);

        public static readonly RectAnchor Right = new(1f, 0.5f);

        public static readonly RectAnchor BottomRight = new(1f, 1f);

        public static readonly RectAnchor Bottom = new(0.5f, 1f);

        public static readonly RectAnchor BottomLeft = new(0f, 1f);

        public static readonly RectAnchor Left = new(0f, 0.5f);

        public static readonly RectAnchor Center = new(0.5f, 0.5f);

        public readonly float X;

        public readonly float Y;

        public RectAnchor(in float x, in float y)
        {
            X = x;
            Y = y;
        }

        public Point GetAnchorLocation(in Rectangle rectangle)
        {
            int top = rectangle.Top;
            int bottom = rectangle.Bottom;
            int left = rectangle.Left;
            int right = rectangle.Right;
            var x = (int)MathUtility.Lerp(left, right, X);
            var y = (int)MathUtility.Lerp(top, bottom, Y);
            return new Point(x, y);
        }

        public Point GetOffsetLocationForAnchor(in Rectangle rectangle)
        {
            Point location = rectangle.Location;
            Point anchorLocation = GetAnchorLocation(rectangle);
            Point delta = location - anchorLocation;
            return location + delta;
        }

        public Rectangle GetAnchoredRectangle(in Rectangle rectangle)
        {
            Point location = GetOffsetLocationForAnchor(rectangle);
            return new Rectangle(location, rectangle.Size);
        }
    }
}