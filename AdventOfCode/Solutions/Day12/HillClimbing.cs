using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class HillClimbing : ISolution
{
    public record Point(int X, int Y);

    public record Node(Point Point)
    {
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Node Parent { get; set; }
    };

    private static readonly object _lock = new();

    public void Run(EntryPoint entryPoint)
    {
        char[][] heatmap = File.ReadAllLines(entryPoint.InputPath).Select(l => l.ToArray()).ToArray();

        var start = new Node(FindPoint(heatmap, 'S'));
        var end = new Node(FindPoint(heatmap, 'E'));

        heatmap[start.Point.X][start.Point.Y] = 'a';
        heatmap[end.Point.X][end.Point.Y] = 'z';

        start.Distance = CalculateDistance(start.Point, end.Point);

        var minFromAny = int.MaxValue;
        var finalNode = FindShortestPath(heatmap, start, end, ref minFromAny);

        Console.WriteLine(
            $"The fewest steps required to move from your current position to the location that should get the best signal is {finalNode.Cost}.");

        var lowestTerrainPoints = heatmap.SelectMany(GetPoints)
            .Where(p => heatmap[p.X][p.Y] == 'a')
            .OrderBy(p => CalculateDistance(p, end.Point));

        Parallel.ForEach(lowestTerrainPoints, point =>
        {
            var newStart = new Node(point);
            if (FindShortestPath(heatmap, newStart, end, ref minFromAny) is not { } newFinalNode)
            {
                return;
            }

            lock (_lock)
            {
                minFromAny = Math.Min(minFromAny, newFinalNode.Cost);
            }
        });

        Console.WriteLine($"The fewest steps required to move from any position to the location that should get the best signal is {minFromAny}.");
    }

    private IEnumerable<Point> GetPoints(char[] r, int rIdx) => r.Select((_, cIdx) => new Point(rIdx, cIdx));

    private static Node? FindShortestPath(char[][] heatmap, Node start, Node end, ref int shortestSoFar)
    {
        var activeNodes = new HashSet<Node> { start };
        var visitedNodes = new HashSet<Node>();

        while (activeNodes.Any())
        {
            var checkTile = activeNodes.OrderBy(x => x.CostDistance).First();

            if (checkTile.Cost >= shortestSoFar)
            {
                return null;
            }

            if (checkTile.Point == end.Point)
            {
                return checkTile;
            }

            visitedNodes.Add(checkTile);
            activeNodes.Remove(checkTile);

            var walkableTiles = GetWalkableNodes(heatmap, checkTile, end);

            foreach (var walkableTile in walkableTiles)
            {
                if (visitedNodes.Any(node => node.Point == walkableTile.Point))
                {
                    continue;
                }

                if (activeNodes.Any(node => node.Point == walkableTile.Point))
                {
                    var existingTile = activeNodes.First(node => node.Point == walkableTile.Point);
                    if (existingTile.CostDistance > checkTile.CostDistance)
                    {
                        activeNodes.Remove(existingTile);
                        activeNodes.Add(walkableTile);
                    }
                }
                else
                {
                    activeNodes.Add(walkableTile);
                }
            }
        }

        return null;
    }

    private static Point FindPoint(char[][] heatmap, char point)
    {
        var x = Array.FindIndex(heatmap, chars => chars.Contains(point));
        var y = Array.FindIndex(heatmap[x], chars => chars == point);

        return new Point(x, y);
    }

    private static IEnumerable<Node> GetWalkableNodes(char[][] map, Node currentTile, Node targetTile)
    {
        foreach (var point in GetWalkablePoints(map, currentTile.Point))
        {
            if (point == currentTile.Parent?.Point) continue;
            yield return currentTile with
            {
                Point = point,
                Distance = CalculateDistance(point, targetTile.Point),
                Parent = currentTile,
                Cost = currentTile.Cost + 1,
            };
        }
    }

    private static IEnumerable<Point> GetWalkablePoints(char[][] map, Point point)
    {
        if (point.X > 0 && map[point.X - 1][point.Y] <= map[point.X][point.Y] + 1)
        {
            yield return point with { X = point.X - 1 };
        }

        if (point.X < map.Length - 1 &&
            map[point.X + 1][point.Y] <= map[point.X][point.Y] + 1)
        {
            yield return point with { X = point.X + 1 };
        }

        if (point.Y > 0 && map[point.X][point.Y - 1] <= map[point.X][point.Y] + 1)
        {
            yield return point with { Y = point.Y - 1 };
        }

        if (point.Y < map[point.X].Length - 1 &&
            map[point.X][point.Y + 1] <= map[point.X][point.Y] + 1)
        {
            yield return point with { Y = point.Y + 1 };
        }
    }

    public static int CalculateDistance(Point from, Point to)
    {
        return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
    }
}