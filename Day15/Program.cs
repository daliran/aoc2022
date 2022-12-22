using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day15
{
    // X = columns (horizontal), Y = row (vertical)
    record struct Position(int X, int Y)
    {
        private const int MIN_COORD = 0;
        //private const int MAX_COORD = 20; // small search space
        private const int MAX_COORD = 4000000; // big search space

        public (Position Position, bool Valid) GoUp(int distance)
        {
            var newPos = new Position(X, Y - distance);
            return (newPos, newPos.IsValid());
        }

        public (Position Position, bool Valid) GoDown(int distance)
        {
            var newPos = new Position(X, Y + distance);
            return (newPos, newPos.IsValid());
        }

        public (Position Position, bool Valid) GoLeft(int distance)
        {
            var newPos = new Position(X - distance, Y);
            return (newPos, newPos.IsValid());
        }

        public (Position Position, bool Valid) GoRight(int distance)
        {
            var newPos = new Position(X + distance, Y);
            return (newPos, newPos.IsValid());
        }

        public (Position Position, bool Valid) GoDownRight(int distance)
        {
            var newPos = new Position(X + distance, Y + distance);
            return (newPos, newPos.IsValid());
        }

        public (Position Position, bool Valid) GoDownLeft(int distance)
        {
            var newPos = new Position(X - distance, Y + distance);
            return (newPos, newPos.IsValid());
        }

        public (Position Position, bool Valid) GoUpLeft(int distance)
        {
            var newPos = new Position(X - distance, Y - distance);
            return (newPos, newPos.IsValid());
        }

        public (Position Position, bool Valid) GoUpRight(int distance)
        {
            var newPos = new Position(X + distance, Y - distance);
            return (newPos, newPos.IsValid());
        }

        public bool IsValid()
        {
            return X <= MAX_COORD && X >= MIN_COORD && Y <= MAX_COORD && Y >= MIN_COORD;
        }
    }

    class SensorReading
    {
        public Position SensorPosition { get; }
        public Position ClosestBeaconPosition { get; }

        public (Position Position, bool Valid) TopVertex { get; }
        public (Position Position, bool Valid) DownVertex { get; }
        public (Position Position, bool Valid) LeftVertex { get; }
        public (Position Position, bool Valid) RightVertex { get; }

        public int ManhattanDistance { get; }

        public SensorReading(Position sensorPosition, Position closestBeaconPosition)
        {
            SensorPosition = sensorPosition;
            ClosestBeaconPosition = closestBeaconPosition;
            ManhattanDistance = CalculateManhattanDistance(sensorPosition, closestBeaconPosition);

            TopVertex = SensorPosition.GoUp(ManhattanDistance);
            DownVertex = SensorPosition.GoDown(ManhattanDistance);
            LeftVertex = SensorPosition.GoLeft(ManhattanDistance);
            RightVertex = SensorPosition.GoRight(ManhattanDistance);
        }

        private static int CalculateManhattanDistance(Position position1, Position position2)
        {
            return Math.Abs(position1.X - position2.X) + Math.Abs(position1.Y - position2.Y);
        }

        public IReadOnlyCollection<Position> GetSensorCoverageForAGivenY(int y)
        {
            List<Position> coveredPositions = new();

            // Outside the manhattan distance
            if (SensorPosition.Y + ManhattanDistance < y)
            {
                return coveredPositions;
            }

            // Check all the positions within the x range (same row) at the same y
            for (int x = SensorPosition.X - ManhattanDistance; x <= SensorPosition.X + ManhattanDistance; x++)
            {
                var positionToAnalyze = new Position(x, y);

                if (CalculateManhattanDistance(SensorPosition, positionToAnalyze) <= ManhattanDistance)
                {
                    coveredPositions.Add(positionToAnalyze);
                }
            }

            return coveredPositions;
        }

        public static IReadOnlyCollection<Position> CalculateNoBeaconPositions(IReadOnlyCollection<SensorReading> sensorReadings, int yToFind)
        {
            List<Position> coveredPositions = new();

            foreach (var sensorReading in sensorReadings)
            {
                var sensorCoveredPotions = sensorReading.GetSensorCoverageForAGivenY(yToFind);

                foreach (var sensorCoveredPotion in sensorCoveredPotions)
                {
                    coveredPositions.Add(sensorCoveredPotion);
                }
            }

            var positionsToExclude = sensorReadings.Select(a => a.SensorPosition).Union(sensorReadings.Select(a => a.ClosestBeaconPosition)).ToList();

            return coveredPositions.Where(coveredPosition => !positionsToExclude.Any(positionToExclude => positionToExclude == coveredPosition))
                  .Distinct()
                  .ToList();
        }

        public IEnumerable<Position> GetValidNeighbourPositions()
        {
            //LinkedList<Position> positions = new();

            // top point
            var topNeighbour = TopVertex.Position.GoUp(1);

            if (topNeighbour.Valid)
            {
                //positions.AddLast(topNeighbour.Position);
                yield return topNeighbour.Position;
            }

            // bottom point
            var bottomNeighbour = DownVertex.Position.GoDown(1);

            if (bottomNeighbour.Valid)
            {
                //positions.AddLast(bottomNeighbour.Position);
                yield return bottomNeighbour.Position;
            }

            // left point
            var leftNeighbour = LeftVertex.Position.GoLeft(1);

            if (leftNeighbour.Valid)
            {
                //positions.AddLast(leftNeighbour.Position);
                yield return leftNeighbour.Position;
            }

            // right point
            var rightNeighbour = RightVertex.Position.GoRight(1);

            if (rightNeighbour.Valid)
            {
                //positions.AddLast(rightNeighbour.Position);
                yield return rightNeighbour.Position;
            }

            // top-right line
            var currentTopRight = topNeighbour;

            while (currentTopRight.Position != rightNeighbour.Position)
            {
                currentTopRight = currentTopRight.Position.GoDownRight(1);

                if (currentTopRight.Valid)
                {
                    //positions.AddLast(currentTopRight.Position);
                    yield return currentTopRight.Position;
                }
            }

            // right-bottom line
            var currentRightBottom = rightNeighbour;

            while (currentRightBottom.Position != bottomNeighbour.Position)
            {
                currentRightBottom = currentRightBottom.Position.GoDownLeft(1);

                if (currentRightBottom.Valid)
                {
                    //positions.AddLast(currentRightBottom.Position);
                    yield return currentRightBottom.Position;
                }
            }

            // bottom-left line
            var currentBottomLeft = bottomNeighbour;

            while (currentBottomLeft.Position != leftNeighbour.Position)
            {
                currentBottomLeft = currentBottomLeft.Position.GoUpLeft(1);

                if (currentBottomLeft.Valid)
                {
                    //positions.AddLast(currentBottomLeft.Position);
                    yield return currentBottomLeft.Position;
                }
            }

            // left-top line
            var currentLeftTop = leftNeighbour;

            while (currentLeftTop.Position != topNeighbour.Position)
            {
                currentLeftTop = currentLeftTop.Position.GoUpRight(1);

                if (currentLeftTop.Valid)
                {
                    //positions.AddLast(currentLeftTop.Position);
                    yield return currentLeftTop.Position;
                }
            }

            //return positions;
        }

        public static Position? GetDistressBeaconPosition(IReadOnlyCollection<SensorReading> sensorReadings)
        {
            foreach (var sensorReading in sensorReadings)
            {
                var sensorsExceptCurrent = sensorReadings.Where(a => a.SensorPosition != sensorReading.SensorPosition).ToArray();

                var candidates = sensorReading.GetValidNeighbourPositions();

                foreach (var candidate in candidates)
                {
                    if (sensorsExceptCurrent.All(a => CalculateManhattanDistance(a.SensorPosition, candidate) > a.ManhattanDistance))
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }
    }

    internal class Program
    {
        static IReadOnlyCollection<SensorReading> ParseInput()
        {
            List<SensorReading> sensorReadings = new();

            Regex regex = new("^Sensor at x=(-?\\d+), y=(-?\\d+): closest beacon is at x=(-?\\d+), y=(-?\\d+)$");

            foreach (var line in File.ReadLines("input.txt"))
            {
                var match = regex.Match(line);

                var sensorX = int.Parse(match.Groups[1].Value);
                var sensorY = int.Parse(match.Groups[2].Value);

                var beaconX = int.Parse(match.Groups[3].Value);
                var beaconY = int.Parse(match.Groups[4].Value);

                sensorReadings.Add(new SensorReading(new Position(sensorX, sensorY), new Position(beaconX, beaconY)));
            }

            return sensorReadings;
        }

        static void Main(string[] args)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            var sensorReadings = ParseInput();

            Console.WriteLine($"Input parsed in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Restart();

            var noBeaconPositions = SensorReading.CalculateNoBeaconPositions(sensorReadings, 2000000);

            Console.WriteLine($"Found {noBeaconPositions.Count} in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Restart();

            var distressBeacon = SensorReading.GetDistressBeaconPosition(sensorReadings);

            if(distressBeacon != null)
            {
                long tuningFrequency = (long)distressBeacon.Value.X * 4000000 + distressBeacon.Value.Y;
                Console.WriteLine($"Tuning frequency: {tuningFrequency}, calculated in {stopWatch.ElapsedMilliseconds} ms");
            }
            else
            {
                Console.WriteLine($"Distress beacon not found");
            }

            stopWatch.Stop();
        }
    }
}