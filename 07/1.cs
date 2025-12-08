var lines = File.ReadAllLines(args[0]);
var grid = new char[lines[0].Length, lines.Length];

var numSplit = 0;
var beams = new HashSet<int>();
for (int y = 1; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        var c = lines[y][x];
        if (y == 1 && lines[y - 1][x] == 'S')
        {
            beams.Add(x);
            break;
        }
        else if (c == '^' && beams.Contains(x))
        {
            numSplit++;
            beams.Remove(x);
            beams.Add(x - 1);
            beams.Add(x + 1);
        }
        else if (c == '|')
        {
            beams.Add(x);
        }
    }
}

Console.WriteLine(numSplit);
