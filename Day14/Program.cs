namespace Day14
{
    class Simulation
    {
        // origin top left        
        public CavePoint[][] Points { get; }

        public int GeneratedSandUnits { get; private set; }

        public Simulation(CavePoint[][] points)
        {
            Points = points;
        }

        public void StartSimulation(bool enableDebugDrawing = false)
        {
            while (true)
            {
                var sandCell = GenerateSandUnit();

                // Exit point for part 2
                if (sandCell == null)
                {
                    GeneratedSandUnits++;
                    DrawCave();
                    return;
                }

                if (enableDebugDrawing)
                {
                    DrawCave();
                }

                bool isAtRest;
                do
                {
                    var res = MoveSandUnit(sandCell);

                    // Exit point for part 1
                    if (res == null)
                    {
                        DrawCave();
                        return;
                    }

                    isAtRest = res.Value.IsRest;
                    sandCell = res.Value.SandCell;

                    if (!isAtRest && enableDebugDrawing)
                    {
                        DrawCave();
                    }
                }
                while (!isAtRest);

                GeneratedSandUnits++;
            }
        }

        private CavePoint? GenerateSandUnit()
        {
            var sourceOfSandLocation = Points.SelectMany(a => a).Where(a => a.Type == CavePointType.SourceOfSand).First();

            var cellDown = Points[sourceOfSandLocation.Row + 1][sourceOfSandLocation.Column];
            var isDownBlocked = cellDown.Type == CavePointType.Sand || cellDown.Type == CavePointType.Rock;

            if (!isDownBlocked)
            {
                cellDown.ChangeType(CavePointType.Sand);
                return cellDown;
            }

            var cellDownLeft = Points[sourceOfSandLocation.Row + 1][sourceOfSandLocation.Column - 1];
            var isDownLeftBlocked = cellDownLeft.Type == CavePointType.Sand || cellDownLeft.Type == CavePointType.Rock;

            if (!isDownLeftBlocked)
            {
                cellDownLeft.ChangeType(CavePointType.Sand);
                return cellDownLeft;
            }

            var cellDownRight = Points[sourceOfSandLocation.Row + 1][sourceOfSandLocation.Column + 1];
            var isDownRightBlocked = cellDownRight.Type == CavePointType.Sand || cellDownRight.Type == CavePointType.Rock;

            if (!isDownRightBlocked)
            {
                cellDownRight.ChangeType(CavePointType.Sand);
                return cellDownRight;
            }

            // Generator stuck in sand
            sourceOfSandLocation.ChangeType(CavePointType.Sand);
            return null;
        }

        private (bool IsRest, CavePoint SandCell)? MoveSandUnit(CavePoint sand)
        {
            // step down
            var cellDown = Points[sand.Row + 1][sand.Column];
            var isDownBlocked = cellDown.Type == CavePointType.Sand || cellDown.Type == CavePointType.Rock;

            if (!isDownBlocked && IsCellInBoundaries(sand.Row + 1, sand.Column))
            {
                cellDown.ChangeType(CavePointType.Sand);
                sand.ChangeType(CavePointType.Air);
                return (false, cellDown);
            }

            // step down left
            if (!IsCellInBoundaries(sand.Row + 1, sand.Column - 1))
            {
                return null;
            }

            var cellDownLeft = Points[sand.Row + 1][sand.Column - 1];
            var isDownLeftBlocked = cellDownLeft.Type == CavePointType.Sand || cellDownLeft.Type == CavePointType.Rock;

            if (!isDownLeftBlocked)
            {

                cellDownLeft.ChangeType(CavePointType.Sand);
                sand.ChangeType(CavePointType.Air);
                return (false, cellDownLeft);
            }

            // step down right
            if (!IsCellInBoundaries(sand.Row + 1, sand.Column + 1))
            {
                return null;
            }

            var cellDownRight = Points[sand.Row + 1][sand.Column + 1];
            var isDownRightBlocked = cellDownRight.Type == CavePointType.Sand || cellDownRight.Type == CavePointType.Rock;

            if (!isDownRightBlocked)
            {
                cellDownRight.ChangeType(CavePointType.Sand);
                sand.ChangeType(CavePointType.Air);
                return (false, cellDownRight);
            }

            return (true, sand);
        }

        private bool IsCellInBoundaries(int cellRow, int cellColumn)
        {
            return cellRow >= 0 && cellRow < Points.Length && cellColumn >= 0 && cellColumn < Points[0].Length;
        }

        public void DrawCave()
        {
            for (int row = 0; row < Points.Length; row++)
            {
                for (int column = 0; column < Points[row].Length; column++)
                {
                    Console.Write(Points[row][column].TypeSymbol);
                }

                Console.Write('\n');
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }

    enum CavePointType
    {
        Rock,
        Air,
        SourceOfSand,
        Sand
    }

    class CavePoint : IComparable<CavePoint>
    {
        public int X { get; }
        public int Y { get; }
        public int Row { get; private set; }
        public int Column { get; private set; }

        public CavePointType Type { get; private set; }

        public char TypeSymbol => Type switch
        {
            CavePointType.Rock => '#',
            CavePointType.Air => '.',
            CavePointType.SourceOfSand => '+',
            CavePointType.Sand => 'O',
            _ => throw new NotImplementedException()
        };

        public CavePoint(int x, int y, CavePointType type)
        {
            X = x;
            Y = y;
            Type = type;
        }

        public void SetRowColumn(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public void ChangeType(CavePointType type)
        {
            Type = type;
        }

        // Used for sorting purposes
        public int CompareTo(CavePoint? other)
        {
            if (other == null)
            {
                return -1;
            }

            if (Y == other.Y)
            {
                return X - other.X;
            }
            else
            {
                return Y - other.Y;
            }
        }

        public override string ToString()
        {
            return $"{X}, {Y} -> {TypeSymbol}";
        }
    }

    internal class Program
    {
        static CavePoint[][] ParseInput(bool part2 = false, int margin = 10)
        {
            List<CavePoint> points = new()
            {
                 // Add source of sand
                new CavePoint(500, 0, CavePointType.SourceOfSand)
            };

            // Parse and add rocks
            foreach (var line in File.ReadLines("input.txt"))
            {
                var itineraryPointsRaw = line.Split("->");

                var itineraryPoints = new List<(int X, int Y)>();

                // Extract itinerary points
                foreach (var itineraryPointRaw in itineraryPointsRaw)
                {
                    var trimmedItineraryPointRaw = itineraryPointRaw.Trim();
                    var coordinates = trimmedItineraryPointRaw.Split(",");

                    var x = int.Parse(coordinates[0]);
                    var y = int.Parse(coordinates[1]);

                    itineraryPoints.Add((x, y));
                }

                // Create paths based on the itinerary points
                for (int pointIndex = 0; pointIndex < itineraryPoints.Count - 1; pointIndex++)
                {
                    // Draw taking the pairs
                    var point = itineraryPoints[pointIndex];
                    var nextPoint = itineraryPoints[pointIndex + 1];

                    var startX = point.X;
                    var endX = nextPoint.X;

                    var startY = point.Y;
                    var endY = nextPoint.Y;

                    var xDifference = endX - startX;
                    var yDifference = endY - startY;

                    if (xDifference != 0) // draw horizontally
                    {
                        bool oppositeDirection = xDifference < 0;

                        for (int i = 0; i < Math.Abs(xDifference) + 1; i++)
                        {
                            int newX = startX + i * (oppositeDirection ? -1 : 1);
                            int newY = startY;

                            if (points.Any(a => a.X == newX && a.Y == newY))
                            {
                                continue;
                            }

                            points.Add(new CavePoint(newX, newY, CavePointType.Rock));
                        }
                    }
                    else if (yDifference != 0) // draw vertically
                    {
                        bool oppositeDirection = yDifference < 0;

                        for (int i = 0; i < Math.Abs(yDifference) + 1; i++)
                        {
                            int newX = startX;
                            int newY = startY + i * (oppositeDirection ? -1 : 1);

                            if (points.Any(a => a.X == newX && a.Y == newY))
                            {
                                continue;
                            }

                            points.Add(new CavePoint(newX, newY, CavePointType.Rock));
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                }
            }

            var allXCoords = points.Select(a => a.X).Distinct().ToArray();
            var allYCoords = points.Select(a => a.Y).Distinct().ToArray();
            var minX = allXCoords.Min();
            var maxX = allXCoords.Max();
            var minY = allYCoords.Min();
            var maxY = allYCoords.Max();

            // draw the floor
            if (part2)
            {
                for (int i = minX - margin; i <= maxX + margin; i++)
                {
                    points.Add(new CavePoint(i, maxY + 2, CavePointType.Rock));
                }
            }

            allXCoords = points.Select(a => a.X).Distinct().ToArray();
            allYCoords = points.Select(a => a.Y).Distinct().ToArray();
            minX = allXCoords.Min();
            maxX = allXCoords.Max();
            minY = allYCoords.Min();
            maxY = allYCoords.Max();

            // Fill the gaps with air
            for (int row = minY; row <= maxY; row++)
            {
                for (int column = minX; column <= maxX; column++)
                {
                    if (points.Any(a => a.X == column && a.Y == row))
                    {
                        continue;
                    }

                    points.Add(new CavePoint(column, row, CavePointType.Air));
                }
            }



            // Sort the points
            points.Sort();

            // Transform to jagged array
            CavePoint[][] transformed = new CavePoint[maxY - minY + 1][];

            for (int i = 0; i < transformed.Length; i++)
            {
                transformed[i] = new CavePoint[maxX - minX + 1];
            }

            for (int row = 0; row < transformed.Length; row++)
            {
                for (int column = 0; column < transformed[row].Length; column++)
                {
                    var cell = points[row * transformed[row].Length + column];
                    cell.SetRowColumn(row, column);
                    transformed[row][column] = cell;
                }
            }

            return transformed;
        }

        static void Main(string[] args)
        {
            // Since it draws everything, 150 is the minium margin for the full input to be displayed properly
            var cavePoints = ParseInput(true, 50);
            var simulation = new Simulation(cavePoints);
            simulation.StartSimulation(false);

            Console.WriteLine($"Generated sand units {simulation.GeneratedSandUnits}");
        }
    }
}