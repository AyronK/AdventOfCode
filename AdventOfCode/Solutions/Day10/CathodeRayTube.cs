using System.Text;
using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class CathodeRayTube : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        Queue<int> cycles = new();


        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line == "noop")
            {
                cycles.Enqueue(0);
            }

            if (line.StartsWith("addx"))
            {
                cycles.Enqueue(0);
                cycles.Enqueue(int.Parse(line.Split(" ")[^1]));
            }
        }

        int count = cycles.Count;
        List<int> signalStrengthByCycle = Enumerable.Repeat(0, count + 1).ToList();
        int registryValue = 1;

        StringBuilder crt = new StringBuilder();

        for (int i = 1; i <= count; i++)
        {
            signalStrengthByCycle[i] = i * registryValue;

            var crtPosition = (i % 40) - 1;

            var shouldDraw = crtPosition >= registryValue - 1 && crtPosition <= registryValue + 1;
            crt.Append(shouldDraw ? '#' : '.');

            if (crtPosition == -1)
            {
                crt.AppendLine();
            }

            registryValue += cycles.TryDequeue(out var value) ? value : 0;
        }

        Console.WriteLine(crt);
        Console.WriteLine($"20th: {signalStrengthByCycle[20]}");
        Console.WriteLine($"60th: {signalStrengthByCycle[60]}");
        Console.WriteLine($"100th: {signalStrengthByCycle[100]}");
        Console.WriteLine($"140th: {signalStrengthByCycle[140]}");
        Console.WriteLine($"180th: {signalStrengthByCycle[180]}");
        Console.WriteLine($"220th: {signalStrengthByCycle[220]}");
        Console.WriteLine(
            $"Sum: {signalStrengthByCycle[20] + signalStrengthByCycle[60] + signalStrengthByCycle[100] + signalStrengthByCycle[140] + signalStrengthByCycle[180] + signalStrengthByCycle[220]}");
    }
}