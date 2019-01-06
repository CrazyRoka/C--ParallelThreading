using System;
using System.Threading.Tasks;

namespace Synchronization
{
    class Program
    {
        static void Main(string[] args)
        {
            WithSynchronization(20);
        }

        static void NoSynchronization(uint numberOfTasks)
        {
            Task[] tasks = new Task[numberOfTasks];
            for(int i = 0; i < numberOfTasks; i++)
            {
                tasks[i] = Task.Run(() => Job.DoJob());
            }
            Task.WaitAll(tasks);
            Console.WriteLine(Job.SharedState);
        }

        static void WithSynchronization(uint numberOfTasks)
        {
            Task[] tasks = new Task[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks[i] = Task.Run(() => { lock (typeof(Job)) { Job.DoJob(); } });
            }
            Task.WaitAll(tasks);
            Console.WriteLine(Job.SharedState);
        }
    }
}
