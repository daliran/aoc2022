namespace Day12
{
    class HeightMapSquare : IEquatable<HeightMapSquare>
    {
        public char HeightLetter { get; }
        public int Height => HeightLetter - 'a';

        public bool IsStart { get; }
        public bool IsDestination { get; }

        public int Row { get; }
        public int Column { get; }

        private readonly List<HeightMapSquare> _reachableSquares = new();

        public IReadOnlyList<HeightMapSquare> ReachableSquares => _reachableSquares;

        public HeightMapSquare(char letter, int row, int column)
        {
            Row = row;
            Column = column;

            if (letter == 'S')
            {
                IsStart = true;
                HeightLetter = 'a';
            }
            else if (letter == 'E')
            {
                IsDestination = true;
                HeightLetter = 'z';
            }
            else
            {
                HeightLetter = letter;
            }
        }

        public void CalculateReachableSquares(IReadOnlyList<IReadOnlyList<HeightMapSquare>> grid)
        {
            if (Row > 0)
            {
                var topSquare = grid[Row - 1][Column];

                if (IsReachable(topSquare))
                {
                    _reachableSquares.Add(topSquare);
                }
            }

            if (Row < grid.Count - 1)
            {
                var bottomSquare = grid[Row + 1][Column];

                if (IsReachable(bottomSquare))
                {
                    _reachableSquares.Add(bottomSquare);
                }
            }

            if (Column > 0)
            {
                var leftSquare = grid[Row][Column - 1];

                if (IsReachable(leftSquare))
                {
                    _reachableSquares.Add(leftSquare);
                }
            }

            // Each row has the same number of columns
            if (Column < grid[0].Count - 1)
            {
                var rightSquare = grid[Row][Column + 1];

                if (IsReachable(rightSquare))
                {
                    _reachableSquares.Add(rightSquare);
                }
            }
        }

        private bool IsReachable(HeightMapSquare square)
        {
            // If lower = reachable
            if (square.Height <= Height)
            {
                return true;
            }

            // If 1 unit higher = reachable
            if (square.Height == Height + 1)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"{HeightLetter} ({Row},{Column})";
        }

        public bool Equals(HeightMapSquare? other)
        {
            if (other == null)
            {
                return false;
            }

            return Row == other.Row && Column == other.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as HeightMapSquare);
        }
    }

    record QueueItem(HeightMapSquare Square, int Depth);

    class PathFinder
    {

        private readonly HashSet<HeightMapSquare> _visitedSquares = new();

        private readonly Queue<QueueItem> _queue = new();

        public PathFinder(IReadOnlyCollection<HeightMapSquare> initialSquares)
        {
            foreach(var square in initialSquares)
            {
                _queue.Enqueue(new QueueItem(square, 0));
            } 
        }

        public QueueItem? ExecuteStep()
        {
            var current = _queue.Dequeue();

            if (!_visitedSquares.Contains(current.Square))
            {
                _visitedSquares.Add(current.Square);
            }
            else
            {
                return null;
            }

            if (current.Square.IsDestination)
            {
                return current;
            }

            foreach (var reachableSquare in current.Square.ReachableSquares)
            {
                if (!_visitedSquares.Contains(reachableSquare))
                {
                    _queue.Enqueue(new QueueItem(reachableSquare, current.Depth + 1));
                }       
            }

            return null;
        }
    }

    internal class Program
    {
        static IReadOnlyList<IReadOnlyList<HeightMapSquare>> ParseInput()
        {
            List<List<HeightMapSquare>> heightMapSquares = new();

            int rowCounter = 0;

            foreach (var line in File.ReadLines("input.txt"))
            {
                List<HeightMapSquare> row = new();

                int columnCounter = 0;

                foreach (var character in line)
                {
                    row.Add(new HeightMapSquare(character, rowCounter, columnCounter));
                    columnCounter++;
                }

                heightMapSquares.Add(row);

                rowCounter++;
            }

            // Calculate all the reachable square for each square
            foreach (var heightMapRow in heightMapSquares)
            {
                foreach (var heightMapColumn in heightMapRow)
                {
                    heightMapColumn.CalculateReachableSquares(heightMapSquares);
                }
            }

            return heightMapSquares;
        }

        static void Main(string[] args)
        {
            // 0,0 is the top left
            // First dimension = row, second dimension = column
            var heightMap = ParseInput();

            var initialSquares = heightMap.SelectMany(a => a).Where(a => a.Height == 0).ToList();

            var pathFinder = new PathFinder(initialSquares);

            QueueItem? reachedDestination = null;

            while (reachedDestination == null)
            {
                reachedDestination = pathFinder.ExecuteStep();
            }
       
            Console.WriteLine($"Steps: {reachedDestination.Depth}");
        }
    }
}