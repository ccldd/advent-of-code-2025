const int Min = 0;
const int Max = 100;
const int Start = 50;

var lines = File.ReadLinesAsync(args[0]);
var dial = Start;
var password = 0;
System.Console.WriteLine(dial);
await foreach (var line in lines)
{
    var direction = line[..1];
    var magnitude = int.Parse(line[1..]);
    var clicks = 0;
    for (int i = 0; i < magnitude; i++)
    {
        dial += direction == "L" ? -1 : 1;
        if (dial < 0)
        {
            dial += Max;
        }
        else if (dial >= Max)
        {
            dial -= Max;
        }

        if (dial == 0)
        {
            clicks++;
        }
    }

    password += clicks;

    Console.WriteLine("{0}{1} -> {2} {3}", direction, magnitude, dial, clicks > 0 ? $"(click {clicks})" : string.Empty);
}
Console.WriteLine("Password: {0}", password);