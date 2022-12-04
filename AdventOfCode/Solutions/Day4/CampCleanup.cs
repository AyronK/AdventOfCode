using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class CampCleanup : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        int fullOverlaps = 0;
        int overlaps = 0;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? string.Empty;
            var sections = line.Split(',', '-').Select(int.Parse).ToArray();

            var leftSection = sections[..2];
            var rightSection = sections[2..];

            if (IncludesOneAnother(leftSection, rightSection))
            {
                fullOverlaps++;
                overlaps++;
            }
            else if (IntersectsOneAnother(leftSection, rightSection))
            {
                overlaps++;
            }
        }


        Console.WriteLine($"In {fullOverlaps} pairs of elves the Camp Cleaning assignment overlaps completely.");
        Console.WriteLine($"In {overlaps} pairs of elves the Camp Cleaning assignment overlaps in any way.");
    }

    private static bool IncludesOneAnother(int[] left, int[] right) => Includes(left, right) || Includes(right, left);
    private static bool IntersectsOneAnother(int[] left, int[] right) => IntersectsLeftEnd(left, right) || IntersectsRightEnd(left, right);

    private static bool Includes(int[] left, int[] right) => right[0] >= left[0] && right[1] <= left[1];
    private static bool IntersectsLeftEnd(int[] left, int[] right) => left[0] >= right[1] && left[1] <= right[0];
    private static bool IntersectsRightEnd(int[] left, int[] right) => right[1] >= left[0] && right[0] <= left[1];
}