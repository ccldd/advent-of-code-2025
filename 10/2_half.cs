var machines = File.ReadAllLines(args[0]);

// Reduce junctions until all the values are even
(short[], short) ToEven(short[] startingJunctions, short[][] switches)
{
    var queue = new Queue<(short[], short[], short)>(
        switches.Select(s => ((short[])startingJunctions.Clone(), s, (short)0))
    );
    while (queue.Count > 0)
    {
        var curr = queue.Dequeue();
        var (junctions, @switch, presses) = curr;

        var newJunctions = (short[])junctions.Clone();
        foreach (var s in @switch)
            newJunctions[s]--;

        presses++;

        if (newJunctions.All(j => j % 2 == 0))
        {
            return (newJunctions, presses);
        }

        foreach (var s in switches)
        {
            queue.Enqueue((newJunctions, s, presses));
        }
    }

    return default;
}

// Recursively find the minimum presses required to reach desired junction configuration
List<short> solves = new();
short? Solve(short[] junctions, short[][] switches, bool first)
{
    if (junctions.All(j => j == 0))
    {
        // Already solved
        return 0;
    }
    else if (junctions.Any(j => j < 0))
    {
        // Invalid solution
        return null;
    }

    var switchesSolvesIt = switches.Any(s =>
    {
        var c = (short[])junctions.Clone();
        foreach (var i in s)
            c[i]--;

        return c.All(j => j == 0);
    });
    if (switchesSolvesIt)
        return 1;

    if (junctions.All(j => j >= 2))
    {
        // We can half it
        short[] halved = junctions.Select(j => (short)(j / 2)).Select(i => i).ToArray();
        var ret = Solve(halved, switches, false);
        if (ret.HasValue)
        {
            ret *= 2;
            if (first)
                solves.Add(ret.Value);
        }
        return ret;
    }

    // Can't half it anymore, just brute force now
    var nextJunctions = switches.Select(s =>
    {
        var c = (short[])junctions.Clone();
        foreach (var i in s)
            c[i]--;

        return c;
    });
    var localScores = nextJunctions.Select(nj => Solve(nj, switches, false)).OfType<short>();
    if (localScores.Any())
        return (short)(localScores.Min() + 1);

    return null;
}

// Solve(
//     [1, 2],
//     [
//         [0, 1],
//         [1],
//     ],
//     true
// );
// Console.WriteLine(solves.Min());

// Solve([0,1] (0, 1) (1)) = 1
// Solve([1,1] (0, 1) (1)) = 1
// Solve([1,2] (0, 1) (1)) = Solve([1, 0]) + Solve([0, 1])

var finalPresses = 0;
foreach (var m in machines)
{
    var split = m.Split(" ");
    var target = split.Last().Trim('{', '}').Split(',').Select(short.Parse).ToArray();
    var switches = split
        .Skip(1)
        .SkipLast(1)
        .Select(s => s.Trim('(', ')'))
        .Select(s => s.Split(',').Select(short.Parse).ToArray())
        .ToArray();

    var (evenJunctions, pressesToEven) = ToEven(target, switches);

    solves = new List<short>();
    var xxxx = Solve(evenJunctions, switches, true);

    var minPresses = solves.Min() + pressesToEven;
    Console.WriteLine($"{m} {{{string.Join(",", evenJunctions)}}} {pressesToEven} | {minPresses}");
    Console.WriteLine(xxxx + pressesToEven);
    finalPresses += minPresses;
}

Console.WriteLine("Answer: " + finalPresses);
