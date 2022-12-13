using System.Diagnostics;

namespace Day11
{
    enum MonkeyWorryLevelOperationType
    {
        Mul,
        Add
    }

    record MonkeyWorryLevelOperation(MonkeyWorryLevelOperationType Type, int? OperationValue);

    record MonkeyItemTest(int TestDivisor, int MokeyIdWhenTrue, int MonkeyIdWhenFalse);

    class KeepAwayState
    {
        public int RoundNumber { get; private set; }
        public int Mcd { get; }
        public IReadOnlyCollection<Monkey> Monkeys { get; }

        public KeepAwayState(IReadOnlyCollection<Monkey> monkeys, int mcd)
        {
            Monkeys = monkeys;
            Mcd = mcd;
        }

        public void ExecuteRound()
        {
            // Monkey turn
            foreach (var monkey in Monkeys)
            {
                // Throw all the items in the inventory one by one
                while (monkey.Inventory.Count > 0)
                {
                    (int, Item)? inspectionResult;

                    inspectionResult = monkey.InspectItemPart(Mcd);

                    if (inspectionResult != null)
                    {
                        var destinationMonkey = Monkeys.First(a => a.Id == inspectionResult.Value.Item1);
                        destinationMonkey.ReceiveItem(inspectionResult.Value.Item2);
                    }
                }
            }

            RoundNumber++;
        }
    }

    class Monkey
    {
        public int Id { get; }
        private readonly Queue<Item> _inventory = new();

        public IReadOnlyCollection<Item> Inventory => _inventory;

        public ulong Inspections { get; private set; }

        public MonkeyWorryLevelOperation WorryLevelOperation { get; }
        public MonkeyItemTest ItemTest { get; }

        public Monkey(
            int id,
            MonkeyWorryLevelOperation worryLevelOperation,
            MonkeyItemTest itemTest,
            IReadOnlyCollection<Item> initialInventory
            )
        {
            Id = id;
            WorryLevelOperation = worryLevelOperation;
            ItemTest = itemTest;

            foreach (Item item in initialInventory)
            {
                _inventory.Enqueue(item);
            }
        }

        public override string ToString()
        {
            return Inspections.ToString();
        }

        public void ReceiveItem(Item item)
        {
            _inventory.Enqueue(item);
        }

        public (int, Item)? InspectItemPart(int mcd)
        {
            if (_inventory.Count == 0)
            {
                return null;
            }

            var item = _inventory.Dequeue();

            var newWorryLevel = CalculateNewWorryLevelPart(item, mcd);

            item.WorryLevel = newWorryLevel;

            var newMoneyId = DecideWhoToThrowToPart(item);

            Inspections++;

            return (newMoneyId, item);
        }

        private ulong CalculateNewWorryLevelPart(Item item, int mcd)
        {
            // The old value is encoded as -1
            ulong value = WorryLevelOperation.OperationValue != null ? (ulong)WorryLevelOperation.OperationValue.Value : item.WorryLevel;

            var newWorryLevel = WorryLevelOperation.Type switch
            {
                MonkeyWorryLevelOperationType.Add => item.WorryLevel + value,
                MonkeyWorryLevelOperationType.Mul => (item.WorryLevel * value),
                _ => throw new NotSupportedException(),
            };

            // For part1
            //Divided by 3 and rounded for part one. The value was double but now is long for part two.
            //newWorryLevel /= 3.0;
            //return (int)Math.Floor(newWorryLevel);

            return newWorryLevel % (ulong)mcd;
        }

        private int DecideWhoToThrowToPart(Item item)
        {
            // Assume the test is always a division
            if (item.WorryLevel % (ulong)ItemTest.TestDivisor == 0)
            {
                return ItemTest.MokeyIdWhenTrue;
            }
            else
            {
                return ItemTest.MonkeyIdWhenFalse;
            }
        }

    }

    class Item
    {
        public ulong WorryLevel { get; set; }

        public Item(int initialWorryLevel)
        {
            WorryLevel = (ulong)initialWorryLevel;
        }
    }

    internal class Program
    {
        static IReadOnlyList<Monkey> ParseInput()
        {
            List<Monkey> monkeys = new();

            // 7 lines = 1 monkey
            foreach (var lines in File.ReadLines("input.txt").Chunk(7))
            {
                var monkeyId = Convert.ToInt32(lines[0].Trim().Split()[1].Replace(":", string.Empty));
                var items = lines[1].Trim().Split().Skip(2).Select(a => new Item(Convert.ToInt32(a.Trim().Replace(",", string.Empty)))).ToArray();

                var line2Split = lines[2].Trim().Split();

                var operation = line2Split[4] switch
                {
                    "*" => MonkeyWorryLevelOperationType.Mul,
                    "+" => MonkeyWorryLevelOperationType.Add,
                    _ => throw new NotImplementedException()
                };

                var valueRaw = line2Split[5];

                var value = line2Split[5] == "old" ? (int?)null : Convert.ToInt32(line2Split[5]);

                var divisor = Convert.ToInt32(lines[3].Trim().Split()[3]);
                var trueCondition = Convert.ToInt32(lines[4].Trim().Split()[5]);
                var falseCondition = Convert.ToInt32(lines[5].Trim().Split()[5]);


                monkeys.Add(new Monkey(monkeyId, new MonkeyWorryLevelOperation(operation, value), new MonkeyItemTest(divisor, trueCondition, falseCondition), items));
            }

            return monkeys;
        }

        static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();

            var monkeys = ParseInput();

            Console.WriteLine($"Input parsed in in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Restart();

            var mcd = monkeys.Select(a => a.ItemTest.TestDivisor).Aggregate((total, current) => total * current);

            var state = new KeepAwayState(monkeys, mcd);

            var rounds = 10000;

            for (int i = 0; i < rounds; i++)
            {
                state.ExecuteRound();
            }

            var topTwoActiveMonkeys = state.Monkeys.OrderByDescending(a => a.Inspections).Take(2);

            var monkeyBusiness = topTwoActiveMonkeys.Select(a => a.Inspections).Aggregate((ulong total, ulong current) => total * current);

            Console.WriteLine($"The level of monkey business for {rounds} rounds is: {monkeyBusiness}, calculated in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Stop();
        }
    }
}