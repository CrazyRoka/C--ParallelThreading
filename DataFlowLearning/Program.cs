using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            UsePipeline();
        }

        public static void ActionBlockSample()
        {
            var processInput = new ActionBlock<string>(s =>
            {
                Console.WriteLine($"user input: {s}");
            });
            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine();
                if(string.Compare(input, "exit", true) == 0)
                {
                    exit = true;
                }
                else
                {
                    processInput.Post(input);
                }
            }
        }

        private static BufferBlock<string> s_buffer = new BufferBlock<string>();
        public static void Producer()
        {
            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine();
                if (string.Compare(input, "exit", true) == 0)
                {
                    exit = true;
                }
                else
                {
                    s_buffer.Post(input);
                }
            }
        }

        public static async Task ConsumerAsync()
        {
            while (true)
            {
                string data = await s_buffer.ReceiveAsync();
                Console.WriteLine($"received: {data}");
            }
        }

        public static void BufferBlockSample()
        {
            Task task1 = Task.Run(() => Producer());
            Task task2 = Task.Run(async () => await ConsumerAsync());
            Task.WaitAll(task1, task2);
        }

        public static IEnumerable<string> GetFileNames(string path)
        {
            foreach (string fileName in Directory.EnumerateFiles(path, "*.cs"))
            {
                yield return fileName;
            }
        }

        public static IEnumerable<string> LoadLines(IEnumerable<string> fileNames)
        {
            foreach(var fileName in fileNames)
            {
                using (FileStream stream = File.OpenRead(fileName)) 
                {
                    var reader = new StreamReader(stream);
                    string line = null;
                    while((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }

        public static IEnumerable<string> GetWords(IEnumerable<string> lines)
        {
            foreach(string line in lines)
            {
                string[] words = line.Split(' ', ';', '(', ')', '{', '}', '.', ',');
                foreach(string word in words)
                {
                    if (!string.IsNullOrEmpty(word))
                        yield return word;
                }
            }
        }

        public static ITargetBlock<string> SetupPipeline()
        {
            var fileNamesForPath = new TransformBlock<string, IEnumerable<string>>(path => GetFileNames(path));
            var lines = new TransformBlock<IEnumerable<string>, IEnumerable<string>>(fileNames => LoadLines(fileNames));
            var words = new TransformBlock<IEnumerable<string>, IEnumerable<string>>(fileLines => GetWords(fileLines));

            var display = new ActionBlock<IEnumerable<string>>(tokens =>
            {
                foreach (string word in tokens)
                {
                    Console.WriteLine(word);
                }
            });

            fileNamesForPath.LinkTo(lines);
            lines.LinkTo(words);
            words.LinkTo(display);
            return fileNamesForPath;
        }

        public static void UsePipeline()
        {
            var target = SetupPipeline();
            target.Post("../../../");
            Console.ReadLine();
        }
    }
}
