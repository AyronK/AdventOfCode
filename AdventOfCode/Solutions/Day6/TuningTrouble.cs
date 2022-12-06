using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class TuningTrouble : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);
        var line = reader.ReadLine() ?? string.Empty;
        
        Queue<char> buffer = new(14);

        int initialSequenceIndex = 0;
        int messageSequenceIndex = 0;

        for (int i = 0; i < line.Length; i++)
        {
            if (buffer.Count == 14)
            {
                buffer.Dequeue();
            }
            
            buffer.Enqueue(line[i]);

            if (initialSequenceIndex == 0 && buffer.TakeLast(4).Distinct().Count() == 4)
            {
                initialSequenceIndex = i + 1;
            }

            if (messageSequenceIndex == 0 && buffer.TakeLast(14).Distinct().Count() == 14)
            {
                messageSequenceIndex = i + 1;
            }
        }

        Console.WriteLine($"The elvish protocol starting sequence comes after {initialSequenceIndex} characters.");
        Console.WriteLine($"The elvish protocol message sequence comes after {messageSequenceIndex} characters.");
    }
}