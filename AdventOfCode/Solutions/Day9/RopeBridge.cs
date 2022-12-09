using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
}

public record Move(Direction Direction, int Length);

public record Point(int X, int Y)
{
    public bool Touch(Point b)
    {
        var x = Math.Abs(X - b.X);
        var y = Math.Abs(Y - b.Y);

        return x <= 1 && y <= 1;
    }
}

internal class RopeBridge : ISolution
{
    public void Run(EntryPoint entryPoint)
    {
        var moves = File.ReadAllLines(entryPoint.InputPath).Select(ParseLine).ToArray();

        var shortRope = Enumerable.Repeat(new Point(0, 0), 2).ToArray();
        var longRope = Enumerable.Repeat(new Point(0, 0), 10).ToArray();

        var visitedInShort = FindPositionsVisitedByTail(shortRope, moves);
        Console.WriteLine($"Tail of the short rope visited {visitedInShort.Count} unique positions.");

        var visitedInLong = FindPositionsVisitedByTail(longRope, moves);
        Console.WriteLine($"Tail of the long rope visited {visitedInLong.Count} unique positions.");
    }

    private static HashSet<Point> FindPositionsVisitedByTail(Point[] rope, IEnumerable<Move> moves)
    {
        var visited = new HashSet<Point> { rope[1] };

        foreach (var move in moves)
        {
            for (int i = 0; i < move.Length; i++)
            {
                rope[0] = MoveInDirection(rope[0], move.Direction);

                for (int ropeNode = 1; ropeNode < rope.Length; ropeNode++)
                {
                    if (!rope[ropeNode].Touch(rope[ropeNode - 1]))
                    {
                        rope[ropeNode] = MoveTowards(rope[ropeNode], rope[ropeNode - 1]);
                    }
                }

                visited.Add(rope[^1]);
            }
        }

        return visited;
    }

    private static Point MoveInDirection(Point point, Direction direction)
    {
        return direction switch
        {
            Direction.Up => point with { Y = point.Y + 1 },
            Direction.Right => point with { X = point.X + 1 },
            Direction.Down => point with { Y = point.Y - 1 },
            Direction.Left => point with { X = point.X - 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private static Point MoveTowards(Point point, Point towards)
    {
        var xDif = Math.Sign(point.X - towards.X);
        var yDif = Math.Sign(point.Y - towards.Y);

        return new Point(X: point.X - xDif, Y: point.Y - yDif);
    }

    private static Move ParseLine(string l)
    {
        var split = l.Split(' ');
        var direction = split[0] switch
        {
            "U" => Direction.Up,
            "R" => Direction.Right,
            "D" => Direction.Down,
            "L" => Direction.Left,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new Move(direction, int.Parse(split[1]));
    }
}