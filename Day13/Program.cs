using System.Text.Json.Nodes;

namespace Day13
{
    class PacketsSorter
    {
        public bool IsRightOrder(JsonArray left, JsonArray right)
        {
            var index = 0;

            bool? result = null;

            // Loop the packages until is clear if the packages are in the right order or not
            while (result == null && (index < left.Count || index < right.Count))
            {
                result = IsRightOrderByIndex(index, left, right);
                index++;
            }

            return result ?? false;
        }

        private bool? IsRightOrderByIndex(int index, JsonArray left, JsonArray right)
        {
            var leftValue = left.ElementAtOrDefault(index);
            var rightValue = right.ElementAtOrDefault(index);

            // Only one or no value available
            if (leftValue == null && rightValue != null)
            {
                return true;
            }
            else if (leftValue != null && rightValue == null)
            {
                return false;
            }

            // Safe guard for nullability and to check that no null value is passing
            if (leftValue == null || rightValue == null)
            {
                throw new InvalidOperationException();
            }

            // Both values available
            if (leftValue is JsonValue && rightValue is JsonValue)
            {
                if (leftValue.GetValue<int>() == rightValue.GetValue<int>())
                {
                    return null;
                }
                else
                {
                    return leftValue.GetValue<int>() < rightValue.GetValue<int>();
                }
            }

            JsonArray leftValuePackets;
            JsonArray rightValuePackets;

            if (leftValue is JsonValue)
            {
                leftValuePackets = new JsonArray
                {
                    leftValue.GetValue<int>()
                };
                rightValuePackets = rightValue.AsArray();
            }
            else if (rightValue is JsonValue)
            {
                leftValuePackets = leftValue.AsArray();
                rightValuePackets = new JsonArray
                {
                    rightValue.GetValue<int>()
                };
            }
            else
            {
                leftValuePackets = leftValue.AsArray();
                rightValuePackets = rightValue.AsArray();
            }

            int listIndex = 0;

            bool? result = null;

            while (result == null && (listIndex < leftValuePackets!.Count || listIndex < rightValuePackets!.Count))
            {
                result = IsRightOrderByIndex(listIndex, leftValuePackets!, rightValuePackets!);
                listIndex++;
            }

            return result;
        }
    }

    record PacketPair(JsonArray Left, JsonArray Right);

    internal class Program
    {
        static IReadOnlyList<PacketPair> ParseInputPart1()
        {
            List<PacketPair> input = new();

            foreach (var line in File.ReadLines("input.txt").Chunk(3))
            {
                var firstPacket = line[0];
                var secondPacket = line[1];

                var parsedFirstPackage = JsonNode.Parse(firstPacket)!.AsArray();
                var parsedSecondPackage = JsonNode.Parse(secondPacket)!.AsArray();

                input.Add(new PacketPair(parsedFirstPackage, parsedSecondPackage));
            }

            return input;
        }

        static IReadOnlyList<JsonArray> ParseInputPart2()
        {
            List<JsonArray> input = new();

            foreach (var line in File.ReadLines("input.txt"))
            {
                // Discard all empty lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                input.Add(JsonNode.Parse(line)!.AsArray());
            }

            // Extra divider packages
            input.Add(JsonNode.Parse("[[2]]")!.AsArray());
            input.Add(JsonNode.Parse("[[6]]")!.AsArray());

            return input;
        }

        static void Main(string[] args)
        {
            var packetSorter = new PacketsSorter();

            var packetPairs = ParseInputPart1();

            List<int> pairsInTheRightOrder = new();

            for (int i = 0; i < packetPairs.Count; i++)
            {
                if (packetSorter.IsRightOrder(packetPairs[i].Left, packetPairs[i].Right))
                {
                    pairsInTheRightOrder.Add(i);
                }
            }

            if (pairsInTheRightOrder.Count > 0)
            {
                // Sum 1 because the indexes are 0 based but the exercise wants 1 based
                var indexesSum = pairsInTheRightOrder.Select(a => a + 1).Aggregate((total, value) => total + value);

                Console.WriteLine($"The sum of indexes is {indexesSum}");
            }

            var packages = ParseInputPart2().ToArray();

            // Sort packages
            for (int i = 0; i < packages.Length; i++)
            {
                for (int j = 0; j < packages.Length - 1; j++)
                {
                    if (!packetSorter.IsRightOrder(packages[j], packages[j + 1]))
                    {
                        var temp = packages[j];
                        packages[j] = packages[j + 1];
                        packages[j + 1] = temp;
                    }
                }
            }

            var indexOf2 = packages.Select((a, index) => new { Text = a.ToJsonString(), Index = index }).First(a => a.Text == "[[2]]").Index;
            var indexOf6 = packages.Select((a, index) => new { Text = a.ToJsonString(), Index = index }).First(a => a.Text == "[[6]]").Index;

            var decoderKey = (indexOf2 + 1) * (indexOf6 + 1);

            Console.WriteLine($"The decoder key of the distress signal is: {decoderKey}");
        }
    }
}