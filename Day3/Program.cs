namespace Day3
{
    class Group
    {
        public IReadOnlyCollection<Rucksack> GroupMemberRucksacks { get; }

        public Group(IReadOnlyCollection<Rucksack> groupMemberRucksacks)
        {
            GroupMemberRucksacks = groupMemberRucksacks;
        }

        public (char Item, int Priority) FindBadgeInAllRucksacks()
        {
            var firstMember = GroupMemberRucksacks.First();
            var otherMembers = GroupMemberRucksacks.Skip(1).ToArray();

            foreach (var firstMemberItem in firstMember.Items)
            {
                if (otherMembers.All(a => a.Items.Contains(firstMemberItem)))
                {
                    var priority = Rucksack.GetItemPriority(firstMemberItem);
                    return (firstMemberItem, priority);
                }
            }

            throw new Exception("No shared badge found");
        }
    }

    class Rucksack
    {
        public string Items { get; }
        public string FirstCompartment { get; }
        public string SecondCompartment { get; }
        public char ItemInCommon { get; }

        public Rucksack(string rucksackItems)
        {
            Items = rucksackItems;

            // Compartment splitting
            FirstCompartment = rucksackItems.Substring(0, rucksackItems.Length / 2);
            SecondCompartment = rucksackItems.Substring(rucksackItems.Length / 2, rucksackItems.Length / 2);
        }

        public static int GetItemPriority(char item)
        {
           return char.IsLower(item) ? item - 'a' + 1 : item - 'A' + 27;
        }

        public (char Item, int Priority) FindCommonItemBetweenCompartments()
        {
            foreach (var x in FirstCompartment)
            {
                if (SecondCompartment.Contains(x))
                {
                    return (x, GetItemPriority(x));
                }
            }

            throw new Exception("Item in common not found");
        }
    }

    internal class Program
    {
        static IReadOnlyCollection<Group> ParseInput()
        {
            List<Group> groups = new();

            foreach (var lines in File.ReadLines(@"input.txt").Chunk(3))
            {
                groups.Add(new Group(lines.Select(a => new Rucksack(a)).ToArray()));
            }

            return groups;
        }

        static void Main(string[] args)
        {
            IReadOnlyCollection<Group> rucksacks = ParseInput();
            var prioritiesSum = rucksacks.Sum(a => a.FindBadgeInAllRucksacks().Priority);
            Console.WriteLine(prioritiesSum);
        }
    }
}