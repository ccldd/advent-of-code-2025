const int Min = 0;
const int Max = 100;
const int Start = 50;

var lines = File.ReadLinesAsync("./input.txt");
var dial = Start;
var password = 0;
await foreach (var line in lines)
{
    var direction = line[..1];
    var magnitude = int.Parse(line[1..]);
    if (direction == "L")
    {
        dial = (dial - magnitude) % (Max);
        if (dial < Min)
        {
            dial += Max;
        }
    }
    else if (direction == "R")
    {
        dial = (dial + magnitude) % (Max);
    }

    if (dial == 0)
    {
        password++;
    }

    Console.WriteLine("{0}{1} -> {2}", direction, magnitude, dial);
}
Console.WriteLine("Password: {0}", password);