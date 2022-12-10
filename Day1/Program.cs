namespace Day1
{
    class ElfInventory
    {
        public List<long> Items = new();

        public long TotalCalories => Items.Sum(a => a);

        public ElfInventory()
        {

        }
    }

    internal class Program
    {
        static List<ElfInventory> ParseInput()
        {
            List<ElfInventory> inventories = new();

            ElfInventory? currentInventory = new();
            inventories.Add(currentInventory);

            foreach (string line in File.ReadLines(@"input.txt"))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    long calories = long.Parse(line);
                    currentInventory.Items.Add(calories);
                }
                else
                {
                    currentInventory = new();
                    inventories.Add(currentInventory);
                }
            }

            return inventories;
        }

        static void Main(string[] args)
        {
            List<ElfInventory> inventories = ParseInput();
            var calories = inventories.OrderByDescending(a => a.TotalCalories).Take(3).Select(a => a.TotalCalories).Sum();
            Console.WriteLine(calories);
        }
    }
}