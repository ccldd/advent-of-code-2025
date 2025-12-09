using System.Diagnostics;

var lines = File.ReadAllLines(args[0]);
var junctions = lines
    .Select(l =>
    {
        var tokens = l.Split(",");
        var parsed = tokens.Select(int.Parse).ToList();
        return (parsed[0], parsed[1], parsed[2]);
    })
    .ToList();

double distance((int, int, int) a, (int, int, int) b)
{
    var (a1, a2, a3) = a;
    var (b1, b2, b3) = b;

    return Math.Sqrt(Math.Pow(a1 - b1, 2) + Math.Pow(a2 - b2, 2) + Math.Pow(a3 - b3, 2));
}

var pairs = junctions.SelectMany((x, i) => junctions.Skip(i + 1).Select(y => (x, y))).ToList();
var sortedPairs = pairs.OrderBy(p => distance(p.Item1, p.Item2)).ToList();

// sortedPairs.ForEach(p => Console.WriteLine(p));

var circuits = new List<HashSet<(int, int, int)>>();
for (int i = 0; i < sortedPairs.Count; i++)
{
    var item = sortedPairs[i];

    var commonCircuit = circuits.FirstOrDefault(c =>
        c.Contains(item.Item1) && c.Contains(item.Item2)
    );
    var aCircuit = circuits.FirstOrDefault(c => c.Contains(item.Item1));
    var bCircuit = circuits.FirstOrDefault(c => c.Contains(item.Item2));
    var aInCircuit = aCircuit is not null;
    var bInCircuit = bCircuit is not null;

    if (!aInCircuit && !bInCircuit)
    {
        // make a circuit
        circuits.Add(new HashSet<(int, int, int)>([item.Item1, item.Item2]));
    }
    else if (aInCircuit && !bInCircuit)
    {
        // b goes to circuit a
        aCircuit.Add(item.Item2);
    }
    else if (!aInCircuit && bInCircuit)
    {
        // a goes to circuit b
        bCircuit.Add(item.Item1);
    }
    else if (aCircuit != bCircuit)
    {
        // both are part of existing circuits, join the two circuits together
        var before = circuits.Count;
        aCircuit.UnionWith(bCircuit);
        circuits.Remove(bCircuit);
        Debug.Assert(before - 1 == circuits.Count);
    }
    else
    {
        Debug.Assert(aCircuit == bCircuit);
    }

    if (circuits.Count == 1 && circuits[0].Count == junctions.Count)
    {
        Console.WriteLine(item);
        Console.WriteLine("Answer: " + (long)item.Item1.Item1 * item.Item2.Item1);
        return;
    }
}
