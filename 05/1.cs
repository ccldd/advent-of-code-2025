using System.Collections.Generic;
using System.Linq;

var lines = File.ReadAllLines(args[0]);
Span<string> ranges = [];
Span<string> ingredientIds = [];

for (var i = 0; i < lines.Length; i++)
{
    if (lines[i] == "")
    {
        ranges = lines[0..i];
        ingredientIds = lines[(i + 1)..];
        break;
    }
}

foreach (var item in ranges)
{
    Console.WriteLine(item);
}
Console.WriteLine();
foreach (var item in ingredientIds)
{
    Console.WriteLine(item);
}

var ids = ingredientIds.ToArray().Select(long.Parse).ToHashSet();
var counts = 0;
foreach (var line in ranges)
{
    var split = line.Split("-");
    var start = long.Parse(split[0]);
    var end = long.Parse(split[1]);
    var fresh = ids.Where(id => start <= id && id <= end).ToList();
    counts += fresh.Count;
    fresh.ForEach(x => ids.Remove(x));
}

Console.WriteLine("Fresh: " + counts);
