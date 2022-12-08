using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class TreetopTreeHouse : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        int[][] grid = File.ReadAllLines(entryPoint.InputPath).Select(l => l.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

        int visible = grid.Length * 2 + (grid[0].Length - 2) * 2;

        for (int row = 1; row < grid.Length - 1; row++)
        {
            for (int column = 1; column < grid[0].Length - 1; column++)
            {
                var currentItem = grid[row][column];

                if (grid[row][..column].All(x => x < currentItem))
                {
                    visible++;
                }
                else if (grid[row][(column + 1)..].All(x => x < currentItem))
                {
                    visible++;
                }
                else if (grid.Take(row).All(r => r[column] < currentItem))
                {
                    visible++;
                }
                else if (grid.Skip(row + 1).All(r => r[column] < currentItem))
                {
                    visible++;
                }
            }
        }

        int maxScenicScore = 0;

        for (int row = 1; row < grid.Length - 1; row++)
        {
            for (int column = 1; column < grid[0].Length - 1; column++)
            {
                int left = 0;
                int right = 0;
                int top = 0;
                int bottom = 0;
                
                var currentItem = grid[row][column];

                for (int i = column -1; i >= 0; i--)
                {
                    left++;
                    if(grid[row][i] >= currentItem) break;
                }
                
                for (int i = column + 1; i < grid[row].Length; i++)
                {
                    right++;
                    if(grid[row][i] >= currentItem) break;
                }

                for (int i = row - 1; i >= 0; i--)
                {
                    top++;
                    if(grid[i][column] >= currentItem) break;
                }
                
                for (int i = row + 1; i < grid.Length; i++)
                {
                    bottom++;
                    if(grid[i][column] >= currentItem) break;
                }

                maxScenicScore = Math.Max(maxScenicScore, left * right * top * bottom);
            }
        }

        Console.WriteLine($"There are {visible} visible trees in the grid.");
        Console.WriteLine($"Maximum scenic score is {maxScenicScore}.");
    }
}