using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskLearning
{
    class Program
    {
        private static object s_logLock = new object();
        static void Main(string[] args)
        {
            ParentAndChild();
        }

        public static void Log(string title)
        {
            lock (s_logLock)
            {
                Console.WriteLine(title);
                Console.WriteLine($"Task id: {Task.CurrentId?.ToString() ?? "no task"}, " +
                    $"thread: {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine($"is pooled thread: " +
                    $"{Thread.CurrentThread.IsThreadPoolThread}");
                Console.WriteLine($"is background thread: " +
                    $"{Thread.CurrentThread.IsBackground}");
                Console.WriteLine();
            }
        }

        public static void TaskMethod(object o)
        {
            Log(o?.ToString());
        }

        public static void TaskUsingThreadPool()
        {
            var tf = new TaskFactory();
            Task t1 = tf.StartNew(TaskMethod, "using a task factory");
            Task t2 = Task.Factory.StartNew(TaskMethod, "factory via a task");
            Task t3 = new Task(TaskMethod, "using a task constructor and start");
            t3.Start();
            Task t4 = Task.Run(() => TaskMethod("using the Run method"));
            Console.ReadLine();
        }

        public static void RunSynchronousTask()
        {
            TaskMethod("Using main thread");
            Task task = new Task(TaskMethod, "Run sync");
            task.RunSynchronously();
        }

        public static void LongRunningTask()
        {
            Task task = new Task(TaskMethod, "Long running task", TaskCreationOptions.LongRunning);
            task.Start();
            Console.ReadLine();
        }

        public static (int Result, int Remainder) TaskWithResult(object division)
        {
            (int x, int y) = ((int x, int y))division;
            int result = x / y;
            int remainder = x % y;
            Console.WriteLine("Task creates a result...");
            return (result, remainder);
        }

        public static void TaskWithResultDemo()
        {
            var task = new Task<(int Result, int Remainder)>(TaskWithResult, (11, 3));
            task.Start();
            Console.WriteLine(task.Result);
            task.Wait();
            Console.WriteLine($"Result from task {task.Result.Result} {task.Result.Remainder}");
        }

        private static void DoFirst()
        {
            Console.WriteLine($"doing first {Task.CurrentId}");
            Task.Delay(3000).Wait();
        }

        private static void DoSecond(Task previous)
        {
            Console.WriteLine($"previous task id {previous.Id}");
            Console.WriteLine($"current task id {Task.CurrentId}");
            Console.WriteLine($"doing second task");
            Task.Delay(3000).Wait();
        }

        public static void ContinuationTasks()
        {
            Task t1 = new Task(DoFirst);
            Task t2 = t1.ContinueWith(DoSecond);
            Task t3 = t1.ContinueWith(DoSecond);
            Task t4 = t2.ContinueWith(DoSecond);
            t1.Start();
            Console.ReadLine();
        }

        public static void ParentAndChild()
        {
            var parent = new Task(ParentTask);
            parent.Start();
            Task.Delay(2000).Wait();
            Console.WriteLine(parent.Status);
            Task.Delay(4000).Wait();
            Console.WriteLine(parent.Status);
        }

        private static void ParentTask()
        {
            Console.WriteLine($"Task id: {Task.CurrentId}");
            var child = new Task(ChildTask, TaskCreationOptions.AttachedToParent);
            child.Start();
            Task.Delay(1000).Wait();
            Console.WriteLine("parent started child");
        }

        private static void ChildTask()
        {
            Console.WriteLine("Child task");
            Task.Delay(5000).Wait();
            Console.WriteLine("Child finished");
        }
    }
}
