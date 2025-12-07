using System.Collections.Generic;
using System.Linq;
using Five;

var lines = File.ReadAllLines(args[0]);
Span<string> ranges = [];

for (var i = 0; i < lines.Length; i++)
{
    if (lines[i] == "")
    {
        ranges = lines[0..i];
        break;
    }
}

foreach (var item in ranges)
{
    Console.WriteLine(item);
}

void mergeRanges(List<MyRange> list)
{
    for (int i = 0; i < list.Count; i++)
    {
        var a = list[i];
        if (a == default)
            continue;
        for (int j = i + 1; j < list.Count; j++)
        {
            var b = list[j];
            if (b == default)
                continue;
            if (a.Start <= b.Start && b.End <= a.End)
            {
                // a includes b
                list[j] = default;
            }
            else if (b.Start <= a.Start && a.End <= b.End)
            {
                // b includes a
                list[i] = default;
                break;
            }
            else if (
                (a.Start <= b.Start && b.Start <= a.End) || (a.Start <= b.End && b.End <= a.End)
            )
            {
                // Overlap, combine
                a = list[j] = new MyRange(Math.Min(a.Start, b.Start), Math.Max(a.End, b.End));
                list[i] = default;
            }
        }
    }
}

var list = new List<MyRange>();
foreach (var line in ranges)
{
    var split = line.Split("-");
    var start = long.Parse(split[0]);
    var end = long.Parse(split[1]);
    list.Add(new MyRange(start, end));
}
mergeRanges(list);

var sum = list.Where(l => l != default).Aggregate(0L, (acc, r) => acc + (r.End - r.Start + 1));

Console.WriteLine("Fresh: " + sum);

list.ForEach(x => Console.WriteLine(x));

namespace Five
{
    record class MyRange(long Start, long End);
}
