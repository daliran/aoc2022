using System.Diagnostics;

namespace Day8
{
    internal class Program
    {
        static List<List<int>> ParseInput()
        {
            List<List<int>> grid = new();

            foreach (var line in File.ReadLines("input.txt"))
            {
                List<int> characters = new();

                foreach (var character in line)
                {
                    characters.Add(character - '0');
                }

                grid.Add(characters);
            }

            return grid;
        }

        #region First part

        static bool IsVisibleFromTop(List<List<int>> grid, int row, int column)
        {
            var treeHeight = grid[row][column];

            for (int i = row - 1; i >= 0; i--)
            {
                if (grid[i][column] >= treeHeight)
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsVisibleFromBottom(List<List<int>> grid, int row, int column)
        {
            var treeHeight = grid[row][column];

            for (int i = row + 1; i < grid.Count; i++)
            {
                if (grid[i][column] >= treeHeight)
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsVisibleFormLeft(List<List<int>> grid, int row, int column)
        {
            var treeHeight = grid[row][column];

            for (int i = column - 1; i >= 0; i--)
            {
                if (grid[row][i] >= treeHeight)
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsVisibleFormRight(List<List<int>> grid, int row, int column)
        {
            var treeHeight = grid[row][column];

            for (int i = column + 1; i < grid[row].Count; i++)
            {
                if (grid[row][i] >= treeHeight)
                {
                    return false;
                }
            }

            return true;
        }

        static int FirstPart(List<List<int>> grid)
        {
            int visibleTrees = 0;

            for (int row = 0; row < grid.Count; row++)
            {
                for (int column = 0; column < grid[row].Count; column++)
                {
                    // edge
                    if (row == 0 || row == grid.Count - 1 || column == 0 || column == grid[row].Count - 1)
                    {
                        visibleTrees++;
                        continue;
                    }

                    if (IsVisibleFromTop(grid, row, column))
                    {
                        visibleTrees++;
                        continue;
                    }

                    if (IsVisibleFromBottom(grid, row, column))
                    {
                        visibleTrees++;
                        continue;
                    }

                    if (IsVisibleFormLeft(grid, row, column))
                    {
                        visibleTrees++;
                        continue;
                    }

                    if (IsVisibleFormRight(grid, row, column))
                    {
                        visibleTrees++;
                        continue;
                    }
                }
            }

            return visibleTrees;
        }

        #endregion


        #region Second part

        static int TopScore(List<List<int>> grid, int row, int column)
        {
            var treeHeight = grid[row][column];

            for (int i = row - 1; i >= 0; i--)
            {
                if (grid[i][column] >= treeHeight)
                {
                    return row - i;
                }
            }

            return row;
        }

        static int BottomScore(List<List<int>> grid, int row, int column)
        {
            var treeHeight = grid[row][column];

            for (int i = row + 1; i < grid.Count; i++)
            {
                if (grid[i][column] >= treeHeight)
                {
                    return i - row;
                }
            }

            return grid.Count - row - 1;
        }

        static int LeftScore(List<List<int>> grid, int row, int column)
        {
            var treeHeight = grid[row][column];

            for (int i = column - 1; i >= 0; i--)
            {
                if (grid[row][i] >= treeHeight)
                {
                    return column - i;
                }
            }

            return column;
        }

        static int RightScore(List<List<int>> grid, int row, int column)
        {
            var treeHeight = grid[row][column];

            for (int i = column + 1; i < grid[row].Count; i++)
            {
                if (grid[row][i] >= treeHeight)
                {
                    return i - column;
                }
            }

            return grid[row].Count - column - 1;
        }

        static int SecondPart(List<List<int>> grid)
        {
            int highestScenicScore = 0;

            for (int row = 0; row < grid.Count; row++)
            {
                for (int column = 0; column < grid[row].Count; column++)
                {
                    int currentScore = 1;

                    // edge
                    if (row == 0 || row == grid.Count - 1 || column == 0 || column == grid[row].Count - 1)
                    {
                        continue;
                    }

                    currentScore *= TopScore(grid, row, column);
                    currentScore *= BottomScore(grid, row, column);
                    currentScore *= LeftScore(grid, row, column);
                    currentScore *= RightScore(grid, row, column);

                    if (currentScore > highestScenicScore)
                    {
                        highestScenicScore = currentScore;
                    }
                }
            }

            return highestScenicScore;
        }

        #endregion

        static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();

            var input = ParseInput();

            Console.WriteLine($"Input parsed in in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Restart();

            var visibleTrees = FirstPart(input);
            Console.WriteLine($"Number of visible trees {visibleTrees}, calculated in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Restart();

            var highestScenicScore = SecondPart(input);
            Console.WriteLine($"Highest scenisc score {highestScenicScore}, calculated in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Stop();
        }
    }
}