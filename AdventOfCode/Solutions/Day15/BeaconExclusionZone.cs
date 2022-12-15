using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions.Day15;

internal class BeaconExclusionZone : ISolution
{
    record Point(int X, int Y);

    public void Run(EntryPoint entryPoint)
    {
        var input = File.ReadAllLines(entryPoint.InputPath).Select(l =>
        {
            l = new string(l.SkipWhile(c => c != '=').ToArray());
            var x = int.Parse(l[1..l.IndexOf(',')]);
            l = new string(l.Skip(l.IndexOf(',')).SkipWhile(c => c != '=').ToArray());
            var y = int.Parse(l[1..l.IndexOf(':')]);
            l = new string(l.Skip(l.IndexOf(':')).SkipWhile(c => c != '=').ToArray());
            var x2 = int.Parse(l[1..l.IndexOf(',')]);
            l = new string(l.Skip(l.IndexOf(',')).SkipWhile(c => c != '=').ToArray());
            var y2 = int.Parse(l[1..]);

            var sensor = new Point(x, y);
            var beacon = new Point(x2, y2);
            return (
                Sensor: sensor,
                Beacon: beacon, Distance: ManhattanDistance(sensor, beacon));
        }).ToArray();

        int rowToCheck = 2000000;

        int maxDistance = input.Select(i => ManhattanDistance(i.Beacon, i.Sensor)).MaxBy(d => d);
        int xMin = Math.Min(input.Min(c => c.Sensor.X), input.Min(c => c.Beacon.X)) - maxDistance;
        int xMax = Math.Max(input.Max(c => c.Sensor.X), input.Max(c => c.Beacon.X)) + maxDistance;

        int count = 0;

        HashSet<Point> beacons = new(input.Select(c => c.Beacon));

        for (int x = xMin; x <= xMax; x++)
        {
            var currentPoint = new Point(x, rowToCheck);

            if (!beacons.Contains(currentPoint) &&
                input.Any(e => e.Distance >= ManhattanDistance(e.Sensor, currentPoint)))
            {
                count++;
            }
        }

        Console.WriteLine(count);
    }

    static int ManhattanDistance(Point a, Point b)
    {
        var sub = Subtract(a, b);
        return Math.Abs(sub.X) + Math.Abs(sub.Y);
    }

    static Point Subtract(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }
}