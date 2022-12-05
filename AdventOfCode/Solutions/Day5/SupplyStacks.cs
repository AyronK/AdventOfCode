﻿using System.Text.RegularExpressions;
using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class SupplyStacks : ISolution
{
    private Regex _regex = new(@"move (\d*) from (\d*) to (\d*)");

    public void Run(EntryPoint entryPoint)
    {
        var allLines = File.ReadAllLines(entryPoint.InputPath);
        var splitIndex = Array.IndexOf(allLines, string.Empty);

        int numberOfColumns = int.Parse(allLines[splitIndex-1][^2].ToString());

        var stacksInputs = allLines.Take(splitIndex - 1).Select(l => l.Split(' '));
        var instructions = allLines.Skip(splitIndex + 1).Select(l =>
        {
            var groups = _regex.Match(l).Groups;
            (int Count, int From, int To) valueTuple = (int.Parse(groups[1].Value), int.Parse(groups[2].Value), int.Parse(groups[3].Value));
            return valueTuple;
        });

        List<Stack<char>> stacks = new(numberOfColumns);

        foreach (var stacksInput in stacksInputs.Reverse())
        {
            for (int i = 0; i < numberOfColumns; i++)
            {
                if (stacks.Count < i + 1)
                {
                    stacks.Add(new());
                }

                if (!string.IsNullOrEmpty(stacksInput[i]) && stacksInput[i] != "[_]")
                {
                    stacks[i].Push(stacksInput[i][1]);
                }
            }
        }

        foreach (var (count, from, to) in instructions)
        {
            for (int i = 0; i < count; i++)
            {
                stacks[to - 1].Push(stacks[from - 1].Pop());
            }
        }

        foreach (var stack in stacks)
        {
            Console.Write(stack.TryPeek(out var c) ? c : string.Empty);
        }
    }
}