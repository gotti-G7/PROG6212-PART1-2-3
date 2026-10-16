
using System;

namespace RaceDayApi1.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int Position { get; set; }

        public DateTime RecordedAt { get; set; }
    }
}

