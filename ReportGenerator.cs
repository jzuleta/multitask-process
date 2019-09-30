using System;
using System.ComponentModel;
using System.Threading;

namespace multitask_process
{
    public class ReportGenerator
    {
        private BackgroundWorker ReportThread;
        public bool IsPause { get; set; }
        public bool ThreadStat { get; private set; }
        public string ReportName { get; set; }
        public int ReportSize { get; set; }
        public int Progress { get; set; }
        public double Percent { get; set; }

        public ReportGenerator(string reportName, int reportSize)
        {

            IsPause = true;
            ThreadStat = true;
            ReportName = reportName;
            ReportSize = reportSize;
            lock (ReportStatus.ReportsQueue)
            {
                ReportStatus.ReportsQueue.Enqueue(this);
            }

            configThread();
        }

        public void startReportCreation()
        {
            ReportThread.RunWorkerAsync();
        }

        private void configThread()
        {
            ReportThread = new BackgroundWorker();
            ReportThread.DoWork += RunWorker;
            ReportThread.RunWorkerCompleted += RunWorkerCompleted;
            ReportThread.ProgressChanged += RunWorkerProgressChanged;

            ReportThread.WorkerReportsProgress = true;
            ReportThread.WorkerSupportsCancellation = true;
        }

        private void RunWorker(object sender, DoWorkEventArgs e)
        {
            while (IsPause) { 
                Thread.Sleep(1000);
            }
            
            Random random = new Random();

            while (Progress < ReportSize)
            {                
                Progress += random.Next(1, 50);                                
                Percent = Math.Round((double)Progress / ReportSize * 100, 2);                  

                if(Percent > 100)
                {
                    Progress = ReportSize;
                    Percent = 100;
                }

                ReportThread.ReportProgress((int)Math.Truncate(Percent), string.Format("{0}%", Percent.ToString()));

                Thread.Sleep(1000);
            }
           
            ReportStatus.CountCreations--;
        }
        private void RunWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Program.UpdateConsoleMessage();
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Percent = 100;            
            ThreadStat = false;

            Program.UpdateConsoleMessage();
        }
    }
}