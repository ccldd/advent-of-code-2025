var lines = File.ReadAllLines(args[0]);
var vertices = lines
    .Select(l => l.Split(",").Select(int.Parse).ToArray())
    .Select(x => (x[0], x[1]))
    .ToList();

// vertices.ForEach(v => Console.WriteLine(v));

// Create pairs of two points
var pairs = vertices.SelectMany((v, i) => vertices.Skip(i + 1).Select((y) => (v, y))).ToList();

// pairs.ForEach(v => Console.WriteLine(v));

long area(((int, int), (int, int)) pairs)
{
    var (a, b) = pairs;
    var (aX, aY) = a;
    var (bX, bY) = b;
    var w = Math.Abs(aX - bX + 1);
    var l = Math.Abs(aY - bY + 1);
    return (long)w * l;
}

var areas = pairs.ToDictionary(x => x, x => area(x)).OrderBy(kvp => kvp.Value);
foreach (var (k, v) in areas)
{
    Console.WriteLine($"{k} = {v}");
}
