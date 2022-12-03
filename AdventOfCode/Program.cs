using AdventOfCode.Cli.Infrastructure;
using AdventOfCode.Cli.Solutions;

var entryPoints = new List<EntryPoint>
{
    new("--- Day 1: Calorie Counting ---", "../../../../Inputs/--- Day 1 Calorie Counting ---.txt"),
    new("--- Day 2: Rock Paper Scissors ---", "../../../../Inputs/--- Day 2 Rock Paper Scissors ---.txt"),
    new("--- Day 3: Rucksack Reorganization ---", "../../../../Inputs/--- Day 3 Rucksack Reorganization ---.txt")
};

Console.WriteLine("--- Advent of Code 2022--- ");
Console.WriteLine("Pick a script to run");

for (var index = 0; index < entryPoints.Count; index++)
{
    var entryPoint = entryPoints[index];
    Console.WriteLine($"[{index + 1}]\t- {entryPoint.Name}");
}

var choice = Console.ReadLine()?.Trim();

if (!int.TryParse(choice, out var choiceNumber) || choiceNumber < 1 || choiceNumber > entryPoints.Count)
{
    Console.WriteLine($"{choice} is not a valid choice.");
}

ISolution solution = choiceNumber switch
{
    1 => new CalorieCounting(),
    2 => new RockPaperScissors(),
    3 => new RucksackReorganization(),
    _ => throw new ArgumentOutOfRangeException(),
};

var chosenEntryPoint = entryPoints[choiceNumber - 1];

Console.WriteLine();
Console.WriteLine(chosenEntryPoint.Name);
Console.WriteLine($"https://adventofcode.com/2022/day/{choiceNumber}");
Console.WriteLine();

solution.Run(chosenEntryPoint);