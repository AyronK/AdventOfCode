using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class DistressSignal : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        int index = 1;
        int sum = 0;

        while (!reader.EndOfStream)
        {
            bool? x = AreInCorrectOrder(reader.ReadLine(), reader.ReadLine());

            if (x.HasValue && x.Value)
            {
                sum += index;
            }

            reader.ReadLine();

            index++;
        }

        Console.WriteLine($"The sum of the indices of pairs in correct order is {sum}.");
    }

    private static bool? AreInCorrectOrder(string a, string b)
    {
        while (!string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b))
        {
            var aToken = GetToken(a);
            if (aToken.Length == 0)
            {
                return true;
            }

            var bToken = GetToken(b);
            if (bToken.Length == 0)
            {
                return false;
            }

            var aRemaining = a[GetRestRange(a, aToken)..];
            var bRemaining = b[GetRestRange(b, bToken)..];

            if (int.TryParse(aToken, out var aValue) && int.TryParse(bToken, out var bValue))
            {
                if (aValue < bValue)
                {
                    return true;
                }

                if (aValue > bValue)
                {
                    return false;
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

                var inner = AreInCorrectOrder(aToken[1..^1], bToken[1..^1]);

                if (inner.HasValue)
                {
                    return inner.Value;
                }
            }

            a = aRemaining;
            b = bRemaining;
        }

        return b.Length > 0 ? true : a.Length > 0 ? false : null;
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