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

    var (x1, y1) = current;
    var (x2, y2) = next;

    if (x1 == x2)
    {
        var y_min = Math.Min(y1, y2);
        var y_max = Math.Max(y1, y2);
        for (int y = y_min + 1; y < y_max; y++)
        {
            greenTiles.Add((x1, y));
        }
    }
    else if (y1 == y2)
    {
        var x_min = Math.Min(x1, x2);
        var x_max = Math.Max(x1, x2);
        for (int x = x_min + 1; x < x_max; x++)
        {
            greenTiles.Add((x, y1));
        }
    }
}

Console.WriteLine($"Created {greenTiles.Count} green tiles");

var validTiles = new HashSet<(int, int)>(redTilesSet);
validTiles.UnionWith(greenTiles);

var minX = validTiles.Min(t => t.Item1);
var maxX = validTiles.Max(t => t.Item1);
var minY = validTiles.Min(t => t.Item2);
var maxY = validTiles.Max(t => t.Item2);

Console.WriteLine($"Bounds: X[{minX},{maxX}], Y[{minY},{maxY}]");

// Compute interior by flood fill from edges
var exterior = new HashSet<(int, int)>();
var queue = new Queue<(int, int)>();

// Start BFS from edges
for (int x = minX - 1; x <= maxX + 1; x++)
{
    if (!validTiles.Contains((x, minY - 1))) queue.Enqueue((x, minY - 1));
    if (!validTiles.Contains((x, maxY + 1))) queue.Enqueue((x, maxY + 1));
}
for (int y = minY - 1; y <= maxY + 1; y++)
{
    if (!validTiles.Contains((minX - 1, y))) queue.Enqueue((minX - 1, y));
    if (!validTiles.Contains((maxX + 1, y))) queue.Enqueue((maxX + 1, y));
}

while (queue.Count > 0)
{
    var (x, y) = queue.Dequeue();
    if (exterior.Contains((x, y)) || validTiles.Contains((x, y)))
        continue;
    if (x < minX - 1 || x > maxX + 1 || y < minY - 1 || y > maxY + 1)
        continue;

    exterior.Add((x, y));

    queue.Enqueue((x + 1, y));
    queue.Enqueue((x - 1, y));
    queue.Enqueue((x, y + 1));
    queue.Enqueue((x, y - 1));
}

// Add interior as valid
int interior_count = 0;
for (int x = minX; x <= maxX; x++)
{
    for (int y = minY; y <= maxY; y++)
    {
        if (!redTilesSet.Contains((x, y)) && !greenTiles.Contains((x, y)) && !exterior.Contains((x, y)))
        {
            validTiles.Add((x, y));
            interior_count++;
        }
    }
}

Console.WriteLine($"Added {interior_count} interior tiles, Total: {validTiles.Count}");

bool isValidRect(int x1, int y1, int x2, int y2)
{
    var minRectX = Math.Min(x1, x2);
    var maxRectX = Math.Max(x1, x2);
    var minRectY = Math.Min(y1, y2);
    var maxRectY = Math.Max(y1, y2);

    // For large rectangles, use row-by-row checking with early exit
    var area = (long)(maxRectX - minRectX + 1) * (maxRectY - minRectY + 1);

    // Check if all points in rectangle are valid
    for (int y = minRectY; y <= maxRectY; y++)
    {
        for (int x = minRectX; x <= maxRectX; x++)
        {
            if (!validTiles.Contains((x, y)))
            {
                return false;
            }
        }
    }

    return true;
}

Console.WriteLine("Searching for max rectangle...");
var sw = System.Diagnostics.Stopwatch.StartNew();

// Get unique X and Y coordinates
var uniqueX = redTiles.Select(t => t.Item1).Distinct().OrderBy(x => x).ToList();
var uniqueY = redTiles.Select(t => t.Item2).Distinct().OrderBy(y => y).ToList();

Console.WriteLine($"Unique X: {uniqueX.Count}, Unique Y: {uniqueY.Count}");

var maxArea = 0L;
var checked_count = 0;

// Only check rectangles defined by actual red tile coordinates
for (int i = 0; i < uniqueX.Count && sw.ElapsedMilliseconds < 4900; i++)
{
    for (int j = i + 1; j < uniqueX.Count && sw.ElapsedMilliseconds < 4900; j++)
    {
        for (int k = 0; k < uniqueY.Count && sw.ElapsedMilliseconds < 4900; k++)
        {
            for (int l = k + 1; l < uniqueY.Count && sw.ElapsedMilliseconds < 4900; l++)
            {
                var x1 = uniqueX[i];
                var x2 = uniqueX[j];
                var y1 = uniqueY[k];
                var y2 = uniqueY[l];

                // Check if corners are red tiles
                if (!redTilesSet.Contains((x1, y1)) || !redTilesSet.Contains((x2, y2)))
                    continue;

                var rectArea = (long)(x2 - x1 + 1) * (y2 - y1 + 1);

                if (rectArea <= maxArea)
                    continue;

                checked_count++;

                if (isValidRect(x1, y1, x2, y2))
                {
                    maxArea = rectArea;
                }
            }
        }
    }
}

sw.Stop();
Console.WriteLine($"Checked {checked_count} rectangles in {sw.ElapsedMilliseconds}ms");
Console.WriteLine($"Max area: {maxArea}");
