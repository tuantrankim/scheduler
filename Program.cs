using System;
using System.Threading;

class timer_example
{
    //set runImmediate to true to run right away
    //set scheduleTime and runPeriod (seconds). 
    //If scheduleTime is in the future the schedule will start at the schedule time
    //Otherwise, shedule time will start at next cycle calculating from scheduletime and period
    static void Main()
    {
        bool runImmediate = true;
        DateTime scheduledTime = DateTime.Parse("06/18/2020 11:14:05");
        Console.WriteLine("{0:MM/d/yyyy h:mm:ss.fff} Start Date.\n", scheduledTime);
        var statusChecker = new StatusChecker(10);
        if (runImmediate)
        {
            statusChecker.CheckStatus(null);
        }


        int runPeriod = 10;//seconds
        int period = runPeriod * 1000;//milliseconds
        DateTime nowTime = DateTime.Now;
        int delay = (int)(((scheduledTime - nowTime).TotalMilliseconds) % period + period)%period;
        Console.WriteLine("delay {0} ms", delay);
        //while (nowTime > scheduledTime)
        //{
        //    //scheduledTime = scheduledTime.AddDays(1);
        //    scheduledTime = scheduledTime.AddSeconds(runPeriod);
        //}
        //Console.WriteLine("{0:MM/d/yyyy h:mm:ss.fff} Start Date.\n", scheduledTime);
        //int tickTime = (int)(scheduledTime - DateTime.Now).TotalMilliseconds;
        //Console.WriteLine("tickTime {0} ms", tickTime);


        var stateTimer = new Timer(statusChecker.CheckStatus,
                                   null, delay, period);
        Console.ReadKey();
    }
    static void Main1()
    {
        // Create an AutoResetEvent to signal the timeout threshold in the
        // timer callback has been reached.
        var autoEvent = new AutoResetEvent(false);

        var statusChecker = new StatusChecker(10);

        // Create a timer that invokes CheckStatus after one second, 
        // and every 1/4 second thereafter.
        Console.WriteLine("{0:h:mm:ss.fff} Creating timer.\n",
                          DateTime.Now);
        var stateTimer = new Timer(statusChecker.CheckStatus,
                                   autoEvent, 1000, 250);

        // When autoEvent signals, change the period to every half second.
        autoEvent.WaitOne();
        stateTimer.Change(0, 500);
        Console.WriteLine("\nChanging period to .5 seconds.\n");

        // When autoEvent signals the second time, dispose of the timer.
        autoEvent.WaitOne();
        stateTimer.Dispose();
        Console.WriteLine("\nDestroying timer.");
    }
}

class StatusChecker
{
    private int invokeCount;
    private int maxCount;

    public StatusChecker(int count)
    {
        invokeCount = 0;
        maxCount = count;
    }

    // This method is called by the timer delegate.
    public void CheckStatus(Object stateInfo)
    {
        AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
        Console.WriteLine("{0} Checking status {1,2}.",
            DateTime.Now.ToString("h:mm:ss.fff"),
            (++invokeCount).ToString());

        if (invokeCount == maxCount)
        {
            // Reset the counter and signal the waiting thread.
            invokeCount = 0;
            if(autoEvent != null) autoEvent.Set();
        }
    }
}
// The example displays output like the following:
//       11:59:54.202 Creating timer.
//       
//       11:59:55.217 Checking status  1.
//       11:59:55.466 Checking status  2.
//       11:59:55.716 Checking status  3.
//       11:59:55.968 Checking status  4.
//       11:59:56.218 Checking status  5.
//       11:59:56.470 Checking status  6.
//       11:59:56.722 Checking status  7.
//       11:59:56.972 Checking status  8.
//       11:59:57.223 Checking status  9.
//       11:59:57.473 Checking status 10.
//       
//       Changing period to .5 seconds.
//       
//       11:59:57.474 Checking status  1.
//       11:59:57.976 Checking status  2.
//       11:59:58.476 Checking status  3.
//       11:59:58.977 Checking status  4.
//       11:59:59.477 Checking status  5.
//       11:59:59.977 Checking status  6.
//       12:00:00.478 Checking status  7.
//       12:00:00.980 Checking status  8.
//       12:00:01.481 Checking status  9.
//       12:00:01.981 Checking status 10.
//       
//       Destroying timer.