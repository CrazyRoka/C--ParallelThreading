using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            ParallelForWithInit();
        }

        public static void Log(string prefix) =>
            Console.WriteLine($"{prefix}, task: {Task.CurrentId}, " +
                $"thread: {Thread.CurrentThread.ManagedThreadId}");

        public static void ParallelFor()
        {
            ParallelLoopResult result = Parallel.For(0, 10, i => {
                Log($"Start {i}");
                Task.Delay(10).Wait();
                Log($"End {i}");
            });
            Console.WriteLine($"Is completed: {result.IsCompleted}");
        }

        public static void ParallelForWithAsync()
        {
            ParallelLoopResult result = Parallel.For(0, 10, async i => {
                Log($"Start {i}");
                await Task.Delay(10);
                Log($"End {i}");
            });
            Console.WriteLine($"Is completed: {result.IsCompleted}");
        }

        public static void ParallelForEarly()
        {
            ParallelLoopResult result = Parallel.For(0, 40, (i, state) =>
            {
                Log($"Start {i}");
                if(i > 15)
                {
                    state.Break();
                    Log($"Break for loop {i}");
                }
                Task.Delay(10).Wait();
                Log($"End {i}");
            });
            Console.WriteLine($"Is completed: {result.IsCompleted}");
            Console.WriteLine($"Lowest break iteration: {result.LowestBreakIteration}");
        }

        public static void ParallelForWithInit()
        {
            Parallel.For<string>(0, 10, () =>
            {
                Log($"init thread");
                return $"thread {Thread.CurrentThread.ManagedThreadId}";
            },
            (i, state, str1) =>
            {
                Log($"body i {i} str1 {str1}");
                Task.Delay(10).Wait();
                return $"i {i}";
            },
            (str1) =>
            {
                Log($"finally {str1}");
            });
        }

        public static void ParallelForEach()
        {
            string[] data = {"zero", "one", "two", "three", "four", "five", "six",
                "seven", "eight", "nine", "ten", "eleven", "twelve"};            Parallel.ForEach(data, (s, state, i) =>
            {
                Console.WriteLine($"{s} {i}");
            });
        }

        public static void ParallelInvoke()
        {
            Parallel.Invoke(Foo, Bar);

            void Foo() => Console.WriteLine("Foo");
            void Bar() => Console.WriteLine("Bar");
        }
    }
}
