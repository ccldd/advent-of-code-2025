var machines = File.ReadAllLines(args[0]);
var finalPresses = new List<int>();

//foreach (var m in machines)
Parallel.ForEach(
    machines,
    // new ParallelOptions() { MaxDegreeOfParallelism = 8 },
    (m, state) =>
    {
        var split = m.Split(" ");
        var target = split.Last().Trim('{', '}').Split(',').Select(short.Parse).ToArray();
        var switches = split
            .Skip(1)
            .SkipLast(1)
            .Select(s => s.Trim('(', ')'))
            .Select(s => s.Split(',').Select(short.Parse).ToArray())
            .ToArray();
        var maxSwitch = switches.SelectMany(s => s).Max();
        var switchesComparer = new SwitchesComparer(maxSwitch);

        var deque = new LinkedList<(short[], short[], short)>(
            switches.Select(s => (new short[target.Length], s, (short)0))
        );
        var mode = "stack";
        var maxmax = 100_000;
        while (deque.Count > 0)
        {
            (short[], short[], short) curr;
            if (mode == "queue")
            {
                mode = "queue";
                curr = deque.First.Value;
                deque.RemoveFirst();
            }
            else
            {
                mode = "stack";
                curr = deque.Last.Value;
                deque.RemoveLast();
            }
            var (junctions, @switch, presses) = curr;
            if (presses >= maxmax)
                continue;

            var newJunctions = (short[])junctions.Clone();
            foreach (var s in @switch)
                newJunctions[s] += 1;

            presses++;
            // Console.WriteLine($"{mode} {string.Join(",", newJunctions)} {string.Join(",", @switch)}");

            if (newJunctions.SequenceEqual(target))
            {
                //finalPresses.Add(presses);
                maxmax = presses;
                Console.WriteLine($"{m} = {presses}");
                break;
            }

            if (newJunctions.Select((j, i) => (j, i)).Any((p) => p.Item1 > target[p.Item2]))
            {
                continue;
            }
            // if (newJunctions.Select((j, i) => (j, i)).Any((p) => p.Item1 <= target[p.Item2]))
            // if (newJunctions.Select((j, i) => (j, i)).All((p) => p.Item1 <= (target[p.Item2] / 2)))
            // {
            //   // We are getting close
            //   mode = "queue";
            // }
            // else
            // {
            //   mode = "stack";
            // }

            // if (newJunctions.Select((j, i) => (j, i)).All((p) => p.Item1 <= target[p.Item2]))
            // {
            //     var orderedSwitches = mode switch
            //     {
            //       "stack" => switches.OrderBy(s => s.Sum(i => i)),
            //       "queue" => switches.OrderByDescending(s => s.Sum(i => i))
            //     };
            //     foreach (var s in orderedSwitches)
            //         deque.AddLast((newJunctions, (short[])s, presses));
            // }

            // Focus on reaching the biggest number
            foreach (var s in switches.OrderByDescending(s => s, switchesComparer))
            {
                deque.AddLast((newJunctions, (short[])s, presses));
            }
        }
        finalPresses.Add(maxmax);
    }
);

Console.WriteLine("Answer: " + finalPresses.Sum());

class SwitchesComparer(short max) : IComparer<short[]>
{
    public int Compare(short[] a, short[] b)
    {
        var aHasMax = a.Contains(max);
        var bHasMax = b.Contains(max);

        return (aHasMax, bHasMax) switch
        {
            (true, true) when a.Length == b.Length => 0,
            (true, true) when a.Length > b.Length => -1,
            (true, true) when a.Length < b.Length => 1,
            (true, false) => -1,
            (false, true) => 1,
            (false, false) when a.Length == b.Length => 0,
            (false, false) when a.Length > b.Length => -1,
            (false, false) when a.Length < b.Length => 1,
            _ => 0,
        };
    }
}
