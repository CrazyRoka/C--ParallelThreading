using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadingIssues
{
    public class SampleTask
    {
        public static void RaceCondition(StateObject stateObject)
        {
            int loop = 0;
            while (true)
            {
                //lock (stateObject)
                {
                    stateObject.ChangeState(++loop);
                }
            }
        }
    }
}
