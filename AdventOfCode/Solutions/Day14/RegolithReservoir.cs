using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class RegolithReservoir : ISolution
{
    public record Point(int X, int Y);

    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        HashSet<Point> rocks = new();
        Point sandSpawner = new Point(500, 0);

        while (!reader.EndOfStream)
        {
            var points = reader.ReadLine().Split(" -> ").Select(s =>
            {
                var coordinates = s.Split(",").Select(int.Parse).ToArray();
                return new Point(coordinates[0], coordinates[1]);
            }).ToArray();

            for (var index = 1; index < points.Length; index++)
            {
                var point = points[index];

                var xDiff = points[index].X - points[index - 1].X;
                var yDiff = points[index].Y - points[index - 1].Y;

                if (xDiff == 0)
                {
                    for (int i = 0; i <= Math.Abs(yDiff); i++)
                    {
                        rocks.Add(point with { Y = point.Y - (i * Math.Sign(yDiff)) });
                    }
                }
                else if (yDiff == 0)
                {
                    for (int i = 0; i <= Math.Abs(xDiff); i++)
                    {
                        rocks.Add(point with { X = point.X - (i * Math.Sign(xDiff)) });
                    }
                }
            }
        }

        var minX = rocks.Append(sandSpawner).Min(r => r.X);
        var minY = rocks.Append(sandSpawner).Min(r => r.Y);
        var diffX = rocks.Append(sandSpawner).Max(r => r.X) - minX;
        var diffY = rocks.Append(sandSpawner).Max(r => r.Y) - minY;

        int sandSoFar = 0;
        Point lastSand = new Point(sandSpawner.X, sandSpawner.Y);
        bool moved = false;
        HashSet<Point> blocked = new(rocks);

        while (lastSand.X - minX < diffX + 1 && lastSand.Y - minY < diffY + 1)
        {
            if (!moved)
            {
                sandSoFar++;
                lastSand = new Point(sandSpawner.X, sandSpawner.Y);
            }

            Point movedSand = MoveSand(lastSand, blocked);
            moved = movedSand != lastSand;

            blocked.Remove(lastSand);
            blocked.Add(movedSand);

            lastSand = movedSand;
        }

        Console.WriteLine(sandSoFar - 1);

        minX = rocks.Concat(blocked).Append(sandSpawner).Min(r => r.X);
        minY = rocks.Concat(blocked).Append(sandSpawner).Min(r => r.Y);
        diffX = rocks.Concat(blocked).Append(sandSpawner).Max(r => r.X) - minX;
        diffY = rocks.Concat(blocked).Append(sandSpawner).Max(r => r.Y) - minY;
        var grid = Enumerable.Range(0, diffY + 1).Select(_ => new string('.', diffX + 1).ToCharArray()).ToArray();

        foreach (var point in blocked)
        {
            grid[point.Y - minY][point.X - minX] = 'o';
        }
        
        foreach (var point in rocks)
        {
            grid[point.Y - minY][point.X - minX] = '#';
        }

        grid[sandSpawner.Y - minY][sandSpawner.X - minX] = '+';

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                Console.Write(grid[i][j]);
            }

            Console.WriteLine();
        }
    }

    private static Point MoveSand(Point sand, HashSet<Point> blocked)
    {
        var target = sand with { Y = sand.Y + 1 };
        if (!blocked.Contains(target))
        {
            return target;
        }

        target = target with { X = target.X - 1 };
        if (!blocked.Contains(target))
        {
            return target;
        }

        target = target with { X = target.X + 2 };
        if (!blocked.Contains(target))
        {
            return target;
        }

        return sand;
    }
}