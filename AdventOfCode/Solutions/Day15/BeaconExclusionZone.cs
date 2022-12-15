using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions.Day15;

internal class BeaconExclusionZone : ISolution
{
    private record Point(int X, int Y);

    private record Range(Point Start, Point End)
    {
        public bool Covers(int x) => Start.X == x || End.X == x || (Start.X < x && End.X > x);
    };

    private record Area(Point Sensor, Point Beacon, int Radius)
    {
        public Range? Covers(int y)
        {
            var diameter = 2 * Radius + 1;
            var yOffset = Math.Abs(y - Sensor.Y);

            if (yOffset > Radius)
            {
                return null;
            }

            var halfChord = (diameter - 2 * yOffset) / 2;
            return new Range(new Point(Sensor.X - halfChord, y), new Point(Sensor.X + halfChord, y));
        }
    };

    public void Run(EntryPoint entryPoint)
    {
        var mapOfSignal = File.ReadAllLines(entryPoint.InputPath).Select(ParseInput).OrderBy(c => c.Sensor.X).ToArray();

        int rowToCheck = 2_000_000;
        int searchThreshold = 4_000_000;
        
        var blockedInRowToCheck = CountBlocked(mapOfSignal, rowToCheck);
        var tuningFrequency = FindTuningFrequency(mapOfSignal, searchThreshold);

        Console.WriteLine($"In row {rowToCheck} there are {blockedInRowToCheck} positions cannot contain a beacon.");
        Console.WriteLine($"Tuning frequency for distressed beacon is equal to {tuningFrequency}.");
    }

    private Area ParseInput(string l)
    {
        var values = l.Split(':', '=', ',').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
        var sensor = new Point(values[0], values[1]);
        var beacon = new Point(values[2], values[3]);
        var radius = ManhattanDistance(new Point(values[0], values[1]), new Point(values[2], values[3]));
        return new Area(sensor, beacon, radius);
    }

    private static int CountBlocked(Area[] areas, int rowToCheck)
    {
        var blockedInRowToCheck = 0;

        HashSet<Point> beacons = new(areas.Select(c => c.Beacon));
        var xMin = areas.Min(c => c.Sensor.X - c.Radius);
        var xMax = areas.Max(c => c.Sensor.X + c.Radius);

        var rangesForRowToCheck = GetCoverRanges(areas, rowToCheck);

        for (var x = xMin; x <= xMax; x++)
        {
            if (!beacons.Contains(new Point(x, rowToCheck)) && rangesForRowToCheck.Any(r => r.Covers(x)))
            {
                blockedInRowToCheck++;
            }
        }

        return blockedInRowToCheck;
    }

    private static long FindTuningFrequency(Area[] areas, int searchThreshold)
    {
        Point? result = null;
        Parallel.For(0, searchThreshold, (y, ctx) =>
        {
            var ranges = GetCoverRanges(areas, y);
            var prev = 0;

            foreach (var (start, end) in ranges)
            {
                if (start.X > prev + 1 && end.X > prev + 1)
                {
                    var point = new Point(prev + 1, y);

                    if (IsValidDistressBeacon(point, searchThreshold))
                    {
                        result = point;
                        ctx.Break();
                    }
                }

                prev = Math.Max(prev, end.X);
            }
        });

        if (result is null)
        {
            throw new InvalidOperationException("Cannot find tuning frequency for given input");
        }

        return GetTuningFrequency(result);
    }

    private static Range[] GetCoverRanges(Area[] input, int rowToCheck)
    {
        IEnumerable<Range> AccumulateRanges(IEnumerable<Range> prev, Area curr) => curr.Covers(rowToCheck) is { } r ? prev.Append(r) : prev;
        return input.Aggregate(Enumerable.Empty<Range>(), AccumulateRanges).OrderBy(c => c.Start.X).ToArray();
    }

    private static long GetTuningFrequency(Point a) => checked((long)a.X * 4000000 + a.Y);

    private static bool IsValidDistressBeacon(Point a, int threshold)
    {
        return a.X >= 0 && a.X <= threshold && a.Y >= 0 && a.Y <= threshold;
    }

    private static int ManhattanDistance(Point a, Point b)
    {
        var sub = Subtract(a, b);
        return Math.Abs(sub.X) + Math.Abs(sub.Y);
    }

    private static Point Subtract(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }
}