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
        var moves = File.ReadAllLines(entryPoint.InputPath).Select(ParseLine);

        var head = new Point(0, 0);
        var tail = new Point(0, 0);

        var visited = new HashSet<Point> { tail };

        foreach (var move in moves)
        {
            for (int i = 0; i < move.Length; i++)
            {
                head = MoveInDirection(head, move.Direction);

                if (!tail.Touch(head))
                {
                    tail = MoveTowards(tail, head);
                    visited.Add(tail);
                }
            }
        }

        Console.WriteLine($"Tail rope visited {visited.Count} unique positions.");
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