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

        int totalScoreForSnakeRound = 0;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? throw new NullReferenceException();
            char firstCharacter = line[0];
            char secondCharacter = line[^1];

            totalScore += _moveScore[line];
            totalScore += _shapeScore[secondCharacter];

            var counterMove = GetCounterMove(firstCharacter, secondCharacter == 'Z', secondCharacter == 'Y');
            totalScoreForSnakeRound += _moveScore[firstCharacter + " " + counterMove];
            totalScoreForSnakeRound += _shapeScore[counterMove];
        }

        Console.WriteLine($"Your total score in elvish rock paper scissors tournament is {totalScore}.");
        Console.WriteLine($"Your total score in snake modified elvish rock paper scissors tournament is {totalScoreForSnakeRound}.");
    }

    private static char GetCounterMove(char opponentsMove, bool win, bool draw)
    {
        return opponentsMove switch
        {
            'A' when draw => 'X',
            'A' when win => 'Y',
            'A' => 'Z',
            'B' when draw => 'Y',
            'B' when win => 'Z',
            'B' => 'X',
            'C' when draw => 'Z',
            'C' when win => 'X',
            'C' => 'Y',
            _ => throw new ArgumentOutOfRangeException(nameof(opponentsMove), opponentsMove, null)
        };
    }
}