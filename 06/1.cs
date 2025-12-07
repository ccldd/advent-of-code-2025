var lines = File.ReadAllLines(args[0]);
List<List<string>> problems = new();

foreach (var l in lines)
{
    var tokens = l.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    for (int j = 0; j < tokens.Length; j++)
    {
        if (problems.Count <= j)
        {
            problems.Add(new());
        }

        problems[j].Add(tokens[j]);
    }
}

var results = problems.Select(p =>
{
    var op = p.Last();
    var nums = p.SkipLast(1).Select(long.Parse);
    var res = op switch
    {
        "+" => nums.Sum(),
        "*" => nums.Aggregate(1L, (acc, x) => acc * x),
        _ => 0,
    };
    Console.Write(string.Join($" {op} ", nums));
    Console.WriteLine(" = " + res);
    return res;
});
var sum = results.Sum();

Console.WriteLine(sum);
