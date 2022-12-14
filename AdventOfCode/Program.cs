using AdventOfCode.Cli.Infrastructure;
using AdventOfCode.Cli.Solutions;
using AdventOfCode.Cli.Solutions.Day15;

var entryPoints = new List<EntryPoint>
{
    new("--- Day 1: Calorie Counting ---", "../../../../Inputs/--- Day 1 Calorie Counting ---.txt"),
    new("--- Day 2: Rock Paper Scissors ---", "../../../../Inputs/--- Day 2 Rock Paper Scissors ---.txt"),
    new("--- Day 3: Rucksack Reorganization ---", "../../../../Inputs/--- Day 3 Rucksack Reorganization ---.txt"),
    new("--- Day 4: Camp Cleanup ---", "../../../../Inputs/--- Day 4 Camp Cleanup ---.txt"),
    new("--- Day 5: Supply Stacks ---", "../../../../Inputs/--- Day 5 Supply Stacks ---.txt"),
    new("--- Day 6: Tuning Trouble ---", "../../../../Inputs/--- Day 6 Tuning Trouble ---.txt"),
    new("--- Day 7: No Space Left On Device ---", "../../../../Inputs/--- Day 7 No Space Left On Device ---.txt"),
    new("--- Day 8: Treetop Tree House ---", "../../../../Inputs/--- Day 8 Treetop Tree House ---.txt"),
    new("--- Day 9: Rope Bridge ---", "../../../../Inputs/--- Day 9 Rope Bridge ---.txt"),
    new("--- Day 10: Cathode-Ray Tube ---", "../../../../Inputs/--- Day 10 Cathode-Ray Tube ---.txt"),
    new("--- Day 11: Monkey in the Middle ---", "../../../../Inputs/--- Day 11 Monkey in the Middle ---.txt"),
    new("--- Day 12: Hill Climbing Algorithm ---", "../../../../Inputs/--- Day 12 Hill Climbing Algorithm ---.txt"),
    new("--- Day 13: Distress Signal ---", "../../../../Inputs/--- Day 13 Distress Signal ---.txt"),
    new("--- Day 14: Regolith Reservoir ---", "../../../../Inputs/--- Day 14 Regolith Reservoir ---.txt"),
    new("--- Day 15: Beacon Exclusion Zone ---", "../../../../Inputs/--- Day 15 Beacon Exclusion Zone ---.txt"),
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
    4 => new CampCleanup(),
    5 => new SupplyStacks(),
    6 => new TuningTrouble(),
    7 => new NoSpaceLeftOnDevice(),
    8 => new TreetopTreeHouse(),
    9 => new RopeBridge(),
    10 => new CathodeRayTube(),
    11 => new MonkeyInTheMiddle(),
    12 => new HillClimbing(),
    13 => new DistressSignal(),
    14 => new RegolithReservoir(),
    15 => new BeaconExclusionZone(),
    _ => throw new ArgumentOutOfRangeException(),
};

var chosenEntryPoint = entryPoints[choiceNumber - 1];

Console.WriteLine();
Console.WriteLine(chosenEntryPoint.Name);
Console.WriteLine($"https://adventofcode.com/2022/day/{choiceNumber}");
Console.WriteLine();

solution.Run(chosenEntryPoint);