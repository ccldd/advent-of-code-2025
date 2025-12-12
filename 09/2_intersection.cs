using System.Diagnostics;
using System.Drawing;

var lines = File.ReadAllLines(args[0]);
var vertices = lines
    .Select(l =>
    {
        var t = l.Split(",").ToArray();
        var x = int.Parse(t[0]);
        var y = int.Parse(t[1]);
        return new Point(x, y);
    })
    .ToList();
var maxX = vertices.MaxBy(p => p.X);
var maxY = vertices.MaxBy(p => p.Y);

Console.WriteLine("vertices:");
vertices.ForEach(l => Console.WriteLine(l));
Console.WriteLine();

var vertexPairs = vertices.SelectMany((v, i) => vertices.Skip(i + 1).Select((y) => (v, y))).ToList();
Console.WriteLine("Edges:");
vertexPairs.ForEach(l => Console.WriteLine(l));
Console.WriteLine();

Rectangle FormRectangle((Point, Point) points)
{
    var (a, b) = points;

    int minX = Math.Min(a.X, b.X);
    int maxX = Math.Max(a.X, b.X);
    int minY = Math.Min(a.Y, b.Y);
    int maxY = Math.Max(a.Y, b.Y);
    var topLeft = new Point(minX, minY);

    return new Rectangle(topLeft.X, topLeft.Y, maxX - minX + 1, maxY - minY + 1);
}

Orientation GetOrientation((Point, Point) a)
{
    var (p1, p2) = a;
    if (p1.X == p2.X)
        return Orientation.Vertical;
    else
        return Orientation.Horizontal;
}

bool IsOverlap((int, int) a, (int, int) b)
{
    var (a1, a2) = a;
    var (b1, b2) = b;
    var aMin = Math.Min(a1, a2);
    var aMax = Math.Max(a1, a2);
    var bMin = Math.Min(b1, b2);
    var bMax = Math.Max(b1, b2);

    return aMin <= bMax && bMin <= aMax;
}

bool isIntersect((Point, Point) a, (Point, Point) b)
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

    return (aOrient, bOrient) switch
    {
        (Orientation.Horizontal, Orientation.Horizontal) => a1.Y == b1.Y
            && IsOverlap((a1.X, a2.X), (b1.X, b2.X)),
        (Orientation.Vertical, Orientation.Vertical) => a1.X == b1.X
            && IsOverlap((a1.Y, a2.Y), (b1.Y, b2.Y)),
        (Orientation.Horizontal, Orientation.Vertical) => (b1.X >= aXMin && b1.X <= aXMax)
            && (a1.Y >= bYMin && a1.Y <= bYMax),
        (Orientation.Vertical, Orientation.Horizontal) => (a1.X >= bXMin && a1.X <= bXMax)
            && (b1.Y >= aYMin && b1.Y <= aYMax),
        _ => false,
    };
}

List<(Point, Point)> GetEdges(Rectangle r)
{
    var topLeft = new Point(r.Left, r.Top);
    var topRight = new Point(r.Right, r.Top);
    var bottomLeft = new Point(r.Left, r.Bottom);
    var bottomRight = new Point(r.Right, r.Bottom);

    var topEdge = (topLeft, topRight);
    var rightEdge = (topRight, bottomRight);
    var bottomEdge = (bottomLeft, bottomRight);
    var leftEdge = (topLeft, bottomLeft);

    return [topEdge, rightEdge, bottomEdge, leftEdge];
}

bool IsValidRectangle(Rectangle r)
{
    var edges = GetEdges(r);
    var anyEdgeIntersects = vertexPairs.Any(e1 => edges.Any(e2 => isIntersect(e1, e2)));
    if (anyEdgeIntersects)
        return false;

    return true;
}

long GetArea(Rectangle r)
{
    return (long)r.Width * r.Height;
}

var areas = vertexPairs.ToDictionary(p => p, p => GetArea(FormRectangle(p))).OrderByDescending(kvp => kvp.Value);
foreach (var kvp in areas)
{
  var r = FormRectangle(kvp.Key); 
  var isValid = IsValidRectangle(r);
  Console.WriteLine($"{kvp.Key}, {kvp.Value}, {isValid}");

  if (isValid) break;
}

enum Orientation
{
    Horizontal,
    Vertical,
}
