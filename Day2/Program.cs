namespace Day2
{
    enum RockPaperScissorChoice
    {
        Rock = 1,
        Paper = 2,
        Scissor = 3
    }

    enum MatchResult
    {
        Win = 6,
        Draw = 3,
        Lose = 0
    }

    class Match
    {
        public RockPaperScissorChoice YourChoice { get; }
        public RockPaperScissorChoice OpponentChoice { get; }
        public MatchResult Result
        {
            get
            {
                int difference = YourChoice - OpponentChoice;

                if (difference == 0)
                {
                    return MatchResult.Draw;
                }
                else if (difference == 1 || difference == -2)
                {
                    return MatchResult.Win;

                }
                else
                {
                    return MatchResult.Lose;
                }
            }
        }

        public int Score => (int)YourChoice + (int)Result;

        public Match(string opponentChoiceValue, string matchResultValue)
        {
            OpponentChoice = opponentChoiceValue switch
            {
                "A" => RockPaperScissorChoice.Rock,
                "B" => RockPaperScissorChoice.Paper,
                "C" => RockPaperScissorChoice.Scissor,
                _ => throw new NotSupportedException(),
            };

            var matchResult = matchResultValue switch
            {
                "X" => MatchResult.Lose,
                "Y" => MatchResult.Draw,
                "Z" => MatchResult.Win,
                _ => throw new NotSupportedException(),
            };

            YourChoice = matchResult switch
            {
                MatchResult.Lose => OpponentChoice == RockPaperScissorChoice.Rock ? RockPaperScissorChoice.Scissor : OpponentChoice - 1,
                MatchResult.Draw => OpponentChoice,
                MatchResult.Win => OpponentChoice == RockPaperScissorChoice.Scissor ? RockPaperScissorChoice.Rock : OpponentChoice + 1,
                _ => throw new NotSupportedException(),
            };
        }
    }

    internal class Program
    {
        static IReadOnlyCollection<Match> ParseInput()
        {
            List<Match> inventories = new();

            foreach (string line in File.ReadLines(@"input.txt"))
            {
                var split = line.Split(" ");
                inventories.Add(new Match(split[0], split[1]));
            }

            return inventories;
        }

        static void Main(string[] args)
        {
            IReadOnlyCollection<Match> matches = ParseInput();
            int totalScore = matches.Sum(a => a.Score);
            Console.WriteLine(totalScore);
        }
    }
}