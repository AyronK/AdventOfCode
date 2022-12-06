using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class TuningTrouble : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);
        var line = reader.ReadLine() ?? string.Empty;

        HashSet<char> set = new(4);
        
        int i = 0;
        
        for (; i < line.Length; i++)
        {
            if (!set.Add(line[i]))
            {
                set.Clear();
            }
            
            if (set.Count == 4)
            {
                break;
            }
        }

        Console.WriteLine($"The elvish protocol starting sequence comes after {i} characters.");
    }
}