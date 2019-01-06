using System;
using System.Threading;
using System.Threading.Tasks;

namespace TimerLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            void TimerAction(object o) => Console.WriteLine($"Timer action: {DateTime.Now:T}");

            using (Timer timer = new Timer(TimerAction, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3)))
            {
                Task.Delay(15000).Wait();
            }
        }
    }
}
