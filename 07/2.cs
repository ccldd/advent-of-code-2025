var lines = File.ReadAllLines(args[0]);
var grid = new char[lines[0].Length, lines.Length];
for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        grid[x, y] = lines[y][x];
    }
}

// Location for S
var sX = lines[0].IndexOf('S');
Console.WriteLine(sX);
var cache = new Dictionary<(int, int), long>();

long solve(int x, int y)
{
  
    if (y >= grid.GetLength(1))
        return 1;
    var c = grid[x, y];
    if (c == '.' || c == 'S')
        return cachingSolve(x, y + 1);
    if (c == '^')
        return cachingSolve(x - 1, y) + cachingSolve(x + 1, y);

    throw new Exception();
}

long cachingSolve(int x, int y)
{

    if (cache.TryGetValue((x, y), out var v))
        return v;

    var z = solve(x, y);
    cache[(x, y)] = z;
    return z;

}

var c = cachingSolve(sX, 0);
Console.WriteLine(c);
