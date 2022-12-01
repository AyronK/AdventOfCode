using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class CalorieCounting : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        var elfBags = new List<int>();

        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        elfBags.Add(0);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
            {
                elfBags.Add(0);
            }
            else
            {
                elfBags[^1] += int.Parse(line);
            }
        }

        var ordered = elfBags.OrderByDescending(b => b).ToArray();

        Console.WriteLine($"Elf with the most calories in the bag has {ordered[0]} calories.");
        Console.WriteLine($"Top three elves with the most calories in the bag have {ordered[..3].Sum()} calories in total.");
    }
}