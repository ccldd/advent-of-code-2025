var banks = File.ReadAllLines(args[0]);

var sum = 0;
foreach (var bank in banks)
{
    int left = 0;
    int right = 0;

    for (int i = 0; i < bank.Length; i++)
    {
        var curr = int.Parse(bank[i].ToString());

        if (i == 0)
        {
            left = curr;
            continue;
        }
        else if (i == bank.Length - 1 && right == 0)
        {
            right = curr;
            continue;
        }
        else if (i == bank.Length - 1 && curr > right)
        {
            right = curr;
            break;
        }
        else if (curr > left)
        {
            left = curr;
            right = 0;
            continue;
        }
        else if (curr > right)
        {
            right = curr;
            continue;
        }
    }
    System.Console.WriteLine("{0} - {1}{2}", bank, left, right);
    sum += (left * 10) + right;
}

System.Console.WriteLine("Sum: {0}", sum);