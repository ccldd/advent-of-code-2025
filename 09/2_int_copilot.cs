using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

class LargestGreenRectangle
{
    enum Orientation { Horizontal, Vertical }

    static void Main(string[] args)
    {
        var path = args.Length > 0 ? args[0] : "input.txt";
        if (!File.Exists(path))
        {
            Console.WriteLine($"Input file not found: {path}");
            return;
        }

        var lines = File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        var vertices = lines
            .Select(l =>
            {
                var t = l.Trim().Split(',').Select(s => s.Trim()).ToArray();
                var x = int.Parse(t[0]);
                var y = int.Parse(t[1]);
                return new Point(x, y);
            })
            .ToList();

        if (vertices.Count < 2)
        {
            Console.WriteLine("Need at least two vertices.");
            return;
        }

        // polygon edges: adjacent pairs, wrapping last -> first
        var polygonEdges = new List<(Point, Point)>();
        for (int i = 0; i < vertices.Count; i++)
        {
            var a = vertices[i];
            var b = vertices[(i + 1) % vertices.Count];
            polygonEdges.Add((a, b));
        }

        // enumerate all unordered vertex pairs as candidate opposite corners
        var candidates = new List<((Point, Point) pair, long area)>();
        for (int i = 0; i < vertices.Count; i++)
        {
            for (int j = i + 1; j < vertices.Count; j++)
            {
                var pair = (vertices[i], vertices[j]);
                var rect = FormRectangle(pair);
                var area = GetArea(rect);
                if (area > 0)
                    candidates.Add((pair, area));
            }
        }

        if (candidates.Count == 0)
        {
            Console.WriteLine("No candidate rectangles.");
            return;
        }

        // check candidates in descending area order
        foreach (var kv in candidates.OrderByDescending(x => x.area))
        {
            var pair = kv.pair;
            var rect = FormRectangle(pair);
            if (IsValidRectangle(rect, polygonEdges))
            {
                // Print detailed result for the found rectangle
                Console.WriteLine($"Largest valid area: {kv.area}");
                Console.WriteLine($"Corner A: {pair.Item1.X},{pair.Item1.Y}");
                Console.WriteLine($"Corner B: {pair.Item2.X},{pair.Item2.Y}");
                Console.WriteLine($"Rectangle: Left={rect.Left}, Top={rect.Top}, Width={rect.Width}, Height={rect.Height}");
                Console.WriteLine($"Inclusive bottom-right: ({rect.Left + rect.Width - 1},{rect.Top + rect.Height - 1})");

                var rectEdges = GetRectangleEdges(rect);
                for (int e = 0; e < rectEdges.Count; e++)
                    Console.WriteLine($"Rect edge {e}: {rectEdges[e].Item1.X},{rectEdges[e].Item1.Y} -> {rectEdges[e].Item2.X},{rectEdges[e].Item2.Y}");

                Console.WriteLine($"Polygon edges: {polygonEdges.Count}");
                return;
            }
        }

        Console.WriteLine("No valid rectangle found.");
    }

    static Rectangle FormRectangle((Point, Point) points)
    {
        var (a, b) = points;
        int minX = Math.Min(a.X, b.X);
        int maxX = Math.Max(a.X, b.X);
        int minY = Math.Min(a.Y, b.Y);
        int maxY = Math.Max(a.Y, b.Y);

        // inclusive coordinates: add 1
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        return new Rectangle(minX, minY, width, height);
    }

    static long GetArea(Rectangle r) => (long)r.Width * r.Height;

    static Orientation GetOrientation((Point, Point) seg)
    {
        var (p1, p2) = seg;
        if (p1.X == p2.X) return Orientation.Vertical;
        return Orientation.Horizontal;
    }

    static bool IsOverlap((int, int) a, (int, int) b)
    {
        var (a1, a2) = a;
        var (b1, b2) = b;
        var aMin = Math.Min(a1, a2);
        var aMax = Math.Max(a1, a2);
        var bMin = Math.Min(b1, b2);
        var bMax = Math.Max(b1, b2);
        return aMin <= bMax && bMin <= aMax;
    }

    // Return true only for a proper crossing: intersection point strictly interior to both segments.
    static bool SegmentsProperlyCross((Point, Point) a, (Point, Point) b)
    {
        var aOrient = GetOrientation(a);
        var bOrient = GetOrientation(b);
        var (a1, a2) = a;
        var (b1, b2) = b;

        var aXMin = Math.Min(a1.X, a2.X);
        var aXMax = Math.Max(a1.X, a2.X);
        var aYMin = Math.Min(a1.Y, a2.Y);
        var aYMax = Math.Max(a1.Y, a2.Y);
        var bXMin = Math.Min(b1.X, b2.X);
        var bXMax = Math.Max(b1.X, b2.X);
        var bYMin = Math.Min(b1.Y, b2.Y);
        var bYMax = Math.Max(b1.Y, b2.Y);

        // same orientation: cannot "cross" unless collinear; collinear overlap is allowed (not a crossing)
        if (aOrient == bOrient)
            return false;

        // one horizontal, one vertical -> check intersection point
        // intersection point is (vertical.X, horizontal.Y)
        if (aOrient == Orientation.Horizontal && bOrient == Orientation.Vertical)
        {
            int ix = b1.X;
            int iy = a1.Y;
            // interior to a? interior means strictly between endpoints (not equal to endpoint)
            bool interiorA = (ix > aXMin && ix < aXMax) && (iy > aYMin && iy < aYMax); // note aYMin==aYMax for horizontal, so second check false; use X-range and Y equality
            // For horizontal segment a, interior on a means ix strictly between aXMin and aXMax and iy == aYMin (which equals aYMax).
            interiorA = (ix > aXMin && ix < aXMax) && (iy == aYMin);
            // interior to b (vertical) means iy strictly between bYMin and bYMax and ix == bXMin
            bool interiorB = (iy > bYMin && iy < bYMax) && (ix == bXMin);
            return interiorA && interiorB;
        }

        // a vertical, b horizontal
        {
            int ix = a1.X;
            int iy = b1.Y;
            bool interiorA = (iy > aYMin && iy < aYMax) && (ix == aXMin);
            bool interiorB = (ix > bXMin && ix < bXMax) && (iy == bYMin);
            return interiorA && interiorB;
        }
    }

    static List<(Point, Point)> GetRectangleEdges(Rectangle r)
    {
        int left = r.Left;
        int top = r.Top;
        int right = r.Left + r.Width - 1;   // inclusive
        int bottom = r.Top + r.Height - 1;  // inclusive

        var topLeft = new Point(left, top);
        var topRight = new Point(right, top);
        var bottomLeft = new Point(left, bottom);
        var bottomRight = new Point(right, bottom);

        return new List<(Point, Point)>
        {
            (topLeft, topRight),
            (topRight, bottomRight),
            (bottomLeft, bottomRight),
            (topLeft, bottomLeft)
        };
    }

    static bool IsValidRectangle(Rectangle r, List<(Point, Point)> polygonEdges)
    {
        var rectEdges = GetRectangleEdges(r);
        // reject only if any rectangle edge properly crosses any polygon edge
        foreach (var pe in polygonEdges)
        {
            foreach (var re in rectEdges)
            {
                if (SegmentsProperlyCross(pe, re))
                    return false;
            }
        }
        return true;
    }
}
