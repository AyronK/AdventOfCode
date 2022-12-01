namespace AdventOfCode.Cli.Solutions;

internal interface ISolution
{
    void Run(EntryPoint entryPoint);
}

internal class CalorieCounting : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        var elfBags = new List<int>();

        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        elfBags.Add(0);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
            {
                elfBags.Add(0);
            }
            else
            {
                elfBags[^1] += int.Parse(line);
            }
        }

        Console.WriteLine($"Elf with the most calories in the bag has {elfBags.Max()} calories.");
    }
}