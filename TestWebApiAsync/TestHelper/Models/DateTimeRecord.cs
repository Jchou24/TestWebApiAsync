using System;
using System.Threading;

namespace TestUtility.Models
{
    public class DateTimeRecord
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public string StartTime { get => StartDateTime.ToString("HH:mm:ss"); }

        public string EndTime { get => EndDateTime.ToString("HH:mm:ss"); }
        public double RunningTime { get => Math.Round((EndDateTime - StartDateTime).TotalSeconds, 2);  }

        public int ThreadId { get; set; }

        public DateTimeRecord(){}

        public DateTimeRecord(DateTime startDateTime)
        {
            StartDateTime = startDateTime;
            EndDateTime = DateTime.Now;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}
