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

// var insideCount = 0;
// Parallel.For(
//     0,
//     length,
//     y =>
//     {
//         var firstX = -1;
//         var lastX = -1;
//         for (int x = 0; x < width; x++)
//         {
//             //var c = grid[x, y];
//             if (redTiles.ContainsKey((x, y))) { }
//             else if (greenTiles.ContainsKey((x, y)))
//             {
//                 if (firstX == -1)
//                     firstX = x;
//                 else
//                     lastX = x;
//             }
//             else
//             {
//                 //grid[x, y] = '.';
//             }
//         }
//
//         if (firstX != -1 && lastX != -1)
//         {
//             Enumerable
//                 .Range(firstX + 1, lastX - firstX - 1)
//                 .ToList()
//                 .ForEach(xx =>
//                 {
//                     //grid[xx, y] = 'X';
//                     greenTiles.TryAdd((xx, y), false);
//                 });
//         }
//         Console.WriteLine($"Added inside greenTiles: {Interlocked.Increment(ref insideCount)}/{length}");
//     }
// );
// Console.WriteLine("Added inside greenTiles");

// Print
// for (int y = 0; y < length; y++)
// {
//     for (int x = 0; x < width; x++)
//     {
//         Console.Write(grid[x, y]);
//     }
//     Console.WriteLine();
// }
//Console.WriteLine("Red Tiles: ");
//redTiles.ToList().ForEach(x => Console.WriteLine(x));
//Console.WriteLine("Green Tiles: ");
//greenTiles.ToList().ForEach(x => Console.WriteLine(x));

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

var isValidPointCache = new Dictionary<(int, int), bool>();
bool isValidPoint((int, int) p)
{
    Console.WriteLine(redTiles.Count);
    Console.WriteLine(greenTiles.Count);
    if (redTiles.Contains(p))
        return true;
    else if (greenTiles.Contains(p))
        return true;

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
        return false;

    var insideX = leftMostTile <= p.Item1 && p.Item1 <= rightMostTile;
    var insideY = topMostTile <= p.Item2 && p.Item2 <= bottomMostTile;
    return insideX && insideY;
}

bool cachedIsValidPoint((int, int) p)
{
    if (isValidPointCache.TryGetValue(p, out var r))
        return r;

    var rr = isValidPoint(p);
    isValidPointCache.Add(p, rr);
    return rr;
}

bool isValidRect(((int, int), (int, int)) pairs)
{
  Console.WriteLine("isValidRect");
    var (a, b) = pairs;
    var (aX, aY) = a;
    var (bX, bY) = b;
    for (int y = Math.Min(aY, bY); y < Math.Max(aY, bY) + 1; y++)
    {
        for (int x = Math.Min(aX, bX); x < Math.Max(aX, bX) + 1; x++)
        {
            if (!cachedIsValidPoint((x, y)))
            {
                return false;
            }
        }
    }

    return true;
}

Console.WriteLine("Calculating areas");
var areas = pairs.ToDictionary(x => x, x => area(x)).OrderBy(kvp => kvp.Value);
foreach (var (k, v) in areas)
{
    Console.WriteLine($"{k} = {v}");
}
// var areas = pairs
//     .Select(x => (x, area(x)))
//     .OrderByDescending(x => x.Item2)
//     .ToDictionary(x => x.Item1, x => x.Item2);
// Console.WriteLine("Solved areas");
// var rect = areas.First(kvp => isValidRect(kvp.Key));
// Console.WriteLine(rect.Key + " " + rect.Value);
// foreach (var (k, v) in areas)
// {
//     Console.WriteLine($"{k} = {v}");
// }
