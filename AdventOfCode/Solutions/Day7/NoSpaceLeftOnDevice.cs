using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class NoSpaceLeftOnDevice : ISolution
{
    public interface IFileNode
    {
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
            var input = reader.ReadLine()!.Split(' ');

            switch (input[0])
            {
                case "$" when input[1] == "ls":
                {
                    break;
                }
                case "$" when input[1] == "cd":
                {
                    currentDirectory = input[2] switch
                    {
                        "/" => root,
                        ".." => currentDirectory.Parent ?? throw new InvalidOperationException(),
                        _ => (DirectoryNode)currentDirectory.Files[input[2]],
                    };
                    break;
                }
                case "dir":
                {
                    currentDirectory.Files.Add(input[1], new DirectoryNode(input[1], currentDirectory));
                    break;
                }
                default:
                {
                    currentDirectory.Files.Add(input[1], new FileNode(input[1], int.Parse(input[0]), currentDirectory));
                    break;
                }
            }
        }

        int total = 0;
        int sizeOfSmallestDirectoryToRemove = int.MaxValue;
        var minimumFreeSpaceNeeded = 30000000 - (70000000 - root.Size);

        SumTotal(root, 100000, ref total);
        FindSmallestToRemove(root, minimumFreeSpaceNeeded , ref sizeOfSmallestDirectoryToRemove);

        Console.WriteLine($"The total size of all of the directories with a total size of at most 100000 is {total}.");
        Console.WriteLine($"The total size directory that will free up the space is {sizeOfSmallestDirectoryToRemove}.");
    }

    private static void SumTotal(DirectoryNode node, int maxSize, ref int total)
    {
        if (node.Size <= maxSize)
        {
            total += node.Size;
        }

        foreach (var fileNode in node.Files.Values)
        {
            if (fileNode is DirectoryNode dir)
            {
                SumTotal(dir, maxSize, ref total);
            }
        }
    }   
    
    private static void FindSmallestToRemove(DirectoryNode node, int minSize, ref int smallestSize)
    {
        if (node.Size >= minSize)
        {
            smallestSize = Math.Min(node.Size, smallestSize);
        }

        foreach (var fileNode in node.Files.Values)
        {
            if (fileNode is DirectoryNode dir)
            {
                FindSmallestToRemove(dir, minSize, ref smallestSize);
            }
        }
    }
}