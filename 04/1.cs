var lines = File.ReadAllLines(args[0]).Select(l => l.Trim()).ToArray();
var numY = lines.Length;
var numX = lines[0].Length;

int getRoll(char c)
{
    return c == '@' ? 1 : 0;
}

var count = 0;
for (int y = 0; y < numY; y++)
{
    for (int x = 0; x < numX; x++)
    {
        var c = lines[y][x];
        if (c != '@')
            continue;

        int left = (x - 1 >= 0) ? getRoll(lines[y][x - 1]) : 0;
        int topleft = (y - 1 >= 0 && x - 1 >= 0) ? getRoll(lines[y - 1][x - 1]) : 0;
        int top = (y - 1 >= 0) ? getRoll(lines[y - 1][x]) : 0;
        int topright = (y - 1 >= 0 && x + 1 < numX) ? getRoll(lines[y - 1][x + 1]) : 0;
        int right = (x + 1 < numX) ? getRoll(lines[y][x + 1]) : 0;
        int bottomRight = (y + 1 < numY && x + 1 < numX) ? getRoll(lines[y + 1][x + 1]) : 0;
        int bottom = (y + 1 < numY) ? getRoll(lines[y + 1][x]) : 0;
        int bottomLeft = (y + 1 < numY && x - 1 >= 0) ? getRoll(lines[y + 1][x - 1]) : 0;

        var _c = left + topleft + top + topright + right + bottomRight + bottom + bottomLeft;
        if (_c < 4)
            count++;
    }
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Count: " + count.ToString());
