using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TFT_simulator
{
    internal class Util
    {
        public static ushort ToRgb565(Color c)
        {
            int r = c.R;
            int g = c.G;
            int b = c.B;
            return (ushort)(((r & 0xF8) << 8) | ((g & 0xFC) << 3) | (b >> 3));
        }
        public static string ToRgb565String(Color c)
        {
            int r = c.R;
            int g = c.G;
            int b = c.B;
            var color = (ushort)(((r & 0xF8) << 8) | ((g & 0xFC) << 3) | (b >> 3));
            return color.ToString();
        }
        public static Point GetPointOffset(Point A, Point B)
        {
            return new Point(A.X - B.X, A.Y - B.Y);
        }
        // 修正 AddPoints 方法，因為 Point 結構不支援 '+' 運算子
        public static Point AddPoints(Point A, Point B)
        {
            return new Point(A.X + B.X, A.Y + B.Y);
        }
        public static TftElement? GetPointerOverlapElement(List<TftElement> list,Point point)
        {
            List<TftElement> overlapped = new List<TftElement>();
            foreach (var element in list)
            {
                if (element.IsPointInside(point))
                {
                    overlapped.Add(element);
                }
            }
            // 使用 Zindex 作為 MaxBy 的選擇器
            return overlapped.Count > 0 ? overlapped.MaxBy(e => e.Zindex) : null;
        }
        public static int PointLineDistanceInt(int x1, int y1, int x2, int y2, int px, int py)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            return (int)(Math.Abs(dy * px - dx * py + x2 * y1 - y2 * x1) / Math.Sqrt(dx * dx + dy * dy));
        }
        public static double DistancePointToSegment(Point a, Point b, Point p)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;

            if (dx == 0 && dy == 0)
            {
                // a and b are the same point
                dx = p.X - a.X;
                dy = p.Y - a.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Compute the projection of p onto the line ab, clamped to [0,1]
            float t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));

            // Compute the closest point on the segment
            float closestX = a.X + t * dx;
            float closestY = a.Y + t * dy;

            // Compute the distance
            dx = p.X - closestX;
            dy = p.Y - closestY;

            return Math.Sqrt(dx * dx + dy * dy);
        }
        public static double PointSegmentDistance(double x1, double y1, double x2, double y2, double px, double py)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            double lenSq = dx * dx + dy * dy;
            if (lenSq == 0) return Math.Sqrt((px - x1) * (px - x1) + (py - y1) * (py - y1));

            double t = ((px - x1) * dx + (py - y1) * dy) / lenSq;
            t = Math.Max(0, Math.Min(1, t));

            double nx = x1 + t * dx;
            double ny = y1 + t * dy;
            return Math.Sqrt((nx - px) * (nx - px) + (ny - py) * (ny - py));
        }
        public static double GetPointDistance(Point A, Point B)
        {
            var off = GetPointOffset(A, B);
            return Math.Sqrt(Math.Pow(off.X, 2) + Math.Pow(off.Y,2));
        }
        public static Point CanvasToScreenPos(Point point, Point offset,int zoom)
        {
            var p = new Point(((point.X * zoom) + offset.X), ((point.Y * zoom) + offset.Y));
            return p;
        }
        public static Point ScreenToCanvasPosition(Point point, Point offset, int zoom)
        {
            var p = new Point((point.X - offset.X) / zoom, (point.Y - offset.Y) / zoom);
            return p;
        }
    }
}
