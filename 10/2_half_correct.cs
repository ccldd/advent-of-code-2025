using System;
using System.Collections.Generic;
using System.Linq;

class Advent2025Day10
{
    // Types
    // Wiring: HashSet<int>
    // Presses: List<HashSet<int>> (a sequence of buttons pressed)

    static string KeyFromSet(HashSet<int> set)
    {
        if (set == null || set.Count == 0) return "";
        var arr = set.OrderBy(i => i).Select(i => i.ToString()).ToArray();
        return string.Join(",", arr);
    }

    static string KeyFromArray(int[] arr)
    {
        if (arr == null || arr.Length == 0) return "";
        return string.Join(",", arr.Select(i => i.ToString()));
    }

    static (HashSet<int> indicators, List<HashSet<int>> buttons, int[] joltages) ParseMachine(string line)
    {
        // Each part is like "[...]" so remove first and last char
        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Substring(1, p.Length - 2))
                        .ToArray();

        var rawIndicators = parts[0];
        var rawButtons = parts.Skip(1).Take(parts.Length - 2).ToArray();
        var rawJoltages = parts.Last();

        var indicators = new HashSet<int>();
        for (int i = 0; i < rawIndicators.Length; i++)
            if (rawIndicators[i] == '#')
                indicators.Add(i);

        var buttons = new List<HashSet<int>>();
        foreach (var rb in rawButtons)
        {
            if (string.IsNullOrWhiteSpace(rb))
            {
                buttons.Add(new HashSet<int>());
                continue;
            }
            var indices = rb.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.Parse(s.Trim()));
            buttons.Add(new HashSet<int>(indices));
        }

        var joltages = rawJoltages.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => int.Parse(s.Trim()))
                                  .ToArray();

        return (indicators, buttons, joltages);
    }

    static Dictionary<string, List<List<HashSet<int>>>> ValidPatterns(List<HashSet<int>> buttons)
    {
        // Key: pattern (as sorted comma-separated string), Value: list of presses (each press is a list of button sets)
        var patterns = new Dictionary<string, List<List<HashSet<int>>>>();

        int n = buttons.Count;
        // generate combinations by number of presses
        for (int k = 0; k <= n; k++)
        {
            foreach (var combo in Combinations(Enumerable.Range(0, n).ToArray(), k))
            {
                var pattern = new HashSet<int>();
                var presses = new List<HashSet<int>>();
                foreach (var idx in combo)
                {
                    var button = buttons[idx];
                    presses.Add(new HashSet<int>(button));
                    // symmetric difference: toggle membership
                    foreach (var b in button)
                    {
                        if (pattern.Contains(b)) pattern.Remove(b);
                        else pattern.Add(b);
                    }
                }
                var key = KeyFromSet(pattern);
                if (!patterns.TryGetValue(key, out var list))
                {
                    list = new List<List<HashSet<int>>>();
                    patterns[key] = list;
                }
                list.Add(presses);
            }
        }

        return patterns;
    }

    static IEnumerable<int[]> Combinations(int[] arr, int k)
    {
        if (k == 0)
        {
            yield return new int[0];
            yield break;
        }
        if (k > arr.Length) yield break;

        var indices = new int[k];
        for (int i = 0; i < k; i++) indices[i] = i;

        while (true)
        {
            var combo = new int[k];
            for (int i = 0; i < k; i++) combo[i] = arr[indices[i]];
            yield return combo;

            int pos = k - 1;
            while (pos >= 0 && indices[pos] == arr.Length - k + pos) pos--;
            if (pos < 0) break;
            indices[pos]++;
            for (int j = pos + 1; j < k; j++) indices[j] = indices[j - 1] + 1;
        }
    }

    static int? ConfigureIndicators(HashSet<int> indicators, Dictionary<string, List<List<HashSet<int>>>> patterns)
    {
        var key = KeyFromSet(indicators);
        if (!patterns.TryGetValue(key, out var pressesList) || pressesList.Count == 0) return null;
        int? best = null;
        foreach (var presses in pressesList)
        {
            int count = presses.Count;
            if (best == null || count < best) best = count;
        }
        return best;
    }

    static int? ConfigureJoltages(int[] joltages, Dictionary<string, List<List<HashSet<int>>>> patterns)
    {
        var memo = new Dictionary<string, int?>();

        int? GetMinPresses(int[] target)
        {
            var tkey = KeyFromArray(target);
            if (memo.TryGetValue(tkey, out var cached)) return cached;

            // if all zeros
            if (!target.Any(x => x != 0))
            {
                memo[tkey] = 0;
                return 0;
            }

            // indicators are positions with odd joltage
            var indicators = new HashSet<int>();
            for (int i = 0; i < target.Length; i++)
                if ((target[i] & 1) == 1) indicators.Add(i);

            var indicatorsKey = KeyFromSet(indicators);
            int? result = null;

            if (!patterns.TryGetValue(indicatorsKey, out var pressesOptions))
            {
                memo[tkey] = null;
                return null;
            }

            foreach (var presses in pressesOptions)
            {
                var targetAfter = (int[])target.Clone();
                bool negative = false;
                foreach (var button in presses)
                {
                    foreach (var idx in button)
                    {
                        if (idx < 0 || idx >= targetAfter.Length) { negative = true; break; }
                        targetAfter[idx] -= 1;
                        if (targetAfter[idx] < 0) { negative = true; break; }
                    }
                    if (negative) break;
                }
                if (negative) continue;

                // all new target levels are even; compute half-target
                var halfTarget = new int[targetAfter.Length];
                for (int i = 0; i < targetAfter.Length; i++) halfTarget[i] = targetAfter[i] / 2;

                var halfPresses = GetMinPresses(halfTarget);
                if (halfPresses == null) continue;

                int numPresses = presses.Count + 2 * halfPresses.Value;
                if (result == null || numPresses < result) result = numPresses;
            }

            memo[tkey] = result;
            return result;
        }

        return GetMinPresses(joltages);
    }

    // Solve given input lines and return tuple of two ints
    static (int indicatorSum, int joltageSum) Solve(IEnumerable<string> lines)
    {
        int numIndicatorPresses = 0;
        int numJoltagePresses = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var (indicators, buttons, joltages) = ParseMachine(line);
            var patterns = ValidPatterns(buttons);

            var indicatorResult = ConfigureIndicators(indicators, patterns);
            if (indicatorResult == null) throw new Exception("No solution for indicators");
            numIndicatorPresses += indicatorResult.Value;

            var joltageResult = ConfigureJoltages(joltages, patterns);
            if (joltageResult == null) throw new Exception("No solution for joltages");
            numJoltagePresses += joltageResult.Value;
        }

        return (numIndicatorPresses, numJoltagePresses);
    }

    // Example console program: read all lines from stdin and print results
    static void Main(string[] args)
    {
        var lines = new List<string>();
        string? line;
        while ((line = Console.ReadLine()) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line.Trim());
        }

        var (indicators, joltages) = Solve(lines);
        Console.WriteLine($"{indicators} {joltages}");
    }
}
