using System;
using System.Threading.Tasks;

namespace ThreadingIssues
{
    class Program
    {
        static void Main(string[] args)
        {
            RaceCondition();
        }

        public static void RaceCondition()
        {
            StateObject stateObject = new StateObject();
            for(int i = 0; i < 3; i++)
            {
                Task.Run(() => SampleTask.RaceCondition(stateObject));
            }
            Console.ReadLine();
        }
    }
}
