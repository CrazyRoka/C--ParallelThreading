using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ThreadingIssues
{
    public class StateObject
    {
        private int _state = 5;
        private object _sync = new object();
        public void ChangeState(int loop)
        {
            //lock (_sync)
            {
                if (_state == 5)
                {
                    _state++;
                    if (_state != 6)
                    {
                        Console.WriteLine($"Race condition occured! Loop: {loop}");
                        Trace.Fail("race condition");
                    }
                }
                _state = 5;
            }
        }
    }
}
