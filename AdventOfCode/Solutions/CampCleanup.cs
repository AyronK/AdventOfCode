using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class CampCleanup : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        int overlaps = 0;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? string.Empty;
            var sections = line.Split(',', '-').Select(int.Parse).ToArray();

            var a = sections[0] >= sections[2] && sections[1] <= sections[3];
            var b = sections[2] >= sections[0] && sections[3] <= sections[1];

            if (a || b)
            {
                overlaps++;
            }
        }


        Console.WriteLine($"In {overlaps} pairs of elves the Camp Cleaning assignment overlaps.");
    }
}