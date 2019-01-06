using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueTaskLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().Wait();   
        }

        static async Task AsyncMain()
        {
            for (int i = 0; i < 20; i++)
            {
                IEnumerable<string> data = await GetSomeDataAsync();
                await Task.Delay(1000);
            }
        }

        public static Task<(IEnumerable<string> data, DateTime receivedTime)> GetRealData() =>
            Task.FromResult((Enumerable.Range(0, 10).Select(x => $"item {x}").AsEnumerable(), DateTime.Now));

        private static DateTime s_retrieved;
        private static IEnumerable<string> s_cachedData;
        public static async ValueTask<IEnumerable<string>> GetSomeDataAsync()
        {
            if(s_retrieved >= DateTime.Now.AddSeconds(-5))
            {
                Console.WriteLine("data from cache");
                return await new ValueTask<IEnumerable<string>>(s_cachedData);
            }
            Console.WriteLine("data from server");
            (s_cachedData, s_retrieved) = await GetRealData();
            return s_cachedData;
        }
    }
}
