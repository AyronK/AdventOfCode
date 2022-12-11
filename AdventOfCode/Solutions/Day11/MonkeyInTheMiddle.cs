using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class MonkeyInTheMiddle : ISolution
{
    delegate long InspectItem(long item);

    delegate int GetThrowTarget(long item);

    class Monkey
    {
        private readonly InspectItem _inspect;

        public Monkey(Queue<long> items, GetThrowTarget throwTo, InspectItem inspect, long divisableBy)
        {
            Items = items;
            ThrowTo = throwTo;
            DivisableBy = divisableBy;
            _inspect = inspect;
        }

        public Queue<long> Items { get; }
        public GetThrowTarget ThrowTo { get; }
        public long Inspected { get; private set; } = 0;
        public long DivisableBy { get; }

        public InspectItem Inspect => item =>
        {
            Inspected++;
            return _inspect(item);
        };
    };

    public void Run(EntryPoint entryPoint)
    {
        int numberOfRounds = 20;

        // Part 1
        var monkeys = GetMonkeys(File.ReadAllLines(entryPoint.InputPath)).ToArray();

        Start(monkeys, i => i / 3, numberOfRounds);
        Console.WriteLine($"After {numberOfRounds} rounds:");
        Display(monkeys);

        // Part 2
        numberOfRounds = 10000;

        monkeys = GetMonkeys(File.ReadAllLines(entryPoint.InputPath)).ToArray();
        long leastCommonMultiple = FindLeastCommonMultiple(monkeys.Select(monkey => monkey.DivisableBy).ToArray());
        Start(monkeys, i => i % leastCommonMultiple, numberOfRounds);

        Console.WriteLine($"After {numberOfRounds} rounds:");
        Display(monkeys);
    }

    private static void Start(Monkey[] monkeys, Func<long, long> newValue, int numberOfRounds)
    {
        for (int round = 1; round <= numberOfRounds; round++)
        {
            foreach (var monkey in monkeys)
            {
                var itemsCount = monkey.Items.Count;

                for (int itemIndex = 0; itemIndex < itemsCount; itemIndex++)
                {
                    long newItemValue = monkey.Items.Dequeue();
                    newItemValue = monkey.Inspect(newItemValue);

                    newItemValue = newValue(newItemValue);
                    monkeys[monkey.ThrowTo(newItemValue)].Items.Enqueue(newItemValue);
                }
            }
        }
    }

    private static void Display(Monkey[] monkeys)
    {
        for (var index = 0; index < monkeys.Length; index++)
        {
            var monkey = monkeys[index];
            Console.WriteLine($"Monkey {index} inspected items {monkey.Inspected} times.");
        }

        var top = monkeys.OrderByDescending(m => m.Inspected).Take(2).ToArray();
        Console.WriteLine($"Monkey business is {top[0].Inspected * top[1].Inspected}");
    }

    private static class StringRanges
    {
        public static Range StartingItems => 18..;
        public static Range Operation => 18..;
        public static Range DivisableBy => 21..;
        public static Range IfTrue => 29..;
        public static Range IfFalse => 30..;
    }


    private static IEnumerable<Monkey> GetMonkeys(string[] input)
    {
        for (int i = 0; i < input.Length; i += 7)
        {
            var items = input[i + 1][StringRanges.StartingItems].Split(',', StringSplitOptions.TrimEntries).Select(long.Parse);
            var operationFormula = input[i + 2][StringRanges.Operation].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var divisibleBy = int.Parse(input[i + 3][StringRanges.DivisableBy]);
            var ifTrue = int.Parse(input[i + 4][StringRanges.IfTrue]);
            var ifFalse = int.Parse(input[i + 5][StringRanges.IfFalse]);

            yield return new Monkey(
                new Queue<long>(items),
                item => item % divisibleBy == 0 ? ifTrue : ifFalse,
                ParseOperation(operationFormula),
                divisibleBy);
        }
    }

    private static InspectItem ParseOperation(string[] operationFormula)
    {
        return operationFormula[1] switch
        {
            "*" when operationFormula[2] == "old" => item => item * item,
            "*" => item => item * int.Parse(operationFormula[2]),
            "+" when operationFormula[2] == "old" => item => item + item,
            "+" => item => item + int.Parse(operationFormula[2]),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private static long FindLeastCommonMultiple(long[] numbers)
    {
        return numbers.Aggregate((source, curr) => source * curr / FindGreatestCommonDivision(source, curr));
    }

    private static long FindGreatestCommonDivision(long a, long b)
    {
        return b == 0 ? a : FindGreatestCommonDivision(b, a % b);
    }
}