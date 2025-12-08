var lines = File.ReadAllLines(args[0]);
var grid = new char[lines[0].Length, lines.Length];
for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        grid[x, y] = lines[y][x];
    }
}

var prints = 0;
void printGrid(int timelineId, char[,] grid)
{
    var p = Interlocked.Increment(ref prints);
    Console.WriteLine($"TimelineId: {p}");
    return;
    for (int y = 0; y < lines.Length; y++)
    {
        for (int x = 0; x < lines[y].Length; x++)
        {
            Console.Write(grid[x, y]);
        }
        Console.WriteLine();
        Console.WriteLine();
    }
}

var timelineId = 0;
var tasks = new List<Task>();

async Task f(char[,] grid, int startX, int startY)
{
    for (int y = startY; y < lines.Length; y++)
    {
        for (int x = startX; x < lines[y].Length; x++)
        {
            var c = lines[y][x];
            if (c == 'S')
            {
                grid[x, y + 1] = '|';
                break;
            }

            if (y >= 1 && grid[x, y - 1] == '|')
            {
                if (c == '^')
                {
                    var left = new char[grid.GetLength(0), grid.GetLength(1)];
                    Array.Copy(grid, left, grid.Length);
                    left[x - 1, y] = '|';
                    tasks.Add(f(left, x - 1, y + 1));

                    var right = new char[grid.GetLength(0), grid.GetLength(1)];
                    Array.Copy(grid, right, grid.Length);
                    right[x + 1, y] = '|';
                    tasks.Add(f(right, x + 1, y + 1));
                    return;
                }
                else
                {
                    grid[x, y] = '|';
                }
            }
        }
    }

    bool hasBeam = false;
    for (int x = 0; x < grid.GetLength(0); x++)
    {
        if (grid[x, grid.GetLength(1) - 1] == '|')
            hasBeam = true;
    }

    printGrid(timelineId, grid);

    if (!hasBeam)
        throw new Exception();
}

tasks.Add(f(grid, 0, 0));
await Task.WhenAll(tasks);
Console.WriteLine(prints);
