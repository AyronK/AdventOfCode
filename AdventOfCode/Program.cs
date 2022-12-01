using AdventOfCode.Cli.Solutions;

var entryPoints = new Dictionary<string, EntryPoint>
{
    {
        "1", new EntryPoint("--- Day 1 Calorie Counting ---", "https://adventofcode.com/2022/day/1", "../../../../Inputs/--- Day 1 Calorie Counting ---.txt")
    },
};

Console.WriteLine("--- Advent of Code 2022--- ");
Console.WriteLine("Pick a script to run");

foreach (var entryPoint in entryPoints)
{
    Console.WriteLine($"[{entryPoint.Key}]\t- {entryPoint.Value}");
}

var choice = Console.ReadLine()?.Trim();

if (!entryPoints.Keys.Contains(choice))
{
    Console.WriteLine($"{choice} is not a valid choice.");
}

ISolution solution = choice switch
{
    "1" => new CalorieCounting(),
    _ => throw new ArgumentOutOfRangeException(),
};

var chosenEntryPoint = entryPoints[choice];

Console.WriteLine();
Console.WriteLine(chosenEntryPoint.Name);
Console.WriteLine(chosenEntryPoint.Url);
Console.WriteLine();

solution.Run(chosenEntryPoint);