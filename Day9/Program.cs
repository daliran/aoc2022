using System.Diagnostics;

namespace Day9
{
    enum RopeMovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    record RopeMovement(RopeMovementDirection Direction, int Steps);

    class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public override string ToString()
        {
            return Row + ", " + Column;
        }
    }

    class RopeKnot
    {
        public int Id { get; }
        public Position Head { get; private set; } = new Position();
        public Position Tail { get; private set; } = new Position();

        private readonly List<Position> _visitedPositions = new();
        public int VisitePositions => _visitedPositions.Count;

        public RopeKnot(int id)
        {
            Id = id;

            // The initial position must be visited
            MoveTail();
        }

        public override string ToString()
        {
            return "Id " + Id + "    Head: (" + Head + ")    Tail: (" + Tail + ")";
        }

        public void MoveHeadByRopeMovement(RopeMovement ropeMovement)
        {
            switch (ropeMovement.Direction)
            {
                case RopeMovementDirection.Up:
                    Head.Row += 1;
                    break;
                case RopeMovementDirection.Down:
                    Head.Row -= 1;
                    break;
                case RopeMovementDirection.Left:
                    Head.Column -= 1;
                    break;
                case RopeMovementDirection.Right:
                    Head.Column += 1;
                    break;
            }

            MoveTail();
        }

        public void MoveHeadByPosition(int row, int column)
        {
            Head.Row = row;
            Head.Column = column;
            MoveTail();
        }

        private void MoveTail()
        {
            var rowDifference = Head.Row - Tail.Row;
            var columnDifference = Head.Column - Tail.Column;

            if (rowDifference == 0 && columnDifference != 0)
            {
                // aligned but in different columns

                var tailOnTheLeft = columnDifference > 0;

                if (Math.Abs(columnDifference) > 1)
                {
                    if (tailOnTheLeft)
                    {
                        Tail.Column = Head.Column - 1;
                    }
                    else
                    {
                        Tail.Column = Head.Column + 1;
                    }
                }
            }
            else if (columnDifference == 0 && rowDifference != 0)
            {
                // aligned but in different rows

                var tailOnTheBottom = rowDifference > 0;

                if (Math.Abs(rowDifference) > 1)
                {
                    if (tailOnTheBottom)
                    {
                        Tail.Row = Head.Row - 1;
                    }
                    else
                    {
                        Tail.Row = Head.Row + 1;
                    }
                }
            }
            else if (Math.Abs(columnDifference) > 1 || Math.Abs(rowDifference) > 1)
            {
                // on a diagonal, with a "diagonal distance" greater than 1

                var tailOnTheBottom = rowDifference > 0;

                if (tailOnTheBottom)
                {
                    Tail.Row += 1;
                }
                else
                {
                    Tail.Row += -1;
                }

                var tailOnTheLeft = columnDifference > 0;

                if (tailOnTheLeft)
                {
                    Tail.Column += 1;
                }
                else
                {
                    Tail.Column += -1;
                }
            }

            VisitPosition(Head);
        }

        private void VisitPosition(Position position)
        {
            // Mark only unvisited positions
            if (!_visitedPositions.Any(a => a.Row == position.Row && a.Column == position.Column))
            {
                _visitedPositions.Add(new Position { Row = position.Row, Column = position.Column });
            }
        }

    }

    internal class Program
    {
        static IReadOnlyList<RopeMovement> ParseInput()
        {
            List<RopeMovement> movements = new();

            foreach (var line in File.ReadLines("input.txt"))
            {
                var split = line.Split(" ");

                RopeMovementDirection direction = split[0] switch
                {
                    "U" => RopeMovementDirection.Up,
                    "D" => RopeMovementDirection.Down,
                    "L" => RopeMovementDirection.Left,
                    "R" => RopeMovementDirection.Right,
                    _ => throw new NotImplementedException(),
                };

                movements.Add(new RopeMovement(direction, Convert.ToInt32(split[1])));
            }

            return movements;
        }

        static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();

            var ropeMovements = ParseInput();

            Console.WriteLine($"Input parsed in in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Restart();

            List<RopeKnot> knots = new();

            // Create all the knots
            for (int i = 0; i < 10; i++)
            {
                knots.Add(new RopeKnot(i));
            }

            for (int movementIndex = 0; movementIndex < ropeMovements.Count; movementIndex++)
            {
                for (int stepIndex = 0; stepIndex < ropeMovements[movementIndex].Steps; stepIndex++)
                {
                    // Real rope head
                    RopeKnot head = knots.First();
                    RopeKnot currentHead = head;

                    // Move the real rope head
                    head.MoveHeadByRopeMovement(ropeMovements[movementIndex]);

                    // Update the position of all the next knots based on the head
                    for (int knotIndex = 1; knotIndex < knots.Count; knotIndex++)
                    {
                        knots[knotIndex].MoveHeadByPosition(currentHead.Tail.Row, currentHead.Tail.Column);
                        currentHead = knots[knotIndex];
                    }
                }
            }

            var visitedPos = knots.Last().VisitePositions;

            Console.WriteLine($"Last knot visited positions {visitedPos}, calculated in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Stop();
        }
    }
}