using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class RegolithReservoir : ISolution
{
    public record Point(int X, int Y);

    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);

        Point sandSpawner = new Point(500, 0);
        HashSet<Point> rocks = new();

        while (!reader.EndOfStream)
        {
            var points = reader.ReadLine()?.Split(" -> ").Select(s =>
            {
                var coordinates = s.Split(",").Select(int.Parse).ToArray();
                return new Point(coordinates[0], coordinates[1]);
            }).ToArray() ?? Array.Empty<Point>();

            ParseLineOfRocks(points, rocks);
        }

        var gridElements = rocks.Append(sandSpawner).ToArray();
        var minX = gridElements.Min(r => r.X);
        var minY = gridElements.Min(r => r.Y);
        var diffX = gridElements.Max(r => r.X) - minX;
        var diffY = gridElements.Max(r => r.Y) - minY;

        int floorLevel = diffY + 1;

        var sandGeneratedUntilSandFlows = GenerateSand(sandSpawner, rocks, null, (last, _) => last.X - minX >= diffX || last.Y - minY >= diffY);
        var sandGeneratedUntilSpawnerIsCovered = GenerateSand(sandSpawner, rocks, floorLevel, (_, blocked) => blocked.Contains(sandSpawner));

        // -1 before last sand overflows
        Console.WriteLine($"{sandGeneratedUntilSandFlows - 1} units of sand come to rest before sand starts flowing into the abyss below");
        Console.WriteLine($"{sandGeneratedUntilSpawnerIsCovered} units of sand come to rest before it reaches spawner.");
    }

    private static int GenerateSand(Point sandSpawner, HashSet<Point> rocks, int? floorLevel, Func<Point, HashSet<Point>, bool> until)
    {
        Point lastSand = new Point(sandSpawner.X, sandSpawner.Y);

        HashSet<Point> blocked = new(rocks);
        int sandSoFar = 0;
        bool moved = false;

        while (!until(lastSand, blocked))
        {
            if (!moved)
            {
                sandSoFar++;
                lastSand = new Point(sandSpawner.X, sandSpawner.Y);
            }

            Point movedSand = MoveSand(lastSand, blocked, floorLevel);
            moved = movedSand != lastSand;

            blocked.Remove(lastSand);
            blocked.Add(movedSand);

            lastSand = movedSand;
        }

        return sandSoFar;
    }

    private static void ParseLineOfRocks(Point[] points, HashSet<Point> rocks)
    {
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

    private static Point MoveSand(Point sand, HashSet<Point> blocked, int? floorLevel)
    {
        if (sand.Y == floorLevel)
        {
            return sand;
        }
        
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