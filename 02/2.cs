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
        var mid = s.Length / 2;
        for (int l = 1; l <= mid; l++)
        {
            var segment = s[..l];
            var repeating = string.Concat(Enumerable.Repeat(segment, s.Length / l));
            if (s == repeating)
            {
                invalidIds.Add(i);
                break;
            }
            
        }
    }
    System.Console.WriteLine($"Range {range}: Invalid IDs: {string.Join(", ", invalidIds.Select(id => id.ToString()))}");
    sum += invalidIds.Sum();
}

System.Console.WriteLine($"Sum: {sum}");