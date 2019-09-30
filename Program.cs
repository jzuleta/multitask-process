using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace multitask_process
{
    public class Program
    {
        private static List<ReportGenerator> ReportList = new List<ReportGenerator>();
        private static BackgroundWorker ThreadChecker = new BackgroundWorker();

        static void Main(string[] args)
        {
            CreateReports();             
        } 

        static string[] ReportsArray()
        {
            var reportCount = 5;
            var reportArray = new string[reportCount];

            for (var x = 0; x < reportCount; x++)
                reportArray[x] = "Report_" + (x + 1);

            return reportArray;
        }

        static void CreateReports()
        {
            var reportArray = ReportsArray();
            var controlSize = 0;
            foreach (var report in reportArray)
            {
                var reportGenerator = 
                    new ReportGenerator(report, 
                    new Random().Next(100 + controlSize, 500 + controlSize));

                reportGenerator.startReportCreation();

                ReportList.Add(reportGenerator);
                controlSize +=50;
            }

            ConfigThreadChecker();

            Console.ReadKey();
        }

        static void ConfigThreadChecker()
        {
            ThreadChecker.DoWork += CheckReportCreation;
            ThreadChecker.RunWorkerCompleted += CreationCompleted;

            ThreadChecker.RunWorkerAsync();
        }

        static void CheckReportCreation(object sender, DoWorkEventArgs e)
        {
            if (ReportList.Any())
            {
                ReportStatus.startQueue();
                while (ReportList.Any(x => x.ThreadStat)) { }
            }            
        }

        static void CreationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Completed!");
        }    
        
        public static void UpdateConsoleMessage()
        {
            var progressMessage = string.Empty;

            lock (ReportList)
            {
                Console.Clear();                
                var customProgressMessage = string.Empty;
                foreach (var reportProgress in ReportList)
                {                    
                    var customProgress = Math.Truncate(reportProgress.Percent / 10);
                    var customProgresBar = "";

                    for(var status = 0; status < 10; status++)                    
                        customProgresBar += status < customProgress ? "||" : "  ";                    

                    customProgressMessage = string.Format("{0} ({1}/{2}) {3} {4}%",
                        reportProgress.ReportName,
                        reportProgress.ReportSize.ToString("000"),
                        reportProgress.Progress.ToString("000"),
                        customProgresBar,
                        reportProgress.Percent.ToString("000.00"));
                    Console.WriteLine(customProgressMessage);    
                }                
            }
        }
    }
}
