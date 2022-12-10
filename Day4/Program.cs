namespace Day4
{
    class AssignmentPair
    {
        public CleaningAssignment First { get; }
        public CleaningAssignment Second { get; }

        public AssignmentPair(CleaningAssignment first, CleaningAssignment second)
        {
            First = first;
            Second = second;
        }

        public bool IsFullyContained()
        {
            return First.SectionIdFrom <= Second.SectionIdFrom && First.SectionIdTo >= Second.SectionIdTo ||
                Second.SectionIdFrom <= First.SectionIdFrom && Second.SectionIdTo >= First.SectionIdTo;
        }

        public bool Overlap()
        {
            return First.SectionIdFrom <= Second.SectionIdFrom && First.SectionIdTo >= Second.SectionIdFrom ||
                Second.SectionIdFrom <= First.SectionIdFrom && Second.SectionIdTo >= First.SectionIdFrom;
        }
    }

    class CleaningAssignment
    {
        public int SectionIdFrom { get; }
        public int SectionIdTo { get; }

        public CleaningAssignment(string sectionIdRange)
        {
            var fromTo = sectionIdRange.Split("-");
            SectionIdFrom = Convert.ToInt32(fromTo[0]);
            SectionIdTo = Convert.ToInt32(fromTo[1]);
        }
    }

    internal class Program
    {
        static IReadOnlyCollection<AssignmentPair> ParseInput()
        {
            List<AssignmentPair> pairs = new();

            foreach (var line in File.ReadLines(@"input.txt"))
            {
                var split = line.Split(",");
                pairs.Add(new AssignmentPair(new CleaningAssignment(split[0]), new CleaningAssignment(split[1])));
            }

            return pairs;
        }

        static void Main(string[] args)
        {
            var pairs = ParseInput();

            var fullyContainedPairs = pairs.Where(a => a.Overlap()).Count();

            Console.WriteLine(fullyContainedPairs);
        }
    }
}