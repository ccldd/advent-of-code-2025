var lines = File.ReadAllLines(args[0]).Select(l => l.Trim()).ToArray();
var numY = lines.Length;
var numX = lines[0].Length;
var grid = new char[numY][];

int getRoll(char c)
{
    return c == '@' ? 1 : 0;
}

for (int r = 0; r < numY; r++)
{
    grid[r] = new char[numX];
    for (int c = 0; c < numX; c++)
    {
        grid[r][c] = lines[r][c];
    }
}

var totalCount = 0;
var count = 0;
do
{
    count = 0;
    for (int y = 0; y < numY; y++)
    {
        for (int x = 0; x < numX; x++)
        {
            var c = grid[y][x];
            if (c != '@')
                continue;

            int left = (x - 1 >= 0) ? getRoll(grid[y][x - 1]) : 0;
            int topleft = (y - 1 >= 0 && x - 1 >= 0) ? getRoll(grid[y - 1][x - 1]) : 0;
            int top = (y - 1 >= 0) ? getRoll(grid[y - 1][x]) : 0;
            int topright = (y - 1 >= 0 && x + 1 < numX) ? getRoll(grid[y - 1][x + 1]) : 0;
            int right = (x + 1 < numX) ? getRoll(grid[y][x + 1]) : 0;
            int bottomRight = (y + 1 < numY && x + 1 < numX) ? getRoll(grid[y + 1][x + 1]) : 0;
            int bottom = (y + 1 < numY) ? getRoll(grid[y + 1][x]) : 0;
            int bottomLeft = (y + 1 < numY && x - 1 >= 0) ? getRoll(grid[y + 1][x - 1]) : 0;

            var _c = left + topleft + top + topright + right + bottomRight + bottom + bottomLeft;
            if (_c < 4)
            {
                count++;
                grid[y][x] = 'x';
            }
        }
    }

    totalCount += count;
    Console.WriteLine("Count: " + count.ToString());
} while (count > 0);

Console.WriteLine("Total Count: " + totalCount.ToString());
