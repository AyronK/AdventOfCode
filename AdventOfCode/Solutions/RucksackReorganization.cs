using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class RucksackReorganization : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        int sumOfPriorities = 0;
        int sumOfBadgePriorities = 0;
        int elfIndex = 0;

        HashSet<char> previousElfRucksack = new();

        while (!reader.EndOfStream)
        {
            elfIndex++;
            
            var rucksackItems = reader.ReadLine()?.ToCharArray() ?? Array.Empty<char>();
            var indexOfRucksackCompartmentsSplit = rucksackItems.Length / 2;

            var firstCompartment = ..^indexOfRucksackCompartmentsSplit;
            var secondCompartment = indexOfRucksackCompartmentsSplit..;

            for (int i = 0; i < rucksackItems[secondCompartment].Length; i++)
            {
                var rucksackItem = rucksackItems[secondCompartment][i];

                if (rucksackItems[firstCompartment].Contains(rucksackItem))
                {
                    sumOfPriorities += GetScore(rucksackItem);
                    break;
                }
            }

            var groupId = elfIndex % 3;
            var isFirstInGroup = groupId == 1;
            var isLastInGroup = groupId == 0;

            if (isFirstInGroup)
            {
                previousElfRucksack.Clear();
                previousElfRucksack.UnionWith(rucksackItems);
            }
            else
            {
                previousElfRucksack.IntersectWith(rucksackItems);

                if (isLastInGroup)
                {
                    sumOfBadgePriorities += GetScore(previousElfRucksack.Single());
                }
            }
        }

        Console.WriteLine($"The sum of priorities of disorganized items in the rucksacks is {sumOfPriorities}.");
        Console.WriteLine($"The sum of priorities of budges is {sumOfBadgePriorities}.");
    }

    private static int GetScore(char c) => c switch
    {
        _ when c >= 65 && c <= 90 => c - 38,  // a -> 1 z -> 26
        _ when c >= 97 && c <= 122 => c - 96, // A -> 27 Z -> 52
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, null),
    };
}