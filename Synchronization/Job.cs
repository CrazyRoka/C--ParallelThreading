using System;
using System.Collections.Generic;
using System.Text;

namespace Synchronization
{
    public class Job
    {
        public static int SharedState { get; private set; }

        public static void DoJob()
        {
            for (int i = 0; i < 50_000; i++)
            {
                SharedState++;
            }
        }

    }
}
