using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class NoSpaceLeftOnDevice : ISolution
{
    public interface IFileNode
    {
        DirectoryNode? Parent { get; }
        string Name { get; }
        int Size { get; }
    }

    public record FileNode(string Name, int Size, DirectoryNode Parent) : IFileNode;

    public record DirectoryNode(string Name, DirectoryNode? Parent) : IFileNode
    {
        public Dictionary<string, IFileNode> Files { get; } = new();
        public int Size => Files.Sum(f => f.Value.Size);
    }

    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        DirectoryNode root = new DirectoryNode("/", null);
        DirectoryNode currentDirectory = root;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? string.Empty;

            var input = line.Split(' ');

            if (input[0] == "$")
            {
                if (input[1] == "ls")
                {
                    continue;
                }

                if (input[1] == "cd" && input[2] == "/")
                {
                    currentDirectory = root;
                }
                else if (input[1] == "cd" && input[2] == "..")
                {
                    currentDirectory = currentDirectory.Parent ?? throw new InvalidOperationException();
                }
                else if (input[1] == "cd")
                {
                    currentDirectory = (DirectoryNode)currentDirectory.Files[input[2]];
                }
            }
            else
            {
                if (input[0] == "dir")
                {
                    currentDirectory.Files.Add(input[1], new DirectoryNode(input[1], currentDirectory));
                }
                else
                {
                    currentDirectory.Files.Add(input[1], new FileNode(input[1], int.Parse(input[0]), currentDirectory));
                }
            }
        }

        int total = 0;
        DisplayAndSumTotal(root, ref total);
        Console.WriteLine(total);
    }

    private void DisplayAndSumTotal(DirectoryNode node, ref int total)
    {
        if (node is { Size: <= 100000 })
        {
            total += node.Size;
        }
        
        Console.WriteLine($"{node.Name} - {node.Size}");

        foreach (var fileNode in node.Files.Values)
        {
            if (fileNode is DirectoryNode dir)
            {
                DisplayAndSumTotal(dir, ref total);
            }
        }
    }
}