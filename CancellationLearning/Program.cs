using System;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            CancelTask();
        }

        public static void CancelParallelFor()
        {
            var cts = new CancellationTokenSource();
            cts.Token.Register(() => Console.WriteLine("*** token cancelled"));
            cts.CancelAfter(500);
            try
            {
                ParallelLoopResult result = Parallel.For(0, 100, new ParallelOptions
                {
                    CancellationToken = cts.Token
                },
                x =>
                {
                    Console.WriteLine($"loop {x} started");
                    int sum = 0;
                    for (int i = 0; i < 100; i++)
                    {
                        Task.Delay(2).Wait();
                        sum += i;
                    }
                    Console.WriteLine($"loop {x} finished");
                });
            }
            catch(OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void CancelTask()
        {
            var cts = new CancellationTokenSource();
            cts.Token.Register(() => Console.WriteLine("*** token canceled"));
            cts.CancelAfter(500);
            Task task = Task.Run(() =>
            {
                Console.WriteLine("in task");
                for (int i = 0; i < 20; i++)
                {
                    Task.Delay(100).Wait();
                    CancellationToken token = cts.Token;
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("cancellation was requested, cancelling from task");
                        token.ThrowIfCancellationRequested();
                        break;
                    }
                    Console.WriteLine("in loop");
                }
                Console.WriteLine("task finished without cancellation");
            }, cts.Token);

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"exception: {ex.GetType().Name}, {ex.Message}");
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine($"inner exception: {ex.InnerException.GetType()}," +
                    $"{ex.InnerException.Message}");
                }
            }
        }
    }
}
