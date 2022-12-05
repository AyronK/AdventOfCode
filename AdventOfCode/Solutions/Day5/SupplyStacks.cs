using System.Text.RegularExpressions;
using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class SupplyStacks : ISolution
{
    record FileInput(string[][] StackInputs, Instruction[] Instructions, int NumberOfColumns);

    record Instruction(int Count, int From, int To);

    delegate void SortStacks(Stack<char>[] stacks, IEnumerable<Instruction> instructions);

    private readonly SortStacks _reorganizeWithSimpleCrane = (stacks, instructions) =>
    {
        foreach (var (count, from, to) in instructions)
        {
            for (int i = 0; i < count; i++)
            {
                stacks[to - 1].Push(stacks[from - 1].Pop());
            }
        }
    };

    private readonly SortStacks _reorganizeWithAdvancedCrane = (stacks, instructions) =>
    {
        Stack<char> tempStack = new();

        foreach (var (count, from, to) in instructions)
        {
            for (int i = 0; i < count; i++)
            {
                tempStack.Push(stacks[from - 1].Pop());
            }

            for (int i = 0; i < count; i++)
            {
                stacks[to - 1].Push(tempStack.Pop());
            }

            tempStack.Clear();
        }
    };

    public void Run(EntryPoint entryPoint)
    {
        var fileInput = ParseEntryPoint(entryPoint);

        Run(fileInput, _reorganizeWithSimpleCrane);
        Run(fileInput, _reorganizeWithAdvancedCrane);
    }

    private static void Run(FileInput fileInput, SortStacks strategy)
    {
        var stacks = GetStacks(fileInput);
        strategy(stacks, fileInput.Instructions);
        ConsoleWriteStacksTops(stacks);
    }

    private FileInput ParseEntryPoint(EntryPoint entryPoint)
    {
        Regex regex = new(@"move (\d*) from (\d*) to (\d*)", RegexOptions.Compiled);

        var allLines = File.ReadAllLines(entryPoint.InputPath);
        var splitIndex = Array.IndexOf(allLines, string.Empty);

        int numberOfColumns = int.Parse(allLines[splitIndex - 1].Trim()[^1].ToString());
        var stacksInputs = allLines.Take(splitIndex - 1).Select(l => l.Split(' ')).Reverse().ToArray();
        var instructions = allLines.Skip(splitIndex + 1).Select(l =>
        {
            var groups = regex.Match(l).Groups;
            return new Instruction(int.Parse(groups[1].Value), int.Parse(groups[2].Value), int.Parse(groups[3].Value));
        }).ToArray();

        var fileInput = new FileInput(stacksInputs, instructions, numberOfColumns);
        return fileInput;
    }

    private static Stack<char>[] GetStacks(FileInput fileInput)
    {
        Stack<char>[] stacks = Enumerable.Range(1, fileInput.NumberOfColumns).Select(_ => new Stack<char>()).ToArray();
        foreach (var stacksInput in fileInput.StackInputs)
        {
            for (int i = 0; i < fileInput.NumberOfColumns; i++)
            {
                if (!string.IsNullOrEmpty(stacksInput[i]) && stacksInput[i] != "[_]") // Input was modified to represent empty stack cells as [_]
                {
                    stacks[i].Push(stacksInput[i][1]);
                }
            }
        }

        return stacks;
    }

    private static void ConsoleWriteStacksTops(IEnumerable<Stack<char>> stacks)
    {
        foreach (var stack in stacks)
        {
            Console.Write(stack.TryPeek(out var c) ? c : string.Empty);
        }

        Console.WriteLine();
    }
}