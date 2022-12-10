using System.Diagnostics;

namespace Day7
{
    public class FileSystemNode
    {
        public string Name { get; }
        public bool IsFolder { get; }
        public int Size { get; private set; }

        private readonly List<FileSystemNode> _subNodes = new();
        public IReadOnlyCollection<FileSystemNode> SubNodes => _subNodes;

        public FileSystemNode? ParentNode { get; set; }

        public FileSystemNode(string name, bool isFolder, FileSystemNode? parentNode, int? size = null)
        {
            Name = name;
            IsFolder = isFolder;
            ParentNode = parentNode;
            Size = size ?? 0;
        }

        public void AddNode(FileSystemNode subnode)
        {
            _subNodes.Add(subnode);
            IncreaseSize(subnode.Size);
        }

        private void IncreaseSize(int nodeSize)
        {
            Size += nodeSize;
            ParentNode?.IncreaseSize(nodeSize);
        }

        public IReadOnlyCollection<FileSystemNode> GetFoldersWithSizeAtMost(int atMostSize)
        {
            List<FileSystemNode> nodes = new();

            if (Size > 0 && Size < atMostSize)
            {
                nodes.Add(this);
            };

            foreach (FileSystemNode folder in _subNodes.Where(a => a.IsFolder))
            {
                nodes.AddRange(folder.GetFoldersWithSizeAtMost(atMostSize));
            }

            return nodes;
        }

        public IReadOnlyCollection<FileSystemNode> GetFoldersWithSizeAtLeast(int atLeast)
        {
            List<FileSystemNode> nodes = new();

            if (Size >= atLeast)
            {
                nodes.Add(this);
            };

            foreach (FileSystemNode folder in _subNodes.Where(a => a.IsFolder))
            {
                nodes.AddRange(folder.GetFoldersWithSizeAtLeast(atLeast));
            }

            return nodes;
        }

    }

    internal class Program
    {
        static FileSystemNode? ParseInput()
        {
            FileSystemNode? rootNode = null;
            FileSystemNode? currentNode = null;

            foreach (var line in File.ReadLines("input.txt"))
            {
                var split = line.Split(" ");

                if (split[0] == "$" && split[1] == "cd")
                {
                    var folderName = split[2];

                    if (folderName == "..")
                    {
                        currentNode = currentNode?.ParentNode;
                    }
                    else if (folderName == "/")
                    {
                        if (rootNode == null)
                        {
                            rootNode = new FileSystemNode(folderName, true, null);
                        }

                        currentNode = rootNode;
                    }
                    else
                    {
                        currentNode = currentNode?.SubNodes.First(a => a.Name == folderName);
                    }
                }
                else if (line.StartsWith("$ ls"))
                {
                    continue;
                }
                else if (line.StartsWith("dir"))
                {
                    // directory
                    currentNode?.AddNode(new FileSystemNode(split[1], true, currentNode));
                }
                else
                {
                    // file
                    currentNode?.AddNode(new FileSystemNode(split[1], false, currentNode, Convert.ToInt32(split[0])));
                }
            }

            return rootNode;
        }

        static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();

            var folderStructure = ParseInput();

            Console.WriteLine($"Input parsed in in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Restart();

            if (folderStructure == null)
            {
                return;
            }

            var folders1 = folderStructure.GetFoldersWithSizeAtMost(100000);
            var sum = folders1.Sum(a => a.Size);
            Console.WriteLine($"Sum of folders <100000: {sum}, calculated in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Restart();

            var freeSpace = 70000000 - folderStructure.Size;
            var neededSpace = 30000000 - freeSpace;

            var folders2 = folderStructure.GetFoldersWithSizeAtLeast(neededSpace);
            var smallestFolderToDelete = folders2.OrderBy(a => a.Size).First();
            Console.WriteLine($"Smallest folder size: {smallestFolderToDelete.Size}, calculated in {stopWatch.ElapsedMilliseconds} ms");

            stopWatch.Stop();
        }
    }
}