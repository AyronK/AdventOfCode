using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class RockPaperScissors : ISolution
{
    // A for Rock, B for Paper, and C for Scissors.
    // X for Rock, Y for Paper, and Z for Scissors.

    private readonly Dictionary<string, int> _moveScore = new()
    {
        { "A X", 3 }, // Rock x Rock -> draw
        { "B X", 0 }, // Paper x Rock -> lose
        { "C X", 6 }, // Scissors x Rock -> win
        { "A Y", 6 }, // Rock x Paper -> win
        { "B Y", 3 }, // Paper x Paper -> draw
        { "C Y", 0 }, // Scissors X Paper -> lose
        { "A Z", 0 }, // Rock x Scissors -> lose
        { "B Z", 6 }, // Rock x Paper -> win
        { "C Z", 3 }, // draw
    };

    private readonly Dictionary<char, int> _shapeScore = new()
    {
        { 'X', 1 },
        { 'Y', 2 },
        { 'Z', 3 },
    };

    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        int totalScore = 0;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? throw new NullReferenceException();

            totalScore += _moveScore[line];
            totalScore += _shapeScore[line[^1]];
        }
        
        Console.WriteLine($"Your total score in elvish rock paper scissors tournament is {totalScore}.");
    }
}