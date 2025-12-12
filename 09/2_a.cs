using System.Collections.Concurrent;
using System.Linq;

var lines = File.ReadAllLines(args[0]);
var vertices = lines
    .Select(l => l.Split(",").Select(int.Parse).ToArray())
    .Select(x => (x[0], x[1]))
    .ToHashSet();
Console.WriteLine("Loaded vertices");

// vertices.ForEach(v => Console.WriteLine(v));

// Create pairs of two points
var pairs = vertices.SelectMany((v, i) => vertices.Skip(i + 1).Select((y) => (v, y))).ToList();

// Use grid to remember red/green points
var width = vertices.Select(v => v.Item1).Max() + 1;
var length = vertices.Select(v => v.Item2).Max() + 1;
Console.WriteLine($"{width} x {length}");

//var grid = new char[width, length];

// Add red red tiles
//var redTiles = new ConcurrentDictionary<(int, int), bool>(vertices.Select(v => KeyValuePair.Create(v, false)));
var redTiles = vertices;

// foreach (var item in redTiles)
// {
//     var (x, y) = item;
//     //grid[x, y] = '#';
// }

// Add surrounding green tiles
// var greenTiles = new ConcurrentDictionary<(int, int), bool>();
var greenTiles = new HashSet<(int, int)>();
var straightPairs = pairs
    .Where(p => p.Item1.Item1 == p.Item2.Item1 || p.Item1.Item2 == p.Item2.Item2)
    .ToList();

//straightPairs.ForEach(v => Console.WriteLine(v));
foreach (var item in straightPairs)
{
    var (a, b) = item;
    var (aX, aY) = a;
    var (bX, bY) = b;
    if (aX == bX)
    {
        Enumerable
            .Range(Math.Min(aY, bY) + 1, Math.Max(aY, bY) - Math.Min(aY, bY) - 1)
            .ToList()
            .ForEach(y =>
            {
                //grid[aX, y] = 'X';
                greenTiles.Add((aX, y));
            });
    }
    else
    {
        Enumerable
            .Range(Math.Min(aX, bX) + 1, Math.Max(aX, bX) - Math.Min(aX, bX) - 1)
            .ToList()
            .ForEach(x =>
            {
                // grid[x, aY] = 'X';
                greenTiles.Add((x, aY));
            });
    }
}
Console.WriteLine("Added outside green tiles");

var validTiles = redTiles.Concat(greenTiles).ToHashSet();
var minX = validTiles.MinBy(p => p.Item1);
var maxX = validTiles.MaxBy(p => p.Item1);
var minY = validTiles.MinBy(p => p.Item2);
var maxY = validTiles.MaxBy(p => p.Item2);

long area(((int, int), (int, int)) pairs)
{
    var (a, b) = pairs;
    var (aX, aY) = a;
    var (bX, bY) = b;
    var w = Math.Abs(aX - bX + 1);
    var l = Math.Abs(aY - bY + 1);
    return (long)w * l;
}

bool isValidPoint((int, int) p)
{
    // Console.WriteLine(redTiles.Count);
    // Console.WriteLine(greenTiles.Count);
    // if (redTiles.Contains(p))
    // {
    //     // Console.WriteLine("In red");
    //     return true;
    // }
    // else if (greenTiles.Contains(p))
    // {
    //     // Console.WriteLine("In green");
    //     return true;
    // }

    // check if inside
    var leftMostTile = validTiles
        .Where(t => t.Item2 == p.Item2)
        .DefaultIfEmpty((-1, -1))
        .MinBy(t => t.Item1)
        .Item1;
    var rightMostTile = validTiles
        .Where(t => t.Item2 == p.Item2)
        .DefaultIfEmpty((-1, -1))
        .MaxBy(t => t.Item1)
        .Item1;
    var topMostTile = validTiles
        .Where(t => t.Item1 == p.Item1)
        .DefaultIfEmpty((-1, -1))
        .MinBy(t => t.Item2)
        .Item2;
    var bottomMostTile = validTiles
        .Where(t => t.Item1 == p.Item1)
        .DefaultIfEmpty((-1, -1))
        .MaxBy(t => t.Item2)
        .Item2;
    if (leftMostTile == -1 || rightMostTile == -1 || topMostTile == -1 || bottomMostTile == -1)
    {
        Console.WriteLine("not inside");
        return false;
    }

    var insideX = leftMostTile <= p.Item1 && p.Item1 <= rightMostTile;
    var insideY = topMostTile <= p.Item2 && p.Item2 <= bottomMostTile;
    var inside = insideX && insideY;
    if (!inside)
        Console.WriteLine("not inside");
    // else
    //     Console.WriteLine("is inside");
    return inside;
}

var isValidPointCache = new ConcurrentDictionary<(int, int), bool>();
bool cachedIsValidPoint((int, int) p)
{
    return isValidPointCache.GetOrAdd(p, (p) => isValidPoint(p));
}

bool isValidRect(((int, int), (int, int)) pairs)
{
    Console.WriteLine("isValidRect");
    var borderPoints = GetBorder(pairs);

    int i = 0;
    var res = Parallel.ForEach(
        borderPoints,
        (p, xs) =>
        {
            if (!cachedIsValidPoint(p))
            {
                xs.Stop();
                return;
            }

            int ii = Interlocked.Increment(ref i);
            Console.WriteLine($"finished={ii}/{borderPoints.Count}");
        }
    );

    return res.IsCompleted;
}

List<(int, int)> GetBorder(((int, int), (int, int)) pairs)
{
    var (a, b) = pairs;
    var (aX, aY) = a;
    var (bX, bY) = b;

    int minX = Math.Min(aX, bX);
    int maxX = Math.Max(aX, bX);
    int totalX = maxX - minX + 1;

    int minY = Math.Min(aY, bY);
    int maxY = Math.Max(aY, bY);
    int totalY = maxY - minY + 1;

    var topLeft = (minX, minY);
    var topRight = (maxX, minY);
    var bottomLeft = (minX, maxY);
    var bottomRight = (maxX, maxY);

    var topEdge = Enumerable.Range(minX + 1, totalX - 2).Select(x => (x, minY));
    var bottomEdge = Enumerable.Range(minX + 1, totalX - 2).Select(x => (x, maxY));
    var leftEdge = Enumerable.Range(minY + 1, totalY - 2).Select(y => (minX, y));
    var rightEdge = Enumerable.Range(minY + 1, totalY - 2).Select(y => (maxX, y));
    return new (int, int)[] { topLeft, topRight, bottomLeft, bottomRight }
        .Concat(topEdge)
        .Concat(leftEdge)
        .Concat(rightEdge)
        .Concat(bottomEdge)
        .ToList();
    //return new (int, int)[] { topLeft, topRight, bottomLeft, bottomRight }.ToList();
}

Console.WriteLine("Calculating areas");
var areas = pairs
    .Select(x => (x, area(x)))
    .OrderByDescending(x => x.Item2)
    .ToDictionary(x => x.Item1, x => x.Item2);
Console.WriteLine("Solved areas");
isValidRect(((0, 0), (1, 1)));
var rect = areas.First(kvp => isValidRect(kvp.Key));

Console.WriteLine(rect.Key + " " + rect.Value);
// foreach (var (k, v) in areas)
// {
//     Console.WriteLine($"{k} = {v}");
// }
