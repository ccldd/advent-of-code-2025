using System.Diagnostics;

const int NumDigits = 12;

var banks = File.ReadAllLines(args[0]);

void appendBuffer(List<int> buffer, int maxLength, int value)
{
    buffer.Add(value);
    if (buffer.Count <= maxLength)
    {
        return;
    }

    Debug.Assert(buffer.Count == maxLength + 1);

    // Buffer is over by 1, need to remove one digit
    var index = findBestPairToRemove(buffer);
    buffer.RemoveAt(index);
}

int findBestPairToRemove(List<int> buffer)
{
    var bestIndex = -1;
    for (int i = 1; i < buffer.Count; i++)
    {
        var left = buffer[i-1];
        var right = buffer[i];
        if (left < right)
        {
            bestIndex = i-1;
            break;
        }
    }

    if (bestIndex == -1)
    {
        return buffer.Count - 1;
    }

    return bestIndex;
}

long sum = 0;
foreach (var bank in banks)
{
    var buffer = new List<int>(capacity: 12);

    for (int i = 0; i < bank.Length; i++)
    {
        var currentDigit = (int)bank[i] - (int)'0';
        appendBuffer(buffer, NumDigits, currentDigit);
    }

    var numberStr = buffer.Aggregate("", (acc, digit) => acc + digit.ToString());
    var number = long.Parse(numberStr);
    System.Console.WriteLine("{0} -> {1}", bank, number);

    sum += number;
}

System.Console.WriteLine("Sum: {0}", sum);