using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class MonkeyInTheMiddle : ISolution
{
    delegate void InspectItem(ref int item);

    delegate int GetThrowTarget(int item);

    class Monkey
    {
        private readonly InspectItem _inspect;

        public Monkey(Queue<int> items, GetThrowTarget throwTo, InspectItem inspect)
        {
            Items = items;
            ThrowTo = throwTo;
            _inspect = inspect;
        }

        public Queue<int> Items { get; init; }
        public GetThrowTarget ThrowTo { get; init; }
        public int Inspected { get; set; } = 0;

        public InspectItem Inspect => (ref int item) =>
        {
            _inspect(ref item);
            Inspected++;
        };
    };

    public void Run(EntryPoint entryPoint)
    {
        var monkeys = GetMonkeys(File.ReadAllLines(entryPoint.InputPath)).ToArray();

        for (int round = 1; round <= 20; round++)
        {
            foreach (var monkey in monkeys)
            {
                var itemsCount = monkey.Items.Count;

                for (int itemIndex = 0; itemIndex < itemsCount; itemIndex++)
                {
                    int newItemValue = monkey.Items.Dequeue();
                    monkey.Inspect(ref newItemValue);
                    newItemValue = (int)Math.Floor(newItemValue / 3.0);
                    monkeys[monkey.ThrowTo(newItemValue)].Items.Enqueue(newItemValue);
                }
            }
        }

        for (var index = 0; index < monkeys.Length; index++)
        {
            var monkey = monkeys[index];
            Console.WriteLine($"Monkey {index} inspected items {monkey.Inspected} times.");
        }

        var top = monkeys.OrderByDescending(m => m.Inspected).Take(2).ToArray();
        Console.WriteLine($"Monkey business after 20 rounds it {top[0].Inspected * top[1].Inspected}");
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
            var items = input[i + 1][StringRanges.StartingItems].Split(',', StringSplitOptions.TrimEntries).Select(int.Parse);
            var operationFormula = input[i + 2][StringRanges.Operation].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var divisibleBy = int.Parse(input[i + 3][StringRanges.DivisableBy]);
            var ifTrue = int.Parse(input[i + 4][StringRanges.IfTrue]);
            var ifFalse = int.Parse(input[i + 5][StringRanges.IfFalse]);

            yield return new Monkey(
                new Queue<int>(items),
                item => item % divisibleBy == 0 ? ifTrue : ifFalse,
                (ref int item) => InspectOperation(ref item, operationFormula)
            );
        }
    }

    private static void InspectOperation(ref int item, string[] operationFormula)
    {
        item = operationFormula[1] switch
        {
            "*" when operationFormula[2] == "old" => item * item,
            "*" => item * int.Parse(operationFormula[2]),
            "+" when operationFormula[2] == "old" => item + item,
            "+" => item + int.Parse(operationFormula[2]),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}