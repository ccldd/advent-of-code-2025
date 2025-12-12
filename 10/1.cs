var machines = File.ReadAllLines(args[0]);
var finalPresses = new List<int>();
foreach (var m in machines)
{
    var split = m.Split(" ");
    var target = split[0].Trim('[', ']').Select(c => c == '#').ToArray();
    var switches = split
        .Skip(1)
        .SkipLast(1)
        .Select(s => s.Trim('(', ')'))
        .Select(s => s.Split(',').Select(int.Parse).ToArray())
        .ToArray();

    var queue = new Queue<(bool[], int[], int)>(
        switches.Select(s => (new bool[target.Length], s, 0))
    );
    while (queue.Count > 0)
    {
        var curr = queue.Dequeue();
        var (lights, @switch, presses) = curr;
        var newLights = (bool[])lights.Clone();
        foreach (var s in @switch)
            newLights[s] = !newLights[s];

        presses++;

        if (newLights.SequenceEqual(target))
        {
          finalPresses.Add(presses);
            Console.WriteLine($"{m} = {presses}");
            break;
        }

        foreach (var s in switches)
            queue.Enqueue((newLights, s, presses));
    }
}

Console.WriteLine("Answer: " + finalPresses.Sum());
