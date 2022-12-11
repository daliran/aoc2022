using System.Diagnostics;
using System.Numerics;

namespace Day11
{
    enum MonkeyWorryLevelOperationType
    {
        Mul,
        Add
    }

    record MonkeyWorryLevelOperation(MonkeyWorryLevelOperationType Type, uint? OperationValue);

    record MonkeyItemTest(uint TestDivisor, uint MokeyIdWhenTrue, uint MonkeyIdWhenFalse);

    class KeepAwayState
    {
        public int RoundNumber { get; private set; }
        public IReadOnlyCollection<Monkey> Monkeys { get; }

        public KeepAwayState(IReadOnlyCollection<Monkey> monkeys)
        {
            Monkeys = monkeys;
        }

        public void ExecuteRound(bool includeRelief)
        {
            // Monkey turn
            foreach (var monkey in Monkeys)
            {
                // Throw all the items in the inventory one by one
                while (monkey.Inventory.Count > 0)
                {
                    (uint, Item)? inspectionResult;

                    if (includeRelief)
                    {
                        inspectionResult = monkey.InspectItemPart1();
                    }
                    else
                    {
                        inspectionResult = monkey.InspectItemPart2();
                    }

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
        public uint Id { get; }
        private readonly Queue<Item> _inventory = new();

        public IReadOnlyCollection<Item> Inventory => _inventory;

        public ulong Inspections { get; private set; }

        private readonly MonkeyWorryLevelOperation _worryLevelOperation;
        private readonly MonkeyItemTest _itemTest;

        public Monkey(
            uint id,
            MonkeyWorryLevelOperation worryLevelOperation,
            MonkeyItemTest itemTest,
            IReadOnlyCollection<Item> initialInventory
            )
        {
            Id = id;
            _worryLevelOperation = worryLevelOperation;
            _itemTest = itemTest;

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

        #region Part1

        public (uint, Item)? InspectItemPart1()
        {
            if (_inventory.Count == 0)
            {
                return null;
            }

            var item = _inventory.Dequeue();

            var newWorryLevel = CalculateNewWorryLevelPart1(item);

            item.WorryLevel = newWorryLevel;

            var newMoneyId = DecideWhoToThrowToPart1(item);

            Inspections++;

            return (newMoneyId, item);
        }

        private uint CalculateNewWorryLevelPart1(Item item)
        {
            // The old value is encoded as -1
            uint value = _worryLevelOperation.OperationValue != null ? _worryLevelOperation.OperationValue.Value : item.WorryLevel;

            var newWorryLevel = _worryLevelOperation.Type switch
            {
                MonkeyWorryLevelOperationType.Add => item.WorryLevel + value,
                MonkeyWorryLevelOperationType.Mul => (double)(item.WorryLevel * value),
                _ => throw new NotSupportedException(),
            };

            //Divided by 3 and rounded for part one. The value was double but now is long for part two.
            newWorryLevel /= 3.0;
            return (uint)Math.Floor(newWorryLevel);
        }

        private uint DecideWhoToThrowToPart1(Item item)
        {
            // Assume the test is always a division
            if (item.WorryLevel % _itemTest.TestDivisor == 0)
            {
                return _itemTest.MokeyIdWhenTrue;
            }
            else
            {
                return _itemTest.MonkeyIdWhenFalse;
            }
        }

        #endregion

        #region Part 2

        public (uint, Item)? InspectItemPart2()
        {
            if (_inventory.Count == 0)
            {
                return null;
            }

            var item = _inventory.Dequeue();

            var newMoneyId = DecideWhoToThrowToPart2(item);

            Inspections++;

            return (newMoneyId, item);
        }

        // TODO: there are bugs to fix
        private uint DecideWhoToThrowToPart2(Item item)
        {
            BigInteger modA;

            if (item.ModWorryLevels.ContainsKey(_itemTest.TestDivisor))
            {
                modA = item.ModWorryLevels[_itemTest.TestDivisor];
            }
            else
            {
                modA = item.WorryLevel % _itemTest.TestDivisor;
                item.ModWorryLevels.Add(_itemTest.TestDivisor, modA);
            }

            var mobB = _worryLevelOperation.OperationValue != null ? new BigInteger(_worryLevelOperation.OperationValue.Value % _itemTest.TestDivisor) : modA;

            if (_worryLevelOperation.Type == MonkeyWorryLevelOperationType.Add)
            {
                var modC = (modA + mobB) % _itemTest.TestDivisor;
                item.ModWorryLevels[_itemTest.TestDivisor] = modC;

                if (modC == 0)
                {
                    return _itemTest.MokeyIdWhenTrue;
                }
                else
                {
                    return _itemTest.MonkeyIdWhenFalse;
                }

            }
            else if (_worryLevelOperation.Type == MonkeyWorryLevelOperationType.Mul)
            {
                var modC = (modA * mobB) % _itemTest.TestDivisor;
                item.ModWorryLevels[_itemTest.TestDivisor] = modC;

                if (modC == 0)
                {
                    return _itemTest.MokeyIdWhenTrue;
                }
                else
                {
                    return _itemTest.MonkeyIdWhenFalse;
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        #endregion
    }

    class Item
    {
        // Modified in part 1, read only in part 2
        public uint WorryLevel { get; set; }

        // Used only by part 2
        public Dictionary<uint, BigInteger> ModWorryLevels { get; } = new();

        public Item(uint initialWorryLevel)
        {
            WorryLevel = initialWorryLevel;
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
                var monkeyId = Convert.ToUInt32(lines[0].Trim().Split()[1].Replace(":", string.Empty));
                var items = lines[1].Trim().Split().Skip(2).Select(a => new Item(Convert.ToUInt32(a.Trim().Replace(",", string.Empty)))).ToArray();

                var line2Split = lines[2].Trim().Split();

                var operation = line2Split[4] switch
                {
                    "*" => MonkeyWorryLevelOperationType.Mul,
                    "+" => MonkeyWorryLevelOperationType.Add,
                    _ => throw new NotImplementedException()
                };

                var valueRaw = line2Split[5];

                var value = line2Split[5] == "old" ? (uint?)null : Convert.ToUInt32(line2Split[5]);

                var divisor = Convert.ToUInt32(lines[3].Trim().Split()[3]);
                var trueCondition = Convert.ToUInt32(lines[4].Trim().Split()[5]);
                var falseCondition = Convert.ToUInt32(lines[5].Trim().Split()[5]);


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

            var state = new KeepAwayState(monkeys);

            var rounds = 20;
            bool includeRelief = false;

            for (int i = 0; i < rounds; i++)
            {
                state.ExecuteRound(includeRelief);
            }

            var topTwoActiveMonkeys = state.Monkeys.OrderByDescending(a => a.Inspections).Take(2);

            var monkeyBusiness = topTwoActiveMonkeys.Select(a => a.Inspections).Aggregate((ulong total, ulong current) => total * current);

            Console.WriteLine($"The level of monkey business for {rounds} rounds is: {monkeyBusiness}, calculated in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Stop();
        }
    }
}