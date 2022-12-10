namespace Day10
{
    record Instruction(int CyclesToConsume, int ValueToAdd);

    class ProgramState
    {
        public int CurrentCyle { get; private set; } = 0;
        public int XRegister { get; private set; } = 1;

        // Columns, rows
        private char[,] _screen = new char[40, 6];

        private readonly List<int> _signalStrengths = new();
        public IReadOnlyCollection<int> SignalStrengths => _signalStrengths;

        public void ExecuteInstruction(Instruction instruction)
        {
            for (int i = 0; i < instruction.CyclesToConsume; i++)
            {
                ExecuteCycle();
            }

            // The register is increased after consuming all the cycles
            XRegister += instruction.ValueToAdd;
        }

        private void ExecuteCycle()
        {
            // The signal is calculated with the 1 based cycle instead of the 0 based cycle
            var cyclePlusOne = CurrentCyle + 1;

            if (cyclePlusOne == 20 || (cyclePlusOne - 20) % 40 == 0 && cyclePlusOne < 221)
            {
                _signalStrengths.Add(XRegister * cyclePlusOne);
            }

            // The draw is executed every cycle
            Draw();

            CurrentCyle++;
        }

        private void Draw()
        {
            // The position of the draw is controlled by the cycle

            int columnCoordinate = CurrentCyle % 40; // 0-39 = 40 columns
            int rowCoordinate = CurrentCyle / 40; // 0-5 = 6 rows

            int spriteCenterPosition = XRegister;
            int currentCycleRelativePosition = CurrentCyle % 40;

            if (spriteCenterPosition == currentCycleRelativePosition || spriteCenterPosition - 1 == currentCycleRelativePosition || spriteCenterPosition + 1 == currentCycleRelativePosition)
            {
                _screen[columnCoordinate, rowCoordinate] = '#';
            }
            else
            {
                _screen[columnCoordinate, rowCoordinate] = '.';
            }
        }

        public void PrintScreen()
        {
            for (int i = 0; i < _screen.GetLength(1); i++)
            {
                for (int j = 0; j < _screen.GetLength(0); j++)
                {
                    Console.Write(_screen[j, i]);
                }

                Console.Write("\n");
            }
        }
    }

    internal class Program
    {
        static IReadOnlyList<Instruction> ParseInput()
        {
            List<Instruction> instructions = new();

            foreach (var line in File.ReadLines("input.txt"))
            {
                var split = line.Split(" ");

                if (split[0] == "noop")
                {
                    instructions.Add(new Instruction(1, 0));
                }
                else if (split[0] == "addx")
                {
                    instructions.Add(new Instruction(2, Convert.ToInt32(split[1])));
                }
            }

            return instructions;
        }

        static void Main(string[] args)
        {
            var instructions = ParseInput();

            var programState = new ProgramState();

            foreach (var instruction in instructions)
            {
                programState.ExecuteInstruction(instruction);
            }

            var totalStength = programState.SignalStrengths.Sum();

            Console.WriteLine($"Total Stenght: {totalStength}");

            programState.PrintScreen();
        }
    }
}