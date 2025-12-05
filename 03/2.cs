using System.Text;

const int NumDigits = 12;

var banks = File.ReadAllLines(args[0]);

int findBestPairToRemove(StringBuilder buffer)
{
    var bestIndex = buffer.Length - 1;
    for (int i = 1; i < buffer.Length; i++)
    {
        var left = buffer[i-1] - '0';
        var right = buffer[i] - '0';
        if (left < right)
        {
            bestIndex = i-1;
            break;
        }
    }

    return bestIndex;
}

long sum = 0;
foreach (var bank in banks)
{
    var buffer = new StringBuilder(bank);
    while (buffer.Length > NumDigits)
    {
        var indexToRemove = findBestPairToRemove(buffer);
        buffer.Remove(indexToRemove, 1);
    }

    var numberStr = buffer.ToString();
    var number = long.Parse(numberStr);
    System.Console.WriteLine("{0} -> {1}", bank, number);

    sum += number;
}

System.Console.WriteLine("Sum: {0}", sum);