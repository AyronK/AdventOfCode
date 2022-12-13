using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class DistressSignal : ISolution
{
    private static readonly string[] DividerPackets = { "[[2]]", "[[6]]" };
    
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        int index = 1;
        int sum = 0;

        List<string> packets = new();

        while (!reader.EndOfStream)
        {
            var a = reader.ReadLine();
            var b = reader.ReadLine();
            int x = Compare(a, b);

            if (x == -1)
            {
                sum += index;
            }

            reader.ReadLine();

            index++;

            packets.Add(a);
            packets.Add(b);
        }

        packets.AddRange(DividerPackets);

        var sorted = packets.ToArray();
        Array.Sort(sorted, Compare);

        var decoderSignal = (Array.IndexOf(sorted, DividerPackets[0]) + 1) * (Array.IndexOf(sorted, DividerPackets[1]) + 1);

        Console.WriteLine($"The sum of the indices of pairs in correct order is {sum}.");
        Console.WriteLine($"The decoder key for the distress signal is {decoderSignal}");
    }

    private static int Compare(string a, string b)
    {
        while (!string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b))
        {
            var aToken = GetToken(a);
            if (aToken.Length == 0)
            {
                return -1;
            }

            var bToken = GetToken(b);
            if (bToken.Length == 0)
            {
                return 1;
            }

            var aRemaining = a[GetRestRange(a, aToken)..];
            var bRemaining = b[GetRestRange(b, bToken)..];

            if (int.TryParse(aToken, out var aValue) && int.TryParse(bToken, out var bValue))
            {
                if (aValue < bValue)
                {
                    return -1;
                }

                if (aValue > bValue)
                {
                    return 1;
                }
            }
            else
            {
                if (aToken.StartsWith('[') && !bToken.StartsWith('['))
                {
                    bToken = $"[{bToken}]";
                }

                if (!aToken.StartsWith('[') && bToken.StartsWith('['))
                {
                    aToken = $"[{aToken}]";
                }

                var inner = Compare(aToken[1..^1], bToken[1..^1]);

                if (inner != 0)
                {
                    return inner;
                }
            }

            a = aRemaining;
            b = bRemaining;
        }

        return b.Length > 0 ? -1 : a.Length > 0 ? 1 : 0;
    }

    private static int GetRestRange(string text, string token)
    {
        return token.Length == text.Length ? token.Length : token.Length + 1;
    }

    private static string GetToken(string text)
    {
        if (int.TryParse(text, out _))
        {
            return new string(text.TakeWhile(c => int.TryParse(text, out _)).ToArray());
        }

        int depth = 0;
        int index = 0;

        for (; index < text.Length; index++)
        {
            if (depth == 0 && (index == text.Length || text[index] == ','))
            {
                break;
            }

            depth += text[index] switch
            {
                '[' => 1,
                ']' => -1,
                _ => 0,
            };
        }

        return text[..index];
    }
}