using System.Text.RegularExpressions;

namespace Day5
{
    record ShipCargo(
        IReadOnlyCollection<CargoStack> CargoStacks,
        IReadOnlyCollection<CraneMovementOperation> CraneMovementOperations
        )
    {
        public void ExecuteCraneMovementsWithCrateMover9000()
        {
            foreach (var craneMovementOperation in CraneMovementOperations)
            {
                var sourceStack = CargoStacks.First(a => a.Index == craneMovementOperation.CargoStackSourceIndex);
                var destinationStack = CargoStacks.First(a => a.Index == craneMovementOperation.CargoStackDestinationIndex);

                for (int i = 0; i < craneMovementOperation.NumberOfCargos; i++)
                {
                    var cargo = sourceStack.Stack.Pop();
                    destinationStack.Stack.Push(cargo);
                }
            }
        }
        public void ExecuteCraneMovementsWithCrateMover9001()
        {
            foreach (var craneMovementOperation in CraneMovementOperations)
            {
                var sourceStack = CargoStacks.First(a => a.Index == craneMovementOperation.CargoStackSourceIndex);
                var destinationStack = CargoStacks.First(a => a.Index == craneMovementOperation.CargoStackDestinationIndex);

                var cargos = new List<string>();

                for (int i = 0; i < craneMovementOperation.NumberOfCargos; i++)
                {
                    cargos.Add(sourceStack.Stack.Pop());
                }

                cargos.Reverse();

                foreach(var cargo in cargos)
                {
                    destinationStack.Stack.Push(cargo);
                }
            }
        }
    }

    record CargoStack(
        int Index,
        Stack<string> Stack
        )
    {
        public string TopCargo => Stack.First();
    }

    record CraneMovementOperation(
        int NumberOfCargos,
        int CargoStackSourceIndex,
        int CargoStackDestinationIndex
        )
    {

    }

    internal class Program
    {
        static ShipCargo ParseInput()
        {
            var file = File.OpenText(@"input.txt");

            List<string> stackInput = new();

            string? lastLine = file.ReadLine();

            // Read cargo stack
            while (!string.IsNullOrEmpty(lastLine))
            {
                stackInput.Add(lastLine);
                lastLine = file.ReadLine();
            }

            // Parse cargo stacks
            IReadOnlyCollection<CargoStack> cargoStacks = ParseCargoStacks(stackInput);

            Regex craneMovementRegex = new(@"^\D+(\d+)\D+(\d+)\D+(\d+)$");

            List<CraneMovementOperation> craneMovementOperations = new();

            // Read crane movement operations
            while (!file.EndOfStream)
            {
                var line = file.ReadLine();

                if (line == null)
                {
                    continue;
                }

                var matchResult = craneMovementRegex.Match(line);

                int numberOfCargos = Convert.ToInt32(matchResult.Groups[1].Value);
                int cargoStackSourceIndex = Convert.ToInt32(matchResult.Groups[2].Value);
                int cargoStackDestinationIndex = Convert.ToInt32(matchResult.Groups[3].Value);

                craneMovementOperations.Add(new(numberOfCargos, cargoStackSourceIndex, cargoStackDestinationIndex));
            }

            return new ShipCargo(cargoStacks, craneMovementOperations);
        }

        static IReadOnlyCollection<CargoStack> ParseCargoStacks(List<string> stackInput)
        {
            int columnsCount = stackInput.Last().Replace(" ", string.Empty).Length;

            List<string> columns = new();

            for (int i = 0; i < columnsCount; i++)
            {
                columns.Add(string.Empty);
            }

            foreach (var row in stackInput)
            {
                for (int i = 0; i < row.Length; i += 4)
                {
                    string temp = row.Substring(i, 3);
                    columns[i / 4] = columns[i / 4] + temp[1];
                }
            }

            List<CargoStack> cargoStacks = new();

            foreach (var column in columns)
            {
                var reversedColumn = column.Reverse().ToArray();
                var index = reversedColumn[0] - '0';

                var stack = new Stack<string>();

                foreach (var cargo in reversedColumn.Skip(1).Where(a => a != ' '))
                {
                    stack.Push(cargo.ToString());
                }

                cargoStacks.Add(new CargoStack(index, stack));
            }

            return cargoStacks;
        }

        static void Main(string[] args)
        {
            ShipCargo shipCargo = ParseInput();

            shipCargo.ExecuteCraneMovementsWithCrateMover9001();
            var topCargos = shipCargo.CargoStacks.Select(a => a.TopCargo).Aggregate((accumulator, next) => accumulator + next);

            Console.WriteLine(topCargos);
        }
    }
}