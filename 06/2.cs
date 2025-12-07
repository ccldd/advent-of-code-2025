var lines = File.ReadAllLines(args[0]);
var height = lines.Length;
var width = lines.Max(l => l.Length);

char op = (char)0;
var sum = 0L;
var nums = new List<int>();
for (int x = 0; x < width; x++)
{
    var col = "";
    for (int y = 0; y < height; y++)
    {
        if (lines[y].Length <= x)
            continue;

        var c = lines[y][x];
        if (c == '+' || c == '*')
        {
            op = c;
        }
        else
        {
            col += c.ToString();
        }
    }

    col = col.Trim();
    if (x == width - 1)
        nums.Add(int.Parse(col));

    if (col == "" || x == width - 1)
    {
        var res = op switch
        {
            '+' => (long)nums.Sum(),
            '*' => nums.Aggregate(1L, (acc, x) => acc * (long)x),
            _ => 0,
        };
        sum += res;
        op = '\0';
        nums = new List<int>();
    }
    else
    {
        nums.Add(int.Parse(col));
    }
}

Console.WriteLine(sum);
