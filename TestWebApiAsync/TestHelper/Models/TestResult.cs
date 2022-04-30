using System;
using System.Collections.Generic;

namespace TestUtility.Models
{
    public class TestResult: DateTimeRecord
    {
        public List<ServiceResult> ServiceResults { get; set; }

        public TestResult(){}

        public TestResult(DateTime startDateTime, List<ServiceResult> serviceResults) : base(startDateTime)
        {
            ServiceResults = serviceResults;
        }
    }
}
