using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions.Day15;

internal class BeaconExclusionZone : ISolution
{
    record Point(int X, int Y);

    record Area(Point Sensor, Point Beacon, int Distance);

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
            var distance = ManhattanDistance(sensor, beacon);
            return new Area(sensor, beacon, distance);
        }).OrderBy(c => c.Sensor.X).ToArray();

        int rowToCheck = 2000000;
        int threshold = 4000000;

        int xMin = input.Min(c => c.Sensor.X - c.Distance);
        int xMax = input.Max(c => c.Sensor.X + c.Distance);

        int count = 0;

        HashSet<Point> beacons = new(input.Select(c => c.Beacon));
        HashSet<Point> possibleBeacons = new();

        var searchYMin = Math.Min(rowToCheck, 0);
        var searchYMax = Math.Max(rowToCheck, threshold);

        for (int y = searchYMin; y <= searchYMax; y++)
        {
            var searchMinX = y == rowToCheck ? xMin : 0;
            var searchMaxX = y == rowToCheck ? xMax : threshold;

            for (int x = searchMinX; x <= searchMaxX; x++)
            {
                var currentPoint = new Point(x, y);
                possibleBeacons.Add(currentPoint);

                if (!beacons.Contains(currentPoint) &&
                    input.FirstOrDefault(e => e.Distance >= ManhattanDistance(e.Sensor, currentPoint)) is { } found)
                {
                    if (y == rowToCheck)
                    {
                        count++;
                    }
                    else
                    {
                        var fullDiameter = (2 * found.Distance + 1);
                        var yOffset = Math.Abs(currentPoint.Y - found.Sensor.Y);
                        var amountOfBlockedInRow = fullDiameter - 2 * yOffset;

                        x += (amountOfBlockedInRow / 2) - Math.Abs(currentPoint.X - found.Sensor.X);
                    }

                    possibleBeacons.Remove(currentPoint);
                }
            }
        }

        foreach (var possibleBeacon in possibleBeacons)
        {
            if (!beacons.Contains(possibleBeacon) && IsValidDistressBeacon(possibleBeacon, threshold))
            {
                Console.WriteLine($"{possibleBeacon.X},{possibleBeacon.Y}");
                Console.WriteLine(GetTuningFrequency(possibleBeacon));
            }
        }

        Console.WriteLine(count);
    }

    static long GetTuningFrequency(Point a)
    {
        checked
        {
            return (long)a.X * 4000000 + a.Y;
        }
    }

    static bool IsValidDistressBeacon(Point a, int threshold)
    {
        return a.X >= 0 && a.X <= threshold && a.Y >= 0 && a.Y <= threshold;
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