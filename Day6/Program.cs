using System.Diagnostics;

namespace Day6
{
    class SignalAnalyzer
    {
        private readonly StreamReader _signal;

        public SignalAnalyzer(StreamReader signal)
        {
            _signal = signal;
        }

        public int FindFirstMarkerPosition(int distinctConsecutiveCharacters)
        {
            var queue = new Queue<char>();

            for (int i = 0; !_signal.EndOfStream; i++)
            {
                var buffer = new char[1];

                _signal.Read(buffer, 0, 1);

                if (queue.Count == distinctConsecutiveCharacters)
                {
                    queue.Dequeue();
                }

                queue.Enqueue(buffer[0]);

                if (queue.Count == distinctConsecutiveCharacters && AllDifferent(queue.ToArray()))
                {
                    return i + 1;
                }
            }

            return 0;
        }

        private static bool AllDifferent(char[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                for (int j = i + 1; j < buffer.Length; j++)
                {
                    if (buffer[i] == buffer[j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            using var reader = File.OpenText("input.txt");
            var buffer = new SignalAnalyzer(reader);

            //int firstPacketMarkerPosition = buffer.FindFirstMarkerPosition(4);
            //Console.WriteLine(firstPacketMarkerPosition);

            int firstMessageMarkerPosition = buffer.FindFirstMarkerPosition(14);
            Console.WriteLine($"{firstMessageMarkerPosition} in {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Stop();

        }
    }
}