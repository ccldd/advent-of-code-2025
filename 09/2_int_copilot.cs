var lines = File.ReadAllLines(args[0]);
var redTiles = lines
    .Select(l => l.Split(",").Select(int.Parse).ToArray())
    .Select(x => (x[0], x[1]))
    .ToList();

var redTilesSet = redTiles.ToHashSet();

Console.WriteLine($"Loaded {redTiles.Count} red tiles");

// Build the green tiles by connecting consecutive red tiles
var greenTiles = new HashSet<(int, int)>();

for (int i = 0; i < redTiles.Count; i++)
{
    var current = redTiles[i];
    var next = redTiles[(i + 1) % redTiles.Count];
    
    // Connect current to next with green tiles
    var (x1, y1) = current;
    var (x2, y2) = next;
    
    if (x1 == x2)
    {
        // Same column, fill vertically
        var y_min = Math.Min(y1, y2);
        var y_max = Math.Max(y1, y2);
        for (int y = y_min + 1; y < y_max; y++)
        {
            greenTiles.Add((x1, y));
        }
    }
    else if (y1 == y2)
    {
        // Same row, fill horizontally
        var x_min = Math.Min(x1, x2);
        var x_max = Math.Max(x1, x2);
        for (int x = x_min + 1; x < x_max; x++)
        {
            greenTiles.Add((x, y1));
        }
    }
}

Console.WriteLine($"Created {greenTiles.Count} green tiles");

// Flood fill to mark all tiles inside the loop as green
var validTiles = new HashSet<(int, int)>(redTilesSet);
validTiles.UnionWith(greenTiles);

// Find bounds
var minX = validTiles.Min(t => t.Item1);
var maxX = validTiles.Max(t => t.Item1);
var minY = validTiles.Min(t => t.Item2);
var maxY = validTiles.Max(t => t.Item2);

// Flood fill interior
var visited = new HashSet<(int, int)>();
var queue = new Queue<(int, int)>();

// Start from boundary and mark all reachable tiles as outside
for (int x = minX; x <= maxX; x++)
{
    if (!validTiles.Contains((x, minY)))
        queue.Enqueue((x, minY));
    if (!validTiles.Contains((x, maxY)))
        queue.Enqueue((x, maxY));
}
for (int y = minY; y <= maxY; y++)
{
    if (!validTiles.Contains((minX, y)))
        queue.Enqueue((minX, y));
    if (!validTiles.Contains((maxX, y)))
        queue.Enqueue((maxX, y));
}

var outside = new HashSet<(int, int)>();
while (queue.Count > 0)
{
    var (x, y) = queue.Dequeue();
    if (x < minX || x > maxX || y < minY || y > maxY)
        continue;
    if (visited.Contains((x, y)) || validTiles.Contains((x, y)))
        continue;
    
    visited.Add((x, y));
    outside.Add((x, y));
    
    queue.Enqueue((x + 1, y));
    queue.Enqueue((x - 1, y));
    queue.Enqueue((x, y + 1));
    queue.Enqueue((x, y - 1));
}

// Mark all interior tiles as green
for (int x = minX; x <= maxX; x++)
{
    for (int y = minY; y <= maxY; y++)
    {
        if (!redTilesSet.Contains((x, y)) && !outside.Contains((x, y)))
        {
            greenTiles.Add((x, y));
            validTiles.Add((x, y));
        }
    }
}

Console.WriteLine($"Total valid tiles: {validTiles.Count}");

Console.WriteLine("Building efficient lookup...");

// Pre-compute valid cells as a 2D array for faster lookup
var validArray = new bool[maxX + 1, maxY + 1];
foreach (var (x, y) in validTiles)
{
    validArray[x, y] = true;
}

// For each y, precompute which x ranges are fully valid
var validXRanges = new Dictionary<int, List<(int, int)>>();
for (int y = minY; y <= maxY; y++)
{
    var ranges = new List<(int, int)>();
    int start = -1;
    for (int x = minX; x <= maxX + 1; x++)
    {
        bool isValid = x <= maxX && validArray[x, y];
        if (isValid && start == -1)
        {
            start = x;
        }
        else if (!isValid && start != -1)
        {
            ranges.Add((start, x - 1));
            start = -1;
        }
    }
    validXRanges[y] = ranges;
}

bool rectCoveredByRanges(int minRectX, int maxRectX, int minRectY, int maxRectY)
{
    for (int y = minRectY; y <= maxRectY; y++)
    {
        var ranges = validXRanges[y];
        bool covered = false;
        foreach (var (rangeStart, rangeEnd) in ranges)
        {
            if (rangeStart <= minRectX && maxRectX <= rangeEnd)
            {
                covered = true;
                break;
            }
        }
        if (!covered) return false;
    }
    return true;
}

Console.WriteLine("Searching for max rectangle...");
var sw = System.Diagnostics.Stopwatch.StartNew();

var maxArea = 0L;

// Use a smarter approach: for each pair of red tiles, check if valid
// Only check promising pairs
for (int i = 0; i < redTiles.Count; i++)
{
    var (x1, y1) = redTiles[i];
    
    for (int j = i + 1; j < redTiles.Count; j++)
    {
        var (x2, y2) = redTiles[j];
        
        var rminX = Math.Min(x1, x2);
        var rmaxX = Math.Max(x1, x2);
        var rminY = Math.Min(y1, y2);
        var rmaxY = Math.Max(y1, y2);
        
        var rectArea = (long)(rmaxX - rminX + 1) * (rmaxY - rminY + 1);
        
        // Skip if obviously smaller than current max
        if (rectArea <= maxArea)
            continue;
        
        // Check if this rectangle is valid
        if (rectCoveredByRanges(rminX, rmaxX, rminY, rmaxY))
        {
            maxArea = rectArea;
        }
        
        // Timeout check
        if (sw.ElapsedMilliseconds > 4900)
        {
            Console.WriteLine($"Timeout at {sw.ElapsedMilliseconds}ms");
            break;
        }
    }
    
    if (sw.ElapsedMilliseconds > 4900)
        break;
}

sw.Stop();
Console.WriteLine($"Completed in {sw.ElapsedMilliseconds}ms");
Console.WriteLine($"Max area: {maxArea}");
