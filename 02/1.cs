var ranges = File.ReadLines(args[0]);
ranges = string.Join(",", ranges)
    .Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

long sum = 0;
foreach (var range in ranges)
{
    List<long> invalidIds = new();
    var first = long.Parse(range.Split('-')[0]);
    var last = long.Parse(range.Split('-')[1]);
    for (long i = first; i <= last; i++)
    {
        var s = i.ToString();
        if (s.StartsWith('0'))
        {
            continue;
        }

        if (s.Length % 2 == 1)
        {
            continue;
        }

        var mid = s.Length / 2;
        var left = s[..mid];
        var right = s[mid..];
        if (left == right)
        {
            invalidIds.Add(i);
        }
    }
    System.Console.WriteLine($"Range {range}: Invalid IDs: {string.Join(", ", invalidIds.Select(id => id.ToString()))}");
    sum += invalidIds.Sum();
}

System.Console.WriteLine($"Sum: {sum}");