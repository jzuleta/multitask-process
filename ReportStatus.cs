using System.Collections.Generic;

namespace multitask_process
{
    public class ReportStatus
    {
        private const long MaxParallelCreations = 5;
        private static bool Checker = true;
        public static short CountCreations;
        public static Queue<ReportGenerator> ReportsQueue = new Queue<ReportGenerator>();

        public static void startQueue()
        {
            while (Checker) checkTransfer(); 
        }

        private static void checkTransfer()
        {
            if (CountCreations < MaxParallelCreations)           
                dequeueReport();           

            Checker = ReportsQueue.Count != 0;
        }

        private static void dequeueReport()
        {
            lock (ReportsQueue)
            {
                ReportGenerator file = ReportsQueue.Dequeue();
                file.IsPause = false;
                CountCreations++;
            }
        }
    }
}